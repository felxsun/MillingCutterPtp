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
using TIS.Imaging;
using TIS.Imaging.VCDHelpers;
using System.IO;

namespace MeasureTester
{
    public partial class frmEngineer : Form
    {
        private millingCutterMeasurer measurer;
        private Bitmap bmpImage;

        private Rectangle calBaseBox1;
        private int calBaseBox2Distance;

		public bool simCamera = true;

        public bool standalone = false;
		public string cameraSetting = "";
        public TIS.Imaging.Device camera = null;

		public string prePath;
        public string forceMeasure = "";

        public frmEngineer()
        {
            InitializeComponent();
            measurer = null;
            bmpImage = null;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (this.prePath == "" || !Directory.Exists(this.prePath))
				this.prePath = Application.StartupPath;
            OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = this.prePath;
            ofd.Filter = "BMP file (*.bmp)|*.bmp|PNG file (*.png)|*.png";
     
            if(ofd.ShowDialog() == DialogResult.OK)
            {
				this.prePath = Path.GetDirectoryName(ofd.FileName);
                TimeSpan span;
                string errorMessage;

                if( this.doLoadPicture(ofd.FileName, out errorMessage, out span))
                {
                    this.lblTimeSpan.Text = span.TotalMilliseconds.ToString();
                    this.redraw();
                    this.picLoading.Image = new Bitmap(Application.StartupPath + "\\19.JPG.bmp");
                }
                else 
                {
                    this.lblTimeSpan.Text="";
                    MessageBox.Show(errorMessage);
                }
            }
        }

        private bool doLoadPicture(string fileName, out string message, out TimeSpan span) {
            message="";
            span = TimeSpan.MinValue;
            bool successful=false;

            DateTime start = DateTime.Now;
            try
            {
                this.bmpImage = new Bitmap(fileName);
                successful = this.doProcess(out message);
                span = DateTime.Now - start;
                
            }
            catch (Exception ex)
            {
                message=fileName + " is not valid image";
                return false;
            }

            return true;
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


            //base calibration
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

            if (!calibBaseSuccess)
            {
                MessageBox.Show("Base Calibrate failed");
                return false;
            }

            //cutter calibration
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

			double handleCutoff;
			if(!double.TryParse(this.txtHandleCutoff.Text,out handleCutoff))
			{
				message="Handle cut off value is invalid  \"" + this.txtHandleCutoff.Text + "\"";
				return false;
			}


            //do measurement
            bool measureSuccess;
            try
            {
                measureSuccess = this.measurer.doMeasure((int)numCutterGap.Value, (int)numCutterBoxWidth.Value, (int)numCutterHeight.Value, (int)numBladeShift.Value, (int)numBladeWidth.Value,handleCutoff,(int)numFlank.Value);
            }
            catch
            {
                message = "Failed on do measurement";
                return false;
            }

			return true; ;
        }

