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
            pnlHeader = new System.Windows.Forms.Panel();
            btnStockIn = new System.Windows.Forms.Button();
            lblModeDisplay = new System.Windows.Forms.Label();
            btnSystemTest = new System.Windows.Forms.Button();
            btnManualSync = new System.Windows.Forms.Button();
            flpLegend = new System.Windows.Forms.FlowLayoutPanel();
            lblLegendError = new System.Windows.Forms.Label();
            pnlColorError = new System.Windows.Forms.Panel();
            lblLegendDispatching = new System.Windows.Forms.Label();
            pnlColorDispatching = new System.Windows.Forms.Panel();
            lblLegendOccupied = new System.Windows.Forms.Label();
            pnlColorOccupied = new System.Windows.Forms.Panel();
            lblLegendFinish = new System.Windows.Forms.Label();
            pnlColorFinish = new System.Windows.Forms.Panel();
            lblLegendEmpty = new System.Windows.Forms.Label();
            pnlColorEmpty = new System.Windows.Forms.Panel();
            btnGlobalMonitor = new System.Windows.Forms.Button();
            btnTransitMonitor = new System.Windows.Forms.Button();
            btnEqpMonitor = new System.Windows.Forms.Button();
            pnlSimStatus = new System.Windows.Forms.Panel();
            lblSimStatus = new System.Windows.Forms.Label();
            pnlGrid = new System.Windows.Forms.Panel();
            tlpShelf = new System.Windows.Forms.TableLayoutPanel();
            lstLog = new System.Windows.Forms.ListBox();
            pnlHeader.SuspendLayout();
            flpLegend.SuspendLayout();
            pnlGrid.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = System.Drawing.Color.DarkGray;
            pnlHeader.Controls.Add(btnStockIn);
            pnlHeader.Controls.Add(lblModeDisplay);
            pnlHeader.Controls.Add(btnSystemTest);
            pnlHeader.Controls.Add(btnManualSync);
            pnlHeader.Controls.Add(flpLegend);
            pnlHeader.Controls.Add(btnGlobalMonitor);
            pnlHeader.Controls.Add(btnTransitMonitor);
            pnlHeader.Controls.Add(btnEqpMonitor);
            pnlHeader.Controls.Add(pnlSimStatus);
            pnlHeader.Controls.Add(lblSimStatus);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Location = new System.Drawing.Point(0, 0);
            pnlHeader.Margin = new System.Windows.Forms.Padding(4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new System.Drawing.Size(2240, 75);
            pnlHeader.TabIndex = 0;
            // 
            // btnStockIn
            // 
            btnStockIn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStockIn.BackColor = System.Drawing.Color.ForestGreen;
            btnStockIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStockIn.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            btnStockIn.ForeColor = System.Drawing.Color.White;
            btnStockIn.Location = new System.Drawing.Point(924, 16);
            btnStockIn.Margin = new System.Windows.Forms.Padding(4);
            btnStockIn.Name = "btnStockIn";
            btnStockIn.Size = new System.Drawing.Size(210, 45);
            btnStockIn.TabIndex = 11;
            btnStockIn.Text = "📥 物料入庫 (Stock In)";
            btnStockIn.UseVisualStyleBackColor = false;
            btnStockIn.Click += btnStockIn_Click;
            // 
            // lblModeDisplay
            // 
            lblModeDisplay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblModeDisplay.AutoSize = true;
            lblModeDisplay.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            lblModeDisplay.ForeColor = System.Drawing.Color.Yellow;
            lblModeDisplay.Location = new System.Drawing.Point(13, 28);
            lblModeDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblModeDisplay.Name = "lblModeDisplay";
            lblModeDisplay.Size = new System.Drawing.Size(110, 18);
            lblModeDisplay.TabIndex = 10;
            lblModeDisplay.Text = "[模式: 條碼綁定]";
            // 
            // btnSystemTest
            // 
            btnSystemTest.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSystemTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            btnSystemTest.Location = new System.Drawing.Point(1144, 16);
            btnSystemTest.Margin = new System.Windows.Forms.Padding(4);
            btnSystemTest.Name = "btnSystemTest";
            btnSystemTest.Size = new System.Drawing.Size(132, 45);
            btnSystemTest.TabIndex = 8;
            btnSystemTest.Text = "System Test";
            btnSystemTest.UseVisualStyleBackColor = true;
            // 
            // btnManualSync
            // 
            btnManualSync.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnManualSync.BackColor = System.Drawing.Color.Orange;
            btnManualSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            btnManualSync.Location = new System.Drawing.Point(811, 8);
            btnManualSync.Margin = new System.Windows.Forms.Padding(4);
            btnManualSync.Name = "btnManualSync";
            btnManualSync.Size = new System.Drawing.Size(105, 60);
            btnManualSync.TabIndex = 9;
            btnManualSync.Text = "Manual Decision";
            btnManualSync.UseVisualStyleBackColor = false;
            btnManualSync.Visible = false;
            btnManualSync.Click += btnManualSync_Click;
            // 
            // flpLegend
            // 
            flpLegend.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            flpLegend.AutoSize = true;
            flpLegend.BackColor = System.Drawing.Color.Transparent;
            flpLegend.Controls.Add(lblLegendError);
            flpLegend.Controls.Add(pnlColorError);
            flpLegend.Controls.Add(lblLegendDispatching);
            flpLegend.Controls.Add(pnlColorDispatching);
            flpLegend.Controls.Add(lblLegendOccupied);
            flpLegend.Controls.Add(pnlColorOccupied);
            flpLegend.Controls.Add(lblLegendFinish);
            flpLegend.Controls.Add(pnlColorFinish);
            flpLegend.Controls.Add(lblLegendEmpty);
            flpLegend.Controls.Add(pnlColorEmpty);
            flpLegend.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            flpLegend.Location = new System.Drawing.Point(1284, 19);
            flpLegend.Margin = new System.Windows.Forms.Padding(4);
            flpLegend.Name = "flpLegend";
            flpLegend.Size = new System.Drawing.Size(525, 41);
            flpLegend.TabIndex = 2;
            flpLegend.WrapContents = false;
            // 
            // lblLegendError
            // 
            lblLegendError.AutoSize = true;
            lblLegendError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblLegendError.ForeColor = System.Drawing.Color.White;
            lblLegendError.Location = new System.Drawing.Point(474, 6);
            lblLegendError.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            lblLegendError.Name = "lblLegendError";
            lblLegendError.Size = new System.Drawing.Size(45, 15);
            lblLegendError.TabIndex = 0;
            lblLegendError.Text = "HOLD";
            // 
            // pnlColorError
            // 
            pnlColorError.BackColor = System.Drawing.Color.Red;
            pnlColorError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlColorError.Location = new System.Drawing.Point(445, 8);
            pnlColorError.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            pnlColorError.Name = "pnlColorError";
            pnlColorError.Size = new System.Drawing.Size(17, 18);
            pnlColorError.TabIndex = 1;
            // 
            // lblLegendDispatching
            // 
            lblLegendDispatching.AutoSize = true;
            lblLegendDispatching.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblLegendDispatching.ForeColor = System.Drawing.Color.White;
            lblLegendDispatching.Location = new System.Drawing.Point(393, 6);
            lblLegendDispatching.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            lblLegendDispatching.Name = "lblLegendDispatching";
            lblLegendDispatching.Size = new System.Drawing.Size(46, 15);
            lblLegendDispatching.TabIndex = 2;
            lblLegendDispatching.Text = "MOVE";
            // 
            // pnlColorDispatching
            // 
            pnlColorDispatching.BackColor = System.Drawing.Color.LimeGreen;
            pnlColorDispatching.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlColorDispatching.Location = new System.Drawing.Point(364, 8);
            pnlColorDispatching.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            pnlColorDispatching.Name = "pnlColorDispatching";
            pnlColorDispatching.Size = new System.Drawing.Size(17, 18);
            pnlColorDispatching.TabIndex = 3;
            // 
            // lblLegendOccupied
            // 
            lblLegendOccupied.AutoSize = true;
            lblLegendOccupied.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblLegendOccupied.ForeColor = System.Drawing.Color.White;
            lblLegendOccupied.Location = new System.Drawing.Point(319, 6);
            lblLegendOccupied.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            lblLegendOccupied.Name = "lblLegendOccupied";
            lblLegendOccupied.Size = new System.Drawing.Size(39, 15);
            lblLegendOccupied.TabIndex = 4;
            lblLegendOccupied.Text = "WAIT";
            // 
            // pnlColorOccupied
            // 
            pnlColorOccupied.BackColor = System.Drawing.Color.SkyBlue;
            pnlColorOccupied.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlColorOccupied.Location = new System.Drawing.Point(290, 8);
            pnlColorOccupied.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            pnlColorOccupied.Name = "pnlColorOccupied";
            pnlColorOccupied.Size = new System.Drawing.Size(17, 18);
            pnlColorOccupied.TabIndex = 5;
            // 
            // lblLegendFinish
            // 
            lblLegendFinish.AutoSize = true;
            lblLegendFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblLegendFinish.ForeColor = System.Drawing.Color.White;
            lblLegendFinish.Location = new System.Drawing.Point(238, 6);
            lblLegendFinish.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            lblLegendFinish.Name = "lblLegendFinish";
            lblLegendFinish.Size = new System.Drawing.Size(46, 15);
            lblLegendFinish.TabIndex = 8;
            lblLegendFinish.Text = "DONE";
            // 
            // pnlColorFinish
            // 
            pnlColorFinish.BackColor = System.Drawing.Color.MediumPurple;
            pnlColorFinish.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlColorFinish.Location = new System.Drawing.Point(209, 8);
            pnlColorFinish.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            pnlColorFinish.Name = "pnlColorFinish";
            pnlColorFinish.Size = new System.Drawing.Size(17, 18);
            pnlColorFinish.TabIndex = 9;
            // 
            // lblLegendEmpty
            // 
            lblLegendEmpty.AutoSize = true;
            lblLegendEmpty.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblLegendEmpty.ForeColor = System.Drawing.Color.White;
            lblLegendEmpty.Location = new System.Drawing.Point(150, 6);
            lblLegendEmpty.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            lblLegendEmpty.Name = "lblLegendEmpty";
            lblLegendEmpty.Size = new System.Drawing.Size(53, 15);
            lblLegendEmpty.TabIndex = 6;
            lblLegendEmpty.Text = "EMPTY";
            // 
            // pnlColorEmpty
            // 
            pnlColorEmpty.BackColor = System.Drawing.Color.LightGray;
            pnlColorEmpty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlColorEmpty.Location = new System.Drawing.Point(121, 8);
            pnlColorEmpty.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            pnlColorEmpty.Name = "pnlColorEmpty";
            pnlColorEmpty.Size = new System.Drawing.Size(17, 18);
            pnlColorEmpty.TabIndex = 7;
            // 
            // btnGlobalMonitor
            // 
            btnGlobalMonitor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnGlobalMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            btnGlobalMonitor.Location = new System.Drawing.Point(2090, 15);
            btnGlobalMonitor.Margin = new System.Windows.Forms.Padding(4);
            btnGlobalMonitor.Name = "btnGlobalMonitor";
            btnGlobalMonitor.Size = new System.Drawing.Size(140, 45);
            btnGlobalMonitor.TabIndex = 1;
            btnGlobalMonitor.Text = "Global Rank";
            btnGlobalMonitor.UseVisualStyleBackColor = true;
            // 
            // btnTransitMonitor
            // 
            btnTransitMonitor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnTransitMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            btnTransitMonitor.Location = new System.Drawing.Point(1947, 15);
            btnTransitMonitor.Margin = new System.Windows.Forms.Padding(4);
            btnTransitMonitor.Name = "btnTransitMonitor";
            btnTransitMonitor.Size = new System.Drawing.Size(136, 45);
            btnTransitMonitor.TabIndex = 3;
            btnTransitMonitor.Text = "Trans Monitor";
            btnTransitMonitor.UseVisualStyleBackColor = true;
            // 
            // btnEqpMonitor
            // 
            btnEqpMonitor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnEqpMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            btnEqpMonitor.Location = new System.Drawing.Point(1818, 15);
            btnEqpMonitor.Margin = new System.Windows.Forms.Padding(4);
            btnEqpMonitor.Name = "btnEqpMonitor";
            btnEqpMonitor.Size = new System.Drawing.Size(122, 45);
            btnEqpMonitor.TabIndex = 4;
            btnEqpMonitor.Text = "EQ Monitor ";
            btnEqpMonitor.UseVisualStyleBackColor = true;
            // 
            // pnlSimStatus
            // 
            pnlSimStatus.BackColor = System.Drawing.Color.Red;
            pnlSimStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlSimStatus.Location = new System.Drawing.Point(133, 26);
            pnlSimStatus.Margin = new System.Windows.Forms.Padding(4);
            pnlSimStatus.Name = "pnlSimStatus";
            pnlSimStatus.Size = new System.Drawing.Size(23, 24);
            pnlSimStatus.TabIndex = 5;
            // 
            // lblSimStatus
            // 
            lblSimStatus.AutoSize = true;
            lblSimStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            lblSimStatus.ForeColor = System.Drawing.Color.White;
            lblSimStatus.Location = new System.Drawing.Point(156, 29);
            lblSimStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSimStatus.Name = "lblSimStatus";
            lblSimStatus.Size = new System.Drawing.Size(129, 17);
            lblSimStatus.TabIndex = 6;
            lblSimStatus.Text = "Simulator Offline";
            // 
            // pnlGrid
            // 
            pnlGrid.Controls.Add(tlpShelf);
            pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlGrid.Location = new System.Drawing.Point(0, 75);
            pnlGrid.Margin = new System.Windows.Forms.Padding(4);
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Size = new System.Drawing.Size(2240, 742);
            pnlGrid.TabIndex = 1;
            // 
            // tlpShelf
            // 
            tlpShelf.BackColor = System.Drawing.SystemColors.ControlLight;
            tlpShelf.ColumnCount = 11;
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.09F));
            tlpShelf.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpShelf.Location = new System.Drawing.Point(0, 0);
            tlpShelf.Margin = new System.Windows.Forms.Padding(4);
            tlpShelf.Name = "tlpShelf";
            tlpShelf.RowCount = 8;
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tlpShelf.Size = new System.Drawing.Size(2240, 742);
            tlpShelf.TabIndex = 0;
            // 
            // lstLog
            // 
            lstLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            lstLog.Font = new System.Drawing.Font("Consolas", 10F);
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 15;
            lstLog.Location = new System.Drawing.Point(0, 817);
            lstLog.Margin = new System.Windows.Forms.Padding(4);
            lstLog.Name = "lstLog";
            lstLog.Size = new System.Drawing.Size(2240, 244);
            lstLog.TabIndex = 2;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(2240, 1061);
            Controls.Add(pnlGrid);
            Controls.Add(lstLog);
            Controls.Add(pnlHeader);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FormMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "APS - Advanced Planning System";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += FormMain_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            flpLegend.ResumeLayout(false);
            flpLegend.PerformLayout();
            pnlGrid.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnGlobalMonitor;
        private System.Windows.Forms.Button btnTransitMonitor;
        private System.Windows.Forms.Button btnEqpMonitor;
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
        private System.Windows.Forms.Label lblModeDisplay;
        private System.Windows.Forms.Button btnStockIn;
    }
}