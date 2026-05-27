using System;
using System.Collections.Generic;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.MES
{
    // --- Existing Models (Previous) ---
    // 保留舊模型以免編譯錯誤，若不再使用後續可移除

    public class ValidateMoveRequest
    {
        public string CassetteID { get; set; }
        public string SourcePort { get; set; }
        public string DestinationPort { get; set; }
    }

    public class ReportStatusRequest
    {
        public string PortID { get; set; }
        public string CassetteID { get; set; }
        public string Status { get; set; }
        public string EventTime { get; set; }
    }

    public class GetTasksRequest
    {
        public string EquipmentID { get; set; }
    }

    public class StandardResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionID { get; set; }
    }

    public class MesTask
    {
        public string TaskID { get; set; }
        public string CassetteID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Priority { get; set; }
    }

    public class GetTasksResponse
    {
        public List<MesTask> Tasks { get; set; }
        public GetTasksResponse()
        {
            Tasks = new List<MesTask>();
        }
    }

    // --- Standard Wrapper for Batch APIs ---
    public class MesApiResponse<T>
    {
        public bool Success { get; set; }
        public string Msg { get; set; } // Message or Msg? Standardizing on 'Msg' or 'Message' is tricky without doc.
        public string Message { get; set; } // Support both common naming conventions
        public T Data { get; set; }
    }

    public class EqpBatchInfoResponse
    {
        public List<WipInfoResponse> Wips { get; set; }
        public List<EqStatusResponse> Statuses { get; set; }
        public EqpBatchInfoResponse()
        {
            Wips = new List<WipInfoResponse>();
            Statuses = new List<EqStatusResponse>();
        }
    }
}