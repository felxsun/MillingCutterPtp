using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingCutterPtp
{
    public class serialRespons
    {
        public int Type { get; private set; }
        public string body { get; private set; }
        public bool isLegal { get; private set; }

        public bool isACK { get { return (this.Type == 6); } }
        public bool isNAK { get { return (this.Type == 15); } }
        public bool isSTX { get { return (this.Type == 2); } }
        
        public string host { get; private set; }
        public string slave { get; private set; }
        public string[] data { get; private set; }

        public serialRespons(int type, string message)
        {
            this.Type = type;
            this.body = message;
            this.isLegal = false;
            char[] c= message.ToArray();
            if (c == null 
                || c.Length < 1 
                || ((c.Length - 1) % 4) != 0 
                || c.Last() != ((char)03)
                || c.Length<5
                )
                return;
            this.host = message.Substring(0, 2);
            this.slave = message.Substring(2, 2);
            
            if(c.Length>5)
                initialData(message.Substring(4,message.Length-5).ToArray());
            this.isLegal = true;
        }

        private void initialData(char[] raw)
        {
            int len=(raw.Length)/4;
            this.data = new string[len];

            int pos = 0;
            for(int i=0; i<len; ++i)
            {
                char[] tData = new char[4];
                for(int j=0; j<tData.Length; ++j)
                {
                    tData[j] = raw[pos];
                    ++pos;
                }
                this.data[i] = new string(tData);
            }
        }
    }
}
