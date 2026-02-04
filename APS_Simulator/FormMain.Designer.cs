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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageDispatch = new System.Windows.Forms.TabPage();
            this.tabPagePerson = new System.Windows.Forms.TabPage();
            this.grpPersonOp = new System.Windows.Forms.GroupBox();
            this.grpClientConfig = new System.Windows.Forms.GroupBox();
            this.btnClientDisconnect = new System.Windows.Forms.Button();
            this.btnClientConnect = new System.Windows.Forms.Button();
            this.numClientPort = new System.Windows.Forms.NumericUpDown();
            this.lblClientPort = new System.Windows.Forms.Label();
            this.txtClientIp = new System.Windows.Forms.TextBox();
            this.lblClientIp = new System.Windows.Forms.Label();
            this.btnPick = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtPortId = new System.Windows.Forms.TextBox();
            this.lblPortId = new System.Windows.Forms.Label();
            this.txtClientLog = new System.Windows.Forms.TextBox();
            this.tabPageMachine = new System.Windows.Forms.TabPage();
            this.flowLayoutPanelMachines = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRefreshMachines = new System.Windows.Forms.Button();
            this.tabPageMES = new System.Windows.Forms.TabPage();
            this.txtMesLog = new System.Windows.Forms.TextBox();
            this.panelMesControl = new System.Windows.Forms.Panel();
            this.numMesPort = new System.Windows.Forms.NumericUpDown();
            this.lblMesPort = new System.Windows.Forms.Label();
            this.btnStopMes = new System.Windows.Forms.Button();
            this.btnStartMes = new System.Windows.Forms.Button();
            this.lblMesStatus = new System.Windows.Forms.Label();
            this.tabControlMain.SuspendLayout();
            this.tabPagePerson.SuspendLayout();
            this.grpPersonOp.SuspendLayout();
            this.grpClientConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientPort)).BeginInit();
            this.tabPageMachine.SuspendLayout();
            this.tabPageMES.SuspendLayout();
            this.panelMesControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMesPort)).BeginInit();
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
            // grpPersonOp
            // 
            this.grpPersonOp.Controls.Add(this.grpClientConfig);
            this.grpPersonOp.Controls.Add(this.btnPick);
            this.grpPersonOp.Controls.Add(this.btnScan);
            this.grpPersonOp.Controls.Add(this.txtBarcode);
            this.grpPersonOp.Controls.Add(this.lblBarcode);
            this.grpPersonOp.Controls.Add(this.txtPortId);
            this.grpPersonOp.Controls.Add(this.lblPortId);
            this.grpPersonOp.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPersonOp.Location = new System.Drawing.Point(3, 3);
            this.grpPersonOp.Name = "grpPersonOp";
            this.grpPersonOp.Size = new System.Drawing.Size(994, 150);
            this.grpPersonOp.TabIndex = 0;
            this.grpPersonOp.TabStop = false;
            this.grpPersonOp.Text = "操作區";
            // 
            // grpClientConfig
            // 
            this.grpClientConfig.Controls.Add(this.btnClientDisconnect);
            this.grpClientConfig.Controls.Add(this.btnClientConnect);
            this.grpClientConfig.Controls.Add(this.numClientPort);
            this.grpClientConfig.Controls.Add(this.lblClientPort);
            this.grpClientConfig.Controls.Add(this.txtClientIp);
            this.grpClientConfig.Controls.Add(this.lblClientIp);
            this.grpClientConfig.Location = new System.Drawing.Point(400, 15);
            this.grpClientConfig.Name = "grpClientConfig";
            this.grpClientConfig.Size = new System.Drawing.Size(500, 80);
            this.grpClientConfig.TabIndex = 6;
            this.grpClientConfig.TabStop = false;
            this.grpClientConfig.Text = "APS 連線設定";
            // 
            // btnClientDisconnect
            // 
            this.btnClientDisconnect.Location = new System.Drawing.Point(340, 20);
            this.btnClientDisconnect.Name = "btnClientDisconnect";
            this.btnClientDisconnect.Size = new System.Drawing.Size(75, 25);
            this.btnClientDisconnect.TabIndex = 5;
            this.btnClientDisconnect.Text = "斷線";
            this.btnClientDisconnect.UseVisualStyleBackColor = true;
            // 
            // btnClientConnect
            // 
            this.btnClientConnect.Location = new System.Drawing.Point(260, 20);
            this.btnClientConnect.Name = "btnClientConnect";
            this.btnClientConnect.Size = new System.Drawing.Size(75, 25);
            this.btnClientConnect.TabIndex = 4;
            this.btnClientConnect.Text = "連線";
            this.btnClientConnect.UseVisualStyleBackColor = true;
            // 
            // numClientPort
            // 
            this.numClientPort.Location = new System.Drawing.Point(180, 22);
            this.numClientPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numClientPort.Name = "numClientPort";
            this.numClientPort.Size = new System.Drawing.Size(60, 22);
            this.numClientPort.TabIndex = 3;
            this.numClientPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            // 
            // lblClientPort
            // 
            this.lblClientPort.AutoSize = true;
            this.lblClientPort.Location = new System.Drawing.Point(150, 25);
            this.lblClientPort.Name = "lblClientPort";
            this.lblClientPort.Size = new System.Drawing.Size(27, 12);
            this.lblClientPort.TabIndex = 2;
            this.lblClientPort.Text = "Port:";
            // 
            // txtClientIp
            // 
            this.txtClientIp.Location = new System.Drawing.Point(40, 22);
            this.txtClientIp.Name = "txtClientIp";
            this.txtClientIp.Size = new System.Drawing.Size(100, 22);
            this.txtClientIp.TabIndex = 1;
            this.txtClientIp.Text = "127.0.0.1";
            // 
            // lblClientIp
            // 
            this.lblClientIp.AutoSize = true;
            this.lblClientIp.Location = new System.Drawing.Point(15, 25);
            this.lblClientIp.Name = "lblClientIp";
            this.lblClientIp.Size = new System.Drawing.Size(18, 12);
            this.lblClientIp.TabIndex = 0;
            this.lblClientIp.Text = "IP:";
            // 
            // btnPick
            // 
            this.btnPick.Location = new System.Drawing.Point(250, 56);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(100, 30);
            this.btnPick.TabIndex = 5;
            this.btnPick.Text = "取走 (PICK)";
            this.btnPick.UseVisualStyleBackColor = true;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(250, 20);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(100, 30);
            this.btnScan.TabIndex = 4;
            this.btnScan.Text = "進料 (SCAN)";
            this.btnScan.UseVisualStyleBackColor = true;
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(80, 25);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(150, 22);
            this.txtBarcode.TabIndex = 3;
            // 
            // lblBarcode
            // 
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Location = new System.Drawing.Point(20, 28);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(47, 12);
            this.lblBarcode.TabIndex = 2;
            this.lblBarcode.Text = "Barcode:";
            // 
            // txtPortId
            // 
            this.txtPortId.Location = new System.Drawing.Point(80, 61);
            this.txtPortId.Name = "txtPortId";
            this.txtPortId.Size = new System.Drawing.Size(150, 22);
            this.txtPortId.TabIndex = 1;
            // 
            // lblPortId
            // 
            this.lblPortId.AutoSize = true;
            this.lblPortId.Location = new System.Drawing.Point(20, 64);
            this.lblPortId.Name = "lblPortId";
            this.lblPortId.Size = new System.Drawing.Size(41, 12);
            this.lblPortId.TabIndex = 0;
            this.lblPortId.Text = "Port ID:";
            // 
            // txtClientLog
            // 
            this.txtClientLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClientLog.Location = new System.Drawing.Point(3, 153);
            this.txtClientLog.Multiline = true;
            this.txtClientLog.Name = "txtClientLog";
            this.txtClientLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClientLog.Size = new System.Drawing.Size(994, 547);
            this.txtClientLog.TabIndex = 1;
            // 
            // tabPageMachine
            // 
            this.tabPageMachine.Controls.Add(this.flowLayoutPanelMachines);
            this.tabPageMachine.Controls.Add(this.btnRefreshMachines);
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
            // btnRefreshMachines
            // 
            this.btnRefreshMachines.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRefreshMachines.Location = new System.Drawing.Point(0, 0);
            this.btnRefreshMachines.Name = "btnRefreshMachines";
            this.btnRefreshMachines.Size = new System.Drawing.Size(1000, 40);
            this.btnRefreshMachines.TabIndex = 0;
            this.btnRefreshMachines.Text = "重新整理機台列表";
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
            this.grpPersonOp.ResumeLayout(false);
            this.grpPersonOp.PerformLayout();
            this.grpClientConfig.ResumeLayout(false);
            this.grpClientConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientPort)).EndInit();
            this.tabPageMachine.ResumeLayout(false);
            this.tabPageMES.ResumeLayout(false);
            this.tabPageMES.PerformLayout();
            this.panelMesControl.ResumeLayout(false);
            this.panelMesControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMesPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageDispatch;
        private System.Windows.Forms.TabPage tabPagePerson;
        private System.Windows.Forms.TabPage tabPageMachine;
        private System.Windows.Forms.TabPage tabPageMES;
        
        // Person
        private System.Windows.Forms.GroupBox grpPersonOp;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtPortId;
        private System.Windows.Forms.Label lblPortId;
        private System.Windows.Forms.TextBox txtClientLog;
        
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

        // MES
        private System.Windows.Forms.Panel panelMesControl;
        private System.Windows.Forms.Button btnStopMes;
        private System.Windows.Forms.Button btnStartMes;
        private System.Windows.Forms.TextBox txtMesLog;
        private System.Windows.Forms.Label lblMesStatus;
        private System.Windows.Forms.Label lblMesPort;
        private System.Windows.Forms.NumericUpDown numMesPort;
    }
}