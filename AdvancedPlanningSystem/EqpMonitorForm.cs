using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;
using AdvancedPlanningSystem.Models;
using AdvancedPlanningSystem.Services;

namespace AdvancedPlanningSystem
{
    public class EqpMonitorForm : Form
    {
        private ApsLocalDbRepository _repo;
        private FlowLayoutPanel flowLayoutPanelMachines;
        private TextBox txtSearchMachine;
        private Button btnRefresh;
        private Label lblLastUpdate;

        public EqpMonitorForm()
        {
            InitializeComponent();
            _repo = new ApsLocalDbRepository();
            
            // Initial forced update when opening
            this.Load += async (s, e) => {
                var mainForm = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if (mainForm != null && mainForm.SyncService != null)
                {
                    await mainForm.SyncService.ForceUpdateCacheAsync();
                }
                LoadMachines();
            };
        }

        private void InitializeComponent()
        {
            this.txtSearchMachine = new TextBox();
            this.btnRefresh = new Button();
            this.lblLastUpdate = new Label();
            this.flowLayoutPanelMachines = new FlowLayoutPanel();
            Panel pnlTop = new Panel();
            Label lblSearch = new Label();

            this.SuspendLayout();

            // pnlTop
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 50;
            pnlTop.Controls.Add(lblSearch);
            pnlTop.Controls.Add(this.txtSearchMachine);
            pnlTop.Controls.Add(this.btnRefresh);
            pnlTop.Controls.Add(this.lblLastUpdate);

            // lblSearch
            lblSearch.Text = "Search Machine ID:";
            lblSearch.Location = new Point(12, 17);
            lblSearch.AutoSize = true;

            // txtSearchMachine
            this.txtSearchMachine.Location = new Point(140, 14);
            this.txtSearchMachine.Size = new Size(200, 25);
            this.txtSearchMachine.TextChanged += (s, e) => FilterMachines(txtSearchMachine.Text.Trim());

            // btnRefresh
            this.btnRefresh.Location = new Point(350, 12);
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += async (s, e) => {
                btnRefresh.Enabled = false;
                btnRefresh.Text = "Updating...";
                
                var mainForm = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if (mainForm != null && mainForm.SyncService != null)
                {
                    await mainForm.SyncService.ForceUpdateCacheAsync();
                }
                
                LoadMachines();
                
                btnRefresh.Text = "Refresh";
                btnRefresh.Enabled = true;
            };

            // lblLastUpdate
            this.lblLastUpdate.Location = new Point(460, 17);
            this.lblLastUpdate.Size = new Size(300, 25);
            this.lblLastUpdate.Text = "Last Update: -";

            // flowLayoutPanelMachines
            this.flowLayoutPanelMachines.Dock = DockStyle.Fill;
            this.flowLayoutPanelMachines.AutoScroll = true;
            this.flowLayoutPanelMachines.Padding = new Padding(10);

            // EqpMonitorForm
            this.ClientSize = new Size(1100, 700);
            this.Controls.Add(this.flowLayoutPanelMachines);
            this.Controls.Add(pnlTop);
            this.Text = "Equipment Real-time Monitor";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private void LoadMachines()
        {
            flowLayoutPanelMachines.SuspendLayout();
            flowLayoutPanelMachines.Controls.Clear();

            // 1. Get Static Configs from DB
            var eqpConfigs = _repo.GetAllEqpConfigs();
            var stepMap = _repo.GetStepEqpMappings();
            var transits = _repo.GetAllTransits();
            var bindings = _repo.GetAllBindings();

            // 2. Get Real-time Status from Cache
            Dictionary<string, EqStatusResponse> statusDict = new Dictionary<string, EqStatusResponse>();
            Dictionary<string, WipInfoResponse> wipDict = new Dictionary<string, WipInfoResponse>();

            var mainForm = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
            if (mainForm != null && mainForm.SyncService != null)
            {
                statusDict = mainForm.SyncService.GetCachedEqStatus();
                wipDict = mainForm.SyncService.GetCachedWip();
            }

            foreach (var config in eqpConfigs.OrderBy(c => c.EqpId))
            {
                var control = new MachineControl();
                string stepId = stepMap.FirstOrDefault(m => m.EqpId == config.EqpId)?.StepId ?? "N/A";
                
                // Real-time Status
                string status = "UNKNOWN";
                if (statusDict.ContainsKey(config.EqpId)) status = statusDict[config.EqpId].status;

                // WIP Calc: Pure MES WIP (Physical only to match Simulator)
                int mesWip = 0;
                if (wipDict.ContainsKey(config.EqpId)) mesWip = wipDict[config.EqpId].current_wip_qty;
                
                control.BindData(config.EqpId, stepId, status, mesWip, config.MaxWipQty);
                control.Margin = new Padding(10);
                flowLayoutPanelMachines.Controls.Add(control);
            }

            FilterMachines(txtSearchMachine.Text.Trim());
            lblLastUpdate.Text = $"Last Update: {DateTime.Now:HH:mm:ss}";
            flowLayoutPanelMachines.ResumeLayout();
        }

        private void FilterMachines(string keyword)
        {
            keyword = keyword.ToUpper();
            foreach (Control c in flowLayoutPanelMachines.Controls)
            {
                if (c is MachineControl mc)
                {
                    bool match = string.IsNullOrEmpty(keyword) || mc.EqpId.ToUpper().Contains(keyword);
                    mc.Visible = match;
                }
            }
        }
    }
}