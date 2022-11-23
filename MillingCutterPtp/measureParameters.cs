using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingCutterPtp
{


    public class measureParameters
    {
        public double calibrationFactor = 0;
		public double interception = 0;
        public int boxX = 1000;
        public int boxY = 550;
        public int boxWidth = 1100;
        public int boxHeight = 650;
        public int boxDistanceY = 650;
        public int baseGapX = 20;
        public int baseWidth = 50;
        public int searchHeight = 250;
        public int bladeShift = 104;
        public int bladeCheckWidth = 156;
        public int seekBorder = 5;
        public int smoothFlank = 5;

        public static measureParameters Create(Ini.IniFile ini)
        {
            measureParameters p = new measureParameters();

            double f;
            if(!double.TryParse(ini.read("Measurement", "calibration"),out f))
                throw new Exception("Faile on parse calibraton factor");
            p.calibrationFactor = f;


			if (!double.TryParse(ini.read("Measurement", "interception"), out f))
				throw new Exception("Faile on parse interception factor");
			p.interception = f;

			int? temp = ini.readInt("Measurement", "boxX");
            if (temp != null) p.boxX = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "boxY");
            if (temp != null) p.boxY = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "boxWidth");
            if (temp != null) p.boxWidth = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "boxHeight");
            if (temp != null) p.boxHeight = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "boxDistanceY");
            if (temp != null) p.boxDistanceY = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "baseGapX");
            if (temp != null) p.baseGapX = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "baseWidth");
            if (temp != null) p.baseWidth = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "searchHeight");
            if (temp != null) p.searchHeight = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "bladeShift");
            if (temp != null) p.bladeShift = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "bladeCheckWidth");
            if (temp != null) p.bladeCheckWidth = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "seekBorder");
            if (temp != null) p.seekBorder = temp.GetValueOrDefault(0);

            temp = ini.readInt("Measurement", "smoothFlank");
            if (temp != null) p.smoothFlank = temp.GetValueOrDefault(0);

            return p;

        }
    }
}
