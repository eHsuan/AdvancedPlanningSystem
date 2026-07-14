using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedPlanningSystem.MES;
using AdvancedPlanningSystem.Models;
using System.Web.Script.Serialization;

namespace AdvancedPlanningSystem
{
    public partial class TestForm : Form
    {
        private IMesService _mesService;
        private AdvancedPlanningSystem.Services.PlcService _plcService;
        private JavaScriptSerializer _jsonSerializer;

        private TabPage tabPageRawJson;
        private TextBox txtRawJsonInput;
        private TextBox txtEndpoint;
        private ComboBox cbMethod;
        private Button btnSendRaw;
        private Button btnFormatRaw;

        // PLC IO Tab elements
        private Timer _plcIoTimer;
        private List<PortControlMapping> _plcControlMappings = new List<PortControlMapping>();
        private Label _lblPlcConnStatus;

        private class PortControlMapping
        {
            public string PortId { get; set; }
            public string X_Door_Address { get; set; }
            public string X_Presence_Address { get; set; }
            public string Y_Red_Address { get; set; }
            public string Y_Lock_Address { get; set; }
            public string Y_Green_Address { get; set; }

            public Label LblDoorStatus { get; set; }
            public Label LblPresenceStatus { get; set; }
            public Label LblRedStatus { get; set; }
            public Label LblLockStatus { get; set; }
            public Label LblGreenStatus { get; set; }
        }

        public TestForm(IMesService mesService, AdvancedPlanningSystem.Services.PlcService plcService)
        {
            InitializeComponent();
            _mesService = mesService;
            _plcService = plcService;
            _jsonSerializer = new JavaScriptSerializer();
            SetupRawJsonTab();

            if (AppConfig.ManualMode)
            {
                SetupPlcIoTab();
            }
        }

        private void Log(string msg)
        {
            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
        }

