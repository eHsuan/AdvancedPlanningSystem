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
        private string _waitReason;
        private string _nextStepId;
        private bool _isFlashing;
        private Timer _flashTimer;
        private bool _flashToggle;

        public PortControl()
        {
            InitializeComponent();
            
            // 啟用雙緩衝
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                          ControlStyles.UserPaint | 
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            // 閃爍 Timer 初始化
            _flashTimer = new Timer();
            _flashTimer.Interval = 500;
            _flashTimer.Tick += (s, e) => {
                _flashToggle = !_flashToggle;
                UpdateBackColor();
            };

            Status = PortStatus.Empty;

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
            bool showWork = (h >= 75);
            lblWorkInfo.Visible = showWork && (_status != PortStatus.Empty);
            
            if (!showWork && (_status != PortStatus.Empty))
            {
                lblTargetInfo.Top = lblWorkInfo.Top;
            }
            else
            {
                lblTargetInfo.Top = 55;
            }
        }

        [Category("Custom Properties")]
        public string PortID
        {
            get { return lblPortID.Text; }
            set { if (lblPortID.Text != value) lblPortID.Text = value; }
        }

        [Category("Custom Properties")]
        public string CassetteID
        {
            get { return lblCstInfo.Text.Replace("CstID : ", ""); }
            set 
            { 
                string newVal = "CstID : " + value;
                if (lblCstInfo.Text != newVal)
                {
                    lblCstInfo.Text = newVal; 
                    UpdateToolTip(); 
                }
            }
        }

        [Category("Custom Properties")]
        public string WorkNo
        {
            get { return lblWorkInfo.Text.Replace("WorkNo : ", ""); }
            set 
            { 
                string newVal = "WorkNo : " + value;
                if (lblWorkInfo.Text != newVal)
                {
                    lblWorkInfo.Text = newVal; 
                    UpdateToolTip(); 
                }
            }
        }

        [Category("Custom Properties")]
        public string TargetEqpId
        {
            get { return _targetEqpId; }
            set 
            { 
                if (_targetEqpId != value)
                {
                    _targetEqpId = value; 
                    lblTargetInfo.Text = "Target : " + value; 
                    UpdateToolTip(); 
                    UpdateBackColor(); 
                }
            }
        }

        [Category("Custom Properties")]
        public string WaitReason
        {
            get { return _waitReason; }
            set 
            { 
                if (_waitReason != value)
                {
                    _waitReason = value; 
                    UpdateBackColor(); 
                }
            }
        }

        [Category("Custom Properties")]
        public string NextStepId
        {
            get { return _nextStepId; }
            set 
            { 
                if (_nextStepId != value)
                {
                    _nextStepId = value; 
                    UpdateBackColor(); 
                }
            }
        }

        [Category("Custom Properties")]
        public PortStatus Status
        {
            get { return _status; }
            set 
            { 
                if (_status != value)
                {
                    _status = value; 
                    UpdateBackColor(); 
                    AdjustLayout(); 
                }
            }
        }

        [Category("Custom Properties")]
        [Description("是否啟用閃爍效果 (用於緊急貨件)")]
        public bool IsFlashing
        {
            get { return _isFlashing; }
            set 
            { 
                if (_isFlashing != value)
                {
                    _isFlashing = value;
                    if (_isFlashing) _flashTimer.Start();
                    else { _flashTimer.Stop(); _flashToggle = false; UpdateBackColor(); }
                }
            }
        }

        private void UpdateToolTip()
        {
            if (_status == PortStatus.Empty)
            {
                toolTipInfo.SetToolTip(this, $"Port: {PortID}");
                return;
            }
            string tip = $"Port: {PortID}\nCstID: {CassetteID}\nWorkNo: {WorkNo}\nTarget: {_targetEqpId}\nStatus: {lblCstStatus.Text}";
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
            lblTargetInfo.Visible = hasContent && !string.IsNullOrEmpty(_targetEqpId);
            
            if (!hasContent) lblWorkInfo.Visible = false;

            switch (_status)
            {
                case PortStatus.Empty:
                    this.BackColor = Color.LightGray;
                    lblCstStatus.Text = "";
                    break;
                case PortStatus.Occupied:
                    this.BackColor = Color.SkyBlue;
                    lblCstStatus.Text = "WAIT";
                    break;
                case PortStatus.Dispatching:
                    // 閃爍邏輯：若為緊急件則在深綠與亮綠間切換
                    if (_isFlashing && _flashToggle) this.BackColor = Color.YellowGreen;
                    else this.BackColor = Color.LimeGreen;
                    lblCstStatus.Text = "MOVE";
                    break;
                case PortStatus.Finish:
                    this.BackColor = Color.MediumPurple;
                    lblCstStatus.Text = "DONE";
                    break;
                case PortStatus.Error:
                    this.BackColor = Color.Red;
                    lblCstStatus.Text = "HOLD";
                    break;
                default:
                    this.BackColor = SystemColors.Control;
                    break;
            }
        }
    }
}