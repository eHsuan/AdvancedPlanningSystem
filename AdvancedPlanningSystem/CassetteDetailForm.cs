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
            lblHeader.Text = $"卡匣: {_cassetteId} (位於 Port {_portId})";

            var binding = _repo.GetBinding(_cassetteId);
            if (binding == null)
            {
                MessageBox.Show("查無此卡匣資料 (可能已出庫或未綁定)。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // 1. 顯示評分資訊
            DisplayScoringInfo(binding);

            // 2. 顯示隊列資訊
            DisplayQueueInfo(binding);

            // 3. 顯示決策結果
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
            // QTime 使用真實剩餘時間 T_Real
            double tReal = binding.TReal;
            if (tReal < 99999)
            {
                lblValQTime.Text = $"{binding.ScoreQTime:N0} (真實剩餘: {tReal:F0} min)";
                // 若 T_Real 小於 15 分鐘變紅 (更緊急的警示)
                lblValQTime.ForeColor = (tReal < 15) ? Color.Red : (tReal < 45 ? Color.Orange : Color.Black);
            }
            else
            {
                lblValQTime.Text = "0 (無限制)";
                lblValQTime.ForeColor = Color.Gray;
            }

            // Priority
            if (binding.PriorityType == 1) 
            {
                lblValUrgent.Text = "Engineering (工程)";
                lblValUrgent.ForeColor = Color.Blue;
            }
            else if (binding.PriorityType == 2)
            {
                lblValUrgent.Text = $"{binding.ScoreUrgent:N0} (急件)";
                lblValUrgent.ForeColor = Color.Red;
            }
            else
            {
                lblValUrgent.Text = "0 (一般)";
                lblValUrgent.ForeColor = Color.Black;
            }

            // 其他分數
            lblValEng.Text = binding.ScoreEng.ToString("N0");
            lblValDue.Text = binding.ScoreDue.ToString("N0");
            lblValLead.Text = binding.ScoreLead.ToString("N0");

            lblTotalScore.Text = binding.DispatchScore.ToString("N0");
        }

        private void DisplayQueueInfo(StateBinding currentBinding)
        {
            lstBatchQueue.Items.Clear();
            string nextStep = currentBinding.NextStepId;

            // 找出候選機台資訊
            var stepEqps = _repo.GetStepEqpMappings().Where(m => m.StepId == nextStep).ToList();
            string eqpInfoStr = $"下一站: {nextStep}\r\n可用機台數: {stepEqps.Count}";
            
            if (stepEqps.Any())
            {
                var firstEqpId = stepEqps.First().EqpId;
                var eqpConfig = _repo.GetEqpConfig(firstEqpId);
                if (eqpConfig != null)
                {
                    eqpInfoStr += $"\r\n標準批次量 (Batch): {eqpConfig.BatchSize}";
                    // 注意：這裡無法直接取得即時 WIP (需 DataSyncService)，暫時顯示靜態 Config
                    eqpInfoStr += $"\r\nMax WIP: {eqpConfig.MaxWipQty}";
                }
            }
            lblEqpInfo.Text = eqpInfoStr;

            // 建立隊列 (同站點的所有卡匣)
            // 包含 Wait 和 Dispatching (如果是剛派出的)
            var allBindings = _repo.GetAllBindings().Where(b => b.NextStepId == nextStep).ToList();
            
            // 排序：Dispatching 在最前 (已經派了)，然後是 Wait 依分數高低
            var sortedList = allBindings
                .OrderByDescending(b => !string.IsNullOrEmpty(b.DispatchTime)) // 派貨中優先
                .ThenByDescending(b => b.DispatchScore)
                .ToList();

            int batchSize = 4; // 預設，若有 config 則覆蓋
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
                // 批次分隔線
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
                lblDecision.Text = $"🚀 已派貨 (Target: {binding.TargetEqpId})";
                lblDecision.BackColor = Color.ForestGreen;
            }
            else if (binding.NextStepId == "END")
            {
                lblDecision.Text = "🏁 完工 (Finished)";
                lblDecision.BackColor = Color.MediumPurple;
            }
            else
            {
                // 顯示等待原因
                string reason = string.IsNullOrEmpty(binding.WaitReason) ? "Analyzing..." : binding.WaitReason;
                lblDecision.Text = $"⏳ 等待中: {reason}";
                
                // 根據原因變色
                if (reason.Contains("DOWN") || reason.Contains("FULL") || reason.Contains("No Route"))
                {
                    lblDecision.BackColor = Color.Crimson; // 異常阻塞
                }
                else
                {
                    lblDecision.BackColor = Color.Orange; // 正常排隊
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
