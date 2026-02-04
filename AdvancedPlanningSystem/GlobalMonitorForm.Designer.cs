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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.rbEqp2 = new System.Windows.Forms.RadioButton();
            this.rbEqp1 = new System.Windows.Forms.RadioButton();
            this.rbUrgent = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvLeaderboard = new System.Windows.Forms.DataGridView();
            this.colRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCassetteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.pnlTop.Controls.Add(this.rbEqp2);
            this.pnlTop.Controls.Add(this.rbEqp1);
            this.pnlTop.Controls.Add(this.rbUrgent);
            this.pnlTop.Controls.Add(this.rbAll);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(1200, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // rbEqp2
            // 
            this.rbEqp2.AutoSize = true;
            this.rbEqp2.Location = new System.Drawing.Point(300, 15);
            this.rbEqp2.Name = "rbEqp2";
            this.rbEqp2.Size = new System.Drawing.Size(61, 16);
            this.rbEqp2.TabIndex = 3;
            this.rbEqp2.Text = "EQP-02";
            this.rbEqp2.UseVisualStyleBackColor = true;
            // 
            // rbEqp1
            // 
            this.rbEqp1.AutoSize = true;
            this.rbEqp1.Location = new System.Drawing.Point(200, 15);
            this.rbEqp1.Name = "rbEqp1";
            this.rbEqp1.Size = new System.Drawing.Size(61, 16);
            this.rbEqp1.TabIndex = 2;
            this.rbEqp1.Text = "EQP-01";
            this.rbEqp1.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.RadioButton rbEqp2;
        private System.Windows.Forms.RadioButton rbEqp1;
        private System.Windows.Forms.RadioButton rbUrgent;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvLeaderboard;
        private System.Windows.Forms.ListBox lstSystemLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCassetteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetEQP;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}
