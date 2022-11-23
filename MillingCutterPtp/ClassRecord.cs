using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace MillingCutterPtp
{
    public partial class ClassRecord : UserControl
    {
        public millingCutterClass data;
        public uint index;

        private Size fiexedSize = new Size(912, 50);
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

        //resource
        private Assembly myAssembly;
        private Stream editIcon;
        private Stream plusIcon;

        public ClassRecord(ushort index, millingCutterClass mcc = null)
        {
            InitializeComponent();
            InitializeEvents();

            myAssembly = Assembly.GetExecutingAssembly();
            editIcon = myAssembly.GetManifestResourceStream("MillingCutterPtp.btnEdit.gif");
            plusIcon = myAssembly.GetManifestResourceStream("MillingCutterPtp.btnPlus.gif");

            this.data = mcc;
        }

        private void InitializeEvents()
        {
            //btnEdit.Click += editButtonClick;
            //btnUp.Click += upButtonClick;
            //btnDown.Click += downButtonClick;
            //btnDelete.Click += deleteButtonClick;
            //btnAdd.Click += addButtonClick;
        }


        private void ClassRecord_Paint(object sender, PaintEventArgs e)
        {
            this.refresh();
        }

        public void refresh()
        {
            lblSN.Text = (this.index > 0) ? this.index.ToString() : "";
            if (this.data == null)
            {
                lblFullMax.Visible = false;
                lblFullMin.Visible = false;
                lblHandleLenMax.Visible = false;
                lblHandleLenMin.Visible = false;
                lblBladeMax.Visible = false;
                lblBladeMin.Visible = false;
                lblHandleWidthMax.Visible = false;
                lblHandleWidthMin.Visible = false;
                btnDelete.Visible = false;
                btnUp.Visible = false;
                btnDown.Visible = false;
                btnEdit.Visible = false;
                btnAdd.Visible = true;
            }
            else
            {
                lblFullMax.Text = (double.IsNaN(data.maxLength)) ? "-" : data.maxLength.ToString("0.00");
                lblFullMin.Text = (double.IsNaN(data.minLength)) ? "-" : data.minLength.ToString("0.00");
                lblHandleLenMax.Text = (double.IsNaN(data.maxHandleLength)) ? "-" : data.maxHandleLength.ToString("0.00");
                lblHandleLenMin.Text = (double.IsNaN(data.minHandleLength)) ? "-" : data.minHandleLength.ToString("0.00");
                lblBladeMax.Text = (double.IsNaN(data.maxBladeWidth)) ? "-" : data.maxBladeWidth.ToString("0.00");
                lblBladeMin.Text = (double.IsNaN(data.minBladeWidth)) ? "-" : data.minBladeWidth.ToString("0.00");
                lblHandleWidthMax.Text = (double.IsNaN(data.maxHandleWidth)) ? "-" : data.maxHandleWidth.ToString("0.00");
                lblHandleWidthMin.Text = (double.IsNaN(data.minHandleWidth)) ? "-" : data.minHandleWidth.ToString("0.00");
                lblFullMax.Visible = true;
                lblFullMin.Visible = true;
                lblHandleLenMax.Visible = true;
                lblHandleLenMin.Visible = true;
                lblBladeMax.Visible = true;
                lblBladeMin.Visible = true;
                lblHandleWidthMax.Visible = true;
                lblHandleWidthMin.Visible = true;
                btnDelete.Visible = true;
                btnUp.Visible = true;
                btnDown.Visible = true;
                btnEdit.Visible = true;
                btnAdd.Visible = false;
            }
        }

        private void ClassRecord_Load(object sender, EventArgs e)
        {
            basePanel.Size = new Size(910, 48);
            basePanel.Location = new Point(1, 1);
            this.btnEdit.Location = new System.Drawing.Point(48, 6);
            this.btnEdit.Size = new System.Drawing.Size(38, 36);
            this.btnAdd.Location = new System.Drawing.Point(48, 6);
            this.btnAdd.Size = new System.Drawing.Size(38, 36);

            this.btnDelete.Location = new System.Drawing.Point(873, 6);
            this.btnDelete.Size = new System.Drawing.Size(38, 36);
            this.btnDown.Location = new System.Drawing.Point(833, 6);
            this.btnDown.Size = new System.Drawing.Size(38, 36);
            this.btnUp.Location = new System.Drawing.Point(796, 6);
            this.btnUp.Size = new System.Drawing.Size(38, 36);

            this.lblSN.Size = new Size(48, 48);
            this.lblSN.Location = new System.Drawing.Point(0, 0);
            this.lblBladeMin.Location = new System.Drawing.Point(530, 9);
            this.lblBladeMin.Size = new System.Drawing.Size(85, 30);
            this.lblBladeMax.Location = new System.Drawing.Point(443, 9);
            this.lblBladeMax.Size = new System.Drawing.Size(85, 30);
            this.lblHandleLenMin.Location = new System.Drawing.Point(352, 9);
            this.lblHandleLenMin.Size = new System.Drawing.Size(85, 30);
            this.lblHandleLenMax.Location = new System.Drawing.Point(265, 9);
            this.lblHandleLenMax.Size = new System.Drawing.Size(85, 30);
            this.lblFullMin.Location = new System.Drawing.Point(174, 9);
            this.lblFullMin.Size = new System.Drawing.Size(85, 30);
            this.lblFullMax.Location = new System.Drawing.Point(87, 9);
            this.lblFullMax.Size = new System.Drawing.Size(85, 30);
            this.lblHandleWidthMin.Location = new System.Drawing.Point(709, 9);
            this.lblHandleWidthMin.Size = new System.Drawing.Size(85, 30);
            this.lblHandleWidthMax.Location = new System.Drawing.Point(622, 9);
            this.lblHandleWidthMax.Size = new System.Drawing.Size(85, 30);
        }


        //events
        public event EventHandler editButtonClick;
        public event EventHandler upButtonClick;
        public event EventHandler downButtonClick;
        public event EventHandler deleteButtonClick;
        public event EventHandler addButtonClick;


        private void btnAdd_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.addButtonClick;
            if (handler != null) handler(this, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.editButtonClick;
            if (handler != null) handler(this, e);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.upButtonClick;
            if (handler != null) handler(this, e);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.downButtonClick;
            if (handler != null) handler(this, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.deleteButtonClick;
            if (handler != null) handler(this, e);
        }
    }
}

