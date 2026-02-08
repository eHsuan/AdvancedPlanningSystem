using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using APSSimulator.Client;
using APSSimulator.DB;
using APSSimulator.Server;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Data;

namespace APSSimulator
{
    public partial class FormMain : Form
    {
        private MesMockServer _mesServer;
        private ApsClient _apsClient;
        
        // Auto Simulation State
        private bool _isAutoSimPaused = false;
        private Dictionary<string, string> _carrierStatuses = new Dictionary<string, string>();
        private Dictionary<string, string> _carrierPorts = new Dictionary<string, string>(); // 新增：記錄卡匣所在 Port
        private string _dbConnStr = DatabaseHelper.ConnectionString;

        public bool IsSimulating => tmrAutoSimulation.Enabled;
        public DateTime SimStartTime { get; private set; }

        public FormMain()
        {
            InitializeComponent();
            InitializeCustomComponents();
            RefreshAutoSimGrid();
            SimStartTime = DateTime.Now;
        }

        private void RefreshAutoSimGrid()
        {
            try
            {
                using (var conn = new SQLiteConnection(_dbConnStr))
                {
                    conn.Open();
                    using (var adapter = new SQLiteDataAdapter("SELECT * FROM mock_mes_orders", conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // 1. 增加狀態欄位 (Index 0)
                        DataColumn colStatus = new DataColumn("目前狀態", typeof(string));
                        dt.Columns.Add(colStatus);
                        colStatus.SetOrdinal(0);

                        // 2. 增加 目前位置 欄位 (Index 1)
                        DataColumn colPort = new DataColumn("目前位置 (Location)", typeof(string));
                        dt.Columns.Add(colPort);
                        colPort.SetOrdinal(1);

                        foreach (DataRow row in dt.Rows)
                        {
                            string cid = row["carrier_id"].ToString();
                            
                            // 填入狀態
                            if (_carrierStatuses.ContainsKey(cid)) row["目前狀態"] = _carrierStatuses[cid];
                            else row["目前狀態"] = (row["next_step_id"].ToString() == "END") ? "製程結束" : "等待入庫";

                            // 填入位置 (PortID 或 EqpID)
                            if (_carrierPorts.ContainsKey(cid)) row["目前位置 (Location)"] = _carrierPorts[cid];
                            else row["目前位置 (Location)"] = "-";
                        }

                        dgvAutoSimOrders.DataSource = dt;
                        
                        // 設定樣式
                        if (dgvAutoSimOrders.Columns.Count > 1)
                        {
                            dgvAutoSimOrders.Columns[0].DefaultCellStyle.BackColor = Color.LightYellow;
                            dgvAutoSimOrders.Columns[0].DefaultCellStyle.Font = new Font(dgvAutoSimOrders.Font, FontStyle.Bold);
                            dgvAutoSimOrders.Columns[1].DefaultCellStyle.BackColor = Color.LightCyan;
                            dgvAutoSimOrders.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
            }
            catch { }
        }

        private void btnAutoSimExportLog_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAutoSimLog.Text))
            {
                MessageBox.Show("目前沒有日誌可供匯出。");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                FileName = $"AutoSimLog_{DateTime.Now:yyyyMMddHHmm}.txt"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, txtAutoSimLog.Text);
                    MessageBox.Show("日誌匯出成功。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("匯出失敗: " + ex.Message);
                }
            }
        }

        private void btnAutoSimResults_Click(object sender, EventArgs e)
        {
            // 稍後實作 SimulationGanttForm
            var ganttForm = new SimulationGanttForm(this, txtAutoSimLog.Text);
            ganttForm.Show();
        }

        private void btnAutoSimExportData_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgvAutoSimOrders.DataSource;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("目前沒有資料可供匯出。");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"AutoSimData_{DateTime.Now:yyyyMMddHHmm}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    IWorkbook wb = new XSSFWorkbook();
                    ISheet sheet = wb.CreateSheet("CurrentOrders");

                    // 標題行 (跳過第一欄「目前狀態」)
                    string[] headers = { "work_no", "carrier_id", "step_id", "next_step_id", "prev_out_time", "priority_type", "due_date", "route_id", "current_seq_no" };
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < headers.Length; i++)
                    {
                        headerRow.CreateCell(i).SetCellValue(headers[i]);
                    }

