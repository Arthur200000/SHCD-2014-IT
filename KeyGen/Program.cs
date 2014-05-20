using System;
using System.Management;

namespace KeyGen
{
	class MainClass
	{
		private static string defaultID = "19";

		internal static int RecursiveMod(int Number, int Times) // Do 'a mod 100' for e times, using the last return value as a respectively.
		{
			if (Times > 1)
			{
				return (RecursiveMod(Number, Times - 1) % 100);
			}
			if (Times == 1)
			{
				return (Number % 100);
			}
			return 1;
		}

		internal static int pow(int a, int e)
		{
			if (e > 1)
			{
				return (MainClass.pow(a, e - 1) % 100);
			}
			if (e == 1)
			{
				return (a % 100);
			}
			return 1;
		}

		internal static bool CheckListCode(string mylistcode)
		{
			if (mylistcode.Length == 18)
			{
				int num;
				if (!mylistcode.StartsWith("14")) // Must starts with 14. 
				{
					return false;
				}
				int[] numArray = new int[18];
				for (num = 0; num < mylistcode.Length; num++)
				{
					numArray[num] = Convert.ToInt32(mylistcode[num].ToString());
				}
				int num2 = 0;
				num2 = Convert.ToInt32((double) ((numArray[2] * Math.Pow(17.0, (double) numArray[2])) % 10.0)); // 2*17^2
				if (numArray[3] == (num2 % 10)) // 289 mod 10
				{
					num2 = 0;
					for (num=2; num <= 4; num++) 
					{
						num2 += Convert.ToInt32((double) ((numArray[num] * Math.Pow(17.0, (double) numArray[num])) % 10.0));
					}
					if (numArray[5] == (num2 % 10))
					{
						num2 = 0;
					}
					else
					{
						return false;
					}
					for (num = 2; num <= 8; num++)
					{
						num2 += Convert.ToInt32((double) ((numArray[num] * Math.Pow(17.0, (double) numArray[num])) % 10.0));
					}
					if (numArray[9] == (num2 % 10))
					{
						num2 = 0;
					}
					else
					{
						return false;
					}
					for (num = 2; num <= 16; num++)
					{
						num2 += Convert.ToInt32((double) ((numArray[num] * Math.Pow(17.0, (double) numArray[num])) % 10.0));
					}
					if (numArray [17] == (num2 % 10))
					{
						num2 = 0;
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

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		}

		internal static string doString(string str)
		{
			string str2 = "";
			foreach (char ch in str.ToCharArray())
			{
				if ((((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z'))) || ((ch >= '0') && (ch <= '9')))
				{
					str2 = str2 + ch.ToString();
				}
			}
			str2 = str2.ToUpper();
			int num = 0;
			for (int i = 0; i < str2.Length; i++)
			{
				int num3;
				if (str2[i] > 'A')
				{
					num3 = (str2[i] - 'A') + 10;
				}
				else
				{
					num3 = str2[i] - '0';
				}
				num += num3 * RecursiveMod (36, i);
				num = num % 100;
			}
			return num.ToString().PadLeft(2, '0');
		}

		// get*ID(): get something's ID, returns 19 by default.
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

		static void GenerateMachineCode ()
		{
			throw new NotImplementedException ();
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("CLI KeyGen for SHCD-2014");
			Console.Write ("If you have a machine code, enter from below:\n");
			// string machcode = Console.ReadLine ();
			// if (machcode.Length < 18) {
			// 	GenerateMachineCode ();
			// 	Console.WriteLine ("Invalid machcode detected. Generating one.");
			// }
			string foo;
			Console.WriteLine ();
			// Console.WriteLine ("Using {0}.", machcode);
			// CheckListCode (machcode);
			Console.WriteLine (string.Format ("pow({0}, {1})={2}", args [0], args [1], Convert.ToString(pow (Convert.ToInt32 (args[0]), Convert.ToInt32 (args[1])))));
		}
	}
}
