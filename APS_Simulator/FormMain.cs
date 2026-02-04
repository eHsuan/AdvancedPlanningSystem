using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
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
            _apsClient.OnConnectionChanged += (connected) => {
                this.Invoke(new Action(() => {
                    if (connected)
                    {
                        AppendClientLog("Connected to APS");
                        btnClientConnect.Enabled = false;
                        btnClientDisconnect.Enabled = true;
                        grpClientConfig.Enabled = false; // Lock config while connected
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

            // Person Page Operations
            btnScan.Click += async (s, e) => {
                string pid = txtPortId.Text.Trim();
                string bc = txtBarcode.Text.Trim();
                if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(bc)) return;
                await _apsClient.ScanAsync(pid, bc);
            };

            btnPick.Click += async (s, e) => {
                 string pid = txtPortId.Text.Trim();
                if (string.IsNullOrEmpty(pid)) return;
                await _apsClient.PickAsync(pid);
            };

            // Machine Page
            btnRefreshMachines.Click += (s, e) => LoadMachines();
            LoadMachines(); // First load
        }

        private void AppendMesLog(string msg)
        {
            if (txtMesLog.InvokeRequired)
            {
                txtMesLog.BeginInvoke(new Action<string>(AppendMesLog), msg);
                return;
            }
            txtMesLog.AppendText(msg + Environment.NewLine);
        }

        private void AppendClientLog(string msg)
        {
            if (txtClientLog.InvokeRequired)
            {
                txtClientLog.BeginInvoke(new Action<string>(AppendClientLog), msg);
                return;
            }
            txtClientLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}" + Environment.NewLine);
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