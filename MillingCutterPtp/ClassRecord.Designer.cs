namespace MillingCutterPtp
{
    partial class ClassRecord
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassRecord));
            this.basePanel = new System.Windows.Forms.Panel();
            this.lblHandleWidthMin = new System.Windows.Forms.Label();
            this.lblHandleWidthMax = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lblBladeMin = new System.Windows.Forms.Label();
            this.lblBladeMax = new System.Windows.Forms.Label();
            this.lblHandleLenMin = new System.Windows.Forms.Label();
            this.lblHandleLenMax = new System.Windows.Forms.Label();
            this.lblFullMin = new System.Windows.Forms.Label();
            this.lblFullMax = new System.Windows.Forms.Label();
            this.lblSN = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.basePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // basePanel
            // 
            this.basePanel.BackColor = System.Drawing.Color.Gainsboro;
            this.basePanel.Controls.Add(this.btnAdd);
            this.basePanel.Controls.Add(this.lblHandleWidthMin);
            this.basePanel.Controls.Add(this.lblHandleWidthMax);
            this.basePanel.Controls.Add(this.btnDelete);
            this.basePanel.Controls.Add(this.btnDown);
            this.basePanel.Controls.Add(this.btnUp);
            this.basePanel.Controls.Add(this.btnEdit);
            this.basePanel.Controls.Add(this.lblBladeMin);
            this.basePanel.Controls.Add(this.lblBladeMax);
            this.basePanel.Controls.Add(this.lblHandleLenMin);
            this.basePanel.Controls.Add(this.lblHandleLenMax);
            this.basePanel.Controls.Add(this.lblFullMin);
            this.basePanel.Controls.Add(this.lblFullMax);
            this.basePanel.Controls.Add(this.lblSN);
            this.basePanel.Location = new System.Drawing.Point(1, 1);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(912, 48);
            this.basePanel.TabIndex = 183;
            this.basePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ClassRecord_Paint);
            // 
            // lblHandleWidthMin
            // 
            this.lblHandleWidthMin.BackColor = System.Drawing.Color.White;
            this.lblHandleWidthMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHandleWidthMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblHandleWidthMin.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHandleWidthMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHandleWidthMin.Location = new System.Drawing.Point(709, 9);
            this.lblHandleWidthMin.Name = "lblHandleWidthMin";
            this.lblHandleWidthMin.Size = new System.Drawing.Size(85, 30);
            this.lblHandleWidthMin.TabIndex = 88;
            this.lblHandleWidthMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHandleWidthMax
            // 
            this.lblHandleWidthMax.BackColor = System.Drawing.Color.White;
            this.lblHandleWidthMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHandleWidthMax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblHandleWidthMax.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHandleWidthMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHandleWidthMax.Location = new System.Drawing.Point(622, 9);
            this.lblHandleWidthMax.Name = "lblHandleWidthMax";
            this.lblHandleWidthMax.Size = new System.Drawing.Size(85, 30);
            this.lblHandleWidthMax.TabIndex = 87;
            this.lblHandleWidthMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnDelete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDelete.BackgroundImage")));
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDelete.Location = new System.Drawing.Point(873, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(38, 36);
            this.btnDelete.TabIndex = 86;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDown.BackgroundImage")));
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(833, 6);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(38, 36);
            this.btnDown.TabIndex = 85;
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUp.BackgroundImage")));
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(796, 6);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(38, 36);
            this.btnUp.TabIndex = 84;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnEdit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEdit.BackgroundImage")));
            this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnEdit.Location = new System.Drawing.Point(48, 6);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(38, 36);
            this.btnEdit.TabIndex = 82;
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblBladeMin
            // 
            this.lblBladeMin.BackColor = System.Drawing.Color.White;
            this.lblBladeMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBladeMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblBladeMin.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBladeMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblBladeMin.Location = new System.Drawing.Point(530, 9);
            this.lblBladeMin.Name = "lblBladeMin";
            this.lblBladeMin.Size = new System.Drawing.Size(85, 30);
            this.lblBladeMin.TabIndex = 81;
            this.lblBladeMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBladeMax
            // 
            this.lblBladeMax.BackColor = System.Drawing.Color.White;
            this.lblBladeMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBladeMax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblBladeMax.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBladeMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblBladeMax.Location = new System.Drawing.Point(443, 9);
            this.lblBladeMax.Name = "lblBladeMax";
            this.lblBladeMax.Size = new System.Drawing.Size(85, 30);
            this.lblBladeMax.TabIndex = 80;
            this.lblBladeMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHandleLenMin
            // 
            this.lblHandleLenMin.BackColor = System.Drawing.Color.White;
            this.lblHandleLenMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHandleLenMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblHandleLenMin.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHandleLenMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHandleLenMin.Location = new System.Drawing.Point(352, 9);
            this.lblHandleLenMin.Name = "lblHandleLenMin";
            this.lblHandleLenMin.Size = new System.Drawing.Size(85, 30);
            this.lblHandleLenMin.TabIndex = 79;
            this.lblHandleLenMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHandleLenMax
            // 
            this.lblHandleLenMax.BackColor = System.Drawing.Color.White;
            this.lblHandleLenMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHandleLenMax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblHandleLenMax.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHandleLenMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHandleLenMax.Location = new System.Drawing.Point(265, 9);
            this.lblHandleLenMax.Name = "lblHandleLenMax";
            this.lblHandleLenMax.Size = new System.Drawing.Size(85, 30);
            this.lblHandleLenMax.TabIndex = 78;
            this.lblHandleLenMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFullMin
            // 
            this.lblFullMin.BackColor = System.Drawing.Color.White;
            this.lblFullMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFullMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFullMin.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblFullMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblFullMin.Location = new System.Drawing.Point(174, 9);
            this.lblFullMin.Name = "lblFullMin";
            this.lblFullMin.Size = new System.Drawing.Size(85, 30);
            this.lblFullMin.TabIndex = 77;
            this.lblFullMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFullMax
            // 
            this.lblFullMax.BackColor = System.Drawing.Color.White;
            this.lblFullMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFullMax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFullMax.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblFullMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblFullMax.Location = new System.Drawing.Point(87, 9);
            this.lblFullMax.Name = "lblFullMax";
            this.lblFullMax.Size = new System.Drawing.Size(85, 30);
            this.lblFullMax.TabIndex = 76;
            this.lblFullMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSN
            // 
            this.lblSN.BackColor = System.Drawing.Color.White;
            this.lblSN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSN.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSN.ForeColor = System.Drawing.Color.Black;
            this.lblSN.Location = new System.Drawing.Point(0, 0);
            this.lblSN.Name = "lblSN";
            this.lblSN.Size = new System.Drawing.Size(48, 48);
            this.lblSN.TabIndex = 58;
            this.lblSN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAdd.BackgroundImage")));
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.Location = new System.Drawing.Point(92, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(38, 36);
            this.btnAdd.TabIndex = 89;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // ClassRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.basePanel);
            this.Name = "ClassRecord";
            this.Size = new System.Drawing.Size(912, 50);
            this.Load += new System.EventHandler(this.ClassRecord_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ClassRecord_Paint);
            this.basePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel basePanel;
        private System.Windows.Forms.Label lblHandleWidthMin;
        private System.Windows.Forms.Label lblHandleWidthMax;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblBladeMin;
        private System.Windows.Forms.Label lblBladeMax;
        private System.Windows.Forms.Label lblHandleLenMin;
        private System.Windows.Forms.Label lblHandleLenMax;
        private System.Windows.Forms.Label lblFullMin;
        private System.Windows.Forms.Label lblFullMax;
        private System.Windows.Forms.Label lblSN;
        private System.Windows.Forms.Button btnAdd;
    }
}
