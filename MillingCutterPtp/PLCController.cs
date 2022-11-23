using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using System.Globalization;
using System.Reflection;

namespace MillingCutterPtp
{
    public class PLCController : IDisposable
    {
        bool disposed = false;

        private tSerialPort com;
        private msgString strlib;

        /// <summary>
        /// 狀態
        /// </summary>
        public enum Statuses
        {
            Pause=0,
            Online=1,
            Stop=65535,
            Error=65534,
            UnKnow=65533
        }
        /// <summary>
        /// 操作類別
        /// </summary>
        public enum Operation
        {
            None=0,
            Classify=1,
            Hold=9
        }
        /// <summary>
        /// 控制器狀態
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public Statuses status { get; private set; }
        /// <summary>
        /// 當前的操作
        /// </summary>
        /// <value>
        /// The current operation.
        /// </value>
        public Operation currentOperation { get; private set; }

        public bool isConnected { get; private set; }
        public int timeOutTime { get; private set; }

        public int addrRegStatus { get; private set; }  //PLC狀態
        public int addrRegRequest { get; private set; }  //PLC向PC提需求
        public int addrRegResult { get; private set; }   //PC回應結果區
        public int addrRegBoxClass { get; private set; }  //box分類起始位址
        public int addrRegBoxQry { get; private set; }  //box容量起始位址

        private fileLog log;

