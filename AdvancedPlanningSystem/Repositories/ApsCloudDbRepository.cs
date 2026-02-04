using System;
using System.Data.SQLite;
using System.IO;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.Repositories
{
    public class ApsCloudDbRepository
    {
        private string _connectionString;
        private static readonly object _cloudLock = new object();

        public ApsCloudDbRepository()
        {
            _connectionString = AppConfig.CloudDbConnectionString;
            
            // 如果是 Local 模式且檔案不存在，建立它
            if (AppConfig.UseCloudDb == false)
            {
                var parts = _connectionString.Split(';');
                foreach (var part in parts)
                {
                    if (part.Trim().StartsWith("Data Source", StringComparison.OrdinalIgnoreCase))
                    {
                        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, part.Split('=')[1].Trim());
                        if (!File.Exists(dbPath))
                        {
                            SQLiteConnection.CreateFile(dbPath);
                            using (var conn = new SQLiteConnection(_connectionString))
                            {
                                conn.Open();
                                string sql = @"
                                    CREATE TABLE cloud_history_log_cache (
                                        log_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        carrier_id TEXT, lot_id TEXT, port_id TEXT,
                                        step_id TEXT, next_step_id TEXT, target_eqp_id TEXT,
                                        final_score REAL, priority_type INTEGER,
                                        bind_time TEXT, dispatch_time TEXT, pickup_time TEXT,
                                        archived_at TEXT DEFAULT CURRENT_TIMESTAMP, note TEXT
                                    );";
                                using (var cmd = new SQLiteCommand(sql, conn)) { cmd.ExecuteNonQuery(); }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void InsertHistoryLog(StateBinding binding, string pickupTime, string note)
        {
            lock (_cloudLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO cloud_history_log_cache 
                        (carrier_id, lot_id, port_id, step_id, next_step_id, target_eqp_id, final_score, priority_type, bind_time, dispatch_time, pickup_time, note)
                        VALUES (@cid, @lid, @pid, @sid, @nid, @teqp, @score, @pri, @btime, @dtime, @ptime, @note)
                    ";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", binding.CarrierId);
                        cmd.Parameters.AddWithValue("@lid", binding.LotId);
                        cmd.Parameters.AddWithValue("@pid", binding.PortId);
                        cmd.Parameters.AddWithValue("@sid", binding.CurrentStepId);
                        cmd.Parameters.AddWithValue("@nid", binding.NextStepId);
                        cmd.Parameters.AddWithValue("@teqp", binding.TargetEqpId);
                        cmd.Parameters.AddWithValue("@score", binding.DispatchScore);
                        cmd.Parameters.AddWithValue("@pri", binding.PriorityType);
                        cmd.Parameters.AddWithValue("@btime", binding.BindTime);
                        cmd.Parameters.AddWithValue("@dtime", binding.DispatchTime);
                        cmd.Parameters.AddWithValue("@ptime", pickupTime);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void InsertGenericLog(string carrierId, string lotId, string note)
        {
            lock (_cloudLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO cloud_history_log_cache (carrier_id, lot_id, note) VALUES (@cid, @lid, @note)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", carrierId);
                        cmd.Parameters.AddWithValue("@lid", lotId);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
