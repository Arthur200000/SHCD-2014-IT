using System;
using System.Windows.Forms;

namespace ForceListCode
{
	class MainClass
	{
		static void HelpText ()
		{
			Console.WriteLine ("ListCode Cracker for SHCD.\nUsage: {0} [/v] StartNum EndNum\nArguments:\n/?\tDisplays this help text.\n/v\tBe verbose.", Application.ExecutablePath);
		}

		internal static bool CheckListCode(string strListCode)
		{
			int i;
			int[] intListCode = new int[16];
			for (i = 0; i < strListCode.Length; i++) {
				intListCode[i] = Convert.ToInt32(strListCode[i].ToString());
			}
			int intVerify = 0;
			intVerify = Convert.ToInt32((double)((intListCode[0] * Math.Pow(17.0, (double)intListCode[2])) % 10.0));
			if (intListCode[1] == (intVerify % 10)) {
				for (i = 1; i <= 2; i++) {
					intVerify += Convert.ToInt32((double)((intListCode[i] * Math.Pow(17.0, (double)intListCode[i])) % 10.0));
				}
				if (intListCode[3] != (intVerify % 10)) {
					return false;
				}
				for (i = 2; i <= 6; i++) {
					intVerify += Convert.ToInt32((double)((intListCode[i] * Math.Pow(17.0, (double)intListCode[i])) % 10.0));
				}
				if (intListCode[7] != (intVerify % 10)) {
					return false;
				}
				for (i = 6; i <= 14; i++) {
					intVerify += Convert.ToInt32((double)((intListCode[i] * Math.Pow(17.0, (double)intListCode[i])) % 10.0));
				}
				if (intListCode[15] == (intVerify % 10)) {
					intVerify = 0;
				} else {
					return false;
				}
				return true;
			}
			return false;
		}

		public static int Main (string[] args)
		{
			long start;
			long end;

			//try {
				if (args [0] == "/?") {
					HelpText ();
					return 0;
				}
				if (args [0] == "/v") {
					//try {
						start = Convert.ToInt64 (args [1]);
						end = Convert.ToInt64 (args [2]);
					//} catch {
					//	Console.WriteLine ("FATAL: Invalid Input.");
					//	HelpText ();
					//	return 1;
					//}
					if (start >= 1e16 || end >= 1e16 || start > end) {
						Console.WriteLine ("FATAL: Invalid Input.\nMake sure your StartNum and EndNum is between 0 and 1e16-1,\nand StartNum is smaller then EndNum.");
						HelpText ();
						return 1;
					}
					int j;
					long[] percent = new long[101];
					for (j = -1; j <= 100; j++) {
						// Number Overflow.
						percent[j] = Convert.ToInt64((double) start +  ((double) end - (double) start) * (double) j / 100);
						Console.WriteLine(Convert.ToString(j) + Convert.ToString(percent[j]));
					}
					return 0;
				}
				return 0;
			//} catch {
			//	HelpText();
			//	return 1;
			//}
		}
	}
}
