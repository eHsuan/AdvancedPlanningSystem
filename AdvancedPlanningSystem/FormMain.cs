using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using AdvancedPlanningSystem.MES;

namespace AdvancedPlanningSystem
{
    public partial class FormMain : Form
    {
        // Communication components
        private TcpServerModule _tcpServer;
        private AdvancedPlanningSystem.Services.PlcService _plcService;
        private IMesService _mesService;
        private AdvancedPlanningSystem.Services.ExternalDataService _externalDb;
        private AdvancedPlanningSystem.Repositories.ApsLocalDbRepository _repo; 
        private AdvancedPlanningSystem.Repositories.ApsCloudDbRepository _cloudRepo; 
        private AdvancedPlanningSystem.Services.DataSyncService _syncService; 
        private AdvancedPlanningSystem.Services.DispatchService _dispatchService; 

        public AdvancedPlanningSystem.Services.DataSyncService SyncService => _syncService;

        private int _currentRows = 0;
        private int _currentCols = 0;
        private Dictionary<string, PortControl> _portMap = new Dictionary<string, PortControl>();

        // --- 入庫處理佇列 ---
        private ConcurrentQueue<ScanEventArgs> _stockInQueue = new ConcurrentQueue<ScanEventArgs>();
        private bool _isProcessingQueue = false;

        public FormMain()
        {
            InitializeComponent();

            // ... 初始化後啟動佇列處理器 ...
            _isProcessingQueue = true;
            Task.Run(() => ProcessStockInQueueAsync());

            // Initialize Repositories
            _repo = new AdvancedPlanningSystem.Repositories.ApsLocalDbRepository();
            _cloudRepo = new AdvancedPlanningSystem.Repositories.ApsCloudDbRepository();

            // Create barcode scan UI elements programmatically
            Label lblMode = new Label
            {
                Text = "識別模式:",
                ForeColor = Color.White,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold),
                Location = new Point(720, 18),
                AutoSize = true
            };

            ComboBox cbMode = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft JhengHei", 9F),
                Location = new Point(785, 15),
                Width = 140
            };
            cbMode.Items.Add("條碼綁定 (DB)");
            cbMode.Items.Add("僅工單 (WO Only)");
            cbMode.Items.Add("混合模式 (Hybrid)");
            cbMode.SelectedIndex = (int)AppConfig.InputMode;

