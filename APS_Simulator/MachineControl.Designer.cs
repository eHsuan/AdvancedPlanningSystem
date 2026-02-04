namespace APSSimulator
{
    partial class MachineControl
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.grpMachine = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblWip = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnIdle = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.numWip = new System.Windows.Forms.NumericUpDown();
            this.btnUpdateWip = new System.Windows.Forms.Button();
            this.grpMachine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWip)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMachine
            // 
            this.grpMachine.Controls.Add(this.btnUpdateWip);
            this.grpMachine.Controls.Add(this.numWip);
            this.grpMachine.Controls.Add(this.btnDown);
            this.grpMachine.Controls.Add(this.btnIdle);
            this.grpMachine.Controls.Add(this.btnRun);
            this.grpMachine.Controls.Add(this.lblWip);
            this.grpMachine.Controls.Add(this.lblStatus);
            this.grpMachine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMachine.Location = new System.Drawing.Point(0, 0);
            this.grpMachine.Name = "grpMachine";
            this.grpMachine.Size = new System.Drawing.Size(220, 150);
            this.grpMachine.TabIndex = 0;
            this.grpMachine.TabStop = false;
            this.grpMachine.Text = "EQP Name";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 25);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 12);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status:";
            // 
            // lblWip
            // 
            this.lblWip.AutoSize = true;
            this.lblWip.Location = new System.Drawing.Point(15, 50);
            this.lblWip.Name = "lblWip";
            this.lblWip.Size = new System.Drawing.Size(29, 12);
            this.lblWip.TabIndex = 1;
            this.lblWip.Text = "WIP:";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(15, 75);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(60, 25);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // btnIdle
            // 
            this.btnIdle.Location = new System.Drawing.Point(80, 75);
            this.btnIdle.Name = "btnIdle";
            this.btnIdle.Size = new System.Drawing.Size(60, 25);
            this.btnIdle.TabIndex = 3;
            this.btnIdle.Text = "IDLE";
            this.btnIdle.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(145, 75);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(60, 25);
            this.btnDown.TabIndex = 4;
            this.btnDown.Text = "DOWN";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // numWip
            // 
            this.numWip.Location = new System.Drawing.Point(15, 115);
            this.numWip.Name = "numWip";
            this.numWip.Size = new System.Drawing.Size(60, 22);
            this.numWip.TabIndex = 5;
            // 
            // btnUpdateWip
            // 
            this.btnUpdateWip.Location = new System.Drawing.Point(85, 113);
            this.btnUpdateWip.Name = "btnUpdateWip";
            this.btnUpdateWip.Size = new System.Drawing.Size(80, 25);
            this.btnUpdateWip.TabIndex = 6;
            this.btnUpdateWip.Text = "Update WIP";
            this.btnUpdateWip.UseVisualStyleBackColor = true;
            // 
            // MachineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMachine);
            this.Name = "MachineControl";
            this.Size = new System.Drawing.Size(220, 150);
            this.grpMachine.ResumeLayout(false);
            this.grpMachine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWip)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMachine;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblWip;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnIdle;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.NumericUpDown numWip;
        private System.Windows.Forms.Button btnUpdateWip;
    }
}
