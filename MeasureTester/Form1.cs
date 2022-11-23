using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MillingCutterMeasurer;

namespace MeasureTester
{
    public partial class Form1 : Form
    {
        private millingCutterMeasurer measurer;
        private Bitmap bmpImage;

        private Rectangle calBaseBox1;
        private int calBaseBox2Distance;
		public TIS.Imaging.Device camera;
		public bool simCamera = true;

        public Form1()
        {
            InitializeComponent();
            measurer = null;
            bmpImage = null;
			this.camera = null;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "BMP file (*.bmp)|*.bmp";
     
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                 try
                 {
                    this.bmpImage = new Bitmap(ofd.FileName);
                    string errorMessage;
                    bool successful=this.doProcess(out errorMessage);
                    this.redraw();
                 }
                 catch
                {
                    MessageBox.Show(ofd.FileName + " is not valid image");
                    return;
                }
                

                this.picLoading.Image = new Bitmap(Application.StartupPath + "\\19.JPG.bmp");
            }
        }

        //success : true; 
        private bool doProcess(out string message)
        {
           
            this.measurer = millingCutterMeasurer.Create(this.bmpImage, out message, (int) numBorder.Value);
            if (this.measurer == null)
                return false;

            //initial calibration1 parameters
            this.calBaseBox1=new Rectangle(
                (int) numCalibBx1_x.Value,
                (int) numCalibBx1_y.Value,
                (int) numCalibBx1_w.Value, 
                (int) numCalibBx1_h.Value
            );
            this.calBaseBox2Distance = (int)numCalibBx2_y.Value;


            bool calibBaseSuccess;
            try
            {
                //calibBaseSuccess = this.measurer.calibrateBase(this.calBaseBox1, this.calBaseBox2Distance, (int)numCutterGap.Value, (int)numCutterBoxWidth.Value);
				calibBaseSuccess = this.measurer.calibrateBase(this.calBaseBox1, this.calBaseBox2Distance);

			}
			catch
            {
                message = "Failed on milling cutter calibration";
                return false;
            }

            bool calibCutterSuccess;
            try
            {
                calibCutterSuccess = this.measurer.calibrateCutter(this.calBaseBox1, this.calBaseBox2Distance,(int) numCutterGap.Value, (int) numCutterBoxWidth.Value);
            }
            catch
            {
                message = "Failed on milling cutter calibration";
                return false;
            }

            if (!calibCutterSuccess)
            {
                MessageBox.Show("空針");
                return false;
            }
            //do measurement
            bool measureSuccess;
            try
            {
                measureSuccess = this.measurer.doMeasure((int)numCutterGap.Value, (int)numCutterBoxWidth.Value, (int)numCutterHeight.Value, (int)numBladeShift.Value, (int)numBladeWidth.Value,0.2,5);
            }
            catch
            {
                message = "Failed on do measurement";
                return false;
            }

            return false;

        }

        private void redraw()
        {
            this.picOrignal.Image = measurer.getSourceImage();
            this.picEdge.Image = this.noteCalBaseBox1(measurer.getOrignalEdge());
            this.picCalibBlur.Image = this.measurer.getBaseCalibratedImage();
            this.picBaseCalibEdge.Image = noteCutterCalibrate(this.measurer.getBaseCalibratedEdge());
            this.picCutBlur.Image = this.measurer.getCutterCalibratedImage();
            this.picCutBlurEdge.Image = noteCutterMeasure(this.measurer.getCutterCalibratedEdge());
        }

        private Bitmap noteCalBaseBox1(Bitmap bmp)
        {
            if (calBaseBox1 == null)
                return null;
            Bitmap rtn = new Bitmap(bmp);
            Pen p = new Pen(Color.Red,3);
            Graphics g = Graphics.FromImage(rtn);
            g.DrawRectangle(p, this.calBaseBox1);
            Rectangle r2 = new Rectangle(this.calBaseBox1.X, this.calBaseBox1.Y + this.calBaseBox2Distance, this.calBaseBox1.Width, this.calBaseBox1.Height);
            g.DrawRectangle(p, r2);

            //draw center line
            Pen centP = new Pen(Color.Red, 3);
            g.DrawLine(centP, 0, bmp.Height / 2, bmp.Width - 1, bmp.Height / 2);


            //debug
            if (this.measurer != null)
            {

                this.drawCalib1CheckLine(g, this.measurer.cal1_leftUps);
                this.drawCalib1CheckLine(g, this.measurer.cal1_leftDowns);
                this.drawCalib1CheckLine(g, this.measurer.cal1_rightUps);
                this.drawCalib1CheckLine(g, this.measurer.cal1_rightDowns);

            }

            g.Dispose();
            return rtn;
        }