                    // 資料行
                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        IRow row = sheet.CreateRow(r + 1);
                        DataRow dr = dt.Rows[r];
                        // 注意：DataTable 第一欄是我們動態加入的「目前狀態」，所以讀取原本資料時要從索引 1 開始（或根據名稱讀取較安全）
                        row.CreateCell(0).SetCellValue(dr["work_no"]?.ToString());
                        row.CreateCell(1).SetCellValue(dr["carrier_id"]?.ToString());
                        row.CreateCell(2).SetCellValue(dr["step_id"]?.ToString());
                        row.CreateCell(3).SetCellValue(dr["next_step_id"]?.ToString());
                        row.CreateCell(4).SetCellValue(dr["prev_out_time"]?.ToString());
                        row.CreateCell(5).SetCellValue(dr["priority_type"]?.ToString());
                        row.CreateCell(6).SetCellValue(dr["due_date"]?.ToString());
                        row.CreateCell(7).SetCellValue(dr["route_id"]?.ToString());
                        row.CreateCell(8).SetCellValue(dr["current_seq_no"]?.ToString());
                    }

                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        wb.Write(fs);
                    }
                    MessageBox.Show("資料匯出成功。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("匯出失敗: " + ex.Message);
                }
            }
        }

        private void btnAutoSimExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", FileName = "MockOrders.xlsx" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try {
                    IWorkbook wb = new XSSFWorkbook();
                    ISheet sheet = wb.CreateSheet("Orders");
                    string[] headers = { "work_no", "carrier_id", "step_id", "next_step_id", "prev_out_time", "priority_type", "due_date", "route_id", "current_seq_no" };
                    IRow row = sheet.CreateRow(0);
                    for (int i = 0; i < headers.Length; i++) row.CreateCell(i).SetCellValue(headers[i]);
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create)) wb.Write(fs);
                    MessageBox.Show("Template Exported.");
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnAutoSimImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try {
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)) {
                        IWorkbook wb = new XSSFWorkbook(fs);
                        ISheet sheet = wb.GetSheetAt(0);
                        using (var conn = new SQLiteConnection(_dbConnStr)) {
                            conn.Open();
                            using (var trans = conn.BeginTransaction()) {
                                new SQLiteCommand("DELETE FROM mock_mes_orders", conn).ExecuteNonQuery();
                                _carrierStatuses.Clear(); 
                                _carrierPorts.Clear(); // 清除舊 Port 資訊

                                for (int i = 1; i <= sheet.LastRowNum; i++) {
                                    IRow row = sheet.GetRow(i); if (row == null) continue;
                                    var cmd = new SQLiteCommand("INSERT INTO mock_mes_orders VALUES (@wn, @cid, @sid, @nsid, @pot, @pt, @dd, @rid, @csn)", conn);
                                    cmd.Parameters.AddWithValue("@wn", row.GetCell(0)?.ToString());
                                    cmd.Parameters.AddWithValue("@cid", row.GetCell(1)?.ToString());
                                    cmd.Parameters.AddWithValue("@sid", row.GetCell(2)?.ToString());
                                    cmd.Parameters.AddWithValue("@nsid", row.GetCell(3)?.ToString());
                                    cmd.Parameters.AddWithValue("@pot", row.GetCell(4)?.ToString());
                                    cmd.Parameters.AddWithValue("@pt", row.GetCell(5)?.ToString() ?? "0");
                                    cmd.Parameters.AddWithValue("@dd", row.GetCell(6)?.ToString());
                                    cmd.Parameters.AddWithValue("@rid", row.GetCell(7)?.ToString());
                                    cmd.Parameters.AddWithValue("@csn", row.GetCell(8)?.ToString() ?? "0");
                                    cmd.ExecuteNonQuery();
                                }
                                trans.Commit();
                            }
                        }
                    }
                    RefreshAutoSimGrid();
                    MessageBox.Show("Import Success.");
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void AppendAutoSimLog(string msg)
        {
            if (txtAutoSimLog.InvokeRequired)
            {
                txtAutoSimLog.Invoke(new Action(() => AppendAutoSimLog(msg)));
                return;
            }
            txtAutoSimLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
            if (txtAutoSimLog.TextLength > 200000) txtAutoSimLog.Clear(); // 避免過長
        }

        private void btnAutoSimStart_Click(object sender, EventArgs e)
        {
            if (!_apsClient.IsConnected) { MessageBox.Show("請先連線至 APS。"); return; }
            
            SimStartTime = DateTime.Now;
            
            tmrAutoSimulation.Start();
            btnAutoSimStart.Enabled = false;
            btnAutoSimStop.Enabled = true;
            btnAutoSimPause.Enabled = true;
            lblAutoSimStatus.Text = "狀態: 執行中";
            lblAutoSimStatus.ForeColor = Color.Green;
            AppendAutoSimLog(">>> 自動模擬啟動");
        }

        private void btnAutoSimStop_Click(object sender, EventArgs e)
        {
            tmrAutoSimulation.Stop();
            btnAutoSimStart.Enabled = true;
            btnAutoSimStop.Enabled = false;
            btnAutoSimPause.Enabled = false;
            lblAutoSimStatus.Text = "狀態: 已停止";
            lblAutoSimStatus.ForeColor = Color.Red;
            _isAutoSimPaused = false;
            btnAutoSimPause.Text = "暫停模擬";
            AppendAutoSimLog(">>> 自動模擬停止");
        }

        private void btnAutoSimPause_Click(object sender, EventArgs e)
        {
            _isAutoSimPaused = !_isAutoSimPaused;
            btnAutoSimPause.Text = _isAutoSimPaused ? "恢復模擬" : "暫停模擬";
            lblAutoSimStatus.Text = _isAutoSimPaused ? "狀態: 暫停中" : "狀態: 執行中";
            lblAutoSimStatus.ForeColor = _isAutoSimPaused ? Color.Orange : Color.Green;
            AppendAutoSimLog(_isAutoSimPaused ? ">>> 自動模擬暫停" : ">>> 自動模擬恢復");
        }

                private async void tmrAutoSimulation_Tick(object sender, EventArgs e)

                {

                    if (_isAutoSimPaused) return;

                    // 批量檢查所有需要入庫或回存的卡匣

                    await ExecuteBatchStockInRequests();

                    

                    if (CheckAllFinished()) {

                        btnAutoSimStop.PerformClick();

                        MessageBox.Show("所有測試工單已完成（NextStep 均為 END）。");

                    }

                }

        

                private async Task ExecuteBatchStockInRequests()

                {

                    DataTable dt = (DataTable)dgvAutoSimOrders.DataSource;

                    if (dt == null) return;

        

                    var tasks = new List<Task>();

                    foreach (DataRow row in dt.Rows) {

                        string cstId = row["carrier_id"].ToString();

                        string nextStep = row["next_step_id"].ToString();

                        

                        bool isInitial = !_carrierStatuses.ContainsKey(cstId) && nextStep != "END";

                        bool isReturn = _carrierStatuses.ContainsKey(cstId) && _carrierStatuses[cstId] == "製程完成";

        

                        if (isInitial || isReturn)

                        {

                            // 標記為正在請求中，避免重複發送

                            _carrierStatuses[cstId] = "請求入庫中...";

                            AppendAutoSimLog($"[入庫請求] 向 APS 請求分配儲位: {cstId}");

                            tasks.Add(_apsClient.SendCommandAsync($"IN,{cstId}"));

                        }

                    }

        

                    if (tasks.Count > 0)

                    {

                        await Task.WhenAll(tasks);

                        this.Invoke(new Action(() => RefreshAutoSimGrid()));

                    }

                }

        

                // 移除舊的 CheckAndAutoStockIn 與 ExecuteBatchStockInFromCache

                private async Task CheckAndAutoStockIn() { await Task.FromResult(0); }

        

        private bool CheckAllFinished()
        {
            DataTable dt = (DataTable)dgvAutoSimOrders.DataSource;
            if (dt == null || dt.Rows.Count == 0) return false;
            foreach (DataRow row in dt.Rows) if (row["next_step_id"].ToString() != "END") return false;
            return true;
        }

        private async void HandleAutoSimOpen(string portId, string targetEqp)
        {
            if (!tmrAutoSimulation.Enabled || _isAutoSimPaused) return;
            
            // --- [精確匹配] 根據 APS 指定的 portId 找出模擬器中對應的卡匣 ID ---
            string cstId = _carrierPorts.FirstOrDefault(x => x.Value == portId).Key;

            if (string.IsNullOrEmpty(cstId))
            {
                AppendAutoSimLog($"[警告] 收到 APS 開啟 {portId} 指令，但模擬器找不到該 Port 對應的卡匣！");
                return;
            }

            AppendAutoSimLog($"[收到訊號] APS 要求開啟 {portId} ({cstId}) 移往 {targetEqp}");
            _carrierStatuses[cstId] = "等待取走";
            this.Invoke(new Action(() => RefreshAutoSimGrid()));

            await Task.Delay(1000);
            AppendAutoSimLog($"[執行模擬] PICK {portId} ({cstId})");
            await _apsClient.SendCommandAsync($"PICK,{portId}");
            
            // --- [Pass99 特殊處理] ---
            if (targetEqp == "Pass99")
            {
                AppendAutoSimLog($"[Pass99] {cstId} 偵測到跳站指令，直接執行製程結束");
                _carrierStatuses[cstId] = "製程完成";
                _carrierPorts[cstId] = "-";
                
                string wn = "";
                DataTable dtObj = (DataTable)dgvAutoSimOrders.DataSource;
                foreach (DataRow r in dtObj.Rows) if (r["carrier_id"].ToString() == cstId) { wn = r["work_no"].ToString(); break; }
                
                if (!string.IsNullOrEmpty(wn)) 
                {
                    // [重要] 跳站也要執行站點推進，否則會一直停在同一站
                    TrackInStepInDb(wn);
                    CompleteProcessingInDb(wn);
                }
                
                this.Invoke(new Action(() => RefreshAutoSimGrid()));
                await _apsClient.SendCommandAsync("GET_EMPTY_PORTS;");
                return; // 結束 Pass99 流程
            }

            // 卡匣被取走，標記為搬運中
            _carrierPorts[cstId] = "(搬運中)";
            this.Invoke(new Action(() => RefreshAutoSimGrid()));
            
            await Task.Delay(2000);
            _carrierStatuses[cstId] = "製程中";
            // 進入機台後，位置顯示為機台編號
            _carrierPorts[cstId] = targetEqp; 
            this.Invoke(new Action(() => RefreshAutoSimGrid()));

            AppendAutoSimLog($"[執行模擬] MOVE {portId} -> {targetEqp} ({cstId})");
            await _apsClient.SendCommandAsync($"MOVE,{portId},{targetEqp}");
            
            // --- [WIP 增加] ---
            if (targetEqp != "Pass99")
            {
                ChangeMachineWipInDb(targetEqp, 1);
                this.Invoke(new Action(() => LoadMachines())); 
            }
            
            // --- [關鍵修正] 先更新 MES 狀態，再通知 APS ---
            string workNo = "";
            DataTable dt = (DataTable)dgvAutoSimOrders.DataSource;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["carrier_id"].ToString() == cstId) { workNo = row["work_no"].ToString(); break; }
                }
            }

            if (!string.IsNullOrEmpty(workNo))
            {
                // 模擬進站 (Track-in)，這會將 MES 中的 StepId 更新為目標站點
                // 這裡我們調用原本手動模式使用的 TrackInStepInDb
                TrackInStepInDb(workNo); 
                AppendAutoSimLog($"[MES 更新] {cstId} 已在 MES 執行進站 (Track-in)");
            }

            // 加入短暫延遲確保 SQLite 寫入完成
            await Task.Delay(500);

            AppendAutoSimLog($"[執行模擬] ENTER {targetEqp} ({cstId})");
            await _apsClient.SendCommandAsync($"ENTER,{targetEqp}");
            
            // 進入機台後，CST 在 APS 的狀態應該從 Transit 轉為正式 WIP (由 APS 處理)
            _carrierStatuses[cstId] = "製程中";
            this.Invoke(new Action(() => RefreshAutoSimGrid()));

            int processTime = new Random().Next(AppConfig.SimProcessMinSec * 1000, AppConfig.SimProcessMaxSec * 1000);
            AppendAutoSimLog($"[製程執行中] 預計耗時 {processTime}ms...");
            await Task.Delay(processTime);
            
            if (!string.IsNullOrEmpty(workNo))
            {
                // 模擬完工 (Track-out)，這會將 MES 中的 StepId 更新為「下一站」
                CompleteProcessingInDb(workNo);
                
                // --- [WIP 減少] ---
                if (targetEqp != "Pass99")
                {
                    ChangeMachineWipInDb(targetEqp, -1);
                    this.Invoke(new Action(() => LoadMachines()));
                }

                AppendAutoSimLog($"[製程結束] {cstId} 已完成過站並離開機台 {targetEqp}");
            }

            _carrierStatuses[cstId] = "製程完成";
            this.Invoke(new Action(() => RefreshAutoSimGrid()));
            
            // 請求空 Port 清單，準備回存
            await _apsClient.SendCommandAsync("GET_EMPTY_PORTS;");
        }

        private void InitializeCustomComponents()
        {
            // --- External DB Sync Prompt ---
            var result = MessageBox.Show(
                "是否要根據目前 Mock MES 工單重新生成 ExternalDB 測試資料？\n(這將同步 CstID 與 WorkNo 的對應關係)", 
                "資料同步", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.SyncExternalDbFromMesOrders();
                    AppendClientLog("ExternalDB 測試資料同步完成。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("同步失敗: " + ex.Message);
                }
            }

            // --- Log Init ---
            UiLogAppender.LogReceived = (msg) => AppendMesLog(msg);

            // --- MES Server Init ---
            _mesServer = new MesMockServer();
            // _mesServer.OnLog is removed, using Log4Net
            
            btnStartMes.Click += (s, e) => {
                StartMESServer();
            };
            
            btnStopMes.Click += (s, e) => {
                StopMESServer();
            };

            // --- APS Client Init ---
            _apsClient = new ApsClient();
            _apsClient.OnLog += (msg) => AppendClientLog(msg);
            
            _apsClient.OnPortAssigned += (portId, cstId) => {
                // 收到 APS 分配的儲位
                _carrierPorts[cstId] = portId;
                _carrierStatuses[cstId] = "等待派送";
                AppendAutoSimLog($"[APS 分配] 卡匣 {cstId} 已分配至儲位 {portId}");
                this.Invoke(new Action(() => RefreshAutoSimGrid()));
            };

            _apsClient.OnMessageReceived += (msg) => {
                this.Invoke(new Action(() => {
                    if (msg.StartsWith("OPEN,"))
                    {
                        // Msg: OPEN,P01,LF0003
                        string[] parts = msg.Split(',');
                        if (parts.Length >= 2)
                        {
                            string portId = parts[1].Trim();
                            string targetEqp = parts.Length >= 3 ? parts[2].Trim() : "";
                            UpdateGridStatusWithTarget(portId, targetEqp, "Ready to Pick (Open)", Color.LightGreen);

                            // Auto Sim logic
                            HandleAutoSimOpen(portId, targetEqp);
                        }
                    }
                }));
            };

            _apsClient.OnConnectionChanged += (connected) => {
                this.Invoke(new Action(() => {
                    if (connected)
                    {
                        AppendClientLog("Connected to APS");
                        btnClientConnect.Enabled = false;
                        btnClientDisconnect.Enabled = true;
                        grpClientConfig.Enabled = false; 
                    }
                    else
                    {
                        AppendClientLog("Disconnected from APS");
                        btnClientConnect.Enabled = true;
                        btnClientDisconnect.Enabled = false;
                        grpClientConfig.Enabled = true;
                    }
                }));
            };

            btnClientConnect.Click += (s, e) => {
                string ip = txtClientIp.Text.Trim();
                int port = (int)numClientPort.Value;
                _apsClient.Start(ip, port);
            };

            btnClientDisconnect.Click += (s, e) => {
                _apsClient.Stop();
            };

            // 自動嘗試連線
            btnClientConnect.PerformClick();

            // [New] Batch Operations
            btnLoadDefault.Click += (s, e) => LoadTestList();

            btnSelectAll.Click += (s, e) => SetAllSelection(true);
            btnDeselectAll.Click += (s, e) => SetAllSelection(false);
            
            btnBatchScan.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow || !Convert.ToBoolean(row.Cells["colSelect"].Value)) continue;
                    await ProcessScanForRow(row);
                }
            };

            btnBatchPick.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow || !Convert.ToBoolean(row.Cells["colSelect"].Value)) continue;
                    await ProcessPickForRow(row);
                }
            };

            btnEnterEq.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow || !Convert.ToBoolean(row.Cells["colSelect"].Value)) continue;
                    await ProcessEnterEqForRow(row);
                }
            };

            // Machine Page
            btnRefreshMachines.Click += (s, e) => LoadMachines();
            btnDoSearch.Click += (s, e) => FilterMachines(txtSearchMachine.Text.Trim());
            txtSearchMachine.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    FilterMachines(txtSearchMachine.Text.Trim());
                    e.SuppressKeyPress = true; // Prevents ding sound
                }
            };
            LoadMachines(); // First load
            LoadTestList(); // Load default test list
        }

        private void StartMESServer()
        {
            int port = (int)numMesPort.Value;
            _mesServer.Start(port);
            lblMesStatus.Text = $"Status: Running (Port {port})";
            lblMesStatus.ForeColor = Color.Green;
            btnStartMes.Enabled = false;
            btnStopMes.Enabled = true;
            numMesPort.Enabled = false;
        }
        private void StopMESServer()
        {
            _mesServer.Stop();
            lblMesStatus.Text = "Status: Stopped";
            lblMesStatus.ForeColor = Color.Red;
            btnStartMes.Enabled = true;
            btnStopMes.Enabled = false;
            numMesPort.Enabled = true;
        }

        private void SetAllSelection(bool selected)
        {
            foreach (DataGridViewRow row in dgvTestList.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells["colSelect"].Value = selected;
            }
        }

        private async Task ProcessScanForRow(DataGridViewRow row)
        {
            string port = row.Cells["colPortId"].Value?.ToString();
            string cass = row.Cells["colCassetteId"].Value?.ToString();
            if (!string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(cass))
            {
                await _apsClient.ScanAsync(port, cass);
                row.Cells["colStatus"].Value = "Scanned (On Shelf)";
                row.DefaultCellStyle.BackColor = Color.LightYellow;
                await System.Threading.Tasks.Task.Delay(100); 
            }
        }

        private async Task ProcessPickForRow(DataGridViewRow row)
        {
            string port = row.Cells["colPortId"].Value?.ToString();
            if (!string.IsNullOrEmpty(port))
            {
                await _apsClient.PickAsync(port);
                row.Cells["colStatus"].Value = "Picked (In Transit)";
                row.DefaultCellStyle.BackColor = Color.White;
                await System.Threading.Tasks.Task.Delay(100);
            }
        }

        private async Task ProcessEnterEqForRow(DataGridViewRow row)
        {
            string workNo = row.Cells["colWorkNo"].Value?.ToString();
            string stepId = row.Cells["colCurrentStep"].Value?.ToString();
            string targetEqp = row.Cells["colTargetEqp"].Value?.ToString();
            if (string.IsNullOrEmpty(workNo)) return;

            TrackInStepInDb(workNo);
            
            // 自動增加機台 WIP
            if (!string.IsNullOrEmpty(targetEqp) && targetEqp != "STOCK")
            {
                ChangeMachineWipInDb(targetEqp, 1);
                LoadMachines(); 
            }

            row.Cells["colStatus"].Value = "Track-In (Processing)...";
            row.DefaultCellStyle.BackColor = Color.SandyBrown;
            RefreshRowData(row, workNo);

            int processSec = GetProcessTime(stepId);
            await Task.Run(async () => {
                await Task.Delay(processSec * 1000);
                CompleteProcessingInDb(workNo);
                
                // 自動減少機台 WIP
                if (!string.IsNullOrEmpty(targetEqp) && targetEqp != "STOCK")
                {
                    ChangeMachineWipInDb(targetEqp, -1);
                }

                this.Invoke(new Action(() => {
                    row.Cells["colStatus"].Value = "Finished (Ready to Scan)";
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.Cells["colTargetEqp"].Value = ""; 
                    AppendClientLog($"[Sim] {workNo} processed and WIP updated.");
                    RefreshRowData(row, workNo);
                    LoadMachines();
                }));
            });
        }

        private void dgvTestList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvTestList.ClearSelection();
                dgvTestList.Rows[e.RowIndex].Selected = true;
                dgvTestList.CurrentCell = dgvTestList.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
            }
        }

        private async void menuScan_Click(object sender, EventArgs e)
        {
            if (dgvTestList.SelectedRows.Count > 0) await ProcessScanForRow(dgvTestList.SelectedRows[0]);
        }

        private async void menuPick_Click(object sender, EventArgs e)
        {
            if (dgvTestList.SelectedRows.Count > 0) await ProcessPickForRow(dgvTestList.SelectedRows[0]);
        }

        private async void menuEnterEq_Click(object sender, EventArgs e)
        {
            if (dgvTestList.SelectedRows.Count > 0) await ProcessEnterEqForRow(dgvTestList.SelectedRows[0]);
        }

        private void FilterMachines(string keyword)
        {
            keyword = keyword.ToUpper();
            foreach (Control c in flowLayoutPanelMachines.Controls)
            {
                if (c is MachineControl mc)
                {
                    // 假設 MachineControl 有提供一個屬性或我們能取得其 ID
                    // 如果沒有，我們可能需要修改 MachineControl 或在這裡解析其內部 Label
                    // 根據之前的 LoadMachines，mc 的 EqpId 是在 BindData 時傳入的
                    // 我們暫時用 Name 或內部控制項搜尋，或者我先讀取 MachineControl.cs 確認
                    bool match = string.IsNullOrEmpty(keyword) || mc.EqpId.ToUpper().Contains(keyword);
                    mc.Visible = match;
                }
            }
        }

        private int GetProcessTime(string stepId)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT std_time_sec FROM mock_mes_step_time WHERE step_id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", stepId);
                        object res = cmd.ExecuteScalar();
                        return res != null ? Convert.ToInt32(res) : 30; // 預設 30s
                    }
                }
            }
            catch { return 30; }
        }

        private void TrackInStepInDb(string workNo)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    // 1. 取得目前該工單對應的下一站 (即將進入的這一站) 資訊
                    string sqlGetThisStep = @"
                        SELECT r.seq_no, r.step_id, o.route_id
                        FROM mock_mes_route_def r 
                        JOIN mock_mes_orders o ON r.route_id = o.route_id 
                        WHERE o.work_no = @wn AND r.seq_no > o.current_seq_no 
                        ORDER BY r.seq_no ASC LIMIT 1";

                    using (var cmd = new SQLiteCommand(sqlGetThisStep, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", workNo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int thisSeq = Convert.ToInt32(reader["seq_no"]);
                                string thisStepId = reader["step_id"].ToString();
                                string routeId = reader["route_id"].ToString();

                                // 2. 計算再下一站 (Next Step of Next Step)
                                string nextStepId = "END";
                                string sqlGetNext = "SELECT step_id FROM mock_mes_route_def WHERE route_id = @rid AND seq_no > @seq ORDER BY seq_no ASC LIMIT 1";
                                using (var nextCmd = new SQLiteCommand(sqlGetNext, conn))
                                {
                                    nextCmd.Parameters.AddWithValue("@rid", routeId);
                                    nextCmd.Parameters.AddWithValue("@seq", thisSeq);
                                    var result = nextCmd.ExecuteScalar();
                                    if (result != null) nextStepId = result.ToString();
                                }

                                // 3. 執行更新
                                string sqlUpdate = "UPDATE mock_mes_orders SET step_id = @sid, next_step_id = @nid, current_seq_no = @seq WHERE work_no = @wn";
                                using (var updCmd = new SQLiteCommand(sqlUpdate, conn))
                                {
                                    updCmd.Parameters.AddWithValue("@sid", thisStepId);
                                    updCmd.Parameters.AddWithValue("@nid", nextStepId);
                                    updCmd.Parameters.AddWithValue("@seq", thisSeq);
                                    updCmd.Parameters.AddWithValue("@wn", workNo);
                                    updCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendClientLog("TrackInStepInDb Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 模擬加工完成出站 (Track-Out)：更新 prev_out_time 並跳轉到下一站的計算點
        /// </summary>
        private void CompleteProcessingInDb(string workNo)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    // 更新 prev_out_time 為現在時間，這代表「這一站加工結束時間」
                    // 這會影響 APS 下一輪評分中的 QTime 計算
                    string sqlUpdate = "UPDATE mock_mes_orders SET prev_out_time = @now WHERE work_no = @wn";
                    using (var cmd = new SQLiteCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyyMMddHHmmss"));
                        cmd.Parameters.AddWithValue("@wn", workNo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                AppendClientLog("CompleteProcessingInDb Error: " + ex.Message);
            }
        }

        private void RefreshRowData(DataGridViewRow row, string workNo)
        {
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                // 借用 MockMesRepository 來抓最新資料 (含計算後的 NextStepId)
                var repo = new APSSimulator.DB.MockMesRepository();
                var orders = repo.GetOrders(conn, new List<string> { workNo });
                if (orders.Count > 0)
                {
                    var o = orders[0];
                    row.Cells["colCurrentStep"].Value = o.StepId;
                    row.Cells["colNextStep"].Value = o.NextStepId;
                }
            }
        }

        private void LoadTestList()
        {
            dgvTestList.Rows.Clear();
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM mock_mes_orders";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int pIndex = 1;
                    while (reader.Read())
                    {
                        string workNo = reader["work_no"].ToString();
                        string carrier = reader["carrier_id"].ToString();
                        string step = reader["step_id"].ToString();
                        string next = reader["next_step_id"].ToString(); // Note: This might be inaccurate if dynamic
                        
                        // Assign a virtual port for testing
                        string portId = $"P{pIndex:D2}";
                        pIndex++;

                        int idx = dgvTestList.Rows.Add();
                        var row = dgvTestList.Rows[idx];
                        row.Cells["colSelect"].Value = true; // Default selected
                        row.Cells["colPortId"].Value = portId;
                        row.Cells["colCassetteId"].Value = carrier;
                        row.Cells["colWorkNo"].Value = workNo;
                        row.Cells["colCurrentStep"].Value = step;
                        row.Cells["colNextStep"].Value = next;
                        row.Cells["colStatus"].Value = "Idle";
                    }
                }
            }
        }

        private void ChangeMachineWipInDb(string eqpId, int delta)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "UPDATE mock_mes_equipments SET current_wip_qty = current_wip_qty + @d WHERE eqp_id = @id";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@d", delta);
                        cmd.Parameters.AddWithValue("@id", eqpId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                AppendClientLog($"ChangeMachineWipInDb Error ({eqpId}): {ex.Message}");
            }
        }

        private void UpdateGridStatusWithTarget(string portId, string targetEqp, string status, Color color)
        {
            foreach (DataGridViewRow row in dgvTestList.Rows)
            {
                if (row.Cells["colPortId"].Value?.ToString() == portId)
                {
                    row.Cells["colStatus"].Value = status;
                    row.Cells["colTargetEqp"].Value = targetEqp;
                    row.DefaultCellStyle.BackColor = color;
                    row.Cells["colAction"].Value = $"Received OPEN: {targetEqp} at {DateTime.Now:HH:mm:ss}";
                    break; 
                }
            }
        }

        private void AppendMesLog(string msg)
        {
            if (txtMesLog.InvokeRequired)
            {
                txtMesLog.BeginInvoke(new Action<string>(AppendMesLog), msg);
                return;
            }
            txtMesLog.AppendText(msg + Environment.NewLine);
            txtMesLog.SelectionStart = txtMesLog.Text.Length;
            txtMesLog.ScrollToCaret();
        }

        private void AppendClientLog(string msg)
        {
            if (txtClientLog.InvokeRequired)
            {
                txtClientLog.BeginInvoke(new Action<string>(AppendClientLog), msg);
                return;
            }
            txtClientLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}" + Environment.NewLine);
            txtClientLog.SelectionStart = txtClientLog.Text.Length;
            txtClientLog.ScrollToCaret();
        }

        private void LoadMachines()
        {
            flowLayoutPanelMachines.Controls.Clear();
            
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                // 移除 group_name 查詢，因為 Schema 已變更
                string sql = "SELECT * FROM mock_mes_equipments ORDER BY eqp_id";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader["eqp_id"].ToString();
                        // 從 eqp_id 解析 Group (前兩碼)
                        string grp = id.Length >= 2 ? id.Substring(0, 2) : "UNK";
                        string status = reader["status"].ToString();
                        int curWip = Convert.ToInt32(reader["current_wip_qty"]);
                        int maxWip = Convert.ToInt32(reader["max_wip_qty"]);

                        var control = new MachineControl();
                        control.BindData(id, grp, status, curWip, maxWip);
                        control.OnStatusChange += (s, newStatus) => UpdateMachineStatus(id, newStatus);
                        control.OnWipChange += (s, newWip) => UpdateMachineWip(id, newWip);
                        control.Margin = new Padding(10);
                        
                        flowLayoutPanelMachines.Controls.Add(control);
                    }
                }
            }
            // 重新整理後自動套用目前的搜尋過濾條件
            FilterMachines(txtSearchMachine.Text.Trim());
        }

        private void UpdateMachineStatus(string eqpId, string status)
        {
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE mock_mes_equipments SET status = @s WHERE eqp_id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@s", status);
                    cmd.Parameters.AddWithValue("@id", eqpId);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadMachines(); // Reload UI
        }

        private void UpdateMachineWip(string eqpId, int wip)
        {
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE mock_mes_equipments SET current_wip_qty = @w WHERE eqp_id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@w", wip);
                    cmd.Parameters.AddWithValue("@id", eqpId);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadMachines(); // Reload UI
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _mesServer?.Stop();
            _apsClient?.Stop();
            base.OnFormClosing(e);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            StartMESServer();
        }
    }
}