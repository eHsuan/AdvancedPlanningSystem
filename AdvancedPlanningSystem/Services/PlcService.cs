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
    public class PlcConfirmArrivalEventArgs : EventArgs
    {
        public string PortID { get; set; }
        public string CarrierID { get; set; }
    }

    public class PlcInvalidPresenceEventArgs : EventArgs
    {
        public string PortID { get; set; }
    }

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
        public event EventHandler<PlcConfirmArrivalEventArgs> OnConfirmArrival; // 新增：確認入庫置入事件
        public event EventHandler<PlcInvalidPresenceEventArgs> OnInvalidPresence; // 新增：無效/放錯儲位警報事件

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
                        
                        // 門開啟邊緣觸發：記錄開門起算時間，重設警報
                        if (!rawDoor)
                        {
                            state.DoorOpenStartTime = DateTime.Now;
                            state.IsDoorOpenAlarmActive = false;
                            state.DoorOpenedDuringUnlock = true;
                        }
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

                // 3. 複合感測互鎖狀態機判定 (Door + Presence)
                bool isInterlocked = state.DebouncedDoor && state.DebouncedPresence; // 門已關且在席有貨
                bool wasInterlocked = state.PrevDoor && state.PrevPresence;

                if (isInterlocked && !wasInterlocked)
                {
                    // 複合互鎖成立 -> 執行防呆置入與上鎖判定
                    _ = Task.Run(() => ConfirmPlacementAndLockAsync(state));
                }
                else if (state.DebouncedDoor && !state.PrevDoor && !state.DebouncedPresence)
                {
                    // 門關上但在席無貨 -> 執行空門關閉與預配取消判定
                    _ = Task.Run(() => HandleEmptyDoorCloseAsync(state));
                }

                // 在席 1 -> 0 (出庫取走偵測)
                if (!state.DebouncedPresence && state.PrevPresence)
                {
                    OnPick?.Invoke(this, new PlcPickEventArgs { PortID = state.Config.PortId });
                }

                // 4. 解鎖自動逾時上鎖 (門未開逾時 15 秒重新鎖上)
                if (state.IsUnlocked && !state.DoorOpenedDuringUnlock)
                {
                    if ((DateTime.Now - state.UnlockTime).TotalSeconds > 15.0)
                    {
                        LogHelper.Dispatch.Warn($"[PLC] Port {state.Config.PortId} unlock timeout (15s, door never opened). Relocking.");
                        await LockDoorInternalAsync(state);
                    }
                }

                // 5. 門未關逾時警報監測 (門開著超過 15 秒)
                if (!state.DebouncedDoor) // 門目前是開著的
                {
                    if ((DateTime.Now - state.DoorOpenStartTime).TotalSeconds > 15.0 && !state.IsDoorOpenAlarmActive)
                    {
                        state.IsDoorOpenAlarmActive = true;
                        LogHelper.Dispatch.Warn($"[PLC ALARM] Port {state.Config.PortId} 門開啟逾時(15秒)！啟動警報提示！");
                        
                        // 啟動間歇蜂鳴器與紅燈閃爍
                        _ = Task.Run(async () =>
                        {
                            while (!state.DebouncedDoor && state.IsDoorOpenAlarmActive && _isRunning)
                            {
                                await _driver.WriteBitAsync("Y003", true); // 鳴叫
                                await _driver.WriteBitAsync(state.Config.Y_Red, true);
                                await Task.Delay(500);
                                await _driver.WriteBitAsync("Y003", false); // 熄滅
                                await _driver.WriteBitAsync(state.Config.Y_Red, false);
                                await Task.Delay(500);
                            }
                            await _driver.WriteBitAsync("Y003", false); // 確保復原
                        });
                    }
                }

                state.PrevDoor = state.DebouncedDoor;
                state.PrevPresence = state.DebouncedPresence;
            }
        }

        private async Task ConfirmPlacementAndLockAsync(PortRuntimeState state)
        {
            try
            {
                state.IsDoorOpenAlarmActive = false;

                // 1. 查詢該 Port 是否有預配的卡匣
                var binding = _repo.GetBindingByPort(state.Config.PortId);
                if (binding != null)
                {
                    // [正確入庫]
                    LogHelper.Dispatch.Info($"[PLC] Port {state.Config.PortId} interlock confirmed. Carrier: {binding.CarrierId}");
                    
                    // A. 更新狀態為 OCCUPIED
                    _repo.ConfirmPortArrival(state.Config.PortId);
                    
                    // B. 電磁閥上鎖 (Lock ON: Y_Lock = false)
                    await LockDoorInternalAsync(state);
                    
                    // C. 綠燈滅，紅燈亮
                    await _driver.WriteBitAsync(state.Config.Y_Green, false);
                    await _driver.WriteBitAsync(state.Config.Y_Red, true);

                    // D. 觸發事件更新主 UI
                    OnConfirmArrival?.Invoke(this, new PlcConfirmArrivalEventArgs { PortID = state.Config.PortId, CarrierID = binding.CarrierId });
                }
                else
                {
                    // [位置錯置警報] 放錯儲位或強行開門置入
                    LogHelper.Dispatch.Warn($"[PLC ALARM] Port {state.Config.PortId} 偵測到置入且關門，但資料庫中無預配記錄！");
                    
                    // A. 電磁閥上鎖防盜 (Lock ON: Y_Lock = false)
                    await LockDoorInternalAsync(state);

                    // B. 啟動警報提示與紅燈閃爍
                    OnInvalidPresence?.Invoke(this, new PlcInvalidPresenceEventArgs { PortID = state.Config.PortId });

                    // C. 鳴叫蜂鳴器 (3秒)
                    await _driver.WriteBitAsync("Y003", true);
                    await Task.Delay(3000);
                    await _driver.WriteBitAsync("Y003", false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[PLC] ConfirmPlacementAndLockAsync Error for {state.Config.PortId}", ex);
            }
        }

        private async Task HandleEmptyDoorCloseAsync(PortRuntimeState state)
        {
            try
            {
                state.IsDoorOpenAlarmActive = false;

                // 檢查該 Port 是否處於預配狀態
                var binding = _repo.GetBindingByPort(state.Config.PortId);
                if (binding != null)
                {
                    // 人員開了門但沒放貨就關門了 -> 逾時或放棄
                    LogHelper.Dispatch.Info($"[PLC] Port {state.Config.PortId} door closed with empty presence. Cancelling pre-assignment.");
                    
                    // A. 取消預配關係，重置為 EMPTY
                    _repo.CancelPreAssignPort(state.Config.PortId);

                    // B. 上鎖門
                    await LockDoorInternalAsync(state);

                    // C. 綠燈滅，紅燈亮
                    await _driver.WriteBitAsync(state.Config.Y_Green, false);
                    await _driver.WriteBitAsync(state.Config.Y_Red, true);

                    // D. 觸發事件刷新 UI
                    OnConfirmArrival?.Invoke(this, new PlcConfirmArrivalEventArgs { PortID = state.Config.PortId, CarrierID = "" });
                }
                else
                {
                    // 一般情況下的關門，直接上鎖確保安全
                    await LockDoorInternalAsync(state);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Dispatch.Error($"[PLC] HandleEmptyDoorCloseAsync Error for {state.Config.PortId}", ex);
            }
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
        
        // 新增門禁逾時與防呆時間欄位
        public DateTime DoorOpenStartTime { get; set; }
        public bool IsDoorOpenAlarmActive { get; set; }
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
