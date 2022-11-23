using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MillingCutterPtp
{
    public partial class dlgPassword : Form
    {
        public bool pass;
		public bool shutdown = false;
		public string password = null;
		public bool shutdownOnly = true;

        public dlgPassword()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            pass = false;
            if (!(password==null) && !(password.Length<1) && txtPass.Text==password)
            {
                this.pass = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("密碼錯誤");
                txtPass.Text = "";
            }
        }

		private void label36_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.shutdown = false;
			if (MessageBox.Show("確認結束程式？", "關閉系統", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				shutdown = true;
				this.Close();
			}
		}

		private void dlgPassword_Load(object sender, EventArgs e)
		{
			if(this.shutdownOnly)
			{
				this.txtPass.Enabled = false;
				this.btnLogin.Enabled = false;
			}
			else
			{
				this.txtPass.Enabled = true;
				this.btnLogin.Enabled = true;
			}
		}
	}
}
