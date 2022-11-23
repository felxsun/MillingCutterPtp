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
    public partial class dlgEngineer : Form
    {
		public string password;

        public dlgEngineer()
        {
            InitializeComponent();
        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void dlgEngineer_Load(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!(this.password==null) && !(this.password.Length<1) && txtPass.Text == this.password)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("密碼錯誤");
                txtPass.Text = "";
            }
        }
    }
}
