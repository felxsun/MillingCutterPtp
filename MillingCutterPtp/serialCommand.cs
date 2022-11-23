using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingCutterPtp
{
    public class serialCommand
    {
        public int type { get; private set; }
        public string host { get; private set; }
        public string slave { get; private set; }
        public string command { get; private set; }
        public int wait { get; private set; }
        public int startAddr { get; private set; }
        public int length { get; private set; }
        public string[] data { get; private set; }
        public string messageBody { get; private set; }

        public serialCommand(int messageType, string message)
        {
            this.type = messageType;
            this.messageBody = this.type.ToString("00") + "H" + message;
            host = (message.Length < 2) ? null : message.Substring(0, 2);
            slave = (message.Length < 4) ? null : message.Substring(2, 2);
            command = (message.Length < 6) ? null : message.Substring(4, 2);
            if (message.Length < 7)
            {
                wait = -1;
            }
            else
            {
                int tWait;
                if (!int.TryParse(message.Substring(6, 1), out tWait))
                    throw new Exception("Failed on parsing wait");
                this.wait = tWait;
            }
            if (message.Length < 12)
            {
                startAddr = -1;
            }
            else
            {
                string sAddr = message.Substring(8, 4);
                int iAddr;
                if (!int.TryParse(sAddr, out iAddr))
                    throw new Exception("imcompatible receved address format");
                this.startAddr = iAddr;
            }

            if (message.Length < 14)
            {
                this.length = 0;
                this.data = null;
                return;
            }
            else
            {
                string slen = message.Substring(12, 2);
                int ilen;
                if (!int.TryParse(slen, out ilen))
                    throw new Exception("imcompatible receved length format");
                this.length = ilen;
            }

            this.data = new string[this.length];
            int cnt = 0;
            for (int i = 14; i < message.Length; i += 4)
            {
                this.data[cnt] = message.Substring(i, 4);
                ++cnt;
                if (cnt >= this.data.Length)
                    return;
            }
        }
    }
}
