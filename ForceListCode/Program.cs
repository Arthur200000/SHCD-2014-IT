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

		public static int Main (string[] args)
		{
			ulong start;
			ulong end;
			if (args [0] == "/?") {
				HelpText ();
				return 0;
			}
			if (args [0] == "/v") {
				try {
					start = Convert.ToUInt64 (args [1]);
					end = Convert.ToUInt64 (args [2]);
				} catch {
					Console.WriteLine ("FATAL: Invalid Input.");
					HelpText ();
					return 1;
				}
				if (start >= 1e16 || end >= 1e16 || start > end) {
					Console.WriteLine ("FATAL: Invalid Input.\nMake sure your StartNum and EndNum is between 0 and 1e16-1,\nand StartNum is smaller then EndNum.");
					HelpText ();
					return 1;
				}
				return 0;
			}
			return 0;
		}
	}
}
