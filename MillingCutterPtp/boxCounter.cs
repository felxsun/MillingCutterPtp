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
    public partial class boxCounter : UserControl
    {
		public uint? SN = null;

        private Size fiexedSize = new Size(477, 93);
		
		public double maxFullLength { set { lblFullLengthMax.Text = doubleString(value);  } }
		public double minFullLength { set { lblFullLengthMin.Text =  doubleString(value); } }
		public double maxHandleLength { set { lblHandleMax.Text = doubleString(value); } }
		public double minHandleLength { set { lblHandleMin.Text = doubleString(value); } }
		public double maxHandleWidth { set { lblHandleWidthMax.Text = doubleString(value); } }
		public double minHandleWidth { set { lblHandleWidthMin.Text = doubleString(value); } }
		public double maxBladeWidth { set { lblBladeWidthMax.Text =  doubleString(value); } }
		public double minBladeWidth { set { lblBladeWidthMin.Text = doubleString(value); } }

		private string doubleString(double value)
		{
			if (double.IsNaN(value))
				return "-";
			if (double.IsInfinity(value))
				return "+";

			return value.ToString("0.00");
		}

		public int count;
		public void reset()
		{
			count = 0;
			setLabelText(this.lblCount, this.count.ToString());
		}
		public void Counting()
		{
			++this.count;
			setLabelText(this.lblCount, this.count.ToString());
		}

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

        public boxCounter()
        {
            InitializeComponent();
			this.count = 0;
        }

		delegate void setLabelTextboxCallBack(Label box, string text);
		private void setLabelText(Label lbl, string text)
		{
			if (lbl.InvokeRequired)
			{
				setLabelTextboxCallBack d = new setLabelTextboxCallBack(setLabelText);
				this.Invoke(d, new object[] { lbl, text });
			}
			else
				lbl.Text = text;
		}

		public static boxCounter Create(uint? sn, millingCutterClass mcc)
        {
            boxCounter rtn = new boxCounter();
			rtn.SN = sn;
            if(mcc!=null) 
                rtn.set(mcc);

            return rtn;
        }
        
        private void initialzeControls()
        {
            this.lblClass.BackColor = this.BackColor;
            this.lblClass.Size = new Size(84, 84);
            this.lblClass.Location = new Point(1, 4);
            this.lblClass.Text = "";

            this.lblFullLengthMax.Size = new Size(90, 30);
            this.lblFullLengthMin.Size = new Size(90, 30);
            this.lblHandleMax.Size = new Size(90, 30);
            this.lblHandleMin.Size = new Size(90, 30);
            this.lblBladeWidthMax.Size = new Size(90, 30);
            this.lblBladeWidthMin.Size = new Size(90, 30);
            this.lblHandleWidthMax.Size = new Size(90, 30);
            this.lblHandleWidthMin.Size = new Size(90, 30);

            this.lblFullLengthMax.Location = new Point(105, 1);
            this.lblFullLengthMin.Location = new Point(105, 31);
            this.lblHandleMax.Location = new Point(198, 1);
            this.lblHandleMin.Location = new Point(198, 31);
            this.lblBladeWidthMax.Location = new Point(291, 1);
            this.lblBladeWidthMin.Location = new Point(291, 31);
            this.lblHandleWidthMax.Location = new Point(384, 1);
            this.lblHandleWidthMin.Location = new Point(384, 31);

            this.lblFullLengthMax.Text = "";
            this.lblFullLengthMin.Text = "";
            this.lblHandleMax.Text = "";
            this.lblHandleMin.Text = "";
            this.lblBladeWidthMax.Text = "";
            this.lblBladeWidthMin.Text = "";
            this.lblHandleWidthMax.Text = "";
            this.lblHandleWidthMin.Text = "";

            this.lblText.Size = new Size(145, 30);
            this.lblText.Location = new Point(105,61);
            this.lblText.Text = "計 數";

            this.lblCount.Size = new Size(178, 30);
            this.lblCount.Location = new Point(296, 61);
            this.lblCount.Text = "";
        }

        public void set(millingCutterClass mcc)
        {
            this.lblFullLengthMax.Text = mcc.maxLength.ToString("0.00");
            this.lblFullLengthMin.Text = mcc.minLength.ToString("0.00");
            this.lblHandleMax.Text = mcc.maxHandleLength.ToString("0.00");
            this.lblHandleMin.Text = mcc.minHandleLength.ToString("0.00");
            this.lblHandleWidthMax.Text = mcc.maxHandleWidth.ToString("0.00");
            this.lblHandleWidthMin.Text = mcc.minHandleWidth.ToString("0.00");
            this.lblBladeWidthMax.Text = mcc.maxBladeWidth.ToString("0.00");
            this.lblBladeWidthMin.Text = mcc.minBladeWidth.ToString("0.00");
			this.count = 0;
        }

		private void boxCounter_Load(object sender, EventArgs e)
		{
			initialzeControls();
		}

		private void boxCounter_Paint(object sender, PaintEventArgs e)
		{
			setLabelText(this.lblClass, (this.SN == null) ? "-" : this.SN.ToString());
			setLabelText(this.lblCount, this.count.ToString());
		}
	}
}
