using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem
{
    public class EqpMonitorForm : Form
    {
        private ApsLocalDbRepository _repo;
        private FlowLayoutPanel _mainPanel;
        private Timer _timer;

        public EqpMonitorForm()
        {
            InitializeComponent();
            _repo = new ApsLocalDbRepository();
            
            _timer = new Timer();
            _timer.Interval = 5000;
            _timer.Tick += (s, e) => RefreshData();
            _timer.Start();
            
            RefreshData();
        }

        private void InitializeComponent()
        {
            this.Text = "機台狀態看板 (Equipment Monitor)";
            this.Size = new Size(1000, 600);
            this.AutoScroll = true;

            _mainPanel = new FlowLayoutPanel();
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.AutoScroll = true;
            _mainPanel.FlowDirection = FlowDirection.TopDown;
            _mainPanel.WrapContents = false;

            this.Controls.Add(_mainPanel);
        }

        private void RefreshData()
        {
            // 1. 取得資料
            var stepMap = _repo.GetStepEqpMappings();
            var eqpConfigs = _repo.GetAllEqpConfigs();
            var transits = _repo.GetAllTransits(); // 用來算 InTransit WIP

            // Group by Step
            var stepGroups = stepMap.GroupBy(x => x.StepId);

            _mainPanel.SuspendLayout();
            _mainPanel.Controls.Clear();

            foreach (var group in stepGroups)
            {
                // Step Container
                var stepBox = new GroupBox();
                stepBox.Text = $"Step: {group.Key}";
                stepBox.Size = new Size(950, 150); // Fixed height for now
                stepBox.Padding = new Padding(10);

                var eqpFlow = new FlowLayoutPanel();
                eqpFlow.Dock = DockStyle.Fill;
                eqpFlow.FlowDirection = FlowDirection.LeftToRight;
                
                foreach (var item in group)
                {
                    // Eqp Panel
                    var eqpId = item.EqpId;
                    var config = eqpConfigs.FirstOrDefault(c => c.EqpId == eqpId);
                    int maxWip = config?.MaxWipQty ?? 0;
                    
                    // WIP Calc (Note: Current WIP is not in Config table unless synced? 
                    // Actually SyncService updates MaxWip, but not CurrentWip into DB?
                    // Ah, DispatchService gets CurrentWip from MES but doesn't save it to DB.
                    // This is a gap. UI cannot show current WIP unless it calls MES or SyncService saves it.
                    // 暫時顯示 MaxWip 與 InTransit
                    int inTransit = transits.Count(t => t.TargetEqpId == eqpId);

                    var pnl = new Panel();
                    pnl.Size = new Size(120, 100);
                    pnl.BorderStyle = BorderStyle.FixedSingle;
                    pnl.BackColor = Color.WhiteSmoke;

                    var lblId = new Label { Text = eqpId, Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold), Location = new Point(5, 5), AutoSize = true };
                    var lblWip = new Label { Text = $"WIP Limit: {maxWip}", Location = new Point(5, 30), AutoSize = true };
                    var lblTransit = new Label { Text = $"In Transit: {inTransit}", Location = new Point(5, 50), AutoSize = true, ForeColor = Color.Blue };

                    pnl.Controls.Add(lblId);
                    pnl.Controls.Add(lblWip);
                    pnl.Controls.Add(lblTransit);
                    
                    eqpFlow.Controls.Add(pnl);
                }

                stepBox.Controls.Add(eqpFlow);
                _mainPanel.Controls.Add(stepBox);
            }

            _mainPanel.ResumeLayout();
        }
    }
}
