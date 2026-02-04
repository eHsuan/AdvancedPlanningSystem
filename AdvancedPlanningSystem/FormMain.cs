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
using AdvancedPlanningSystem.MES;

namespace AdvancedPlanningSystem
{
    public partial class FormMain : Form
    {
        // 通訊組件
        private TcpServerModule _tcpServer;
        private IMesService _mesService;
        private MesMockServer _mesMockServer;
        private AdvancedPlanningSystem.Services.ExternalDataService _externalDb;
        private AdvancedPlanningSystem.Repositories.ApsLocalDbRepository _repo; // Repository 負責 DB 存取
        private AdvancedPlanningSystem.Repositories.ApsCloudDbRepository _cloudRepo; // CloudDB 負責歷史紀錄
        private AdvancedPlanningSystem.Services.DataSyncService _syncService; // 資料同步服務
        private AdvancedPlanningSystem.Services.DispatchService _dispatchService; // 派貨決策服務

        // 從 AppConfig 讀取預設配置
        private int _currentRows = 0;
        private int _currentCols = 0;

        public FormMain()
        {
            InitializeComponent();

            // 初始化 Repository
            _repo = new AdvancedPlanningSystem.Repositories.ApsLocalDbRepository();
            _cloudRepo = new AdvancedPlanningSystem.Repositories.ApsCloudDbRepository();

            // 綁定按鈕事件來測試動態切換
            this.btnGlobalMonitor.Click += BtnGlobalMonitor_Click;
            this.btnTransitMonitor.Click += (s, e) => new TransitMonitorForm().Show();
            this.btnEqpMonitor.Click += (s, e) => new EqpMonitorForm().Show();
            this.btnSystemTest.Click += (s, e) => new TestForm(_mesService).Show();

            // 啟動 UI 刷新 Timer (每秒刷新一次 Port 顯示)
            var uiTimer = new Timer();
            uiTimer.Interval = 1000;
            uiTimer.Tick += (s, e) => RefreshShelfGrid();
            uiTimer.Start();
        }

        private void RefreshShelfGrid()
        {
            // 從 DB 讀取所有有狀態的 Port
            var activePorts = _repo.GetActivePorts(); // 包含 CarrierId, LotId, Status
            
            // 建立快速查找字典
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
                        
                        // 轉換狀態字串為 Enum
                        // [Fix] 優先檢查是否為派貨中 (Dispatching)
                        if (!string.IsNullOrEmpty(data.DispatchTime))
                        {
                            portCtrl.Status = PortStatus.Dispatching;
                        }
                        else if (Enum.TryParse(data.Status, true, out PortStatus statusEnum))
                        {
                            portCtrl.Status = statusEnum;
                        }
                        else
                        {
                            portCtrl.Status = PortStatus.Occupied; // Default occupied
                        }
                    }
                    else
                    {
                        // 若 DB 沒資料，視為 Empty
                        portCtrl.CassetteID = "";
                        portCtrl.WorkNo = "";
                        portCtrl.Status = PortStatus.Empty;
                    }
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 初始載入：根據總 Port 數自動計算行列
            int totalPorts = AppConfig.TotalPortCount;
            int cols = (int)Math.Ceiling(Math.Sqrt(totalPorts));
            int rows = (int)Math.Ceiling((double)totalPorts / cols);

            GenerateShelfGrid(rows, cols, totalPorts);

            // 初始化通訊
            InitializeCommunications();
        }

        /// <summary>
        /// 按鈕點擊事件：開啟全局狀態監控視窗
        /// </summary>
        private void BtnGlobalMonitor_Click(object sender, EventArgs e)
        {
            GlobalMonitorForm monitorForm = new GlobalMonitorForm();
            monitorForm.Show();
        }

        /// <summary>
        /// 動態生成貨架網格
        /// </summary>
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

                // 動態生成 PortControl (只生成指定數量)
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

        /// <summary>
        /// Port 點擊事件處理：開啟詳細資訊視窗
        /// </summary>
        private void Port_Click(object sender, EventArgs e)
        {
            PortControl port = sender as PortControl;
            if (port != null && !string.IsNullOrEmpty(port.CassetteID))
            {
                CassetteDetailForm detailForm = new CassetteDetailForm(port.CassetteID, port.PortID);
                detailForm.ShowDialog();
            }
        }

