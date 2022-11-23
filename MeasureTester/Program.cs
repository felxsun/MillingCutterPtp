using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureTester
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmEngineer frmEng = new frmEngineer();
            frmEng.standalone = true;
            frmEng.camera = null;
            frmEng.cameraSetting = "";

            //for fast debug
            //frmEng.forceMeasure = Application.StartupPath + "\\0_75726a4c-82e1-42d9-ac9f-66cebf31d3fc.bmp";
            Application.Run(frmEng);
        }
    }
}