        private string FormatJson(object obj)
        {
            try
            {
                // Basic JSON serialization
                string json = _jsonSerializer.Serialize(obj);
                
                // Manual pretty-printing for better readability
                var sb = new StringBuilder();
                int indent = 0;
                bool inString = false;
                
                foreach (char c in json)
                {
                    if (c == '"') inString = !inString;
                    
                    if (!inString)
                    {
                        if (c == '{' || c == '[')
                        {
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            indent++;
                            sb.Append(new string(' ', indent * 2));
                        }
                        else if (c == '}' || c == ']')
                        {
                            sb.Append(Environment.NewLine);
                            indent--;
                            sb.Append(new string(' ', indent * 2));
                            sb.Append(c);
                        }
                        else if (c == ',')
                        {
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', indent * 2));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"JSON Format Error: {ex.Message}";
            }
        }

        private async void btnTestGetWip_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing Get WIP and EqStatus for APS_Eqp...");
                string input = txtInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    input = "EQP01,EQP02,EQP03"; // Default test IDs
                    Log($"Using default IDs: {input}");
                }
                
                var ids = input.Split(',').Select(s => s.Trim()).ToList();
                
                var ask = new ApsEqpAsk
                {
                    TransactionName = "WOQRY",
                    EqpNo = AppConfig.MesDefaultEqpNo,
                    WONO = "0000000000",
                    UserID = "000000",
                    GetAPSInfo_ByEqp = string.Join(",", ids)
                };
                txtOutput.AppendText($"=== Request Body ==={Environment.NewLine}{FormatJson(ask)}{Environment.NewLine}{Environment.NewLine}");

                Log("1/2: Fetching WIP Batch...");
                var wipResult = await _mesService.GetWipBatchAsync(ids);
                Log($"WIP Result Count: {wipResult.Count}");
                txtOutput.AppendText("=== WIP Info ===" + Environment.NewLine + FormatJson(wipResult) + Environment.NewLine);
                
                Log("2/2: Fetching Equipment Status...");
                var statusResult = await _mesService.GetEquipmentStatusBatchAsync(ids);
                Log($"Status Result Count: {statusResult.Count}");
                txtOutput.AppendText("=== EqStatus Info ===" + Environment.NewLine + FormatJson(statusResult) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestGetOrder_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetOrderInfoBatchAsync...");
                string input = txtInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    input = "LOT001,LOT002"; 
                    Log($"Using default IDs: {input}");
                }

                var ids = input.Split(',').Select(s => s.Trim()).ToList();
                
                var ask = new ApsLotAsk
                {
                    TransactionName = "WOQRY",
                    EqpNo = AppConfig.MesDefaultEqpNo,
                    WONO = "0000000000",
                    UserID = "000000",
                    GetAPSInfo_ByLot = string.Join(",", ids)
                };
                txtOutput.AppendText($"=== Request Body ==={Environment.NewLine}{FormatJson(ask)}{Environment.NewLine}{Environment.NewLine}");

                var result = await _mesService.GetOrderInfoBatchAsync(ids);

                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestQTime_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetAllQTimeLimitsAsync...");
                
                var ask = new ApsQTimeAsk
                {
                    TransactionName = "WOQRY",
                    EqpNo = AppConfig.MesDefaultEqpNo,
                    WONO = "0000000000",
                    UserID = "000000",
                    GetAPSInfo_QTime = "UP"
                };
                txtOutput.AppendText($"=== Request Body ==={Environment.NewLine}{FormatJson(ask)}{Environment.NewLine}{Environment.NewLine}");

                var result = await _mesService.GetAllQTimeLimitsAsync();
                
                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestStepTime_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetAllStepTimesAsync...");
                txtOutput.AppendText($"[Info] No request sent. StepTime API is removed in formal MES spec (using 5 mins default fallback).{Environment.NewLine}{Environment.NewLine}");

                var result = await _mesService.GetAllStepTimesAsync();

                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }

        private void SetupRawJsonTab()
        {
            tabPageRawJson = new TabPage("Raw JSON Sender");
            tabPageRawJson.Padding = new Padding(10);

            var split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Vertical;
            split.SplitterDistance = 450;

            var grpJson = new GroupBox();
            grpJson.Text = "JSON Body (Request Payload)";
            grpJson.Dock = DockStyle.Fill;

            txtRawJsonInput = new TextBox();
            txtRawJsonInput.Multiline = true;
            txtRawJsonInput.ScrollBars = ScrollBars.Vertical;
            txtRawJsonInput.Font = new Font("Consolas", 10F);
            txtRawJsonInput.Dock = DockStyle.Fill;
            txtRawJsonInput.Text = "{\r\n  \"TransactionName\": \"WOQRY\",\r\n  \"GetAPSInfo_ByEqp\": \"CL0006\"\r\n}";

            grpJson.Controls.Add(txtRawJsonInput);
            split.Panel1.Controls.Add(grpJson);

            var grpControl = new GroupBox();
            grpControl.Text = "Request Options";
            grpControl.Dock = DockStyle.Fill;
            grpControl.Padding = new Padding(10);

            var lblEndpoint = new Label();
            lblEndpoint.Text = "Endpoint / Route (e.g. /woqry):";
            lblEndpoint.Location = new Point(15, 25);
            lblEndpoint.Size = new Size(200, 15);

            txtEndpoint = new TextBox();
            txtEndpoint.Text = "/EqpTransaction";
            txtEndpoint.Location = new Point(15, 45);
            txtEndpoint.Width = 280;
            txtEndpoint.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblMethod = new Label();
            lblMethod.Text = "HTTP Method:";
            lblMethod.Location = new Point(15, 80);
            lblMethod.Size = new Size(100, 15);

            cbMethod = new ComboBox();
            cbMethod.Items.Add("POST");
            cbMethod.Items.Add("GET");
            cbMethod.SelectedIndex = 0;
            cbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMethod.Location = new Point(15, 100);
            cbMethod.Width = 120;

            btnSendRaw = new Button();
            btnSendRaw.Text = "Send Request";
            btnSendRaw.Location = new Point(15, 150);
            btnSendRaw.Size = new Size(130, 35);
            btnSendRaw.Click += btnSendRaw_Click;

            btnFormatRaw = new Button();
            btnFormatRaw.Text = "Format JSON";
            btnFormatRaw.Location = new Point(165, 150);
            btnFormatRaw.Size = new Size(130, 35);
            btnFormatRaw.Click += btnFormatRaw_Click;

            grpControl.Controls.Add(lblEndpoint);
            grpControl.Controls.Add(txtEndpoint);
            grpControl.Controls.Add(lblMethod);
            grpControl.Controls.Add(cbMethod);
            grpControl.Controls.Add(btnSendRaw);
            grpControl.Controls.Add(btnFormatRaw);

            split.Panel2.Controls.Add(grpControl);
            tabPageRawJson.Controls.Add(split);

            this.tabControl1.TabPages.Add(tabPageRawJson);
        }

        private void btnFormatRaw_Click(object sender, EventArgs e)
        {
            string raw = txtRawJsonInput.Text.Trim();
            if (string.IsNullOrEmpty(raw)) return;
            try
            {
                var obj = _jsonSerializer.DeserializeObject(raw);
                txtRawJsonInput.Text = FormatJson(obj);
                Log("[Format] JSON body formatted successfully.");
            }
            catch (Exception ex)
            {
                Log($"[Format Error] {ex.Message}");
            }
        }

        private async void btnSendRaw_Click(object sender, EventArgs e)
        {
            string endpoint = txtEndpoint.Text.Trim();
            if (!endpoint.StartsWith("/")) endpoint = "/" + endpoint;

            string mesBaseUrl = AppConfig.MesMockEnabled
                ? $"{AppConfig.MesMockUrl}:{AppConfig.MesMockPort}/api"
                : AppConfig.RealMesUrl;

            string fullUrl = mesBaseUrl + endpoint;
            string method = cbMethod.SelectedItem?.ToString() ?? "POST";
            string jsonBody = txtRawJsonInput.Text;

            try
            {
                Log($"Sending {method} Request to: {fullUrl} ...");
                if (method == "POST" && !string.IsNullOrEmpty(jsonBody))
                {
                    txtOutput.AppendText($"=== Request Body ==={Environment.NewLine}{jsonBody}{Environment.NewLine}{Environment.NewLine}");
                }

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    System.Net.Http.HttpResponseMessage response;

                    if (method == "POST")
                    {
                        if (endpoint.Equals("/EqpTransaction", StringComparison.OrdinalIgnoreCase) || endpoint.Equals("/WOQRY", StringComparison.OrdinalIgnoreCase))
                        {
                            var dict = new Dictionary<string, string> { { "pParameter", jsonBody } };
                            var content = new System.Net.Http.FormUrlEncodedContent(dict);
                            response = await client.PostAsync(fullUrl, content);
                        }
                        else
                        {
                            var content = new System.Net.Http.StringContent(jsonBody, Encoding.UTF8, "application/json");
                            response = await client.PostAsync(fullUrl, content);
                        }
                    }
                    else
                    {
                        response = await client.GetAsync(fullUrl);
                    }

                    string resBody = await response.Content.ReadAsStringAsync();
                    Log($"Response Status: {(int)response.StatusCode} ({response.StatusCode})");

                    string jsonToDecode = resBody;
                    if (resBody.TrimStart().StartsWith("<"))
                    {
                        try
                        {
                            var doc = System.Xml.Linq.XDocument.Parse(resBody);
                            jsonToDecode = doc.Root.Value;
                        }
                        catch (Exception ex)
                        {
                            Log($"Failed to parse XML response: {ex.Message}");
                        }
                    }

                    try
                    {
                        var parsedJson = _jsonSerializer.DeserializeObject(jsonToDecode);
                        txtOutput.AppendText($"=== Response Body ==={Environment.NewLine}{FormatJson(parsedJson)}{Environment.NewLine}");
                    }
                    catch
                    {
                        txtOutput.AppendText($"=== Response Body (Raw) ==={Environment.NewLine}{resBody}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Request Error: {ex.Message}");
            }
        }

        private void SetupPlcIoTab()
        {
            TabPage tabPagePlcIo = new TabPage("PLC IO 手動控制");
            tabPagePlcIo.Padding = new Padding(10);

            // Connection Status Panel at the top
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            Label lblPlcConnTitle = new Label
            {
                Text = "PLC 連線狀態: ",
                Location = new Point(15, 12),
                AutoSize = true,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold)
            };

            _lblPlcConnStatus = new Label
            {
                Text = "Checking...",
                Location = new Point(110, 10),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold)
            };

            pnlTop.Controls.Add(lblPlcConnTitle);
            pnlTop.Controls.Add(_lblPlcConnStatus);

            // FlowLayoutPanel for Ports
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            if (_plcService == null)
            {
                Label lblError = new Label
                {
                    Text = "PLC 服務未初始化 (可能還在進行 MES 連線中...)",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(20, 60),
                    Font = new Font("Microsoft JhengHei", 12F, FontStyle.Bold)
                };
                tabPagePlcIo.Controls.Add(lblError);
            }
            else if (_plcService.PortStates == null || _plcService.PortStates.Count == 0)
            {
                Label lblError = new Label
                {
                    Text = "載入 PLC 點位設定失敗或設定為空 (請確認 PLC_Adress.xml 是否正確)",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(20, 60),
                    Font = new Font("Microsoft JhengHei", 12F, FontStyle.Bold)
                };
                tabPagePlcIo.Controls.Add(lblError);
            }
            else
            {
                var states = _plcService.PortStates;
                foreach (var state in states)
                {
                    var cfg = state.Config;
                    
                    GroupBox grpPort = new GroupBox
                    {
                        Text = $"Port {cfg.PortId} 控制 (Index: {cfg.Index})",
                        Width = 360,
                        Height = 180,
                        Font = new Font("Microsoft JhengHei", 9F)
                    };

                    TableLayoutPanel tbl = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 4,
                        RowCount = 5,
                        Padding = new Padding(5)
                    };
                    tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Name/Addr
                    tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); // Status
                    tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5F)); // Action 1
                    tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5F)); // Action 2

                    // Row 0: X_Door
                    tbl.Controls.Add(new Label { Text = $"X_Door ({cfg.X_Door})", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
                    Label lblDoor = new Label { Text = "Loading...", BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Margin = new Padding(2) };
                    tbl.Controls.Add(lblDoor, 1, 0);

                    // Row 1: X_Presence
                    tbl.Controls.Add(new Label { Text = $"X_Presence ({cfg.X_Presence})", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
                    Label lblPresence = new Label { Text = "Loading...", BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Margin = new Padding(2) };
                    tbl.Controls.Add(lblPresence, 1, 1);

                    // Row 2: Y_Red
                    tbl.Controls.Add(new Label { Text = $"Y_Red ({cfg.Y_Red})", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
                    Label lblRed = new Label { Text = "Loading...", BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Margin = new Padding(2) };
                    tbl.Controls.Add(lblRed, 1, 2);
                    Button btnRedOn = new Button { Text = "ON", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnRedOn.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Red, true);
                    Button btnRedOff = new Button { Text = "OFF", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnRedOff.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Red, false);
                    tbl.Controls.Add(btnRedOn, 2, 2);
                    tbl.Controls.Add(btnRedOff, 3, 2);

                    // Row 3: Y_Lock
                    tbl.Controls.Add(new Label { Text = $"Y_Lock ({cfg.Y_Lock})", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
                    Label lblLock = new Label { Text = "Loading...", BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Margin = new Padding(2) };
                    tbl.Controls.Add(lblLock, 1, 3);
                    Button btnLockOn = new Button { Text = "Unlock", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnLockOn.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Lock, true);
                    Button btnLockOff = new Button { Text = "Lock", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnLockOff.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Lock, false);
                    tbl.Controls.Add(btnLockOn, 2, 3);
                    tbl.Controls.Add(btnLockOff, 3, 3);

                    // Row 4: Y_Green
                    tbl.Controls.Add(new Label { Text = $"Y_Green ({cfg.Y_Green})", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
                    Label lblGreen = new Label { Text = "Loading...", BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Margin = new Padding(2) };
                    tbl.Controls.Add(lblGreen, 1, 4);
                    Button btnGreenOn = new Button { Text = "ON", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnGreenOn.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Green, true);
                    Button btnGreenOff = new Button { Text = "OFF", Dock = DockStyle.Fill, Margin = new Padding(1) };
                    btnGreenOff.Click += async (s, e) => await WritePlcBitWithCheck(cfg.Y_Green, false);
                    tbl.Controls.Add(btnGreenOn, 2, 4);
                    tbl.Controls.Add(btnGreenOff, 3, 4);

                    grpPort.Controls.Add(tbl);
                    flowPanel.Controls.Add(grpPort);

                    _plcControlMappings.Add(new PortControlMapping
                    {
                        PortId = cfg.PortId,
                        X_Door_Address = cfg.X_Door,
                        X_Presence_Address = cfg.X_Presence,
                        Y_Red_Address = cfg.Y_Red,
                        Y_Lock_Address = cfg.Y_Lock,
                        Y_Green_Address = cfg.Y_Green,
                        LblDoorStatus = lblDoor,
                        LblPresenceStatus = lblPresence,
                        LblRedStatus = lblRed,
                        LblLockStatus = lblLock,
                        LblGreenStatus = lblGreen
                    });
                }
            }

            // Correct Z-order: Top first, then Fill
            tabPagePlcIo.Controls.Add(pnlTop);
            tabPagePlcIo.Controls.Add(flowPanel);

            this.tabControl1.TabPages.Add(tabPagePlcIo);

            // Initialize and start timer
            _plcIoTimer = new Timer();
            _plcIoTimer.Interval = 500;
            _plcIoTimer.Tick += PlcIoTimer_Tick;
            _plcIoTimer.Start();

            this.FormClosing += (s, e) => {
                _plcIoTimer?.Stop();
                _plcIoTimer?.Dispose();
            };
        }

        private async void PlcIoTimer_Tick(object sender, EventArgs e)
        {
            if (_plcService == null) return;

            bool isConn = _plcService.IsConnected;
            _lblPlcConnStatus.Text = isConn ? "PLC 已連線" : "PLC 未連線";
            _lblPlcConnStatus.BackColor = isConn ? Color.LightGreen : Color.Tomato;

            if (!isConn) return;

            // Use internal cache from PlcService to read X inputs, and read Y outputs directly to show actual status
            foreach (var mapping in _plcControlMappings)
            {
                try
                {
                    // Find matching port runtime state from PlcService cache
                    var state = _plcService.PortStates?.FirstOrDefault(ps => ps.Config.PortId == mapping.PortId);
                    if (state != null)
                    {
                        bool doorClosed = state.DebouncedDoor;
                        bool presence = state.DebouncedPresence;

                        mapping.LblDoorStatus.Text = doorClosed ? "Closed" : "Opened";
                        mapping.LblDoorStatus.BackColor = doorClosed ? Color.LightGreen : Color.Tomato;

                        mapping.LblPresenceStatus.Text = presence ? "Present" : "Empty";
                        mapping.LblPresenceStatus.BackColor = presence ? Color.LightBlue : Color.LightGray;
                    }

                    // For outputs, read live status from PLC
                    bool red = await _plcService.ReadBitAsync(mapping.Y_Red_Address);
                    bool lockState = await _plcService.ReadBitAsync(mapping.Y_Lock_Address);
                    bool green = await _plcService.ReadBitAsync(mapping.Y_Green_Address);

                    mapping.LblRedStatus.Text = red ? "ON" : "OFF";
                    mapping.LblRedStatus.BackColor = red ? Color.Tomato : Color.LightGray;

                    mapping.LblLockStatus.Text = lockState ? "Unlock (ON)" : "Lock (OFF)";
                    mapping.LblLockStatus.BackColor = lockState ? Color.LightGreen : Color.LightGray;

                    mapping.LblGreenStatus.Text = green ? "ON" : "OFF";
                    mapping.LblGreenStatus.BackColor = green ? Color.LightGreen : Color.LightGray;
                }
                catch
                {
                    // Ignore transient errors
                }
            }
        }

        private async Task WritePlcBitWithCheck(string address, bool value)
        {
            if (!AppConfig.ManualMode)
            {
                MessageBox.Show("目前非手動模式，無法控制 IO！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_plcService == null || !_plcService.IsConnected)
            {
                MessageBox.Show("PLC 未連線，無法控制 IO！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                await _plcService.WriteBitAsync(address, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"寫入 PLC 點位 {address} 失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
