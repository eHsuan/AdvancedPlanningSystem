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
        private JavaScriptSerializer _jsonSerializer;

        private TabPage tabPageRawJson;
        private TextBox txtRawJsonInput;
        private TextBox txtEndpoint;
        private ComboBox cbMethod;
        private Button btnSendRaw;
        private Button btnFormatRaw;

        public TestForm(IMesService mesService)
        {
            InitializeComponent();
            _mesService = mesService;
            _jsonSerializer = new JavaScriptSerializer();
            SetupRawJsonTab();
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

        private string ShowInputPrompt(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 300 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 290, Text = defaultValue };
            Button confirmation = new Button() { Text = "確定", Left = 120, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "取消", Left = 210, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void btnGenerateMockSql_Click(object sender, EventArgs e)
        {
            try
            {
                string input = ShowInputPrompt("請輸入欲生成的資料筆數（大於 0 代表生成）：", "詢問是否生成模擬資料", "5");
                if (string.IsNullOrEmpty(input)) return;

                int count;
                if (int.TryParse(input, out count) && count > 0)
                {
                    var dr = MessageBox.Show($"已輸入 {count} 筆。是否要生成 SQL 語法？", "確認生成", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("-- ========================================================");
                        sb.AppendLine($"-- 模擬資料 INSERT 語法 (共 {count} 筆)");
                        sb.AppendLine($"-- 產生時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        sb.AppendLine("-- ========================================================");
                        sb.AppendLine("INSERT INTO [DIAEAP].[dbo].[EAP_CassetteBind] ([CassetteID], [WorkOrder], [BindTime]) VALUES ");

                        Random rand = new Random();
                        string timeStr = DateTime.Now.ToString("yyyyMMddHHmmss");

                        for (int i = 0; i < count; i++)
                        {
                            int randNum = rand.Next(1000, 9999);
                            string cassetteId = $"CASS-{randNum}-{i + 1:D3}";
                            string workOrder = $"WO-{DateTime.Now:yyyyMMdd}-{randNum}";

                            sb.Append($"('{cassetteId}', '{workOrder}', '{timeStr}')");
                            if (i < count - 1)
                            {
                                sb.AppendLine(",");
                            }
                            else
                            {
                                sb.AppendLine(";");
                            }
                        }

                        Form sqlForm = new Form()
                        {
                            Width = 650,
                            Height = 450,
                            Text = "產生的 SQL 語法",
                            StartPosition = FormStartPosition.CenterScreen,
                            MaximizeBox = true,
                            MinimizeBox = false,
                            ShowIcon = false
                        };

                        TextBox txtSql = new TextBox()
                        {
                            Multiline = true,
                            ScrollBars = ScrollBars.Both,
                            Dock = DockStyle.Fill,
                            ReadOnly = true,
                            Text = sb.ToString(),
                            Font = new Font("Consolas", 10)
                        };

                        sqlForm.Controls.Add(txtSql);
                        sqlForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
