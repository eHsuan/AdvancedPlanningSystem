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
        private IMesService _mesService;
        private AdvancedPlanningSystem.Services.ExternalDataService _externalDb;
        private AdvancedPlanningSystem.Repositories.ApsLocalDbRepository _repo; 
        private AdvancedPlanningSystem.Repositories.ApsCloudDbRepository _cloudRepo; 
        private AdvancedPlanningSystem.Services.DataSyncService _syncService; 
        private AdvancedPlanningSystem.Services.DispatchService _dispatchService; 

        public AdvancedPlanningSystem.Services.DataSyncService SyncService => _syncService;

        private int _currentRows = 0;
        private int _currentCols = 0;

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

            // Bind button events
            this.btnGlobalMonitor.Click += BtnGlobalMonitor_Click;
            this.btnTransitMonitor.Click += (s, e) => new TransitMonitorForm().Show();
            this.btnEqpMonitor.Click += (s, e) => new EqpMonitorForm().Show();
            this.btnSystemTest.Click += (s, e) => new TestForm(_mesService).Show();

            // Start UI Refresh Timer (1s interval)
            var uiTimer = new Timer();
            uiTimer.Interval = 1000;
            uiTimer.Tick += (s, e) => RefreshShelfGrid();
            uiTimer.Start();
        }

        private void RefreshShelfGrid()
        {
            // Read active ports from DB
            var activePorts = _repo.GetActivePorts(); 
            
            // Create lookup dictionary
            var portDict = activePorts.ToDictionary(p => p.PortId, p => p);

            foreach (Control c in tlpShelf.Controls)
            {
                if (c is PortControl portCtrl)
                {
                    if (portDict.ContainsKey(portCtrl.PortID))
                    {
                        var data = portDict[portCtrl.PortID];
                        portCtrl.CassetteID = data.CarrierId ?? "";
                        portCtrl.WorkNo = data.LotId ?? "";
                        portCtrl.TargetEqpId = data.TargetEqpId ?? ""; 
                        
                        if (!string.IsNullOrEmpty(data.DispatchTime))
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
                        portCtrl.CassetteID = "";
                        portCtrl.WorkNo = "";
                        portCtrl.Status = PortStatus.Empty;
                    }
                }
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

            try 
            {
                var qtimes = await _mesService.GetAllQTimeLimitsAsync();
                AddLog($"[MES] QTime settings loaded: {qtimes?.Count ?? 0} items");
            }
            catch (Exception ex)
            {
                AddLog($"[MES] QTime loading failed: {ex.Message}");
            }

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
            
            _tcpServer.OnScan += (s, e) => {
                // [佇列化] 收到訊號後僅放入佇列，不直接處理，確保序列化分配儲位
                _stockInQueue.Enqueue(e);
            };

            _tcpServer.OnPick += async (s, e) => {
                string rawPortId = e.PortID;
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
                        int bufferMin = 30; 
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

            _dispatchService = new AdvancedPlanningSystem.Services.DispatchService(_repo, _cloudRepo, _tcpServer);
            _syncService = new AdvancedPlanningSystem.Services.DataSyncService(_mesService, _repo, _cloudRepo, _dispatchService, lstLog);
            _dispatchService.SetDataSyncService(_syncService);

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
    }
}
