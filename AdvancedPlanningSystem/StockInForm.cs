using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public class StockInForm : Form
    {
        public string CassetteId { get; private set; } = "";
        public string WorkNo { get; private set; } = "";

        private TextBox txtFirst;
        private TextBox txtSecond;
        private Label lblFirst;
        private Label lblSecond;
        private Button btnSubmit;
        private Button btnCancel;

        public StockInForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "物料入庫 (Stock In)";
            this.Size = new Size(420, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = GetTitleByMode(),
                Font = new Font("Microsoft JhengHei", 12F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlMain.Controls.Add(lblTitle);

            if (AppConfig.InputMode == CarrierInputMode.Hybrid)
            {
                // 混合模式：兩個欄位
                lblFirst = new Label { Text = "Cassette ID (卡匣條碼):", Font = new Font("Microsoft JhengHei", 9.5F, FontStyle.Bold), Location = new Point(20, 50), AutoSize = true };
                txtFirst = new TextBox { Font = new Font("Consolas", 11F), Location = new Point(20, 72), Width = 360 };

                lblSecond = new Label { Text = "工單號碼 (WorkNo):", Font = new Font("Microsoft JhengHei", 9.5F, FontStyle.Bold), Location = new Point(20, 105), AutoSize = true };
                txtSecond = new TextBox { Font = new Font("Consolas", 11F), Location = new Point(20, 127), Width = 360 };

                txtFirst.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; txtSecond.Focus(); }
                };
                txtSecond.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Submit(); }
                };

                pnlMain.Controls.Add(lblFirst);
                pnlMain.Controls.Add(txtFirst);
                pnlMain.Controls.Add(lblSecond);
                pnlMain.Controls.Add(txtSecond);
            }
            else
            {
                // 單一欄位模式 (BarcodeBinding 或 WorkOrderOnly)
                string labelText = AppConfig.InputMode == CarrierInputMode.WorkOrderOnly ? "請輸入/掃描 工單號碼 (WorkNo):" : "請輸入/掃描 Cassette ID (卡匣條碼):";
                lblFirst = new Label { Text = labelText, Font = new Font("Microsoft JhengHei", 10F, FontStyle.Bold), Location = new Point(20, 60), AutoSize = true };
                txtFirst = new TextBox { Font = new Font("Consolas", 12F), Location = new Point(20, 90), Width = 360 };

                txtFirst.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Submit(); }
                };

                pnlMain.Controls.Add(lblFirst);
                pnlMain.Controls.Add(txtFirst);
            }

            btnSubmit = new Button
            {
                Text = "確認入庫",
                Font = new Font("Microsoft JhengHei", 10F, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 35),
                Location = new Point(150, 170)
            };
            btnSubmit.Click += (s, e) => Submit();

            btnCancel = new Button
            {
                Text = "取消",
                Font = new Font("Microsoft JhengHei", 10F),
                Size = new Size(90, 35),
                Location = new Point(270, 170)
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlMain.Controls.Add(btnSubmit);
            pnlMain.Controls.Add(btnCancel);
            this.Controls.Add(pnlMain);

            this.Shown += (s, e) => {
                txtFirst?.Focus();
            };
        }

        private string GetTitleByMode()
        {
            switch (AppConfig.InputMode)
            {
                case CarrierInputMode.WorkOrderOnly:
                    return "【僅工單模式】請輸入工單號碼";
                case CarrierInputMode.Hybrid:
                    return "【混合模式】請輸入 Cassette ID 與 工單號碼";
                case CarrierInputMode.BarcodeBinding:
                default:
                    return "【條碼綁定模式】請輸入 Cassette ID";
            }
        }

        private void Submit()
        {
            if (AppConfig.InputMode == CarrierInputMode.Hybrid)
            {
                CassetteId = txtFirst?.Text?.Trim() ?? "";
                WorkNo = txtSecond?.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(CassetteId) || string.IsNullOrEmpty(WorkNo))
                {
                    MessageBox.Show("Cassette ID 與 工單號碼皆不可為空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (string.IsNullOrEmpty(CassetteId)) txtFirst?.Focus();
                    else txtSecond?.Focus();
                    return;
                }
            }
            else if (AppConfig.InputMode == CarrierInputMode.WorkOrderOnly)
            {
                WorkNo = txtFirst?.Text?.Trim() ?? "";
                CassetteId = WorkNo;

                if (string.IsNullOrEmpty(WorkNo))
                {
                    MessageBox.Show("工單號碼不可為空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirst?.Focus();
                    return;
                }
            }
            else
            {
                CassetteId = txtFirst?.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(CassetteId))
                {
                    MessageBox.Show("Cassette ID 不可為空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirst?.Focus();
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
