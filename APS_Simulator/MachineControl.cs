using System;
using System.Drawing;
using System.Windows.Forms;

namespace APSSimulator
{
    public partial class MachineControl : UserControl
    {
        public event EventHandler<string> OnStatusChange;
        public event EventHandler<int> OnWipChange;

        private string _eqpId;
        private int _maxWip = 10;
        public string EqpId => _eqpId;

        public MachineControl()
        {
            InitializeComponent();
            
            btnRun.Click += (s, e) => {
                UpdateStatusDisplay("RUN");
                OnStatusChange?.Invoke(this, "RUN");
            };
            btnIdle.Click += (s, e) => {
                UpdateStatusDisplay("IDLE");
                OnStatusChange?.Invoke(this, "IDLE");
            };
            btnDown.Click += (s, e) => {
                UpdateStatusDisplay("DOWN");
                OnStatusChange?.Invoke(this, "DOWN");
            };
            btnUpdateWip.Click += (s, e) => {
                int newWip = (int)numWip.Value;
                UpdateWipDisplay(newWip);
                OnWipChange?.Invoke(this, newWip);
            };
        }

        public void BindData(string id, string grp, string status, int curWip, int maxWip)
        {
            _eqpId = id;
            _maxWip = maxWip;
            grpMachine.Text = $"{grp} - {id}";
            
            UpdateStatusDisplay(status);
            UpdateWipDisplay(curWip);
        }

        public void UpdateStatusDisplay(string status)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => UpdateStatusDisplay(status)));
                return;
            }
            lblStatus.Text = $"Status: {status}";
            if (status == "RUN") lblStatus.ForeColor = Color.Green;
            else if (status == "DOWN") lblStatus.ForeColor = Color.Red;
            else lblStatus.ForeColor = Color.Black;
        }

        public void UpdateWipDisplay(int curWip)
        {
            if (lblWip.InvokeRequired)
            {
                lblWip.Invoke(new Action(() => UpdateWipDisplay(curWip)));
                return;
            }
            lblWip.Text = $"WIP: {curWip} / {_maxWip}";
            numWip.Maximum = _maxWip > 100 ? _maxWip : 100;
            if (curWip >= numWip.Minimum && curWip <= numWip.Maximum)
            {
                numWip.Value = curWip;
            }
        }
    }
}
