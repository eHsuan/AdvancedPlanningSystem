using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public partial class PortControl : UserControl
    {
        private PortStatus _status;
        private string _targetEqpId;

        public PortControl()
        {
            InitializeComponent();
            
            // 啟用雙緩衝以減少閃爍
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                          ControlStyles.UserPaint | 
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            // 設定預設狀態
            Status = PortStatus.Empty;

            // 將內部控制項的 Click 事件轉發給 UserControl 本身
            this.lblPortID.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
            this.lblCstInfo.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
            this.lblWorkInfo.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
            this.lblTargetInfo.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int h = this.Height;
            // 高度判斷：完整顯示需約 80px
            // 順序：WorkNo 先隱藏 -> Target 隱藏 (或 Cst 隱藏，視需求)
            
            bool showWork = (h >= 75);
            // 若高度非常小，Target 是否優先於 Cst? 通常 Dispatching 時 Target 比較重要
            // 這裡暫時策略：只要非 Empty，Cst 永遠顯示 (佔位 22)，Target 在最下方
            // 如果高度 < 60，Target 可能會被切，考慮隱藏 WorkNo 騰出空間給 Target

            lblWorkInfo.Visible = showWork && (_status != PortStatus.Empty);
            
            // 如果 WorkInfo 隱藏，TargetInfo 往上移? 
            // 為了簡化，直接設定 Visible，不改變 Top 座標 (假設 Layout 夠用)
            // 若要更精緻，可動態調整 Top
            if (!showWork && (_status != PortStatus.Empty))
            {
                // Work 隱藏，Target 上移填補空缺
                lblTargetInfo.Top = lblWorkInfo.Top;
            }
            else
            {
                // 還原
                lblTargetInfo.Top = lblWorkInfo.Bottom + 2; // 假設間距
                // 從 Designer 看是 Work=38, Target=55 -> 差17
                lblTargetInfo.Top = 55;
            }
        }

        [Category("Custom Properties")]
        [Description("Port ID to display in top-left corner.")]
        public string PortID
        {
            get { return lblPortID.Text; }
            set { lblPortID.Text = value; }
        }

        [Category("Custom Properties")]
        [Description("Cassette ID to display.")]
        public string CassetteID
        {
            get { return lblCstInfo.Text.Replace("CstID : ", ""); }
            set 
            { 
                lblCstInfo.Text = "CstID : " + value; 
                UpdateToolTip();
            }
        }

        [Category("Custom Properties")]
        [Description("Work Order Number (WorkNo) to display.")]
        public string WorkNo
        {
            get { return lblWorkInfo.Text.Replace("WorkNo : ", ""); }
            set 
            { 
                lblWorkInfo.Text = "WorkNo : " + value; 
                UpdateToolTip();
            }
        }

        [Category("Custom Properties")]
        [Description("Target Equipment ID to display.")]
        public string TargetEqpId
        {
            get { return _targetEqpId; }
            set 
            { 
                _targetEqpId = value;
                lblTargetInfo.Text = "Target : " + value;
                UpdateToolTip();
                UpdateBackColor(); // Re-evaluate visibility
            }
        }

        [Category("Custom Properties")]
        [Description("Status text for the CST (e.g. WAIT, MOVE).")]
        public string CstStatusText
        {
            get { return lblCstStatus.Text; }
            set { lblCstStatus.Text = value; }
        }

        [Category("Custom Properties")]
        [Description("Current status of the port, which affects the background color.")]
        public PortStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                UpdateBackColor();
                AdjustLayout(); // Re-evaluate layout
            }
        }

        private void UpdateToolTip()
        {
            if (_status == PortStatus.Empty)
            {
                toolTipInfo.SetToolTip(this, $"Port: {PortID}");
                return;
            }
            
            string tip = $"Port: {PortID}\nCstID: {CassetteID}\nWorkNo: {WorkNo}";
            if (!string.IsNullOrEmpty(_targetEqpId))
            {
                tip += $"\nTarget: {_targetEqpId}";
            }
            tip += $"\nStatus: {lblCstStatus.Text}";

            toolTipInfo.SetToolTip(this, tip);
            toolTipInfo.SetToolTip(lblCstInfo, tip);
            toolTipInfo.SetToolTip(lblWorkInfo, tip);
            toolTipInfo.SetToolTip(lblTargetInfo, tip);
        }

        private void UpdateBackColor()
        {
            bool hasContent = (_status != PortStatus.Empty);
            lblCstStatus.Visible = hasContent;
            lblCstInfo.Visible = hasContent;
            
            bool hasTarget = hasContent && !string.IsNullOrEmpty(_targetEqpId);
            lblTargetInfo.Visible = hasTarget;
            
            // WorkInfo visibility is controlled by AdjustLayout based on height, 
            // but fundamentally it must have content
            if (!hasContent) lblWorkInfo.Visible = false;

            switch (_status)
            {
                case PortStatus.Empty:
                    this.BackColor = Color.LightGray;
                    lblCstStatus.Text = "";
                    break;
                case PortStatus.Occupied:
                    this.BackColor = Color.SkyBlue;
                    lblCstStatus.ForeColor = Color.DarkBlue;
                    lblCstStatus.Text = "WAIT";
                    break;
                case PortStatus.Dispatching:
                    this.BackColor = Color.LimeGreen;
                    lblCstStatus.ForeColor = Color.White;
                    lblCstStatus.Text = "Dispatching";
                    break;
                case PortStatus.Finish:
                    this.BackColor = Color.MediumPurple;
                    lblCstStatus.ForeColor = Color.White;
                    lblCstStatus.Text = "DONE";
                    break;
                case PortStatus.Error:
                    this.BackColor = Color.Red;
                    lblCstStatus.ForeColor = Color.Yellow;
                    lblCstStatus.Text = "ERROR";
                    break;
                default:
                    this.BackColor = SystemColors.Control;
                    break;
            }
        }
    }
}
