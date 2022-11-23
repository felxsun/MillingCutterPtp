using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
using System.Threading;

namespace MillingCutterPtp
{
    public class lightController : IDisposable
    {
        bool disposed = false;

        private tSerialPort com;
        private msgString strlib;
        public byte ID { get; private set; }

        public bool isConnected { get; private set; }

        //執行逾時計時器
        private System.Timers.Timer timeout;
        public int timeOutTime { get; private set; }

        public lightController(tSerialPort sPort, byte id, int time=1000)
        {
			if(false)
			{
				this.isConnected = true;
			}
			else
			{

				strlib = new msgString();

				this.com = sPort;
				this.timeout = new System.Timers.Timer(time);

				if (sPort == null)
					throw new Exception(strlib.err(7));

				try
				{
					this.com.Open();
				}
				catch
				{
					throw new Exception(strlib.err(8));
				}
				if (!this.com.IsOpen)
					throw new Exception(strlib.err(8));


				this.ID = id;
                //this.isConnected = false;
                //this.checkExist();
                this.isConnected = true;

            }
			
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if(disposing)
            {

                if (this.com != null)
                {
                    if (this.com.IsOpen)
                        this.com.Close();

                    this.com.Dispose();
                }
            }

            disposed = true;
        }

        ~lightController()
        {
            Dispose(false);
        }

        private void queryHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.timeout.Stop();
            this.com.DataReceived -= queryHandler;

            tSerialPort p = (tSerialPort)sender;
            byte[] response = new byte[p.BytesToRead];
            p.Read(response, 0, response.Length);

            byte[] okRespnse = new byte[] { 0xcd, 0xdc, 0x03, 0x52, (byte)'o', (byte)'k', (byte)'.' };
            if (response.Length<6)
            {
                this.isConnected = false;
                return;
            }
                
            for(int i=0; i<6; ++i)
            {
                if(response[i]!=okRespnse[i])
                {
                    this.isConnected = false;
                    return;
                }
            }

            this.isConnected = true;
        }

        //private void timeoutHandler(object sender,)

        public void setLevel(byte ch, byte level)
        {
            if (this.isConnected == false)
                return;

            byte[] message = new byte[] { 0xab, 0xba, 0x05, 0x11, this.ID, 0x31, ch, level };
            this.com.Write(message, 0, message.Length);
        }

        public void checkExist()
        {
            try
            {
                this.com.DataReceived += queryHandler;
                byte[] message= new byte[] { 0xab, 0xba, 0x03, 0x10, 0xfc, this.ID };
                this.com.Write(message, 0, message.Length);
                this.timeout.Start();
            }
            catch
            {
                throw new Exception(strlib.err(9));
            }

        }

        //light on/off

        //set light level

        //set lock

        /// <summary>
        /// 處理逾時事件
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timeout.Stop();
            this.onTimeout(e);
        }

        /// <summary>
        /// 執行逾時事件處理器
        /// </summary>
        public event EventHandler onTimeout_Handler;
        /// <summary>
        /// 發佈執行逾時事件
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void onTimeout(EventArgs e)
        {
            EventHandler handler = this.onTimeout_Handler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
