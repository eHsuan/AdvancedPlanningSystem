using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;
using AdvancedPlanningSystem.Models;
using System.Collections.Generic;

namespace AdvancedPlanningSystem
{
    public partial class CassetteDetailForm : Form
    {
        private string _cassetteId;
        private string _portId;
        private ApsLocalDbRepository _repo;
        private Timer _refreshTimer;

        public CassetteDetailForm(string cassetteId, string portId)
        {
            _cassetteId = cassetteId;
            _portId = portId;
            _repo = new ApsLocalDbRepository();

            InitializeComponent();

            this.lstBatchQueue.DrawItem += LstBatchQueue_DrawItem;
            this.FormClosing += (s, e) => { _refreshTimer?.Stop(); _refreshTimer?.Dispose(); };

            LoadRealData();

            // 啟動自動刷新 (2秒一次)
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 2000;
            _refreshTimer.Tick += (s, e) => LoadRealData();
            _refreshTimer.Start();
        }

        private void LoadRealData()
        {
            lblHeader.Text = $"Carrier: {_cassetteId} (Port {_portId})";

            var binding = _repo.GetBinding(_cassetteId);
            if (binding == null)
            {
                NotificationForm.ShowAsync("Error", "No data found for this carrier (may have been checked out or not bound).", NotificationLevel.Warning, 5);
                this.Close();
                return;
            }

            // 1. Scoring Info
            DisplayScoringInfo(binding);

            // 2. Queue Info
            DisplayQueueInfo(binding);

            // 3. Decision Result
            DisplayDecision(binding);
        }

