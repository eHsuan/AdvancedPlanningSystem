using System;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace APSSimulator.DB
{
    public static class DatabaseHelper
    {
        private static string _dbName = "APSSimulatorDB.db";
        public static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbName);
        public static string ConnectionString => $"Data Source={DbPath};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            ResetDatabase();
        }

        public static void ResetDatabase()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. 清空舊表 & 重建 Schema
                        ExecuteSql(conn, GetSchemaSql());

                        // 2. 寫入機台資料
                        ExecuteSql(conn, GetEquipmentSeedSql());

                        // 3. 寫入製程流程資料 (03SEC & uBMU)
                        ExecuteSql(conn, GetRouteDefSeedSql());

                        // 4. 寫入測試工單
                        ExecuteSql(conn, GetOrderSeedSql());

                        // 5. 補寫入其他 Config 表
                        ExecuteSql(conn, GetExtraConfigSeedSql());
                        
                        // 6. 寫入 QTime 規則
                        ExecuteSql(conn, GetMesQTimeRuleSeedSql());

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new Exception("Database Reset Failed: " + ex.Message);
                    }
                }
            }
        }

        private static void ExecuteSql(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetSchemaSql()
        {
            // 根據使用者提供的 DB 結構
            return @"
                DROP TABLE IF EXISTS mock_mes_equipments;
                DROP TABLE IF EXISTS mock_mes_route_def;
                DROP TABLE IF EXISTS mock_mes_orders;
                DROP TABLE IF EXISTS mock_mes_step_time;
                DROP TABLE IF EXISTS mock_mes_qtime_rule;
                -- 保留 op_log 因為它是 UI 顯示用的，不在使用者提供的核心 Schema 內但需要它
                DROP TABLE IF EXISTS op_log;

                CREATE TABLE mock_mes_equipments (
                    eqp_id TEXT PRIMARY KEY,
                    status TEXT DEFAULT 'RUN',
                    status_duration TEXT DEFAULT '60',
                    current_work_no TEXT,
                    current_wip_qty INTEGER DEFAULT 0,
                    max_wip_qty INTEGER DEFAULT 50
                );

                CREATE TABLE mock_mes_orders (
                    work_no TEXT,
                    carrier_id TEXT,
                    step_id TEXT,
                    next_step_id TEXT,
                    prev_out_time TEXT,
                    priority_type INTEGER DEFAULT 0,
                    due_date TEXT,
                    route_id TEXT,
                    current_seq_no INTEGER,
                    PRIMARY KEY(work_no)
                );

                CREATE TABLE mock_mes_route_def (
                    route_id TEXT NOT NULL,
                    seq_no INTEGER NOT NULL,
                    step_id TEXT NOT NULL,
                    step_name TEXT NOT NULL,
                    PRIMARY KEY(route_id, seq_no)
                );

                CREATE TABLE mock_mes_qtime_rule (
                    step_id TEXT NOT NULL,
                    next_step_id TEXT NOT NULL,
                    qtime_limit_min INTEGER,
                    PRIMARY KEY (step_id, next_step_id)
                );

                CREATE TABLE mock_mes_step_time (
                    step_id TEXT PRIMARY KEY,
                    std_time_sec INTEGER DEFAULT 3600
                );

                CREATE TABLE op_log (
                    log_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    action_type TEXT,
                    port_id TEXT,
                    target_id TEXT,
                    timestamp TEXT DEFAULT CURRENT_TIMESTAMP
                );
            ";
        }

        private static string GetEquipmentSeedSql()
        {
            var sb = new StringBuilder();
            // 移除 group_name, 配合 DB 結構
            sb.Append("INSERT INTO mock_mes_equipments (eqp_id, status, status_duration, current_work_no, current_wip_qty, max_wip_qty) VALUES ");

            // 1. ABF壓合 (LF)
            sb.Append("('LF0003', 'RUN', '3600', '2P2552A1B2C3', 5, 20),");

            // 2. 雷射 (Laser)
            sb.Append("('DR0026', 'RUN', '1200', '2P25523A7B9C', 8, 30), ('DR0009', 'RUN', '1200', '2P25525D2E4F', 10, 30),");

            // 3. CF4 Plasma
            sb.Append("('DL0017', 'RUN', '500', '2P25526G8H0I', 2, 20), ('DS0003', 'DOWN', '0', NULL, 0, 20),");

            // 4. 酸洗 (Acid Clean)
            sb.Append("('CL0201', 'RUN', '300', '2P2552J2K4L6', 15, 50), ('CL0022', 'IDLE', '0', NULL, 0, 50),");

            // 5. 粗化 (Roughness)
            sb.Append("('CL0200', 'RUN', '300', '2P2552M8N0O2', 10, 50),");

            // 6. 著膜 (Sputter)
            sb.Append("('SP0002', 'RUN', '4000', '2P2552P4Q6R8', 4, 15), ('SP0004', 'IDLE', '0', NULL, 0, 15),");

            // 7. 乾膜壓合 (Dry Film)
            sb.Append("('LF0014', 'RUN', '600', '2P2552Z1A2B3', 3, 20),");

            // 8. 綠漆印刷 (Solder Mask)
            sb.Append("('PR0254', 'RUN', '600', '2P2552C4D5E6', 3, 20), ('PR0518', 'RUN', '600', '2P2552F7G8H9', 2, 20),");

            // 9. 曝光 (Exposure)
            sb.Append("('EX0028', 'RUN', '300', '2P2552S0T2U4', 5, 10), ('EX0044', 'RUN', '250', '2P2552V6W8X0', 6, 10),");

            // 10. 乾膜/濕膜顯影 (Developer)
            sb.Append("('DE0025', 'RUN', '450', '2P2552Y2Z4A6', 8, 40), ('DE0005', 'RUN', '500', '2P2552B8C0D2', 7, 40),");

            // 11. 電鍍 (Plating)
            sb.Append("('PL0084', 'RUN', '3600', '2P2552E4F6G8', 20, 50), ('PL0007', 'IDLE', '0', NULL, 0, 50),");

            // 12. 去膜 (Stripper)
            sb.Append("('ST0012', 'RUN', '600', '2P2552H0I2J4', 5, 40),");

            // 13. Cu蝕刻 (Etch)
            sb.Append("('ET0023', 'RUN', '300', '2P2552Q1R2S3', 10, 40),");

            // 14. NiCr蝕刻
            sb.Append("('ET0026', 'RUN', '350', '2P2552K6L8M0', 8, 40),");

            // 15. AOI
            sb.Append("('VC0680', 'RUN', '200', '2P2552N2O4P6', 2, 10),");

            // 16. SMT (大量機台)
            sb.Append("('PR0008', 'RUN', '600', '2P2552T4U5V6', 5, 30), ('PR0187', 'RUN', '600', '2P2552W7X8Y9', 4, 30),");
            sb.Append("('VC0164', 'IDLE', '0', NULL, 0, 30), ('OV0805', 'RUN', '600', '2P2552Q8R0S2', 2, 30),");
            sb.Append("('VC0497', 'RUN', '600', '2P2552X1Y2Z3', 3, 30), ('SM0007', 'RUN', '500', '2P2552A4B5C6', 6, 30),");
            sb.Append("('SM0015', 'RUN', '500', '2P2552D7E8F9', 5, 30), ('AC1837', 'RUN', '400', '2P2552G0H1I2', 2, 30),");
            sb.Append("('VC0498', 'RUN', '600', '2P2552J3K4L5', 1, 30), ('VC1346', 'RUN', '600', '2P2552M6N7O8', 1, 30),");
            sb.Append("('VC1347', 'RUN', '600', '2P2552P9Q0R1', 1, 30), ('VC1348', 'IDLE', '0', NULL, 0, 30),");
            sb.Append("('OV1241', 'RUN', '600', '2P2552S2T3U4', 2, 30),");

            // 17. Slicer (切片)
            sb.Append("('CU0551', 'RUN', '300', '2P2552V5W6X7', 10, 50), ('CU0646', 'RUN', '300', '2P2552Y8Z9A0', 8, 50),");
            sb.Append("('CU0665', 'IDLE', '0', NULL, 0, 50), ('CU0454', 'RUN', '300', '2P2552B1C2D3', 9, 50),");
            sb.Append("('CU0481', 'RUN', '300', '2P2552E4F5G6', 7, 50), ('CU0567', 'IDLE', '0', NULL, 0, 50),");
            sb.Append("('CU0722', 'RUN', '300', '2P2552H7I8J9', 11, 50),");

            // 18. NiAu (化金)
            sb.Append("('PL0088', 'RUN', '3600', '2P2552K0L1M2', 15, 50),");

            // 19. uPOL-DB
            sb.Append("('SM0006', 'RUN', '800', '2P2552N3O4P5', 3, 20), ('SM0014', 'RUN', '800', '2P2552Q6R7S8', 2, 20), ('SM0054', 'IDLE', '0', NULL, 0, 20),");

            // 20. uPOL-TP (測試包裝)
            sb.Append("('TP1660', 'RUN', '900', '2P2552T9U0V1', 1, 10), ('TP1661', 'RUN', '900', '2P2552W2X3Y4', 1, 10),");
            sb.Append("('TP1662', 'RUN', '900', '2P2552Z5A6B7', 1, 10), ('TP1663', 'RUN', '900', '2P2552C8D9E0', 1, 10),");
            sb.Append("('TP1664', 'IDLE', '0', NULL, 0, 10), ('TP1793', 'RUN', '900', '2P2552F1G2H3', 1, 10),");
            sb.Append("('TP1796', 'RUN', '900', '2P2552I4J5K6', 1, 10), ('TP1797', 'RUN', '900', '2P2552L7M8N9', 1, 10),");
            sb.Append("('TP1872', 'RUN', '900', '2P2552O0P1Q2', 1, 10), ('TP1873', 'DOWN', '0', NULL, 0, 10),");
            sb.Append("('TP1874', 'RUN', '900', '2P2552R3S4T5', 1, 10), ('TP2007', 'RUN', '900', '2P2552U6V7W8', 1, 10),");
            sb.Append("('TP2006', 'RUN', '900', '2P2552X9Y0Z1', 1, 10), ('TP0873', 'RUN', '900', '2P2552A2B3C4', 1, 10),");
            sb.Append("('TP2174', 'RUN', '900', '2P2552D5E6F7', 1, 10), ('TP2175', 'IDLE', '0', NULL, 0, 10),");
            sb.Append("('TP2223', 'RUN', '900', '2P2552G8H9I0', 1, 10), ('TP2224', 'RUN', '900', '2P2552J1K2L3', 1, 10);");

            return sb.ToString();
        }

        private static string GetRouteDefSeedSql()
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO mock_mes_route_def (route_id, seq_no, step_id, step_name) VALUES ");

            // 03SEC (完整保留)
            sb.Append("('03SEC', 2, 'UPPA008', 'L/F貼耐熱膠帶'),");
            sb.Append("('03SEC', 501, 'UPDB002', 'D/B-IC D/B'),");
            sb.Append("('03SEC', 600, 'UPMS001', 'D/B-量測'),");
            sb.Append("('03SEC', 603, 'UPBO003', 'B-ABF 粗化'),");
            sb.Append("('03SEC', 700, 'UPOV076', 'ABF 粗化烘烤'),");
            sb.Append("('03SEC', 800, 'UPLM010', 'ABF 絕緣膠材對壓'),");
            sb.Append("('03SEC', 901, 'UPLM011', 'B-ABF-film 壓合'),");
            sb.Append("('03SEC', 1000, 'UPOV003', 'ABF-film Half Cure'),");
            sb.Append("('03SEC', 1102, 'UPRM001', 'ABF-film 撕耐熱膠帶'),");
            sb.Append("('03SEC', 1200, 'UPOV048', 'ABF film- B面- Pre+Full cure'),");
            sb.Append("('03SEC', 1301, 'UPDR003', 'B-ABF-film 去膠渣-CF4 plasma'),");
            sb.Append("('03SEC', 1500, 'UPCL001', 'RDL0-酸處理'),");
            sb.Append("('03SEC', 1600, 'UPOV006', 'RDL0-烘烤'),");
            sb.Append("('03SEC', 1701, 'UPSP006', 'A-RDL-0-著膜'),");
            sb.Append("('03SEC', 1801, 'UPLM027', 'A-RDL0-乾膜壓合'),");
            sb.Append("('03SEC', 1901, 'UPEX029', 'A-RDL0-A面曝光'),");
            sb.Append("('03SEC', 2001, 'UPEX030', 'A-RDL0-B面全面曝光'),");
            sb.Append("('03SEC', 2100, 'UPOV007', 'RDL0-曝後烤'),");
            sb.Append("('03SEC', 2201, 'UPDE010', 'A-RDL0-顯影'),");
            sb.Append("('03SEC', 2300, 'UPOV008', 'RDL0-顯後烤'),");
            sb.Append("('03SEC', 2501, 'UPPL013', 'A-RDL0-電鍍Cu'),");
            sb.Append("('03SEC', 2601, 'UPDF001', 'RDL0-去膜'),");
            sb.Append("('03SEC', 2701, 'UPET018', 'A-RDL0-Cu蝕刻'),");
            sb.Append("('03SEC', 2801, 'UPET007', 'AB-RDL0-NiCr蝕刻'),");
            sb.Append("('03SEC', 2901, 'UPMS027', 'A-RDL0-尺寸量測'),");
            sb.Append("('03SEC', 2925, 'UPSC049', 'A-RDL0-AOI外觀檢查'),");
            sb.Append("('03SEC', 2950, 'UPSC031', 'RDL-0-外觀檢查'),");
            sb.Append("('03SEC', 3001, 'UPCL012', 'A-ABF-film 酸處理'),");
            sb.Append("('03SEC', 3100, 'UPRO005', 'A-ABF-film 粗化'),");
            sb.Append("('03SEC', 3200, 'UPOV010', 'ABF film- 粗化烘烤'),");
            sb.Append("('03SEC', 3300, 'UPLM002', 'A-ABF-film 壓合'),");
            sb.Append("('03SEC', 3400, 'UPOV057', 'ABF film- A面- Pre+Full cure'),");
            sb.Append("('03SEC', 3500, 'UPLA001', 'VIA-film LASER-打孔'),");
            sb.Append("('03SEC', 3600, 'UPDR004', 'VIA-film Laser 去膠渣-CF4 plasma'),");
            sb.Append("('03SEC', 3701, 'UPCL022', 'A,B Desmear-水平酸洗'),");
            sb.Append("('03SEC', 3750, 'UPO2001', 'A,B-Desmear1-O2 plasma'),");
            sb.Append("('03SEC', 3801, 'UPET016', 'Desmear-Cu蝕刻'),");
            sb.Append("('03SEC', 3850, 'UPSC035', 'A-Desmear-AOI外觀檢查'),");
            sb.Append("('03SEC', 3855, 'UPSC038', 'B-Desmear-2-AOI外觀檢查'),");
            sb.Append("('03SEC', 3901, 'UPMS017', 'Desmear-厚度孔徑量測'),");
            sb.Append("('03SEC', 4000, 'UPSC001', 'Desmear-外觀檢查'),");
            sb.Append("('03SEC', 4100, 'UPQC001', 'QC-1'),");
            sb.Append("('03SEC', 4200, 'UPCL003', 'RDL-1-酸處理'),");
            sb.Append("('03SEC', 4300, 'UPOV012', 'RDL-1-酸洗烘烤'),");
            sb.Append("('03SEC', 4400, 'UPSP003', 'B-RDL-1-著膜'),");
            sb.Append("('03SEC', 4401, 'UPSP002', 'A-RDL-1-著膜'),");
            sb.Append("('03SEC', 4503, 'UPLM035', 'RDL-1-乾膜壓合(雙面)'),");
            sb.Append("('03SEC', 4602, 'UPEX024', 'B-RDL-1-B面曝光'),");
            sb.Append("('03SEC', 4603, 'UPEX023', 'A-RDL-1-A面曝光'),");
            sb.Append("('03SEC', 4800, 'UPOV013', 'RDL-1-曝後烤'),");
            sb.Append("('03SEC', 4902, 'UPDE012', 'RDL-1-顯影(雙面)'),");
            sb.Append("('03SEC', 5001, 'UPOV017', 'RDL-1-顯後烤'),");
            sb.Append("('03SEC', 5100, 'UPO2016', 'RDL-1-雙面O2 plasma'),");
            sb.Append("('03SEC', 5202, 'UPPL018', 'RDL-1-雙面電鍍Cu'),");
            sb.Append("('03SEC', 5301, 'UPDF003', 'RDL-1-去膜'),");
            sb.Append("('03SEC', 5400, 'UPET002', 'A-RDL-1-Cu蝕刻'),");
            sb.Append("('03SEC', 5401, 'UPET003', 'B-RDL-1-Cu蝕刻'),");
            sb.Append("('03SEC', 5501, 'UPET017', 'AB-RDL1-NiCr蝕刻'),");
            sb.Append("('03SEC', 5602, 'UPMS019', 'B-RDL-1-尺寸量測'),");
            sb.Append("('03SEC', 5603, 'UPMS018', 'A-RDL-1-尺寸量測'),");
            sb.Append("('03SEC', 5700, 'UPMS046', 'B-RDL-1-尺寸量測線徑'),");
            sb.Append("('03SEC', 5702, 'UPSC039', 'A-RDL1-AOI外觀檢查'),");
            sb.Append("('03SEC', 5800, 'UPSC047', 'B-RDL-1-AOI外觀檢查'),");
            sb.Append("('03SEC', 5900, 'UPSC023', 'RDL-1-外觀檢查(雙面)'),");
            sb.Append("('03SEC', 7400, 'UPQC002', 'QC-2'),");
            sb.Append("('03SEC', 7900, 'UPCL025', 'Via-2-水平酸洗'),");
            sb.Append("('03SEC', 8001, 'UPRO006', 'B-Via-2-粗化'),");
            sb.Append("('03SEC', 8100, 'UPOV020', 'Via-2-粗化烘烤'),");
            sb.Append("('03SEC', 8201, 'UPPR005', 'B-Via-2-樹脂印刷'),");
            sb.Append("('03SEC', 8300, 'UPOV021', 'Via-2-軟烤'),");
            sb.Append("('03SEC', 8401, 'UPEX018', 'B-Via-2-B面曝光'),");
            sb.Append("('03SEC', 8501, 'UPDE008', 'B-Via-2-顯影'),");
            sb.Append("('03SEC', 8601, 'UPMS012', 'B-Via-2尺寸量測'),");
            sb.Append("('03SEC', 8701, 'UPO2011', 'B-Via-2-O2 plasma'),");
            sb.Append("('03SEC', 8801, 'UPEX019', 'B-Via-2-B面全面曝光'),");
            sb.Append("('03SEC', 8901, 'UPOV044', 'B-Via-2-Oven硬化'),");
            sb.Append("('03SEC', 8950, 'UPSC059', 'B-Via-2-AOI外觀檢查'),");
            sb.Append("('03SEC', 9001, 'UPSC014', 'B-Via-2-外觀檢查'),");
            sb.Append("('03SEC', 9101, 'UPET013', 'B-RDL-2-Cu蝕刻#2'),");
            sb.Append("('03SEC', 9200, 'UPCL006', 'RDL-2-酸處理'),");
            sb.Append("('03SEC', 9300, 'UPOV023', 'RDL-2-酸洗烘烤'),");
            sb.Append("('03SEC', 9401, 'UPSP005', 'B-RDL-2-著膜'),");
            sb.Append("('03SEC', 9502, 'UPLM026', 'B-RDL-2-乾膜壓合'),");
            sb.Append("('03SEC', 9601, 'UPEX020', 'B-RDL-2-B面曝光'),");
            sb.Append("('03SEC', 9702, 'UPEX021', 'B-RDL-2-A面保護曝光'),");
            sb.Append("('03SEC', 9800, 'UPOV024', 'RDL-2-曝後烤'),");
            sb.Append("('03SEC', 9901, 'UPDE009', 'B-RDL-2-顯影'),");
            sb.Append("('03SEC', 10000, 'UPOV025', 'RDL-2-顯後烤'),");
            sb.Append("('03SEC', 10201, 'UPPL010', 'B-RDL-2-電鍍Cu'),");
            sb.Append("('03SEC', 10301, 'UPDF004', 'RDL-2-去膜'),");
            sb.Append("('03SEC', 10401, 'UPET012', 'B-RDL-2-Cu蝕刻#1'),");
            sb.Append("('03SEC', 10501, 'UPET014', 'RDL-2-NiCr蝕刻'),");
            sb.Append("('03SEC', 10602, 'UPMS026', 'B-RDL-2-尺寸量測'),");
            sb.Append("('03SEC', 10650, 'UPSC052', 'B-RDL-2-AOI外觀檢查'),");
            sb.Append("('03SEC', 10701, 'UPSC015', 'B-RDL-2-外觀檢查'),");
            sb.Append("('03SEC', 10800, 'UPCL007', 'A-SM-酸處理'),");
            sb.Append("('03SEC', 10900, 'UPRO003', 'A-SM-粗化'),");
            sb.Append("('03SEC', 11000, 'UPOV026', 'SM-粗化烘烤'),");
            sb.Append("('03SEC', 11100, 'UPPR002', 'A-SM-樹脂印刷'),");
            sb.Append("('03SEC', 11200, 'UPOV027', 'A-SM-軟烤'),");
            sb.Append("('03SEC', 11301, 'UPEX027', 'A-SM-A面曝光'),");
            sb.Append("('03SEC', 11400, 'UPDE006', 'A-SM-顯影'),");
            sb.Append("('03SEC', 11501, 'UPMS022', 'A-SM-尺寸量測'),");
            sb.Append("('03SEC', 11600, 'UPO2008', 'A-SM-O2 plasma'),");
            sb.Append("('03SEC', 11700, 'UPEX012', 'A-SM-A面全面曝光'),");
            sb.Append("('03SEC', 11800, 'UPOV028', 'A-SM-Oven硬化'),");
            sb.Append("('03SEC', 11900, 'UPSC006', 'A-SM-外觀檢查'),");
            sb.Append("('03SEC', 12000, 'UPCL008', 'B-SM-酸處理'),");
            sb.Append("('03SEC', 12100, 'UPRO004', 'B-SM-粗化'),");
            sb.Append("('03SEC', 12200, 'UPOV036', 'B-SM-粗化烘烤'),");
            sb.Append("('03SEC', 12300, 'UPPR003', 'B-SM-樹脂印刷'),");
            sb.Append("('03SEC', 12400, 'UPOV030', 'B-SM-軟烤'),");
            sb.Append("('03SEC', 12500, 'UPEX013', 'B-SM-B面曝光'),");
            sb.Append("('03SEC', 12600, 'UPDE007', 'B-SM-顯影'),");
            sb.Append("('03SEC', 12701, 'UPMS023', 'B-SM-尺寸量測'),");
            sb.Append("('03SEC', 12800, 'UPO2009', 'B-SM-O2 plasma'),");
            sb.Append("('03SEC', 12900, 'UPEX014', 'B-SM-B面全面曝光'),");
            sb.Append("('03SEC', 13000, 'UPOV031', 'B-SM-Oven硬化'),");
            sb.Append("('03SEC', 13100, 'UPSC007', 'B-SM-外觀檢查'),");
            sb.Append("('03SEC', 13200, 'UPET011', 'A、B-Cu蝕刻'),");
            sb.Append("('03SEC', 13300, 'UPCL013', 'B-Cu-酸處理'),");
            sb.Append("('03SEC', 13400, 'UPOV041', 'B-Cu-烘烤'),");
            sb.Append("('03SEC', 13500, 'UPPL015', 'Cu柱雙面電鍍'),");
            sb.Append("('03SEC', 13600, 'UPMS011', 'B-Cu-尺寸量測'),");
            sb.Append("('03SEC', 13700, 'UPMS030', 'A-Cu-尺寸量測'),");
            sb.Append("('03SEC', 13800, 'UPCL028', 'NiAu-水平酸洗'),");
            sb.Append("('03SEC', 13900, 'UPOV033', 'A-NiAu-酸洗烘烤'),");
            sb.Append("('03SEC', 14700, 'UPPL014', '雙面電鍍NiAu'),");
            sb.Append("('03SEC', 14800, 'UPMS036', 'NiAu-厚度量測'),");
            sb.Append("('03SEC', 14900, 'UPSC040', 'A-NiAu-AOI外觀檢查'),");
            sb.Append("('03SEC', 15000, 'UPSC041', 'B-NiAu-AOI外觀檢查'),");
            sb.Append("('03SEC', 15400, 'UPSC008', 'NiAu-外觀檢查'),");
            sb.Append("('03SEC', 15500, 'UPQC003', 'QC-3'),");
            sb.Append("('03SEC', 15600, 'UPCL010', 'SMT-酸處理'),");
            sb.Append("('03SEC', 15700, 'UPOV035', 'SMT-酸洗烘烤'),");
            sb.Append("('03SEC', 15800, 'UPPA003', '黃光-Wafer B面貼膠帶'),");
            sb.Append("('03SEC', 15900, 'UPPR004', 'SMT-Solder Printing'),");
            sb.Append("('03SEC', 16000, 'UPSC010', 'SMT-SPI'),");
            sb.Append("('03SEC', 16100, 'UPST001', 'SMT-Component SMT'),");
            sb.Append("('03SEC', 16200, 'UPRE001', 'SMT-Reflow'),");
            sb.Append("('03SEC', 16300, 'UPSC011', 'SMT-Visual Inspection (AOI)'),");
            sb.Append("('03SEC', 16500, 'UPSL001', '切割-切割'),");
            sb.Append("('03SEC', 16800, 'UPSC012', '切割-晶粒挑檢1'),");
            sb.Append("('03SEC', 17001, 'UPSC016', '切割-外觀檢驗'),");
            sb.Append("('03SEC', 17150, 'UPRE003', 'Tapping Sorting'),");
            sb.Append("('03SEC', 17200, 'UPTE001', '測包-Test/Packing'),");
            sb.Append("('03SEC', 17300, 'UPTE002', '測包-Taping Check'),");
            sb.Append("('03SEC', 17320, 'UPQC009', 'EQA'),");
            sb.Append("('03SEC', 17340, 'UPPK002', '真空包裝'),");
            sb.Append("('03SEC', 17400, 'UPQC008', '(R)FQC/EQA'),");

            // uBMU (完整保留)
            sb.Append("('uBMU', 100, 'UPPA008', 'L/F貼耐熱膠帶'),");
            sb.Append("('uBMU', 200, 'UPBO002', 'L/F粗化'),");
            sb.Append("('uBMU', 300, 'UPOV005', 'L/F粗化烘烤'),");
            sb.Append("('uBMU', 400, 'UPST003', 'SMT_IC'),");
            sb.Append("('uBMU', 500, 'UPMS001', 'D/B-量測'),");
            sb.Append("('uBMU', 600, 'UPOV002', 'ABF壓合預烘'),");
            sb.Append("('uBMU', 700, 'UPLM010', 'ABF 絕緣膠材對壓'),");
            sb.Append("('uBMU', 800, 'UPLM011', 'B-ABF-film 壓合'),");
            sb.Append("('uBMU', 900, 'UPOV003', 'ABF-film Half Cure'),");
            sb.Append("('uBMU', 1000, 'UPRM001', 'ABF-film 撕耐熱膠帶'),");
            sb.Append("('uBMU', 1100, 'UPOV048', 'ABF film- B面- Pre+Full cure'),");
            sb.Append("('uBMU', 1200, 'UPDR011', 'ABF film- Desmear-1'),");
            sb.Append("('uBMU', 1300, 'UPCL012', 'A-ABF-film 酸處理'),");
            sb.Append("('uBMU', 1400, 'UPRO005', 'A-ABF-film 粗化'),");
            sb.Append("('uBMU', 1500, 'UPOV010', 'ABF film- 粗化烘烤'),");
            sb.Append("('uBMU', 1600, 'UPLM002', 'A-ABF-film 壓合'),");
            sb.Append("('uBMU', 1700, 'UPOV057', 'ABF film- A面- Pre+Full cure'),");
            sb.Append("('uBMU', 1800, 'UPLA006', 'A-VIA1-LASER打孔'),");
            sb.Append("('uBMU', 2000, 'UPDR004', 'VIA-film Laser 去膠渣-CF4 plasma'),");
            sb.Append("('uBMU', 2100, 'UPCL022', 'A,B Desmear-水平酸洗'),");
            sb.Append("('uBMU', 2150, 'UPO2001', 'A,B-Desmear1-O2 plasma'),");
            sb.Append("('uBMU', 2200, 'UPET016', 'Desmear-Cu蝕刻'),");
            sb.Append("('uBMU', 2250, 'UPCL033', 'Desmear-外觀前酸洗'),");
            sb.Append("('uBMU', 2275, 'UPOV060', 'Desmear-酸洗烘烤'),");
            sb.Append("('uBMU', 2300, 'UPMS034', 'A-Desmear-厚度孔徑量測'),");
            sb.Append("('uBMU', 2350, 'UPSC035', 'A-Desmear-AOI外觀檢查'),");
            sb.Append("('uBMU', 2400, 'UPSC001', 'Desmear-外觀檢查'),");
            sb.Append("('uBMU', 2500, 'UPQC001', 'QC-1'),");
            sb.Append("('uBMU', 2600, 'UPCL003', 'RDL-1-酸處理'),");
            sb.Append("('uBMU', 2700, 'UPOV012', 'RDL-1-酸洗烘烤'),");
            sb.Append("('uBMU', 2800, 'UPSP002', 'A-RDL-1-著膜'),");
            sb.Append("('uBMU', 2900, 'UPLM005', 'A-RDL-1-乾膜壓合'),");
            sb.Append("('uBMU', 3000, 'UPEX023', 'A-RDL-1-A面曝光'),");
            sb.Append("('uBMU', 3100, 'UPEX004', 'A-RDL-1-B面保護曝光'),");
            sb.Append("('uBMU', 3200, 'UPOV013', 'RDL-1-曝後烤'),");
            sb.Append("('uBMU', 3300, 'UPDE002', 'A-RDL-1-顯影'),");
            sb.Append("('uBMU', 3400, 'UPOV017', 'RDL-1-顯後烤'),");
            sb.Append("('uBMU', 3500, 'UPO2004', 'A-RDL-1-O2 plasma');");

            return sb.ToString();
        }

        private static string GetOrderSeedSql()
        {
            return @"
                INSERT INTO mock_mes_orders 
                (work_no, carrier_id, step_id, next_step_id, prev_out_time, priority_type, due_date, route_id, current_seq_no) VALUES
                ('2P25521UP124', 'P63X31', 'UPET002', 'PN-A', '03SEC', 2, 'UPPA008', 'UPDB002', 2),
                ('2P25407UP111', 'C13D66', 'L002', 'PN-B', '03SEC', 1500, 'UPCL001', 'UPOV006', 0),
                ('2P25521UP166', 'X9D655', 'L003', 'PN-C', '03SEC', 100, 'UPPA008', 'UPBO002', 1);
            ";
        }
        
        private static string GetExtraConfigSeedSql()
        {
            // 預設 Steptime (for existing API compatibility)
            // 簡單對應一些常用 Step
            return @"
                INSERT INTO mock_mes_step_time (step_id, std_time_sec) VALUES 
                ('UPPA008', 300),
                ('UPDB002', 600),
                ('UPMS001', 120),
                ('UPCL001', 300),
                ('UPEX029', 180);
            ";
        }

        private static string GetMesQTimeRuleSeedSql()
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO mock_mes_qtime_rule (step_id, next_step_id, qtime_limit_min) VALUES ");

            // 03SEC 完整製程流程順序
            var stepFlow = new[] 
            {
                "UPPA008", "UPDB002", "UPMS001", "UPBO003", "UPOV076", "UPLM010", "UPLM011", "UPOV003", 
                "UPRM001", "UPOV048", "UPDR003", "UPCL001", "UPOV006", "UPSP006", "UPLM027", "UPEX029", 
                "UPEX030", "UPOV007", "UPDE010", "UPOV008", "UPPL013", "UPDF001", "UPET018", "UPET007", 
                "UPMS027", "UPSC049", "UPSC031", "UPCL012", "UPRO005", "UPOV010", "UPLM002", "UPOV057", 
                "UPLA001", "UPDR004", "UPCL022", "UPO2001", "UPET016", "UPSC035", "UPSC038", "UPMS017", 
                "UPSC001", "UPQC001", "UPCL003", "UPOV012", "UPSP003", "UPSP002", "UPLM035", "UPEX024", 
                "UPEX023", "UPOV013", "UPDE012", "UPOV017", "UPO2016", "UPPL018", "UPDF003", "UPET002", 
                "UPET003", "UPET017", "UPMS019", "UPMS018", "UPMS046", "UPSC039", "UPSC047", "UPSC023", 
                "UPQC002", "UPCL025", "UPRO006", "UPOV020", "UPPR005", "UPOV021", "UPEX018", "UPDE008", 
                "UPMS012", "UPO2011", "UPEX019", "UPOV044", "UPSC059", "UPSC014", "UPET013", "UPCL006", 
                "UPOV023", "UPSP005", "UPLM026", "UPEX020", "UPEX021", "UPOV024", "UPDE009", "UPOV025", 
                "UPPL010", "UPDF004", "UPET012", "UPET014", "UPMS026", "UPSC052", "UPSC015", "UPCL007", 
                "UPRO003", "UPOV026", "UPPR002", "UPOV027", "UPEX027", "UPDE006", "UPMS022", "UPO2008", 
                "UPEX012", "UPOV028", "UPSC006", "UPCL008", "UPRO004", "UPOV036", "UPPR003", "UPOV030", 
                "UPEX013", "UPDE007", "UPMS023", "UPO2009", "UPEX014", "UPOV031", "UPSC007", "UPET011", 
                "UPCL013", "UPOV041", "UPPL015", "UPMS011", "UPMS030", "UPCL028", "UPOV033", "UPPL014", 
                "UPMS036", "UPSC040", "UPSC041", "UPSC008", "UPQC003", "UPCL010", "UPOV035", "UPPA003", 
                "UPPR004", "UPSC010", "UPST001", "UPRE001", "UPSC011", "UPSL001", "UPSC012", "UPSC016", 
                "UPRE003", "UPTE001", "UPTE002", "UPQC009", "UPPK002", "UPQC008"
            };

            // 產生 SQL：每一站 -> 下一站，QTime 限制預設 120 分鐘
            for (int i = 0; i < stepFlow.Length - 1; i++)
            {
                string fromStep = stepFlow[i];
                string toStep = stepFlow[i + 1];
                int limitMin = 120; // 預設 QTime 限制 2 小時

                sb.Append($"('{fromStep}', '{toStep}', {limitMin}),");
            }

            // 移除最後一個逗號並加上分號
            if (sb.Length > 0 && sb[sb.Length - 1] == ',')
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append(";");
            }

            return sb.ToString();
        }
    }
}
