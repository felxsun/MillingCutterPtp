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
    public partial class frmMain : Form
    {

        private bool inOperation;

        public frmMain()
        {
            InitializeComponent();
            this.inOperation = true;
        }

        private void btnConfig_MouseEnter(object sender, EventArgs e)
        {
            btnConfig.ForeColor = Color.DarkBlue;
        }

        private void btnConfig_MouseLeave(object sender, EventArgs e)
        {
            btnConfig.ForeColor = Color.FromArgb(128, 192, 255);
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("結束程式?", "銑刀分類系統", MessageBoxButtons.YesNo) != DialogResult.Yes)
                e.Cancel = true;
        }

        
        

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.inOperation = !this.inOperation;

            if (!this.inOperation)
            {
                btnPause.Text = ">>   執行";
                lblClassifier.Text = "暫停";
                lblMessage.Text = "";
            }
            else
            {
                btnPause.Text = "II   暫停";
                lblClassifier.Text = "執行中";
                lblMessage.Text = "等待....";
            }
        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void label172_Click(object sender, EventArgs e)
        {

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if(this.inOperation)
            {
                MessageBox.Show("系統執行中,不可更改系統設定");
                return;
            }

            dlgPassword dp = new dlgPassword();
            dp.ShowDialog();


        }
    }
}
