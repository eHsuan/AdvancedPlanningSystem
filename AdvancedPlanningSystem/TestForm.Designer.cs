namespace AdvancedPlanningSystem
{
    partial class TestForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMes = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.lblHelp = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnTestStepTime = new System.Windows.Forms.Button();
            this.btnTestQTime = new System.Windows.Forms.Button();
            this.btnTestGetOrder = new System.Windows.Forms.Button();
            this.btnTestGetEqStatus = new System.Windows.Forms.Button();
            this.btnTestGetWip = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPageMes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpInput.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 600);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageMes
            // 
            this.tabPageMes.Controls.Add(this.splitContainer1);
            this.tabPageMes.Location = new System.Drawing.Point(4, 22);
            this.tabPageMes.Name = "tabPageMes";
            this.tabPageMes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMes.Size = new System.Drawing.Size(792, 574);
            this.tabPageMes.TabIndex = 0;
            this.tabPageMes.Text = "MES API Test";
            this.tabPageMes.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpInput);
            this.splitContainer1.Panel1.Controls.Add(this.grpActions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
            this.splitContainer1.Size = new System.Drawing.Size(786, 568);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpInput
            // 
            this.grpInput.Controls.Add(this.lblHelp);
            this.grpInput.Controls.Add(this.txtInput);
            this.grpInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInput.Location = new System.Drawing.Point(0, 0);
            this.grpInput.Name = "grpInput";
            this.grpInput.Padding = new System.Windows.Forms.Padding(10);
            this.grpInput.Size = new System.Drawing.Size(436, 150);
            this.grpInput.TabIndex = 1;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Input (Comma separated IDs)";
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.ForeColor = System.Drawing.Color.Gray;
            this.lblHelp.Location = new System.Drawing.Point(13, 50);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(181, 12);
            this.lblHelp.TabIndex = 1;
            this.lblHelp.Text = "e.g. EQP01,EQP02 or LOT01,LOT02";
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtInput.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtInput.Location = new System.Drawing.Point(10, 25);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(416, 23);
            this.txtInput.TabIndex = 0;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnClear);
            this.grpActions.Controls.Add(this.btnTestStepTime);
            this.grpActions.Controls.Add(this.btnTestQTime);
            this.grpActions.Controls.Add(this.btnTestGetOrder);
            this.grpActions.Controls.Add(this.btnTestGetEqStatus);
            this.grpActions.Controls.Add(this.btnTestGetWip);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpActions.Location = new System.Drawing.Point(436, 0);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(350, 150);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(16, 100);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(140, 30);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear Output";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnTestStepTime
            // 
            this.btnTestStepTime.Location = new System.Drawing.Point(168, 100);
            this.btnTestStepTime.Name = "btnTestStepTime";
            this.btnTestStepTime.Size = new System.Drawing.Size(140, 30);
            this.btnTestStepTime.TabIndex = 5;
            this.btnTestStepTime.Text = "Get Std Times";
            this.btnTestStepTime.UseVisualStyleBackColor = true;
            this.btnTestStepTime.Click += new System.EventHandler(this.btnTestStepTime_Click);
            // 
            // btnTestQTime
            // 
            this.btnTestQTime.Location = new System.Drawing.Point(168, 60);
            this.btnTestQTime.Name = "btnTestQTime";
            this.btnTestQTime.Size = new System.Drawing.Size(140, 30);
            this.btnTestQTime.TabIndex = 3;
            this.btnTestQTime.Text = "Get All QTimes";
            this.btnTestQTime.UseVisualStyleBackColor = true;
            this.btnTestQTime.Click += new System.EventHandler(this.btnTestQTime_Click);
            // 
            // btnTestGetOrder
            // 
            this.btnTestGetOrder.Location = new System.Drawing.Point(168, 20);
            this.btnTestGetOrder.Name = "btnTestGetOrder";
            this.btnTestGetOrder.Size = new System.Drawing.Size(140, 30);
            this.btnTestGetOrder.TabIndex = 2;
            this.btnTestGetOrder.Text = "Get Order Info (Batch)";
            this.btnTestGetOrder.UseVisualStyleBackColor = true;
            this.btnTestGetOrder.Click += new System.EventHandler(this.btnTestGetOrder_Click);
            // 
            // btnTestGetEqStatus
            // 
            this.btnTestGetEqStatus.Location = new System.Drawing.Point(16, 60);
            this.btnTestGetEqStatus.Name = "btnTestGetEqStatus";
            this.btnTestGetEqStatus.Size = new System.Drawing.Size(140, 30);
            this.btnTestGetEqStatus.TabIndex = 1;
            this.btnTestGetEqStatus.Text = "Get EqStatus (Batch)";
            this.btnTestGetEqStatus.UseVisualStyleBackColor = true;
            this.btnTestGetEqStatus.Click += new System.EventHandler(this.btnTestGetEqStatus_Click);
            // 
            // btnTestGetWip
            // 
            this.btnTestGetWip.Location = new System.Drawing.Point(16, 20);
            this.btnTestGetWip.Name = "btnTestGetWip";
            this.btnTestGetWip.Size = new System.Drawing.Size(140, 30);
            this.btnTestGetWip.TabIndex = 0;
            this.btnTestGetWip.Text = "Get WIP (Batch)";
            this.btnTestGetWip.UseVisualStyleBackColor = true;
            this.btnTestGetWip.Click += new System.EventHandler(this.btnTestGetWip_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(786, 414);
            this.txtOutput.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.tabControl1);
            this.Name = "TestForm";
            this.Text = "System Test Utility";
            this.tabControl1.ResumeLayout(false);
            this.tabPageMes.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMes;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Button btnTestGetWip;
        private System.Windows.Forms.Button btnTestGetEqStatus;
        private System.Windows.Forms.Button btnTestGetOrder;
        private System.Windows.Forms.Button btnTestQTime;
        private System.Windows.Forms.Button btnTestStepTime;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtOutput;
    }
}
