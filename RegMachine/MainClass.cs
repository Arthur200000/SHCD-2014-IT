using System;
using System.IO;
using System.Text;
using System.Management;

namespace RegMachine
{
	class MainClass
	{
		private static string defaultID = "19";

		internal static int modBy100(int a, int e)
		{
			if (e >= 1) {
				return e % 100;
			} else {
				return 1;
			}
		}

		internal static bool CheckListCode(string strListCode)
		{
			if (strListCode.Length == 18)
			{
				int i;
				if (!strListCode.StartsWith("14"))
				{
					return false;
				}
				int[] intListCode = new int[18];
				for (i = 0; i < strListCode.Length; i++)
				{
					intListCode[i] = Convert.ToInt32(strListCode[i].ToString());
				}
				int intVerify = 0;
				intVerify = Convert.ToInt32((double) ((intListCode[2] * Math.Pow(17.0, (double) intListCode[2])) % 10.0));
				if (intListCode[3] == (intVerify % 10))
				{
					for (i = 3; i <= 4; i++) 
					{
						intVerify += Convert.ToInt32((double) ((intListCode[i] * Math.Pow(17.0, (double) intListCode[i])) % 10.0));
					}
					if (intListCode[5] == (intVerify % 10))
					{
						intVerify = 0;
					}
					else
					{
						return false;
					}
					for (i = 2; i <= 8; i++)
					{
						intVerify += Convert.ToInt32((double) ((intListCode[i] * Math.Pow(17.0, (double) intListCode[i])) % 10.0));
					}
					if (intListCode[9] == (intVerify % 10))
					{
						intVerify = 0;
					}
					else
					{
						return false;
					}
					for (i = 2; i <= 16; i++)
					{
						intVerify += Convert.ToInt32((double) ((intListCode[i] * Math.Pow(17.0, (double) intListCode[i])) % 10.0));
					}
					if (intListCode [17] == (intVerify % 10))
					{
						intVerify = 0;
					}
					else
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		internal static string doString(string stringDoing)
		{
			string strUpperDoing = "";
			foreach (char ch in stringDoing.ToCharArray())
			{
				// Filters string to be limited inside [a-zA-Z0-9]
				if ((((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z'))) || ((ch >= '0') && (ch <= '9')))
				{
					strUpperDoing += ch.ToString();
				}
			}
			strUpperDoing = strUpperDoing.ToUpper(); // Change to UPPERCASE
			int num = 0;
			for (int i = 0; i < strUpperDoing.Length; i++)
			{
				int num3;
				if (strUpperDoing[i] > 'A')
				{
					num3 = (strUpperDoing[i] - 'A') + 10;
				}
				else
				{
					num3 = strUpperDoing[i] - '0';
				}
				num += num3 * modBy100 (36, i); // i = 0 -> modBy100 returns 1, and i > 0 -> modBy100(a,b) returns a % 100
				num = num % 100;
			}
			return num.ToString().PadLeft(2, '0');
		}

		internal static string getBaseBoardId()
		{
			try
			{
				ManagementObjectCollection instances = new ManagementClass("Win32_BaseBoard").GetInstances();
				foreach (ManagementObject obj2 in instances)
				{
					return obj2.Properties["SerialNumber"].Value.ToString();
				}
				return defaultID;
			}
			catch
			{
				return defaultID;
			}
		}

		internal static string getBIOSId()
		{
			try
			{
				ManagementObjectCollection instances = new ManagementClass("Win32_BIOS").GetInstances();
				foreach (ManagementObject obj2 in instances)
				{
					return obj2.Properties["SerialNumber"].Value.ToString();
				}
				return defaultID;
			}
			catch
			{
				return defaultID;
			}
		}

		internal static string getCpuId()
		{
			try
			{
				ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances();
				foreach (ManagementObject obj2 in instances)
				{
					return obj2.Properties["processorid"].Value.ToString();
				}
				return defaultID;
			}
			catch
			{
				return defaultID;
			}
		}

		internal static string getPhysicalMediaId()
		{
			try
			{
				ManagementObjectCollection instances = new ManagementClass("Win32_PhysicalMedia").GetInstances();
				foreach (ManagementObject obj2 in instances)
				{
					return obj2.Properties["SerialNumber"].Value.ToString();
				}
				return defaultID;
			}
			catch
			{
				return defaultID;
			}
		}

		static string GenerateMachineCode ()
		{
			return (doString (getCpuId ()) + doString (getBaseBoardId ()) + doString (getBIOSId ()) + doString (getPhysicalMediaId ())); 
		}

		static string CreateMachCodeVerify (string listcodeverify, string machineHash)
		{
			int[] intMachineHash = new int[8];
			int j;
			for (j = 0; j < 8; j++) // machineHash.Length = 8
			{
				intMachineHash[j] = Convert.ToInt32(machineHash[j].ToString());
			}
			Array.Sort<int>(intMachineHash);
			int machineHashVerify = 0;
			for (j = 0; j < 8; j++) // intMachineHash.Length = 8
			{
				machineHashVerify += intMachineHash[j] * ((int) Math.Pow(10.0, (double) (7 - j))); // intMachineHash.Length = 8
			}
			machineHashVerify = 100000000 - machineHashVerify;
			long num5 = Convert.ToInt64(listcodeverify.Substring(2)) % ((long) machineHashVerify);
			return num5.ToString().PadLeft(8, '0');
		}
			

		// internal string ChooseListCode (string[] listcodesinput)
		// {
		// 	int rannum;
		// 	rannum = System.Random.Next (0, (listcodesinput.Length - 1));
		// 	string listcodeout = listcodesinput [rannum];
		//  return listcodeout;
		// }

		public static void Main (string[] args)
		{
			Console.WriteLine ("CLI RegMachine for SHCD-2014\nBy Arthur200000, Released under GPLv2.\nSource Code Available at https://github.com/Arthur200000/SHCD-2014-IT.");
			string machCode = GenerateMachineCode ();
			string listCode = "";
			Console.WriteLine ("Using {0} as machine code.", machCode);
			if (false) // (File.Exists(".\\ListCodes.txt"))
			{
				// string[] fileListCodes = File.ReadAllLines(".\\ListCodes.txt");
				// listCode = ChooseListCode(fileListCodes);
				// Console.WriteLine ("Using {0} as ListCode.", listCode);
			}
			else
			{
				Console.WriteLine ("Err, Do you have a CD Key?\nIf you have one, then enter this below, else just press enter.");
				listCode = Console.ReadLine ();
			}
			if (!CheckListCode(listCode))
			{
				Console.WriteLine ("Well, it seems that there\'s something wrong with your ListCode...\nUsing Defaults.");
				listCode = "145593741419389738";
			}
			string machineCodeVerify = CreateMachCodeVerify (listCode, machCode);
			File.WriteAllText((Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini"), Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(listCode + machCode + machineCodeVerify)));
			Console.WriteLine (Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes (listCode + machCode + machineCodeVerify)));
		}
	}
}
