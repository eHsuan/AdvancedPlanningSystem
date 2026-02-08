using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace APSSimulator
{
    public partial class SimulationGanttForm : Form
    {
        private FormMain _mainForm;
        private string _logContent;
        private List<GanttEvent> _events = new List<GanttEvent>();
        private Dictionary<string, int> _cstPriorities = new Dictionary<string, int>(); // 新增：記錄 CST 優先級
        
        // 繪圖參數
        private int _rowHeight = 45;
        private int _leftMargin = 120;
        private int _topMargin = 40;
        private double _pixelsPerSecond = 3.0;
        private DateTime _minTime = DateTime.MaxValue;
        private DateTime _maxTime = DateTime.MinValue;

        private Timer _autoRefreshTimer;

        public SimulationGanttForm(FormMain main, string currentLog)
        {
            InitializeComponent();
            _mainForm = main;
            _logContent = currentLog;

            // 隱藏原本的純文字標籤，改用 Paint 繪製圖例
            lblInfo.Visible = false;
            pnlTop.Paint += PnlTop_Paint;
            
            // 開啟雙緩衝避免閃爍
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, pnlChart, new object[] { true });

            ParseLogAndRefresh();

            // 初始化自動更新計時器 (10秒)
            _autoRefreshTimer = new Timer();
            _autoRefreshTimer.Interval = 10000;
            _autoRefreshTimer.Tick += (s, e) => {
                if (_mainForm.IsSimulating) btnUpdate_Click(null, null);
                else _autoRefreshTimer.Stop();
            };
            _autoRefreshTimer.Start();

            this.FormClosing += (s, e) => _autoRefreshTimer.Stop();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var logCtrl = _mainForm.Controls.Find("txtAutoSimLog", true).FirstOrDefault() as TextBox;
            if (logCtrl != null)
            {
                _logContent = logCtrl.Text;
                ParseLogAndRefresh();
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            // 檢查是否正在模擬
            if (_mainForm.IsSimulating)
            {
                MessageBox.Show("自動模擬運行中，禁止讀取外部檔案。請先停止模擬。");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog { Filter = "Text Files (*.txt)|*.txt" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _logContent = File.ReadAllText(ofd.FileName);
                ParseLogAndRefresh();
            }
        }

        private void ParseLogAndRefresh()
        {
            _events.Clear();
            _cstPriorities.Clear();
            
            // --- [關鍵修正] 使用模擬啟動時間作為最左側基準 ---
            _minTime = _mainForm.SimStartTime;
            _maxTime = DateTime.Now;

            // 建立合法 CST ID 白清單與優先級映射
            HashSet<string> validCstIds = new HashSet<string>();
            try {
                DataTable dt = _mainForm.Controls.Find("dgvAutoSimOrders", true).FirstOrDefault() is DataGridView dgv 
                               ? dgv.DataSource as DataTable : null;
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string cid = dr["carrier_id"].ToString();
                        validCstIds.Add(cid);
                        // 記錄優先級: 0=一般, 1=工程, 2=急件
                        int pri = 0;
                        int.TryParse(dr["priority_type"]?.ToString(), out pri);
                        _cstPriorities[cid] = pri;
                    }
                }
            } catch { }

            string[] lines = _logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, GanttEvent> activeEvents = new Dictionary<string, GanttEvent>();
            Regex logRegex = new Regex(@"^\[(?<time>\d{2}:\d{2}:\d{2})\]\s+(?<msg>.*)$");

            foreach (var line in lines)
            {
                var match = logRegex.Match(line);
                if (!match.Success) continue;

                DateTime time = DateTime.Parse(match.Groups["time"].Value);
                string msg = match.Groups["msg"].Value;

                string cstId = ExtractCstIdStrict(msg, validCstIds);

                if (msg.Contains("入庫") || msg.Contains("回存"))
                {
                    if (!string.IsNullOrEmpty(cstId))
                    {
                        CloseActiveEvent(activeEvents, cstId, time);
                        activeEvents[cstId] = new GanttEvent { CstId = cstId, Start = time, Location = ExtractPortId(msg), Type = EventType.Shelf };
                    }
                }
                else if (msg.Contains("PICK"))
                {
                    if (!string.IsNullOrEmpty(cstId))
                    {
                        CloseActiveEvent(activeEvents, cstId, time);
                        activeEvents[cstId] = new GanttEvent { CstId = cstId, Start = time, Location = "搬運中", Type = EventType.Transport };
                    }
                }
                else if (msg.Contains("ENTER"))
                {
                    string targetCst = !string.IsNullOrEmpty(cstId) ? cstId : 
                                      activeEvents.Values.FirstOrDefault(v => v.Type == EventType.Transport && v.End == DateTime.MinValue)?.CstId;
                    
                    if (!string.IsNullOrEmpty(targetCst))
                    {
                        CloseActiveEvent(activeEvents, targetCst, time);
                        // 從日誌中提取機台編號 (例如: [執行模擬] ENTER ET0026 (CST01))
                        string eqp = msg.Replace("[執行模擬] ENTER ", "").Split(' ').FirstOrDefault()?.Trim('(', ')', '[', ']', ';') ?? "機台";
                        activeEvents[targetCst] = new GanttEvent { CstId = targetCst, Start = time, Location = eqp, Type = EventType.Process };
                    }
                }
                else if (msg.Contains("製程結束") || msg.Contains("Pass99"))
                {
                    if (!string.IsNullOrEmpty(cstId)) CloseActiveEvent(activeEvents, cstId, time);
                }
            }

            foreach (var kvp in activeEvents)
            {
                if (kvp.Value.End == DateTime.MinValue) kvp.Value.End = _maxTime;
                _events.Add(kvp.Value);
            }

            if (_events.Count > 0)
            {
                int totalHeight = _events.Select(e => e.CstId).Distinct().Count() * _rowHeight + 100;
                int totalWidth = (int)((_maxTime - _minTime).TotalSeconds * _pixelsPerSecond) + 300;
                pnlChart.AutoScrollMinSize = new Size(totalWidth, totalHeight);
                
                // 自動滾動到最右邊 (最新進度)
                pnlChart.HorizontalScroll.Value = pnlChart.HorizontalScroll.Maximum;
            }
            pnlChart.Invalidate();
        }

        private string ExtractCstIdStrict(string msg, HashSet<string> validIds)
        {
            if (validIds == null || validIds.Count == 0) return "";
            var mParen = Regex.Match(msg, @"\((?<id>[^)]+)\)");
            if (mParen.Success && validIds.Contains(mParen.Groups["id"].Value)) return mParen.Groups["id"].Value;
            string[] words = msg.Split(new[] { ' ', '[', ']', '(', ')', '-', '>', ':', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var w in words) if (validIds.Contains(w)) return w;
            return "";
        }

        private void CloseActiveEvent(Dictionary<string, GanttEvent> active, string cstId, DateTime endTime)
        {
            if (active.ContainsKey(cstId))
            {
                var ev = active[cstId];
                if (endTime > ev.Start) ev.End = endTime;
                else ev.End = ev.Start.AddSeconds(1); // 防止時間倒流或重疊
                _events.Add(ev);
                active.Remove(cstId);
            }
        }

        private string ExtractCstId(string msg)
        {
            // 1. 優先找括號內的: (CST01)
            var mParen = Regex.Match(msg, @"\((?<id>[^)]+)\)");
            if (mParen.Success) return mParen.Groups["id"].Value;

            // 2. 特殊關鍵字提取
            if (msg.Contains("卡匣 ")) 
            {
                var m = Regex.Match(msg, @"卡匣\s+(?<id>[A-Za-z0-9_-]+)");
                if (m.Success) return m.Groups["id"].Value;
            }

            // 3. 排除指令關鍵字後提取第一個看起來像 ID 的單字
            string[] words = msg.Split(new[] { ' ', '[', ']', '(', ')', '-', '>', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] blacklist = { "執行模擬", "入庫模擬", "回存模擬", "製程結束", "PICK", "MOVE", "ENTER", "Pass99", "Port", "已完成過站", "已製程結束", "卡匣" };
            
            foreach (var w in words)
            {
                if (w.Length >= 4 && !blacklist.Contains(w) && !Regex.IsMatch(w, @"^P\d{2}$"))
                {
                    return w;
                }
            }
            return "";
        }

        private void PnlTop_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font f = new Font("Microsoft JhengHei", 9);
            int startX = 260;
            int y = 18;

            // 圖例 1: 等待派送
            DrawLegendItem(g, startX, y, Color.LightGreen, "等待派送", f);
            // 圖例 2: 搬運
            DrawLegendItem(g, startX + 100, y, Color.Gold, "搬運中", f);
            // 圖例 3: 製程中
            DrawLegendItem(g, startX + 200, y, Color.LightSkyBlue, "製程中", f);
            
            // 屬性圖例
            int startX2 = 560;
            g.DrawString("CST屬性:", f, Brushes.Gray, startX2, y);
            g.DrawString("急件", f, Brushes.Red, startX2 + 70, y);
            g.DrawString("工程", f, Brushes.Purple, startX2 + 120, y);
            g.DrawString("一般", f, Brushes.Black, startX2 + 170, y);
        }

        private void DrawLegendItem(Graphics g, int x, int y, Color color, string text, Font f)
        {
            g.FillRectangle(new SolidBrush(color), x, y, 15, 15);
            g.DrawRectangle(Pens.Black, x, y, 15, 15);
            g.DrawString(text, f, Brushes.Black, x + 20, y);
        }

        private string ExtractPortId(string msg)
        {
            // [修正] 優先匹配帶有 Port 關鍵字的組合，防止誤抓 P63X31 這種 ID
            var m = Regex.Match(msg, @"Port\s+(?<id>P\d{2})");
            if (m.Success) return m.Groups["id"].Value;

            m = Regex.Match(msg, @"空位\s+(?<id>P\d{2})");
            if (m.Success) return m.Groups["id"].Value;

            // 如果沒帶關鍵字，則進行嚴格的邊界匹配 \b (不匹配 P63X31)
            m = Regex.Match(msg, @"\bP\d{2}\b");
            return m.Success ? m.Value : "";
        }

        private void pnlChart_Paint(object sender, PaintEventArgs e)
        {
            if (_events.Count == 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 取得當前滾動位置
            int scrollX = pnlChart.AutoScrollPosition.X;
            int scrollY = pnlChart.AutoScrollPosition.Y;

            var cstList = _events.Select(ev => ev.CstId).Distinct().OrderBy(s => s).ToList();
            var cstYMap = new Dictionary<string, int>();
            for (int i = 0; i < cstList.Count; i++) cstYMap[cstList[i]] = i;

            // --- 第一層：受滾動影響的內容 (甘特條、格線、時間軸) ---
            g.TranslateTransform(scrollX, scrollY);

            Pen gridPen = new Pen(Color.LightGray, 1);
            Font labelFont = new Font("Arial", 8);

            for (int i = 0; i < cstList.Count; i++)
            {
                int y = _topMargin + i * _rowHeight;
                g.DrawLine(gridPen, _leftMargin, y, pnlChart.AutoScrollMinSize.Width, y);
            }

            foreach (var ev in _events)
            {
                if (!cstYMap.ContainsKey(ev.CstId)) continue;

                int y = _topMargin + cstYMap[ev.CstId] * _rowHeight + 5;
                int xStart = _leftMargin + (int)((ev.Start - _minTime).TotalSeconds * _pixelsPerSecond);
                int width = (int)((ev.End - ev.Start).TotalSeconds * _pixelsPerSecond);
                if (width < 5) width = 5; 

                Color barColor = Color.Gray;
                switch (ev.Type)
                {
                    case EventType.Shelf: barColor = Color.LightGreen; break;
                    case EventType.Transport: barColor = Color.Gold; break;
                    case EventType.Process: barColor = Color.LightSkyBlue; break;
                }

                Rectangle rect = new Rectangle(xStart, y, width, _rowHeight - 10);
                g.FillRectangle(new SolidBrush(barColor), rect);
                g.DrawRectangle(Pens.DimGray, rect);

                if (width > 20)
                {
                    g.DrawString(ev.Location, labelFont, Brushes.Black, xStart + 2, y + 8);
                }
            }

            // 繪製時間軸刻度
            int labelCounter = 0;
            for (int s = 0; s <= (_maxTime - _minTime).TotalSeconds; s += 10)
            {
                int x = _leftMargin + (int)(s * _pixelsPerSecond);
                g.DrawLine(Pens.Black, x, _topMargin - 5, x, _topMargin);
                int yPos = (labelCounter % 2 == 0) ? 5 : 20;
                g.DrawString(_minTime.AddSeconds(s).ToString("HH:mm:ss"), labelFont, Brushes.Black, x - 20, yPos);
                labelCounter++;
            }

            // --- 第二層：固定不動的標題列 (CST ID) ---
            g.ResetTransform();
            
            // 繪製標題列背景 (遮住水平滾動過來的內容)
            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, _leftMargin, pnlChart.Height);
            g.DrawLine(Pens.Gray, _leftMargin, 0, _leftMargin, pnlChart.Height);

            // 重新繪製標籤 (標籤只需跟隨「垂直」滾動，不隨水平滾動移動)
            g.TranslateTransform(0, scrollY);
            for (int i = 0; i < cstList.Count; i++)
            {
                string cid = cstList[i];
                int y = _topMargin + i * _rowHeight;
                
                Brush textBrush = Brushes.Black;
                if (_cstPriorities.ContainsKey(cid))
                {
                    int p = _cstPriorities[cid];
                    if (p == 1) textBrush = Brushes.Purple;
                    else if (p == 2) textBrush = Brushes.Red;
                }

                g.DrawString(cid, labelFont, textBrush, 10, y + 15);
            }
        }
    }

    public enum EventType { Shelf, Transport, Process }
    public class GanttEvent
    {
        public string CstId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; }
        public EventType Type { get; set; }
    }
}
