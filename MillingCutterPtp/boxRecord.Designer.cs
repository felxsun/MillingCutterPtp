namespace MillingCutterPtp
{
    partial class boxRecord
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
			this.basePanel = new System.Windows.Forms.Panel();
			this.lblQty = new System.Windows.Forms.Label();
			this.lblClass = new System.Windows.Forms.Label();
			this.lblClassTitle = new System.Windows.Forms.Label();
			this.lblQtyTitle = new System.Windows.Forms.Label();
			this.lblBoxID = new System.Windows.Forms.Label();
			this.basePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// basePanel
			// 
			this.basePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.basePanel.Controls.Add(this.lblQty);
			this.basePanel.Controls.Add(this.lblClass);
			this.basePanel.Controls.Add(this.lblClassTitle);
			this.basePanel.Controls.Add(this.lblQtyTitle);
			this.basePanel.Controls.Add(this.lblBoxID);
			this.basePanel.Location = new System.Drawing.Point(1, 1);
			this.basePanel.Name = "basePanel";
			this.basePanel.Size = new System.Drawing.Size(258, 85);
			this.basePanel.TabIndex = 210;
			this.basePanel.Click += new System.EventHandler(this.basePanel_Click);
			this.basePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.basePanel_Paint);
			// 
			// lblQty
			// 
			this.lblQty.BackColor = System.Drawing.Color.White;
			this.lblQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblQty.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblQty.Location = new System.Drawing.Point(158, 47);
			this.lblQty.Name = "lblQty";
			this.lblQty.Size = new System.Drawing.Size(90, 35);
			this.lblQty.TabIndex = 75;
			this.lblQty.Tag = "器";
			this.lblQty.Text = "-";
			this.lblQty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblQty.Click += new System.EventHandler(this.lblQty_Click);
			// 
			// lblClass
			// 
			this.lblClass.BackColor = System.Drawing.Color.White;
			this.lblClass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblClass.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblClass.Location = new System.Drawing.Point(158, 9);
			this.lblClass.Name = "lblClass";
			this.lblClass.Size = new System.Drawing.Size(90, 35);
			this.lblClass.TabIndex = 74;
			this.lblClass.Tag = "器";
			this.lblClass.Text = "-";
			this.lblClass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblClass.Click += new System.EventHandler(this.lblClass_Click);
			// 
			// lblClassTitle
			// 
			this.lblClassTitle.AutoSize = true;
			this.lblClassTitle.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblClassTitle.Location = new System.Drawing.Point(90, 9);
			this.lblClassTitle.Name = "lblClassTitle";
			this.lblClassTitle.Size = new System.Drawing.Size(62, 31);
			this.lblClassTitle.TabIndex = 73;
			this.lblClassTitle.Tag = "器";
			this.lblClassTitle.Text = "分類";
			this.lblClassTitle.Click += new System.EventHandler(this.lblClassTitle_Click);
			// 
			// lblQtyTitle
			// 
			this.lblQtyTitle.AutoSize = true;
			this.lblQtyTitle.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblQtyTitle.Location = new System.Drawing.Point(90, 48);
			this.lblQtyTitle.Name = "lblQtyTitle";
			this.lblQtyTitle.Size = new System.Drawing.Size(62, 31);
			this.lblQtyTitle.TabIndex = 71;
			this.lblQtyTitle.Text = "容量";
			this.lblQtyTitle.Click += new System.EventHandler(this.lblQtyTitle_Click);
			// 
			// lblBoxID
			// 
			this.lblBoxID.BackColor = System.Drawing.Color.Gray;
			this.lblBoxID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblBoxID.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblBoxID.ForeColor = System.Drawing.Color.White;
			this.lblBoxID.Location = new System.Drawing.Point(0, 0);
			this.lblBoxID.Name = "lblBoxID";
			this.lblBoxID.Size = new System.Drawing.Size(84, 84);
			this.lblBoxID.TabIndex = 70;
			this.lblBoxID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblBoxID.Click += new System.EventHandler(this.lblBoxID_Click);
			// 
			// boxRecord
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.basePanel);
			this.Name = "boxRecord";
			this.Size = new System.Drawing.Size(260, 87);
			this.Load += new System.EventHandler(this.boxRecord_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.boxRecord_Paint);
			this.basePanel.ResumeLayout(false);
			this.basePanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel basePanel;
        private System.Windows.Forms.Label lblClassTitle;
        private System.Windows.Forms.Label lblQtyTitle;
        private System.Windows.Forms.Label lblBoxID;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblClass;
    }
}
