using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public partial class PortControl : UserControl
    {
        private PortStatus _status;

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
            this.lblCassetteID.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
            this.lblWorkNo.Click += (s, e) => this.InvokeOnClick(this, EventArgs.Empty);
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
            get { return lblCassetteID.Text; }
            set { lblCassetteID.Text = value; }
        }

        [Category("Custom Properties")]
        [Description("Work Order Number (WorkNo) to display.")]
        public string WorkNo
        {
            get { return lblWorkNo.Text; }
            set { lblWorkNo.Text = value; }
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
            }
        }

        private void UpdateBackColor()
        {
            switch (_status)
            {
                case PortStatus.Empty:
                    this.BackColor = Color.LightGray;
                    break;
                case PortStatus.Occupied:
                    this.BackColor = Color.SkyBlue;
                    break;
                case PortStatus.Dispatching:
                    this.BackColor = Color.LimeGreen;
                    break;
                case PortStatus.Error:
                    this.BackColor = Color.Red;
                    break;
                default:
                    this.BackColor = SystemColors.Control;
                    break;
            }
        }
    }
}
