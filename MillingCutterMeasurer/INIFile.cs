using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;

namespace Ini
{
    /// <summary>
    /// Class to read from INI files. Based loosely on the Delphi class of the same name.
    /// </summary>
    public class IniFile
    {
        private string fileName;

        /// <summary>
        /// Creates a new <see cref="IniFile"/> instance.
        /// </summary>
        /// <param name="fileName">Name of the INI file.</param>
        public IniFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName + " does not exist", fileName);
            this.fileName = fileName;
        }

        // native methods
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
          string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section, IntPtr lpReturnedString,
          int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        /// <summary>
        /// Reads a string value from the INI file.
        /// </summary>
        /// <param name="section">Section to read.</param>
        /// <param name="key">Key to read.</param>
        public string read(string section, string key)
        {
            const int bufferSize = 255;
            StringBuilder temp = new StringBuilder(bufferSize);
            GetPrivateProfileString(section, key, "", temp, bufferSize, fileName);
            return temp.ToString();
        }

        /// <summary>
        /// read as boolean
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool readBool(string section, string key)
        {
            string tStr = this.read(section, key);
            if (tStr == null || tStr.Length < 1)
                return false;

            return (tStr == "1");
        }

        public bool writeBool(string section, string key, bool value)
        {
            try
            {
                write(section, key, (value) ? "1" : "0");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// read integer 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int? readInt(string section, string key)
        {
            string tStr = this.read(section, key);
            int v;
            if(int.TryParse(tStr,out v))
            {
                return v;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Reads a whole section of the INI file.
        /// </summary>
        /// <param name="section">Section to read.</param>
        public string[] ReadSection(string section)
        {
            const int bufferSize = 2048;

            StringBuilder returnedString = new StringBuilder();

            IntPtr pReturnedString = Marshal.AllocCoTaskMem(bufferSize);
            try
            {
                int bytesReturned = GetPrivateProfileSection(section, pReturnedString, bufferSize, fileName);

                //bytesReturned -1 to remove trailing \0
                for (int i = 0; i < bytesReturned - 1; i++)
                    returnedString.Append((char)Marshal.ReadByte(new IntPtr((uint)pReturnedString + (uint)i)));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pReturnedString);
            }

            string sectionData = returnedString.ToString();
            return sectionData.Split('\0');
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.fileName);
        }

    }
}

/*/
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key,string def, String retVal,
            int size,string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section,string Key,string Value)
        {
            WritePrivateProfileString(Section,Key,Value,this.path);
        }
        
        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section,string Key)
        {
            String temp = new String('\0',255);
            int i = GetPrivateProfileString(Section,Key,"",temp, 
                                            255, this.path);
            return temp.ToString();

        }

        ///<summary>
        ///读取指定区域Keys列表。
        ///</summary>
        ///<paramname="Section"></param>
        ///<paramname="Strings"></param>
        ///<returns></returns>
        public List<string> ReadSingleSection(string Section)
        {
            List<string>result=new List<string>();
            String buf = new String('\0',65536);
            int lenf = GetPrivateProfileString(  Section,  null, "aardvark", buf,buf.Length, path);
            List<string> rtn = new List<string>(buf.Split('\0'));
            int j=0;
            for(int i=0; i<lenf; i++)
                if(buf[i]==0)
                {
                    result.Add(buf.ToString());
                    j=i+1;
                }
            return result;
        }
    }
}
/*/