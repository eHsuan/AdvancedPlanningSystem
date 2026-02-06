using System;
using System.Collections.Generic;

namespace AdvancedPlanningSystem.Models
{
    // local_config_eqp
    public class ConfigEqp
    {
        public string EqpId { get; set; }          // eqp_id
        public int MaxWipQty { get; set; }         // max_wip_qty
        public int BatchSize { get; set; }         // batch_size
        public int ForceIdleSec { get; set; }      // force_idle_sec
        public int MaxWaitSec { get; set; }        // max_wait_sec
        public int IsActive { get; set; }          // is_active
    }

    // local_config_qtime
    public class ConfigQTime
    {
        public string FromStepId { get; set; }     // from_step_id
        public string ToStepId { get; set; }       // to_step_id
        public int TransportBufferMin { get; set; } // transport_buffer_min
        public string UpdatedAt { get; set; }      // updated_at
        
        // 記憶體專用 (非 DB 欄位，從 MES API 取得)
        public int QTimeLimitMin { get; set; }     
    }

    // local_config_step_eqp
    public class ConfigStepEqp
    {
        public string StepId { get; set; }         // step_id
        public string EqpId { get; set; }          // eqp_id
        public int Priority { get; set; }          // priority
    }

    // local_state_port
    public class StatePort
    {
        public string PortId { get; set; }         // port_id
        public string Status { get; set; }         // status
        public string DoorState { get; set; }      // door_state
        public string LastUpdate { get; set; }     // last_update
        
        // Join 查詢用的擴充欄位 (非 Port 表原生)
        public string CarrierId { get; set; }
        public string LotId { get; set; }
        public string DispatchTime { get; set; }   // 派送時間 (用於 UI 顯示綠色狀態)
        public string NextStepId { get; set; }     // 下一站 (用於判斷是否完工)
        public string TargetEqpId { get; set; }    // 目標機台
        public int IsHold { get; set; }            // 是否異常攔截
        public double DispatchScore { get; set; }  // 派貨分數
        public double TReal { get; set; }          // 真實剩餘時間
    }

    // local_state_binding
    public class StateBinding
    {
        public string CarrierId { get; set; }      // carrier_id (PK)
        public string PortId { get; set; }         // port_id
        public string LotId { get; set; }          // lot_id
        
        public string CurrentStepId { get; set; }  // current_step_id
        public string NextStepId { get; set; }     // next_step_id
        public string TargetEqpId { get; set; }    // target_eqp_id
        
        public string QTimeDeadline { get; set; }  // qtime_deadline
        public double DispatchScore { get; set; }  // dispatch_score (Total)
        
        // 分數細項 (Score Breakdown)
        public double ScoreQTime { get; set; }
        public double ScoreUrgent { get; set; }
        public double ScoreEng { get; set; }
        public double ScoreDue { get; set; }
        public double ScoreLead { get; set; }

        public double TReal { get; set; }          // 真實剩餘時間 (T_Real)

        public int PriorityType { get; set; }      // priority_type
        public int IsHold { get; set; }            // is_hold
        
        public string WaitReason { get; set; }     // wait_reason (新增: 等待原因)

        public string BindTime { get; set; }       // bind_time
        public string DispatchTime { get; set; }   // dispatch_time

        // 運算用暫存欄位 (非 DB 原生)
        public DateTime? PrevOutTime { get; set; }
        public DateTime? DueDate { get; set; }
        public double T_safe { get; set; }
    }

    // local_state_transit
    public class StateTransit
    {
        public string CarrierId { get; set; }      // carrier_id
        public string LotId { get; set; }          // lot_id
        public string TargetEqpId { get; set; }    // target_eqp_id
        public string NextStepId { get; set; }     // next_step_id
        
        public string DispatchTime { get; set; }   // dispatch_time
        public string PickupTime { get; set; }     // pickup_time
        public string ExpectedArrivalTime { get; set; } // expected_arrival_time
        public int IsOverdue { get; set; }         // is_overdue
    }

    // --- API DTOs (維持 Phase 1 的更新) ---
    public class OrderInfoResponse
    {
        public string WorkNo { get; set; }
        public string carrier_id { get; set; }
        public string step_id { get; set; }
        public string next_step_id { get; set; }
        public string prev_out_time { get; set; }
        public int priority_type { get; set; }
        public string due_date { get; set; }
    }

    public class StepTimeResponse
    {
        public string step_id { get; set; }
        public int std_time_sec { get; set; }
    }

    public class QTimeLimitResponse
    {
        public string step_id { get; set; }
        public string next_step_id { get; set; }
        public int qtime_limit_min { get; set; }
    }

    public class WipInfoResponse
    {
        public string eq_id { get; set; }
        public int current_wip_qty { get; set; }
        public int max_wip_qty { get; set; }
    }

    public class EqStatusResponse
    {
        public string eqp_id { get; set; }
        public string status { get; set; } 
        public string duration { get; set; } 
        public string current_WorkNo { get; set; }
    }
}
