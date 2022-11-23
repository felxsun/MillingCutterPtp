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
	public partial class boxDialog : Form
	{
		public classBox data;
		public millingCutterClass[] Classes;
		public Guid selectedClass { get; private set; }
		public int Quantity { get; private set; }

		public bool anotherBox = false;

		public boxDialog(classBox cb,millingCutterClass[] classes, uint boxIndex)
		{
			if (cb == null)
				throw new ArgumentNullException("classBox is null");

			InitializeComponent();
			this.data = cb;
			this.Classes = classes;

			this.cboClass.Items.Clear();
			for (int i = 0; i < classes.Length; ++i)
			{
				cboClass.Items.Add(i+1);
				if (data.Class == classes[i].guid)
					cboClass.SelectedIndex = i;
			}

			this.numQty.Value = this.data.Quantity;

            //title
            this.lblTitle.Text = "編輯集裝盒 " + (boxIndex + 1).ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("放棄更新？", "更新集裝盒設置", MessageBoxButtons.YesNo) != DialogResult.Yes)
					return;
			
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.Quantity = (int)numQty.Value;

			this.selectedClass = (this.anotherBox) ? Guid.Empty : this.Classes[this.cboClass.SelectedIndex].guid;

			if (MessageBox.Show("確認更新？", "更新集裝盒設置", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void boxDialog_Load(object sender, EventArgs e)
		{
			if(this.anotherBox)
			{
				cboClass.Visible = false;
			}
			else
			{
				cboClass.Visible = true;
			}
				
		}
	}
}
