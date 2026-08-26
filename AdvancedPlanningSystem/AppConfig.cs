using System;
using System.Linq;

namespace AdvancedPlanningSystem
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
        /// 物料入 PORT 識別模式
        /// </summary>
        public static CarrierInputMode InputMode = CarrierInputMode.WorkOrderOnly;

        // 核心計時器設定
        /// <summary>
        /// 自動同步與派貨決策計時器間隔 (秒)
        /// </summary>
        public static int SyncIntervalSec = 30;

        // 貨架設定
        /// <summary>
        /// Port 總數量
        /// </summary>
        public static int TotalPortCount = 10;

        /// <summary>
        /// 設定 Bypass 停用的 Port 清單 (逗號分隔，例如 "4,6" 或 "P04,P06")
        /// </summary>
        public static string BypassedPorts = "";

        /// <summary>
        /// 判斷指定的 PortId 或 Port 序號是否被 Bypass 停用
        /// </summary>
        public static bool IsPortBypassed(string portId)
        {
            if (string.IsNullOrWhiteSpace(BypassedPorts) || string.IsNullOrWhiteSpace(portId))
                return false;

            var tokens = BypassedPorts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string cleanPortId = portId.Trim().ToUpper();
            string digitsOnly = new string(cleanPortId.Where(char.IsDigit).ToArray()).TrimStart('0');
            if (string.IsNullOrEmpty(digitsOnly)) digitsOnly = "0";

            foreach (var token in tokens)
            {
                string cleanToken = token.Trim().ToUpper();
                string tokenDigits = new string(cleanToken.Where(char.IsDigit).ToArray()).TrimStart('0');
                if (string.IsNullOrEmpty(tokenDigits)) tokenDigits = "0";

                if (cleanPortId == cleanToken || digitsOnly == tokenDigits)
                {
                    return true;
                }
            }

            return false;
        }

        // 資料庫設定
        public static bool UseMockExternalDb = false; // 預設為 true (模擬模式)
        public static string ExternalDbConnectionString = "Server=twtpesqlqa2;Database=DIAEAP;User ID=DIAEAP;Password=D@08!aEAp;";

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
        public static string RealMesUrl = "http://twcynmes01.delta.corp/CyntecDataCenter/service/Eqp/Eqp_Portal.asmx"; // 真實 MES URL (備用)

        /// <summary>
        /// MES 預設查詢機台編號 (避免 WONO 查詢時 EqpNo 為空)
        /// </summary>
        public static string MesDefaultEqpNo = "AC3811";

        public static bool ManualMode = false; // 是否啟用手動決策模式

        public static double DueBaseHours = 240.0; // 交期評分基準 (小時)

        public static double TransportBufferMin = 30.0; // QTIME計算人員搬運緩衝時間 (分鐘)

        // 其他未來可能的設定
        // public static string ConnectionString = "...";

        // PLC 設定
        public static bool PlcEnabled = true;
        public static string PlcIp = "192.168.1.10";
        public static int PlcPort = 6000;
        public static string PlcBarcodeBaseAddress = "D1000";
        public static int PlcPollIntervalMs = 100;

        /// <summary>
        /// Port 門解鎖未開啟之自動重新上鎖逾時時間 (秒)
        /// </summary>
        public static double PlcUnlockTimeoutSec = 60.0;

        /// <summary>
        /// Port 門開啟逾時警報時間 (秒)
        /// </summary>
        public static double PlcDoorOpenAlarmTimeoutSec = 60.0;

        // --- N+2 前瞻派貨與 Q-Time 防護設定 ---
        /// <summary>
        /// 是否啟用 N+2 前瞻派貨與跨站 Q-Time 防護
        /// </summary>
        public static bool EnableLookAheadN2 = false;

        /// <summary>
        /// N+2 最少需處於 RUN/IDLE 狀態的機台數量門檻 (低於此數量則硬阻擋派往 N+1)
        /// </summary>
        public static int N2MinRunEqpCount = 1;

        /// <summary>
        /// N+2 下游擁塞扣分權重 (軟性降權係數)
        /// </summary>
        public static double N2CongestionWeight = 20000.0;
    }
}
