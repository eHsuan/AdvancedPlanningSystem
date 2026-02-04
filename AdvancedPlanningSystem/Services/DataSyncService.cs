using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedPlanningSystem.MES;
using AdvancedPlanningSystem.Models;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem.Services
{
    /// <summary>
    /// 資料同步服務 (Data Sync & Scoring Engine)
    /// 負責週期性從 MES 同步資料、計算 QTime 風險、執行加權評分，以及維護 Transit 狀態。
    /// 同時作為 MES 資料的快取中心 (WIP, EqStatus)。
    /// </summary>
    public class DataSyncService
    {
        private IMesService _mesService;
        private ApsLocalDbRepository _repo;
        private ApsCloudDbRepository _cloudRepo;
        private DispatchService _dispatchService;
        private System.Windows.Forms.ListBox _logBox;
        private SynchronizationContext _uiContext;

        private Timer _timer;
        private bool _isSyncing = false;

        // --- Phase 2: MES Caching ---
        private Dictionary<string, WipInfoResponse> _cachedWip = new Dictionary<string, WipInfoResponse>();
        private Dictionary<string, EqStatusResponse> _cachedEqpStatus = new Dictionary<string, EqStatusResponse>();
        // [New] QTime Cache from MES API
        private List<QTimeLimitResponse> _qTimeCache = new List<QTimeLimitResponse>();
        
        private DateTime _lastMesSyncTime = DateTime.MinValue;
        private readonly object _cacheLock = new object();
        private const int MES_SYNC_INTERVAL_SEC = 60;
        private const int DATA_STALE_LIMIT_MIN = 5;

        public DataSyncService(IMesService mesService, ApsLocalDbRepository repo, ApsCloudDbRepository cloudRepo, DispatchService dispatchService, System.Windows.Forms.ListBox logBox)
        {
            _mesService = mesService;
            _repo = repo;
            _cloudRepo = cloudRepo;
            _dispatchService = dispatchService;
            _logBox = logBox;
            _uiContext = SynchronizationContext.Current;
        }

        public void Start(int intervalMs = 60000)
        {
            // 執行異常恢復邏輯
            RestoreSystemState();

            _timer = new Timer(async state => await SyncRoutine(), null, 0, intervalMs);
            LogAndUI($"DataSyncService 已啟動 ({intervalMs/1000}s interval)");
            LogHelper.Logger.Info("DataSyncService Started.");
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
            LogAndUI("DataSyncService 已停止");
            LogHelper.Logger.Info("DataSyncService Stopped.");
        }

        /// <summary>
        /// 提供給 DispatchService 獲取快取的 WIP 資料
        /// </summary>
        public Dictionary<string, WipInfoResponse> GetCachedWip()
        {
            lock (_cacheLock)
            {
                CheckDataFreshness();
                return new Dictionary<string, WipInfoResponse>(_cachedWip);
            }
        }

        /// <summary>
        /// 提供給 DispatchService 獲取快取的機台狀態
        /// </summary>
        public Dictionary<string, EqStatusResponse> GetCachedEqStatus()
        {
            lock (_cacheLock)
            {
                CheckDataFreshness();
                return new Dictionary<string, EqStatusResponse>(_cachedEqpStatus);
            }
        }

        private void CheckDataFreshness()
        {
            if ((DateTime.Now - _lastMesSyncTime).TotalMinutes > DATA_STALE_LIMIT_MIN)
            {
                string msg = $"MES Data Stale! Last Update: {_lastMesSyncTime}";
                LogHelper.Logger.Error(msg);
                // 這裡可以選擇是否拋出例外阻斷派貨，或者只是 Log 警告
                // 安全起見，若資料過期太久應視為不可靠
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 異常恢復邏輯 (Recovery Plan v2)
        /// 目的：解決系統崩潰重啟後，卡匣狀態卡在 DISPATCHING 的問題。
        /// </summary>
        private void RestoreSystemState()
        {
            LogAndUI("執行系統狀態檢查與恢復...");
            try
            {
                // 取得所有 Binding 資料與目前 Port 的硬體狀態
                var allBindings = _repo.GetAllBindings();
                var portStatusDict = _repo.GetActivePorts().ToDictionary(p => p.PortId, p => p.Status);

                foreach (var b in allBindings)
                {
                    // 只處理狀態為「派送中」的項目
                    if (!string.IsNullOrEmpty(b.DispatchTime))
                    {
                        bool isOccupied = portStatusDict.ContainsKey(b.PortId);
                        
                        // 情況 A: 貨已取走 (Port 表中查無此 Port 為 OCCUPIED)
                        // 判定：實際上已經 Pickup，只是系統沒收到 PICK 訊號或沒來得及處理
                        if (!isOccupied)
                        {
                            LogAndUI($"[Recovery] {b.CarrierId} 已取走，移至 Transit。");
                            var transit = new StateTransit
                            {
                                CarrierId = b.CarrierId, LotId = b.LotId,
                                TargetEqpId = b.TargetEqpId, NextStepId = b.NextStepId,
                                DispatchTime = b.DispatchTime,
                                PickupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                ExpectedArrivalTime = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss"), // 預設 10 分鐘
                                IsOverdue = 0
                            };
                            _repo.MoveToTransit(transit);
                        }
                        // 情況 B: 貨仍在架上 (Sensor ON)
                        // 判定：之前的派貨指令無效或失敗，重置狀態讓系統重新評分
                        else
                        {
                            LogAndUI($"[Recovery] {b.CarrierId} 仍在架上，重置為 WAIT。");
                            b.DispatchTime = null; // 清空時間，視為 WAIT
                            _repo.InsertBinding(b);
                        }
                    }
                }
                LogAndUI("系統狀態恢復完成。");
            }
            catch (Exception ex)
            {
                LogAndUI($"[Recovery Error] {ex.Message}");
                LogHelper.Logger.Error($"Recovery Error: {ex.Message}", ex);
            }
        }
        
        // ReloadConfig 已不再需要，因為 QTime 改為動態 API 獲取
        public void ReloadConfig() { }

        // ... (Stop, GetCachedWip, GetCachedEqStatus, CheckDataFreshness, RestoreSystemState remain the same) ...

        /// <summary>
        /// 核心同步迴圈
        /// </summary>
        private async Task SyncRoutine()
        {
            if (_isSyncing) return;
            _isSyncing = true;

            try
            {
                // --- Phase 2: MES Data Caching Logic ---
                await UpdateMesCacheAsync();
                
                // [New] 同步 QTime 設定 (API c.)
                // 直接呼叫 API 並更新記憶體快取
                try 
                {
                    var qTimes = await _mesService.GetAllQTimeLimitsAsync();
                    if (qTimes != null) 
                    {
                        lock (_cacheLock) { _qTimeCache = qTimes; }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Logger.Error("[QTime Sync] Failed to fetch limits: " + ex.Message);
                }

                // 1. 準備批次查詢名單 (Port + Transit 都要查)
                var activePorts = _repo.GetActivePorts(); // 取得架上卡匣
                var transits = _repo.GetAllTransits();    // 取得運送中卡匣
                
                var portWorkNos = activePorts.Where(p => !string.IsNullOrEmpty(p.LotId)).Select(p => p.LotId).ToList();
                var transitWorkNos = transits.Where(t => !string.IsNullOrEmpty(t.LotId)).Select(t => t.LotId).ToList();
                
                var allWorkNos = portWorkNos.Concat(transitWorkNos).Distinct().ToList();

                if (allWorkNos.Count == 0) 
                {
                    // 即使沒有工單要查，仍需觸發 Dispatcher 檢查機台是否 IDLE (防空轉)
                    // 注意：Dispatcher 內部現在應使用 GetCachedWip/GetCachedEqStatus
                    await _dispatchService.ExecuteDispatchAsync();
                    return;
                }

                // 2. 批次呼叫 MES API 取得最新工單資訊
                var orderInfos = await _mesService.GetOrderInfoBatchAsync(allWorkNos);
                var orderDict = orderInfos.ToDictionary(o => o.WorkNo, o => o);

                // 3. 處理架上卡匣 (Sync & Score)
                foreach (var port in activePorts)
                {
                    if (string.IsNullOrEmpty(port.LotId) || !orderDict.ContainsKey(port.LotId)) continue;
                    var orderInfo = orderDict[port.LotId];
                    // 執行單一卡匣的同步與評分
                    ProcessBindingSyncAndScore(port, orderInfo);
                }

                // 4. 處理運送中卡匣 (Transit Check)
                foreach (var transit in transits)
                {
                    if (!orderDict.ContainsKey(transit.LotId)) continue;
                    var orderInfo = orderDict[transit.LotId];

                    // A. 到站檢查：若 MES 回傳的 StepId 已經等於我們記錄的 NextStepId，代表已過帳
                    if (orderInfo.step_id == transit.NextStepId)
                    {
                        LogAndUI($"[Transit] {transit.CarrierId} 已到站 ({orderInfo.step_id})。記錄至 CloudDB。");
                        _cloudRepo.InsertGenericLog(transit.CarrierId, transit.LotId, $"到站完成: {orderInfo.step_id}");
                        _repo.RemoveTransit(transit.CarrierId);
                        continue;
                    }

                    // B. 超時檢查：若當前時間 > 預計到達時間
                    if (DateTime.TryParse(transit.ExpectedArrivalTime, out DateTime expTime))
                    {
                        if (DateTime.Now > expTime)
                        {
                            LogAndUI($"[Transit] {transit.CarrierId} 超時未達! 記錄至 CloudDB。");
                            _cloudRepo.InsertGenericLog(transit.CarrierId, transit.LotId, $"搬運超時: Exp={transit.ExpectedArrivalTime}");
                            _repo.RemoveTransit(transit.CarrierId);
                            continue;
                        }
                    }
                }

                // 5. 觸發派貨決策服務 (Step 4 & 5)
                // DispatchService 將會呼叫 GetCachedWip() 存取剛更新的資料
                await _dispatchService.ExecuteDispatchAsync();
            }
            catch (Exception ex)
            {
                LogAndUI($"[Sync Error] {ex.Message}");
                LogHelper.Logger.Error("SyncRoutine Error", ex);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private async Task UpdateMesCacheAsync()
        {
            try
            {
                // 取得所有相關機台 ID (從 StepEqpMapping 取得，這裡假設可從 Repo 獲取或 DispatchService 知道)
                // 為了簡化，我們先讀取 StepEqpMapping。實務上可快取此清單。
                var mapping = _repo.GetStepEqpMappings();
                var eqpIds = mapping.Select(m => m.EqpId).Distinct().ToList();

                if (eqpIds.Any())
                {
                    var wips = await _mesService.GetWipBatchAsync(eqpIds);
                    var statuses = await _mesService.GetEquipmentStatusBatchAsync(eqpIds);

                    lock (_cacheLock)
                    {
                        _cachedWip = wips.ToDictionary(w => w.eq_id, w => w);
                        _cachedEqpStatus = statuses.ToDictionary(s => s.eqp_id, s => s);
                        _lastMesSyncTime = DateTime.Now;
                    }
                    
                    // 同步 MES 的 MaxWip 到本地資料庫
                    foreach (var wip in wips)
                    {
                        _repo.UpdateEqpMaxWip(wip.eq_id, wip.max_wip_qty);
                    }
                    
                    LogHelper.Logger.Info($"MES Cache Updated. Wips: {wips.Count}, Statuses: {statuses.Count}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("Failed to update MES Cache", ex);
                // 不拋出例外，以免中斷主要的 SyncRoutine，但若連續失敗會觸發 Stale Check
            }
        }

        /// <summary>
        /// 單一卡匣的資料同步與評分邏輯 (Step 1~3)
        /// </summary>
        private void ProcessBindingSyncAndScore(StatePort port, OrderInfoResponse orderInfo)
        {
            // 1. QTime 計算
            DateTime? prevOut = null;
            DateTime? dueDate = null;
            string qTimeDeadline = ""; 
            double tSafe = 999999; 

            if (!string.IsNullOrEmpty(orderInfo.prev_out_time) && DateTime.TryParse(orderInfo.prev_out_time, out DateTime dtPrev))
            {
                prevOut = dtPrev;
                
                // [修正] 從 MES API 快取中取得 QTime Limit
                int limit = 120; // Default fallback
                
                QTimeLimitResponse matchedQTime = null;
                lock (_cacheLock)
                {
                    matchedQTime = _qTimeCache.FirstOrDefault(q => q.step_id == orderInfo.step_id && q.next_step_id == orderInfo.next_step_id);
                }

                if (matchedQTime != null)
                {
                    limit = matchedQTime.qtime_limit_min;
                }

                double elapsedMin = (DateTime.Now - dtPrev).TotalMinutes;
                tSafe = limit - elapsedMin;
                qTimeDeadline = dtPrev.AddMinutes(limit).ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!string.IsNullOrEmpty(orderInfo.due_date) && DateTime.TryParse(orderInfo.due_date, out DateTime dtDue))
            {
                dueDate = dtDue;
            }

            // 2. 例外攔截 (Step 2)
            int isHold = (tSafe < 0) ? 1 : 0;

            // 3. 加權評分 (Step 3: Scoring)
            double score = 0;
            if (isHold == 0) 
            {
                if (tSafe < 99999) score += 1000000.0 / Math.Max(0.1, tSafe);
                if (orderInfo.priority_type == 2) score += 100000.0;
                if (orderInfo.priority_type == 1) score += 50000.0;
                if (dueDate.HasValue) score += (240.0 - (dueDate.Value - DateTime.Now).TotalHours) * 100.0 + 10000.0;
                if (prevOut.HasValue) score += (DateTime.Now - prevOut.Value).TotalMinutes * 10.0;
            }

            var existing = _repo.GetBinding(port.CarrierId);
            
            var binding = new StateBinding
            {
                CarrierId = port.CarrierId,
                PortId = port.PortId,
                LotId = port.LotId,
                CurrentStepId = orderInfo.step_id,
                NextStepId = orderInfo.next_step_id,
                TargetEqpId = existing?.TargetEqpId ?? "",
                QTimeDeadline = qTimeDeadline,
                DispatchScore = Math.Round(score, 2),
                PriorityType = orderInfo.priority_type,
                IsHold = isHold,
                BindTime = existing?.BindTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DispatchTime = existing?.DispatchTime 
            };

            _repo.InsertBinding(binding);
        }

        private void LogAndUI(string msg)
        {
            if (_logBox == null) return;
            if (_uiContext != null)
            {
                _uiContext.Post(_ => {
                    string log = $"[{DateTime.Now:HH:mm:ss}] {msg}";
                    _logBox.Items.Insert(0, log);
                    if (_logBox.Items.Count > 100) _logBox.Items.RemoveAt(100);
                }, null);
            }
        }
    }
}