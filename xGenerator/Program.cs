using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MillingCutterPtp;
using System.Windows.Forms;

namespace xGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args[0]=="-es" && args.Length>1 && args[1].Length>0)
			{ 
				try
				{
					MillingCutterMeasurer.crypto.writeToFile(
						MillingCutterPtp.frmMain.setupPassFile
						, args[1]
						, MillingCutterPtp.frmMain.encryptoKey1
						, MillingCutterPtp.frmMain.encryptoKey2);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Failed to encrypt: \n"+ex.Message);
                }
            }
			else if (args[0] == "-eg" && args.Length > 1 && args[1].Length > 0)
			{
				try
				{
					MillingCutterMeasurer.crypto.writeToFile(
						MillingCutterPtp.frmMain.EngneerPassFile
						, args[1]
						, MillingCutterPtp.frmMain.encryptoKey1
						, MillingCutterPtp.frmMain.encryptoKey2);
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("Failed to encrypt: \n" + ex.Message);
				}
			}
			else
			{
				{
					Console.Out.WriteLine(" Generate setup password\n" + "Generate -es passwordString\n");
					Console.Out.WriteLine(" Generate enginnering password\n" + "Generate -eg passwordString\n");

					return;
				}

				return;
			}
		}
	}
}