        public PLCController(tSerialPort sPort,int addrStatus, int addrRequest, int addrResult, int addrBoxC, int addrBoxQ, fileLog flog, int tTimeout=1000)
        {
            strlib = new msgString();
            this.log = flog;
            this.isConnected = false;
            this.status = Statuses.UnKnow;

            this.com = sPort;
            if (tTimeout < 0)
                throw new Exception(strlib.err(12));
            this.timeOutTime = tTimeout;

            if (sPort == null)
                throw new Exception(strlib.err(12));

            try
            {
                this.com.Open();
            }
            catch
            {
                throw new Exception(strlib.err(13));
            }
            if (!this.com.IsOpen)
                throw new Exception(strlib.err(13));

            this.isConnected = false;
            this.addrRegStatus = addrStatus;
            this.addrRegRequest = addrRequest;
            this.addrRegResult = addrResult;
            this.addrRegBoxClass = addrBoxC;
            this.addrRegBoxQry = addrBoxQ;

            //this.com.DataReceived += queryHandler;
            this.com.timeoutHandler += Com_timeoutHandler;

            this.checkStatus();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
               if(this.com!=null)
                {
                    if (this.com != null)
                    {
                        this.com.Close();
                        this.com.Dispose();
                    }
                }
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~PLCController()
        {
            Dispose(false);
        }

        public void checkStatus()
        {
            this.com.DataReceived += checkStatusHandler;
            try
            {
                string command = "00FFWR1D" + this.addrRegStatus.ToString() + "01";
                char[] bytesCommand = new char[command.Length + 1];
                bytesCommand[0] = (char) 0x05;
                Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
				//無連線時會丟例外
                this.com.Write(bytesCommand, 0, bytesCommand.Length);
                this.com.setTimeout(this.timeOutTime);
            }
            catch (Exception ex)
            {
                this.isConnected = false;
                throw new Exception(strlib.err(9));
            }
        }

        public void checkRequest()
        {
            this.com.DataReceived += this.checkRequestHandler;
            this.log.add("plc += checkReauestHandler");

            try
            {
                string command = "00FFWR1D" + this.addrRegRequest.ToString() + "01";
                char[] bytesCommand = new char[command.Length + 1];
                bytesCommand[0] = (char)0x05;
                Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
                this.log.add("plc --> " + bytesCommand[0].ToString()+" " + command);
                this.com.Write(bytesCommand, 0, bytesCommand.Length);
                this.com.setTimeout(this.timeOutTime);
            }
            catch (Exception ex)
            {
                this.isConnected = false;
                this.status = Statuses.Error;

                throw new Exception(strlib.err(9));
            }
        }


		public void writeBoxClasses(uint[] classes)
		{
			this.com.DataReceived += writeBoxClassesHandler;

			try
			{
				string command = "00FFWW1D" + this.addrRegBoxClass.ToString()+classes.Length.ToString("X2");
				for (int i = 0; i < classes.Length; ++i)
					command += classes[i].ToString("X4");
				char[] bytesCommand = new char[command.Length + 1];
				bytesCommand[0] = (char)0x05;
				Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
				this.com.Write(bytesCommand, 0, bytesCommand.Length);
				this.com.setTimeout(this.timeOutTime);
			}
			catch
			{
				throw new Exception(strlib.err(9));
			}
		}


		private void writeBoxClassesHandler(object sender, SerialDataReceivedEventArgs e)
		{
			this.com.DataReceived -= writeBoxClassesHandler;
			this.com.setTimeout(0);
			tSerialPort port = (tSerialPort)sender;

			int type = port.ReadByte();
			string line = port.ReadExisting();
			this.status = Statuses.UnKnow;
			this.isConnected = true;
			serialRespons r;

			try
			{
				r = new serialRespons(type, line);
				if (r.isACK)
				{
					this.OnBoxClassesWrited(e);
				}
				else
				{
					
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public event EventHandler onBoxClassesWrited_Handler;
		/// <summary>
		/// 發佈寫入Box數量事件
		/// </summary>
		public virtual void OnBoxClassesWrited(EventArgs e)
		{
			EventHandler handler = this.onBoxClassesWrited_Handler;
			if (handler != null) handler(this,e);
		}

		public void writeBoxQuantities(uint[] qtys)
		{
			this.com.DataReceived += writeBoxQuantitiesHandler;

			try
			{
				string command = "00FFWW1D" + this.addrRegBoxQry.ToString() + qtys.Length.ToString("X2");
				for (int i = 0; i < qtys.Length; ++i)
					command += qtys[i].ToString("X4");
				char[] bytesCommand = new char[command.Length + 1];
				bytesCommand[0] = (char)0x05;
				Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
				this.com.Write(bytesCommand, 0, bytesCommand.Length);
				this.com.setTimeout(this.timeOutTime);
			}
			catch
			{
				throw new Exception(strlib.err(9));
			}
		}

		private void writeBoxQuantitiesHandler(object sender, SerialDataReceivedEventArgs e)
		{
			this.com.DataReceived -= writeBoxQuantitiesHandler;
			this.com.setTimeout(0);
			tSerialPort port = (tSerialPort)sender;

			int type = port.ReadByte();
			string line = port.ReadExisting();
			this.status = Statuses.UnKnow;
			this.isConnected = true;
			serialRespons r;

			try
			{
				r = new serialRespons(type, line);
				if (r.isACK)
				{
					this.onBoxQuantityWrited(e);
				}
				else
				{

				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public event EventHandler onBoxQuantityWrited_Handler;
		public virtual void onBoxQuantityWrited(EventArgs e)
		{
			EventHandler handler = this.onBoxQuantityWrited_Handler;
			if (handler != null) handler(this, e);
		}

		public void writeClass(int cNum)
        {
            this.com.DataReceived += writeClassHandler;
            try
            {
                string command = "00FFWW0D" + this.addrRegResult.ToString() + "01"+cNum.ToString("X4");
                char[] bytesCommand = new char[command.Length + 1];
                bytesCommand[0] = (char)0x05;
                Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
                this.com.Write(bytesCommand, 0, bytesCommand.Length);
                this.com.setTimeout(this.timeOutTime);
            }
            catch
            {
                throw new Exception(strlib.err(9));
            }
        }

        private void writeClassHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.com.DataReceived -= writeClassHandler;
            this.com.setTimeout(0);
            tSerialPort port = (tSerialPort)sender;

            int type = port.ReadByte();
            string line = port.ReadExisting();
            this.status = Statuses.UnKnow;
            this.isConnected = true;
			serialRespons r;

			try
            {
                 r = new serialRespons(type, line);
                if (r.isACK)
                {
                    //do measure
                    //this.onRequestClassify(new EventArgs());
                }
                else
                {
					//throw new Exception("unreconized respons ["+r.body+"]");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Com timeout event handler
        private void Com_timeoutHandler(object sender, EventArgs e)
        {
            this.isConnected = false;
            //丟出PLC time out
            this.onComTimeout(new EventArgs());
        }

        

        //檢查request
        private void checkRequestHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.log.add("plc >>> checkRequestHandler");
            this.com.DataReceived -= checkRequestHandler;
            this.log.add("plc -= checkRequestHandler");

            this.com.setTimeout(0);
            tSerialPort port = (tSerialPort)sender;

            int type = port.ReadByte();
			string line = "";
			int cnt = 0;

			do
			{
				if (cnt > 10)
					throw new Exception("Invalid UART message format");
				Thread.Sleep(2);
				line += port.ReadExisting();
			} while (line.Length % 4 != 1);
				
            this.log.add("plc <-- "+line);

            this.status = Statuses.UnKnow;

			serialRespons r;

			try
            {
                r = new serialRespons(type, line);
                if (r.isSTX && r.data!=null && r.data.Length>0)
                {
                    int code;
                    if (int.TryParse(r.data[0], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code))
                    {
                        if(code==1)//PLC請求量測
                        {
                            this.log.add("plc <-- measurement request");
                            this.clearRequest(Operation.Classify);
                        }
                        else if (code==2)  //PLC詢問狀態
                        {
                            this.log.add("plc <-- status request");
                            //回應0  正常
                        }
                        else if(code==-1) //PLC發生錯誤
                        {
                            this.log.add("plc <-- error request");
                            //進入檢查狀態
                        }
                        else if(code==0)  // no request
                        {
                            this.log.add("plc <-- no request");
                            this.onNoRequest(new EventArgs());
                        }

                    }
                }
                else
                {
                    this.log.add("plc get wrong message in checkRequestHandler");
                    //throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                this.log.add("plc get exception in checkRequestHandler" + " - " + ex.Message);
                throw ex;
            }
        }

        //清除RegRequest
        public void clearRequest(Operation op)
        {
            this.log.add("plc += classifyHandler");
            this.com.DataReceived += classifyHandler;
            try
            {
                string command = "00FFWW1D" + this.addrRegRequest.ToString() + "010000";
                char[] bytesCommand = new char[command.Length + 1];
                bytesCommand[0] = (char)0x05;
                Array.Copy(command.ToArray(), 0, bytesCommand, 1, command.Length);
                this.log.add("plc --> - " + bytesCommand[0].ToString() + command);
                this.com.Write(bytesCommand, 0, bytesCommand.Length);
                this.com.setTimeout(this.timeOutTime);
            }
            catch
            {
                this.log.add("plc error in clearRequest");
                throw new Exception(strlib.err(9));
            }
        }

        private void classifyHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.com.DataReceived -= classifyHandler;
            this.log.add("plc -= classifyHandler");
            this.com.setTimeout(0);
            tSerialPort port = (tSerialPort)sender;

            int type = port.ReadByte();
            string line = port.ReadExisting();
            this.log.add("plc <-- " + line);
            this.status = Statuses.UnKnow;
            this.isConnected = true;
			serialRespons r;

			int c = 0;
			try
            {
				c = 1;
                 r = new serialRespons(type, line);
                if (r.isACK)
                {
					c = 2;
					this.log.add("plc do RequestClassify");
					c = 3;
					//do measure
					this.onRequestClassify(new EventArgs());
                }
                else
                {
					c =4;
					this.log.add("plg get unexcept message in classifyHandler");
					//throw new NotImplementedException();
				}
			}
            catch (Exception ex)
            {
                this.log.add("plc get exception - " + ex.Message);
                throw ex;
            }
        }

        private void checkStatusHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.com.DataReceived -= checkStatusHandler;
            this.com.setTimeout(0);
            tSerialPort port = (tSerialPort)sender;

            int type = port.ReadByte();
			string line = "";
			int loop = 0;

			do
			{
				Thread.Sleep(2);
				line += port.ReadExisting();
				++loop;
			} while (line.Length % 4 != 1 && loop<10);

			this.status = Statuses.UnKnow;
            this.isConnected = true;
			serialRespons r;

			try
            {
                r = new serialRespons(type, line);
                if (r.isSTX)
                {
                    int code;
                    if(int.TryParse(r.data[0], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code))
                    {
                        switch(code)
                        {
                            case 0:
                                this.status = Statuses.Pause;
                                break;
                            case 1:
                                this.status = Statuses.Online;
                                break;
                            case 65535:
                                this.status = Statuses.Stop;
                                break;
                            case 65534:
                                this.status = Statuses.Error;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void queryHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort port = (System.IO.Ports.SerialPort)sender;
            this.com.setTimeout(0);

            int type = port.ReadByte();
            string line = port.ReadExisting();
            try
            {
                this.dispathMessage(new serialCommand(type, line));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dispathMessage(serialCommand message)
        {
            Console.WriteLine("Get" + message.messageBody);
        }





        //***********  events ***********************
        public event EventHandler onRequestClassify_Handler;
        /// <summary>
        /// 發佈需求分類事件
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void onRequestClassify(EventArgs e)
        {
            EventHandler handler = this.onRequestClassify_Handler;
            if (handler != null) handler(this, e);
        }

		
		public event EventHandler onRequestWriteBoxQty_Handler;
		/// <summary>
		/// 發佈寫入Box數量事件
		/// </summary>
		public virtual void onRequestWriteBoxQty(EventArgs e)
		{
			EventHandler handler = this.onRequestWriteBoxQty_Handler;
			if (handler != null) handler(this,e);
		}

        public event EventHandler onNoRequest_Handler;
        /// <summary>
        /// 發佈無需求事件
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void onNoRequest(EventArgs e)
        {
            EventHandler handler = this.onNoRequest_Handler;
            if (handler != null) handler(this, e);
        }

        public event EventHandler onComTimeout_Handler;
        public virtual void onComTimeout(EventArgs e)
        {
            EventHandler handler = this.onComTimeout_Handler;
            if (handler != null) handler(this, e);
        }
    }

    
}
