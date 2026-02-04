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

        public MachineControl()
        {
            InitializeComponent();
            
            btnRun.Click += (s, e) => OnStatusChange?.Invoke(this, "RUN");
            btnIdle.Click += (s, e) => OnStatusChange?.Invoke(this, "IDLE");
            btnDown.Click += (s, e) => OnStatusChange?.Invoke(this, "DOWN");
            btnUpdateWip.Click += (s, e) => OnWipChange?.Invoke(this, (int)numWip.Value);
        }

        public void BindData(string id, string grp, string status, int curWip, int maxWip)
        {
            _eqpId = id;
            grpMachine.Text = $"{grp} - {id}";
            
            lblStatus.Text = $"Status: {status}";
            if (status == "RUN") lblStatus.ForeColor = Color.Green;
            else if (status == "DOWN") lblStatus.ForeColor = Color.Red;
            else lblStatus.ForeColor = Color.Black;

            lblWip.Text = $"WIP: {curWip} / {maxWip}";
            
            numWip.Maximum = maxWip > 100 ? maxWip : 100; // 簡單防呆
            numWip.Value = curWip;
        }
    }
}
