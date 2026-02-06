using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem
{
    public partial class GlobalMonitorForm : Form
    {
        private ApsLocalDbRepository _repo;
        private List<ConfigStepEqp> _allMappings;

        public GlobalMonitorForm()
        {
            InitializeComponent();
            _repo = new ApsLocalDbRepository(); 

            dgvLeaderboard.AutoGenerateColumns = false;

            dgvLeaderboard.CellFormatting += DgvLeaderboard_CellFormatting;
            dgvLeaderboard.CellDoubleClick += DgvLeaderboard_CellDoubleClick;

            InitializeFilterControls();
            InitializeLogs();
            RefreshLeaderboard();
        }

        private void InitializeFilterControls()
        {
            try
            {
                _allMappings = _repo.GetStepEqpMappings();
                
                // 1. Initialize Step Menu
                cmbStep.Items.Clear();
                cmbStep.Items.Add("All Steps");
                var steps = _allMappings.Select(m => m.StepId).Distinct().OrderBy(s => s).ToList();
                foreach (var s in steps) cmbStep.Items.Add(s);
                cmbStep.SelectedIndex = 0;

                // 2. Initialize Equipment Menu
                cmbEqp.Items.Clear();
                cmbEqp.Items.Add("All Equipment");
                cmbEqp.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LogHelper.System.Error("InitializeFilterControls Error: " + ex.Message);
            }
        }

        private void cmbStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStep = cmbStep.SelectedItem?.ToString();
            
            cmbEqp.Items.Clear();
            cmbEqp.Items.Add("All Equipment");

            if (!string.IsNullOrEmpty(selectedStep) && selectedStep != "All Steps")
            {
                var eqps = _allMappings.Where(m => m.StepId == selectedStep)
                                       .Select(m => m.EqpId)
                                       .Distinct()
                                       .OrderBy(id => id)
                                       .ToList();
                foreach (var id in eqps) cmbEqp.Items.Add(id);
            }
            
            cmbEqp.SelectedIndex = 0;
        }

        private void InitializeLogs()
        {
            lstSystemLog.Items.Clear();
            lstSystemLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Monitor Window Started.");
            lstSystemLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Mode: Manual Refresh");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLeaderboard();
            lstSystemLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Data refreshed manually.");
            if (lstSystemLog.Items.Count > 50) lstSystemLog.Items.RemoveAt(0);
            lstSystemLog.SelectedIndex = lstSystemLog.Items.Count - 1;
        }

        private void RefreshLeaderboard()
        {
            // Read from DB
            var bindings = _repo.GetSortedWaitBindings();

            // 1. Priority Filtering
            if (rbUrgent.Checked)
            {
                bindings = bindings.Where(b => b.PriorityType == 2).ToList();
            }
            else if (rbEng.Checked)
            {
                bindings = bindings.Where(b => b.PriorityType == 1).ToList();
            }

            // 2. Step Filtering
            string selectedStep = cmbStep.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedStep) && selectedStep != "All Steps")
            {
                bindings = bindings.Where(b => b.NextStepId == selectedStep).ToList();
            }

            // 3. Equipment Filtering
            string selectedEqp = cmbEqp.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedEqp) && selectedEqp != "All Equipment")
            {
                bindings = bindings.Where(b => 
                    b.TargetEqpId == selectedEqp || 
                    (string.IsNullOrEmpty(b.TargetEqpId) && _allMappings.Any(m => m.StepId == b.NextStepId && m.EqpId == selectedEqp))
                ).ToList();
            }
            
            // Mapping StateBinding -> LeaderboardItem
            var displayList = bindings.Select((b, index) => new LeaderboardItem(
                index + 1,
                b.PortId,
                b.CarrierId,
                b.LotId,
                string.IsNullOrEmpty(b.TargetEqpId) ? b.NextStepId : b.TargetEqpId, 
                GetPriorityText(b.PriorityType),
                (int)b.DispatchScore,
                string.IsNullOrEmpty(b.DispatchTime) ? "WAIT" : "DISPATCHING"
            )).ToList();

            dgvLeaderboard.DataSource = displayList;
        }

        private string GetPriorityText(int p)
        {
            if (p == 2) return "Urgent";
            if (p == 1) return "Engineering";
            return "Normal";
        }

        private void DgvLeaderboard_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Batch Highlight: Top 4 (RowIndex 0~3)
            if (e.RowIndex >= 0 && e.RowIndex <= 3)
            {
                e.CellStyle.BackColor = Color.FromArgb(220, 255, 220);
            }

            if (dgvLeaderboard.Columns[e.ColumnIndex].Name == "colPriority" && e.Value != null)
            {
                string value = e.Value.ToString();
                if (value.Contains("Urgent"))
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
                else if (value.Contains("Engineering"))
                {
                    e.CellStyle.ForeColor = Color.Purple;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }
        }

        private void DgvLeaderboard_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvLeaderboard.Rows[e.RowIndex].DataBoundItem as LeaderboardItem;
            if (item != null)
            {
                CassetteDetailForm detailForm = new CassetteDetailForm(item.CassetteID, item.Port);
                detailForm.ShowDialog();
            }
        }
    }
}