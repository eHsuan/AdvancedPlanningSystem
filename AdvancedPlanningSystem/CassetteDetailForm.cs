using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public partial class CassetteDetailForm : Form
    {
        private string _cassetteId;
        private string _portId;

        public CassetteDetailForm(string cassetteId, string portId)
        {
            _cassetteId = cassetteId;
            _portId = portId;

            // 呼叫由 Designer 產生的初始化方法
            InitializeComponent();

            // 訂閱自定義繪製事件 (這是邏輯部分，放在 .cs)
            this.lstBatchQueue.DrawItem += LstBatchQueue_DrawItem;

            LoadMockData();
        }

        private void LoadMockData()
        {
            lblHeader.Text = $"卡匣: {_cassetteId} (位於 Port {_portId})";

            // 模擬資料 (此處可進階為查詢資料庫或演算法引擎)
            lblValQTime.Text = "0  (剩餘安全時間: 120 min)";
            lblValUrgent.Text = "+ 100,000  (優先級: 急件)";
            lblValEng.Text = "0";
            lblValDue.Text = "+ 50,000  (剩餘 1 hr)";
            lblValLead.Text = "+ 500  (滯留 500 min)";
            lblTotalScore.Text = "150,500";

            // 模擬隊列
            lstBatchQueue.Items.Clear();
            lstBatchQueue.Items.Add(new QueueItem($"{_cassetteId} (急件) <--- [目前卡匣]", true));
            lstBatchQueue.Items.Add(new QueueItem("CASS-888 (工程)", false));
            lstBatchQueue.Items.Add(new QueueItem("CASS-777 (一般)", false));
            lstBatchQueue.Items.Add(new QueueItem("CASS-666 (一般)", false));
            lstBatchQueue.Items.Add(new QueueItem("--- 批次分隔線 (Batch Cut) ---", false, true));
            lstBatchQueue.Items.Add(new QueueItem("CASS-555 (候補)", false));
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

            if (item.Text.Contains("急件")) { textBrush = Brushes.Red; font = new Font(font, FontStyle.Bold); }
            else if (item.Text.Contains("工程")) { textBrush = Brushes.Purple; }

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
