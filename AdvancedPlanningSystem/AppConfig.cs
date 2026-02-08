namespace AdvancedPlanningSystem
{
    public static class AppConfig
    {
        // 核心計時器設定
        /// <summary>
        /// 自動同步與派貨決策計時器間隔 (秒)
        /// </summary>
        public static int SyncIntervalSec = 15;

        // 貨架設定
        /// <summary>
        /// Port 總數量
        /// </summary>
        public static int TotalPortCount = 12;

        // 資料庫設定
        public static bool UseMockExternalDb = false; // 預設為 true (模擬模式)
        public static string ExternalDbConnectionString = $"Data Source=D:\\SourceCode\\CS\\AdvancedPlanningSystem\\ExternalDB.db;Version=3;";

        // Cloud DB 設定
        public static bool UseCloudDb = false; // 預設使用 Local APSCloudDB.db
        public static string CloudDbConnectionString = $"Data Source=D:\\SourceCode\\CS\\AdvancedPlanningSystem\\APSCloudDB.db;Version=3;";

        // 模擬器 (硬體) 連線設定
        public static bool SimulatorEnabled = true; // 是否啟用模擬器連線
        public static string SimulatorIp = "127.0.0.1"; // (若作為 Client 連出時使用，目前作為 Server 監聽通常只需 Port)
        public static int SimulatorPort = 5000; // 監聽埠號

        // MES 模擬設定
        public static bool MesMockEnabled = true; // 是否啟用 MES 模擬器
        public static string MesMockUrl = "http://localhost"; // MES 模擬器位址
        public static int MesMockPort = 9000; // 模擬 MES Port
        public static string RealMesUrl = "http://192.168.1.100:80/api"; // 真實 MES URL (備用)

        public static bool ManualMode = false; // 是否啟用手動決策模式

        public static double DueBaseHours = 240.0; // 交期評分基準 (小時)

        public static double TransportBufferMin = 30.0; // QTIME計算人員搬運緩衝時間 (分鐘)

        // 其他未來可能的設定
        // public static string ConnectionString = "...";
    }
}
