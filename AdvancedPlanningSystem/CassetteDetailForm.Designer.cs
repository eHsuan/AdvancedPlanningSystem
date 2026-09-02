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
            pnlHeader = new System.Windows.Forms.Panel();
            lblHeader = new System.Windows.Forms.Label();
            mainSplitContainer = new System.Windows.Forms.SplitContainer();
            grpScoring = new System.Windows.Forms.GroupBox();
            tlpScores = new System.Windows.Forms.TableLayoutPanel();
            lblNameQTime = new System.Windows.Forms.Label();
            lblValQTime = new System.Windows.Forms.Label();
            lblNameUrgent = new System.Windows.Forms.Label();
            lblValUrgent = new System.Windows.Forms.Label();
            lblNameEng = new System.Windows.Forms.Label();
            lblValEng = new System.Windows.Forms.Label();
            lblNameDue = new System.Windows.Forms.Label();
            lblValDue = new System.Windows.Forms.Label();
            lblNameLead = new System.Windows.Forms.Label();
            lblValLead = new System.Windows.Forms.Label();
            lblSeparator = new System.Windows.Forms.Label();
            lblTotalLabel = new System.Windows.Forms.Label();
            lblTotalScore = new System.Windows.Forms.Label();
            grpQueue = new System.Windows.Forms.GroupBox();
            pnlRightInner = new System.Windows.Forms.Panel();
            lstBatchQueue = new System.Windows.Forms.ListBox();
            lblEqpInfo = new System.Windows.Forms.Label();
            lblDecision = new System.Windows.Forms.Label();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            grpScoring.SuspendLayout();
            tlpScores.SuspendLayout();
            grpQueue.SuspendLayout();
            pnlRightInner.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = System.Drawing.Color.DarkSlateBlue;
            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Location = new System.Drawing.Point(0, 0);
            pnlHeader.Margin = new System.Windows.Forms.Padding(4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new System.Windows.Forms.Padding(23, 25, 23, 25);
            pnlHeader.Size = new System.Drawing.Size(1148, 100);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeader
            // 
            lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.White;
            lblHeader.Location = new System.Drawing.Point(23, 25);
            lblHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new System.Drawing.Size(1102, 50);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "卡匣詳細診斷";
            lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            mainSplitContainer.Location = new System.Drawing.Point(0, 100);
            mainSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(grpScoring);
            mainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(12);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(grpQueue);
            mainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(12);
            mainSplitContainer.Size = new System.Drawing.Size(1148, 789);
            mainSplitContainer.SplitterDistance = 560;
            mainSplitContainer.SplitterWidth = 5;
            mainSplitContainer.TabIndex = 1;
            // 
            // grpScoring
            // 
            grpScoring.Controls.Add(tlpScores);
            grpScoring.Dock = System.Windows.Forms.DockStyle.Fill;
            grpScoring.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            grpScoring.Location = new System.Drawing.Point(12, 12);
            grpScoring.Margin = new System.Windows.Forms.Padding(4);
            grpScoring.Name = "grpScoring";
            grpScoring.Padding = new System.Windows.Forms.Padding(12);
            grpScoring.Size = new System.Drawing.Size(536, 765);
            grpScoring.TabIndex = 0;
            grpScoring.TabStop = false;
            grpScoring.Text = "演算法評分細節 (Scoring Breakdown)";
            // 
            // tlpScores
            // 
            tlpScores.ColumnCount = 2;
            tlpScores.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.36364F));
            tlpScores.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.63636F));
            tlpScores.Controls.Add(lblNameQTime, 0, 0);
            tlpScores.Controls.Add(lblValQTime, 1, 0);
            tlpScores.Controls.Add(lblNameUrgent, 0, 1);
            tlpScores.Controls.Add(lblValUrgent, 1, 1);
            tlpScores.Controls.Add(lblNameEng, 0, 2);
            tlpScores.Controls.Add(lblValEng, 1, 2);
            tlpScores.Controls.Add(lblNameDue, 0, 3);
            tlpScores.Controls.Add(lblValDue, 1, 3);
            tlpScores.Controls.Add(lblNameLead, 0, 4);
            tlpScores.Controls.Add(lblValLead, 1, 4);
            tlpScores.Controls.Add(lblSeparator, 0, 5);
            tlpScores.Controls.Add(lblTotalLabel, 0, 6);
            tlpScores.Controls.Add(lblTotalScore, 1, 6);
            tlpScores.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpScores.Location = new System.Drawing.Point(12, 31);
            tlpScores.Margin = new System.Windows.Forms.Padding(4);
            tlpScores.Name = "tlpScores";
            tlpScores.RowCount = 8;
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpScores.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlpScores.Size = new System.Drawing.Size(512, 722);
            tlpScores.TabIndex = 0;
            // 
            // lblNameQTime
            // 
            lblNameQTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblNameQTime.AutoSize = true;
            lblNameQTime.Location = new System.Drawing.Point(4, 0);
            lblNameQTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameQTime.Name = "lblNameQTime";
            lblNameQTime.Size = new System.Drawing.Size(108, 20);
            lblNameQTime.TabIndex = 0;
            lblNameQTime.Text = "1. QTime 分數";
            // 
            // lblValQTime
            // 
            lblValQTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblValQTime.AutoSize = true;
            lblValQTime.Location = new System.Drawing.Point(241, 0);
            lblValQTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValQTime.Name = "lblValQTime";
            lblValQTime.Size = new System.Drawing.Size(18, 20);
            lblValQTime.TabIndex = 1;
            lblValQTime.Text = "0";
            // 
            // lblNameUrgent
            // 
            lblNameUrgent.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblNameUrgent.AutoSize = true;
            lblNameUrgent.Location = new System.Drawing.Point(4, 20);
            lblNameUrgent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameUrgent.Name = "lblNameUrgent";
            lblNameUrgent.Size = new System.Drawing.Size(153, 20);
            lblNameUrgent.TabIndex = 2;
            lblNameUrgent.Text = "2. 急件加權 (Urgent)";
            // 
            // lblValUrgent
            // 
            lblValUrgent.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblValUrgent.AutoSize = true;
            lblValUrgent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            lblValUrgent.ForeColor = System.Drawing.Color.Red;
            lblValUrgent.Location = new System.Drawing.Point(241, 20);
            lblValUrgent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValUrgent.Name = "lblValUrgent";
            lblValUrgent.Size = new System.Drawing.Size(89, 20);
            lblValUrgent.TabIndex = 3;
            lblValUrgent.Text = "+ 100,000";
            // 
            // lblNameEng
            // 
            lblNameEng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblNameEng.AutoSize = true;
            lblNameEng.Location = new System.Drawing.Point(4, 40);
            lblNameEng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameEng.Name = "lblNameEng";
            lblNameEng.Size = new System.Drawing.Size(133, 20);
            lblNameEng.TabIndex = 4;
            lblNameEng.Text = "3. 工程加權 (Eng)";
            // 
            // lblValEng
            // 
            lblValEng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblValEng.AutoSize = true;
            lblValEng.Location = new System.Drawing.Point(241, 40);
            lblValEng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValEng.Name = "lblValEng";
            lblValEng.Size = new System.Drawing.Size(18, 20);
            lblValEng.TabIndex = 5;
            lblValEng.Text = "0";
            // 
            // lblNameDue
            // 
            lblNameDue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblNameDue.AutoSize = true;
            lblNameDue.Location = new System.Drawing.Point(4, 60);
            lblNameDue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameDue.Name = "lblNameDue";
            lblNameDue.Size = new System.Drawing.Size(134, 20);
            lblNameDue.TabIndex = 6;
            lblNameDue.Text = "4. 交期分數 (Due)";
            // 
            // lblValDue
            // 
            lblValDue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblValDue.AutoSize = true;
            lblValDue.Location = new System.Drawing.Point(241, 60);
            lblValDue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValDue.Name = "lblValDue";
            lblValDue.Size = new System.Drawing.Size(71, 20);
            lblValDue.TabIndex = 7;
            lblValDue.Text = "+ 50,000";
            // 
            // lblNameLead
            // 
            lblNameLead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblNameLead.AutoSize = true;
            lblNameLead.Location = new System.Drawing.Point(4, 80);
            lblNameLead.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameLead.Name = "lblNameLead";
            lblNameLead.Size = new System.Drawing.Size(140, 20);
            lblNameLead.TabIndex = 8;
            lblNameLead.Text = "5. 排隊分數 (Lead)";
            // 
            // lblValLead
            // 
            lblValLead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblValLead.AutoSize = true;
            lblValLead.Location = new System.Drawing.Point(241, 80);
            lblValLead.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValLead.Name = "lblValLead";
            lblValLead.Size = new System.Drawing.Size(49, 20);
            lblValLead.TabIndex = 9;
            lblValLead.Text = "+ 500";
            // 
            // lblSeparator
            // 
            lblSeparator.AutoSize = true;
            tlpScores.SetColumnSpan(lblSeparator, 2);
            lblSeparator.Location = new System.Drawing.Point(4, 100);
            lblSeparator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSeparator.Name = "lblSeparator";
            lblSeparator.Size = new System.Drawing.Size(259, 20);
            lblSeparator.TabIndex = 10;
            lblSeparator.Text = "--------------------------------------------------";
            // 
            // lblTotalLabel
            // 
            lblTotalLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblTotalLabel.AutoSize = true;
            lblTotalLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            lblTotalLabel.Location = new System.Drawing.Point(4, 143);
            lblTotalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTotalLabel.Name = "lblTotalLabel";
            lblTotalLabel.Size = new System.Drawing.Size(182, 22);
            lblTotalLabel.TabIndex = 11;
            lblTotalLabel.Text = "總分 (Total Score):";
            // 
            // lblTotalScore
            // 
            lblTotalScore.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblTotalScore.AutoSize = true;
            lblTotalScore.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            lblTotalScore.ForeColor = System.Drawing.Color.Blue;
            lblTotalScore.Location = new System.Drawing.Point(241, 138);
            lblTotalScore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTotalScore.Name = "lblTotalScore";
            lblTotalScore.Size = new System.Drawing.Size(112, 32);
            lblTotalScore.TabIndex = 12;
            lblTotalScore.Text = "150,500";
            // 
            // grpQueue
            // 
            grpQueue.Controls.Add(pnlRightInner);
            grpQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            grpQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            grpQueue.Location = new System.Drawing.Point(12, 12);
            grpQueue.Margin = new System.Windows.Forms.Padding(4);
            grpQueue.Name = "grpQueue";
            grpQueue.Padding = new System.Windows.Forms.Padding(12);
            grpQueue.Size = new System.Drawing.Size(559, 765);
            grpQueue.TabIndex = 0;
            grpQueue.TabStop = false;
            grpQueue.Text = "目標機台與湊批隊列 (Target & Batch)";
            // 
            // pnlRightInner
            // 
            pnlRightInner.Controls.Add(lstBatchQueue);
            pnlRightInner.Controls.Add(lblEqpInfo);
            pnlRightInner.Controls.Add(lblDecision);
            pnlRightInner.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRightInner.Location = new System.Drawing.Point(12, 31);
            pnlRightInner.Margin = new System.Windows.Forms.Padding(4);
            pnlRightInner.Name = "pnlRightInner";
            pnlRightInner.Size = new System.Drawing.Size(535, 722);
            pnlRightInner.TabIndex = 0;
            // 
            // lstBatchQueue
            // 
            lstBatchQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            lstBatchQueue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            lstBatchQueue.Font = new System.Drawing.Font("Consolas", 11F);
            lstBatchQueue.FormattingEnabled = true;
            lstBatchQueue.ItemHeight = 30;
            lstBatchQueue.Location = new System.Drawing.Point(0, 95);
            lstBatchQueue.Margin = new System.Windows.Forms.Padding(4);
            lstBatchQueue.Name = "lstBatchQueue";
            lstBatchQueue.Size = new System.Drawing.Size(535, 565);
            lstBatchQueue.TabIndex = 1;
            // 
            // lblEqpInfo
            // 
            lblEqpInfo.AutoSize = true;
            lblEqpInfo.Dock = System.Windows.Forms.DockStyle.Top;
            lblEqpInfo.Location = new System.Drawing.Point(0, 0);
            lblEqpInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEqpInfo.Name = "lblEqpInfo";
            lblEqpInfo.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            lblEqpInfo.Size = new System.Drawing.Size(199, 95);
            lblEqpInfo.TabIndex = 0;
            lblEqpInfo.Text = "目標機台: EQP-01 (乾蝕刻)\r\n狀態: RUNNING\r\nWIP: 2 / 10\r\n標準批次量: 4";
            // 
            // lblDecision
            // 
            lblDecision.BackColor = System.Drawing.Color.ForestGreen;
            lblDecision.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblDecision.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            lblDecision.ForeColor = System.Drawing.Color.White;
            lblDecision.Location = new System.Drawing.Point(0, 660);
            lblDecision.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDecision.Name = "lblDecision";
            lblDecision.Size = new System.Drawing.Size(535, 62);
            lblDecision.TabIndex = 2;
            lblDecision.Text = "🚀 結論：DISPATCH (派貨)";
            lblDecision.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CassetteDetailForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1148, 889);
            Controls.Add(mainSplitContainer);
            Controls.Add(pnlHeader);
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CassetteDetailForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "APS - 單一卡匣診斷 (Detail Drill-down)";
            pnlHeader.ResumeLayout(false);
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            grpScoring.ResumeLayout(false);
            tlpScores.ResumeLayout(false);
            tlpScores.PerformLayout();
            grpQueue.ResumeLayout(false);
            pnlRightInner.ResumeLayout(false);
            pnlRightInner.PerformLayout();
            ResumeLayout(false);

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