        private DateTime? ParseDbTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr)) return null;
            DateTime dt;
            if (DateTime.TryParseExact(timeStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt)) return dt;
            if (DateTime.TryParse(timeStr, out dt)) return dt;
            return null;
        }

        private void DisplayScoringInfo(StateBinding binding)
        {
            // QTime using T_Real
            double tReal = binding.TReal;
            if (tReal < 99999)
            {
                lblValQTime.Text = $"{binding.ScoreQTime:N0} (真實剩餘: {tReal:F0} min)";
                // Warning if T_Real < 15 mins
                lblValQTime.ForeColor = (tReal < 15) ? Color.Red : (tReal < 45 ? Color.Orange : Color.Black);
            }
            else
            {
                lblValQTime.Text = "0 (No Limit)";
                lblValQTime.ForeColor = Color.Gray;
            }

            // Priority
            if (binding.PriorityType == 1) 
            {
                lblValUrgent.Text = "Engineering";
                lblValUrgent.ForeColor = Color.Blue;
            }
            else if (binding.PriorityType == 2)
            {
                lblValUrgent.Text = $"{binding.ScoreUrgent:N0} (Urgent)";
                lblValUrgent.ForeColor = Color.Red;
            }
            else
            {
                lblValUrgent.Text = "0 (Normal)";
                lblValUrgent.ForeColor = Color.Black;
            }

            // Other scores
            lblValEng.Text = binding.ScoreEng.ToString("N0");
            lblValDue.Text = binding.ScoreDue.ToString("N0");
            lblValLead.Text = binding.ScoreLead.ToString("N0");

            lblTotalScore.Text = binding.DispatchScore.ToString("N0");
        }

        private void DisplayQueueInfo(StateBinding currentBinding)
        {
            lstBatchQueue.Items.Clear();
            string nextStep = currentBinding.NextStepId;

            // Find candidate eqp info
            var stepEqps = _repo.GetStepEqpMappings().Where(m => m.StepId == nextStep).ToList();
            string eqpInfoStr = $"Next Step: {nextStep}\r\nAvailable Eqps: {stepEqps.Count}";
            
            if (stepEqps.Any())
            {
                var firstEqpId = stepEqps.First().EqpId;
                var eqpConfig = _repo.GetEqpConfig(firstEqpId);
                if (eqpConfig != null)
                {
                    eqpInfoStr += $"\r\nStandard Batch Size: {eqpConfig.BatchSize}";
                    // Note: Real-time WIP requires DataSyncService, using static Config for now
                    eqpInfoStr += $"\r\nMax WIP: {eqpConfig.MaxWipQty}";
                }
            }
            lblEqpInfo.Text = eqpInfoStr;

            // Build Queue
            var allBindings = _repo.GetAllBindings().Where(b => b.NextStepId == nextStep).ToList();
            
            // Sort: Dispatching first, then Wait by score
            var sortedList = allBindings
                .OrderByDescending(b => !string.IsNullOrEmpty(b.DispatchTime)) 
                .ThenByDescending(b => b.DispatchScore)
                .ToList();

            int batchSize = 4; 
            if (stepEqps.Any())
            {
                var cfg = _repo.GetEqpConfig(stepEqps.First().EqpId);
                if (cfg != null) batchSize = cfg.BatchSize;
            }

            int count = 0;
            foreach (var b in sortedList)
            {
                bool isMe = (b.CarrierId == _cassetteId);
                string status = !string.IsNullOrEmpty(b.DispatchTime) ? "[MOVE]" : "[WAIT]";
                string text = $"{status} {b.CarrierId} (Score: {b.DispatchScore:N0})";
                
                if (isMe) text += " <--- YOU";

                lstBatchQueue.Items.Add(new QueueItem(text, isMe));
                
                count++;
                // Batch Separator
                if (count % batchSize == 0 && count < sortedList.Count)
                {
                    lstBatchQueue.Items.Add(new QueueItem($"--- Batch Cut ({count}) ---", false, true));
                }
            }
        }

        private void DisplayDecision(StateBinding binding)
        {
            if (!string.IsNullOrEmpty(binding.DispatchTime))
            {
                lblDecision.Text = $"🚀 Dispatched (Target: {binding.TargetEqpId})";
                lblDecision.BackColor = Color.ForestGreen;
            }
            else if (binding.NextStepId == "END")
            {
                lblDecision.Text = "🏁 Finished";
                lblDecision.BackColor = Color.MediumPurple;
            }
            else
            {
                // Display Wait Reason
                string reason = string.IsNullOrEmpty(binding.WaitReason) ? "Analyzing..." : binding.WaitReason;
                lblDecision.Text = $"⏳ Waiting: {reason}";
                
                // Color based on reason
                if (reason.Contains("DOWN") || reason.Contains("FULL") || reason.Contains("No Route"))
                {
                    lblDecision.BackColor = Color.Crimson; // Blocked
                }
                else
                {
                    lblDecision.BackColor = Color.Orange; // Normal Queue
                }
            }
        }

        private void LstBatchQueue_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            QueueItem item = (QueueItem)lstBatchQueue.Items[e.Index];

            if (item.IsCurrent) e.Graphics.FillRectangle(Brushes.LightYellow, e.Bounds);
            else if (item.IsSeparator) e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            else e.DrawBackground();

            Brush textBrush = Brushes.Black;
            Font font = e.Font;

            if (item.Text.Contains("MOVE")) { textBrush = Brushes.Green; font = new Font(font, FontStyle.Bold); }
            else if (item.IsCurrent) { font = new Font(font, FontStyle.Bold); }

            if (item.IsSeparator)
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(item.Text, font, Brushes.DimGray, e.Bounds, sf);
            }
            else
            {
                float y = e.Bounds.Y + (e.Bounds.Height - e.Graphics.MeasureString(item.Text, font).Height) / 2;
                e.Graphics.DrawString(item.Text, font, textBrush, e.Bounds.X + 5, y);
            }
            e.DrawFocusRectangle();
        }

        private class QueueItem
        {
            public string Text { get; set; }
            public bool IsCurrent { get; set; }
            public bool IsSeparator { get; set; }
            public QueueItem(string text, bool isCurrent, bool isSeparator = false)
            {
                Text = text; IsCurrent = isCurrent; IsSeparator = isSeparator;
            }
            public override string ToString() => Text;
        }
    }
}
