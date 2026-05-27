using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedPlanningSystem.Services
{
    /// <summary>
    /// 負責與外部資料庫 (雲端 SQL Server) 進行互動的服務。
    /// 支援模擬模式與真實 SQL Server 模式。
    /// </summary>
    public class ExternalDataService
    {
        private string _connectionString;
        private Dictionary<string, string> _mockDbTable;

        public ExternalDataService()
        {
            // 讀取設定
            _connectionString = AppConfig.ExternalDbConnectionString;

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
        /// 初始化資料庫
        /// </summary>
        private void InitializeDatabase()
        {
            // 雲端 SQL Server 資料庫由上層 EAP 系統維護，APS 僅進行連線，不在此執行 DDL 建表或寫入預設測試資料。
            LogHelper.Logger.Info("External SQL Server database client initialized.");
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
                // 真實 SQL Server 模式
                return await Task.Run(() => 
                {
                    string result = $"WO-DB-{barcode}"; // 預設值
                    try
                    {
                        using (var conn = new SqlConnection(_connectionString))
                        {
                            conn.Open();
                            string sql = "SELECT WorkOrder FROM EAP_CassetteBind WHERE CassetteID = @barcode";
                            using (var cmd = new SqlCommand(sql, conn))
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
                        LogHelper.Logger.Error($"External SQL Server DB query failed for barcode {barcode}: {ex.Message}", ex);
                        result = "DB_ERROR";
                    }
                    return result;
                });
            }
        }
    }
}