            Label lblScanTitle = new Label
            {
                Name = "lblScanTitle",
                Text = AppConfig.InputMode == CarrierInputMode.WorkOrderOnly ? "工單掃描:" : (AppConfig.InputMode == CarrierInputMode.Hybrid ? "WO,CST 掃描:" : "CST 條碼掃描:"),
                ForeColor = Color.White,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold),
                Location = new Point(935, 18),
                AutoSize = true
            };

            TextBox txtScanInput = new TextBox
            {
                Name = "txtManualScan",
                Font = new Font("Consolas", 10F),
                Location = new Point(1030, 15),
                Width = 180,
            };
            txtScanInput.KeyDown += TxtScanInput_KeyDown;

            cbMode.SelectedIndexChanged += (s, e) => {
                AppConfig.InputMode = (CarrierInputMode)cbMode.SelectedIndex;
                switch (AppConfig.InputMode)
                {
                    case CarrierInputMode.WorkOrderOnly:
                        lblScanTitle.Text = "工單掃描:";
                        break;
                    case CarrierInputMode.Hybrid:
                        lblScanTitle.Text = "WO,CST 掃描:";
                        break;
                    case CarrierInputMode.BarcodeBinding:
                    default:
                        lblScanTitle.Text = "CST 條碼掃描:";
                        break;
                }
                AddLog($"[Mode Changed] 入庫識別模式切換為: {cbMode.SelectedItem}");
            };

            this.pnlHeader.Controls.Add(lblMode);
            this.pnlHeader.Controls.Add(cbMode);
            this.pnlHeader.Controls.Add(lblScanTitle);
            this.pnlHeader.Controls.Add(txtScanInput);

            // Start Pre-Assign Timeout Checker Timer (1s interval)
            var preAssignTimer = new Timer();
            preAssignTimer.Interval = 1000;
            preAssignTimer.Tick += (s, e) => CheckPreAssignTimeouts();
            preAssignTimer.Start();

            // Bind button events
            this.btnGlobalMonitor.Click += BtnGlobalMonitor_Click;
            this.btnTransitMonitor.Click += (s, e) => new TransitMonitorForm().Show();
            this.btnEqpMonitor.Click += (s, e) => new EqpMonitorForm().Show();
            this.btnSystemTest.Click += (s, e) => new TestForm(_mesService, _plcService).Show();

            // Start UI Refresh Timer (1s interval)
            var uiTimer = new Timer();
            uiTimer.Interval = 1000;
            uiTimer.Tick += (s, e) => RefreshShelfGrid();
            uiTimer.Start();

            this.FormClosing += (s, e) => {
                _plcService?.Dispose();
            };
        }

        private void RefreshShelfGrid()
        {
            try
            {
                tlpShelf.SuspendLayout();

                // 1. 從資料庫讀取 active ports 資料 (包含已佔用及預配中)
                var activePorts = _repo.GetActiveAndReservedPorts(); 
                var portDataDict = activePorts.ToDictionary(p => p.PortId, p => p);

                // 2. 直接遍歷索引中的控制項 (不再遍歷 tlpShelf.Controls)
                foreach (var kvp in _portMap)
                {
                    string portId = kvp.Key;
                    PortControl portCtrl = kvp.Value;

                    if (portDataDict.ContainsKey(portId))
                    {
                        var data = portDataDict[portId];
                        portCtrl.CassetteID = data.CarrierId ?? "";
                        portCtrl.WorkNo = data.LotId ?? "";
                        portCtrl.TargetEqpId = data.TargetEqpId ?? ""; 
                        portCtrl.WaitReason = data.WaitReason ?? "";
                        portCtrl.NextStepId = data.NextStepId ?? "";
                        
                        if (data.Status == "PRE_ASSIGN")
                        {
                            portCtrl.Status = PortStatus.PreAssign;
                            portCtrl.IsFlashing = true; // 預配中閃爍引導
                        }
                        else if (!string.IsNullOrEmpty(data.DispatchTime))
                        {
                            portCtrl.Status = PortStatus.Dispatching;
                            portCtrl.IsFlashing = (data.DispatchScore >= 9000000);
                        }
                        else if (data.IsHold == 1)
                        {
                            portCtrl.Status = PortStatus.Error;
                            portCtrl.IsFlashing = false;
                        }
                        else
                        {
                            portCtrl.Status = (data.NextStepId == "END") ? PortStatus.Finish : PortStatus.Occupied;
                            portCtrl.IsFlashing = (data.DispatchScore >= 9000000);
                        }
                    }
                    else
                    {
                        // 若資料庫中沒有該 Port 的 Occupied 資料，重設為空
                        portCtrl.CassetteID = "";
                        portCtrl.WorkNo = "";
                        portCtrl.Status = PortStatus.Empty;
                    }
                }
            }
            finally
            {
                tlpShelf.ResumeLayout();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (AppConfig.MesMockEnabled)
            {
                var result = MessageBox.Show(
                    "Mock Mode detected.\nDo you want to clear all binding and state data (Reset DB)?", 
                    "System Initialization", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _repo.ClearAllStates();
                    AddLog("System states cleared (User Request).");
                }
            }

            int totalPorts = AppConfig.TotalPortCount;
            int cols = (int)Math.Ceiling(Math.Sqrt(totalPorts));
            int rows = (int)Math.Ceiling((double)totalPorts / cols);

            GenerateShelfGrid(rows, cols, totalPorts);

            if (AppConfig.ManualMode)
            {
                btnManualSync.Visible = true;
                AddLog(">>> [MODE] Manual Decision Mode. Click button to execute each round.");
            }

            InitializeCommunications();
        }

        private void BtnGlobalMonitor_Click(object sender, EventArgs e)
        {
            GlobalMonitorForm monitorForm = new GlobalMonitorForm();
            monitorForm.Show();
        }

        public void GenerateShelfGrid(int rows, int cols, int targetCount)
        {
            _currentRows = rows;
            _currentCols = cols;
            EnableDoubleBuffering(tlpShelf);
            tlpShelf.SuspendLayout();
            _portMap.Clear();
            
            try
            {
                for (int i = tlpShelf.Controls.Count - 1; i >= 0; i--)
                {
                    Control c = tlpShelf.Controls[i];
                    tlpShelf.Controls.RemoveAt(i);
                    c.Dispose();
                }
                
                tlpShelf.ColumnStyles.Clear();
                tlpShelf.RowStyles.Clear();
                tlpShelf.ColumnCount = cols;
                tlpShelf.RowCount = rows;

                float colPercent = 100.0f / cols;
                float rowPercent = 100.0f / rows;

                for (int c = 0; c < cols; c++) tlpShelf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, colPercent));
                for (int r = 0; r < rows; r++) tlpShelf.RowStyles.Add(new RowStyle(SizeType.Percent, rowPercent));

                for (int i = 1; i <= targetCount; i++)
                {
                    PortControl port = new PortControl();
                    port.Dock = DockStyle.Fill;
                    port.PortID = "P" + i.ToString("D2");
                    port.CassetteID = "";
                    port.Status = PortStatus.Empty;
                    port.Click += Port_Click;
                    tlpShelf.Controls.Add(port);
                    
                    // 加入索引
                    _portMap[port.PortID] = port;
                }

                RefreshShelfGrid();
            }
            finally
            {
                tlpShelf.ResumeLayout();
            }
        }

        private void Port_Click(object sender, EventArgs e)
        {
            PortControl port = sender as PortControl;
            if (port != null && !string.IsNullOrEmpty(port.CassetteID))
            {
                CassetteDetailForm detailForm = new CassetteDetailForm(port.CassetteID, port.PortID);
                detailForm.ShowDialog();
            }
        }

        private async void btnManualSync_Click(object sender, EventArgs e)
        {
            if (_syncService == null) return;
            
            btnManualSync.Enabled = false;
            btnManualSync.Text = "Running...";
            
            try
            {
                await _syncService.TriggerManualSyncAsync();
            }
            finally
            {
                btnManualSync.Enabled = true;
                btnManualSync.Text = "Manual Decision";
            }
        }

        private async void InitializeCommunications()
        {
            AddLog("System starting...");

            string mesBaseUrl;
            if (AppConfig.MesMockEnabled)
            {
                mesBaseUrl = $"{AppConfig.MesMockUrl}:{AppConfig.MesMockPort}/api";
                AddLog($"[MODE] Mock Mode - Connecting to external simulator: {mesBaseUrl}");
            }
            else
            {
                mesBaseUrl = AppConfig.RealMesUrl;
                AddLog($"[MODE] Production Mode - Connecting to REAL MES: {mesBaseUrl}");
            }

            _mesService = new MesHttpClient(mesBaseUrl);
            AddLog($"MES Client Initialized: {mesBaseUrl}");

            _externalDb = new AdvancedPlanningSystem.Services.ExternalDataService();
            _tcpServer = new TcpServerModule();
            
            _tcpServer.OnConnected += (s, e) => {
                AddLog("Hardware Simulator Connected");
                this.Invoke(new Action(() => {
                    pnlSimStatus.BackColor = Color.LimeGreen;
                    lblSimStatus.Text = "Simulator Online";
                }));
            };
            
            _tcpServer.OnDisconnected += (s, e) => {
                AddLog("Hardware Simulator Disconnected");
                this.Invoke(new Action(() => {
                    pnlSimStatus.BackColor = Color.Red;
                    lblSimStatus.Text = "Simulator Offline";
                }));
            };
            
            _tcpServer.OnScan += async (s, e) => {
                // 將模擬器的 SCAN 訊息重導向至人員手動掃碼邏輯
                await ProcessManualScanInputAsync(e.Barcode, e.PortID);
            };

            _tcpServer.OnPlace += async (s, e) => {
                await HandlePresenceSensorTriggerAsync(e.PortID);
            };

            _tcpServer.OnPick += async (s, e) => {
                await HandleCarrierPickupAsync(e.PortID);
            };

            _tcpServer.OnEnterEqp += async (s, e) => {
                AddLog($"[ENTER] Eqp: {e.EqpID}. Checking for transit completion...");
                // 收到進入機台訊號，立即觸發 Transit 移除檢查 (根據 MES 最新狀態)
                await _syncService.CheckAndRemoveArrivedTransitsAsync();
            };

            if (AppConfig.SimulatorEnabled)
            {
                _tcpServer.Start(AppConfig.SimulatorPort);
                AddLog($"TCP Server Started (Port: {AppConfig.SimulatorPort}). Waiting for simulator...");
            }
            else
            {
                AddLog("TCP Server Disabled.");
                pnlSimStatus.BackColor = Color.Gray;
                lblSimStatus.Text = "Simulator Disabled";
            }

            // 初始化與啟動 PLC 控制服務
            _plcService = new AdvancedPlanningSystem.Services.PlcService(_repo);

            _plcService.OnConfirmArrival += (s, e) => {
                this.Invoke(new Action(() => {
                    AddLog($"[IN] Port: {e.PortID}, Barcode: {e.CarrierID} 自動確認入庫");
                    RefreshShelfGrid();
                }));
            };

            _plcService.OnInvalidPresence += (s, e) => {
                this.Invoke(new Action(() => {
                    AddLog($"[ALARM] Port: {e.PortID} 偵測到非預期置入或放錯儲位！已啟動警報！");
                    NotificationForm.ShowAsync("位置錯置警報", $"儲位 {e.PortID} 偵測到非預期的卡匣置入！請即刻確認！", NotificationLevel.Critical, 10);
                }));
            };

            _plcService.OnPick += async (s, e) => {
                await HandleCarrierPickupAsync(e.PortID);
            };

            if (AppConfig.PlcEnabled)
            {
                _plcService.Start();
                AddLog("PLC Control Service Enabled and Started.");
            }
            else
            {
                AddLog("PLC Control Service Disabled.");
            }

            _dispatchService = new AdvancedPlanningSystem.Services.DispatchService(_repo, _cloudRepo, _tcpServer, _plcService);
            _syncService = new AdvancedPlanningSystem.Services.DataSyncService(_mesService, _repo, _cloudRepo, _dispatchService, lstLog);
            _dispatchService.SetDataSyncService(_syncService);

            // 背景非同步載入 MES 點位設定限制，避免阻塞其它本機服務初始化
            try 
            {
                var qtimes = await _mesService.GetAllQTimeLimitsAsync();
                AddLog($"[MES] QTime settings loaded: {qtimes?.Count ?? 0} items");
            }
            catch (Exception ex)
            {
                AddLog($"[MES] QTime loading failed: {ex.Message}");
            }

            _syncService.Start(); 
        }

        private async Task ProcessStockInQueueAsync()
        {
            while (_isProcessingQueue)
            {
                if (_stockInQueue.TryDequeue(out var e))
                {
                    try
                    {
                        string portId = e.PortID;
                        string cstId = e.Barcode;

                        // --- [序列化分配邏輯] ---
                        // 在此迴圈中處理，保證一次只有一筆請求在查詢與佔用空位
                        if (string.IsNullOrEmpty(portId))
                        {
                            var activePorts = _repo.GetActivePorts().Select(p => p.PortId).ToList();
                            for (int i = 1; i <= AppConfig.TotalPortCount; i++)
                            {
                                string candidate = "P" + i.ToString("D2");
                                if (!activePorts.Contains(candidate))
                                {
                                    portId = candidate;
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(portId))
                            {
                                AddLog($"[ALARM] 貨架已滿，無法為 {cstId} 分配儲位");
                                NotificationForm.ShowAsync("貨架滿載", $"貨架已滿，無法入庫 {cstId}", NotificationLevel.Warning, 5);
                                continue;
                            }

                            await _tcpServer.SendCommand($"ASSIGNED_PORT,{portId},{cstId}");
                            AddLog($"[Auto Assign] 卡匣 {cstId} 分配至 {portId}");
                        }
                        else
                        {
                            // 手動指定檢查
                            int pNum;
                            if (int.TryParse(portId, out pNum)) portId = "P" + pNum.ToString("D2");

                            var existing = _repo.GetActivePorts().FirstOrDefault(p => p.PortId == portId);
                            if (existing != null)
                            {
                                string msg = $"Port {portId} 已有卡匣 {existing.CarrierId}，拒絕入庫 {cstId}";
                                AddLog($"[ALARM] {msg}");
                                NotificationForm.ShowAsync("碰撞警報", msg, NotificationLevel.Critical, 5);
                                continue;
                            }
                        }

                        // 執行入庫
                        string workNo = await _externalDb.GetWorkNoByBarcodeAsync(cstId);
                        UpdatePortStatus(portId, cstId, workNo, PortStatus.Occupied);
                        _repo.HandleScanArrival(portId, cstId, workNo);
                        
                        AddLog($"[IN] Port: {portId}, Barcode: {cstId} 處理完成");
                    }
                    catch (Exception ex)
                    {
                        AddLog($"[Queue Error] 處理入庫要求時發生錯誤: {ex.Message}");
                    }
                }
                else
                {
                    // 佇列為空時稍作休息，避免佔用 CPU
                    await Task.Delay(100);
                }
            }
        }

        private void AddLog(string msg)
        {
            string log = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => AddLog(msg)));
                return;
            }
            lstLog.Items.Add(log); 
            if (lstLog.Items.Count > 100) lstLog.Items.RemoveAt(0); 
            lstLog.TopIndex = lstLog.Items.Count - 1; 
        }

        private void UpdatePortStatus(string portId, string cassetteId, string workNo, PortStatus status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdatePortStatus(portId, cassetteId, workNo, status)));
                return;
            }

            foreach (Control c in tlpShelf.Controls)
            {
                if (c is PortControl port && port.PortID == portId)
                {
                    port.CassetteID = cassetteId;
                    port.WorkNo = workNo;
                    port.Status = status;
                    break;
                }
            }
        }

        private void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered", 
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, 
                null, 
                control, 
                new object[] { true });
        }

        private async Task HandleCarrierPickupAsync(string rawPortId)
        {
            string portId = rawPortId;
            int pNum;
            if (int.TryParse(rawPortId, out pNum)) portId = "P" + pNum.ToString("D2");

            AddLog($"[PICK] Port: {portId}");

            var portState = _repo.GetActivePorts().FirstOrDefault(p => p.PortId == portId);
            if (portState != null && !string.IsNullOrEmpty(portState.CarrierId))
            {
                var binding = _repo.GetBinding(portState.CarrierId);
                if (binding != null)
                {
                    int bufferMin = (int)AppConfig.TransportBufferMin; 
                    var transit = new AdvancedPlanningSystem.Models.StateTransit
                    {
                        CarrierId = binding.CarrierId,
                        LotId = binding.LotId,
                        TargetEqpId = binding.TargetEqpId,
                        NextStepId = binding.NextStepId,
                        DispatchTime = binding.DispatchTime ?? DateTime.Now.ToString("yyyyMMddHHmmss"),
                        PickupTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        ExpectedArrivalTime = DateTime.Now.AddMinutes(bufferMin).ToString("yyyyMMddHHmmss"),
                        IsOverdue = 0
                    };

                    _repo.MoveToTransit(transit);
                    AddLog($"[Transit] {binding.CarrierId} picked up, expected arrival in {bufferMin} mins.");
                }
            }
            UpdatePortStatus(portId, "", "", PortStatus.Empty);
            _repo.UpdatePortStateOnly(portId, "EMPTY");
            await Task.CompletedTask;
        }

        private async void TxtScanInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox txt = sender as TextBox;
                if (txt == null) return;
                
                string barcode = txt.Text.Trim();
                txt.Clear();
                
                if (string.IsNullOrEmpty(barcode)) return;

                // 防呆：單件交易鎖定
                bool hasReserved = _repo.GetActiveAndReservedPorts().Any(p => p.Status == "PRE_ASSIGN");
                if (hasReserved)
                {
                    AddLog($"[ALARM] 拒絕掃碼 {barcode}：目前已有儲位正在引導置入中！");
                    NotificationForm.ShowAsync("交易鎖定", "目前已有儲位正在引導置入中，請先完成前一卡匣的置放與關門！", NotificationLevel.Warning, 5);
                    return;
                }

                await ProcessManualScanInputAsync(barcode);
            }
        }

        private async Task ProcessManualScanInputAsync(string inputData, string preferredPortId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputData)) return;

                string barcode = "";
                string workNo = "";

                switch (AppConfig.InputMode)
                {
                    case CarrierInputMode.WorkOrderOnly:
                        // 1. 僅工單模式：人員輸入即為工單號碼，CassetteID 預設同 WorkNo
                        workNo = inputData.Trim();
                        barcode = workNo;
                        AddLog($"[Scan-WorkOrderOnly] 收到工單號碼: {workNo}");
                        break;

                    case CarrierInputMode.Hybrid:
                        // 2. 混合模式：支援 "WorkNo,CassetteID" 或 "CassetteID,WorkNo" 拆解
                        AddLog($"[Scan-Hybrid] 收到混合輸入: {inputData}");
                        var parts = inputData.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            workNo = parts[0].Trim();
                            barcode = parts[1].Trim();
                        }
                        else if (parts.Length == 1)
                        {
                            workNo = parts[0].Trim();
                            barcode = parts[0].Trim();
                        }
                        else
                        {
                            AddLog($"[ALARM] 混合模式格式錯誤！請提供 '工單號碼,CassetteID'");
                            NotificationForm.ShowAsync("輸入錯誤", "混合模式格式錯誤，格式應為 '工單號碼,CassetteID'", NotificationLevel.Warning, 5);
                            return;
                        }
                        break;

                    case CarrierInputMode.BarcodeBinding:
                    default:
                        // 3. CassetteID 綁定模式：刷 CassetteID 查 WorkNo
                        barcode = inputData.Trim();
                        AddLog($"[Scan-Binding] 收到卡匣條碼: {barcode}");
                        workNo = await _externalDb.GetWorkNoByBarcodeAsync(barcode);
                        if (string.IsNullOrEmpty(workNo))
                        {
                            AddLog($"[ALARM] 外部資料庫找不到條碼 {barcode} 對應的工單！");
                            NotificationForm.ShowAsync("條碼錯誤", $"無效的卡匣條碼: {barcode}", NotificationLevel.Warning, 5);
                            return;
                        }
                        break;
                }

                if (string.IsNullOrEmpty(workNo) || string.IsNullOrEmpty(barcode))
                {
                    AddLog("[ALARM] 工單號碼或卡匣ID不可為空！");
                    return;
                }

                // 2. 分配空 Port (排除 OCCUPIED 與 PRE_ASSIGN)
                var activeAndReserved = _repo.GetActiveAndReservedPorts().Select(p => p.PortId).ToList();
                string portId = preferredPortId;

                if (string.IsNullOrEmpty(portId))
                {
                    for (int i = 1; i <= AppConfig.TotalPortCount; i++)
                    {
                        string candidate = "P" + i.ToString("D2");
                        if (!activeAndReserved.Contains(candidate))
                        {
                            portId = candidate;
                            break;
                        }
                    }
                }
                else
                {
                    // 手動指定檢查
                    int pNum;
                    if (int.TryParse(portId, out pNum)) portId = "P" + pNum.ToString("D2");
                    if (activeAndReserved.Contains(portId))
                    {
                        AddLog($"[ALARM] 指定的 Port {portId} 已被佔用或預配中，無法分配給 {barcode}");
                        NotificationForm.ShowAsync("分配失敗", $"Port {portId} 已被佔用或預配中", NotificationLevel.Warning, 5);
                        return;
                    }
                }

                if (string.IsNullOrEmpty(portId))
                {
                    AddLog($"[ALARM] 貨架已滿，無法為 {barcode} 分配儲位");
                    NotificationForm.ShowAsync("貨架滿載", $"貨架已滿，無法預配儲位", NotificationLevel.Warning, 5);
                    return;
                }

                // 3. 資料庫標記預配
                _repo.PreAssignPort(portId, barcode, workNo);
                AddLog($"[Pre-Assign] 卡匣 {barcode} (工單: {workNo}) 預配至 {portId}，引導開門...");

                // 4. 引導開門與閃綠燈
                if (AppConfig.PlcEnabled && _plcService != null)
                {
                    var state = _plcService.PortStates.FirstOrDefault(s => s.Config.PortId == portId);
                    if (state != null)
                    {
                        // 門解鎖
                        await _plcService.UnlockDoorAsync(portId);
                        // 綠燈亮起引導置入
                        await _plcService.WriteBitAsync(state.Config.Y_Green, true);
                        // 紅燈熄滅
                        await _plcService.WriteBitAsync(state.Config.Y_Red, false);
                    }
                }
                else
                {
                    // TCP 模擬器模式：發送 OPEN 開門指令 (附帶卡匣 ID 供模擬器對照)
                    await _tcpServer.SendCommand($"OPEN,{portId},STOCK,{barcode}");
                }

                RefreshShelfGrid();
            }
            catch (Exception ex)
            {
                AddLog($"[Scan Error] 處理輸入 {inputData} 時發生錯誤: {ex.Message}");
            }
        }

        private void CheckPreAssignTimeouts()
        {
            try
            {
                var reservedPorts = _repo.GetActiveAndReservedPorts().Where(p => p.Status == "PRE_ASSIGN").ToList();
                foreach (var port in reservedPorts)
                {
                    // 檢查 last_update，若超過 15 秒且門已關閉或根本沒開，則執行逾時還原
                    if (DateTime.TryParseExact(port.LastUpdate, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime lastUpdate))
                    {
                        if ((DateTime.Now - lastUpdate).TotalSeconds > 15.0)
                        {
                            // 檢查門狀態：若 PLC 啟動，需確認沒有人把門開著才上鎖
                            bool isDoorOpen = false;
                            if (AppConfig.PlcEnabled && _plcService != null)
                            {
                                var state = _plcService.PortStates.FirstOrDefault(s => s.Config.PortId == port.PortId);
                                if (state != null)
                                {
                                    isDoorOpen = !state.DebouncedDoor; // DebouncedDoor = false 代表開門
                                }
                            }

                            if (!isDoorOpen)
                            {
                                AddLog($"[Timeout] Port {port.PortId} 預配逾時 (15秒)，還原儲位並上鎖...");
                                _repo.CancelPreAssignPort(port.PortId);

                                if (AppConfig.PlcEnabled && _plcService != null)
                                {
                                    var state = _plcService.PortStates.FirstOrDefault(s => s.Config.PortId == port.PortId);
                                    if (state != null)
                                    {
                                        // 關閉綠燈，上鎖門，點亮紅燈
                                        _ = Task.Run(async () => {
                                            await _plcService.WriteBitAsync(state.Config.Y_Green, false);
                                            await _plcService.WriteBitAsync(state.Config.Y_Lock, false); // Lock OFF
                                            await _plcService.WriteBitAsync(state.Config.Y_Red, true);
                                        });
                                    }
                                }
                                RefreshShelfGrid();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("CheckPreAssignTimeouts Error", ex);
            }
        }

        private async Task HandlePresenceSensorTriggerAsync(string portId)
        {
            // 模擬器 PLACE 事件處理 (相當於 PLC 中 X_Presence=1 且 X_Door=true 的結果)
            int pNum;
            if (int.TryParse(portId, out pNum)) portId = "P" + pNum.ToString("D2");

            var binding = _repo.GetBindingByPort(portId);
            if (binding != null)
            {
                AddLog($"[Simulator PLACE] Port {portId} 確認卡匣 {binding.CarrierId} 置入，入庫上鎖");
                _repo.ConfirmPortArrival(portId);
                RefreshShelfGrid();
            }
            else
            {
                string msg = $"Port {portId} 偵測到模擬置入，但該儲位並非預配狀態！已自動報警！";
                AddLog($"[ALARM] {msg}");
                NotificationForm.ShowAsync("位置錯置警報", msg, NotificationLevel.Critical, 5);
            }
            await Task.CompletedTask;
        }
    }
}
