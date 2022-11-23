using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MillingCutterPtp
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
            //
            frmMain mainForm = new frmMain();
            if (mainForm.initialSuccess)
				Application.Run(mainForm);
            /*/
            settingPage frmSetting = new settingPage();

            frmSetting.simCamera = true;
            frmSetting.classMgr = classManager.fromFile(Application.StartupPath + "\\class1.csv");
            frmSetting.WindowState = FormWindowState.Normal;
            frmSetting.FormBorderStyle = FormBorderStyle.Sizable;
            frmSetting.Size = new System.Drawing.Size(1960, 1080);

            //without calibration
            frmSetting.calibImage = null;
            frmSetting.measureParas = new measureParameters();


            Application.Run(frmSetting);
			/**/
        }
    }
}