        private Bitmap noteCutterCalibrate(Bitmap bmp)
        {
            if (bmp == null) return null;

            Graphics g = Graphics.FromImage(bmp);

            Pen edgePen = new Pen(Color.Blue, 3);
            g.DrawLine(edgePen, (int) this.measurer.baseLeftEdgeX, 0, (int) this.measurer.baseLeftEdgeX, bmp.Height - 1);
            g.DrawLine(edgePen, (int) this.measurer.baseRightEdgeX, 0, (int)this.measurer.baseRightEdgeX, bmp.Height - 1);


            return bmp;
        }
        private void drawCalib1CheckLine(Graphics g, Point[] ptrArr)
        {
            if (ptrArr != null && ptrArr.Length > 1)
            {
                Pen edgeP = new Pen(Color.Green, 3);
                for (int i = 1; i < ptrArr.Length; ++i)
                    g.DrawLine(edgeP, ptrArr[i - 1], ptrArr[i]);
            }
        }

        private Bitmap noteCutterMeasure(Bitmap bmp)
        {
            if (bmp == null)
                return null;
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Green, 3);

            g.DrawLine(p, 0, bmp.Height / 2, bmp.Width - 1, bmp.Height / 2);
            g.DrawLine(p, this.measurer.measureLeftEdgeX, 0, this.measurer.measureLeftEdgeX, bmp.Height - 1);
            g.DrawLine(p, this.measurer.measureRightEdgeX, 0, this.measurer.measureRightEdgeX, bmp.Height - 1);

            Pen cp = new Pen(Color.Blue, 3);
            g.DrawLine(cp, 0, this.measurer.cutterCenter, bmp.Width - 1, this.measurer.cutterCenter);


            Pen ep = new Pen(Color.Red, 3);
            g.DrawLine(ep, (int) numBorder.Value, 0, (int) numBorder.Value, bmp.Height - 1);
            g.DrawLine(ep, bmp.Width- (int) numBorder.Value, 0, bmp.Width - (int) numBorder.Value, bmp.Height - 1);

            //tail
        
            if(!Double.IsNaN(this.measurer.handle_end) && !Double.IsInfinity(this.measurer.handle_end))
                g.DrawLine(ep, (int) this.measurer.handle_end, this.measurer.cutterCenter - (int)numCutterHeight.Value, (int) this.measurer.handle_end, this.measurer.cutterCenter + (int)numCutterHeight.Value);
            if (!Double.IsNaN(this.measurer.blade_end) && !Double.IsInfinity(this.measurer.blade_end))
                g.DrawLine(ep, (int)this.measurer.blade_end, this.measurer.cutterCenter - (int)numCutterHeight.Value, (int)this.measurer.blade_end, this.measurer.cutterCenter + (int)numCutterHeight.Value);
            if (!Double.IsNaN(this.measurer.handle_start))
                g.DrawLine(ep, (int)this.measurer.handle_start, this.measurer.cutterCenter - (int)numCutterHeight.Value, (int)this.measurer.handle_start, this.measurer.cutterCenter + (int)numCutterHeight.Value);


            //blade
            if(!double.IsNaN(this.measurer.blade_inner_upper))
                g.DrawLine(ep, this.measurer.blade_width_check_start, (int)this.measurer.blade_inner_upper, this.measurer.blade_width_check_end, (int)this.measurer.blade_inner_upper);
            if (!double.IsNaN(this.measurer.blade_outer_upper))
                g.DrawLine(ep, this.measurer.blade_width_check_start, (int)this.measurer.blade_outer_upper, this.measurer.blade_width_check_end, (int)this.measurer.blade_outer_upper);

