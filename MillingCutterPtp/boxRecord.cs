using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MillingCutterPtp
{
    public partial class boxRecord : UserControl
    {
        public uint quantity = 4000;
        public uint? Class = null;

        public uint ID;

        private Size fiexedSize = new Size(260, 87);
        public override Size MinimumSize
        {
            get { return fiexedSize; }
            set { }
        }
        public override Size MaximumSize
        {
            get { return fiexedSize; }
            set { }
        }

        public boxRecord()
        {
            InitializeComponent();
        }


        private void boxRecord_Load(object sender, EventArgs e)
        {
            basePanel.Location = new Point(1, 1);
            basePanel.Size = new Size(258, 85);

            this.lblQty.Location = new System.Drawing.Point(158, 44);
            this.lblQty.Size = new System.Drawing.Size(90, 35);

            this.lblBoxID.Location = new System.Drawing.Point(0, 0);
            this.lblBoxID.Size = new System.Drawing.Size(84, 84);

            this.lblClass.Location = new System.Drawing.Point(158, 5);
            this.lblClass.Size = new System.Drawing.Size(90, 35);

            this.lblClassTitle.Location = new System.Drawing.Point(90, 9);
            this.lblClassTitle.Size = new System.Drawing.Size(62, 31);

            this.lblQtyTitle.Location = new System.Drawing.Point(90, 48);
            this.lblQtyTitle.Size = new System.Drawing.Size(62, 31);
        }

        private void basePanel_Paint(object sender, PaintEventArgs e)
        {
			this.doPaint();
        }
		public void doPaint()
		{
			this.lblBoxID.Text = (this.ID + 1).ToString();
			this.lblClass.Text = (this.Class == null) ? "-" : this.Class.ToString();
			this.lblQty.Text = this.quantity.ToString();
		}

        
		//event 
		public event EventHandler editClick;

		private void lblBoxID_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this,e);
		}

		private void basePanel_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this, e);
		}

		private void lblClass_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this, e);
		}

		private void lblQty_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this, e);
		}

		private void lblClassTitle_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this, e);
		}

		private void lblQtyTitle_Click(object sender, EventArgs e)
		{
			EventHandler handler = this.editClick;
			if (handler != null) handler(this, e);
		}

		private void boxRecord_Paint(object sender, PaintEventArgs e)
		{
			
		}
	}
}
