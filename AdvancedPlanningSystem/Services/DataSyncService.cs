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
        // [New] StepTime Cache from MES API
        private List<StepTimeResponse> _stepTimeCache = new List<StepTimeResponse>();
        
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
            // Execute system recovery logic
            RestoreSystemState();

            if (AppConfig.ManualMode)
            {
                LogAndUI("System in [Manual Mode], automatic timer disabled.");
                LogHelper.Logger.Info("DataSyncService started in Manual Mode.");
            }
            else
            {
                _timer = new Timer(async state => await SyncRoutine(), null, 0, intervalMs);
                LogAndUI($"DataSyncService Started ({intervalMs / 1000}s interval)");
                LogHelper.Logger.Info("DataSyncService Started (Auto).");
            }
        }

        public async Task TriggerManualSyncAsync()
        {
            if (!AppConfig.ManualMode) return;
            LogAndUI(">>> Manual sync and decision triggered...");
            await SyncRoutine();
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
            LogAndUI("DataSyncService Stopped");
            LogHelper.Logger.Info("DataSyncService Stopped.");
        }

        /// <summary>
        /// Provides cached WIP data to DispatchService
        /// </summary>
        public Dictionary<string, WipInfoResponse> GetCachedWip()
        {
            lock (_cacheLock)
            {
                return new Dictionary<string, WipInfoResponse>(_cachedWip);
            }
        }

        /// <summary>
        /// Provides cached Equipment status to DispatchService
        /// </summary>
        public Dictionary<string, EqStatusResponse> GetCachedEqStatus()
        {
            lock (_cacheLock)
            {
                return new Dictionary<string, EqStatusResponse>(_cachedEqpStatus);
            }
        }

        private bool IsDataFresh()
        {
            if (_lastMesSyncTime == DateTime.MinValue) return false;
            double elapsedMin = (DateTime.Now - _lastMesSyncTime).TotalMinutes;
            
            if (elapsedMin > 15.0)
            {
                LogHelper.Logger.Error($"MES Data Stale! Last Update: {_lastMesSyncTime} ({elapsedMin:F1} min ago)");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Recovery Logic (Recovery Plan v2)
        /// </summary>
        private void RestoreSystemState()
        {
            LogAndUI("Performing system state check and recovery...");
            try
            {
                var allBindings = _repo.GetAllBindings();
                var portStatusDict = _repo.GetActivePorts().ToDictionary(p => p.PortId, p => p.Status);

                foreach (var b in allBindings)
                {
                    if (!string.IsNullOrEmpty(b.DispatchTime))
                    {
                        bool isOccupied = portStatusDict.ContainsKey(b.PortId);
                        if (!isOccupied)
                        {
                            LogAndUI($"[Recovery] {b.CarrierId} picked up, moving to Transit.");
                            var transit = new StateTransit
                            {
                                CarrierId = b.CarrierId, LotId = b.LotId,
                                TargetEqpId = b.TargetEqpId, NextStepId = b.NextStepId,
                                DispatchTime = b.DispatchTime,
                                PickupTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                                ExpectedArrivalTime = DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"), 
                                IsOverdue = 0
                            };
                            _repo.MoveToTransit(transit);
                        }
                        else
                        {
                            LogAndUI($"[Recovery] {b.CarrierId} still on shelf, resetting to WAIT.");
                            b.DispatchTime = null; 
                            _repo.InsertBinding(b);
                        }
                    }
                }
                LogAndUI("System state recovery completed.");
            }
            catch (Exception ex)
            {
                LogAndUI($"[Recovery Error] {ex.Message}");
                LogHelper.Logger.Error($"Recovery Error: {ex.Message}", ex);
            }
        }
        
        public void ReloadConfig() { }

        /// <summary>
        /// Core Sync Routine
        /// </summary>
        private async Task SyncRoutine()
        {
            if (_isSyncing) return;
            _isSyncing = true;

            try
            {
                // Attempt to update cache
                await UpdateMesCacheAsync();

                // Check data freshness (15-minute limit)
                if (!IsDataFresh())
                {
                    LogAndUI("[CRITICAL] Data Stale (>15m). Dispatching Suspended.");
                    string msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MES data has not been updated for over 15 minutes. Dispatching is suspended for safety.";
                    NotificationForm.ShowAsync("Critical: Data Stale", msg, NotificationLevel.Critical, 0);
                    return; 
                }
                
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
                    LogHelper.Score.Error("[QTime Sync] Failed to fetch limits: " + ex.Message);
                }

                try
                {
                    var stepTimes = await _mesService.GetAllStepTimesAsync();
                    if (stepTimes != null)
                    {
                        lock (_cacheLock) { _stepTimeCache = stepTimes; }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Score.Error("[StepTime Sync] Failed to fetch times: " + ex.Message);
                }

                var activePorts = _repo.GetActivePorts(); 
                var transits = _repo.GetAllTransits();    
                
                var portWorkNos = activePorts.Where(p => !string.IsNullOrEmpty(p.LotId)).Select(p => p.LotId).ToList();
                var transitWorkNos = transits.Where(t => !string.IsNullOrEmpty(t.LotId)).Select(t => t.LotId).ToList();
                
                var allWorkNos = portWorkNos.Concat(transitWorkNos).Distinct().ToList();

                if (allWorkNos.Count == 0) 
                {
                    await _dispatchService.ExecuteDispatchAsync();
                    return;
                }

                var orderInfos = await _mesService.GetOrderInfoBatchAsync(allWorkNos);
                var orderDict = orderInfos.ToDictionary(o => o.WorkNo, o => o);

                foreach (var port in activePorts)
                {
                    if (string.IsNullOrEmpty(port.LotId) || !orderDict.ContainsKey(port.LotId)) continue;
                    var orderInfo = orderDict[port.LotId];
                    ProcessBindingSyncAndScore(port, orderInfo);
                }

                foreach (var transit in transits)
                {
                    if (!orderDict.ContainsKey(transit.LotId)) continue;
                    var orderInfo = orderDict[transit.LotId];

                    if (orderInfo.step_id == transit.NextStepId)
                    {
                        LogAndUI($"[Transit] {transit.CarrierId} arrived at {orderInfo.step_id}.");
                        _cloudRepo.InsertGenericLog(transit.CarrierId, transit.LotId, $"Arrival Completed: {orderInfo.step_id}");
                        _repo.RemoveTransit(transit.CarrierId);
                        continue;
                    }

                    var expTime = ParseDbTime(transit.ExpectedArrivalTime);
                    if (expTime.HasValue)
                    {
                        if (DateTime.Now > expTime.Value)
                        {
                            LogAndUI($"[Transit] {transit.CarrierId} overdue! Logging to CloudDB.");
                            _cloudRepo.InsertGenericLog(transit.CarrierId, transit.LotId, $"Transport Overdue: Exp={transit.ExpectedArrivalTime}");
                            _repo.RemoveTransit(transit.CarrierId);
                            continue;
                        }
                    }
                }

                LogAndUI(">>> Dispatching Decision Loop Started...");
                await _dispatchService.ExecuteDispatchAsync();
                LogAndUI("<<< Dispatching Decision Loop Finished.");
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

        public async Task ForceUpdateCacheAsync()
        {
            LogHelper.Logger.Info("Forced MES cache update requested.");
            await UpdateMesCacheAsync();
        }

        private async Task UpdateMesCacheAsync()
        {
            try
            {
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
                string msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Failed to connect to MES Server. Using cached data if available.\nError: " + ex.Message;
                NotificationForm.ShowAsync("MES Sync Warning", msg, NotificationLevel.Warning, 0);
            }
        }

        private DateTime? ParseDbTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr)) return null;
            DateTime dt;
            if (DateTime.TryParseExact(timeStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt)) return dt;
            if (DateTime.TryParse(timeStr, out dt)) return dt;
            return null;
        }

        /// <summary>
        /// 單一卡匣的資料同步與評分邏輯 (Step 1~3)
        /// 三段式 QTime 判定實作 (Green/Red/Dead Zone)
        /// </summary>
        private void ProcessBindingSyncAndScore(StatePort port, OrderInfoResponse orderInfo)
        {
            LogHelper.Score.Debug($"[Scoring Start] Carrier: {port.CarrierId}, Lot: {port.LotId}, Step: {orderInfo.step_id} -> {orderInfo.next_step_id}");

            // 1. 取得時間基礎資料
            DateTime? prevOut = ParseDbTime(orderInfo.prev_out_time);
            DateTime? dueDate = ParseDbTime(orderInfo.due_date);
            string qTimeDeadline = ""; 
            
            double tReal = 999999; 
            double tSafe = 999999;

            if (prevOut.HasValue)
            {
                // A. 取得 QTime Limit (T_Limit)
                int limit = 120; 
                QTimeLimitResponse matchedQTime = null;
                lock (_cacheLock) { matchedQTime = _qTimeCache.FirstOrDefault(q => q.step_id == orderInfo.step_id && q.next_step_id == orderInfo.next_step_id); }
                if (matchedQTime != null) limit = matchedQTime.qtime_limit_min;

                // B. 取得下一站標準工時 (T_Std_Process)
                double stdProcessMin = 5.0; 
                StepTimeResponse matchedStepTime = null;
                lock (_cacheLock) { matchedStepTime = _stepTimeCache.FirstOrDefault(s => s.step_id == orderInfo.next_step_id); }
                if (matchedStepTime != null) stdProcessMin = matchedStepTime.std_time_sec / 60.0;

                // C. 取得搬運緩衝 (T_Buffer)
                double bufferMin = AppConfig.TransportBufferMin;

                // D. 計算已流逝時間
                double elapsedMin = (DateTime.Now - prevOut.Value).TotalMinutes;

                // E. 計算 T_Real 與 T_Safe
                tReal = limit - elapsedMin - stdProcessMin;
                tSafe = tReal - bufferMin;
                
                qTimeDeadline = prevOut.Value.AddMinutes(limit).ToString("yyyyMMddHHmmss");
                
                LogHelper.Score.Debug($"  - QTime Formula: Limit:{limit} - Elapsed:{elapsedMin:F1} - Std:{stdProcessMin:F1} = T_Real:{tReal:F1}m (T_Safe:{tSafe:F1}m)");
            }

            // 2. 例外攔截 (Step 2)
            // 只有真實剩餘時間 T_Real <= 0 才是真正死亡
            int isHold = (tReal <= 0) ? 1 : 0;
            if (isHold == 1) LogHelper.Score.Warn($"  - DEAD ZONE: T_Real ({tReal:F1}) <= 0, Item is HOLD.");

            // 3. 加權評分 (Step 3: Scoring)
            double score = 0;
            double s_qtime = 0, s_urgent = 0, s_eng = 0, s_due = 0, s_lead = 0;

            if (isHold == 0) 
            {
                if (tSafe > 0 && tSafe < 99999) 
                {
                    // 階段 A: 安全期 (Green Zone)
                    s_qtime = 1000000.0 / Math.Max(0.1, tSafe);
                    LogHelper.Score.Debug($"  - QTime Stage A (Green): {s_qtime:F2}");
                }
                else if (tSafe <= 0 && tReal > 0)
                {
                    // 階段 B: 緩衝期/緊急期 (Red Zone)
                    // 給予 900 萬起跳的無限大分數
                    s_qtime = 9000000.0 + (1000000.0 / Math.Max(0.1, tReal));
                    LogHelper.Score.Debug($"  - QTime Stage B (Red): {s_qtime:F2} (T_Real: {tReal:F1})");
                }
                score += s_qtime;
                
                if (orderInfo.priority_type == 2) 
                {
                    s_urgent = 100000.0;
                    score += s_urgent;
                }
                
                if (orderInfo.priority_type == 1) 
                {
                    s_eng = 50000.0;
                    score += s_eng;
                }
                
                if (dueDate.HasValue) 
                {
                    double hoursLeft = (dueDate.Value - DateTime.Now).TotalHours;
                    s_due = Math.Max(0, (AppConfig.DueBaseHours - hoursLeft) * 100.0 + 10000.0);
                    score += s_due;
                }
                
                if (prevOut.HasValue) 
                {
                    double leadMin = (DateTime.Now - prevOut.Value).TotalMinutes;
                    s_lead = leadMin * 10.0;
                    score += s_lead;
                }
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
                
                ScoreQTime = Math.Round(s_qtime, 2),
                ScoreUrgent = Math.Round(s_urgent, 2),
                ScoreEng = Math.Round(s_eng, 2),
                ScoreDue = Math.Round(s_due, 2),
                ScoreLead = Math.Round(s_lead, 2),

                TReal = tReal, // 儲存真實剩餘時間供 UI 顯示

                PriorityType = orderInfo.priority_type,
                IsHold = isHold,
                WaitReason = existing?.WaitReason ?? "",
                BindTime = existing?.BindTime ?? DateTime.Now.ToString("yyyyMMddHHmmss"),
                DispatchTime = existing?.DispatchTime 
            };

            _repo.InsertBinding(binding);
            LogHelper.Score.Info($"[Scoring End] Carrier: {port.CarrierId}, Score: {binding.DispatchScore}");
        }

        private void LogAndUI(string msg)
        {
            if (_logBox == null) return;
            if (_uiContext != null)
            {
                _uiContext.Post(_ => {
                    string log = $"[{DateTime.Now:HH:mm:ss}] {msg}";
                    _logBox.Items.Add(log);
                    if (_logBox.Items.Count > 100) _logBox.Items.RemoveAt(0);
                    _logBox.SelectedIndex = _logBox.Items.Count - 1;
                }, null);
            }
        }
    }
}
