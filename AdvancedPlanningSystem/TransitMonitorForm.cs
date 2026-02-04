using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem
{
    public class TransitMonitorForm : Form
    {
        private DataGridView _dgv;
        private ApsLocalDbRepository _repo;
        private Timer _timer;

        public TransitMonitorForm()
        {
            InitializeComponent();
            _repo = new ApsLocalDbRepository();
            
            _timer = new Timer();
            _timer.Interval = 2000;
            _timer.Tick += (s, e) => RefreshData();
            _timer.Start();
            
            RefreshData();
        }

        private void InitializeComponent()
        {
            this.Text = "運送中監控 (Transit Monitor)";
            this.Size = new Size(800, 500);

            _dgv = new DataGridView();
            _dgv.Dock = DockStyle.Fill;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgv.ReadOnly = true;
            _dgv.AllowUserToAddRows = false;
            _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Context Menu for Deletion
            var ctxMenu = new ContextMenuStrip();
            var itemDel = new ToolStripMenuItem("移除紀錄 (Force Remove)");
            itemDel.Click += ItemDel_Click;
            ctxMenu.Items.Add(itemDel);
            _dgv.ContextMenuStrip = ctxMenu;

            this.Controls.Add(_dgv);
        }

        private void RefreshData()
        {
            var data = _repo.GetAllTransits();
            _dgv.DataSource = data;
        }

        private void ItemDel_Click(object sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count > 0)
            {
                var carrierId = _dgv.SelectedRows[0].Cells["CarrierId"].Value.ToString();
                if (MessageBox.Show($"確定要移除 {carrierId} 嗎?", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _repo.RemoveTransit(carrierId);
                    RefreshData();
                }
            }
        }
    }
}
