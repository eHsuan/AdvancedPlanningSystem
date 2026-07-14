using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdvancedPlanningSystem.Repositories;
using Protocol.Drivers.Mitsubishi;

namespace AdvancedPlanningSystem.Services
{
    /// <summary>
    /// PLC 門禁與指示燈控制服務
    /// 負責與真實 PLC 進行 MC Protocol 通訊，處理 I/O 點位輪詢、去顫、自動上鎖及三色燈狀態更新。
    /// </summary>
    public class PlcService : IDisposable
    {
        private readonly IApsLocalDbRepository _repo;
        private readonly IPlcDriver _driver;
        private bool _isRunning;
        private Task _pollTask;
        private readonly List<PortRuntimeState> _portStates;
        private bool _toggle; // 用於指示燈閃爍控制

        public bool IsConnected => _driver?.IsConnected ?? false;

        public event EventHandler<PlcScanEventArgs> OnScan;
        public event EventHandler<PlcPickEventArgs> OnPick;

        public PlcService(IApsLocalDbRepository repo, string xmlPath = null)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _driver = new McDriver();

            string path = xmlPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PLC_Adress.xml");
            var configs = LoadPortConfigsFromXml(path);

            _portStates = configs.Select(cfg => new PortRuntimeState
            {
                Config = cfg,
                DebouncedDoor = true,       // 預設門關閉 (true)
                DebouncedPresence = false,  // 預設無卡匣 (false)
                PrevDoor = true,
                PrevPresence = false
            }).ToList();
        }

        private List<PortIoConfig> LoadPortConfigsFromXml(string path)
        {
            if (!File.Exists(path))
            {
                string msg = string.Format("[PLC] XML 位址設定檔不存在: {0}", path);
                LogHelper.Dispatch.Error(msg);
                throw new FileNotFoundException(msg);
            }

            try
            {
                var doc = XDocument.Load(path);
                var configs = doc.Descendants("Port").Select(p => new PortIoConfig
                {
                    PortId = p.Attribute("PortId")?.Value,
                    Index = int.Parse(p.Attribute("Index")?.Value ?? "0"),
                    X_Door = p.Element("X_Door")?.Value,
                    X_Presence = p.Element("X_Presence")?.Value,
                    Y_Red = p.Element("Y_Red")?.Value,
                    Y_Lock = p.Element("Y_Lock")?.Value,
                    Y_Green = p.Element("Y_Green")?.Value
                }).ToList();

                LogHelper.Dispatch.Info(string.Format("[PLC] 成功從 XML 載入 {0} 個 Port 設定資訊。", configs.Count));
                return configs;
            }
            catch (Exception ex)
            {
                string msg = string.Format("[PLC] 解析 XML 設定檔失敗: {0}", path);
                LogHelper.Dispatch.Error(msg, ex);
                throw new InvalidOperationException(msg, ex);
            }
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _pollTask = Task.Run(() => PollLoopAsync());
            LogHelper.Dispatch.Info("[PLC] PlcService Started.");
        }

        public void Stop()
        {
            _isRunning = false;
            try
            {
                _driver?.Disconnect();
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error("[PLC] Disconnect Error during Stop", ex);
            }
            LogHelper.Dispatch.Info("[PLC] PlcService Stopped.");
        }

