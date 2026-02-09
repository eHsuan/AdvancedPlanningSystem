namespace AdvancedPlanningSystem
{
    partial class CassetteDetailForm
    {
        /// <summary>
        /// 設計器所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計器產生的程式碼

        /// <summary>
        /// 此為設計器支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.grpScoring = new System.Windows.Forms.GroupBox();
            this.tlpScores = new System.Windows.Forms.TableLayoutPanel();
            this.lblNameQTime = new System.Windows.Forms.Label();
            this.lblValQTime = new System.Windows.Forms.Label();
            this.lblNameUrgent = new System.Windows.Forms.Label();
            this.lblValUrgent = new System.Windows.Forms.Label();
            this.lblNameEng = new System.Windows.Forms.Label();
            this.lblValEng = new System.Windows.Forms.Label();
            this.lblNameDue = new System.Windows.Forms.Label();
            this.lblValDue = new System.Windows.Forms.Label();
            this.lblNameLead = new System.Windows.Forms.Label();
            this.lblValLead = new System.Windows.Forms.Label();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblTotalScore = new System.Windows.Forms.Label();
            this.grpQueue = new System.Windows.Forms.GroupBox();
            this.pnlRightInner = new System.Windows.Forms.Panel();
            this.lstBatchQueue = new System.Windows.Forms.ListBox();
            this.lblEqpInfo = new System.Windows.Forms.Label();
            this.lblDecision = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.grpScoring.SuspendLayout();
            this.tlpScores.SuspendLayout();
            this.grpQueue.SuspendLayout();
            this.pnlRightInner.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20);
            this.pnlHeader.Size = new System.Drawing.Size(984, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(20, 20);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(944, 40);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "卡匣詳細診斷";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 80);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.grpScoring);
            this.mainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.grpQueue);
            this.mainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.mainSplitContainer.Size = new System.Drawing.Size(984, 631);
            this.mainSplitContainer.SplitterDistance = 480;
            this.mainSplitContainer.TabIndex = 1;
            // 
            // grpScoring
            // 
            this.grpScoring.Controls.Add(this.tlpScores);
            this.grpScoring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpScoring.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grpScoring.Location = new System.Drawing.Point(10, 10);
            this.grpScoring.Name = "grpScoring";
            this.grpScoring.Padding = new System.Windows.Forms.Padding(10);
            this.grpScoring.Size = new System.Drawing.Size(460, 611);
            this.grpScoring.TabIndex = 0;
            this.grpScoring.TabStop = false;
            this.grpScoring.Text = "演算法評分細節 (Scoring Breakdown)";
            // 
            // tlpScores
            // 
            this.tlpScores.ColumnCount = 2;
            this.tlpScores.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.36364F));
            this.tlpScores.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.63636F));
            this.tlpScores.Controls.Add(this.lblNameQTime, 0, 0);
            this.tlpScores.Controls.Add(this.lblValQTime, 1, 0);
            this.tlpScores.Controls.Add(this.lblNameUrgent, 0, 1);
            this.tlpScores.Controls.Add(this.lblValUrgent, 1, 1);
            this.tlpScores.Controls.Add(this.lblNameEng, 0, 2);
            this.tlpScores.Controls.Add(this.lblValEng, 1, 2);
            this.tlpScores.Controls.Add(this.lblNameDue, 0, 3);
            this.tlpScores.Controls.Add(this.lblValDue, 1, 3);
            this.tlpScores.Controls.Add(this.lblNameLead, 0, 4);
            this.tlpScores.Controls.Add(this.lblValLead, 1, 4);
            this.tlpScores.Controls.Add(this.lblSeparator, 0, 5);
            this.tlpScores.Controls.Add(this.lblTotalLabel, 0, 6);
            this.tlpScores.Controls.Add(this.lblTotalScore, 1, 6);
            this.tlpScores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpScores.Location = new System.Drawing.Point(10, 29);
            this.tlpScores.Name = "tlpScores";
            this.tlpScores.RowCount = 8;
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpScores.Size = new System.Drawing.Size(440, 572);
            this.tlpScores.TabIndex = 0;
            // 
            // lblNameQTime
            // 
            this.lblNameQTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameQTime.AutoSize = true;
            this.lblNameQTime.Location = new System.Drawing.Point(3, 0);
            this.lblNameQTime.Name = "lblNameQTime";
            this.lblNameQTime.Size = new System.Drawing.Size(108, 20);
            this.lblNameQTime.TabIndex = 0;
            this.lblNameQTime.Text = "1. QTime 分數";
            // 
            // lblValQTime
            // 
            this.lblValQTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValQTime.AutoSize = true;
            this.lblValQTime.Location = new System.Drawing.Point(207, 0);
            this.lblValQTime.Name = "lblValQTime";
            this.lblValQTime.Size = new System.Drawing.Size(18, 20);
            this.lblValQTime.TabIndex = 1;
            this.lblValQTime.Text = "0";
            // 
            // lblNameUrgent
            // 
            this.lblNameUrgent.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameUrgent.AutoSize = true;
            this.lblNameUrgent.Location = new System.Drawing.Point(3, 20);
            this.lblNameUrgent.Name = "lblNameUrgent";
            this.lblNameUrgent.Size = new System.Drawing.Size(153, 20);
            this.lblNameUrgent.TabIndex = 2;
            this.lblNameUrgent.Text = "2. 急件加權 (Urgent)";
            // 
            // lblValUrgent
            // 
            this.lblValUrgent.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValUrgent.AutoSize = true;
            this.lblValUrgent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblValUrgent.ForeColor = System.Drawing.Color.Red;
            this.lblValUrgent.Location = new System.Drawing.Point(207, 20);
            this.lblValUrgent.Name = "lblValUrgent";
            this.lblValUrgent.Size = new System.Drawing.Size(89, 20);
            this.lblValUrgent.TabIndex = 3;
            this.lblValUrgent.Text = "+ 100,000";
            // 
            // lblNameEng
            // 
            this.lblNameEng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameEng.AutoSize = true;
            this.lblNameEng.Location = new System.Drawing.Point(3, 40);
            this.lblNameEng.Name = "lblNameEng";
            this.lblNameEng.Size = new System.Drawing.Size(133, 20);
            this.lblNameEng.TabIndex = 4;
            this.lblNameEng.Text = "3. 工程加權 (Eng)";
            // 
            // lblValEng
            // 
            this.lblValEng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValEng.AutoSize = true;
            this.lblValEng.Location = new System.Drawing.Point(207, 40);
            this.lblValEng.Name = "lblValEng";
            this.lblValEng.Size = new System.Drawing.Size(18, 20);
            this.lblValEng.TabIndex = 5;
            this.lblValEng.Text = "0";
            // 
            // lblNameDue
            // 
            this.lblNameDue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameDue.AutoSize = true;
            this.lblNameDue.Location = new System.Drawing.Point(3, 60);
            this.lblNameDue.Name = "lblNameDue";
            this.lblNameDue.Size = new System.Drawing.Size(134, 20);
            this.lblNameDue.TabIndex = 6;
            this.lblNameDue.Text = "4. 交期分數 (Due)";
            // 
            // lblValDue
            // 
            this.lblValDue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValDue.AutoSize = true;
            this.lblValDue.Location = new System.Drawing.Point(207, 60);
            this.lblValDue.Name = "lblValDue";
            this.lblValDue.Size = new System.Drawing.Size(71, 20);
            this.lblValDue.TabIndex = 7;
            this.lblValDue.Text = "+ 50,000";
            // 
            // lblNameLead
            // 
            this.lblNameLead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameLead.AutoSize = true;
            this.lblNameLead.Location = new System.Drawing.Point(3, 80);
            this.lblNameLead.Name = "lblNameLead";
            this.lblNameLead.Size = new System.Drawing.Size(140, 20);
            this.lblNameLead.TabIndex = 8;
            this.lblNameLead.Text = "5. 排隊分數 (Lead)";
            // 
            // lblValLead
            // 
            this.lblValLead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValLead.AutoSize = true;
            this.lblValLead.Location = new System.Drawing.Point(207, 80);
            this.lblValLead.Name = "lblValLead";
            this.lblValLead.Size = new System.Drawing.Size(49, 20);
            this.lblValLead.TabIndex = 9;
            this.lblValLead.Text = "+ 500";
            // 
            // lblSeparator
            // 
            this.lblSeparator.AutoSize = true;
            this.tlpScores.SetColumnSpan(this.lblSeparator, 2);
            this.lblSeparator.Location = new System.Drawing.Point(3, 100);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(259, 20);
            this.lblSeparator.TabIndex = 10;
            this.lblSeparator.Text = "--------------------------------------------------";
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTotalLabel.AutoSize = true;
            this.lblTotalLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalLabel.Location = new System.Drawing.Point(3, 135);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(182, 22);
            this.lblTotalLabel.TabIndex = 11;
            this.lblTotalLabel.Text = "總分 (Total Score):";
            // 
            // lblTotalScore
            // 
            this.lblTotalScore.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTotalScore.AutoSize = true;
            this.lblTotalScore.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotalScore.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalScore.Location = new System.Drawing.Point(207, 130);
            this.lblTotalScore.Name = "lblTotalScore";
            this.lblTotalScore.Size = new System.Drawing.Size(112, 32);
            this.lblTotalScore.TabIndex = 12;
            this.lblTotalScore.Text = "150,500";
            // 
            // grpQueue
            // 
            this.grpQueue.Controls.Add(this.pnlRightInner);
            this.grpQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grpQueue.Location = new System.Drawing.Point(10, 10);
            this.grpQueue.Name = "grpQueue";
            this.grpQueue.Padding = new System.Windows.Forms.Padding(10);
            this.grpQueue.Size = new System.Drawing.Size(480, 611);
            this.grpQueue.TabIndex = 0;
            this.grpQueue.TabStop = false;
            this.grpQueue.Text = "目標機台與湊批隊列 (Target & Batch)";
            // 
            // pnlRightInner
            // 
            this.pnlRightInner.Controls.Add(this.lstBatchQueue);
            this.pnlRightInner.Controls.Add(this.lblEqpInfo);
            this.pnlRightInner.Controls.Add(this.lblDecision);
            this.pnlRightInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightInner.Location = new System.Drawing.Point(10, 29);
            this.pnlRightInner.Name = "pnlRightInner";
            this.pnlRightInner.Size = new System.Drawing.Size(460, 572);
            this.pnlRightInner.TabIndex = 0;
            // 
            // lstBatchQueue
            // 
            this.lstBatchQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBatchQueue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstBatchQueue.Font = new System.Drawing.Font("Consolas", 11F);
            this.lstBatchQueue.FormattingEnabled = true;
            this.lstBatchQueue.ItemHeight = 30;
            this.lstBatchQueue.Location = new System.Drawing.Point(0, 92);
            this.lstBatchQueue.Name = "lstBatchQueue";
            this.lstBatchQueue.Size = new System.Drawing.Size(460, 430);
            this.lstBatchQueue.TabIndex = 1;
            // 
            // lblEqpInfo
            // 
            this.lblEqpInfo.AutoSize = true;
            this.lblEqpInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEqpInfo.Location = new System.Drawing.Point(0, 0);
            this.lblEqpInfo.Name = "lblEqpInfo";
            this.lblEqpInfo.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lblEqpInfo.Size = new System.Drawing.Size(199, 92);
            this.lblEqpInfo.TabIndex = 0;
            this.lblEqpInfo.Text = "目標機台: EQP-01 (乾蝕刻)\r\n狀態: RUNNING\r\nWIP: 2 / 10\r\n標準批次量: 4";
            // 
            // lblDecision
            // 
            this.lblDecision.BackColor = System.Drawing.Color.ForestGreen;
            this.lblDecision.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDecision.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblDecision.ForeColor = System.Drawing.Color.White;
            this.lblDecision.Location = new System.Drawing.Point(0, 522);
            this.lblDecision.Name = "lblDecision";
            this.lblDecision.Size = new System.Drawing.Size(460, 50);
            this.lblDecision.TabIndex = 2;
            this.lblDecision.Text = "🚀 結論：DISPATCH (派貨)";
            this.lblDecision.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CassetteDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 711);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.pnlHeader);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CassetteDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "APS - 單一卡匣診斷 (Detail Drill-down)";
            this.pnlHeader.ResumeLayout(false);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.grpScoring.ResumeLayout(false);
            this.tlpScores.ResumeLayout(false);
            this.tlpScores.PerformLayout();
            this.grpQueue.ResumeLayout(false);
            this.pnlRightInner.ResumeLayout(false);
            this.pnlRightInner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.GroupBox grpScoring;
        private System.Windows.Forms.TableLayoutPanel tlpScores;
        private System.Windows.Forms.Label lblNameQTime;
        private System.Windows.Forms.Label lblValQTime;
        private System.Windows.Forms.Label lblNameUrgent;
        private System.Windows.Forms.Label lblValUrgent;
        private System.Windows.Forms.Label lblNameEng;
        private System.Windows.Forms.Label lblValEng;
        private System.Windows.Forms.Label lblNameDue;
        private System.Windows.Forms.Label lblValDue;
        private System.Windows.Forms.Label lblNameLead;
        private System.Windows.Forms.Label lblValLead;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Label lblTotalLabel;
        private System.Windows.Forms.Label lblTotalScore;
        private System.Windows.Forms.GroupBox grpQueue;
        private System.Windows.Forms.Panel pnlRightInner;
        private System.Windows.Forms.ListBox lstBatchQueue;
        private System.Windows.Forms.Label lblEqpInfo;
        private System.Windows.Forms.Label lblDecision;
    }
}
