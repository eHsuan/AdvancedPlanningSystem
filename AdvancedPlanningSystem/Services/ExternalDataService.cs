using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedPlanningSystem.Services
{
    /// <summary>
    /// 負責與外部資料庫 (ExternalDB.db) 進行互動的服務。
    /// 支援模擬模式與真實 SQLite 模式。
    /// </summary>
    public class ExternalDataService
    {
        private string _dbPath;
        private string _connectionString;
        private Dictionary<string, string> _mockDbTable;

        public ExternalDataService()
        {
            // 讀取設定
            _connectionString = AppConfig.ExternalDbConnectionString;
            
            // 解析 DB 檔案路徑以檢查是否存在 (從 ConnectionString 簡單解析)
            // 假設格式為 "Data Source=ExternalDB.db;Version=3;"
            var parts = _connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.Trim().StartsWith("Data Source", StringComparison.OrdinalIgnoreCase))
                {
                    var value = part.Split('=')[1].Trim();
                    _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
                    break;
                }
            }

            if (AppConfig.UseMockExternalDb)
            {
                InitializeMockData();
            }
            else
            {
                InitializeDatabase();
            }
        }

        private void InitializeMockData()
        {
            _mockDbTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            // 預填一些測試資料 (Barcode -> WorkNo)
            _mockDbTable.Add("BC-001", "WO-20231027-001");
            _mockDbTable.Add("BC-002", "WO-20231027-002");
            _mockDbTable.Add("CASS-001", "WO-A001");
            _mockDbTable.Add("CASS-123", "WO-SPEED-01");
        }

        /// <summary>
        /// 若真實 DB 不存在，則自動建立並塞入測試資料
        /// </summary>
        private void InitializeDatabase()
        {
            if (string.IsNullOrEmpty(_dbPath)) return;

            // 確保資料庫檔案存在
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // 修正: 統一使用 CstID2WorkNo 作為表名，並使用 IF NOT EXISTS 避免錯誤
                string sql = @"
                    CREATE TABLE IF NOT EXISTS CstID2WorkNo (
                        CstID TEXT PRIMARY KEY, 
                        WorkNo TEXT
                    );
                    
                    -- 檢查是否已有資料，若無則塞入預設測試資料
                    INSERT OR IGNORE INTO CstID2WorkNo (CstID, WorkNo) VALUES ('BC-001', 'WO-REAL-DB-001');
                    INSERT OR IGNORE INTO CstID2WorkNo (CstID, WorkNo) VALUES ('CASS-001', 'WO-REAL-A001');
                    INSERT OR IGNORE INTO CstID2WorkNo (CstID, WorkNo) VALUES ('CASS-123', 'WO-REAL-SPEED');
                ";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 根據 Barcode 查詢工單號碼 (WorkNo)
        /// </summary>
        /// <param name="barcode">卡匣條碼</param>
        /// <returns>工單號碼，若查無資料則回傳 "Unknown"</returns>
        public async Task<string> GetWorkNoByBarcodeAsync(string barcode)
        {
            if (AppConfig.UseMockExternalDb)
            {
                // 模擬模式
                await Task.Delay(50); // 模擬 IO
                if (_mockDbTable.ContainsKey(barcode))
                {
                    return _mockDbTable[barcode];
                }
                return $"WO-MOCK-{barcode}";
            }
            else
            {
                // 真實 SQLite 模式
                return await Task.Run(() => 
                {
                    string result = $"WO-DB-{barcode}"; // 預設值
                    try
                    {
                        using (var conn = new SQLiteConnection(_connectionString))
                        {
                            conn.Open();
                            string sql = "SELECT WorkNo FROM CstID2WorkNo WHERE CstID = @barcode";
                            using (var cmd = new SQLiteCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@barcode", barcode);
                                var val = cmd.ExecuteScalar();
                                if (val != null && val != DBNull.Value)
                                {
                                    result = val.ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DB Error: {ex.Message}");
                        result = "DB_ERROR";
                    }
                    return result;
                });
            }
        }
    }
}