        private async Task PollLoopAsync()
        {
            while (_isRunning)
            {
                if (!IsConnected && AppConfig.PlcEnabled)
                {
                    try
                    {
                        LogHelper.Dispatch.Info($"[PLC] Connecting to PLC at {AppConfig.PlcIp}:{AppConfig.PlcPort}...");
                        bool connected = await _driver.ConnectAsync(AppConfig.PlcIp, AppConfig.PlcPort);
                        if (connected)
                        {
                            LogHelper.Dispatch.Info("[PLC] Connected to PLC successfully.");
                            await InitializeLightsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Dispatch.Error($"[PLC] Connection failed: {ex.Message}");
                    }

                    if (!IsConnected)
                    {
                        await Task.Delay(5000); // 斷線重連等待 5 秒
                        continue;
                    }
                }

                if (IsConnected && AppConfig.PlcEnabled)
                {
                    try
                    {
                        await PollStatesAsync();
                        await UpdateGlobalAlarmLightsAsync();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Dispatch.Error($"[PLC] Poll error: {ex.Message}");
                        // 若拋出通訊錯誤，呼叫中斷以利下輪重連
                        _driver.Disconnect();
                    }
                }

                await Task.Delay(AppConfig.PlcPollIntervalMs);
            }
        }

        /// <summary>
        /// 初始化所有門鎖與指示燈狀態 (全部紅燈亮，綠燈滅，上鎖)
        /// </summary>
        private async Task InitializeLightsAsync()
        {
            try
            {
                foreach (var state in _portStates)
                {
                    await _driver.WriteBitAsync(state.Config.Y_Red, true);
                    await _driver.WriteBitAsync(state.Config.Y_Lock, false);
                    await _driver.WriteBitAsync(state.Config.Y_Green, false);
                    state.IsUnlocked = false;
                    state.DoorOpenedDuringUnlock = false;
                }
                // 初始化全域三色燈
                await _driver.WriteBitAsync("Y000", false); // 紅滅
                await _driver.WriteBitAsync("Y001", false); // 黃滅
                await _driver.WriteBitAsync("Y002", true);  // 綠亮
                await _driver.WriteBitAsync("Y003", false); // 蜂鳴器滅
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error("[PLC] Failed to initialize lights", ex);
            }
        }

        /// <summary>
        /// 輪詢各 Port 的 I/O 點位，執行去顫與狀態邊緣檢查
        /// </summary>
        private async Task PollStatesAsync()
        {
            foreach (var state in _portStates)
            {
                // 1. 門檢輪詢 (X_Door)
                bool rawDoor = await _driver.ReadBitAsync(state.Config.X_Door);
                if (rawDoor != state.DebouncedDoor)
                {
                    state.DoorConfirmCount++;
                    if (state.DoorConfirmCount >= 3)
                    {
                        state.DebouncedDoor = rawDoor;
                        state.DoorConfirmCount = 0;
                        LogHelper.Dispatch.Debug($"[PLC] Port {state.Config.PortId} Door debounced to: {(rawDoor ? "Closed" : "Opened")}");
                    }
                }
                else
                {
                    state.DoorConfirmCount = 0;
                }

                // 2. 在席輪詢 (X_Presence)
                bool rawPresence = await _driver.ReadBitAsync(state.Config.X_Presence);
                if (rawPresence != state.DebouncedPresence)
                {
                    state.PresenceConfirmCount++;
                    if (state.PresenceConfirmCount >= 3)
                    {
                        state.DebouncedPresence = rawPresence;
                        state.PresenceConfirmCount = 0;
                        LogHelper.Dispatch.Debug($"[PLC] Port {state.Config.PortId} Presence debounced to: {(rawPresence ? "Present" : "Empty")}");
                    }
                }
                else
                {
                    state.PresenceConfirmCount = 0;
                }

                // 3. 事件觸發與自動上鎖
                // 在席 0 -> 1 (入庫偵測)
                if (state.DebouncedPresence && !state.PrevPresence)
                {
                    _ = Task.Run(() => HandleScanArrivalAsync(state));
                }
                // 在席 1 -> 0 (出庫偵測)
                else if (!state.DebouncedPresence && state.PrevPresence)
                {
                    OnPick?.Invoke(this, new PlcPickEventArgs { PortID = state.Config.PortId });
                }

                // 4. 自動上鎖控制
                if (state.IsUnlocked)
                {
                    // 偵測門打開 (DebouncedDoor == false 代表開門)
                    if (!state.DebouncedDoor)
                    {
                        state.DoorOpenedDuringUnlock = true;
                    }

                    // 門曾打開且現在重新關上 -> 執行自動上鎖
                    if (state.DoorOpenedDuringUnlock && state.DebouncedDoor)
                    {
                        LogHelper.Dispatch.Info($"[PLC] Port {state.Config.PortId} door closed. Activating auto-lock.");
                        await LockDoorInternalAsync(state);
                    }
                    // 防呆：解鎖逾時重新鎖上 (15秒未開門)
                    else if ((DateTime.Now - state.UnlockTime).TotalSeconds > 15.0)
                    {
                        LogHelper.Dispatch.Warn($"[PLC] Port {state.Config.PortId} unlock timeout (15s). Relocking.");
                        await LockDoorInternalAsync(state);
                    }
                }

                state.PrevDoor = state.DebouncedDoor;
                state.PrevPresence = state.DebouncedPresence;
            }
        }

        /// <summary>
        /// 異步讀取 PLC 條碼暫存器並觸發 OnScan
        /// </summary>
        private async Task HandleScanArrivalAsync(PortRuntimeState state)
        {
            string barcode = "";
            try
            {
                string address = AppConfig.PlcBarcodeBaseAddress;
                if (!string.IsNullOrEmpty(address))
                {
                    if (address.StartsWith("D") && int.TryParse(address.Substring(1), out int baseAddr))
                    {
                        string portAddr = "D" + (baseAddr + state.Config.Index * 20);
                        barcode = await _driver.ReadAsciiAsync(portAddr, 20);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[PLC] Read Barcode Error for {state.Config.PortId}", ex);
            }

            if (string.IsNullOrEmpty(barcode))
            {
                barcode = $"CST-{state.Config.PortId}-{DateTime.Now:yyyyMMddHHmmss}";
                LogHelper.Dispatch.Info($"[PLC] Read Barcode empty/failed. Auto-generated mock: {barcode}");
            }
            else
            {
                LogHelper.Dispatch.Info($"[PLC] Read Barcode success for {state.Config.PortId}: {barcode}");
            }

            OnScan?.Invoke(this, new PlcScanEventArgs { PortID = state.Config.PortId, Barcode = barcode });
        }

        /// <summary>
        /// 解鎖指定 Port 的門鎖
        /// </summary>
        public async Task UnlockDoorAsync(string portId)
        {
            var state = _portStates.FirstOrDefault(s => s.Config.PortId == portId);
            if (state == null) return;

            if (!IsConnected)
            {
                LogHelper.Dispatch.Warn($"[PLC] Cannot unlock door for {portId}. PLC offline.");
                return;
            }

            try
            {
                LogHelper.Dispatch.Info($"[PLC] Command: Unlock port {portId}");
                state.UnlockTime = DateTime.Now;
                state.DoorOpenedDuringUnlock = false;
                state.IsUnlocked = true;

                await _driver.WriteBitAsync(state.Config.Y_Lock, true);   // 門鎖 ON (解鎖)
                await _driver.WriteBitAsync(state.Config.Y_Red, false);   // 紅燈滅
                await _driver.WriteBitAsync(state.Config.Y_Green, true);  // 綠燈亮
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[PLC] Unlock error for {portId}", ex);
            }
        }

        /// <summary>
        /// 執行上鎖的內部方法
        /// </summary>
        private async Task LockDoorInternalAsync(PortRuntimeState state)
        {
            try
            {
                await _driver.WriteBitAsync(state.Config.Y_Lock, false); // 門鎖 OFF (上鎖)
                await _driver.WriteBitAsync(state.Config.Y_Red, true);   // 紅燈 ON (上鎖狀態)
                await _driver.WriteBitAsync(state.Config.Y_Green, false); // 綠燈 OFF
                state.IsUnlocked = false;
                state.DoorOpenedDuringUnlock = false;
                LogHelper.Dispatch.Info($"[PLC] Port {state.Config.PortId} auto-locked.");
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[PLC] Lock error for {state.Config.PortId}", ex);
            }
        }

        /// <summary>
        /// 根據系統狀態更新全域三色燈與警報蜂鳴器 (Y000 ~ Y003)
        /// </summary>
        private async Task UpdateGlobalAlarmLightsAsync()
        {
            try
            {
                _toggle = !_toggle;
                bool hasHold = _repo.GetActivePorts().Any(p => p.IsHold == 1);

                if (hasHold)
                {
                    // 警報狀態：紅燈閃爍，黃綠滅，蜂鳴器響
                    await _driver.WriteBitAsync("Y000", _toggle); // 紅燈
                    await _driver.WriteBitAsync("Y001", false);   // 黃燈
                    await _driver.WriteBitAsync("Y002", false);   // 綠燈
                    await _driver.WriteBitAsync("Y003", _toggle); // 蜂鳴器
                }
                else
                {
                    // 檢查是否有任何 Port 正在派送中 (IsUnlocked)
                    bool hasDispatching = _portStates.Any(s => s.IsUnlocked);
                    if (hasDispatching)
                    {
                        // 派送中：綠燈閃爍，紅黃滅，蜂鳴器滅
                        await _driver.WriteBitAsync("Y000", false);
                        await _driver.WriteBitAsync("Y001", false);
                        await _driver.WriteBitAsync("Y002", _toggle); // 綠燈閃爍
                        await _driver.WriteBitAsync("Y003", false);
                    }
                    else
                    {
                        // 正常運作：綠燈常亮，紅黃滅，蜂鳴器滅
                        await _driver.WriteBitAsync("Y000", false);
                        await _driver.WriteBitAsync("Y001", false);
                        await _driver.WriteBitAsync("Y002", true);    // 綠燈常亮
                        await _driver.WriteBitAsync("Y003", false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error("[PLC] Failed to update global alarm lights", ex);
            }
        }

        internal List<PortRuntimeState> PortStates => _portStates;

        public async Task WriteBitAsync(string address, bool value)
        {
            if (_driver == null) return;
            await _driver.WriteBitAsync(address, value);
        }

        public async Task<bool> ReadBitAsync(string address)
        {
            if (_driver == null) return false;
            return await _driver.ReadBitAsync(address);
        }

        public void Dispose()
        {
            Stop();
        }
    }

    #region 輔助結構定義

    public class PortIoConfig
    {
        public string PortId { get; set; }
        public int Index { get; set; }
        public string X_Door { get; set; }
        public string X_Presence { get; set; }
        public string Y_Red { get; set; }
        public string Y_Lock { get; set; }
        public string Y_Green { get; set; }
    }

    internal class PortRuntimeState
    {
        public PortIoConfig Config { get; set; }
        public bool RawDoor { get; set; }
        public bool RawPresence { get; set; }
        public bool DebouncedDoor { get; set; }
        public bool DebouncedPresence { get; set; }
        public int DoorConfirmCount { get; set; }
        public int PresenceConfirmCount { get; set; }
        public bool IsUnlocked { get; set; }
        public bool DoorOpenedDuringUnlock { get; set; }
        public DateTime UnlockTime { get; set; }
        public bool PrevDoor { get; set; }
        public bool PrevPresence { get; set; }
    }

    public class PlcScanEventArgs : EventArgs
    {
        public string PortID { get; set; }
        public string Barcode { get; set; }
    }

    public class PlcPickEventArgs : EventArgs
    {
        public string PortID { get; set; }
    }

    #endregion
}
