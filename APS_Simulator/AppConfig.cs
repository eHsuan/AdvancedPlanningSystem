using System;
using System.IO;

namespace APSSimulator
{
    public enum CarrierInputMode
    {
        BarcodeBinding = 0, // CassetteID 綁定模式 (刷 CassetteID -> 向外部 DB 查 WorkNo)
        WorkOrderOnly = 1,  // 僅工單模式 (刷 WorkNo -> CassetteID 同 WorkNo)
        Hybrid = 2          // 工單 + CassetteID 混合模式 (輸入 WorkNo 與 CassetteID)
    }

    public static class AppConfig
    {
        /// <summary>
        /// 與主程式同步之物料入 PORT 識別模式 (預設為僅工單模式)
        /// </summary>
        public static CarrierInputMode InputMode = CarrierInputMode.WorkOrderOnly;

        // 外部資料庫設定 (對齊 APS 格式)
        public static string ExternalDbPath = @"D:\SourceCode\CS\AdvancedPlanningSystem\ExternalDB.db";
        
        // 方便取得連線字串
        public static string ExternalDbConnectionString => $"Data Source={ExternalDbPath};Version=3;";

        public static int MesServerPort = 9000;

        public static int ApsClientPort = 5000;

        public static string ApsServerIp = "127.0.0.1";

        // 自動模擬設定
        public static int SimProcessMinSec = 15;

        public static int SimProcessMaxSec = 30;
    }
}