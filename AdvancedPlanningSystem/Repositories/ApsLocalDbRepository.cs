using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.Repositories
{
    public class ApsLocalDbRepository
    {
        private string _connectionString;
        private string _dbPath;
        private static readonly object _dbLock = new object();

        public ApsLocalDbRepository(string dbName = "APSLocalDB.db")
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            // 加入 Busy Timeout 與 WAL 模式設定
            _connectionString = $"Data Source={_dbPath};Version=3;Busy Timeout=5000;";
            EnsureDatabaseExists();
        }

        private void EnsureDatabaseExists()
        {
            lock (_dbLock)
            {
                if (!File.Exists(_dbPath))
                {
                    SQLiteConnection.CreateFile(_dbPath);
                }

                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    // 開啟 WAL 模式以支援讀寫併發
                    using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 檢查是否需要建立表
                    CreateTables(conn);
                    SeedTestData(conn);
                }
            }
        }

        private void SeedTestData(SQLiteConnection conn)
        {
            // 檢查是否已有資料
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM local_config_eqp", conn))
            {
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0) return; // 已有資料，跳過初始化
            }

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. 初始化機台設定 (local_config_eqp)
                    var sb = new System.Text.StringBuilder();
                    sb.Append("INSERT INTO local_config_eqp (eqp_id, max_wip_qty, batch_size, force_idle_sec, max_wait_sec, is_active) VALUES ");
                    sb.Append("('LF0003', 20, 1, 18000, 24000, 1),");
                    sb.Append("('DR0026', 30, 1, 18000, 24000, 1), ('DR0009', 30, 1, 18000, 24000, 1),");
                    sb.Append("('DL0017', 20, 1, 18000, 24000, 1), ('DS0003', 20, 1, 18000, 24000, 1),");
                    sb.Append("('CL0201', 50, 1, 18000, 24000, 1), ('CL0022', 50, 1, 18000, 24000, 1),");
                    sb.Append("('CL0200', 50, 1, 18000, 24000, 1),");
                    sb.Append("('SP0002', 15, 1, 18000, 24000, 1), ('SP0004', 15, 1, 18000, 24000, 1),");
                    sb.Append("('LF0014', 20, 1, 18000, 24000, 1),");
                    sb.Append("('PR0254', 20, 1, 18000, 24000, 1), ('PR0518', 20, 1, 18000, 24000, 1),");
                    sb.Append("('EX0028', 10, 1, 18000, 24000, 1), ('EX0044', 10, 1, 18000, 24000, 1),");
                    sb.Append("('DE0025', 40, 1, 18000, 24000, 1), ('DE0005', 40, 1, 18000, 24000, 1),");
                    sb.Append("('PL0084', 50, 1, 18000, 24000, 1), ('PL0007', 50, 1, 18000, 24000, 1),");
                    sb.Append("('ST0012', 40, 1, 18000, 24000, 1),");
                    sb.Append("('ET0023', 40, 1, 18000, 24000, 1),");
                    sb.Append("('ET0026', 40, 1, 18000, 24000, 1),");
                    sb.Append("('VC0680', 10, 1, 18000, 24000, 1),");
                    sb.Append("('PR0008', 30, 1, 18000, 24000, 1), ('PR0187', 30, 1, 18000, 24000, 1), ('VC0164', 30, 1, 18000, 24000, 1),");
                    sb.Append("('OV0805', 30, 1, 18000, 24000, 1), ('VC0497', 30, 1, 18000, 24000, 1), ('SM0007', 30, 1, 18000, 24000, 1),");
                    sb.Append("('SM0015', 30, 1, 18000, 24000, 1), ('AC1837', 30, 1, 18000, 24000, 1), ('VC0498', 30, 1, 18000, 24000, 1),");
                    sb.Append("('VC1346', 30, 1, 18000, 24000, 1), ('VC1347', 30, 1, 18000, 24000, 1), ('VC1348', 30, 1, 18000, 24000, 1), ('OV1241', 30, 1, 18000, 24000, 1),");
                    sb.Append("('CU0551', 50, 1, 18000, 24000, 1), ('CU0646', 50, 1, 18000, 24000, 1), ('CU0665', 50, 1, 18000, 24000, 1),");
                    sb.Append("('CU0454', 50, 1, 18000, 24000, 1), ('CU0481', 50, 1, 18000, 24000, 1), ('CU0567', 50, 1, 18000, 24000, 1), ('CU0722', 50, 1, 18000, 24000, 1),");
                    sb.Append("('PL0088', 50, 1, 18000, 24000, 1),");
                    sb.Append("('SM0006', 20, 1, 18000, 24000, 1), ('SM0014', 20, 1, 18000, 24000, 1), ('SM0054', 20, 1, 18000, 24000, 1),");
                    sb.Append("('TP1660', 10, 1, 18000, 24000, 1), ('TP1661', 10, 1, 18000, 24000, 1), ('TP1662', 10, 1, 18000, 24000, 1),");
                    sb.Append("('TP1663', 10, 1, 18000, 24000, 1), ('TP1664', 10, 1, 18000, 24000, 1), ('TP1793', 10, 1, 18000, 24000, 1),");
                    sb.Append("('TP1796', 10, 1, 18000, 24000, 1), ('TP1797', 10, 1, 18000, 24000, 1), ('TP1872', 10, 1, 18000, 24000, 1),");
                    sb.Append("('TP1873', 10, 1, 18000, 24000, 1), ('TP1874', 10, 1, 18000, 24000, 1), ('TP2007', 10, 1, 18000, 24000, 1),");
                    sb.Append("('TP2006', 10, 1, 18000, 24000, 1), ('TP0873', 10, 1, 18000, 24000, 1), ('TP2174', 10, 1, 18000, 24000, 1),");
                    sb.Append("('TP2175', 10, 1, 18000, 24000, 1), ('TP2223', 10, 1, 18000, 24000, 1), ('TP2224', 10, 1, 18000, 24000, 1);");

                    using (var cmd = new SQLiteCommand(sb.ToString(), conn)) { cmd.ExecuteNonQuery(); }

                    // 2. 初始化站點機台對應 (local_config_step_eqp)
                    string stepEqpSql = GetStepEqpMappingSeedSql();
                    if (!string.IsNullOrEmpty(stepEqpSql))
                    {
                        using (var cmd = new SQLiteCommand(stepEqpSql, conn)) { cmd.ExecuteNonQuery(); }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.Logger.Error("SeedTestData Error: " + ex.Message);
                }
            }
        }

        private string GetStepEqpMappingSeedSql()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("INSERT INTO local_config_step_eqp (step_id, eqp_id, priority) VALUES ");
            foreach (var step in new[] { "UPLM010", "UPLM011", "UPLM002" }) sb.Append($"('{step}', 'LF0003', 1),");
            foreach (var step in new[] { "UPLM027", "UPLM035", "UPLM005", "UPLM026" }) sb.Append($"('{step}', 'LF0014', 1),");
            sb.Append("('UPPA003', 'LF0014', 2),");
            foreach (var step in new[] { "UPLA001", "UPLA006" }) { sb.Append($"('{step}', 'DR0026', 1),"); sb.Append($"('{step}', 'DR0009', 2),"); }
            foreach (var step in new[] { "UPDR003", "UPDR004", "UPDR011" }) { sb.Append($"('{step}', 'DL0017', 1),"); sb.Append($"('{step}', 'DS0003', 2),"); }
            foreach (var step in new[] { "UPCL001", "UPCL012", "UPCL022", "UPCL033", "UPCL003", "UPCL025", "UPCL006", "UPCL007", "UPCL008", "UPCL013", "UPCL028", "UPCL010" }) { sb.Append($"('{step}', 'CL0201', 1),"); sb.Append($"('{step}', 'CL0022', 2),"); }
            foreach (var step in new[] { "UPBO003", "UPRO005", "UPBO002", "UPRO006", "UPRO003", "UPRO004" }) sb.Append($"('{step}', 'CL0200', 1),");
            foreach (var step in new[] { "UPSP006", "UPSP003", "UPSP002", "UPSP005" }) { sb.Append($"('{step}', 'SP0002', 1),"); sb.Append($"('{step}', 'SP0004', 2),"); }
            foreach (var step in new[] { "UPPR005", "UPPR002", "UPPR003", "UPPR004" }) { sb.Append($"('{step}', 'PR0254', 1),"); sb.Append($"('{step}', 'PR0518', 2),"); }
            foreach (var step in new[] { "UPEX029", "UPEX030", "UPEX024", "UPEX023", "UPEX004", "UPEX018", "UPEX019", "UPEX020", "UPEX021", "UPEX027", "UPEX012", "UPEX013", "UPEX014" }) { sb.Append($"('{step}', 'EX0028', 1),"); sb.Append($"('{step}', 'EX0044', 2),"); }
            foreach (var step in new[] { "UPDE010", "UPDE002", "UPDE012", "UPDE008", "UPDE009", "UPDE006", "UPDE007" }) sb.Append($"('{step}', 'DE0025', 1),");
            foreach (var step in new[] { "UPDF001", "UPDF003", "UPDF004" }) sb.Append($"('{step}', 'ST0012', 1),");
            foreach (var step in new[] { "UPPL013", "UPPL018", "UPPL010", "UPPL015" }) { sb.Append($"('{step}', 'PL0084', 1),"); sb.Append($"('{step}', 'PL0007', 2),"); }
            sb.Append("('UPPL014', 'PL0088', 1),");
            foreach (var step in new[] { "UPET018", "UPET016", "UPET002", "UPET003", "UPET013", "UPET012", "UPET011" }) sb.Append($"('{step}', 'ET0023', 1),");
            foreach (var step in new[] { "UPET007", "UPET017", "UPET014" }) sb.Append($"('{step}', 'ET0026', 1),");
            foreach (var step in new[] { "UPSC049", "UPSC035", "UPSC038", "UPSC039", "UPSC047", "UPSC059", "UPSC052", "UPSC040", "UPSC041" }) sb.Append($"('{step}', 'VC0680', 1),");
            var smtEqps = new[] { "PR0008", "PR0187", "VC0164", "OV0805", "VC0497", "SM0007", "SM0015", "AC1837", "VC0498", "VC1346", "VC1347", "VC1348", "OV1241" };
            foreach (var step in new[] { "UPST003", "UPPR004", "UPSC010", "UPST001", "UPRE001", "UPSC011" }) { for (int i = 0; i < smtEqps.Length; i++) sb.Append($"('{step}', '{smtEqps[i]}', {i + 1}),"); }
            var slicerEqps = new[] { "CU0551", "CU0646", "CU0665", "CU0454", "CU0481", "CU0567", "CU0722" };
            foreach (var step in new[] { "UPSL001", "UPSC012" }) { for (int i = 0; i < slicerEqps.Length; i++) sb.Append($"('{step}', '{slicerEqps[i]}', {i + 1}),"); }
            var dbEqps = new[] { "SM0006", "SM0014", "SM0054" };
            foreach (var step in new[] { "UPDB002" }) { for (int i = 0; i < dbEqps.Length; i++) sb.Append($"('{step}', '{dbEqps[i]}', {i + 1}),"); }
            var tpEqps = new[] { "TP1660", "TP1661", "TP1662", "TP1663", "TP1664", "TP1793", "TP1796", "TP1797", "TP1872", "TP1873", "TP1874", "TP2007", "TP2006", "TP0873", "TP2174", "TP2175", "TP2223", "TP2224" };
            foreach (var step in new[] { "UPTE001", "UPTE002", "UPQC009", "UPPK002", "UPQC008" }) { for (int i = 0; i < tpEqps.Length; i++) sb.Append($"('{step}', '{tpEqps[i]}', {i + 1}),"); }
            if (sb.Length > 0 && sb[sb.Length - 1] == ',') { sb.Remove(sb.Length - 1, 1); sb.Append(";"); }
            return sb.ToString();
        }

        private void CreateTables(SQLiteConnection conn)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS local_config_eqp (
                    eqp_id TEXT PRIMARY KEY, max_wip_qty INTEGER DEFAULT 10, 
                    batch_size INTEGER DEFAULT 1, force_idle_sec INTEGER DEFAULT 600, 
                    max_wait_sec INTEGER DEFAULT 1800, is_active INTEGER DEFAULT 1
                );
                CREATE TABLE IF NOT EXISTS local_config_qtime (
                    from_step_id TEXT NOT NULL, to_step_id TEXT NOT NULL, 
                    transport_buffer_min INTEGER DEFAULT 10, updated_at TEXT,
                    qtime_limit_min INTEGER NOT NULL,
                    PRIMARY KEY (from_step_id, to_step_id)
                );
                CREATE TABLE IF NOT EXISTS local_config_step_eqp (
                    step_id TEXT NOT NULL, eqp_id TEXT NOT NULL, priority INTEGER DEFAULT 1,
                    PRIMARY KEY (step_id, eqp_id)
                );
                CREATE TABLE IF NOT EXISTS local_state_port (
                    port_id TEXT PRIMARY KEY, status TEXT DEFAULT 'EMPTY', 
                    door_state TEXT DEFAULT 'CLOSED', last_update TEXT
                );
                CREATE TABLE IF NOT EXISTS local_state_binding (
                    carrier_id TEXT PRIMARY KEY, port_id TEXT, lot_id TEXT,
                    current_step_id TEXT, next_step_id TEXT, target_eqp_id TEXT,
                    qtime_deadline TEXT, dispatch_score REAL DEFAULT 0, 
                    score_qtime REAL DEFAULT 0, score_urgent REAL DEFAULT 0, 
                    score_eng REAL DEFAULT 0, score_due REAL DEFAULT 0, score_lead REAL DEFAULT 0,
                    priority_type INTEGER DEFAULT 0, is_hold INTEGER DEFAULT 0,
                    wait_reason TEXT, 
                    bind_time TEXT, dispatch_time TEXT,
                    FOREIGN KEY (port_id) REFERENCES local_state_port(port_id)
                );
                CREATE TABLE IF NOT EXISTS local_state_transit (
                    carrier_id TEXT PRIMARY KEY, lot_id TEXT, target_eqp_id TEXT NOT NULL, 
                    next_step_id TEXT, dispatch_time TEXT, pickup_time TEXT, 
                    expected_arrival_time TEXT, is_overdue INTEGER DEFAULT 0
                );
            ";
            using (var cmd = new SQLiteCommand(sql, conn)) { cmd.ExecuteNonQuery(); }
        }

        public List<ConfigStepEqp> GetStepEqpMappings()
        {
            lock (_dbLock)
            {
                var list = new List<ConfigStepEqp>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM local_config_step_eqp", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ConfigStepEqp { StepId = reader["step_id"].ToString(), EqpId = reader["eqp_id"].ToString(), Priority = Convert.ToInt32(reader["priority"]) });
                        }
                    }
                }
                return list;
            }
        }

        public List<ConfigQTime> GetQTimeConfigs()
        {
            lock (_dbLock)
            {
                var list = new List<ConfigQTime>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM local_config_qtime", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ConfigQTime { FromStepId = reader["from_step_id"].ToString(), ToStepId = reader["to_step_id"].ToString(), TransportBufferMin = Convert.ToInt32(reader["transport_buffer_min"]), QTimeLimitMin = Convert.ToInt32(reader["qtime_limit_min"]), UpdatedAt = reader["updated_at"].ToString() });
                        }
                    }
                }
                return list;
            }
        }

        public ConfigEqp GetEqpConfig(string eqpId)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM local_config_eqp WHERE eqp_id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", eqpId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ConfigEqp { EqpId = reader["eqp_id"].ToString(), MaxWipQty = Convert.ToInt32(reader["max_wip_qty"]), BatchSize = Convert.ToInt32(reader["batch_size"]), ForceIdleSec = Convert.ToInt32(reader["force_idle_sec"]), MaxWaitSec = Convert.ToInt32(reader["max_wait_sec"]), IsActive = Convert.ToInt32(reader["is_active"]) };
                            }
                        }
                    }
                }
                return null;
            }
        }

        public List<ConfigEqp> GetAllEqpConfigs()
        {
            lock (_dbLock)
            {
                var list = new List<ConfigEqp>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM local_config_eqp", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ConfigEqp { EqpId = reader["eqp_id"].ToString(), MaxWipQty = Convert.ToInt32(reader["max_wip_qty"]), BatchSize = Convert.ToInt32(reader["batch_size"]), ForceIdleSec = Convert.ToInt32(reader["force_idle_sec"]), MaxWaitSec = Convert.ToInt32(reader["max_wait_sec"]), IsActive = Convert.ToInt32(reader["is_active"]) });
                        }
                    }
                }
                return list;
            }
        }

        public void UpdateEqpMaxWip(string eqpId, int maxWip)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE local_config_eqp SET max_wip_qty = @m WHERE eqp_id = @id";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@m", maxWip);
                        cmd.Parameters.AddWithValue("@id", eqpId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdatePortStateOnly(string portId, string status)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT OR REPLACE INTO local_state_port (port_id, status, last_update) VALUES (@p, @s, @t)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@p", portId);
                        cmd.Parameters.AddWithValue("@s", status);
                        cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<StatePort> GetActivePorts()
        {
            lock (_dbLock)
            {
                var list = new List<StatePort>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT p.*, b.carrier_id, b.lot_id, b.dispatch_time, b.next_step_id, b.target_eqp_id 
                        FROM local_state_port p 
                        LEFT JOIN local_state_binding b ON p.port_id = b.port_id 
                        WHERE p.status = 'OCCUPIED'";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StatePort { PortId = reader["port_id"].ToString(), Status = reader["status"].ToString(), DoorState = reader["door_state"].ToString(), LastUpdate = reader["last_update"].ToString(), CarrierId = reader["carrier_id"] == DBNull.Value ? null : reader["carrier_id"].ToString(), LotId = reader["lot_id"] == DBNull.Value ? null : reader["lot_id"].ToString(), DispatchTime = reader["dispatch_time"] == DBNull.Value ? null : reader["dispatch_time"].ToString(), NextStepId = reader["next_step_id"] == DBNull.Value ? null : reader["next_step_id"].ToString(), TargetEqpId = reader["target_eqp_id"] == DBNull.Value ? null : reader["target_eqp_id"].ToString() });
                        }
                    }
                }
                return list;
            }
        }

        public void InsertBinding(StateBinding binding)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT OR REPLACE INTO local_state_binding 
                        (carrier_id, port_id, lot_id, current_step_id, next_step_id, target_eqp_id, 
                         qtime_deadline, dispatch_score, 
                         score_qtime, score_urgent, score_eng, score_due, score_lead,
                         priority_type, is_hold, wait_reason, bind_time, dispatch_time)
                        VALUES (@cid, @pid, @lid, @sid, @nid, @teqp, @dead, @score, 
                                @sq, @su, @se, @sd, @sl,
                                @pri, @hold, @wreason, @btime, @dtime)
                    ";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", binding.CarrierId);
                        cmd.Parameters.AddWithValue("@pid", binding.PortId);
                        cmd.Parameters.AddWithValue("@lid", binding.LotId);
                        cmd.Parameters.AddWithValue("@sid", binding.CurrentStepId ?? "");
                        cmd.Parameters.AddWithValue("@nid", binding.NextStepId ?? "");
                        cmd.Parameters.AddWithValue("@teqp", binding.TargetEqpId ?? "");
                        cmd.Parameters.AddWithValue("@dead", binding.QTimeDeadline ?? "");
                        cmd.Parameters.AddWithValue("@score", binding.DispatchScore);
                        cmd.Parameters.AddWithValue("@sq", binding.ScoreQTime);
                        cmd.Parameters.AddWithValue("@su", binding.ScoreUrgent);
                        cmd.Parameters.AddWithValue("@se", binding.ScoreEng);
                        cmd.Parameters.AddWithValue("@sd", binding.ScoreDue);
                        cmd.Parameters.AddWithValue("@sl", binding.ScoreLead);
                        cmd.Parameters.AddWithValue("@pri", binding.PriorityType);
                        cmd.Parameters.AddWithValue("@hold", binding.IsHold);
                        cmd.Parameters.AddWithValue("@wreason", binding.WaitReason ?? "");
                        cmd.Parameters.AddWithValue("@btime", binding.BindTime ?? "");
                        cmd.Parameters.AddWithValue("@dtime", (object)binding.DispatchTime ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<StateBinding> GetWaitBindings()
        {
            lock (_dbLock)
            {
                var list = new List<StateBinding>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM local_state_binding WHERE dispatch_time IS NULL OR dispatch_time = ''";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader()) { while (reader.Read()) list.Add(MapBinding(reader)); }
                }
                return list;
            }
        }

        public List<StateBinding> GetSortedWaitBindings()
        {
            lock (_dbLock)
            {
                var list = new List<StateBinding>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM local_state_binding WHERE (dispatch_time IS NULL OR dispatch_time = '') ORDER BY dispatch_score DESC";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader()) { while (reader.Read()) list.Add(MapBinding(reader)); }
                }
                return list;
            }
        }

        public StateBinding GetBinding(string carrierId)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM local_state_binding WHERE carrier_id = @cid";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", carrierId);
                        using (var reader = cmd.ExecuteReader()) { if (reader.Read()) return MapBinding(reader); }
                    }
                }
                return null;
            }
        }

        public List<StateBinding> GetAllBindings()
        {
            lock (_dbLock)
            {
                var list = new List<StateBinding>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM local_state_binding";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader()) { while (reader.Read()) list.Add(MapBinding(reader)); }
                }
                return list;
            }
        }

        public void MoveToTransit(StateTransit transit)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SQLiteCommand("DELETE FROM local_state_binding WHERE carrier_id = @cid", conn)) { cmd.Parameters.AddWithValue("@cid", transit.CarrierId); cmd.ExecuteNonQuery(); }
                            string insSql = "INSERT OR REPLACE INTO local_state_transit (carrier_id, lot_id, target_eqp_id, next_step_id, dispatch_time, pickup_time, expected_arrival_time, is_overdue) VALUES (@cid, @lot, @teqp, @nstep, @dtime, @ptime, @exp, @ovd)";
                            using (var cmd = new SQLiteCommand(insSql, conn)) { cmd.Parameters.AddWithValue("@cid", transit.CarrierId); cmd.Parameters.AddWithValue("@lot", transit.LotId); cmd.Parameters.AddWithValue("@teqp", transit.TargetEqpId); cmd.Parameters.AddWithValue("@nstep", transit.NextStepId); cmd.Parameters.AddWithValue("@dtime", transit.DispatchTime); cmd.Parameters.AddWithValue("@ptime", transit.PickupTime); cmd.Parameters.AddWithValue("@exp", transit.ExpectedArrivalTime); cmd.Parameters.AddWithValue("@ovd", transit.IsOverdue); cmd.ExecuteNonQuery(); }
                            transaction.Commit();
                        }
                        catch { transaction.Rollback(); throw; }
                    }
                }
            }
        }

        public List<StateTransit> GetAllTransits()
        {
            lock (_dbLock)
            {
                var list = new List<StateTransit>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM local_state_transit", conn))
                    using (var reader = cmd.ExecuteReader()) { while (reader.Read()) list.Add(MapTransit(reader)); }
                }
                return list;
            }
        }

        public void RemoveTransit(string carrierId)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM local_state_transit WHERE carrier_id = @cid", conn)) { cmd.Parameters.AddWithValue("@cid", carrierId); cmd.ExecuteNonQuery(); }
                }
            }
        }

        public void ClearAllStates()
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            new SQLiteCommand("DELETE FROM local_state_binding", conn).ExecuteNonQuery();
                            new SQLiteCommand("DELETE FROM local_state_transit", conn).ExecuteNonQuery();
                            new SQLiteCommand("DELETE FROM local_state_port", conn).ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        private StateBinding MapBinding(SQLiteDataReader reader)
        {
            Func<string, double> safeReadDouble = (col) => { try { return reader[col] == DBNull.Value ? 0 : Convert.ToDouble(reader[col]); } catch { return 0; } };
            string safeReadString(string col) { try { return reader[col] == DBNull.Value ? "" : reader[col].ToString(); } catch { return ""; } }
            return new StateBinding { CarrierId = reader["carrier_id"].ToString(), PortId = reader["port_id"].ToString(), LotId = reader["lot_id"].ToString(), CurrentStepId = reader["current_step_id"].ToString(), NextStepId = reader["next_step_id"].ToString(), TargetEqpId = reader["target_eqp_id"].ToString(), QTimeDeadline = reader["qtime_deadline"].ToString(), DispatchScore = Convert.ToDouble(reader["dispatch_score"]), ScoreQTime = safeReadDouble("score_qtime"), ScoreUrgent = safeReadDouble("score_urgent"), ScoreEng = safeReadDouble("score_eng"), ScoreDue = safeReadDouble("score_due"), ScoreLead = safeReadDouble("score_lead"), PriorityType = Convert.ToInt32(reader["priority_type"]), IsHold = Convert.ToInt32(reader["is_hold"]), WaitReason = safeReadString("wait_reason"), BindTime = reader["bind_time"].ToString(), DispatchTime = reader["dispatch_time"] == DBNull.Value ? null : reader["dispatch_time"].ToString() };
        }

        private StateTransit MapTransit(SQLiteDataReader reader)
        {
            return new StateTransit { CarrierId = reader["carrier_id"].ToString(), LotId = reader["lot_id"].ToString(), TargetEqpId = reader["target_eqp_id"].ToString(), NextStepId = reader["next_step_id"].ToString(), DispatchTime = reader["dispatch_time"].ToString(), PickupTime = reader["pickup_time"].ToString(), ExpectedArrivalTime = reader["expected_arrival_time"].ToString(), IsOverdue = Convert.ToInt32(reader["is_overdue"]) };
        }
    }
}