        private void redraw()
        {
            this.picOrignal.Image = measurer.getSourceImage();
            this.picEdge.Image = this.noteCalBaseBox1(measurer.getOrignalEdge());
            this.picCalibBlur.Image = this.measurer.getBaseCalibratedImage();
            this.picBaseCalibEdge.Image = noteCutterCalibrate(this.measurer.getBaseCalibratedEdge());
            this.picCutBlur.Image = this.measurer.getCutterCalibratedImage();
            this.picCutBlurEdge.Image = noteCutterMeasure(this.measurer.getCutterCalibratedEdge());
            this.picHandle.Image = this.measurer.getHandleImage();
			try
			{
				this.picHandleEdge.Image = this.noteHandleWidth(this.measurer.getHandleEdge(), this.measurer.handleEdgeHeights, this.measurer.handleEdgeSmoothHeights, this.measurer.handleEdgeSloops);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			//this.picHandleEdge.Image = this.measurer.getHandleEdge();

			this.lblLength.Text = this.measurer.full_Length.ToString();
			this.lblHandleLength.Text = this.measurer.handle_Length.ToString();
			this.lblHandleWidth.Text = this.measurer.handle_Width.ToString();
			this.lblBladeWidth.Text = this.measurer.blade_inner_width.ToString();
        }

        private Bitmap noteHandleWidth(Bitmap bmp,int[] widthArr, double[] smoothWidth, double[] sloops=null)
        {
            Bitmap rtn = new Bitmap(bmp.Width, bmp.Height);
            for (int x = 0; x < bmp.Width; ++x)
                for (int y = 0; y < bmp.Height; ++y)
                {
                    Color c = bmp.GetPixel(x, y);
                    rtn.SetPixel(x, y, Color.FromArgb(c.R,c.R,c.R));
                }

			int center = bmp.Height / 2;
			double minSloop = double.MaxValue;
			int maxPos = 0;
			for (int i = 0; i < bmp.Width; ++i)
			{
				if(widthArr[i]>0)
					rtn.SetPixel(i, bmp.Height - widthArr[i], Color.Yellow);

				if(smoothWidth[i]>0)
					rtn.SetPixel(i, bmp.Height - (int)smoothWidth[i]+10,Color.Green);

				
				if (sloops != null)
				{
					//double tStage = (double.IsNaN(sloops[i]) || sloops[i]>0) ? 0 : sloops[i] * center * 10;
					double tStage = (double.IsNaN(sloops[i])) ? 0 : sloops[i] * center * 10;
					if (sloops[i] < minSloop)
					{
						minSloop = sloops[i];
						maxPos = i;
					}

					int tempY = center + (int)tStage;

					if( i>0 && i< rtn.Width && tempY >0 && tempY < rtn.Height)
						rtn.SetPixel(i, tempY , Color.Red);
				}
					
			}

			//note max score
			Graphics g = Graphics.FromImage(rtn);
			Pen p = new Pen(Color.White);
			g.DrawLine(p, maxPos, 0, maxPos, bmp.Height);
			g.DrawString(minSloop.ToString("0.00000"), new Font("Times New Roman", 12), new SolidBrush(Color.White), 50, 50);

            return rtn;
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
			if (MessageBox.Show("Exit?", "MeasureDev", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (icImg1.DeviceValid)
					icImg1.LiveStop();
			}
			else e.Cancel = true;
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

       private void Form1_Load(object sender, EventArgs e)
		{
            if(this.camera==null)
            {
                /**/
                icImg1.ShowDeviceSettingsDialog();

                if (!icImg1.DeviceValid)
                {
                    if (MessageBox.Show("Run camera simulation mode?", "Camera no found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        this.simCamera = true;
                        btnMeasure.Enabled = false;
                        mnuSetting.Enabled = false;
                    }
                    else
                    {
                        Close();
                        return;
                    }
                }
                else
                {
                    icControlInitial();
                    btnMeasure.Enabled = true;
                    mnuSetting.Enabled = true;
                }
            }
            else
            {
				icImg1.Device=this.camera.Name;
				icControlInitial();
				btnMeasure.Enabled = true;
                mnuSetting.Enabled = true;
            }

            this.btnSaveSetup.Enabled = true;
           if(this.forceMeasure!="")
           {
               string errorMessage;
               TimeSpan span = TimeSpan.MinValue;

               if (this.doLoadPicture(this.forceMeasure, out errorMessage, out span))
               {
                   this.lblTimeSpan.Text = span.TotalMilliseconds.ToString();
                   this.redraw();
                   this.picLoading.Image = new Bitmap(Application.StartupPath + "\\19.JPG.bmp");
               }
               else
               {
                   this.lblTimeSpan.Text = "";
                   MessageBox.Show(errorMessage);
               }
           }
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

		private void button2_Click(object sender, EventArgs e)
		{
			if(this.icImg1.DeviceValid)
			{

                this.lblBladeWidth.Text = "-";
                this.lblHandleLength.Text = "-";
                this.lblHandleWidth.Text = "-";
                this.lblLength.Text = "-";
                this.lblTimeSpan.Text = "-";

                DateTime startTime = DateTime.Now;

				icImg1.MemorySnapImage();
				this.bmpImage = icImg1.ImageActiveBuffer.Bitmap;
				try
				{

					string errorMessage;
					bool successful = this.doProcess(out errorMessage);
                    TimeSpan span = DateTime.Now - startTime;

					this.redraw();
                    this.lblTimeSpan.Text = span.TotalMilliseconds.ToString();
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
			initialCamera();
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

        private void btnSaveSetup_Click(object sender, EventArgs e)
        {
            if (!icImg1.DeviceValid)
			{
				MessageBox.Show("無法儲存相機參數");
				return;
			}
			try
			{
				string properties = icImg1.VCDPropertyItems.Save();
				File.WriteAllText(this.cameraSetting, properties);


				MessageBox.Show("參數儲存成功");
			}
			catch (Exception ex)
			{
				MessageBox.Show("參數儲存失敗 ： " + ex.Message);
			}
				
			icImg1.LiveStart();
        }

		private void btnLoad_Click(object sender, EventArgs e)
		{
			if(icImg1.DeviceValid)
			{
				string properties = File.ReadAllText(Application.StartupPath + "\\camera.prop");
				icImg1.VCDPropertyItems.Load(properties);
			}
			
		}

		private void picCalibBlur_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				PictureBox pbx = (PictureBox)sender;
				if (pbx.Image == null)
					return;
				try
				{
					this.savePictureBoxImage((Bitmap)pbx.Image, "baseCalib");
				}
				catch
				{
					MessageBox.Show("不支援的影像格式");
				}


			}
		}

		private void tabPages_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				PictureBox pbx = (PictureBox)sender;
				if (pbx.Image == null)
					return;
				try
				{
					this.savePictureBoxImage((Bitmap)pbx.Image, "baseEdge");
				}
				catch
				{
					MessageBox.Show("不支援的影像格式");
				}


			}
		}

		private void picCutBlurEdge_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				PictureBox pbx = (PictureBox)sender;
				if (pbx.Image == null)
					return;
				try
				{
					this.savePictureBoxImage((Bitmap)pbx.Image, "finalEdge");
				}
				catch
				{
					MessageBox.Show("不支援的影像格式");
				}


			}
		}

		private void txtHandleCutoff_Validating(object sender, CancelEventArgs e)
		{
			TextBox bx = (TextBox)sender;
			double v;
			if (!double.TryParse(bx.Text, out v))
				e.Cancel = true;
		}

		private void pictureBox_DoubleClick(object sender, EventArgs e)
		{
			PictureBox bx = (PictureBox)sender;
			bx.SizeMode = (bx.SizeMode == PictureBoxSizeMode.Zoom) ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom;
		}

        private void picHandle_Click(object sender, EventArgs e)
        {
            
        }

        private void picHandle_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "Handle");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }


            }
        }

        private void picHandleEdge_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pbx = (PictureBox)sender;
                if (pbx.Image == null)
                    return;
                try
                {
                    this.savePictureBoxImage((Bitmap)pbx.Image, "HandleEdge");
                }
                catch
                {
                    MessageBox.Show("不支援的影像格式");
                }


            }
        }
    }
}
