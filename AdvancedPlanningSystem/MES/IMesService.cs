using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.MES
{
    /// <summary>
    /// 定義與 MES 系統溝通的標準介面 (Batch Update)
    /// </summary>
    public interface IMesService
    {
        // 1. API a. 工單資訊批次查詢
        Task<List<OrderInfoResponse>> GetOrderInfoBatchAsync(List<string> workNos);

        // 2. API b. 標準工時查詢 (全部)
        Task<List<StepTimeResponse>> GetAllStepTimesAsync();

        // 3. API c. QTime 設定查詢 (維持不變)
        Task<List<QTimeLimitResponse>> GetAllQTimeLimitsAsync();

        // 4. API d. WIP 監控批次查詢
        Task<List<WipInfoResponse>> GetWipBatchAsync(List<string> eqpIds);

        // 5. API e. 機台狀態批次查詢
        Task<List<EqStatusResponse>> GetEquipmentStatusBatchAsync(List<string> eqpIds);

        // --- Legacy / Others ---
        Task<StandardResponse> ValidateMoveAsync(string cassetteId, string source, string destination);
        Task<StandardResponse> ReportStatusAsync(string portId, string cassetteId, string status);
    }
}