            if (!double.IsNaN(this.measurer.blade_inner_lower))
                g.DrawLine(ep, this.measurer.blade_width_check_start, (int)this.measurer.blade_inner_lower, this.measurer.blade_width_check_end, (int)this.measurer.blade_inner_lower);
            if (!double.IsNaN(this.measurer.blade_outer_lower))
                g.DrawLine(ep, this.measurer.blade_width_check_start, (int)this.measurer.blade_outer_lower, this.measurer.blade_width_check_end, (int)this.measurer.blade_outer_lower);

            return bmp;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Exit?", "MeasureDev", MessageBoxButtons.YesNo) != DialogResult.Yes)
                Application.Exit();
        }

        private void picEdge_Click(object sender, EventArgs e)
        {

        }

        private void picOrignal_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "Ori");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }
                

            }
        }

        private void savePictureBoxImage(Bitmap bmp, string pid)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Png Image|*.png";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.InitialDirectory = Application.StartupPath;

            //fileName
            string fName = string.Format("{0}_{1}.png", Guid.NewGuid(),pid);
            saveFileDialog1.FileName = fName;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
                bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void picEdge_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "edge");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }


            }
        }

        private void picEdge2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "Algn");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }


            }
        }

        private void picEdge3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "final");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }


            }
        }

        private void numCalibBx1_y_ValueChanged(object sender, EventArgs e)
        {
            string message;
            this.doProcess(out message);
        }

        private void reDrawCalibBx1()
        {
            this.redraw();
        }

        private void numCalibBx2_w_ValueChanged(object sender, EventArgs e)
        {
            reDrawCalibBx1();
        }

        private void numCalibBx2_h_ValueChanged(object sender, EventArgs e)
        {
            reDrawCalibBx1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message;
            this.doProcess(out message);
            this.reDrawCalibBx1();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

		private void Form1_Load(object sender, EventArgs e)
		{
			/**/
			icImg1.ShowDeviceSettingsDialog();

			if (!icImg1.DeviceValid)
			{
				if (MessageBox.Show("Run camera simulation mode?", "Camera no found", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					this.simCamera = true;
				}
				else
				{
					Close();
					return;
				}
			}
			else
				icControlInitial();
			/**/
		}

		private void icControlInitial()
		{
			icImg1.LiveCaptureContinuous = true;
			icImg1.MemoryCurrentGrabberColorformat = TIS.Imaging.ICImagingControlColorformats.ICRGB24;

			//設定顯示視窗尺吋
			icImg1.LiveDisplayDefault = false;
			icImg1.LiveDisplayHeight = icImg1.Height;
			icImg1.LiveDisplayWidth = icImg1.Width;

			TIS.Imaging.FrameHandlerSink fhs = new TIS.Imaging.FrameHandlerSink();
			fhs.SnapMode = true;
			icImg1.Sink = fhs;

			icImg1.LiveStart();
		}

		private void icImg1_Click(object sender, EventArgs e)
		{
			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(this.icImg1.DeviceValid)
			{
				icImg1.MemorySnapImage();
				this.bmpImage = icImg1.ImageActiveBuffer.Bitmap;
				try
				{

					string errorMessage;
					bool successful = this.doProcess(out errorMessage);
					this.redraw();
				}
				catch
				{
					MessageBox.Show("Capture failed");
					return;
				}
			}
			else if(simCamera)
			{
				//this.picLoading.Image = new Bitmap(Application.StartupPath + "\\19.JPG.bmp");
			}
		}

		private void cameraSettingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(initialCamera())
			{
				MessageBox.Show("doAdj");
			}
		}

		private bool initialCamera()
		{
			if(this.icImg1.DeviceValid)
				this.icImg1.LiveStop();

			if (this.camera != null)
				this.camera.Dispose();


			TIS.Imaging.Device[] cameras;
			try { 
				cameras = icImg1.Devices;
			}
			catch
			{
				
				return false;
			}

			if (cameras == null)
			{
				
				return false;
			}

			if (cameras.Length < 1)
			{
				
				return false;
			}

			icImg1.ShowDeviceSettingsDialog();
			this.camera = null;
			if(icImg1.Device==null || icImg1.Device=="")
			{
				this.camera = null;
			}
			else
			{
				foreach (TIS.Imaging.Device c in cameras)
					if (c.Name == icImg1.Device)
						this.camera = c;
				this.icImg1.LiveStart();

			}

			return this.camera != null;
		}
	}
}
