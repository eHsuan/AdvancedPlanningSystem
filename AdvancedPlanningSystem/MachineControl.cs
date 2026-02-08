using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public partial class MachineControl : UserControl
    {
        private string _eqpId;
        private string _stepId;
        public string EqpId => _eqpId;
        public string StepId => _stepId;

        // UI Controls (Display Only)
        private GroupBox grpMachine;
        private Label lblStatus;
        private Label lblWip;
        private Label lblStep;
        private ProgressBar pbWip;

        public MachineControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.grpMachine = new GroupBox();
            this.lblStatus = new Label();
            this.lblWip = new Label();
            this.lblStep = new Label();
            this.pbWip = new ProgressBar();
            this.SuspendLayout();

            // grpMachine
            this.grpMachine.Controls.Add(this.pbWip);
            this.grpMachine.Controls.Add(this.lblStep);
            this.grpMachine.Controls.Add(this.lblWip);
            this.grpMachine.Controls.Add(this.lblStatus);
            this.grpMachine.Dock = DockStyle.Fill;
            this.grpMachine.Font = new Font("Arial", 9F, FontStyle.Bold);
            this.grpMachine.Location = new Point(0, 0);
            this.grpMachine.Size = new Size(180, 120);
            this.grpMachine.Text = "MachineID";

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Arial", 9F);
            this.lblStatus.Location = new Point(10, 25);
            this.lblStatus.Text = "Status: UNKNOWN";

            // lblStep
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new Font("Arial", 9F);
            this.lblStep.Location = new Point(10, 45);
            this.lblStep.Text = "Step: -";

            // lblWip
            this.lblWip.AutoSize = true;
            this.lblWip.Font = new Font("Arial", 9F);
            this.lblWip.Location = new Point(10, 65);
            this.lblWip.Text = "WIP: 0 / 0";

            // pbWip
            this.pbWip.Location = new Point(10, 85);
            this.pbWip.Size = new Size(155, 15);
            this.pbWip.Style = ProgressBarStyle.Continuous;

            // MachineControl
            this.Controls.Add(this.grpMachine);
            this.Size = new Size(180, 120);
            this.ResumeLayout(false);
        }

        public void BindData(string id, string step, string status, int curWip, int maxWip)
        {
            _eqpId = id;
            _stepId = step;
            grpMachine.Text = id;
            lblStep.Text = $"Step: {step}";
            lblStatus.Text = $"Status: {status}";

            if (status == "RUN") lblStatus.ForeColor = Color.Green;
            else if (status == "DOWN") lblStatus.ForeColor = Color.Red;
            else if (status == "IDLE") lblStatus.ForeColor = Color.Orange;
            else lblStatus.ForeColor = Color.Black;

            lblWip.Text = $"WIP: {curWip} / {maxWip}";
            
            if (maxWip > 0)
            {
                pbWip.Maximum = maxWip;
                pbWip.Value = Math.Min(curWip, maxWip);
                
                if (curWip >= maxWip) pbWip.ForeColor = Color.Red;
                else pbWip.ForeColor = Color.Green;
            }
        }
    }
}