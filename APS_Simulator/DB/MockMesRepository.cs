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

                    string nextStep = GetNextStep(conn, routeId, currSeq);

                    // 注意: 新 Schema 沒有 lot_id, part_no, quantity, status
                    // 為了相容 Model，我們給預設值，或盡量從現有欄位對應
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

        private string GetNextStep(SQLiteConnection conn, string routeId, int currentSeq)
        {
            if (string.IsNullOrEmpty(routeId)) return null;

            string sql = @"
                SELECT step_id 
                FROM mock_mes_route_def 
                WHERE route_id = @rid AND seq_no > @seq 
                ORDER BY seq_no ASC 
                LIMIT 1";
            
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@rid", routeId);
                cmd.Parameters.AddWithValue("@seq", currentSeq);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "END";
            }
        }
    }
}
