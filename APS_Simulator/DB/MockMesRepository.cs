using System;
using System.Collections.Generic;
using System.Data.SQLite;
using APSSimulator.Models;

namespace APSSimulator.DB
{
    public class MockMesRepository
    {
        public List<OrderInfoResponse> GetOrders(SQLiteConnection conn, List<string> workNos)
        {
            var list = new List<OrderInfoResponse>();
            // 使用傳入的 conn，不需再 Open/Dispose
            DatabaseHelper.EnsureOrdersTableColumns(conn);
            
            string sql = "SELECT * FROM mock_mes_orders"; 
            
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // 修正欄位名稱
                    string wo = reader["work_no"].ToString();
                    if (workNos != null && workNos.Count > 0 && !workNos.Contains(wo)) continue;

                    string routeId = reader["route_id"] == DBNull.Value ? "" : reader["route_id"].ToString();
                    int currSeq = reader["current_seq_no"] == DBNull.Value ? 0 : Convert.ToInt32(reader["current_seq_no"]);
                    string currStep = reader["step_id"] == DBNull.Value ? "" : reader["step_id"].ToString();

                    GetNextSteps(conn, routeId, currSeq, out string nextStep, out string next2Step);

                    string mach1 = reader["mach_nos_next1"] != DBNull.Value ? reader["mach_nos_next1"].ToString() : "";
                    if (string.IsNullOrEmpty(mach1)) mach1 = DatabaseHelper.GetDefaultEqpsForStep(nextStep);

                    string mach2 = reader["mach_nos_next2"] != DBNull.Value ? reader["mach_nos_next2"].ToString() : "";
                    if (string.IsNullOrEmpty(mach2)) mach2 = DatabaseHelper.GetDefaultEqpsForStep(next2Step);

                    list.Add(new OrderInfoResponse
                    {
                        WorkOrderNumber = wo,
                        CarrierId = reader["carrier_id"].ToString(),
                        LotID = reader["carrier_id"].ToString(), // 暫時用 carrier_id 頂替 LotID
                        PartNo = "N/A",
                        Quantity = 25,
                        Status = "Released",
                        StepId = currStep,
                        NextStepId = nextStep,
                        Next2StepId = next2Step,
                        MachNosNext1 = mach1,
                        MachNosNext2 = mach2,
                        RouteId = routeId,
                        CurrentSeqNo = currSeq,
                        PriorityType = reader["priority_type"] == DBNull.Value ? 0 : Convert.ToInt32(reader["priority_type"]),
                        DueDate = reader["due_date"] == DBNull.Value ? "" : reader["due_date"].ToString(),
                        PrevOutTime = reader["prev_out_time"] == DBNull.Value ? "" : reader["prev_out_time"].ToString()
                    });
                }
            }
            return list;
        }

        private void GetNextSteps(SQLiteConnection conn, string routeId, int currentSeq, out string nextStep, out string next2Step)
        {
            nextStep = "END";
            next2Step = "END";
            if (string.IsNullOrEmpty(routeId)) return;

            string sql = @"
                SELECT step_id 
                FROM mock_mes_route_def 
                WHERE route_id = @rid AND seq_no > @seq 
                ORDER BY seq_no ASC 
                LIMIT 2";
            
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@rid", routeId);
                cmd.Parameters.AddWithValue("@seq", currentSeq);
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read()) nextStep = r["step_id"].ToString();
                    if (r.Read()) next2Step = r["step_id"].ToString();
                }
            }
        }
    }
}
