using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini;
using System.IO.Ports;
using System.Timers;
using System.IO;
using TIS.Imaging;
using TIS.Imaging.VCDHelpers;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace MillingCutterPtp
{
    public partial class frmMain : Form
    {
        //constant
        /// <summary>
        /// 影像擷取成功值
        /// </summary>
        public const int IMAGE_CAPTURED= 999;
        /// <summary>
        /// 影像擷取重試次數
        /// </summary>
        public const int IMAGE_CAPTURE_LIMIT= 10;

        //version control
        /// <summary>
        /// Assembly Version
        /// </summary>
        public string assembly
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        /// <summary>
        /// Product version
        /// </summary>
        public string production
        {
            get
            {
                return System.Windows.Forms.Application.ProductVersion;
            }
        }
        /// <summary>
        /// File Version
        /// </summary>
        public string fileVersion
        {
            get
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            }
        }

        /// <summary>
        /// 轉譯字串
        /// </summary>
        private msgString strlib;

        //operation status
        /// <summary>
        /// 執行/暫停服務
        /// </summary>
        private bool inOperation=true;
        /// <summary>
        /// 強制程式自動終止
        /// </summary>
        private bool forceExit = false;
        //private bool showMeasure = false;
        //private bool showTime = false;

        //for Simulation
        /// <summary>
        /// 類別計數器
        /// </summary>
        private int number=0;
        /// <summary>
        /// 模擬相機存在
        /// </summary>
		private bool SimCamera=false;
        /// <summary>
        /// 模擬執行分類結果
        /// </summary>
        private bool SimClassify=false;

        //Crypto
        /// <summary>
        /// 編密用密碼
        /// </summary>
        public const string encryptoKey1="sVRTe#6x";
        public const string encryptoKey2 = "4SVsr3*x";
        //密碼檔要用XGenerator產生   “xGenerator -?"
        /// <summary>
        /// 管理功能密碼
        /// </summary>
        public const string setupPassFile = "millingCutterPtp.stpa";
        /// <summary>
        /// 工程模式密碼
        /// </summary>
        public const string EngneerPassFile = "millingCutterPtp.stpb";

        //log
        private fileLog log;
		private DateTime timeStart;
		private DateTime timeCapture;
		private DateTime timeMeasureStart;
		private DateTime timeMeasureEnd;
		private DateTime timeClassify;
		private DateTime timeEnd;

        //狀態進程顯示
        /// <summary>
        /// 狀態色碼
        /// </summary>
		private int vColor = 0;
        /// <summary>
        /// 狀態進度色表
        /// </summary>
		private Color[] colors = { Color.LightGreen, Color.Green, Color.LightGreen, Color.LightBlue, Color.Blue, Color.LightBlue };

        //外部部件
        //相機
        /// <summary>
        /// 相機
        /// </summary>
        private TIS.Imaging.Device Camera;
        /// <summary>
        /// 相機狀態
        /// </summary>
        private int cameraStatus;  //0 : un-initial  1: ready   -1: not ready
        /// <summary>
        /// 相機拍下的照片
        /// </summary>
		private Bitmap capImage;

        /// <summary>
        /// Light
        /// </summary>
        private lightController light;

        /// <summary>
        /// PLC
        /// </summary>
        private PLCController plc;


        //Setting
        /// <summary>
        /// 設定值
        /// </summary>
        private IniFile setup;
        /// <summary>
        /// 量測參數
        /// </summary>
        private measureParameters measureParas;
        /// <summary>
        /// 分類器
        /// </summary>
        private classManager classifier;

        public bool initialSuccess;  //啟動成功

       
        //class display
        private boxCounter[] classCounter;

        //operation timer
        private System.Timers.Timer opTimer;

		//debug
		private int logClass = -1;

		//delegate to safe use controls
		delegate void classCountingCallBack(boxCounter bc);
		private void countingClass(boxCounter bc)
		{
			if (bc.InvokeRequired)
			{
				classCountingCallBack d = new classCountingCallBack(countingClass);
				this.Invoke(d, new object[] { bc });
			}
			else
			{
				bc.Counting();
			}
		}

		//delegate to safe use controls
		delegate void SetTextboxCallBack(TextBox box, string text);
        private void setText(TextBox bx, string text)
        {
            if(bx.InvokeRequired)
            {
                SetTextboxCallBack d = new SetTextboxCallBack(setText);
                this.Invoke(d, new object[] { bx, text });
            }
            else
            {
                bx.Text = text;
            }
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
		delegate void setPicImageCallBack(PictureBox bx, Bitmap bp);
		private void setPicImage(PictureBox bx, Bitmap bp)
		{
			if (bx.InvokeRequired)
			{
				setPicImageCallBack d = new setPicImageCallBack(setPicImage);
				this.Invoke(d, new object[] { bx, bp });
			}
			else
				bx.Image = bp;
		}
		delegate void savePicImageCallBack(PictureBox bx, string fileName);
		private void savePicImage(PictureBox bx, string fileName)
		{
			if (bx.InvokeRequired)
			{
				savePicImageCallBack d = new savePicImageCallBack(savePicImage);
				this.Invoke(d, new object[] { bx, fileName });
			}
			else
				bx.Image.Save(fileName);
		}

		delegate void setLabelColorCallBack(Label lbl, Color c);
		private void setLabelColor(Label lbl, Color c)
		{
			if (lbl.InvokeRequired)
			{
				setLabelColorCallBack d = new setLabelColorCallBack(setLabelColor);
				this.Invoke(d, new object[] { lbl, c });
			}
			else
				lbl.BackColor = c;
		}

		private int emptyCounter = 0;
		private int totalCounter = 0;

        //file version
        private FileVersionInfo MeasurerVersion = null;
        private void loadIdentifierVersion()
        {
            string IdnFile = Application.StartupPath + "\\MillingCutterMeasurer.dll";
            if (File.Exists(IdnFile))
            {
                try
                {
                    this.MeasurerVersion = FileVersionInfo.GetVersionInfo(IdnFile);
                }
                catch
                {
                    this.MeasurerVersion = null;
                }

            }
        }


        public frmMain()
        {
            InitializeComponent();

            //version
            this.lblVersion.Text = "Version " + this.production + "    Build " + this.assembly ;
#if DEBUG
            this.lblVersion.Text += " [" + this.fileVersion + "]";
#endif

            this.lblMeasurerVersion.Text = getMeasurerVersionInfo();

            strlib = new msgString();
            try
            {
                setup = new IniFile(Application.StartupPath + "\\setup.ini");
            }
            catch
            {
                MessageBox.Show(strlib.err(1));
                return;
            }


            /*/Do write encrypt password
			if(!MillingCutterMeasurer.crypto.writeToFile(Application.StartupPath+"\\"+this.setupPassFile,"888888xa",encryptoKey1, encryptoKey2))
			{
				MessageBox.Show("Setup password generate faile");
			}
			else
			{
				MessageBox.Show("Setup password generated");
			}
			return;
			/**/

			/*/Do write encrypt password
			if(!MillingCutterMeasurer.crypto.writeToFile(Application.StartupPath + "\\" + this.EngneerPassFile, "000012345678", encryptoKey1, encryptoKey2))
			{
				MessageBox.Show("Engineering password generate faile");
			}
			else
			{
				MessageBox.Show("Engineering password generated");
			}
			return;
			/**/
			
            //設定是否顯示量測圖
            if (setup.readInt("DEBUG", "Showmeasure") < 1)
            {
                this.tabMain.TabPages.Remove(this.tabMain.TabPages[1]);
            }

            
            //init measure parameters
            this.measureParas = measureParameters.Create(this.setup);
            if(this.measureParas==null)
            {
                MessageBox.Show("載入量測設定值失敗");
                Application.Exit();
            }

            this.lblFactor.Text = this.measureParas.calibrationFactor.ToString();

            //initial measurer
            try
            {
                this.classifier = classManager.fromFile(Application.StartupPath + "\\"+this.setup.read("Class", "classFile"));
            }
            catch
            {
                MessageBox.Show("載入分類設定檔失敗");
                Application.Exit();
            }

            //get loadFile
            try
            {
                this.lblFile.Text = setup.read("Class", "loadFile");
            }
            catch
            {

            }

            this.classCounter = new boxCounter[16];

            this.cameraStatus = -1;
			this.SimCamera = (setup.readInt("SimulationMode", "SimCamera") == 1);
            this.SimClassify= (setup.readInt("SimulationMode", "SimClassifty") == 1);

            string logfile = this.setup.read("Log","File");
            int? logLen = this.setup.readInt("Log", "displaySize");


            if (logLen == null || logLen<=0) logLen = 0;
            this.log = new fileLog((int)logLen, logfile);

			//進入執行狀態
			this.initialSuccess = true;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //更新分類表
            this.initialControls();

			//是否顯示時間
            if (setup.readInt("DEBUG", "ShowTime").GetValueOrDefault(0) != 1)
            {
                titelTimeClass.Visible = false;
                titleTimeComm.Visible = false;
                titleTimeCapture.Visible = false;
                titleTimeMeasure.Visible = false;
                titleTimePost.Visible = false;
                titleTimeTotal.Visible = false;

                lblTimeCapture.Visible = false;
                lblTimeClassify.Visible = false;
                lblTimeMeasure.Visible = false;
                lblTimePostComm.Visible = false;
                lblTimePreComm.Visible = false;
                lblTimeTotal.Visible = false;
            }

			//this.initialChecks();
            this.logClass = setup.readInt("DEBUG", "LogClass").GetValueOrDefault(-1);

            //check initial
            //運作定時器  定時操作
            this.opTimer = new System.Timers.Timer((int)setup.readInt("Timing", "checkState"));
            this.opTimer.Elapsed += checkStatusHandler;
            this.log.add("opTimer attach checkTimeOutHandler");
            this.log.add("Start opTimer");
            this.opTimer.Start();
        }

        private string getMeasurerVersionInfo()
        {
            string IdnFile = Application.StartupPath + "\\MillingCutterMeasurer.dll";

            string rtn = "";
            if (File.Exists(IdnFile))
            {
                try
                {
                    rtn = "Measurer ";
                    rtn += FileVersionInfo.GetVersionInfo(IdnFile).ProductVersion.ToString();
                    rtn += "  Build " + Assembly.LoadFrom(IdnFile).GetName().Version.ToString();

#if DEBUG
                    rtn += " [" + FileVersionInfo.GetVersionInfo(IdnFile).FileVersion.ToString() + "]";
#endif
                }
                catch (Exception ex)
                {
                    rtn = "Measurer version unknown";
                }

            }
            else
                rtn = "Measurer not found";

            return rtn;
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
			if (this.inOperation)
			{
				MessageBox.Show("系統執行中,不可進入工程模式");
				return;
			}


			dlgEngineer dlg = new dlgEngineer();
			dlg.password = MillingCutterMeasurer.crypto.readFromFile(Application.StartupPath + "\\" + frmMain.EngneerPassFile, frmMain.encryptoKey1, frmMain.encryptoKey2);
			if (dlg.password == null || dlg.password.Length < 1)
				return;

            dlg.ShowDialog();
            if ( dlg.DialogResult == DialogResult.OK)
            {
                dlg.Close();
                MeasureTester.frmEngineer frmEng = new MeasureTester.frmEngineer();
                if (icImage.DeviceValid)
                    icImage.LiveStop();
                frmEng.camera = this.Camera;
				frmEng.cameraSetting = Application.StartupPath+"\\"+setup.read("Camera", "Setting");
				frmEng.ShowDialog();
                frmEng.Dispose();
            }
            else
                dlg.Close();
            if (icImage.DeviceValid)
                this.initialCamera();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (
                !this.forceExit   //強制關機
                && 
                MessageBox.Show(strlib.str("exit?"), strlib.str("millingCuterClassifier"), MessageBoxButtons.YesNo) != DialogResult.Yes
                )
                e.Cancel = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.inOperation = !this.inOperation;

			if(this.inOperation)
			{
				this.opTimer.Start();
			}
			else
			{
				this.opTimer.Stop();
			}

            this.refreshBtnPause();
        }

        private void refreshBtnPause()
        {
            if (!this.inOperation)
            {
                btnPause.Text = ">>   執行";
                lblMessage.Text = "暫停";
                lblMessage.Text = "";
            }
            else
            {
                btnPause.Text = "II   暫停";
                lblMessage.Text = "執行中";
                lblMessage.Text = "等待....";
            }
        }

       
        private void btnSetting_Click(object sender, EventArgs e)
        {
            if(this.inOperation)
            {
                MessageBox.Show("系統執行中,不可更改系統設定");
                return;
            }

			this.opTimer.Stop();

            dlgPassword dp = new dlgPassword();
			dp.password = MillingCutterMeasurer.crypto.readFromFile(Application.StartupPath + "\\" + frmMain.setupPassFile, frmMain.encryptoKey1,frmMain.encryptoKey2);
			dp.shutdownOnly = dp.password == null || dp.password.Length < 1;
				

			this.icImage.LiveStop();
            dp.ShowDialog();
			if (dp.shutdown)
				Application.Exit();

            if(dp.pass)
            {
				this.opTimer.Stop();

                settingPage sp = new settingPage();
				sp.newCalibrateApplied = false;
				sp.newClassApplied = false;
				sp.plc = this.plc;
				sp.Camera = this.Camera;

				sp.calibImage = null;
				sp.measureParas = this.measureParas;
				sp.classMgr = (classManager)this.classifier.Clone();
                sp.simCamera = this.SimCamera;
                sp.lblFile.Text = this.lblFile.Text;
                
                sp.ShowDialog();

				if(sp.newClassApplied)
				{
					this.classifier = (classManager) sp.classMgr.Clone();
					this.refreashClasses();
					this.classifier.toFile(Application.StartupPath + "\\" + this.setup.read("Class", "classFile"));
					this.counterReset();
                    this.setup.write("Class", "loadFile", sp.lblFile.Text);
                    this.lblFile.Text = this.setup.read("Class", "loadFile");
                }

                if (sp.newCalibrateApplied)
				{
					this.setup.write("Measurement", "calibration",this.measureParas.calibrationFactor.ToString());
					this.setup.write("Measurement", "interception", this.measureParas.interception.ToString());
					
				}

				
            }
			this.refreashClasses();

			if(!this.SimCamera && this.icImage.DeviceValid)
				this.icImage.LiveStart();

			this.opTimer.Start();
		}

        private void icImagingControl1_Load(object sender, EventArgs e)
        {
            this.cameraStatus = (this.initialCamera()) ? 1 : 0;
        }
        private bool initialCamera()
        {
            if (this.SimCamera)
                return true;

			if(icImage.DeviceValid)
			{
				icImage.LiveStart();
				return true;
			}

            if (this.Camera != null)
                this.Camera.Dispose();
            

            TIS.Imaging.Device[] cameras;
            try
            {
                cameras = icImage.Devices;
            }
            catch
            {
                MessageBox.Show(strlib.err(4));
                return false;
            }

            if (cameras == null)
            {
                MessageBox.Show(strlib.err(4));
                return false;
            }

            if (cameras.Length < 1)
            {
                MessageBox.Show(strlib.err(5));
                return false;
            }
            else
            {
                string cName = setup.read("Camera", "Name");
                string cSerial = setup.read("Camera", "Serial");
                this.Camera = findCamera(cameras, cName, cSerial);
                if (this.Camera == null)
                {
                    MessageBox.Show(string.Format(strlib.err(2), cName, cSerial));
                    return false;
                }
            }
             icImage.Device = this.Camera.Name;

            icImage.DeviceFrameRate = 10;
            string s = icImage.DeviceState;

            try
            {
                icControlInitial();
            } 
            catch
            {
                
                MessageBox.Show(strlib.err(6));
                return false;
            }

            return true;
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

			string properties = File.ReadAllText(Application.StartupPath + "\\" + this.setup.read("Camera", "Setting"));
			icImage.VCDPropertyItems.Load(properties);

			icImage.LiveStart();
        }
        private TIS.Imaging.Device findCamera(TIS.Imaging.Device[] cs, string name, string serial)
        {
            string tSerial = "";
            foreach (TIS.Imaging.Device c in cs)
            {
                if (c.GetSerialNumber(out tSerial))
                    if (c.Name == name) // && tSerial == serial)
                        return c;
            }

            return null;
        }

        
        /// <summary>
        /// 初始化分類表
        /// </summary>
        private void initialControls()
        {
            //類別計數分類顯示表
            int posX = 29;
            int posY = 74;
            int dX = 476;
            int dY = 102;

            for (int i = 0; i < 16; ++i)
            {
                this.classCounter[i] = boxCounter.Create((uint) ((i + 1)%16), null);
                this.panel3.Controls.Add(this.classCounter[i]);
                this.classCounter[i].Location = new Point(posX + ((i / 8) * dX), posY + ((i % 8) * dY));
				this.classCounter[i].Visible = false;
            }
			this.classCounter.Last().SN = null;
			this.classCounter.Last().Visible = true;

            //更新目前的分類表
			this.refreashClasses();

			this.lblTotal.Text = "0";
			this.lblEmptyCount.Text = "0";
        }

		private void refreashClasses()
		{
			if (this.classifier.Count >= this.classCounter.Length)
				throw new Exception("classifer.count >= classCounter.length");

			for (int i = 0; i < this.classCounter.Length-1; ++i)
				this.classCounter[i].Visible = false;

			millingCutterClass[] mcm = this.classifier.classes;
			for(int i=0; i<mcm.Length; ++i)
			{
				this.classCounter[i].maxFullLength = mcm[i].maxLength;
				this.classCounter[i].minFullLength = mcm[i].minLength;
				this.classCounter[i].maxHandleLength = mcm[i].maxHandleLength;
				this.classCounter[i].minHandleLength = mcm[i].minHandleLength;
				this.classCounter[i].maxHandleWidth = mcm[i].maxHandleWidth;
				this.classCounter[i].minHandleWidth = mcm[i].minHandleWidth;
				this.classCounter[i].maxBladeWidth = mcm[i].maxBladeWidth;
				this.classCounter[i].minBladeWidth = mcm[i].minBladeWidth;


				this.classCounter[i].reset();
				this.classCounter[i].Visible = true;
			}

            string bladeTitle = (this.classifier.bladeUseInner) ? "芯厚" : "刃外徑";
            lblTitleBlade1.Text = bladeTitle;
            lblTitleBlade2.Text = bladeTitle;
            lblTitleBlade3.Text = bladeTitle;
        }
        private void initialChecks()
        {
            initialLight();
            initialPLC();
        }
        
        /// <summary>
        /// 初始化PLC元件
        /// </summary>
        private void initialPLC()
        {
            this.log.add("opTimer initialPLC");

            if (this.plc != null) this.plc.Dispose();
            
            tSerialPort com = new tSerialPort();
            com.PortName = setup.read("PLC", "port");
            com.BaudRate = setup.readInt("PLC", "baudrate").GetValueOrDefault(0);
            if (com.BaudRate == 0) throw new Exception(strlib.err(12));

            com.DataBits = setup.readInt("PLC", "databits").GetValueOrDefault(0);
            if (com.DataBits == 0) throw new Exception(strlib.err(12));

            switch (setup.readInt("PLC", "stopbits").GetValueOrDefault(-1))
            {
                case 1:
                    com.StopBits = StopBits.One;
                    break;
                case 0:
                    com.StopBits = StopBits.None;
                    break;
                case 2:
                    com.StopBits = StopBits.Two;
                    break;
                default:
                    throw new Exception(strlib.err(12));
            }
            string party = setup.read("PLC", "parity");
            if (party.ToUpper() == "ODD")
            {
                com.Parity = Parity.Odd;
            }
            else if (party.ToUpper() == "EVEN")
            {
                com.Parity = Parity.Even;
            }
            else if (party.ToUpper() == "NONE")
            {
                com.Parity = Parity.None;
            }
            else
            {
                throw new Exception(strlib.err(12));
            }


            int? tStatus = setup.readInt("PLC", "addrRegStatus");
            int? tRequest = setup.readInt("PLC", "addrRegRequest");
            int? tResult = setup.readInt("PLC", "addrRegResult");
            int? tBoxClass = setup.readInt("PLC", "addrRegBoxClass");
            int? tBoxQty = setup.readInt("PLC", "addrRegBoxQty");

            if (tStatus == null || tRequest == null || tResult == null || tBoxClass == null || tBoxQty == null)
                throw new Exception(strlib.err(12));

            this.log.add("init plc");
            this.plc = new PLCController(com, (int)tStatus, (int)tRequest, (int)tResult, (int)tBoxClass, (int)tBoxQty, this.log);
            this.log.add("this.plc attach Plc_onRequestClassify_Handler");

            this.plc.onRequestClassify_Handler += Plc_onRequestClassify_Handler;
            this.plc.onNoRequest_Handler += Plc_onNoRequest_Handler;
            this.plc.onComTimeout_Handler += Plc_onTimeOut_Handler;
        }

        /// <summary>
        /// 無Requset的處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plc_onNoRequest_Handler(object sender, EventArgs e)
        {

            this.log.add(" <-- no request");
        
            this.log.add("opTime += checkRequestHandler");
            this.opTimer.Elapsed += checkRequestHandler;
            this.log.add("Start opTimer");
            this.opTimer.Start();
        }

        /// <summary>
        /// 處理分類量測事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plc_onRequestClassify_Handler(object sender, EventArgs e)
        {

            Bitmap im;
            try
            {
				this.timeCapture = DateTime.Now;
				im = this.capImage;
            }
            catch
            {
				if(!SimCamera)
				{
					throw new Exception("相機連接失效");
				}
                string path = Application.StartupPath + "\\pictures\\";
                string[] files = Directory.GetFiles(path, "*.bmp");

                Random rd = new Random();
                double dIndex = rd.Next(files.Length);
                int index = ((int) Math.Round(dIndex)) % files.Length;
                im=new Bitmap(files[index]);
            }
            this.picMesaure.Image = im;
            
            
            string message;
			bool measureSuccess = false;
			int measureCnt = 0;

			MillingCutterMeasurer.millingCutterMeasurer m = null;
			while (measureCnt < 3)
			{
				 m = MillingCutterMeasurer.millingCutterMeasurer.Create(im, out message, this.measureParas.seekBorder, this.measureParas.interception, this.measureParas.calibrationFactor);
				try
				{
					this.timeMeasureStart = DateTime.Now;
					measureSuccess = doMeasure(m, out message, this.measureParas);
					this.timeMeasureEnd = DateTime.Now;
					if(measureSuccess)
					{
						measureCnt = 999;
					}
					else
					{
						this.log.errAdd("doMeasure : " + message);
						++measureCnt;
						Thread.Sleep(20);
						icImage.MemorySnapImage();
						im= icImage.ImageActiveBuffer.Bitmap;
					}
				}
				catch (Exception ex)
				{
					this.log.errAdd("doMeasure : " + ex.Message);
					++measureCnt;
				}
			}

			int cNumber;
			if (measureSuccess && m!=null)
            {
                setLabelText(lblFullLength, m.getCalibrateLength("full_length").ToString("0.00"));
                setLabelText(lblHandleLength, m.getCalibrateLength("handle_length").ToString("0.00"));
                setLabelText(lblBladeWidth, m.getCalibrateLength((this.classifier.bladeUseInner) ? "blade_width" : "blade_outer_width" ).ToString("0.00"));
                setLabelText(lblHandleWidth, m.getCalibrateLength("handle_width").ToString("0.00"));

				if (this.SimClassify)
				{
					cNumber = number + 1;
					this.number += 1;
					this.number %= 16;
				}
				else
					cNumber = this.classifier.classify(m);

				this.classCounter[cNumber - 1].Counting();
				this.timeClassify = DateTime.Now;
			}
            else
            {
				setLabelText(lblFullLength, "-");
				setLabelText(lblHandleLength, "-");
				setLabelText(lblBladeWidth, "-");
				setLabelText(lblHandleWidth, "-");
				cNumber = 0;
				++this.emptyCounter;
				setLabelText(lblEmptyCount, this.emptyCounter.ToString());
			}
            /**/

            plc.writeClass(cNumber);
			this.timeEnd = DateTime.Now;
			setLabelText(lblClassNumber, (cNumber).ToString());

			if (setup.readInt("DEBUG", "ShowTime") > 0)
			{
				setLabelText(lblTimeTotal, (this.timeEnd - this.timeStart).TotalMilliseconds.ToString());
				setLabelText(lblTimePreComm, (this.timeCapture - this.timeStart).TotalMilliseconds.ToString());
				setLabelText(lblTimeCapture, (this.timeMeasureStart - this.timeCapture).TotalMilliseconds.ToString());
				setLabelText(lblTimeMeasure, (this.timeMeasureEnd - this.timeMeasureStart).TotalMilliseconds.ToString());
				setLabelText(lblTimeClassify, (this.timeClassify - this.timeMeasureEnd).TotalMilliseconds.ToString());
				setLabelText(lblTimePostComm, (this.timeEnd - this.timeClassify).TotalMilliseconds.ToString());
			}


			++this.totalCounter;
			setLabelText(lblTotal, this.totalCounter.ToString());

			if (cNumber == this.logClass)
			{
				int cnt = 0;
				while (cnt < 3) // 3 try
				{
					try
					{
						savePicImage(this.picMesaure, cNumber.ToString() + "_" + Guid.NewGuid().ToString() + ".bmp");
						cnt = 3;
					}
					catch (Exception ex)
					{
						++cnt;
						this.log.errAdd("savePicImage : " + ex.Message);
						Thread.Sleep(200);
					}
				}

			}

			this.log.add("opTime += checkRequestHandler");
            this.opTimer.Elapsed += checkRequestHandler;
            this.log.add("Start opTime");
            this.opTimer.Start();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plc_onTimeOut_Handler(object sender, EventArgs e)
        {
            this.log.add(" PLC com time out");

            this.log.add("opTime -= checkRequestHandler");
            this.opTimer.Elapsed -= checkRequestHandler;
            this.log.add("opTime += this.checkStatusHandler");
            this.opTimer.Elapsed += this.checkStatusHandler;
            this.log.add("Start opTimer");
            this.opTimer.Start();
        }

        private bool doMeasure(MillingCutterMeasurer.millingCutterMeasurer measurer, out string message, measureParameters paras)
        {

            bool calibBaseSuccess;
            try
            {
                //calibBaseSuccess = measurer.calibrateBase(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY, paras.baseGapX, paras.baseWidth);
				calibBaseSuccess = measurer.calibrateBase(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY);
			}
			catch
            {
                message = "影像校正失敗, 請檢查影像";
                return false;
            }

            bool calibCutterSuccess;
            try
            {
                //calibCutterSuccess = measurer.calibrateCutter(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY, paras.baseGapX, paras.baseWidth);
				calibCutterSuccess = measurer.calibrateCutter(new Rectangle(paras.boxX, paras.boxY, paras.boxWidth, paras.boxHeight), paras.boxDistanceY,paras.baseGapX, paras.baseWidth);
			}
			catch
            {
                message = "銑刀位置校正失敗, 請檢查影像";
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
                measureSuccess = measurer.doMeasure(paras.baseGapX, paras.baseWidth, paras.searchHeight, paras.bladeShift, paras.bladeCheckWidth, 0.3, 5);
            }
            catch
            {
                message = "量測失敗";
                return false;
            }

            message = "";
            return true;

        }


        private void initialLight()
        {
            //setup com object
            tSerialPort com = new tSerialPort();
            com.PortName = setup.read("Light", "port");
            com.BaudRate = 19200;
            com.DataBits = 8;
            com.StopBits = StopBits.One;
            com.Parity = Parity.None;

            if (this.light != null) this.light.Dispose();

            this.light = new lightController(com, 0x00);
        }

        /// <summary>
        /// 檢查系統狀態事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkStatusHandler(object sender, EventArgs e)
        {
            this.log.add("do checkTimeOutHandler");
            this.log.add("Stop opTime");
            this.opTimer.Stop();
            this.opTimer.Elapsed -= checkStatusHandler;
            this.log.add("opTimer dettach checkTimeOutHandler");

            //check camera
            if (this.cameraStatus==1)
            {
                this.setLabelText(lblCameraMessage, strlib.str("connected"));
                this.lblCameraSignal.BackColor = Color.LightGreen;
            }
            else
            {
                this.setLabelText(lblCameraMessage, strlib.str("disconnected"));
                this.lblCameraSignal.BackColor = Color.Gray;
            }


            //check Light
            if(this.light!=null && this.light.isConnected)
            {
                this.lblLightSignal.BackColor = Color.LightGreen;
                this.setLabelText(lblLightMessage, strlib.str("Turn On"));
                string strCh = setup.read("Light", "chnnel");
                byte ch;
                if (!byte.TryParse(strCh, out ch))
                    throw new Exception(strlib.err(10));
                string strLevel = setup.read("Light", "level");
                byte level;
                if (!byte.TryParse(strLevel, out level))
                    throw new Exception(strlib.err(11));

                this.light.setLevel(ch, level);
            }
            else
            {
                this.lblLightSignal.BackColor = Color.Gray;
                this.setLabelText(lblLightMessage, strlib.str("Turn Off"));
            }


            //check PLC
            string statusStr;
			if (this.plc != null && this.plc.isConnected)
            {
                this.lblPLCSignal.BackColor = Color.LightGreen;
                switch(plc.status)
                {
                    case PLCController.Statuses.Online:
                        statusStr="OnLine";
                        break;
                    case PLCController.Statuses.Pause:
                        statusStr = "Pause";
                        break;
                    case PLCController.Statuses.Stop:
                        statusStr = "Stop";
                        break;
                    case PLCController.Statuses.Error:
                        statusStr = "Error";
                        break;
                    default:
                        statusStr = "connected";
                        break;
                }
            }
            else
            {
                this.lblPLCSignal.BackColor = Color.Gray;
                statusStr = "disconnected";
            }
            this.setLabelText(lblPLCMessage, strlib.str(statusStr));


            //check all
            if(this.plc!=null && plc.isConnected && this.light!=null && light.isConnected &&  this.cameraStatus==1)
            {
                this.lblSystemSignal.BackColor = Color.LightGreen;
                if(plc.status==PLCController.Statuses.Online)
                {
                    this.setLabelText(lblMessage, strlib.str("OnLine"));
                    this.opTimer.Elapsed += checkRequestHandler;
                    this.opTimer.Interval = (int) setup.readInt("Timing","checkRequest");
                    this.log.add("opTimer += checkRequestHandler");
                    this.log.add("Start opTime");
                    this.opTimer.Start();
                    return;
                }
                else
                {
                    this.setLabelText(lblMessage, strlib.str("NotReady"));
                }
            }
            else
            {
                this.lblSystemSignal.BackColor = Color.Gray;
                this.setLabelText(lblMessage, strlib.str("disconnected"));
            }

            initialChecks();
            this.opTimer.Elapsed += checkStatusHandler;
            this.opTimer.Interval =(int) setup.readInt("Timing", "checkState");
            this.log.add("opTimer += checkTimeOutHandler");
            this.log.add("start opTime");
            this.opTimer.Start();
        }

        /// <summary>
        /// 檢查量測要求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkRequestHandler(object sender, EventArgs e)
        {
            this.log.add(">>> checkRequestHandler");

            this.log.add("stop opTimer");
            this.opTimer.Elapsed -= checkRequestHandler;
            this.opTimer.Stop();


			this.timeStart = DateTime.Now;
			this.capImage = null;
			plc.checkRequest();

            if(!this.SimCamera)
            {
                int tryCnt = 0;
                do
                {
                    try
                    {
                        icImage.MemorySnapImage();
                        this.capImage = icImage.ImageActiveBuffer.Bitmap;
                        tryCnt = IMAGE_CAPTURED;
                    }
                    catch (Exception ex)
                    {
                        this.log.errAdd("Capture image : " + ex.Message);
                        ++tryCnt;
                        Thread.Sleep(IMAGE_CAPTURE_LIMIT);
                    }
                } while (tryCnt < 10);

                if(tryCnt < IMAGE_CAPTURED)
                    throw new Exception("影像擷取失敗");
            }

			

			//do changeColor
			setLabelColor(lblSystemSignal, colors[vColor]);
			++this.vColor;
			this.vColor %= 6;
        }

        private void icImage_DeviceLost(object sender, TIS.Imaging.ICImagingControl.DeviceLostEventArgs e)
        {
            this.setLabelText(lblCameraMessage, strlib.str("disconnected"));
            this.lblCameraSignal.BackColor = Color.Gray;
            this.cameraStatus = 0;
            MessageBox.Show("Disconnected");


        }

        private void lblBladeWidthPx_Click(object sender, EventArgs e)
        {

        }

		private void btnCountReset_Click(object sender, EventArgs e)
		{
			counterReset();
			this.setLabelText(lblTotal, this.totalCounter.ToString());
			this.setLabelText(lblEmptyCount, this.emptyCounter.ToString());
		}

		private void counterReset()
		{
			if (this.inOperation)
				return;

			if (MessageBox.Show("全部計數清零？", "重置計數值", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;

			this.totalCounter = 0;
			this.emptyCounter = 0;

			foreach (boxCounter bc in classCounter)
				bc.reset();

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}
	}
}
