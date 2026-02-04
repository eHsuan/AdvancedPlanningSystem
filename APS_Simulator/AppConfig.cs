using System;
using System.IO;

namespace APSSimulator
{
    public static class AppConfig
    {
        // 外部資料庫設定 (對齊 APS 格式)
        public static string ExternalDbPath = @"D:\SourceCode\CS\AdvancedPlanningSystem\ExternalDB.db";
        
        // 方便取得連線字串
        public static string ExternalDbConnectionString => $"Data Source={ExternalDbPath};Version=3;";

        // 其他模擬器設定 (可依需求擴充)
        public static int MesServerPort = 9000;
        public static int ApsClientPort = 5000;
        public static string ApsServerIp = "127.0.0.1";
    }
}