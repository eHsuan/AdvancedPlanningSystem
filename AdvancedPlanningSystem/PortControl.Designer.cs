namespace AdvancedPlanningSystem
{
    partial class PortControl
    {
        /// <summary> 
        /// 設計變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblPortID;
        private System.Windows.Forms.Label lblCassetteID;
        private System.Windows.Forms.Label lblWorkNo;

        /// <summary> 
        /// 清除正在使用的資源。
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
            this.lblPortID = new System.Windows.Forms.Label();
            this.lblCassetteID = new System.Windows.Forms.Label();
            this.lblWorkNo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPortID
            // 
            this.lblPortID.AutoSize = true;
            this.lblPortID.BackColor = System.Drawing.Color.Transparent;
            this.lblPortID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPortID.Location = new System.Drawing.Point(2, 2);
            this.lblPortID.Name = "lblPortID";
            this.lblPortID.Size = new System.Drawing.Size(29, 15);
            this.lblPortID.TabIndex = 0;
            this.lblPortID.Text = "P01";
            // 
            // lblCassetteID
            // 
            this.lblCassetteID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCassetteID.BackColor = System.Drawing.Color.Transparent;
            this.lblCassetteID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCassetteID.Location = new System.Drawing.Point(0, 20);
            this.lblCassetteID.Name = "lblCassetteID";
            this.lblCassetteID.Size = new System.Drawing.Size(158, 40);
            this.lblCassetteID.TabIndex = 1;
            this.lblCassetteID.Text = "CASS01";
            this.lblCassetteID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWorkNo
            // 
            this.lblWorkNo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWorkNo.BackColor = System.Drawing.Color.Transparent;
            this.lblWorkNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblWorkNo.Location = new System.Drawing.Point(0, 60);
            this.lblWorkNo.Name = "lblWorkNo";
            this.lblWorkNo.Size = new System.Drawing.Size(158, 30);
            this.lblWorkNo.TabIndex = 2;
            this.lblWorkNo.Text = "WO-0000";
            this.lblWorkNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PortControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblPortID);
            this.Controls.Add(this.lblCassetteID);
            this.Controls.Add(this.lblWorkNo);
            this.Name = "PortControl";
            this.Size = new System.Drawing.Size(160, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
