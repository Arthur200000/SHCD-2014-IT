using System.Management;

namespace SHCD
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using Microsoft.Win32;
    using Qisi.General.Controls;

    internal static class Program
    {
        internal static string answerDir;
        internal static string dataDir;
        private static string defaultID = "19";
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        internal static string paperDir;
        internal static string tempAnswerDir;
        private static uint WM_SETTINGCHANGE = 0x1a;

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
				// 4*(17^4) = 334084
				if (intListCode[3] == (intVerify % 10)) // ListCode[3] = 4
                {
					for (i=3; i <= 4; i++) 
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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
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
				num += num3 * 36; // num += num3 * modBy100 (36, i);
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

        [STAThread, SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlAppDomain)]
        private static void Main()
        {
            bool flag;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Mutex mutex = new Mutex(false, @"Global\SHCD", out flag);
            if (!flag)
            {
                FlatMessageBox.Show("程序已经在运行！", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
            }
            else
            {
                string tempPath;
				//if (Environment.OSVersion.Version.Major >= 6)
				//{
				//    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
				//    if ((key != null) && (((int) key.GetValue("EnableLUA", 1)) != 0))
				//    {
				//        FlatMessageBox.Show("请在控制面板->用户帐户和家庭安全->用户帐户中将“用户账户控制设置”更改为“从不通知”，并重新启动后，再运行本程序。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
				//        return;
				//    }
				//}
                new StartForm { OpacityIncreaseMilliseconds = 500, OpacityDecreaseMilliseconds = 500, KeepOpacityMilliseconds = 0x3e8 }.ShowDialog();
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load("http://www.keys-edu.com/update/update.xml");
                    Version version = new Version(Application.ProductVersion);
                    string str = "";
                    foreach (XmlNode node in document.DocumentElement.ChildNodes)
                    {
                        if ((((node.Attributes["name"] != null) && (node.Attributes["version"] != null)) && (node.Attributes["url"] != null)) && (node.Attributes["name"].Value == "上海光盘2014"))
                        {
                            Version version2 = new Version(node.Attributes["version"].Value);
                            if (version2.CompareTo(version) > 0)
                            {
                                version = version2;
                                str = node.Attributes["url"].Value;
                            }
                        }
                    }
                    if ((str != "") && (FlatMessageBox.Show("练习光盘已发布最新版本，是否进行更新？", "提示", FlatMessageBox.KeysButtons.YesNo, FlatMessageBox.KeysIcon.Information) == DialogResult.Yes))
                    {
                        new Process { StartInfo = { FileName = Path.Combine(Application.StartupPath, "Update.exe"), Arguments = str, WindowStyle = ProcessWindowStyle.Normal } }.Start();
                        return;
                    }
                }
                catch
                {
                }
                try
                {
                    tempPath = Path.GetTempPath();
                }
                catch (SecurityException)
                {
                    FlatMessageBox.Show("当前用户没有权限读取临时文件夹！\r\n请切换到管理员账户或者尝试右击程序图标选择“以管理员身份运行”。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    return;
                }
                paperDir = Path.Combine(tempPath, "Qisi");
                if (!Directory.Exists(paperDir))
                {
                    Directory.CreateDirectory(paperDir);
                }
                paperDir = Path.Combine(paperDir, "Paper");
                if (!Directory.Exists(paperDir))
                {
                    Directory.CreateDirectory(paperDir);
                }
                dataDir = Path.Combine(tempPath, "Qisi");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                dataDir = Path.Combine(dataDir, "VirtualDir");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                tempAnswerDir = Path.Combine(tempPath, "Qisi");
                if (!Directory.Exists(tempAnswerDir))
                {
                    Directory.CreateDirectory(tempAnswerDir);
                }
                tempAnswerDir = Path.Combine(tempAnswerDir, "Answer");
                if (!Directory.Exists(tempAnswerDir))
                {
                    Directory.CreateDirectory(tempAnswerDir);
                }
                answerDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                if (!Directory.Exists(answerDir))
                {
                    string str3;
                    if (Environment.OSVersion.Version.Major >= 0)
                    {
                        str3 = "没有找到当前用户的“文档”文件夹。";
                    }
                    else
                    {
                        str3 = "没有找到当前用户的“我的文档”文件夹。";
                    }
                    FlatMessageBox.Show(str3, "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                }
                else
                {
                    Process process;
                    FormReg reg;
                    RegistryKey key2;
                    answerDir = Path.Combine(answerDir, "Qisi");
                    if (!Directory.Exists(answerDir))
                    {
                        Directory.CreateDirectory(answerDir);
                    }
					// REG ABSTRACT:
					// The base64 string of the Machine's only code is stored in %USERPROFILE%\\SHCD.ini.
					// When it is converted as ASCII, it meets the following requirements:
					// 0 1 2 3 ……………………………………………………… 15 16 17| 18 ……………… 25 | 26 … 33 | junk
					// ListCode, something like a product ID | Machine-Hash | Verify  | ignored
					// ListCode:
					// Starts with 14(actually 144), and the nth digits equals to
					// (Sum(x*(17^x), 1, n-1) mod 10) ,n={5, 9, 16} (Psuedo-CASIO fx-991es Sum :) )
					// Machine-Hash:
					// 
                    if (!File.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini"))
                    {
                        reg = new FormReg();
                        if (reg.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                    }
                    else
                    {
						string cfgPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini";
						string strCfgKey = "";
						bool unregistered = false;
                        try
                        {
                            strCfgKey = File.ReadAllText(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini");
                        }
                        catch
                        {
                            strCfgKey = "";
                        }
                        finally
                        {
							byte[] keyTmp = Convert.FromBase64String(strCfgKey);
                            strCfgKey = Encoding.ASCII.GetString(keyTmp);
                            if (strCfgKey.Length < 34)
                            {
                                unregistered = true;
                            }
                            else
                            {
                                int j;
								string listCode = strCfgKey.Substring (0, 18);
								string machineHash = strCfgKey.Substring (18, 8);
								string verify = strCfgKey.Substring (26, 8);
                                unregistered = !CheckListCode(listCode);
                                if (machineHash != (doString(getCpuId()) + doString(getBaseBoardId()) + doString(getBIOSId()) + doString(getPhysicalMediaId())))
                                {
                                    unregistered = true;
                                }
								int[] intMachineHash = new int[8];
								for (j = 0; j < 8; j++) // machineHash.Length = 8
                                {
                                    intMachineHash[j] = Convert.ToInt32(machineHash[j].ToString());
								}
                                Array.Sort<int>(intMachineHash);
								int machineHashVerify = 0;
								for (j = 0; j < 8; j++) // intMachineHash.Length = 8
                                {
                                    machineHashVerify += intMachineHash[j] * ((int) Math.Pow(10.0, (double) ((intMachineHash.Length - 1) - j)));
                                }
                                machineHashVerify = 100000000 - machineHashVerify;
                                long num5 = Convert.ToInt64(listCode.Substring(2)) % ((long) machineHashVerify);
								if (verify != num5.ToString().PadLeft(8, '0')) //
                                {
                                    unregistered = true;
                                }
                            }
                        }
                        if (unregistered)
                        {
                            reg = new FormReg();
                            if (reg.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }
                        }
                    }
                    string str9 = "";
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    DriveInfo info = new DriveInfo("C");
                    foreach (DriveInfo info2 in drives)
                    {
                        if (info2.RootDirectory.FullName == Path.GetPathRoot(dataDir))
                        {
                            info = info2;
                            break;
                        }
                    }
                    string volumeLabel = info.VolumeLabel;
                    for (char ch = 'Z'; ch >= 'A'; ch = (char) (ch - '\x0001'))
                    {
                        bool flag3 = false;
                        foreach (DriveInfo info2 in drives)
                        {
                            if (info2.Name.Contains(ch.ToString()))
                            {
                                flag3 = true;
                                break;
                            }
                        }
                        if (!flag3)
                        {
                            str9 = ch.ToString();
                            break;
                        }
                    }
                    int num3 = 0;
                    try
                    {
                        while (!Directory.Exists(str9 + @":\"))
                        {
                            Thread.Sleep(500);
                            process = new Process {
                                StartInfo = { FileName = "cmd.exe", LoadUserProfile = true, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
                            };
                            process.Start();
                            process.StandardInput.WriteLine("subst.exe " + str9 + ": \"" + dataDir + "\"");
                            num3++;
                            if (num3 == 10)
                            {
                                break;
                            }
                        }
                        if (num3 != 10)
                        {
                            info.VolumeLabel = "考生目录";
                            dataDir = str9 + @":\";
                        }
                        else
                        {
                            string destDirName = Path.Combine(Directory.GetParent(dataDir).FullName, "考生目录");
                            Directory.Move(dataDir, destDirName);
                            dataDir = destDirName;
                        }
                        if (!Directory.Exists(dataDir))
                        {
                            Directory.CreateDirectory(dataDir);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    byte[] buffer2 = new byte[4];
                    foreach (DriveInfo info2 in drives)
                    {
                        buffer2[(info2.Name.ToCharArray()[0] - 'A') / 8] = (byte) (buffer2[(info2.Name.ToCharArray()[0] - 'A') / 8] + ((byte) Math.Pow(2.0, (double) ((info2.Name.ToCharArray()[0] - 'A') % 8))));
                    }
                    try
                    {
                        key2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                        if (key2 == null)
                        {
                            Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", RegistryKeyPermissionCheck.ReadWriteSubTree);
                            key2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                        }
                        key2.SetValue("NoDrives", buffer2, RegistryValueKind.Binary);
                    }
                    catch
                    {
                    }
                    IntPtr result = new IntPtr();
                    SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_AbortIfHung | SendMessageTimeoutFlags.SMTO_Block, 0xbb8, out result);
                    Application.Run(new FormSel());
                    for (char ch2 = str9[0]; ch2 <= 'Z'; ch2 = (char) (ch2 + '\x0001'))
                    {
                        int num4 = 0;
                        while (Directory.Exists(ch2.ToString() + @":\"))
                        {
                            Thread.Sleep(500);
                            process = new Process {
                                StartInfo = { FileName = "cmd.exe", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
                            };
                            process.Start();
                            process.StandardInput.WriteLine("subst.exe  " + ch2.ToString() + ": /D");
                            num4++;
                            if (num4 == 10)
                            {
                                break;
                            }
                        }
                    }
                    info.VolumeLabel = volumeLabel;
                    key2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                    if (key2 == null)
                    {
                        Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", RegistryKeyPermissionCheck.ReadWriteSubTree);
                        key2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                    }
                    byte[] buffer3 = new byte[4];
                    key2.SetValue("NoDrives", buffer3, RegistryValueKind.Binary);
					SendMessageTimeout (HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_AbortIfHung | SendMessageTimeoutFlags.SMTO_Block, 3000, out result);
                }
            }
        }

        internal static int modBy100(int a, int e)
        {
            if (e > 1)
            {
				return (Program.modBy100(a, e - 1) % 100);
            }
            if (e == 1)
            {
                return (a % 100);
            }
            return 1;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr SendMessageTimeout(IntPtr windowHandle, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, uint timeout, out IntPtr result);
        private static void UIThreadException(object sender, ThreadExceptionEventArgs e)
        {
        }

        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_AbortIfHung = 2,
            SMTO_Block = 1,
            SMTO_Normal = 0,
            SMTO_NoTimeoutIfHung = 8
        }
    }
}

