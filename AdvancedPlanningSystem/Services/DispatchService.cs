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
                var allCandidates = _repo.GetSortedWaitBindings(); 
                
                // 嚴格攔截：過濾掉被標記為 HOLD (如 QTime 逾期) 的卡匣
                var candidates = allCandidates.Where(c => c.IsHold == 0).ToList();
                var holdItems = allCandidates.Where(c => c.IsHold == 1).ToList();
                
                if (holdItems.Any())
                {
                    LogHelper.Dispatch.Warn($"[Hold Alert] {holdItems.Count} carriers are blocked due to HOLD status.");
                    // 立即更新這些項目的等待原因
                    foreach (var h in holdItems)
                    {
                        if (h.WaitReason != "HOLD (Exception)")
                        {
                            h.WaitReason = "HOLD (Exception)";
                            _repo.InsertBinding(h);
                        }
                    }
                }

                // 即使 candidates 為空，也繼續執行（為了讓 loop 跑完並更新其他狀態）
                LogHelper.Dispatch.Debug($"[Dispatch Loop Start] Candidates (Non-Hold): {candidates.Count}");

                // 取得目前「已派送但尚未被人員取走」的卡匣清單，用於計算 Committed WIP
                var allBindings = _repo.GetAllBindings();
                var dispatchedButNotPicked = allBindings
                    .Where(b => !string.IsNullOrEmpty(b.DispatchTime))
                    .GroupBy(b => b.TargetEqpId)
                    .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());

                // 1. 從快取取得 MES 資料 (WIP & Status)
                if (_dataSyncService == null) return;
                var wipDict = _dataSyncService.GetCachedWip();
                var statusDict = _dataSyncService.GetCachedEqStatus();

                if (wipDict.Count == 0 || statusDict.Count == 0)
                {
                    LogHelper.Dispatch.Warn("[Dispatch] MES Cache is empty, skipping dispatch.");
                    return;
                }

                var allTransits = _repo.GetAllTransits();
                var qTimeConfigs = _repo.GetQTimeConfigs();

                // 5. 分組處理
                var stepGroups = candidates.GroupBy(c => c.NextStepId);

                foreach (var group in stepGroups)
                {
                    string nextStep = group.Key;
                    var cassetteList = group.ToList(); 
                    
                    LogHelper.Dispatch.Debug($"  - Analyzing Group: NextStep={nextStep}, Count={cassetteList.Count}");

                    if (nextStep == "END")
                    {
                        await ProcessFinishDispatchAsync(cassetteList);
                        continue;
                    }

                    var availableEqps = _stepEqpMapping.Where(m => m.StepId == nextStep).Select(m => m.EqpId).ToList();
                    LogHelper.Dispatch.Debug($"    - Defined Route Eqps: {string.Join(", ", availableEqps)}");

                    // --- [Wait Analysis] ---
                    int totalEqp = availableEqps.Count;
                    int downCount = 0;
                    int fullCount = 0;

                    foreach (var eqpId in availableEqps)
                    {
                        bool isDown = false;
                        bool isFull = false;

                        if (statusDict.ContainsKey(eqpId))
                        {
                            var s = statusDict[eqpId];
                            if (s.status != "RUN" && s.status != "IDLE") isDown = true;
                        }
                        else { isDown = true; } 

                        if (!isDown)
                        {
                            var eqpConfig = _repo.GetEqpConfig(eqpId);
                            int maxWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].max_wip_qty : (eqpConfig?.MaxWipQty ?? 10);
                            int mesWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].current_wip_qty : 0;
                            int transitCount = allTransits.Count(t => t.TargetEqpId == eqpId);
                            int dispatchedCount = dispatchedButNotPicked.ContainsKey(eqpId) ? dispatchedButNotPicked[eqpId] : 0;
                            
                            if ((mesWip + transitCount + dispatchedCount) >= maxWip) isFull = true;
                        }

                        if (isDown) downCount++;
                        else if (isFull) fullCount++;
                    }

                    // --- [Dispatch Execution] ---
                    foreach (var eqpId in availableEqps)
                    {
                        int dispatchedCount = dispatchedButNotPicked.ContainsKey(eqpId) ? dispatchedButNotPicked[eqpId] : 0;
                        await ProcessEqpDispatchAsync(eqpId, cassetteList, wipDict, statusDict, allTransits, qTimeConfigs, dispatchedCount);
                    }

                    // --- [Wait Reason Update] ---
                    string reason = "Queueing (Batch/Score)";
                    if (totalEqp == 0) reason = "No Route Defined";
                    else if (downCount == totalEqp) reason = "All Eqp DOWN";
                    else if (downCount + fullCount == totalEqp) reason = "All Eqp FULL";

                    foreach (var c in cassetteList)
                    {
                        if (string.IsNullOrEmpty(c.DispatchTime))
                        {
                            if (c.WaitReason != reason)
                            {
                                c.WaitReason = reason;
                                _repo.InsertBinding(c); 
                                LogHelper.Dispatch.Debug($"    - Updated WaitReason for {c.CarrierId}: {reason}");
                            }
                        }
                    }
                }
                LogHelper.Dispatch.Debug("[Dispatch Loop End]");
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error("ExecuteDispatchAsync Error", ex);
            }
        }

        private async Task ProcessFinishDispatchAsync(List<StateBinding> finishList)
        {
            try
            {
                var available = finishList.Where(c => string.IsNullOrEmpty(c.DispatchTime)).ToList();
                if (available.Count == 0) return;

                foreach (var cassette in available)
                {
                    if (!string.IsNullOrEmpty(cassette.PortId))
                    {
                        string cmd = $"OPEN,{cassette.PortId},STOCK";
                        await _tcpServer.SendCommand(cmd);

                        cassette.DispatchTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                        cassette.TargetEqpId = "STOCK"; 
                        _repo.InsertBinding(cassette);

                        _cloudRepo.InsertGenericLog(cassette.CarrierId, cassette.LotId, "完工出庫 -> STOCK");
                        LogHelper.Dispatch.Info($"[Dispatch Finish] {cassette.CarrierId} -> STOCK");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[Dispatch Finish Error] {ex.Message}", ex);
            }
        }

        private async Task ProcessEqpDispatchAsync(
            string eqpId, 
            List<StateBinding> potentialCassettes,
            Dictionary<string, WipInfoResponse> wipDict,
            Dictionary<string, EqStatusResponse> statusDict,
            List<StateTransit> allTransits,
            List<ConfigQTime> qTimeConfigs,
            int dispatchedCount)
        {
            try 
            {
                LogHelper.Dispatch.Debug($"      > Evaluating Eqp: {eqpId}");

                if (!statusDict.ContainsKey(eqpId)) return;
                var eqpStatus = statusDict[eqpId];
                if (eqpStatus.status != "RUN" && eqpStatus.status != "IDLE") return;

                var eqpConfig = _repo.GetEqpConfig(eqpId);
                int maxWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].max_wip_qty : (eqpConfig?.MaxWipQty ?? 10);
                int batchSize = eqpConfig?.BatchSize ?? 1;

                int mesWip = wipDict.ContainsKey(eqpId) ? wipDict[eqpId].current_wip_qty : 0;
                int transitCount = allTransits.Count(t => t.TargetEqpId == eqpId);
                int currentTotalWip = mesWip + transitCount + dispatchedCount;

                LogHelper.Dispatch.Debug($"        - WIP Check: MES={mesWip}, Transit={transitCount}, Dispatched={dispatchedCount} | Total={currentTotalWip}/{maxWip}");

                if (currentTotalWip >= maxWip) return;

                var availableForThisEqp = potentialCassettes.Where(c => string.IsNullOrEmpty(c.DispatchTime)).ToList();
                if (availableForThisEqp.Count == 0) return;

                bool shouldDispatch = false; 
                string triggerReason = "";   

                if (availableForThisEqp.Count >= batchSize)
                {
                    shouldDispatch = true;
                    triggerReason = "FullBatch";
                }
                else
                {
                    if (eqpStatus.status == "IDLE")
                    {
                        int idleSec = 0;
                        int.TryParse(eqpStatus.duration, out idleSec);
                        int idleLimit = eqpConfig?.ForceIdleSec ?? 300; 
                        if (idleSec > idleLimit)
                        {
                            shouldDispatch = true;
                            triggerReason = $"Force_Idle({idleSec}s)";
                        }
                    }

                    if (!shouldDispatch)
                    {
                        foreach (var c in availableForThisEqp)
                        {
                            var dtDead = ParseDbTime(c.QTimeDeadline);
                            if (dtDead.HasValue)
                            {
                                int riskThresholdMin = 45; // 30+15
                                double remainingMin = (dtDead.Value - DateTime.Now).TotalMinutes;
                                if (remainingMin < riskThresholdMin)
                                {
                                    shouldDispatch = true;
                                    triggerReason = $"Force_QTimeRisk({remainingMin:F1}m)";
                                    break; 
                                }
                            }
                        }
                    }
                }

                if (shouldDispatch)
                {
                    int availableSpace = maxWip - currentTotalWip;
                    int dispatchCount = (triggerReason == "FullBatch") 
                        ? (Math.Min(availableForThisEqp.Count, availableSpace) / batchSize) * batchSize
                        : Math.Min(availableForThisEqp.Count, availableSpace);

                    if (dispatchCount > 0)
                    {
                        var dispatchList = availableForThisEqp.Take(dispatchCount).ToList();
                        foreach (var cassette in dispatchList)
                        {
                            if (!string.IsNullOrEmpty(cassette.PortId))
                            {
                                string cmd = $"OPEN,{cassette.PortId},{eqpId}";
                                await _tcpServer.SendCommand(cmd);
                                cassette.DispatchTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                                cassette.TargetEqpId = eqpId;
                                _repo.InsertBinding(cassette); 
                                _cloudRepo.InsertGenericLog(cassette.CarrierId, cassette.LotId, $"派貨執行: {triggerReason} -> {eqpId}");
                                LogHelper.Dispatch.Info($"        - [EXEC] {cassette.CarrierId} -> {eqpId}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                LogHelper.Dispatch.Error($"[Dispatch Error] {eqpId}: {ex.Message}", ex);
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
    }
}
