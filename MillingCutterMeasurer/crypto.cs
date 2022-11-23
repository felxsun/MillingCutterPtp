using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace MillingCutterMeasurer
{
	public class crypto
	{
		static public byte[] encryptString(string encKey1, string encKey2, string value)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			byte[] source = Encoding.ASCII.GetBytes(value);
			byte[] rtn;
			try
			{
				des.Key = Encoding.ASCII.GetBytes(encKey1);
				des.IV = Encoding.ASCII.GetBytes(encKey2);
				using (MemoryStream ms = new MemoryStream())
				using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(source, 0, source.Length);
					cs.FlushFinalBlock();
					//rtn = Convert.ToBase64String(ms.ToArray());
					rtn = ms.ToArray();
				}
			}
			catch (Exception ex)
			{
				rtn = null;
			}

			return rtn;
		}

		static public byte[] decryptString(string encKey1, string encKey2, string encryptString)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();

			try
			{
				des.Key = Encoding.ASCII.GetBytes(encKey1);
				des.IV = Encoding.ASCII.GetBytes(encKey2);
				byte[] source = Convert.FromBase64String(encryptString);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(source, 0, source.Length);
						cs.FlushFinalBlock();
						//return Encoding.UTF8.GetString(ms.ToArray());
						return ms.ToArray();
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return null;
			}
		}

		static public byte[] decryptString(string encKey1, string encKey2, byte[] source)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();

			try
			{
				des.Key = Encoding.ASCII.GetBytes(encKey1);
				des.IV = Encoding.ASCII.GetBytes(encKey2);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(source, 0, source.Length);
						cs.FlushFinalBlock();
						//return Encoding.UTF8.GetString(ms.ToArray());
						return ms.ToArray();
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return null;
			}
		}

		static public string readFromFile(string file, string encKey1, string encKey2)
		{
			if (!File.Exists(file))
			{
				Console.Error.WriteLine("Crypto file " + file + " is not exist");
				return null;
			}

			byte[] encString;
			try
			{
				encString=File.ReadAllBytes(file);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Failed on read encrypted file");
				return null;
			}

			byte[] desString = decryptString(encKey1, encKey2, encString);
			if (desString == null)
			{
				Console.Error.WriteLine("deccryption failed");
				return null;
			}

			return Encoding.UTF8.GetString(desString);

		}

		static public bool writeToFile(string file,string value, string encKey1, string encKey2)
		{
			//bool rtn = false;

			//if(!File.Exists(file))
			//{
			//	Console.Error.WriteLine("Crypto file " + file + " is not exist");
			//	return false;
			//}

			byte[] encString = encryptString(encKey1, encKey2, value);
			if(encString==null)
			{
				Console.Error.WriteLine("encryption failed");
				return false;
			}

			try
			{
				File.WriteAllBytes(file, encString);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Write Crypto file " + file + "failed");
				return false;
			}

			return true;

		}
	}
}
