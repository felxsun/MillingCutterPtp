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
    public class tSerialPort : SerialPort
    {
        private System.Timers.Timer timer;    //用以計算timeout

        /// <summary>
        /// 逾時事件
        /// </summary>
        public event EventHandler timeoutHandler; //timerout event

        public tSerialPort()
        {
            this.timer = new System.Timers.Timer(1);
            this.timer.Enabled = false;
            this.timer.Elapsed += timerHandler;  //掛上timeout event
        }

        /// <summary>
        /// 設置時限事件計時
        /// </summary>
        /// <param name="inteval">時限</param>
        public void setTimeout(int inteval) //<1==>timer off
        {
            if (inteval < 1)
            {
                timer.Stop();
            }
            else
            {
                this.timer.Interval = inteval;
                this.timer.Start();
            }
        }

        /// <summary>
        /// 逾時事件處理器
        /// <para>讓外部程式掛上timeout event</para>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        public void timerHandler(object sender, ElapsedEventArgs e)
        {
            EventHandler handler = timeoutHandler;
            if (null != handler) handler(this, ElapsedEventArgs.Empty);
        }
    }
}

