using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MillingCutterPtp
{
    public partial class settingPage : Form
    {
        //clsses
        private const int CLASS_INIT_X = 12;
        private const int CLASS_INIT_Y = 108;
        private const int CLASS_X_INCRESE = 927;
        private const int CLASS_Y_INCRESE = 54;
        private const int CLASS_ROW_MAX = 8;
        private const int CLASS_MAX = 15;

        //boxes
        private const int BOX_INIT_X = 13;
        private const int BOX_INIT_Y = 617;
        private const int BOX_X_INCRESE = 275;
        private const int BOX_Y_INCRESE = 90;
        private const int BOX_ROW_MAX = 3;
        private const int BOX_MAX = 16;

        public classManager classMgr;
        //public classManager sourceClassMgr;

        public measureParameters measureParas;
        public Bitmap calibImage;
        public bool simCamera;

		public bool newClassApplied = false;
		public bool newCalibrateApplied = false;
		public TIS.Imaging.Device Camera;
		public PLCController plc;


		private List<ClassRecord> classRecords;
        private boxRecord[] boxRecords;

        public settingPage()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            //create class controls
            classRecords = new List<ClassRecord>();
            for (ushort i = 0; i < CLASS_MAX; ++i)
            {
                ClassRecord tCR = new ClassRecord(i);

                int posX, posY;
                if (i < CLASS_ROW_MAX)
                {
                    posX = CLASS_INIT_X;
                    posY = CLASS_INIT_Y + (i * CLASS_Y_INCRESE);
                }
                else
                {
                    posX = CLASS_INIT_X + CLASS_X_INCRESE;
                    posY = CLASS_INIT_Y + ((i - CLASS_ROW_MAX) * CLASS_Y_INCRESE);
                }

                //tCR.Show();
                this.panelClasses.Controls.Add(tCR);
                this.classRecords.Add(tCR);
                tCR.Location = new Point(posX, posY);
                tCR.Visible = false;
                tCR.addButtonClick += buttonClassAdd_Click;
                tCR.editButtonClick += buttonClassEdit_Click;
                tCR.upButtonClick += buttonClassUp_Click;
                tCR.downButtonClick += buttonClassDown_Click;
                tCR.deleteButtonClick += buttonClassDelete_Click;
            }

            //create box controls
            this.boxRecords = new boxRecord[BOX_MAX];
            for (int i = 0; i < BOX_MAX; ++i)
            {
                this.boxRecords[i] = new boxRecord();
                this.panelClasses.Controls.Add(this.boxRecords[i]);

                int subi, step;

                subi = i % BOX_ROW_MAX;
                step = i / BOX_ROW_MAX;

                this.boxRecords[i].ID = (uint)i;
                this.boxRecords[i].Location = new Point(BOX_INIT_X + (step * BOX_X_INCRESE), BOX_INIT_Y + (subi * BOX_Y_INCRESE));
                this.boxRecords[i].Visible = true;

				this.boxRecords[i].editClick += this.boxEdit_Click;
            }

			this.lblNewFactor.Text = "";
			this.lblNewIntercept.Text = "";
        }

        private void settingPage_Load(object sender, EventArgs e)
		{ 

            //更新分類
            this.refreshBoxes();
            this.refreshClasses();

            for (ushort i = 0; i < CLASS_MAX; ++i)
            {
                ClassRecord tCR = new ClassRecord(i);

                tCR.addButtonClick += buttonClassAdd_Click;
                tCR.editButtonClick += buttonClassEdit_Click;
                tCR.upButtonClick += buttonClassUp_Click;
                tCR.downButtonClick += buttonClassDown_Click;
                tCR.deleteButtonClick += buttonClassDelete_Click;
            }

            //calibraton
            this.lblCalibFactor.Text = measureParas.calibrationFactor.ToString("0.000000");
			this.lblCalibIntercept.Text = measureParas.interception.ToString("0.00");
			this.icControlInitial();
        }

		private void icControlInitial()
		{
			icImage.LiveCaptureContinuous = true;
			icImage.MemoryCurrentGrabberColorformat = TIS.Imaging.ICImagingControlColorformats.ICRGB24;

			//設定顯示視窗尺吋
			icImage.LiveDisplayDefault = false;
			icImage.LiveDisplayHeight = icImage.Height;
			icImage.LiveDisplayWidth = icImage.Width;

			TIS.Imaging.FrameHandlerSink fhs = new TIS.Imaging.FrameHandlerSink();
			fhs.SnapMode = true;
			icImage.Sink = fhs;
			if (this.Camera != null)
			{
				this.icImage.Device = this.Camera;
				icImage.LiveStart();
			}
		}

		private void refreshClasses()
        {
            millingCutterClass[] mccs = this.classMgr.classes;

            for (int i = 0; i < this.classRecords.Count; ++i)
            {
                this.classRecords[i].index = (uint)i+1 ;
                if (i < mccs.Length)
                {
                    this.classRecords[i].data = mccs[i];
                    this.classRecords[i].Visible = true;

                    int[] bis = this.classMgr.boxIndexOfClass(mccs[i].guid);
					for (int j = 0; j < bis.Length; ++j)
					{
						this.boxRecords[bis[j]].Class = (uint)i + 1;
						this.boxRecords[bis[j]].doPaint();
					}
                }
                else
                {
                    this.classRecords[i].data = null;
                    this.classRecords[i].Visible = false;
                }
                this.classRecords[i].refresh();

                if (mccs.Count() < CLASS_MAX)
                    this.classRecords[mccs.Count()].Visible = true;
            }

            //if (this.rdoInnerBlade.Checked != this.classMgr.bladeUseInner)
            if (this.classMgr.bladeUseInner)
            {
                this.rdoInnerBlade.Checked = true;
            }
            else
                this.rdoOutterBlade.Checked = true;

            if(this.classMgr.bladeUseInner)
            {
                lblTitleBladeUp1.Text = "芯厚上限";
                lblTitleBladeUp2.Text = "芯厚上限";
                lblTitleBladeLower1.Text = "芯厚下限";
                lblTitleBladeLower2.Text = "芯厚下限";
            }
            else
            {
                lblTitleBladeUp1.Text = "外徑上限";
                lblTitleBladeUp2.Text = "外徑上限";
                lblTitleBladeLower1.Text = "外徑下限";
                lblTitleBladeLower2.Text = "外徑下限";
            }
        }

        private void refreshBoxes()
        {
            classBox[] boxes = this.classMgr.getBoxes();
            for (int i = 0; i < boxes.Length; ++i)
            {
                this.boxRecords[i].quantity = boxes[i].Quantity;
				this.boxRecords[i].Class = null;
				this.boxRecords[i].Refresh();
            }
        }

        // buttons
        // 新增
        private void buttonClassAdd_Click(object sender, EventArgs e)
        {
            ClassRecord cr = (ClassRecord)sender;
            int idx = 0;
            for (int i = 0; i < this.classRecords.Count; ++i)
            {
                if (this.classRecords[i] == cr)
                    idx = i + 1;
            }

            classDialog cDlg = new classDialog(null,idx,this.rdoInnerBlade.Checked);

            cDlg.ShowDialog();
            if(cDlg.DialogResult==DialogResult.OK)
            {
                this.classMgr.Add(cDlg.rtn);
                this.refreshBoxes();
                this.refreshClasses();
            }

        }
        //編輯
        private void buttonClassEdit_Click(object sender, EventArgs e)
        {
            ClassRecord cr = (ClassRecord) sender;
            int idx = 0;
            for(int i=0; i<this.classRecords.Count; ++i)
            {
                if (this.classRecords[i] == cr)
                    idx = i + 1;
            }


            classDialog cDlg = new classDialog(cr.data,idx, this.rdoInnerBlade.Checked);

            cDlg.ShowDialog();
            if (cDlg.DialogResult == DialogResult.OK)
            {
                millingCutterClass mcc = this.classMgr.classfierOf(cr.data.guid);
                mcc.maxLength = cDlg.rtn.maxLength;
                mcc.minLength = cDlg.rtn.minLength;
                mcc.maxHandleLength = cDlg.rtn.maxHandleLength;
                mcc.minHandleLength = cDlg.rtn.minHandleLength;
                mcc.maxBladeWidth = cDlg.rtn.maxBladeWidth;
                mcc.minBladeWidth = cDlg.rtn.minBladeWidth;
                mcc.maxHandleWidth = cDlg.rtn.maxHandleWidth;
                mcc.minHandleWidth = cDlg.rtn.minHandleWidth;

                this.refreshBoxes();
                this.refreshClasses();
            }
        }

        //moveUp
        private void buttonClassUp_Click(object sender, EventArgs e)
        {
            ClassRecord cr = (ClassRecord)sender;
            int index = this.classMgr.classfierIndexOf(cr.data.guid);
            if (index < 0)
                throw new ArgumentOutOfRangeException("Class not found");
            this.classMgr.moveForward((uint) index);

            this.refreshBoxes();
            this.refreshClasses();
        }
        //moveUp
        private void buttonClassDown_Click(object sender, EventArgs e)
        {
            ClassRecord cr = (ClassRecord)sender;
            int index = this.classMgr.classfierIndexOf(cr.data.guid);
            if (index < 0)
                throw new ArgumentOutOfRangeException("Class not found");
            this.classMgr.moveBackward((uint)index);

            this.refreshBoxes();
            this.refreshClasses();
        }
        //deleteClass
        private void buttonClassDelete_Click(object sender, EventArgs e)
        {
            ClassRecord cr = (ClassRecord)sender;
			int index = this.classMgr.classfierIndexOf(cr.data.guid);
			if (index < 0)
				throw new ArgumentOutOfRangeException("Class not found");

			int[] boxss = this.classMgr.boxIndexOfClass(cr.data.guid);
            if(boxss.Length>0)
            {
                string message = "集裝盒 "+(boxss[0]+1).ToString();
                for (int i = 1; i < boxss.Length; ++i)
                    message += ", " + (boxss[i]+1).ToString();
                message += "使用本類別,變更集裝盒分類設定為其他分類後才可刪除本類別";
                MessageBox.Show(message);
                return;
            }
			if (MessageBox.Show("確認刪除？", "刪除類別", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;

			this.classMgr.remove((uint) index);

            this.refreshBoxes();
            this.refreshClasses();
        }

		//EditClass
		private void boxEdit_Click(object sender, EventArgs e)
		{
			boxRecord br = (boxRecord)sender;
			int index = Array.IndexOf(this.boxRecords, br);

			boxDialog bDlg = new boxDialog(this.classMgr.getBoxes()[index], this.classMgr.classes,(uint) index);
			bDlg.anotherBox = index >= boxRecords.Length - 1;
			bDlg.ShowDialog();

			if(bDlg.DialogResult==DialogResult.OK)
			{
				this.classMgr.setBox(index, bDlg.selectedClass,(uint) bDlg.Quantity);
				this.refreshBoxes();
				this.refreshClasses();
			}
			br.doPaint();

		}

		private void btnReturn_Click(object sender, EventArgs e)
        {
			if (MessageBox.Show("請確認所有變更都已存檔或執行！", "離開設定頁", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;

			
			this.icImage.LiveStop();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			Bitmap bmp;
			if (simCamera)
			{
				bmp = this.calibImage;
				this.picCalibration.Image = bmp;
			}
			else
			{
				this.icImage.MemorySnapImage();
				bmp = icImage.ImageActiveBuffer.Bitmap;
			}

			string message;
			MillingCutterMeasurer.millingCutterMeasurer m = MillingCutterMeasurer.millingCutterMeasurer.Create(bmp, out message, this.measureParas.seekBorder, this.measureParas.interception, this.measureParas.calibrationFactor);
			//if (!calibrationFactor(m))
			//	return;

			bool measureSuccess = true;
			try
			{
				measureSuccess = doMeasure(m, out message, this.measureParas);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}

			if (!measureSuccess)
			{
				MessageBox.Show(message);
				return;
			}

			

			double stdLength, stdWidth;

			if (txtStdLength.Text.Length < 1 || txtStdWidth.Text.Length < 1)
			{
				MessageBox.Show("未設置標準品尺寸");
				return;
			}

			if(!validDoubleInput(txtStdLength.Text,out stdLength,50,1) || !validDoubleInput(txtStdWidth.Text, out stdWidth, 50, 1))
			{
				MessageBox.Show("標準品尺吋無效");
				return;
			}

			double newFactor, newIntercept;
			this.linearCalib(stdLength,stdWidth,m.full_Length,m.handle_Width,out newFactor, out newIntercept);

			m.calibrationFactor = newFactor;
			m.calibrationInterception = newIntercept;

			this.lblNewFactor.Text = newFactor.ToString("0.000000");
			this.lblNewIntercept.Text = newIntercept.ToString("0.00");

			double newLength = m.getCalibrateLength("full_length");
			double newWidth = m.getCalibrateLength("handle_width");

			this.lblFulllength.Text = m.full_Length.ToString("0.00");
			this.lblFullLengthmm.Text = newLength.ToString("0.00");

			this.lblhandleWidth.Text = m.handle_Width.ToString("0.00");
			this.lblHandleWidthmm.Text = newWidth.ToString("0.00");

			this.lblFulllengthError.Text = (m.getCalibrateLength("full_length") / newLength).ToString("0.00");
			this.lblHandleWidthError.Text=(m.getCalibrateLength("handle_width")/newWidth).ToString("0.00");

			Application.DoEvents();

			if (MessageBox.Show("確認寫入校正值,寫入後無法還回,只能重新再校正", "校正", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				this.measureParas.calibrationFactor = newFactor;
				this.measureParas.interception = newIntercept;
				this.newCalibrateApplied = true;
			}
        }

		private void linearCalib(double stdLen, double stdWidth, double newLenPx, double newWidthPx, out double factor, out double intercept)
		{
			double dY = stdLen - stdWidth;
			double dX = newLenPx - newWidthPx;
			factor = dY / dX;

			intercept = stdLen - (factor * newLenPx);

		}

        private void button38_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "載入設定檔";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				classManager cm;

				try
				{
					cm = classManager.fromFile(ofd.FileName);

					if(cm==null)
					{
						MessageBox.Show("開啟分類檔 "+ ofd.FileName +" 失敗");
					}
					else
					{
						this.classMgr = cm;
						this.refreshBoxes();
						this.refreshClasses();
                        this.lblFile.Text = Path.GetFileName(ofd.FileName);
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show("開啟分類檔 " + ofd.FileName + " 失敗 ( "+ex.Message+" )");
				}
			}
        }

        private void button39_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "儲存設定檔";
            if(sfd.ShowDialog()==DialogResult.OK)
			{
				if(!this.classMgr.isRecordsValid())
				{
					MessageBox.Show("分類檔內容有誤");
					return;
				}

				try
				{
					this.classMgr.toFile(sfd.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show("存檔失敗！ ：" + ex.Message);
				}
			}
        }

        private void button42_Click(object sender, EventArgs e)
        {
            MessageBox.Show("確定清除分類設定?", "清除分類", MessageBoxButtons.YesNo);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (simCamera)
            {
                bmp = this.calibImage;
                this.picCalibration.Image = bmp;
            }
            else
            {
				this.icImage.MemorySnapImage();
				bmp = icImage.ImageActiveBuffer.Bitmap;
			}
            string message;
            MillingCutterMeasurer.millingCutterMeasurer m = MillingCutterMeasurer.millingCutterMeasurer.Create(bmp, out message, this.measureParas.seekBorder, this.measureParas.interception, this.measureParas.calibrationFactor);

            bool measureSuccess = true;
            try
            {
                measureSuccess = doMeasure(m, out message, this.measureParas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (measureSuccess)
            {
                this.lblFulllength.Text = m.full_Length.ToString("0.00");
                this.lblFullLengthmm.Text = m.getCalibrateLength("full_length").ToString("0.00");

                this.lblhandleWidth.Text = m.handle_Width.ToString("0.00");
                this.lblHandleWidthmm.Text = m.getCalibrateLength("handle_width").ToString("0.00");
            }
            else
            {
                MessageBox.Show(message);
            }

        }

        private bool doMeasure(MillingCutterMeasurer.millingCutterMeasurer measurer, out string message, measureParameters paras)
        {

            bool calibBaseSuccess;
            try
            {
                calibBaseSuccess = measurer.calibrateBase(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY);
            }
            catch (Exception ex)
            {
                message = "Failed on milling cutter calibration";
                return false;
            }

            bool calibCutterSuccess;
            try
            {
                calibCutterSuccess = measurer.calibrateCutter(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY, paras.baseGapX, paras.baseWidth);
            }
            catch
            {
                message = "Failed on milling cutter calibration";
                return false;
            }

            if (!calibCutterSuccess)
            {
                message = "空針";
                return false;
            }
            //do measurement
            bool measureSuccess;
            try
            {
                measureSuccess = measurer.doMeasure(paras.baseGapX, paras.baseWidth, paras.searchHeight, paras.bladeShift, paras.bladeCheckWidth, 0.1, 5);
            }
            catch
            {
                message = "Failed on do measurement";
                return false;
            }

            message = "";
            return true;

        }

		private void btnSetAllQty_Click(object sender, EventArgs e)
		{
			if(MessageBox.Show("全部設置","設定數量",MessageBoxButtons.YesNo)==DialogResult.Yes)
			{
				classBox[] cbs = this.classMgr.getBoxes();
				for (int i = 0; i < cbs.Length; ++i)
					this.classMgr.setBoxQuantity(i, (uint)this.numAllQty.Value);

				this.refreshBoxes();
				this.refreshClasses();
			}
			

			
		}

		private void btnWrite_Click(object sender, EventArgs e)
		{

            /*/For Debug
            {
                MessageBox.Show("Debug Mode");
                boxQuantityWrited(null, new EventArgs());
                return;
            } /**/

        
            if(!this.plc.isConnected)
			{
				MessageBox.Show("PLC 未連線");
				return;
			}

			classBox[] classBxs = this.classMgr.getBoxes();

			uint[] classes = new uint[classBxs.Length];
			for (int i = 0; i < classBxs.Length; ++i)
			{
				if(classBxs[i].Class==Guid.Empty)
				{
					classes[i] = classManager.ANOTHERCLASS;
				}
				else
				{
					classes[i] = (uint) this.classMgr.classfierIndexOf(classBxs[i].Class) + 1;
				}
			}
			classes[classBxs.Length - 1] = classManager.ANOTHERCLASS;

			this.plc.onBoxClassesWrited_Handler += this.writeBoxQuantity;
			this.plc.writeBoxClasses(classes);
		}

		private void writeBoxQuantity(object sender, EventArgs e)
		{
			this.plc.onBoxClassesWrited_Handler -= this.writeBoxQuantity;
			//do write
			classBox[] classBxs = this.classMgr.getBoxes();

			uint[] qtys = new uint[classBxs.Length];
			for (int i = 0; i < classBxs.Length; ++i)
				qtys[i] = classBxs[i].Quantity;

			this.plc.onBoxQuantityWrited_Handler += this.boxQuantityWrited;
			this.plc.writeBoxQuantities(qtys);

		}

		private void boxQuantityWrited(object sender, EventArgs e)
		{
			this.plc.onBoxClassesWrited_Handler -= boxQuantityWrited;

			this.newClassApplied = true;

			MessageBox.Show("集裝盒分配與集裝盒數量寫入成功");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.icImage.LiveStop();
			icImage.ShowDeviceSettingsDialog();
			this.icImage.LiveStart();
		}

		private void txtStdLength_Validating(object sender, CancelEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			double v;
			if(tbx.Text.Length>0)
			{
				if (!validDoubleInput(tbx.Text, out v, 50, 1))
					e.Cancel = true;
			}
			
		}

		private bool validDoubleInput(string str, out double value, double max=100, double min=1)
		{
			if (!double.TryParse(str, out value))
				return false;

			if (value > max || value < min)
				return false;

			return true;
		}

		private void txtStdWidth_TextChanged(object sender, EventArgs e)
		{
			
		}

		private void txtStdWidth_Validating(object sender, CancelEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			double v;
			if (tbx.Text.Length > 0)
			{
				if (!validDoubleInput(tbx.Text, out v, 50, 1))
					e.Cancel = true;
			}
		}

		private void lblNewFactor_Click(object sender, EventArgs e)
		{

		}

		private void pageMeasure_Paint(object sender, PaintEventArgs e)
		{

		}

        private void rdoInnerBlade_CheckedChanged(object sender, EventArgs e)
        {
            this.classMgr.bladeUseInner = rdoInnerBlade.Checked;
            this.refreshClasses();
        }

        private void rdoOutterBlade_CheckedChanged(object sender, EventArgs e)
        {
            //this.classMgr.bladeUseInner = false;
            //refreshClasses();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
