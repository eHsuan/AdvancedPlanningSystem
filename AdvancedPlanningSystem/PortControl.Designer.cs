namespace AdvancedPlanningSystem
{
    partial class PortControl
    {
        /// <summary> 
        /// 設計變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblPortID;
        private System.Windows.Forms.Label lblCstInfo;
        private System.Windows.Forms.Label lblWorkInfo;
        private System.Windows.Forms.Label lblTargetInfo;
        private System.Windows.Forms.Label lblCstStatus;
        private System.Windows.Forms.ToolTip toolTipInfo;

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
            this.components = new System.ComponentModel.Container();
            this.lblPortID = new System.Windows.Forms.Label();
            this.lblCstInfo = new System.Windows.Forms.Label();
            this.lblWorkInfo = new System.Windows.Forms.Label();
            this.lblTargetInfo = new System.Windows.Forms.Label();
            this.lblCstStatus = new System.Windows.Forms.Label();
            this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
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
            // lblCstInfo
            // 
            this.lblCstInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCstInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblCstInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCstInfo.Location = new System.Drawing.Point(2, 22);
            this.lblCstInfo.Name = "lblCstInfo";
            this.lblCstInfo.Size = new System.Drawing.Size(156, 15);
            this.lblCstInfo.TabIndex = 1;
            this.lblCstInfo.Text = "CstID : ";
            this.lblCstInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWorkInfo
            // 
            this.lblWorkInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWorkInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblWorkInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkInfo.Location = new System.Drawing.Point(2, 38);
            this.lblWorkInfo.Name = "lblWorkInfo";
            this.lblWorkInfo.Size = new System.Drawing.Size(156, 15);
            this.lblWorkInfo.TabIndex = 2;
            this.lblWorkInfo.Text = "WorkNo : ";
            this.lblWorkInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTargetInfo
            // 
            this.lblTargetInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTargetInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblTargetInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetInfo.ForeColor = System.Drawing.Color.Maroon;
            this.lblTargetInfo.Location = new System.Drawing.Point(2, 55);
            this.lblTargetInfo.Name = "lblTargetInfo";
            this.lblTargetInfo.Size = new System.Drawing.Size(156, 18);
            this.lblTargetInfo.TabIndex = 3;
            this.lblTargetInfo.Text = "Target : ";
            this.lblTargetInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCstStatus
            // 
            this.lblCstStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCstStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblCstStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCstStatus.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCstStatus.Location = new System.Drawing.Point(50, 2);
            this.lblCstStatus.Name = "lblCstStatus";
            this.lblCstStatus.Size = new System.Drawing.Size(108, 15);
            this.lblCstStatus.TabIndex = 4;
            this.lblCstStatus.Text = "WAIT";
            this.lblCstStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PortControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblCstStatus);
            this.Controls.Add(this.lblTargetInfo);
            this.Controls.Add(this.lblWorkInfo);
            this.Controls.Add(this.lblCstInfo);
            this.Controls.Add(this.lblPortID);
            this.Name = "PortControl";
            this.Size = new System.Drawing.Size(160, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
