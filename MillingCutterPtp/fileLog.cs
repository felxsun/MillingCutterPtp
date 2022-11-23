using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MillingCutterPtp
{
    public class fileLog
    {
        static object lockMe = new object();

        public string logFile { get; private set; }
        public bool doFileLog { get; private set; }
		public bool doOpLog = false;

        private Queue<string> messageQueue;
        private string dateTimeFormat;
        private string timeFormat;
        public int queueLimit { get; private set; }

        public fileLog(int queueSize=50, string file=null)
        {
            this.dateTimeFormat = "yyyy/MM/dd - HH:mm:ss";
            this.timeFormat = "HH:mm:ss";

            //initial queue
            if (queueSize < 0)
            {
                throw new ArgumentOutOfRangeException("queueSize < 0");
            }
            else if (queueSize == 0) 
            {
                messageQueue = null;
            }
            else
            {
                messageQueue = new Queue<string>(queueSize);
                this.queueLimit = queueSize;
            }
            
            //init file
            if(file==null || file=="")
            {
                this.doFileLog = false;
            }
            else
            {
                if (!File.Exists(file))
                {
                    try
                    {
                        File.WriteAllText(file, "Initial Log @ "+DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed on open log file (" + ex.Message + ")");
                    }
                }
                this.logFile = file;
                this.doFileLog = true;
            }
        }

        public void add(string message)
        {
			if (!doFileLog || !doOpLog)
				return;

				DateTime moment = DateTime.Now;

            //append to queue
            if (messageQueue != null)
                this.add2Queue(moment, message);
            //append to file
            if (doFileLog)
                this.add2File(moment, message);
        }

		public void errAdd(string message)
		{
			if (!doFileLog)
				return;

			DateTime moment = DateTime.Now;

			//append to queue
			if (messageQueue != null)
				this.add2Queue(moment, message);
			//append to file
			if (doFileLog)
				this.add2File(moment, message);
		}

		public void add2Queue(DateTime moment, string message)
        {
            if(messageQueue.Count>=this.queueLimit)
                this.messageQueue.Dequeue();

            this.messageQueue.Enqueue(moment.ToString(this.timeFormat) + "(" + moment.Millisecond.ToString() + ")" + "  " + message);
        }

        public void add2File(DateTime moment, string message)
        {
            fileLog.writeLog(this.logFile, moment.ToString(this.dateTimeFormat) + "  " + message + Environment.NewLine);
        }

        public static void writeLog(string file, string message)
        {
            lock(lockMe)
            {
                File.AppendAllText(file, message);
            }
        }

        public string queue()
        {
            if (this.messageQueue == null || this.messageQueue.Count<1)
                return "";

            string[] qArray = this.messageQueue.ToArray();
            string rtn = "";
            for (int i = qArray.Length - 1; i >= 0; --i)
                rtn += qArray[i] + Environment.NewLine;

            return rtn;
        }
    }
}
