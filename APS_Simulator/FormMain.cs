using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using APSSimulator.Client;
using APSSimulator.DB;
using APSSimulator.Server;

namespace APSSimulator
{
    public partial class FormMain : Form
    {
        private MesMockServer _mesServer;
        private ApsClient _apsClient;

        public FormMain()
        {
            InitializeComponent();
            InitializeCustomComponents();
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
                int port = (int)numMesPort.Value;
                _mesServer.Start(port);
                lblMesStatus.Text = $"Status: Running (Port {port})";
                lblMesStatus.ForeColor = Color.Green;
                btnStartMes.Enabled = false;
                btnStopMes.Enabled = true;
                numMesPort.Enabled = false;
            };
            
            btnStopMes.Click += (s, e) => {
                _mesServer.Stop();
                lblMesStatus.Text = "Status: Stopped";
                lblMesStatus.ForeColor = Color.Red;
                btnStartMes.Enabled = true;
                btnStopMes.Enabled = false;
                numMesPort.Enabled = true;
            };

            // 自動啟動 (使用預設值)
            btnStartMes.PerformClick();

            // --- APS Client Init ---
            _apsClient = new ApsClient();
            _apsClient.OnLog += (msg) => AppendClientLog(msg);
            
            // [New] Handle incoming OPEN command
            _apsClient.OnMessageReceived += (msg) => {
                this.Invoke(new Action(() => {
                    if (msg.StartsWith("OPEN,"))
                    {
                        // Msg: OPEN,P01
                        string[] parts = msg.Split(',');
                        if (parts.Length >= 2)
                        {
                            string portId = parts[1].Trim();
                            UpdateGridStatus(portId, "Ready to Pick (Open)", Color.LightGreen);
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
            
            btnBatchScan.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow) continue;
                    bool selected = Convert.ToBoolean(row.Cells["colSelect"].Value);
                    if (selected)
                    {
                        string port = row.Cells["colPortId"].Value?.ToString();
                        string cass = row.Cells["colCassetteId"].Value?.ToString();
                        if (!string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(cass))
                        {
                            await _apsClient.ScanAsync(port, cass);
                            row.Cells["colStatus"].Value = "Scanned (On Shelf)";
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                            await System.Threading.Tasks.Task.Delay(100); // Small delay to avoid flooding
                        }
                    }
                }
            };

            btnBatchPick.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow) continue;
                    bool selected = Convert.ToBoolean(row.Cells["colSelect"].Value);
                    if (selected)
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
                }
            };

            btnEnterEq.Click += async (s, e) => {
                foreach (DataGridViewRow row in dgvTestList.Rows)
                {
                    if (row.IsNewRow) continue;
                    bool selected = Convert.ToBoolean(row.Cells["colSelect"].Value);
                    if (!selected) continue;

                    string workNo = row.Cells["colWorkNo"].Value?.ToString();
                    string stepId = row.Cells["colCurrentStep"].Value?.ToString();
                    if (string.IsNullOrEmpty(workNo)) continue;

                    // 1. 查詢加工時間
                    int processSec = GetProcessTime(stepId);
                    row.Cells["colStatus"].Value = $"Processing ({processSec}s)...";
                    row.DefaultCellStyle.BackColor = Color.SandyBrown;

                    // 2. 異步等待
                    var task = Task.Run(async () => {
                        await Task.Delay(processSec * 1000);
                        
                        // 3. 加工完成，更新資料庫 (過帳到下一站)
                        CompleteProcessingInDb(workNo);

                        // 4. 通知 UI 更新
                        this.Invoke(new Action(() => {
                            row.Cells["colStatus"].Value = "Finished (Ready to Scan)";
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                            AppendClientLog($"[Sim] {workNo} processed in EQ ({processSec}s).");
                            // 重新讀取該行資料以更新 Current/Next Step 顯示
                            RefreshRowData(row, workNo);
                        }));
                    });
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

        private void CompleteProcessingInDb(string workNo)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    // 1. 取得當前 NextStepId 所在的 seq_no
                    string sqlGetNext = @"
                        SELECT r.seq_no, r.step_id 
                        FROM mock_mes_route_def r 
                        JOIN mock_mes_orders o ON r.route_id = o.route_id 
                        WHERE o.work_no = @wn AND r.seq_no > o.current_seq_no 
                        ORDER BY r.seq_no ASC LIMIT 1";
                    
                    int nextSeq = -1;
                    string nextStepId = "";

                    using (var cmd = new SQLiteCommand(sqlGetNext, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", workNo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nextSeq = Convert.ToInt32(reader["seq_no"]);
                                nextStepId = reader["step_id"].ToString();
                            }
                        }
                    }

                    // 2. 更新工單狀態：
                    // - current_seq_no 設為剛完成站點的 seq (或下一個，視 MES 邏輯，這裡我們設為剛找到的 nextSeq 表示已到該站)
                    // - step_id 設為 nextStepId
                    // - prev_out_time 設為現在 (觸發下一站的 QTime 計算)
                    if (nextSeq != -1)
                    {
                        string sqlUpdate = @"
                            UPDATE mock_mes_orders 
                            SET current_seq_no = @seq, 
                                step_id = @sid, 
                                prev_out_time = @now 
                            WHERE work_no = @wn";
                        
                        using (var cmd = new SQLiteCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@seq", nextSeq);
                            cmd.Parameters.AddWithValue("@sid", nextStepId);
                            cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                            cmd.Parameters.AddWithValue("@wn", workNo);
                            cmd.ExecuteNonQuery();
                        }
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

        private void UpdateGridStatus(string portId, string status, Color color)
        {
            foreach (DataGridViewRow row in dgvTestList.Rows)
            {
                if (row.Cells["colPortId"].Value?.ToString() == portId)
                {
                    row.Cells["colStatus"].Value = status;
                    row.DefaultCellStyle.BackColor = color;
                    row.Cells["colAction"].Value = $"Received OPEN at {DateTime.Now:HH:mm:ss}";
                    break; // Found
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
    }
}