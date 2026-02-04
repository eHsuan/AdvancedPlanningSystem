using Newtonsoft.Json;
using System.Collections.Generic;

namespace APSSimulator.Models
{
    // 對應 /order/batch 回應
    public class OrderInfoResponse
    {
        [JsonProperty("WorkNo")] // 協議要求大寫 W
        public string WorkOrderNumber { get; set; }

        [JsonProperty("lot_id")] // 雖然協議沒寫，保留給內部或其他用途
        public string LotID { get; set; }

        [JsonProperty("carrier_id")]
        public string CarrierId { get; set; }

        [JsonProperty("part_no")]
        public string PartNo { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("step_id")]
        public string StepId { get; set; }

        [JsonProperty("next_step_id")]
        public string NextStepId { get; set; }

        [JsonProperty("route_id")] // 協議沒明確列出，但內部邏輯可能需要
        public string RouteId { get; set; }

        [JsonProperty("current_seq_no")]
        public int CurrentSeqNo { get; set; }

        [JsonProperty("priority_type")]
        public int PriorityType { get; set; }

        [JsonProperty("due_date")]
        public string DueDate { get; set; }

        [JsonProperty("prev_out_time")]
        public string PrevOutTime { get; set; }
    }

    // 對應 /steptime/all 回應
    public class StepTimeResponse
    {
        [JsonProperty("step_id")]
        public string StepId { get; set; }

        [JsonProperty("eqp_group")]
        public string EqpGroup { get; set; }

        [JsonProperty("std_time_sec")]
        public int ProcessTimeSec { get; set; }
    }

    // 對應 /qtime/all 回應
    public class QTimeLimitResponse
    {
        [JsonProperty("step_id")]
        public string FromStepId { get; set; }

        [JsonProperty("next_step_id")]
        public string ToStepId { get; set; }

        [JsonProperty("qtime_limit_min")]
        public int QTimeLimitMin { get; set; }
    }

    // 對應 /wip/batch 回應
    public class WipInfoResponse
    {
        [JsonProperty("eq_id")] // 依據協議 API d 範例
        public string EqpId { get; set; }

        [JsonProperty("current_wip_qty")]
        public int CurrentWipQty { get; set; }

        [JsonProperty("max_wip_qty")]
        public int MaxWipQty { get; set; }
    }

    // 對應 /equipment/batch 回應
    public class EqStatusResponse
    {
        [JsonProperty("eqp_id")] // 依據協議 API e 範例
        public string EqpId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } 

        [JsonProperty("duration")] // 依據協議 API e 範例
        public string Duration { get; set; }

        [JsonProperty("current_WorkNo")] // 依據協議 API e 範例 (注意大小寫)
        public string CurrentWorkNo { get; set; }
    }
}
