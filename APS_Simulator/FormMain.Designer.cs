namespace APSSimulator
{
    partial class FormMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
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

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageDispatch = new System.Windows.Forms.TabPage();
            this.tabPagePerson = new System.Windows.Forms.TabPage();
            this.dgvTestList = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colPortId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCassetteId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWorkNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrentStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNextStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetEqp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtClientLog = new System.Windows.Forms.TextBox();
            this.grpPersonOp = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnEnterEq = new System.Windows.Forms.Button();
            this.btnBatchPick = new System.Windows.Forms.Button();
            this.btnBatchScan = new System.Windows.Forms.Button();
            this.btnLoadDefault = new System.Windows.Forms.Button();
            this.grpClientConfig = new System.Windows.Forms.GroupBox();
            this.btnClientDisconnect = new System.Windows.Forms.Button();
            this.btnClientConnect = new System.Windows.Forms.Button();
            this.numClientPort = new System.Windows.Forms.NumericUpDown();
            this.lblClientPort = new System.Windows.Forms.Label();
            this.txtClientIp = new System.Windows.Forms.TextBox();
            this.lblClientIp = new System.Windows.Forms.Label();
            this.tabPageMachine = new System.Windows.Forms.TabPage();
            this.flowLayoutPanelMachines = new System.Windows.Forms.FlowLayoutPanel();
            this.panelMachineTool = new System.Windows.Forms.Panel();
            this.btnDoSearch = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchMachine = new System.Windows.Forms.TextBox();
            this.btnRefreshMachines = new System.Windows.Forms.Button();
            this.tabPageMES = new System.Windows.Forms.TabPage();
            this.txtMesLog = new System.Windows.Forms.TextBox();
            this.panelMesControl = new System.Windows.Forms.Panel();
            this.numMesPort = new System.Windows.Forms.NumericUpDown();
            this.lblMesPort = new System.Windows.Forms.Label();
            this.btnStopMes = new System.Windows.Forms.Button();
            this.btnStartMes = new System.Windows.Forms.Button();
            this.lblMesStatus = new System.Windows.Forms.Label();
            this.ctxMenuRow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuScan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPick = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEnterEq = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlMain.SuspendLayout();
            this.tabPagePerson.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestList)).BeginInit();
            this.grpPersonOp.SuspendLayout();
            this.grpClientConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientPort)).BeginInit();
            this.tabPageMachine.SuspendLayout();
            this.panelMachineTool.SuspendLayout();
            this.tabPageMES.SuspendLayout();
            this.panelMesControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMesPort)).BeginInit();
            this.ctxMenuRow.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageDispatch);
            this.tabControlMain.Controls.Add(this.tabPagePerson);
            this.tabControlMain.Controls.Add(this.tabPageMachine);
            this.tabControlMain.Controls.Add(this.tabPageMES);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1008, 729);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageDispatch
            // 
            this.tabPageDispatch.Location = new System.Drawing.Point(4, 22);
            this.tabPageDispatch.Name = "tabPageDispatch";
            this.tabPageDispatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDispatch.Size = new System.Drawing.Size(1000, 703);
            this.tabPageDispatch.TabIndex = 0;
            this.tabPageDispatch.Text = "派送流程監控";
            this.tabPageDispatch.UseVisualStyleBackColor = true;
            // 
            // tabPagePerson
            // 
            this.tabPagePerson.Controls.Add(this.dgvTestList);
            this.tabPagePerson.Controls.Add(this.txtClientLog);
            this.tabPagePerson.Controls.Add(this.grpPersonOp);
            this.tabPagePerson.Location = new System.Drawing.Point(4, 22);
            this.tabPagePerson.Name = "tabPagePerson";
            this.tabPagePerson.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePerson.Size = new System.Drawing.Size(1000, 703);
            this.tabPagePerson.TabIndex = 1;
            this.tabPagePerson.Text = "人員模擬 (OP)";
            this.tabPagePerson.UseVisualStyleBackColor = true;
            // 
            // dgvTestList
            // 
            this.dgvTestList.AllowUserToAddRows = false;
            this.dgvTestList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTestList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelect,
            this.colPortId,
            this.colCassetteId,
            this.colWorkNo,
            this.colCurrentStep,
            this.colNextStep,
            this.colTargetEqp,
            this.colStatus,
            this.colAction});
            this.dgvTestList.ContextMenuStrip = this.ctxMenuRow;
            this.dgvTestList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTestList.Location = new System.Drawing.Point(3, 103);
            this.dgvTestList.Name = "dgvTestList";
            this.dgvTestList.RowTemplate.Height = 24;
            this.dgvTestList.Size = new System.Drawing.Size(994, 497);
            this.dgvTestList.TabIndex = 1;
            this.dgvTestList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTestList_CellMouseDown);
            // 
            // colSelect
            // 
            this.colSelect.HeaderText = "選取";
            this.colSelect.Name = "colSelect";
            this.colSelect.Width = 50;
            // 
            // colPortId
            // 
            this.colPortId.HeaderText = "Port ID";
            this.colPortId.Name = "colPortId";
            this.colPortId.Width = 80;
            // 
            // colCassetteId
            // 
            this.colCassetteId.HeaderText = "Cassette ID";
            this.colCassetteId.Name = "colCassetteId";
            this.colCassetteId.Width = 120;
            // 
            // colWorkNo
            // 
            this.colWorkNo.HeaderText = "工單號 (WorkNo)";
            this.colWorkNo.Name = "colWorkNo";
            this.colWorkNo.Width = 150;
            // 
            // colCurrentStep
            // 
            this.colCurrentStep.HeaderText = "當前站點";
            this.colCurrentStep.Name = "colCurrentStep";
            this.colCurrentStep.Width = 100;
            // 
            // colNextStep
            // 
            this.colNextStep.HeaderText = "下一站";
            this.colNextStep.Name = "colNextStep";
            this.colNextStep.Width = 100;
            // 
            // colTargetEqp
            // 
            this.colTargetEqp.HeaderText = "目標機台";
            this.colTargetEqp.Name = "colTargetEqp";
            this.colTargetEqp.ReadOnly = true;
            this.colTargetEqp.Width = 100;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "目前狀態";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 150;
            // 
            // colAction
            // 
            this.colAction.HeaderText = "指令紀錄";
            this.colAction.Name = "colAction";
            this.colAction.Width = 200;
            // 
            // txtClientLog
            // 
            this.txtClientLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtClientLog.Location = new System.Drawing.Point(3, 600);
            this.txtClientLog.Multiline = true;
            this.txtClientLog.Name = "txtClientLog";
            this.txtClientLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClientLog.Size = new System.Drawing.Size(994, 100);
            this.txtClientLog.TabIndex = 2;
            // 
            // grpPersonOp
            // 
            this.grpPersonOp.Controls.Add(this.btnSelectAll);
            this.grpPersonOp.Controls.Add(this.btnDeselectAll);
            this.grpPersonOp.Controls.Add(this.btnEnterEq);
            this.grpPersonOp.Controls.Add(this.btnBatchPick);
            this.grpPersonOp.Controls.Add(this.btnBatchScan);
            this.grpPersonOp.Controls.Add(this.btnLoadDefault);
            this.grpPersonOp.Controls.Add(this.grpClientConfig);
            this.grpPersonOp.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPersonOp.Location = new System.Drawing.Point(3, 3);
            this.grpPersonOp.Name = "grpPersonOp";
            this.grpPersonOp.Size = new System.Drawing.Size(994, 100);
            this.grpPersonOp.TabIndex = 0;
            this.grpPersonOp.TabStop = false;
            this.grpPersonOp.Text = "人員操作模擬 (Person Simulation)";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(20, 70);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(60, 25);
            this.btnSelectAll.TabIndex = 11;
            this.btnSelectAll.Text = "全選";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Location = new System.Drawing.Point(85, 70);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(60, 25);
            this.btnDeselectAll.TabIndex = 12;
            this.btnDeselectAll.Text = "全取消";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            // 
            // btnEnterEq
            // 
            this.btnEnterEq.BackColor = System.Drawing.Color.Orange;
            this.btnEnterEq.Location = new System.Drawing.Point(410, 25);
            this.btnEnterEq.Name = "btnEnterEq";
            this.btnEnterEq.Size = new System.Drawing.Size(120, 40);
            this.btnEnterEq.TabIndex = 10;
            this.btnEnterEq.Text = "批量進入機台";
            this.btnEnterEq.UseVisualStyleBackColor = false;
            // 
            // btnBatchPick
            // 
            this.btnBatchPick.BackColor = System.Drawing.Color.LightGreen;
            this.btnBatchPick.Location = new System.Drawing.Point(280, 25);
            this.btnBatchPick.Name = "btnBatchPick";
            this.btnBatchPick.Size = new System.Drawing.Size(120, 40);
            this.btnBatchPick.TabIndex = 9;
            this.btnBatchPick.Text = "批量取走 (PICK)";
            this.btnBatchPick.UseVisualStyleBackColor = false;
            // 
            // btnBatchScan
            // 
            this.btnBatchScan.BackColor = System.Drawing.Color.LightBlue;
            this.btnBatchScan.Location = new System.Drawing.Point(150, 25);
            this.btnBatchScan.Name = "btnBatchScan";
            this.btnBatchScan.Size = new System.Drawing.Size(120, 40);
            this.btnBatchScan.TabIndex = 8;
            this.btnBatchScan.Text = "批量入庫 (SCAN)";
            this.btnBatchScan.UseVisualStyleBackColor = false;
            // 
            // btnLoadDefault
            // 
            this.btnLoadDefault.Location = new System.Drawing.Point(20, 25);
            this.btnLoadDefault.Name = "btnLoadDefault";
            this.btnLoadDefault.Size = new System.Drawing.Size(120, 40);
            this.btnLoadDefault.TabIndex = 7;
            this.btnLoadDefault.Text = "載入預設列表";
            this.btnLoadDefault.UseVisualStyleBackColor = true;
            // 
            // grpClientConfig
            // 
            this.grpClientConfig.Controls.Add(this.btnClientDisconnect);
            this.grpClientConfig.Controls.Add(this.btnClientConnect);
            this.grpClientConfig.Controls.Add(this.numClientPort);
            this.grpClientConfig.Controls.Add(this.lblClientPort);
            this.grpClientConfig.Controls.Add(this.txtClientIp);
            this.grpClientConfig.Controls.Add(this.lblClientIp);
            this.grpClientConfig.Location = new System.Drawing.Point(550, 15);
            this.grpClientConfig.Name = "grpClientConfig";
            this.grpClientConfig.Size = new System.Drawing.Size(430, 80);
            this.grpClientConfig.TabIndex = 6;
            this.grpClientConfig.TabStop = false;
            this.grpClientConfig.Text = "APS 連線設定";
            // 
            // btnClientDisconnect
            // 
            this.btnClientDisconnect.Location = new System.Drawing.Point(340, 25);
            this.btnClientDisconnect.Name = "btnClientDisconnect";
            this.btnClientDisconnect.Size = new System.Drawing.Size(75, 30);
            this.btnClientDisconnect.TabIndex = 5;
            this.btnClientDisconnect.Text = "斷線";
            this.btnClientDisconnect.UseVisualStyleBackColor = true;
            // 
            // btnClientConnect
            // 
            this.btnClientConnect.Location = new System.Drawing.Point(260, 25);
            this.btnClientConnect.Name = "btnClientConnect";
            this.btnClientConnect.Size = new System.Drawing.Size(75, 30);
            this.btnClientConnect.TabIndex = 4;
            this.btnClientConnect.Text = "連線";
            this.btnClientConnect.UseVisualStyleBackColor = true;
            // 
            // numClientPort
            // 
            this.numClientPort.Location = new System.Drawing.Point(180, 27);
            this.numClientPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numClientPort.Name = "numClientPort";
            this.numClientPort.Size = new System.Drawing.Size(60, 22);
            this.numClientPort.TabIndex = 3;
            this.numClientPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            // 
            // lblClientPort
            // 
            this.lblClientPort.AutoSize = true;
            this.lblClientPort.Location = new System.Drawing.Point(150, 30);
            this.lblClientPort.Name = "lblClientPort";
            this.lblClientPort.Size = new System.Drawing.Size(27, 12);
            this.lblClientPort.TabIndex = 2;
            this.lblClientPort.Text = "Port:";
            // 
            // txtClientIp
            // 
            this.txtClientIp.Location = new System.Drawing.Point(40, 27);
            this.txtClientIp.Name = "txtClientIp";
            this.txtClientIp.Size = new System.Drawing.Size(100, 22);
            this.txtClientIp.TabIndex = 1;
            this.txtClientIp.Text = "127.0.0.1";
            // 
            // lblClientIp
            // 
            this.lblClientIp.AutoSize = true;
            this.lblClientIp.Location = new System.Drawing.Point(15, 30);
            this.lblClientIp.Name = "lblClientIp";
            this.lblClientIp.Size = new System.Drawing.Size(18, 12);
            this.lblClientIp.TabIndex = 0;
            this.lblClientIp.Text = "IP:";
            // 
            // tabPageMachine
            // 
            this.tabPageMachine.Controls.Add(this.flowLayoutPanelMachines);
            this.tabPageMachine.Controls.Add(this.panelMachineTool);
            this.tabPageMachine.Location = new System.Drawing.Point(4, 22);
            this.tabPageMachine.Name = "tabPageMachine";
            this.tabPageMachine.Size = new System.Drawing.Size(1000, 703);
            this.tabPageMachine.TabIndex = 2;
            this.tabPageMachine.Text = "機台模擬";
            this.tabPageMachine.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelMachines
            // 
            this.flowLayoutPanelMachines.AutoScroll = true;
            this.flowLayoutPanelMachines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMachines.Location = new System.Drawing.Point(0, 40);
            this.flowLayoutPanelMachines.Name = "flowLayoutPanelMachines";
            this.flowLayoutPanelMachines.Size = new System.Drawing.Size(1000, 663);
            this.flowLayoutPanelMachines.TabIndex = 1;
            // 
            // panelMachineTool
            // 
            this.panelMachineTool.Controls.Add(this.btnDoSearch);
            this.panelMachineTool.Controls.Add(this.lblSearch);
            this.panelMachineTool.Controls.Add(this.txtSearchMachine);
            this.panelMachineTool.Controls.Add(this.btnRefreshMachines);
            this.panelMachineTool.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMachineTool.Location = new System.Drawing.Point(0, 0);
            this.panelMachineTool.Name = "panelMachineTool";
            this.panelMachineTool.Size = new System.Drawing.Size(1000, 40);
            this.panelMachineTool.TabIndex = 2;
            // 
            // btnDoSearch
            // 
            this.btnDoSearch.Location = new System.Drawing.Point(380, 7);
            this.btnDoSearch.Name = "btnDoSearch";
            this.btnDoSearch.Size = new System.Drawing.Size(75, 25);
            this.btnDoSearch.TabIndex = 3;
            this.btnDoSearch.Text = "搜尋";
            this.btnDoSearch.UseVisualStyleBackColor = true;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(160, 14);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(56, 12);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "設備搜尋:";
            // 
            // txtSearchMachine
            // 
            this.txtSearchMachine.Location = new System.Drawing.Point(220, 9);
            this.txtSearchMachine.Name = "txtSearchMachine";
            this.txtSearchMachine.Size = new System.Drawing.Size(150, 22);
            this.txtSearchMachine.TabIndex = 1;
            // 
            // btnRefreshMachines
            // 
            this.btnRefreshMachines.Location = new System.Drawing.Point(0, 0);
            this.btnRefreshMachines.Name = "btnRefreshMachines";
            this.btnRefreshMachines.Size = new System.Drawing.Size(150, 40);
            this.btnRefreshMachines.TabIndex = 0;
            this.btnRefreshMachines.Text = "重新整理列表";
            this.btnRefreshMachines.UseVisualStyleBackColor = true;
            // 
            // tabPageMES
            // 
            this.tabPageMES.Controls.Add(this.txtMesLog);
            this.tabPageMES.Controls.Add(this.panelMesControl);
            this.tabPageMES.Location = new System.Drawing.Point(4, 22);
            this.tabPageMES.Name = "tabPageMES";
            this.tabPageMES.Size = new System.Drawing.Size(1000, 703);
            this.tabPageMES.TabIndex = 3;
            this.tabPageMES.Text = "MES Server 模擬";
            this.tabPageMES.UseVisualStyleBackColor = true;
            // 
            // txtMesLog
            // 
            this.txtMesLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMesLog.Location = new System.Drawing.Point(0, 50);
            this.txtMesLog.Multiline = true;
            this.txtMesLog.Name = "txtMesLog";
            this.txtMesLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMesLog.Size = new System.Drawing.Size(1000, 653);
            this.txtMesLog.TabIndex = 1;
            // 
            // panelMesControl
            // 
            this.panelMesControl.Controls.Add(this.numMesPort);
            this.panelMesControl.Controls.Add(this.lblMesPort);
            this.panelMesControl.Controls.Add(this.lblMesStatus);
            this.panelMesControl.Controls.Add(this.btnStopMes);
            this.panelMesControl.Controls.Add(this.btnStartMes);
            this.panelMesControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMesControl.Location = new System.Drawing.Point(0, 0);
            this.panelMesControl.Name = "panelMesControl";
            this.panelMesControl.Size = new System.Drawing.Size(1000, 50);
            this.panelMesControl.TabIndex = 0;
            // 
            // numMesPort
            // 
            this.numMesPort.Location = new System.Drawing.Point(330, 15);
            this.numMesPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numMesPort.Name = "numMesPort";
            this.numMesPort.Size = new System.Drawing.Size(60, 22);
            this.numMesPort.TabIndex = 4;
            this.numMesPort.Value = new decimal(new int[] { 9000, 0, 0, 0 });
            // 
            // lblMesPort
            // 
            this.lblMesPort.AutoSize = true;
            this.lblMesPort.Location = new System.Drawing.Point(300, 19);
            this.lblMesPort.Name = "lblMesPort";
            this.lblMesPort.Size = new System.Drawing.Size(27, 12);
            this.lblMesPort.TabIndex = 3;
            this.lblMesPort.Text = "Port:";
            // 
            // btnStopMes
            // 
            this.btnStopMes.Location = new System.Drawing.Point(120, 10);
            this.btnStopMes.Name = "btnStopMes";
            this.btnStopMes.Size = new System.Drawing.Size(100, 30);
            this.btnStopMes.TabIndex = 1;
            this.btnStopMes.Text = "停止 Server";
            this.btnStopMes.UseVisualStyleBackColor = true;
            // 
            // btnStartMes
            // 
            this.btnStartMes.Location = new System.Drawing.Point(10, 10);
            this.btnStartMes.Name = "btnStartMes";
            this.btnStartMes.Size = new System.Drawing.Size(100, 30);
            this.btnStartMes.TabIndex = 0;
            this.btnStartMes.Text = "啟動 Server";
            this.btnStartMes.UseVisualStyleBackColor = true;
            // 
            // lblMesStatus
            // 
            this.lblMesStatus.AutoSize = true;
            this.lblMesStatus.Location = new System.Drawing.Point(230, 19);
            this.lblMesStatus.Name = "lblMesStatus";
            this.lblMesStatus.Size = new System.Drawing.Size(35, 12);
            this.lblMesStatus.TabIndex = 2;
            this.lblMesStatus.Text = "Status: Stopped";
            // 
            // ctxMenuRow
            // 
            this.ctxMenuRow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuScan,
            this.menuPick,
            this.menuEnterEq});
            this.ctxMenuRow.Name = "ctxMenuRow";
            this.ctxMenuRow.Size = new System.Drawing.Size(181, 70);
            // 
            // menuScan
            // 
            this.menuScan.Name = "menuScan";
            this.menuScan.Size = new System.Drawing.Size(180, 22);
            this.menuScan.Text = "單獨入庫 (Scan)";
            this.menuScan.Click += new System.EventHandler(this.menuScan_Click);
            // 
            // menuPick
            // 
            this.menuPick.Name = "menuPick";
            this.menuPick.Size = new System.Drawing.Size(180, 22);
            this.menuPick.Text = "單獨取走 (Pick)";
            this.menuPick.Click += new System.EventHandler(this.menuPick_Click);
            // 
            // menuEnterEq
            // 
            this.menuEnterEq.Name = "menuEnterEq";
            this.menuEnterEq.Size = new System.Drawing.Size(180, 22);
            this.menuEnterEq.Text = "進入機台 (Track-In)";
            this.menuEnterEq.Click += new System.EventHandler(this.menuEnterEq_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tabControlMain);
            this.Name = "FormMain";
            this.Text = "APS Simulator (Mock Environment)";
            this.tabControlMain.ResumeLayout(false);
            this.tabPagePerson.ResumeLayout(false);
            this.tabPagePerson.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestList)).EndInit();
            this.grpPersonOp.ResumeLayout(false);
            this.grpClientConfig.ResumeLayout(false);
            this.grpClientConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientPort)).EndInit();
            this.tabPageMachine.ResumeLayout(false);
            this.panelMachineTool.ResumeLayout(false);
            this.panelMachineTool.PerformLayout();
            this.tabPageMES.ResumeLayout(false);
            this.tabPageMES.PerformLayout();
            this.panelMesControl.ResumeLayout(false);
            this.panelMesControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMesPort)).EndInit();
            this.ctxMenuRow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageDispatch;
        private System.Windows.Forms.TabPage tabPagePerson;
        private System.Windows.Forms.TabPage tabPageMachine;
        private System.Windows.Forms.TabPage tabPageMES;
        
        // Person
        private System.Windows.Forms.DataGridView dgvTestList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPortId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCassetteId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWorkNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNextStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetEqp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.TextBox txtClientLog;
        
        private System.Windows.Forms.GroupBox grpPersonOp;
        private System.Windows.Forms.Button btnLoadDefault;
        private System.Windows.Forms.Button btnBatchScan;
        private System.Windows.Forms.Button btnBatchPick;
        private System.Windows.Forms.Button btnEnterEq;
        
        // Client Config
        private System.Windows.Forms.GroupBox grpClientConfig;
        private System.Windows.Forms.Button btnClientDisconnect;
        private System.Windows.Forms.Button btnClientConnect;
        private System.Windows.Forms.NumericUpDown numClientPort;
        private System.Windows.Forms.Label lblClientPort;
        private System.Windows.Forms.TextBox txtClientIp;
        private System.Windows.Forms.Label lblClientIp;

        // Machine
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMachines;
        private System.Windows.Forms.Button btnRefreshMachines;
        private System.Windows.Forms.Panel panelMachineTool;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchMachine;
        private System.Windows.Forms.Button btnDoSearch;

        // MES
        private System.Windows.Forms.Panel panelMesControl;
        private System.Windows.Forms.Button btnStopMes;
        private System.Windows.Forms.Button btnStartMes;
        private System.Windows.Forms.TextBox txtMesLog;
        private System.Windows.Forms.Label lblMesStatus;
        private System.Windows.Forms.Label lblMesPort;
        private System.Windows.Forms.NumericUpDown numMesPort;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.ContextMenuStrip ctxMenuRow;
        private System.Windows.Forms.ToolStripMenuItem menuScan;
        private System.Windows.Forms.ToolStripMenuItem menuPick;
        private System.Windows.Forms.ToolStripMenuItem menuEnterEq;
    }
}