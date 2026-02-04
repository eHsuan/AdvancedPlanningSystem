using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem
{
    public partial class GlobalMonitorForm : Form
    {
        private ApsLocalDbRepository _repo;
        private Timer _refreshTimer;

        public GlobalMonitorForm()
        {
            InitializeComponent();
            _repo = new ApsLocalDbRepository(); // 初始化 Repo

            // 訂閱 DataGridView 事件
            dgvLeaderboard.CellFormatting += DgvLeaderboard_CellFormatting;
            dgvLeaderboard.CellDoubleClick += DgvLeaderboard_CellDoubleClick;

            InitializeLogs();
            
            // 啟動刷新 Timer
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 2000; // 2秒刷新
            _refreshTimer.Tick += (s, e) => RefreshLeaderboard();
            _refreshTimer.Start();

            RefreshLeaderboard();
        }

        private void InitializeLogs()
        {
            lstSystemLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] 系統監控啟動...");
            lstSystemLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] 連線至 DB... 成功");
        }

        private void RefreshLeaderboard()
        {
            // 從 DB 讀取真實資料 (Sorted Wait Bindings)
            // 為了讓 DataGridView 能顯示，我們需要將 StateBinding 轉換為與 Grid 相容的物件
            // 由於原 LeaderboardItem 結構適合顯示，我們可以 Mapping 過去
            
            var bindings = _repo.GetSortedWaitBindings();
            
            // Mapping StateBinding -> LeaderboardItem
            var displayList = bindings.Select((b, index) => new LeaderboardItem(
                index + 1,
                b.PortId,
                b.CarrierId,
                b.LotId,
                b.TargetEqpId ?? b.NextStepId, // Target or NextStep
                GetPriorityText(b.PriorityType),
                (int)b.DispatchScore,
                string.IsNullOrEmpty(b.DispatchTime) ? "WAIT" : "DISPATCHING"
            )).ToList();

            // 為了避免閃爍，可以保存 Scroll 位置，但這裡簡單重設 DataSource
            dgvLeaderboard.DataSource = displayList;
        }

        private string GetPriorityText(int p)
        {
            if (p == 2) return "急件 (Urgent)";
            if (p == 1) return "工程 (Eng)";
            return "一般 (Normal)";
        }

        private void DgvLeaderboard_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 1. 批次線：前 4 名 (RowIndex 0~3) 背景顯示淺綠色
            if (e.RowIndex >= 0 && e.RowIndex <= 3)
            {
                e.CellStyle.BackColor = Color.FromArgb(220, 255, 220);
            }

            // 2. 優先級顏色處理
            // 確認目前處理的是 "Priority" 欄位
            if (dgvLeaderboard.Columns[e.ColumnIndex].Name == "colPriority" && e.Value != null)
            {
                string value = e.Value.ToString();

                if (value.Contains("急件"))
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
                else if (value.Contains("工程"))
                {
                    e.CellStyle.ForeColor = Color.Purple;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }
        }

        private void DgvLeaderboard_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 忽略標頭點擊 (RowIndex < 0)
            if (e.RowIndex < 0) return;

            // 取得綁定的資料物件
            var item = dgvLeaderboard.Rows[e.RowIndex].DataBoundItem as LeaderboardItem;
            if (item != null)
            {
                // 開啟詳細資訊視窗
                CassetteDetailForm detailForm = new CassetteDetailForm(item.CassetteID, item.Port);
                detailForm.ShowDialog();
            }
        }
    }
}
