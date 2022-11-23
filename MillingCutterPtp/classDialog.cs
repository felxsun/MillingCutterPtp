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
    public partial class classDialog : Form
    {
        public millingCutterClass data;
        public millingCutterClass rtn;
        private bool useBladeInner=true;
        private int indexNumber=0;

        public classDialog(millingCutterClass mcc, int index, bool usingBladeInner)
        {
            InitializeComponent();
            this.data = mcc;
            this.useBladeInner = usingBladeInner;
            this.indexNumber = index;

            rtn = (data == null) ?  new millingCutterClass() : new millingCutterClass(data.ToString());
        }

        private bool checkAndSetResult()
        {
            int countHasValue = 0;

            if (txtFullLeghtUp.Text == "")
            {
                rtn.maxLength = double.NaN;
            }
            else if (!double.TryParse(txtFullLeghtUp.Text, out rtn.maxLength))
            {
                MessageBox.Show("全長上限內容錯誤");
                txtFullLeghtUp.Focus();
                return false;

            }
            else
                countHasValue++;

            if (txtFullLeghtLower.Text == "")
            {
                rtn.minLength = double.NaN;
            }
            else if (!double.TryParse(txtFullLeghtLower.Text, out rtn.minLength))
            {
                MessageBox.Show("全長下限內容錯誤");
                txtFullLeghtLower.Focus();
                return false;
            }
            else
                ++countHasValue;

            if(txtFullLeghtUp.Text!="" && txtFullLeghtLower.Text!="" &&  rtn.maxLength<rtn.minLength)
            {
                MessageBox.Show("全長上限值小於全長下限值");
                txtFullLeghtUp.Focus();
                return false;
            }

            if (txtHandleUp.Text == "")
            {
                rtn.maxHandleLength = double.NaN;
            }
            else if (!double.TryParse(txtHandleUp.Text, out rtn.maxHandleLength))
            {
                MessageBox.Show("柄長上限內容錯誤");
                txtHandleUp.Focus();
                return false;
            }
            else
                ++countHasValue;

            if (txtHandleDown.Text == "")
            {
                rtn.minHandleLength = double.NaN;
            }
            else if (!double.TryParse(txtHandleDown.Text, out rtn.minHandleLength))
            {
                MessageBox.Show("柄長下限內容錯誤");
                txtHandleDown.Focus();
                return false;
            }
            else
                ++countHasValue;

            if(txtHandleUp.Text!="" && txtHandleDown.Text!="" && rtn.maxHandleLength<rtn.minHandleLength)
            {
                MessageBox.Show("柄長上限值小於柄長下限值");
                txtHandleUp.Focus();
                return false;
            }

            if (txtBladeUp.Text == "")
            {
                rtn.maxBladeWidth = double.NaN;
            }
            else if (!double.TryParse(txtBladeUp.Text, out rtn.maxBladeWidth))
            {
                MessageBox.Show("刃徑上限內容錯誤");
                txtBladeUp.Focus();
                return false;
            }
            else
                ++countHasValue;

            if (txtBladeLower.Text == "")
            {
                rtn.minBladeWidth = double.NaN;
            }
            else if (!double.TryParse(txtBladeLower.Text, out rtn.minBladeWidth))
            {
                MessageBox.Show("刃徑下限內容錯誤");
                txtBladeLower.Focus();
                return false;
            }
            else
                ++countHasValue;

            if (txtBladeUp.Text != "" && txtBladeLower.Text != "" && rtn.maxBladeWidth < rtn.minBladeWidth)
            {
                MessageBox.Show("刃徑上限值小於刃徑下限值");
                txtBladeUp.Focus();
                return false;
            }

            if (txtHandleWithUp.Text == "")
            {
                rtn.maxHandleWidth = double.NaN;
            }

            else if (!double.TryParse(txtHandleWithUp.Text, out rtn.maxHandleWidth))
            {
                MessageBox.Show("柄徑上限內容錯誤");
                txtHandleWithUp.Focus();
                return false;
            }
            else
                ++countHasValue;

            if (txtHandleWithDown.Text == "")
            {
                rtn.minHandleWidth = double.NaN;
            }
            else if (!double.TryParse(txtHandleWithDown.Text, out rtn.minHandleWidth))
            {
                MessageBox.Show("柄徑下限內容錯誤");
                txtHandleWithDown.Focus();
                return false;
            }
            else
                ++countHasValue;

            if (txtHandleWithUp.Text != "" && txtHandleWithDown.Text != "" && rtn.maxHandleWidth < rtn.minHandleWidth)
            {
                MessageBox.Show("柄徑上限值小於柄徑下限值");
                txtHandleWithUp.Focus();
                return false;
            }

            if (countHasValue < 1)
            {
                MessageBox.Show("不得全部項目設置為忽略 (與“其他類”功能重疊)");
                return false;
            }

            return true;
        }

        private void button39_Click(object sender, EventArgs e)
        {

            if (!this.checkAndSetResult())
                return;
            if(this.data==null)
            {
                if (MessageBox.Show("確認新增？", "新增分類", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
            }
            else
            {
                if (MessageBox.Show("確認更新？", "更新分類", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void setTitle(string str)
        {
            this.lblTitle.Text = str;
        }

        public void setClassID(int id)
        {
            this.lblAddClassID.Text = id.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(data==null)
            {
                if (MessageBox.Show("放棄新增？", "新增分類類別", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;    
            }
            else
            {
                if (MessageBox.Show("放棄編輯內容？", "分類項目編輯", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            rtn = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void classDialog_Load(object sender, EventArgs e)
        {
            if(this.data==null)
            {
                this.lblTitle.Text = "新增分類"+ ((this.indexNumber > 0) ? " "+this.indexNumber.ToString() : "");
                this.txtFullLeghtUp.Text = "";
                this.txtFullLeghtLower.Text = "";
                this.txtHandleUp.Text = "";
                this.txtHandleDown.Text = "";
                this.txtBladeUp.Text = "";
                this.txtBladeLower.Text = "";
                this.txtHandleWithUp.Text = "";
                this.txtHandleWithDown.Text = "";
            }
            else
            {
                this.txtFullLeghtUp.Text = (double.IsNaN(this.data.maxLength)) ? "" : this.data.maxLength.ToString("0.00");
                this.txtFullLeghtLower.Text = (double.IsNaN(this.data.minLength)) ? "" : this.data.minLength.ToString("0.00");
                this.txtHandleUp.Text = (double.IsNaN(this.data.maxHandleLength)) ? "" : this.data.maxHandleLength.ToString("0.00");
                this.txtHandleDown.Text = (double.IsNaN(this.data.minHandleLength)) ? "" : this.data.minHandleLength.ToString("0.00");
                this.txtBladeUp.Text = (double.IsNaN(this.data.maxBladeWidth)) ? "" : this.data.maxBladeWidth.ToString("0.00");
                this.txtBladeLower.Text = (double.IsNaN(this.data.minBladeWidth)) ? "" : this.data.minBladeWidth.ToString("0.00");
                this.txtHandleWithUp.Text = (double.IsNaN(this.data.maxHandleWidth)) ? "" : this.data.maxHandleWidth.ToString("0.00");
                this.txtHandleWithDown.Text = (double.IsNaN(this.data.minHandleWidth)) ? "" : this.data.minHandleWidth.ToString("0.00");
                this.lblTitle.Text = "編輯分類" + ((this.indexNumber > 0) ? " " + this.indexNumber.ToString() : "");
            }

            this.lblBlade.Text = (this.useBladeInner) ? "芯厚" : "刃外徑";
        }

        private void txt_Validating(object sender, CancelEventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            double val;

            if (tbx.Text == "")
                return;

            if(!double.TryParse(tbx.Text,out val))
            {
                MessageBox.Show("不合法的數值, 請修正或留白");
                e.Cancel = true;
                return;
            }

            tbx.Text = val.ToString("0.00");
        }

        
    }
}
