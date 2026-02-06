namespace AdvancedPlanningSystem
{
    partial class FormMain
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
            this.btnSystemTest = new System.Windows.Forms.Button();
            this.btnManualSync = new System.Windows.Forms.Button();
            this.flpLegend = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLegendError = new System.Windows.Forms.Label();
            this.pnlColorError = new System.Windows.Forms.Panel();
            this.lblLegendDispatching = new System.Windows.Forms.Label();
            this.pnlColorDispatching = new System.Windows.Forms.Panel();
            this.lblLegendOccupied = new System.Windows.Forms.Label();
            this.pnlColorOccupied = new System.Windows.Forms.Panel();
            this.lblLegendFinish = new System.Windows.Forms.Label();
            this.pnlColorFinish = new System.Windows.Forms.Panel();
            this.lblLegendEmpty = new System.Windows.Forms.Label();
            this.pnlColorEmpty = new System.Windows.Forms.Panel();
            this.btnGlobalMonitor = new System.Windows.Forms.Button();
            this.btnTransitMonitor = new System.Windows.Forms.Button();
            this.btnEqpMonitor = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSimStatus = new System.Windows.Forms.Panel();
            this.lblSimStatus = new System.Windows.Forms.Label();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.tlpShelf = new System.Windows.Forms.TableLayoutPanel();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.pnlHeader.SuspendLayout();
            this.flpLegend.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.DarkGray;
            this.pnlHeader.Controls.Add(this.btnSystemTest);
            this.pnlHeader.Controls.Add(this.btnManualSync);
            this.pnlHeader.Controls.Add(this.flpLegend);
            this.pnlHeader.Controls.Add(this.btnGlobalMonitor);
            this.pnlHeader.Controls.Add(this.btnTransitMonitor);
            this.pnlHeader.Controls.Add(this.btnEqpMonitor);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.pnlSimStatus);
            this.pnlHeader.Controls.Add(this.lblSimStatus);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1920, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnSystemTest
            // 
            this.btnSystemTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSystemTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSystemTest.Location = new System.Drawing.Point(675, 12);
            this.btnSystemTest.Name = "btnSystemTest";
            this.btnSystemTest.Size = new System.Drawing.Size(113, 36);
            this.btnSystemTest.TabIndex = 8;
            this.btnSystemTest.Text = "System Test";
            this.btnSystemTest.UseVisualStyleBackColor = true;
            // 
            // btnManualSync
            // 
            this.btnManualSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManualSync.BackColor = System.Drawing.Color.Orange;
            this.btnManualSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnManualSync.Location = new System.Drawing.Point(796, 12);
            this.btnManualSync.Name = "btnManualSync";
            this.btnManualSync.Size = new System.Drawing.Size(160, 36);
            this.btnManualSync.TabIndex = 9;
            this.btnManualSync.Text = "Manual Decision";
            this.btnManualSync.UseVisualStyleBackColor = false;
            this.btnManualSync.Visible = false;
            this.btnManualSync.Click += new System.EventHandler(this.btnManualSync_Click);
            // 
            // flpLegend
            // 
            this.flpLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpLegend.AutoSize = true;
            this.flpLegend.BackColor = System.Drawing.Color.Transparent;
            this.flpLegend.Controls.Add(this.lblLegendError);
            this.flpLegend.Controls.Add(this.pnlColorError);
            this.flpLegend.Controls.Add(this.lblLegendDispatching);
            this.flpLegend.Controls.Add(this.pnlColorDispatching);
            this.flpLegend.Controls.Add(this.lblLegendOccupied);
            this.flpLegend.Controls.Add(this.pnlColorOccupied);
            this.flpLegend.Controls.Add(this.lblLegendFinish);
            this.flpLegend.Controls.Add(this.pnlColorFinish);
            this.flpLegend.Controls.Add(this.lblLegendEmpty);
            this.flpLegend.Controls.Add(this.pnlColorEmpty);
            this.flpLegend.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpLegend.Location = new System.Drawing.Point(900, 15);
            this.flpLegend.Name = "flpLegend";
            this.flpLegend.Size = new System.Drawing.Size(450, 33);
            this.flpLegend.TabIndex = 2;
            this.flpLegend.WrapContents = false;
            // 
            // lblLegendError
            // 
            this.lblLegendError.AutoSize = true;
            this.lblLegendError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLegendError.ForeColor = System.Drawing.Color.White;
            this.lblLegendError.Location = new System.Drawing.Point(400, 5);
            this.lblLegendError.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.lblLegendError.Name = "lblLegendError";
            this.lblLegendError.Size = new System.Drawing.Size(45, 15);
            this.lblLegendError.TabIndex = 0;
            this.lblLegendError.Text = "HOLD";
            // 
            // pnlColorError
            // 
            this.pnlColorError.BackColor = System.Drawing.Color.Red;
            this.pnlColorError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorError.Location = new System.Drawing.Point(375, 6);
            this.pnlColorError.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.pnlColorError.Name = "pnlColorError";
            this.pnlColorError.Size = new System.Drawing.Size(15, 15);
            this.pnlColorError.TabIndex = 1;
            // 
            // lblLegendDispatching
            // 
            this.lblLegendDispatching.AutoSize = true;
            this.lblLegendDispatching.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLegendDispatching.ForeColor = System.Drawing.Color.White;
            this.lblLegendDispatching.Location = new System.Drawing.Point(324, 5);
            this.lblLegendDispatching.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.lblLegendDispatching.Name = "lblLegendDispatching";
            this.lblLegendDispatching.Size = new System.Drawing.Size(46, 15);
            this.lblLegendDispatching.TabIndex = 2;
            this.lblLegendDispatching.Text = "MOVE";
            // 
            // pnlColorDispatching
            // 
            this.pnlColorDispatching.BackColor = System.Drawing.Color.LimeGreen;
            this.pnlColorDispatching.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorDispatching.Location = new System.Drawing.Point(299, 6);
            this.pnlColorDispatching.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.pnlColorDispatching.Name = "pnlColorDispatching";
            this.pnlColorDispatching.Size = new System.Drawing.Size(15, 15);
            this.pnlColorDispatching.TabIndex = 3;
            // 
            // lblLegendOccupied
            // 
            this.lblLegendOccupied.AutoSize = true;
            this.lblLegendOccupied.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLegendOccupied.ForeColor = System.Drawing.Color.White;
            this.lblLegendOccupied.Location = new System.Drawing.Point(255, 5);
            this.lblLegendOccupied.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.lblLegendOccupied.Name = "lblLegendOccupied";
            this.lblLegendOccupied.Size = new System.Drawing.Size(39, 15);
            this.lblLegendOccupied.TabIndex = 4;
            this.lblLegendOccupied.Text = "WAIT";
            // 
            // pnlColorOccupied
            // 
            this.pnlColorOccupied.BackColor = System.Drawing.Color.SkyBlue;
            this.pnlColorOccupied.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorOccupied.Location = new System.Drawing.Point(230, 6);
            this.pnlColorOccupied.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.pnlColorOccupied.Name = "pnlColorOccupied";
            this.pnlColorOccupied.Size = new System.Drawing.Size(15, 15);
            this.pnlColorOccupied.TabIndex = 5;
            // 
            // lblLegendFinish
            // 
            this.lblLegendFinish.AutoSize = true;
            this.lblLegendFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLegendFinish.ForeColor = System.Drawing.Color.White;
            this.lblLegendFinish.Location = new System.Drawing.Point(179, 5);
            this.lblLegendFinish.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.lblLegendFinish.Name = "lblLegendFinish";
            this.lblLegendFinish.Size = new System.Drawing.Size(46, 15);
            this.lblLegendFinish.TabIndex = 8;
            this.lblLegendFinish.Text = "DONE";
            // 
            // pnlColorFinish
            // 
            this.pnlColorFinish.BackColor = System.Drawing.Color.MediumPurple;
            this.pnlColorFinish.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorFinish.Location = new System.Drawing.Point(154, 6);
            this.pnlColorFinish.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.pnlColorFinish.Name = "pnlColorFinish";
            this.pnlColorFinish.Size = new System.Drawing.Size(15, 15);
            this.pnlColorFinish.TabIndex = 9;
            // 
            // lblLegendEmpty
            // 
            this.lblLegendEmpty.AutoSize = true;
            this.lblLegendEmpty.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLegendEmpty.ForeColor = System.Drawing.Color.White;
            this.lblLegendEmpty.Location = new System.Drawing.Point(96, 5);
            this.lblLegendEmpty.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.lblLegendEmpty.Name = "lblLegendEmpty";
            this.lblLegendEmpty.Size = new System.Drawing.Size(53, 15);
            this.lblLegendEmpty.TabIndex = 6;
            this.lblLegendEmpty.Text = "EMPTY";
            // 
            // pnlColorEmpty
            // 
            this.pnlColorEmpty.BackColor = System.Drawing.Color.LightGray;
            this.pnlColorEmpty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorEmpty.Location = new System.Drawing.Point(71, 6);
            this.pnlColorEmpty.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.pnlColorEmpty.Name = "pnlColorEmpty";
            this.pnlColorEmpty.Size = new System.Drawing.Size(15, 15);
            this.pnlColorEmpty.TabIndex = 7;
            // 
            // btnGlobalMonitor
            // 
            this.btnGlobalMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGlobalMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnGlobalMonitor.Location = new System.Drawing.Point(1730, 12);
            this.btnGlobalMonitor.Name = "btnGlobalMonitor";
            this.btnGlobalMonitor.Size = new System.Drawing.Size(160, 36);
            this.btnGlobalMonitor.TabIndex = 1;
            this.btnGlobalMonitor.Text = "Global Rank";
            this.btnGlobalMonitor.UseVisualStyleBackColor = true;
            // 
            // btnTransitMonitor
            // 
            this.btnTransitMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransitMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnTransitMonitor.Location = new System.Drawing.Point(1560, 12);
            this.btnTransitMonitor.Name = "btnTransitMonitor";
            this.btnTransitMonitor.Size = new System.Drawing.Size(160, 36);
            this.btnTransitMonitor.TabIndex = 3;
            this.btnTransitMonitor.Text = "Trans Monitor";
            this.btnTransitMonitor.UseVisualStyleBackColor = true;
            // 
            // btnEqpMonitor
            // 
            this.btnEqpMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEqpMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnEqpMonitor.Location = new System.Drawing.Point(1390, 12);
            this.btnEqpMonitor.Name = "btnEqpMonitor";
            this.btnEqpMonitor.Size = new System.Drawing.Size(160, 36);
            this.btnEqpMonitor.TabIndex = 4;
            this.btnEqpMonitor.Text = "EQ Monitor ";
            this.btnEqpMonitor.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "實體貨架監控 (Physical Shelf Monitor)";
            // 
            // pnlSimStatus
            // 
            this.pnlSimStatus.BackColor = System.Drawing.Color.Red;
            this.pnlSimStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSimStatus.Location = new System.Drawing.Point(515, 20);
            this.pnlSimStatus.Name = "pnlSimStatus";
            this.pnlSimStatus.Size = new System.Drawing.Size(20, 20);
            this.pnlSimStatus.TabIndex = 5;
            // 
            // lblSimStatus
            // 
            this.lblSimStatus.AutoSize = true;
            this.lblSimStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSimStatus.ForeColor = System.Drawing.Color.White;
            this.lblSimStatus.Location = new System.Drawing.Point(540, 22);
            this.lblSimStatus.Name = "lblSimStatus";
            this.lblSimStatus.Size = new System.Drawing.Size(129, 17);
            this.lblSimStatus.TabIndex = 6;
            this.lblSimStatus.Text = "Simulator Offline";
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.tlpShelf);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 60);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(1920, 821);
            this.pnlGrid.TabIndex = 1;
            // 
            // tlpShelf
            // 
            this.tlpShelf.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tlpShelf.ColumnCount = 11;
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            this.tlpShelf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpShelf.Location = new System.Drawing.Point(0, 0);
            this.tlpShelf.Name = "tlpShelf";
            this.tlpShelf.RowCount = 8;
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpShelf.Size = new System.Drawing.Size(1920, 821);
            this.tlpShelf.TabIndex = 0;
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstLog.Font = new System.Drawing.Font("Consolas", 10F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 15;
            this.lstLog.Location = new System.Drawing.Point(0, 881);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(1920, 199);
            this.lstLog.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "APS - Advanced Planning System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.flpLegend.ResumeLayout(false);
            this.flpLegend.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnGlobalMonitor;
        private System.Windows.Forms.Button btnTransitMonitor;
        private System.Windows.Forms.Button btnEqpMonitor;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSimStatus;
        private System.Windows.Forms.Label lblSimStatus;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.TableLayoutPanel tlpShelf;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.FlowLayoutPanel flpLegend;
        private System.Windows.Forms.Label lblLegendEmpty;
        private System.Windows.Forms.Panel pnlColorEmpty;
        private System.Windows.Forms.Label lblLegendOccupied;
        private System.Windows.Forms.Panel pnlColorOccupied;
        private System.Windows.Forms.Label lblLegendDispatching;
        private System.Windows.Forms.Panel pnlColorDispatching;
        private System.Windows.Forms.Label lblLegendError;
        private System.Windows.Forms.Panel pnlColorError;
        private System.Windows.Forms.Button btnSystemTest;
        private System.Windows.Forms.Button btnManualSync;
        private System.Windows.Forms.Label lblLegendFinish;
        private System.Windows.Forms.Panel pnlColorFinish;
    }
}