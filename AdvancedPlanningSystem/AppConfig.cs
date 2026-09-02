using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using log4net;

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
        private static readonly ILog Log = LogManager.GetLogger(typeof(AppConfig));

        static AppConfig()
        {
            Load();
        }

        /// <summary>
        /// 物料入 PORT 識別模式
        /// </summary>
        public static CarrierInputMode InputMode { get; set; } = CarrierInputMode.WorkOrderOnly;

        // 核心計時器設定
        /// <summary>
        /// 自動同步與派貨決策計時器間隔 (秒)
        /// </summary>
        public static int SyncIntervalSec { get; set; } = 30;

        // 貨架設定
        /// <summary>
        /// Port 總數量
        /// </summary>
        public static int TotalPortCount { get; set; } = 10;

        /// <summary>
        /// 設定 Bypass 停用的 Port 清單 (逗號分隔，例如 "4,6" 或 "P04,P06")
        /// </summary>
        public static string BypassedPorts { get; set; } = "";

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
        public static bool UseMockExternalDb { get; set; } = false;
        public static string ExternalDbConnectionString { get; set; } = "Server=twtpesqlqa2;Database=DIAEAP;User ID=DIAEAP;Password=D@08!aEAp;";

        // Cloud DB 設定
        public static bool UseCloudDb { get; set; } = false;
        public static string CloudDbConnectionString { get; set; } = "Data Source=APSCloudDB.db;Version=3;";

        // 模擬器 (硬體) 連線設定
        public static bool SimulatorEnabled { get; set; } = true;
        public static string SimulatorIp { get; set; } = "127.0.0.1";
        public static int SimulatorPort { get; set; } = 5000;

        // MES 模擬設定
        public static bool MesMockEnabled { get; set; } = true;
        public static string MesMockUrl { get; set; } = "http://localhost";
        public static int MesMockPort { get; set; } = 9000;
        public static string RealMesUrl { get; set; } = "http://twcynmes01.delta.corp/CyntecDataCenter/service/Eqp/Eqp_Portal.asmx";

        /// <summary>
        /// MES 預設查詢機台編號 (避免 WONO 查詢時 EqpNo 為空)
        /// </summary>
        public static string MesDefaultEqpNo { get; set; } = "AC3811";

        public static bool ManualMode { get; set; } = false;

        public static double DueBaseHours { get; set; } = 240.0;

        public static double TransportBufferMin { get; set; } = 30.0;

        // PLC 設定
        public static bool PlcEnabled { get; set; } = true;
        public static string PlcIp { get; set; } = "192.168.1.10";
        public static int PlcPort { get; set; } = 6000;
        public static string PlcBarcodeBaseAddress { get; set; } = "D1000";
        public static int PlcPollIntervalMs { get; set; } = 100;

        /// <summary>
        /// Port 門解鎖未開啟之自動重新上鎖逾時時間 (秒)
        /// </summary>
        public static double PlcUnlockTimeoutSec { get; set; } = 60.0;

        /// <summary>
        /// Port 門開啟逾時警報時間 (秒)
        /// </summary>
        public static double PlcDoorOpenAlarmTimeoutSec { get; set; } = 60.0;

        // --- N+2 前瞻派貨與 Q-Time 防護設定 ---
        /// <summary>
        /// 是否啟用 N+2 前瞻派貨與跨站 Q-Time 防護
        /// </summary>
        public static bool EnableLookAheadN2 { get; set; } = true;

        /// <summary>
        /// N+2 最少需處於 RUN/IDLE 狀態的機台數量門檻 (低於此數量則硬阻擋派往 N+1)
        /// </summary>
        public static int N2MinRunEqpCount { get; set; } = 1;

        /// <summary>
        /// N+2 下游擁塞扣分權重 (軟性降權係數)
        /// </summary>
        public static double N2CongestionWeight { get; set; } = 20000.0;

        /// <summary>
        /// 從外部 JSON 設定檔 (appsettings.json) 載入系統參數
        /// </summary>
        public static void Load(string explicitPath = null)
        {
            try
            {
                string targetPath = explicitPath;
                if (string.IsNullOrEmpty(targetPath))
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string standardPath = Path.Combine(baseDir, "appsettings.json");
                    if (File.Exists(standardPath))
                    {
                        targetPath = standardPath;
                    }
                    else if (File.Exists("appsettings.json"))
                    {
                        targetPath = "appsettings.json";
                    }
                }

                if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
                {
                    Log.Warn($"[AppConfig] Configuration file not found. Using default in-memory settings.");
                    return;
                }

                string jsonContent = File.ReadAllText(targetPath);
                var root = JObject.Parse(jsonContent);

                // --- General Section ---
                if (root["General"] is JObject general)
                {
                    if (general["InputMode"] != null)
                    {
                        string modeStr = general["InputMode"].ToString();
                        if (Enum.TryParse<CarrierInputMode>(modeStr, true, out var mode))
                            InputMode = mode;
                    }
                    if (general["SyncIntervalSec"] != null) SyncIntervalSec = general["SyncIntervalSec"].Value<int>();
                    if (general["TotalPortCount"] != null) TotalPortCount = general["TotalPortCount"].Value<int>();
                    if (general["BypassedPorts"] != null) BypassedPorts = general["BypassedPorts"].Value<string>() ?? "";
                    if (general["ManualMode"] != null) ManualMode = general["ManualMode"].Value<bool>();
                }

                // --- Database Section ---
                if (root["Database"] is JObject db)
                {
                    if (db["UseMockExternalDb"] != null) UseMockExternalDb = db["UseMockExternalDb"].Value<bool>();
                    if (db["ExternalDbConnectionString"] != null) ExternalDbConnectionString = db["ExternalDbConnectionString"].Value<string>();
                    if (db["UseCloudDb"] != null) UseCloudDb = db["UseCloudDb"].Value<bool>();
                    if (db["CloudDbConnectionString"] != null) CloudDbConnectionString = db["CloudDbConnectionString"].Value<string>();
                }

                // --- Simulator Section ---
                if (root["Simulator"] is JObject sim)
                {
                    if (sim["Enabled"] != null) SimulatorEnabled = sim["Enabled"].Value<bool>();
                    if (sim["Ip"] != null) SimulatorIp = sim["Ip"].Value<string>();
                    if (sim["Port"] != null) SimulatorPort = sim["Port"].Value<int>();
                }

                // --- MES Section ---
                if (root["Mes"] is JObject mes)
                {
                    if (mes["MockEnabled"] != null) MesMockEnabled = mes["MockEnabled"].Value<bool>();
                    if (mes["MockUrl"] != null) MesMockUrl = mes["MockUrl"].Value<string>();
                    if (mes["MockPort"] != null) MesMockPort = mes["MockPort"].Value<int>();
                    if (mes["RealUrl"] != null) RealMesUrl = mes["RealUrl"].Value<string>();
                    if (mes["DefaultEqpNo"] != null) MesDefaultEqpNo = mes["DefaultEqpNo"].Value<string>();
                }

                // --- PLC Section ---
                if (root["Plc"] is JObject plc)
                {
                    if (plc["Enabled"] != null) PlcEnabled = plc["Enabled"].Value<bool>();
                    if (plc["Ip"] != null) PlcIp = plc["Ip"].Value<string>();
                    if (plc["Port"] != null) PlcPort = plc["Port"].Value<int>();
                    if (plc["BarcodeBaseAddress"] != null) PlcBarcodeBaseAddress = plc["BarcodeBaseAddress"].Value<string>();
                    if (plc["PollIntervalMs"] != null) PlcPollIntervalMs = plc["PollIntervalMs"].Value<int>();
                    if (plc["UnlockTimeoutSec"] != null) PlcUnlockTimeoutSec = plc["UnlockTimeoutSec"].Value<double>();
                    if (plc["DoorOpenAlarmTimeoutSec"] != null) PlcDoorOpenAlarmTimeoutSec = plc["DoorOpenAlarmTimeoutSec"].Value<double>();
                }

                // --- Dispatch Section ---
                if (root["Dispatch"] is JObject dispatch)
                {
                    if (dispatch["DueBaseHours"] != null) DueBaseHours = dispatch["DueBaseHours"].Value<double>();
                    if (dispatch["TransportBufferMin"] != null) TransportBufferMin = dispatch["TransportBufferMin"].Value<double>();
                    if (dispatch["EnableLookAheadN2"] != null) EnableLookAheadN2 = dispatch["EnableLookAheadN2"].Value<bool>();
                    if (dispatch["N2MinRunEqpCount"] != null) N2MinRunEqpCount = dispatch["N2MinRunEqpCount"].Value<int>();
                    if (dispatch["N2CongestionWeight"] != null) N2CongestionWeight = dispatch["N2CongestionWeight"].Value<double>();
                }

                Log.Info($"[AppConfig] Successfully loaded configuration from: {targetPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"[AppConfig] Error loading configuration from JSON file: {ex.Message}", ex);
            }
        }
    }
}
