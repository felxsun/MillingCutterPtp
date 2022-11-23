using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MillingCutterPtp
{
    public class millingCutterClass : ICloneable
    {
        //todo: check max/min
        public double maxHandleLength = double.NaN;
        public double minHandleLength = double.NaN;
        public double maxHandleWidth = double.NaN;
        public double minHandleWidth = double.NaN;
        public double maxLength = double.NaN;
        public double minLength = double.NaN;
        public double maxBladeWidth = double.NaN;
        public double minBladeWidth = double.NaN;

        public Guid guid = System.Guid.Empty;

        public millingCutterClass(string line = null)
        {
            if (line == null || line.Length < 1)
                return;

            string[] parts = line.Split(',');
            if (parts.Length != 8)
                throw new Exception("Invalid class string");

            double temp;
            if (parts[0] != "-")
            {
                if (!double.TryParse(parts[0], out temp))
                    throw new Exception("Invalid class string");
                this.maxHandleLength = temp;
            }

            if (parts[1] != "-")
            {
                if (!double.TryParse(parts[1], out temp))
                    throw new Exception("Invalid class string");
                this.minHandleLength = temp;
            }

            if (parts[2] != "-")
            {
                if (!double.TryParse(parts[2], out temp))
                    throw new Exception("Invalid class string");
                this.maxHandleWidth = temp;
            }

            if (parts[3] != "-")
            {
                if (!double.TryParse(parts[3], out temp))
                    throw new Exception("Invalid class string");
                this.minHandleWidth = temp;
            }

            if (parts[4] != "-")
            {
                if (!double.TryParse(parts[4], out temp))
                    throw new Exception("Invalid class string");
                this.maxLength = temp;
            }

            if (parts[5] != "-")
            {
                if (!double.TryParse(parts[5], out temp))
                    throw new Exception("Invalid class string");
                this.minLength = temp;
            }

            if (parts[6] != "-")
            {
                if (!double.TryParse(parts[6], out temp))
                    throw new Exception("Invalid class string");
                this.maxBladeWidth = temp;
            }

            if (parts[7] != "-")
            {
                if (!double.TryParse(parts[7], out temp))
                    throw new Exception("Invalid class string");
                this.minBladeWidth = temp;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static millingCutterClass create(string line)
        {
            if (line == null || line.Length < 1)
                return null;

            millingCutterClass rtn;
            try
            {
                rtn = new millingCutterClass(line);
            }
            catch
            {
                return null;
            }

            return rtn;
        }

        /// <summary>
        /// Transfer class to string line in csv formate
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string rtn = "";
            rtn = (double.IsNaN(this.maxHandleLength)) ? "-" : this.maxHandleLength.ToString();

            rtn += ",";
            rtn += (double.IsNaN(this.minHandleLength)) ? "-" : this.minHandleLength.ToString();

            rtn += ",";
            rtn += (double.IsNaN(this.maxHandleWidth)) ? "-" : this.maxHandleWidth.ToString();
            rtn += ",";
            rtn += (double.IsNaN(this.minHandleWidth)) ? "-" : this.minHandleWidth.ToString();

            rtn += ",";
            rtn += (double.IsNaN(this.maxLength)) ? "-" : this.maxLength.ToString();
            rtn += ",";
            rtn += (double.IsNaN(this.minLength)) ? "-" : this.minLength.ToString();

            rtn += ",";
            rtn += (double.IsNaN(this.maxBladeWidth)) ? "-" : this.maxBladeWidth.ToString();
            rtn += ",";
            rtn += (double.IsNaN(this.minBladeWidth)) ? "-" : this.minBladeWidth.ToString();

            return rtn;
        }

        public bool Check(MillingCutterMeasurer.millingCutterMeasurer m, bool useBladeInnerWidth)
        {
            if (m == null)
                return false;

            double fullLen = m.getCalibrateLength("full_length");
            if (!double.IsNaN(this.maxLength))
            {
                if (double.IsNaN(fullLen) || fullLen > this.maxLength)
                    return false;
            }

            if (!double.IsNaN(this.minLength))
            {
                if (double.IsNaN(fullLen) || fullLen < this.minLength)
                    return false;
            }

            double HandLen = m.getCalibrateLength("handle_length");
            if (!double.IsNaN(this.maxHandleLength))
            {
                if (double.IsNaN(HandLen) || HandLen > this.maxHandleLength)
                    return false;
            }
            if (!double.IsNaN(this.minHandleLength))
            {
                if (double.IsNaN(HandLen) || HandLen < this.minHandleLength)
                    return false;
            }

            double HandWidth = m.getCalibrateLength("handle_width");
            if (!double.IsNaN(this.maxHandleWidth))
            {
                if (double.IsNaN(HandWidth) || HandWidth > this.maxHandleWidth)
                    return false;
            }
            if (!double.IsNaN(this.minHandleWidth))
            {
                if (double.IsNaN(HandWidth) || HandWidth < this.minHandleWidth)
                    return false;
            }

            double BladeWidth = (useBladeInnerWidth) ? m.getCalibrateLength("blade_width") : m.getCalibrateLength("blade_outer_width");
            if (!double.IsNaN(this.maxBladeWidth))
            {
                if (double.IsNaN(BladeWidth) || BladeWidth > this.maxBladeWidth)
                    return false;
            }
            if (!double.IsNaN(this.minBladeWidth))
            {
                if (double.IsNaN(BladeWidth) || BladeWidth < this.minBladeWidth)
                    return false;
            }

            return true;
        }
    }

    public class classBox : ICloneable
    {
        public const uint DEFAULTQTY = 4000;

        public Guid Class = Guid.Empty;
        public uint Quantity;
		public uint? cNumber;

		public classBox(uint quantity=DEFAULTQTY,uint? cNum=null, Guid? classId=null)
		{
			this.Quantity = quantity;
			this.cNumber = cNum;
			this.Class = classId.GetValueOrDefault(Guid.Empty);
		}

        public object Clone()
        {
            return this.MemberwiseClone();
        }

		static public classBox fromString(string line)
		{
			uint qty;
            uint cNum;

			string[] parts = line.Split('|');
			if (parts.Length != 2)
				throw new Exception("Invalid box unit string");

            if (!uint.TryParse(parts[1], out qty))
                throw new Exception("Invalid box quantity");

            if (parts[0]=="-")
                return new classBox(qty, null);

            if (!uint.TryParse(parts[0], out cNum))
                throw new Exception("Invalid box class id");

            return new classBox(qty, cNum);
		}
    }

    public class classManager : ICloneable
    {
        public const int MAXRAW = 15;
        public const int MAXBOX = 15; // number of boxes
        public const int EMPTYCLASS = 0;  

        public const int ANOTHERCLASS = 16;
        public const int ANOTHERBOX = 0;

        static public string OPTIONHEAD = "Options";
        static public string BOXHEAD = "bx:";

        static public string USEBLADEINNER = "useBladeInner";


        protected List<millingCutterClass> classList;
        public millingCutterClass[] classes { get { return (classList == null) ? null : classList.ToArray(); } }
        public int Count { get { return classList.Count; } }
        protected classBox[] boxes;

        public bool bladeUseInner = true;
        public bool isClassBoxMatch { get; private set; }

        public classBox[] getBoxes()
        {
            classBox[] rtn = new classBox[this.boxes.Length];
            Array.Copy(this.boxes, rtn, rtn.Length);
            return rtn;
        }

        public classManager()
        {
            this.classList = new List<millingCutterClass>();
            this.boxes = new classBox[MAXBOX + 1];
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static classManager fromFile(string file)
        {
            if (file == null || file.Length < 1)
                throw new Exception("Invalid file string");
            if (!File.Exists(file))
                throw new Exception("File is not exist");


            classManager rtn = new classManager();
            rtn.isClassBoxMatch = false;

            System.IO.StreamReader infile = new System.IO.StreamReader(file);
            string line;
            int count = 0;
            while ((line = infile.ReadLine()) != null)
            {
                if(Regex.IsMatch(line,classManager.BOXHEAD))
                {
					parseBoxLine(rtn.boxes,line);
				}
				else if(Regex.IsMatch(line, classManager.OPTIONHEAD))
                {
                    rtn.parseOptions(line);
                }
                else
				{
					millingCutterClass c = millingCutterClass.create(line);
                    if (c == null)
                    {
                        infile.Close();
                        throw new Exception("Failed on parse line #" + (count + 1).ToString());
                    }

                    if (rtn.Count >= classManager.MAXRAW)
                    {
                        infile.Close();
                        return rtn;
                    }

					rtn.Add(c);
					++count;
				}
                
            }
			infile.Close();

            //match class and boxes 
            if (!rtn.boxesMatchClasses())
                throw new Exception("分類表與集裝箱設定不相容");


            return rtn;
        }

		public bool isRecordsValid()
		{
			foreach(millingCutterClass mcc in this.classList)
			{
				if (!double.IsNaN(mcc.maxLength) && !double.IsNaN(mcc.minLength) && mcc.minLength > mcc.maxLength)
					return false;
				if (!double.IsNaN(mcc.maxHandleLength) && !double.IsNaN(mcc.minHandleLength) && mcc.minHandleLength > mcc.maxHandleLength)
					return false;
				if(!double.IsNaN(mcc.maxBladeWidth) && !double.IsNaN(mcc.minBladeWidth) && mcc.minBladeWidth>mcc.maxBladeWidth)
					return false;
				if (!double.IsNaN(mcc.maxHandleWidth) && !double.IsNaN(mcc.minHandleWidth) && mcc.minHandleWidth > mcc.maxHandleWidth)
					return false;
				bool hasClassDefinition = !double.IsNaN(mcc.maxLength)
					|| !double.IsNaN(mcc.minLength)
					|| !double.IsNaN(mcc.maxHandleLength)
					|| !double.IsNaN(mcc.minHandleLength)
					|| !double.IsNaN(mcc.maxBladeWidth)
					|| !double.IsNaN(mcc.minBladeWidth)
					|| !double.IsNaN(mcc.maxHandleWidth)
					|| !double.IsNaN(mcc.minHandleWidth);

				if(!hasClassDefinition || !boxesMatchClasses())
					return false;
			}

			return true;
		}

		public void toFile(string file)
		{
			if (this.classList.Count < 1)
				throw new Exception("沒有任何類別定義");

			string path = Path.GetDirectoryName(file);
			if (!Directory.Exists(path))
				throw new Exception("目錄不存在");



			List<string> lines = new List<string>();
			for (int i = 0; i < this.classList.Count; ++i)
				lines.Add(this.classList[i].ToString());

			string bxLine = classManager.BOXHEAD;
			for(int i=0; i<this.boxes.Length; ++i)
			{
				if (i > 0)
					bxLine += ",";
				string boxC = (this.boxes[i].Class == Guid.Empty | i>=(this.boxes.Length-1)) ? 
					ANOTHERCLASS.ToString() 
					: 
					(1+this.classfierIndexOf(this.boxes[i].Class)).ToString();

				bxLine += boxC + "|" + this.boxes[i].Quantity.ToString();
			}
			lines.Add(bxLine);

            //options
            string optionLine = classManager.OPTIONHEAD;
            optionLine += ",useBladeInner=" + ((this.bladeUseInner) ? "1" : "0");
            lines.Add(optionLine);


            try
			{
				File.WriteAllLines(file, lines);
			}
			catch
			{
				throw new Exception("存檔失敗");
			}
		}

        /// <summary>
        /// do match between boxes and classes
        /// </summary>
        /// <returns></returns>
        public bool boxesMatchClasses()
        {
            classBox[] tBoxes = new classBox[this.boxes.Length];
            Array.Copy(this.boxes, tBoxes, tBoxes.Length);

            bool hasAnotherBox = false;

            //match boxes to classes
            for(int i=0; i<=MAXBOX; ++i)
            {
                uint? cNumber = this.boxes[i].cNumber;
                if(cNumber==null)
                {
                    //allow box without assignment
                    tBoxes[i].Class = Guid.Empty;
                }
                else if(cNumber==ANOTHERCLASS)
                {
                    tBoxes[i].Class = Guid.Empty;
                    hasAnotherBox=true;
                }
                else if(cNumber<0 || cNumber>MAXBOX)  //0=anotherBox, mismatch
                {
                    return false;
                } 
                else
                    tBoxes[i].Class = this.classes[(int) cNumber-1].guid;
            }

            if (!hasAnotherBox)
                return false;

            //match classes
            for(int i=0; i<this.classList.Count; ++i )
                if (this.boxIndexOfClass(this.classList[i].guid).Length < 1)
                    return false;

            this.boxes = tBoxes;

            return true;
        }

        public int[] boxIndexOfClass(Guid cid)
        {
            List<int> rtn = new List<int>();
            for (int i = 0; i < this.boxes.Length; ++i)
                if (this.boxes[i].Class != Guid.Empty && this.boxes[i].Class == cid)
                    rtn.Add(i);

            return rtn.ToArray();
        }

        public int classfierIndexOf(Guid cid)
        {
            int rtn = -1;
            for (int i = 0; i < this.classList.Count; ++i)
                if (this.classList[i].guid == cid)
                    rtn = i;
            return rtn;
        }

        public millingCutterClass classfierOf(Guid cid)
        {
            for (int i = 0; i < this.classList.Count; ++i)
                if (this.classList[i].guid == cid)
                    return this.classList[i];
            return null;
        }

        private static void parseBoxLine(classBox[] boxes, string line)
		{
            string[] parts = line.Split(':')[1].Split(',');
            if (parts.Length != (MAXBOX + 1))
                throw new Exception("Invalid number of defined boxes");

            for (int i = 0; i <= MAXBOX; ++i)
                boxes[i] = classBox.fromString(parts[i]);

			//last (16類盒
			boxes[MAXBOX].Class = Guid.Empty;
			boxes[MAXBOX].cNumber = 16;
		}

        public void parseOptions(string line)
        {
            string[] parts = line.Split(',')[1].Split(',');
            foreach(string part in parts)
            {
                string[] pair = part.Split('=');

                if(pair[0]== USEBLADEINNER)
                {
                    this.bladeUseInner = (pair[1] == "1");
                }
            }
        }

        public bool Add(millingCutterClass c)
        {
            if (c == null || classList.Count >= MAXRAW) return false;
            c.guid = System.Guid.NewGuid();
            classList.Add(c);
            return true;
        }

        public int classify(MillingCutterMeasurer.millingCutterMeasurer m)
        {
            if (m == null)
                throw new Exception("Null millingCutterMeasurer");

            for (int i = 0; i < classList.Count; ++i)
            {
                if (classList[i].Check(m,this.bladeUseInner))
                    return i + 1;
            }

            return ANOTHERCLASS;
        }

        public bool moveForward(uint index)
        {
            int idx = (int)index;

            if (this.classList.Count < 1 || idx >= this.classList.Count)
                throw new IndexOutOfRangeException("index out of classList count");
            if (idx == 0) //at first
                return false;

            millingCutterClass m = this.classList[idx];
            this.classList.RemoveAt(idx);
            this.classList.Insert(idx - 1, m);
            return true;
        }

        public bool moveBackward(uint index)
        {
            int idx = (int)index;

            if (this.classList.Count < 1 || idx >= this.classList.Count)
                throw new IndexOutOfRangeException("index out of classList count");
            if (idx == this.classList.Count - 1)  //at last
                return false;

            millingCutterClass m = this.classList[idx];
            this.classList.RemoveAt(idx);
            this.classList.Insert(idx + 1, m);
            return true;
        }

        public void remove(uint index)
        {
            int idx = (int)index;
            if (this.classList.Count < 1 || idx >= this.classList.Count)
                throw new IndexOutOfRangeException("index out of classList count");

            this.classList.RemoveAt(idx);
        }

        public void setBoxClass(int index, Guid cid)
        {
            if (index < 0 || index >= this.boxes.Length)
                throw new ArgumentOutOfRangeException("box index out of range");

            int cIdx = this.classfierIndexOf(cid);
            if (cIdx<0)
            {
                this.boxes[index].Class = Guid.Empty;
                this.boxes[index].cNumber = null;
            }
            else
            {
                this.boxes[index].Class = cid;
                this.boxes[index].cNumber = (uint) cIdx + 1;
            }
                
        }

		public void setBox(int index, Guid cid,  uint qty)
		{
			if (index < 0 || index >= this.boxes.Length)
				throw new ArgumentOutOfRangeException("box index out of range");

			int cIdx = this.classfierIndexOf(cid);
			if (cIdx < 0)
			{
				this.boxes[index].Class = Guid.Empty;
				this.boxes[index].cNumber = null;
				this.boxes[index].Quantity = qty;
			}
			else
			{
				this.boxes[index].Class = cid;
				this.boxes[index].cNumber = (uint)cIdx + 1;
				this.boxes[index].Quantity = qty;
			}

		}

		public void setBoxQuantity(int index, uint qty)
		{
			if (index < 0 || index >= this.boxes.Length)
				throw new ArgumentOutOfRangeException("box index out of range");

			
			this.boxes[index].Quantity = qty;

		}
	}
}
