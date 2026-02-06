namespace AdvancedPlanningSystem
{
    partial class GlobalMonitorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.rbEng = new System.Windows.Forms.RadioButton();
            this.cmbEqp = new System.Windows.Forms.ComboBox();
            this.cmbStep = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.rbUrgent = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvLeaderboard = new System.Windows.Forms.DataGridView();
            this.colRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCassetteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWorkNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetEQP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lstSystemLog = new System.Windows.Forms.ListBox();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaderboard)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.rbEng);
            this.pnlTop.Controls.Add(this.cmbEqp);
            this.pnlTop.Controls.Add(this.cmbStep);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.rbUrgent);
            this.pnlTop.Controls.Add(this.rbAll);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(1200, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // rbEng
            // 
            this.rbEng.AutoSize = true;
            this.rbEng.Location = new System.Drawing.Point(180, 15);
            this.rbEng.Name = "rbEng";
            this.rbEng.Size = new System.Drawing.Size(71, 16);
            this.rbEng.TabIndex = 7;
            this.rbEng.Text = "只看工程";
            this.rbEng.UseVisualStyleBackColor = true;
            // 
            // cmbEqp
            // 
            this.cmbEqp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEqp.FormattingEnabled = true;
            this.cmbEqp.Location = new System.Drawing.Point(480, 14);
            this.cmbEqp.Name = "cmbEqp";
            this.cmbEqp.Size = new System.Drawing.Size(150, 20);
            this.cmbEqp.TabIndex = 6;
            // 
            // cmbStep
            // 
            this.cmbStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStep.FormattingEnabled = true;
            this.cmbStep.Location = new System.Drawing.Point(320, 14);
            this.cmbStep.Name = "cmbStep";
            this.cmbStep.Size = new System.Drawing.Size(150, 20);
            this.cmbStep.TabIndex = 5;
            this.cmbStep.SelectedIndexChanged += new System.EventHandler(this.cmbStep_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.LightBlue;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(1080, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "手動刷新";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // rbUrgent
            // 
            this.rbUrgent.AutoSize = true;
            this.rbUrgent.Location = new System.Drawing.Point(100, 15);
            this.rbUrgent.Name = "rbUrgent";
            this.rbUrgent.Size = new System.Drawing.Size(71, 16);
            this.rbUrgent.TabIndex = 1;
            this.rbUrgent.Text = "只看急件";
            this.rbUrgent.UseVisualStyleBackColor = true;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(20, 15);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(47, 16);
            this.rbAll.TabIndex = 0;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "全部";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 50);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dgvLeaderboard);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.lstSystemLog);
            this.splitContainer.Size = new System.Drawing.Size(1200, 750);
            this.splitContainer.SplitterDistance = 550;
            this.splitContainer.TabIndex = 1;
            // 
            // dgvLeaderboard
            // 
            this.dgvLeaderboard.AllowUserToAddRows = false;
            this.dgvLeaderboard.AllowUserToDeleteRows = false;
            this.dgvLeaderboard.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLeaderboard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLeaderboard.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRank,
            this.colPort,
            this.colCassetteID,
            this.colWorkNo,
            this.colTargetEQP,
            this.colPriority,
            this.colTotalScore,
            this.colStatus});
            this.dgvLeaderboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLeaderboard.Location = new System.Drawing.Point(0, 0);
            this.dgvLeaderboard.Name = "dgvLeaderboard";
            this.dgvLeaderboard.ReadOnly = true;
            this.dgvLeaderboard.RowHeadersVisible = false;
            this.dgvLeaderboard.RowTemplate.Height = 24;
            this.dgvLeaderboard.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeaderboard.Size = new System.Drawing.Size(1200, 550);
            this.dgvLeaderboard.TabIndex = 0;
            // 
            // colRank
            // 
            this.colRank.DataPropertyName = "Rank";
            this.colRank.HeaderText = "排名";
            this.colRank.Name = "colRank";
            this.colRank.ReadOnly = true;
            // 
            // colPort
            // 
            this.colPort.DataPropertyName = "Port";
            this.colPort.HeaderText = "Port";
            this.colPort.Name = "colPort";
            this.colPort.ReadOnly = true;
            // 
            // colCassetteID
            // 
            this.colCassetteID.DataPropertyName = "CassetteID";
            this.colCassetteID.HeaderText = "卡匣ID";
            this.colCassetteID.Name = "colCassetteID";
            this.colCassetteID.ReadOnly = true;
            // 
            // colWorkNo
            // 
            this.colWorkNo.DataPropertyName = "WorkNo";
            this.colWorkNo.HeaderText = "工單號";
            this.colWorkNo.Name = "colWorkNo";
            this.colWorkNo.ReadOnly = true;
            // 
            // colTargetEQP
            // 
            this.colTargetEQP.DataPropertyName = "TargetEQP";
            this.colTargetEQP.HeaderText = "目標機台";
            this.colTargetEQP.Name = "colTargetEQP";
            this.colTargetEQP.ReadOnly = true;
            // 
            // colPriority
            // 
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.HeaderText = "優先級";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            // 
            // colTotalScore
            // 
            this.colTotalScore.DataPropertyName = "TotalScore";
            this.colTotalScore.HeaderText = "總分";
            this.colTotalScore.Name = "colTotalScore";
            this.colTotalScore.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "狀態";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // lstSystemLog
            // 
            this.lstSystemLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSystemLog.Font = new System.Drawing.Font("Consolas", 10F);
            this.lstSystemLog.FormattingEnabled = true;
            this.lstSystemLog.ItemHeight = 15;
            this.lstSystemLog.Location = new System.Drawing.Point(0, 0);
            this.lstSystemLog.Name = "lstSystemLog";
            this.lstSystemLog.Size = new System.Drawing.Size(1200, 196);
            this.lstSystemLog.TabIndex = 0;
            // 
            // GlobalMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlTop);
            this.Name = "GlobalMonitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "APS - 全局狀態監控 (Global Status Monitor)";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaderboard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.RadioButton rbUrgent;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvLeaderboard;
        private System.Windows.Forms.ListBox lstSystemLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCassetteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWorkNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetEQP;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RadioButton rbEng;
        private System.Windows.Forms.ComboBox cmbEqp;
        private System.Windows.Forms.ComboBox cmbStep;
    }
}