        private async void InitializeCommunications()
        {
            AddLog("系統啟動中...");

            // 1. MES 設定 (現在一律連向外部，不再啟動內部模擬器)
            string mesBaseUrl;

            if (AppConfig.MesMockEnabled)
            {
                // 連向外部的 APS Simulator (扮演 MES 角色)
                mesBaseUrl = $"{AppConfig.MesMockUrl}:{AppConfig.MesMockPort}/api";
                AddLog($"[模式] 模擬模式 - 將連線至外部模擬器: {mesBaseUrl}");
            }
            else
            {
                mesBaseUrl = AppConfig.RealMesUrl;
                AddLog($"[模式] 正式模式 - 將連線至真實 MES: {mesBaseUrl}");
            }

            // 2. 初始化 MES Client
            _mesService = new MesHttpClient(mesBaseUrl);
            AddLog($"MES Client 初始化完成: {mesBaseUrl}");

            // 3. 取得 QTime 設定 (Cache in memory)
            try 
            {
                var qtimes = await _mesService.GetAllQTimeLimitsAsync();
                AddLog($"[MES] 已載入 QTime 設定: {qtimes?.Count ?? 0} 筆");
            }
            catch (Exception ex)
            {
                AddLog($"[MES] QTime 載入失敗: {ex.Message}");
            }

            // 4. 初始化 External DB Service (模擬 SQLite)
            _externalDb = new AdvancedPlanningSystem.Services.ExternalDataService();

            // 5. 初始化 TCP Server (模擬器通訊)
            _tcpServer = new TcpServerModule();
            
            // ... (TCP Event Handlers skipped for brevity, keeping existing code) ...
            // 訂閱連線狀態事件 (UI 更新)
            _tcpServer.OnConnected += (s, e) => {
                AddLog("模擬器硬體已連線");
                this.Invoke(new Action(() => {
                    pnlSimStatus.BackColor = Color.LimeGreen;
                    lblSimStatus.Text = "模擬器已連線";
                }));
            };
            
            _tcpServer.OnDisconnected += (s, e) => {
                AddLog("模擬器硬體連線中斷");
                this.Invoke(new Action(() => {
                    pnlSimStatus.BackColor = Color.Red;
                    lblSimStatus.Text = "模擬器斷線";
                }));
            };
            
            // --- Scan 事件 (入庫) ---
            _tcpServer.OnScan += async (s, e) => {
                // ... (Keep existing Scan logic) ...
                string rawPortId = e.PortID;
                string portId = rawPortId;
                // [Fix] Normalize PortID (e.g., "1" -> "P01")
                int pNum;
                if (int.TryParse(rawPortId, out pNum)) portId = "P" + pNum.ToString("D2");
                
                AddLog($"[SCAN] RawPort: {rawPortId} -> Port: {portId}, Barcode: {e.Barcode}");
                string workNo = await _externalDb.GetWorkNoByBarcodeAsync(e.Barcode);
                AddLog($"[DB] 查詢工單: {e.Barcode} -> {workNo}");
                UpdatePortStatus(portId, e.Barcode, workNo, PortStatus.Occupied);
                _repo.UpdatePortStateOnly(portId, "OCCUPIED");
                _repo.InsertBinding(new AdvancedPlanningSystem.Models.StateBinding { CarrierId = e.Barcode, PortId = portId, LotId = workNo, BindTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            };

            // --- Pick 事件 (出庫) ---
            _tcpServer.OnPick += async (s, e) => {
                string rawPortId = e.PortID;
                string portId = rawPortId;
                int pNum;
                if (int.TryParse(rawPortId, out pNum)) portId = "P" + pNum.ToString("D2");

                AddLog($"[PICK] RawPort: {rawPortId} -> Port: {portId}");

                // 1. 計算預計到達時間並移至 Transit 表
                var portState = _repo.GetActivePorts().FirstOrDefault(p => p.PortId == portId);
                if (portState != null && !string.IsNullOrEmpty(portState.CarrierId))
                {
                    var binding = _repo.GetBinding(portState.CarrierId);
                    if (binding != null)
                    {
                        // [修正] 暫時停用 local_config_qtime，強制預設為 30
                        int bufferMin = 30; 

                        // 建立 Transit 紀錄
                        var transit = new AdvancedPlanningSystem.Models.StateTransit
                        {
                            CarrierId = binding.CarrierId,
                            LotId = binding.LotId,
                            TargetEqpId = binding.TargetEqpId,
                            NextStepId = binding.NextStepId,
                            DispatchTime = binding.DispatchTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            PickupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ExpectedArrivalTime = DateTime.Now.AddMinutes(bufferMin).ToString("yyyy-MM-dd HH:mm:ss"),
                            IsOverdue = 0
                        };

                        _repo.MoveToTransit(transit);
                        AddLog($"[Transit] {binding.CarrierId} 已取走, 預計 {bufferMin} 分鐘後到達");
                    }
                }
                UpdatePortStatus(portId, "", "", PortStatus.Empty);
                _repo.UpdatePortStateOnly(portId, "EMPTY");
            };

            if (AppConfig.SimulatorEnabled)
            {
                _tcpServer.Start(AppConfig.SimulatorPort);
                AddLog($"TCP Server 已啟動 (Port: {AppConfig.SimulatorPort})，等待模擬器連線...");
            }
            else
            {
                AddLog("TCP Server 未啟動 (設定為停用)。");
                pnlSimStatus.BackColor = Color.Gray;
                lblSimStatus.Text = "模擬器已停用";
            }

            // 6. 啟動資料同步服務 (背景 Sync)
            // Phase 2 Dependency Injection Wiring
            _dispatchService = new AdvancedPlanningSystem.Services.DispatchService(_repo, _cloudRepo, _tcpServer);
            _syncService = new AdvancedPlanningSystem.Services.DataSyncService(_mesService, _repo, _cloudRepo, _dispatchService, lstLog);
            _dispatchService.SetDataSyncService(_syncService);

            // 啟動 Sync
            _syncService.Start(60000); // 每 60 秒同步一次 (內部包含 MES Cache Update)
        }

        private void AddLog(string msg)
        {
            string log = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => AddLog(msg)));
                return;
            }
            lstLog.Items.Insert(0, log);
            if (lstLog.Items.Count > 100) lstLog.Items.RemoveAt(100);
        }

        private void UpdatePortStatus(string portId, string cassetteId, string workNo, PortStatus status)
        {
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

        /// <summary>
        /// 啟用雙重緩衝 (解決閃爍問題)
        /// </summary>
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