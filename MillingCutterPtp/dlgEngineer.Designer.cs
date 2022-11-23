namespace MillingCutterPtp
{
    partial class dlgEngineer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.btnLogout = new System.Windows.Forms.Button();
			this.btnLogin = new System.Windows.Forms.Button();
			this.txtPass = new System.Windows.Forms.TextBox();
			this.label36 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnLogout
			// 
			this.btnLogout.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnLogout.Location = new System.Drawing.Point(21, 132);
			this.btnLogout.Name = "btnLogout";
			this.btnLogout.Size = new System.Drawing.Size(114, 46);
			this.btnLogout.TabIndex = 65;
			this.btnLogout.Text = "取消";
			this.btnLogout.UseVisualStyleBackColor = true;
			this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
			// 
			// btnLogin
			// 
			this.btnLogin.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnLogin.Location = new System.Drawing.Point(166, 132);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(114, 46);
			this.btnLogin.TabIndex = 64;
			this.btnLogin.Text = "登入";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// txtPass
			// 
			this.txtPass.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtPass.Location = new System.Drawing.Point(21, 80);
			this.txtPass.Name = "txtPass";
			this.txtPass.PasswordChar = '*';
			this.txtPass.Size = new System.Drawing.Size(259, 40);
			this.txtPass.TabIndex = 63;
			this.txtPass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label36
			// 
			this.label36.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label36.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.label36.ForeColor = System.Drawing.Color.Red;
			this.label36.Location = new System.Drawing.Point(21, 25);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(259, 40);
			this.label36.TabIndex = 62;
			this.label36.Text = "請輸入工程模式密碼";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label36.Click += new System.EventHandler(this.label36_Click);
			// 
			// dlgEngineer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(299, 191);
			this.ControlBox = false;
			this.Controls.Add(this.btnLogout);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.txtPass);
			this.Controls.Add(this.label36);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEngineer";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "系統";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.dlgEngineer_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label label36;
    }
}