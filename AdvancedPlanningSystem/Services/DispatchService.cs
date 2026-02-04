using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedPlanningSystem.MES;
using AdvancedPlanningSystem.Models;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem.Services
{
    /// <summary>
    /// 派貨決策服務 (Dispatch Decision Engine)
    /// 負責執行演算法 Step 4 (分組過濾) 與 Step 5 (批次與強制派貨)。
    /// </summary>
    public class DispatchService
    {
        // 移除直接依賴 IMesService，改依賴 DataSyncService 取得快取
        private DataSyncService _dataSyncService;
        private ApsLocalDbRepository _repo;
        private ApsCloudDbRepository _cloudRepo;
        private TcpServerModule _tcpServer;
        private List<ConfigStepEqp> _stepEqpMapping;

        public DispatchService(ApsLocalDbRepository repo, ApsCloudDbRepository cloudRepo, TcpServerModule tcpServer)
        {
            _repo = repo;
            _cloudRepo = cloudRepo;
            _tcpServer = tcpServer;
        }

        // 屬性注入 (因為 DataSyncService 建構時需要 DispatchService，避免循環依賴)
        public void SetDataSyncService(DataSyncService dataSyncService)
        {
            _dataSyncService = dataSyncService;
        }

        /// <summary>
        /// 執行派貨決策流程
        /// </summary>
        public async Task ExecuteDispatchAsync()
        {
            try
            {
                // 0. 準備資料
                _stepEqpMapping = _repo.GetStepEqpMappings();
                var candidates = _repo.GetSortedWaitBindings(); // 取得依分數排序後的候選名單 (Status=WAIT)
                if (candidates.Count == 0) return;

                // 1. 從快取取得 MES 資料 (WIP & Status)
                // 若資料過期 (>5min) 這裡會拋出例外，中止派貨以確保安全
                if (_dataSyncService == null) return;
                var wipDict = _dataSyncService.GetCachedWip();
                var statusDict = _dataSyncService.GetCachedEqStatus();

                if (wipDict.Count == 0 || statusDict.Count == 0)
                {
                    LogHelper.Logger.Warn("[Dispatch] MES Cache is empty, skipping dispatch.");
                    return;
                }

                // 3. 讀取 Transit 資訊 (計算 WIP 用)
                var allTransits = _repo.GetAllTransits();
                
                // 4. 讀取 QTime 設定 (用於計算防過期閾值)
                var qTimeConfigs = _repo.GetQTimeConfigs();

                // 5. 分組處理 (Step 4: Grouping)
                var stepGroups = candidates.GroupBy(c => c.NextStepId);

                foreach (var group in stepGroups)
                {
                    string nextStep = group.Key;
                    var cassetteList = group.ToList(); 
                    
                    var availableEqps = _stepEqpMapping.Where(m => m.StepId == nextStep).Select(m => m.EqpId).ToList();

                    foreach (var eqpId in availableEqps)
                    {
                        await ProcessEqpDispatchAsync(eqpId, cassetteList, wipDict, statusDict, allTransits, qTimeConfigs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("ExecuteDispatchAsync Error", ex);
            }
        }

        /// <summary>
        /// 針對單一機台進行派貨評估 (核心演算法 Step 4 & 5)
        /// </summary>
        /// <param name="eqpId">目標機台 ID</param>
        /// <param name="potentialCassettes">該站點對應的所有候選卡匣</param>
        /// <param name="wipDict">從 MES 取得的 WIP 資料字典</param>
        /// <param name="statusDict">從 MES 取得的機台狀態字典</param>
        /// <param name="allTransits">目前運送中 (Transit) 的卡匣清單</param>
        /// <param name="qTimeConfigs">站點間的 QTime 設定檔</param>
        private async Task ProcessEqpDispatchAsync(
            string eqpId, 
            List<StateBinding> potentialCassettes,
            Dictionary<string, WipInfoResponse> wipDict,
            Dictionary<string, EqStatusResponse> statusDict,
            List<StateTransit> allTransits,
            List<ConfigQTime> qTimeConfigs)
        {
            try 
            {
                // [過濾 1] 機台可用性檢查 (Status Filter)
                // 只允許派往狀態為 RUN 或 IDLE 的機台。若機台 Down 機或 PM 則直接跳過。
                if (!statusDict.ContainsKey(eqpId)) return;
                var eqpStatus = statusDict[eqpId];
                if (eqpStatus.status != "RUN" && eqpStatus.status != "IDLE") return;

                // [過濾 2] 產能負荷檢查 (Capacity Filter)
                var eqpConfig = _repo.GetEqpConfig(eqpId);
                // 優先使用來自 MES API 的即時 max_wip_qty
                int maxWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].max_wip_qty : (eqpConfig?.MaxWipQty ?? 10);
                int batchSize = eqpConfig?.BatchSize ?? 1;

                // 計算當前總占用量 = MES 機台內與排隊量 + 系統目前正在搬運往該機台的量 (Transit)
                int mesWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].current_wip_qty : 0;
                int transitCount = allTransits.Count(t => t.TargetEqpId == eqpId);
                int currentTotalWip = mesWip + transitCount;

                // 若總占用量已達到或超過最大上限，則不派貨，避免造成下游塞車。
                if (currentTotalWip >= maxWip) return;

                // [決策準備] 從候選名單中挑出尚未被派發的卡匣
                // 必須檢查 DispatchTime 是否為空，因為在同一週期內卡匣可能已被指派給同站點的其他機台。
                var availableForThisEqp = potentialCassettes.Where(c => string.IsNullOrEmpty(c.DispatchTime)).ToList();
                if (availableForThisEqp.Count == 0) return;

                bool shouldDispatch = false; // 是否觸發派貨
                string triggerReason = "";   // 觸發原因 (用於 Log)

                // [決策 Rule A] 標準湊批 (Full Batch)
                // 當候選卡匣數量達到該機台設定的批次量時，立即觸發派貨。
                if (availableForThisEqp.Count >= batchSize)
                {
                    shouldDispatch = true;
                    triggerReason = "FullBatch";
                }
                else
                {
                    // [決策 Rule C] 強制派貨邏輯 (Forced Dispatch)
                    // 當數量未滿批次時，若滿足以下任一異常條件，則仍強制派送。
                    
                    // C-1: 防空轉 (Anti-Idle)
                    // 若機台處於 IDLE (閒置) 狀態且持續時間 (duration) 超過 force_idle_sec 限制。
                    if (eqpStatus.status == "IDLE")
                    {
                        int idleSec = 0;
                        int.TryParse(eqpStatus.duration, out idleSec);
                        int idleLimit = eqpConfig?.ForceIdleSec ?? 300; // 預設 300 秒

                        if (idleSec > idleLimit)
                        {
                            shouldDispatch = true;
                            triggerReason = $"Force_Idle({idleSec}s)";
                        }
                    }

                    // C-2: 防過期 (QTime Risk Control)
                    // 若群組內任一卡匣的 QTime 剩餘時間已逼近「搬運+作業」的死線。
                    if (!shouldDispatch)
                    {
                        foreach (var c in availableForThisEqp)
                        {
                            if (!string.IsNullOrEmpty(c.QTimeDeadline) && DateTime.TryParse(c.QTimeDeadline, out DateTime dtDead))
                            {
                                // [修正] 暫時停用 local_config_qtime，強制預設為 30
                                // 動態閾值 = 搬運時間 (30m) + 15 分鐘安全緩衝
                                int transportMin = 30;
                                int riskThresholdMin = transportMin + 15; 

                                double remainingMin = (dtDead - DateTime.Now).TotalMinutes;

                                if (remainingMin < riskThresholdMin)
                                {
                                    shouldDispatch = true;
                                    triggerReason = $"Force_QTimeRisk(Rem:{remainingMin:F1}m < {riskThresholdMin}m)";
                                    break; 
                                }
                            }
                        }
                    }
                }

                // [執行執行] 執行開門與記錄 (Execution)
                if (shouldDispatch)
                {
                    // 依照分數由高至低選取 N 個卡匣 (N = BatchSize)
                    var dispatchList = availableForThisEqp.Take(batchSize).ToList();

                    foreach (var cassette in dispatchList)
                    {
                        if (!string.IsNullOrEmpty(cassette.PortId))
                        {
                            // 1. 下達硬體指令：發送 OPEN 指令開啟貨架電子鎖
                            string cmd = $"OPEN,{cassette.PortId}";
                            await _tcpServer.SendCommand(cmd);
                            
                            // 2. 更新資料庫狀態：標記 DispatchTime 與目標機台，正式轉為 DISPATCHING 狀態
                            cassette.DispatchTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            cassette.TargetEqpId = eqpId;
                            _repo.InsertBinding(cassette); 
                            
                            // 3. 歷史紀錄：將派貨原因與決策結果寫入 CloudDB
                            _cloudRepo.InsertGenericLog(cassette.CarrierId, cassette.LotId, $"派貨執行: {triggerReason} -> {eqpId}");
                            
                            LogHelper.Logger.Info($"[Dispatch] {triggerReason} -> {cmd} (Score: {cassette.DispatchScore}) -> {eqpId}");
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                LogHelper.Logger.Error($"[Dispatch Error] {eqpId}: {ex.Message}", ex);
            }
        }
    }
}