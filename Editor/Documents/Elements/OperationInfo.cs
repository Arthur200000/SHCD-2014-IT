namespace Qisi.Editor.Documents.Elements
{
    using Microsoft.Win32;
    using Qisi.Editor.Controls;
    using Qisi.Editor.Properties;
    using Qisi.General;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class OperationInfo : Element
    {
        private List<MemoryFile> backupFileList;
        private Image buttonImage;
        private string dataPath;
        private PointF doButtonLocation;
        private Region doButtonRegion;
        private SizeF doButtonSize;
        private static string doButtonText1 = "点击按钮答题(本题未做)";
        private static string doButtonText2 = "点击按钮答题(本题已做)";
        private List<MemoryFile> fileList;
        private bool formDisposed;
        private FormFlow formFlow;
        private string gifpath;
        private Region imageRegion;
        private static SizeF imageSize = new SizeF(50f, 50f);
        private Process openFileProcess;
        private OperationType operationType;
        private bool processExited;
        private List<string> processList;
        private Hashtable processWnd;
        private PointF redoButtonLocation;
        private Region redoButtonRegion;
        private SizeF redoButtonSize;
        private static string redoButtonText = "点此重做";
        private string rtfpath;
        private string stdAnswer;
        private const int WM_CLOSE = 0x10;
        private const int WM_DESTORY = 2;
        private const int WM_QUIT = 0x12;

        internal OperationInfo(OperationType opType, Font font, string dirpath, string rtf, string gif) : base(font)
        {
            this.processWnd = null;
            this.OperationID = "1";
            this.dataPath = "";
            this.formFlow = null;
            this.openFileProcess = null;
            this.processList = null;
            this.rtfpath = rtf;
            this.gifpath = gif;
            this.operationType = opType;
            FontFamily fontFamily = font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(font.Style);
            this.BaseLine = (font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(font.Style));
            this.Sized = false;
            this.backupFileList = new List<MemoryFile>();
            this.fileList = new List<MemoryFile>();
            foreach (string str in Directory.GetFiles(dirpath))
            {
                this.backupFileList.Add(new MemoryFile(str));
                this.fileList.Add(new MemoryFile(str));
            }
            if (this.operationType == OperationType.Flash)
            {
                this.buttonImage = Resources.flash;
            }
            else if (this.operationType == OperationType.PhotoShop)
            {
                this.buttonImage = Resources.photoshop;
            }
            else if (this.operationType == OperationType.Access)
            {
                this.buttonImage = Resources.access;
            }
            else if (this.operationType == OperationType.VisualBasic)
            {
                this.buttonImage = Resources.vb;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.buttonImage != null)
                {
                    this.buttonImage.Dispose();
                }
                if (this.doButtonRegion != null)
                {
                    this.doButtonRegion.Dispose();
                }
                if (this.imageRegion != null)
                {
                    this.imageRegion.Dispose();
                }
                if (this.redoButtonRegion != null)
                {
                    this.redoButtonRegion.Dispose();
                }
            }
            if (this.buttonImage != null)
            {
                this.buttonImage = null;
            }
            if (this.doButtonRegion != null)
            {
                this.doButtonRegion = null;
            }
            if (this.imageRegion != null)
            {
                this.imageRegion = null;
            }
            if (this.redoButtonRegion != null)
            {
                this.redoButtonRegion = null;
            }
            base.Dispose(disposing);
        }

        internal void Do(Image stuImg, string stuInfo, string path, int examLeftTime, int tipTime, string stdanswer)
        {
            if (this.dataPath == "")
            {
                this.dataPath = path;
            }
            if (this.stdAnswer == "")
            {
                this.stdAnswer = stdanswer;
            }
            CommonMethods.ClearDirectory(this.dataPath);
            string openFile = "";
            foreach (MemoryFile file in this.fileList)
            {
                string str2 = Path.Combine(path, file.FileName);
                string extension = Path.GetExtension(str2);
                try
                {
                    File.WriteAllBytes(str2, file.FileByte);
                }
                catch
                {
                }
                if ((this.operationType == OperationType.Access) && (extension == ".mdb"))
                {
                    openFile = str2;
                }
                else if ((this.operationType == OperationType.Flash) && (extension == ".fla"))
                {
                    openFile = str2;
                }
                else if ((this.operationType == OperationType.PhotoShop) && (extension == ".psd"))
                {
                    openFile = str2;
                }
                else if ((this.operationType == OperationType.VisualBasic) && (extension == ".vbp"))
                {
                    openFile = str2;
                }
            }
            this.Open(stuImg, stuInfo, examLeftTime, tipTime, openFile);
        }

        internal override void Draw(Graphics g)
        {
            if (this.buttonImage != null)
            {
                base.Draw(g);
                g.DrawImage(this.buttonImage, new RectangleF(base.OutLocation, imageSize));
                if (!this.Review)
                {
                    StringFormat genericTypographic = StringFormat.GenericTypographic;
                    genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    if (this.Opened)
                    {
                        g.DrawString(doButtonText2, new Font(base.Font.Name, base.Font.Size, base.Font.Style | FontStyle.Bold, base.Font.Unit), SystemBrushes.ControlText, this.DoButtonLocation, genericTypographic);
                        g.DrawString(redoButtonText, new Font(base.Font.Name, base.Font.Size, base.Font.Style | FontStyle.Bold, base.Font.Unit), SystemBrushes.ControlText, this.ReDoButtonLocation, genericTypographic);
                    }
                    else
                    {
                        g.DrawString(doButtonText1, new Font(base.Font.Name, base.Font.Size, base.Font.Style | FontStyle.Bold, base.Font.Unit), SystemBrushes.ControlText, this.DoButtonLocation, genericTypographic);
                        g.DrawString(redoButtonText, new Font(base.Font.Name, base.Font.Size, base.Font.Style, base.Font.Unit), SystemBrushes.ControlText, this.ReDoButtonLocation, genericTypographic);
                    }
                    genericTypographic.Dispose();
                }
            }
        }

        internal override void DrawHighLight(Graphics g)
        {
            if (this.buttonImage != null)
            {
                base.Draw(g);
                g.DrawImage(this.buttonImage, new RectangleF(base.OutLocation, imageSize));
                if (!this.Review)
                {
                    StringFormat genericTypographic = StringFormat.GenericTypographic;
                    genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    if (this.Opened)
                    {
                        g.DrawString(doButtonText2, base.Font, SystemBrushes.HighlightText, this.DoButtonLocation, genericTypographic);
                        g.DrawString(redoButtonText, base.Font, SystemBrushes.HighlightText, this.ReDoButtonLocation, genericTypographic);
                    }
                    else
                    {
                        g.DrawString(doButtonText1, base.Font, SystemBrushes.HighlightText, this.DoButtonLocation, genericTypographic);
                        g.DrawString(redoButtonText, base.Font, SystemBrushes.HighlightText, this.ReDoButtonLocation, genericTypographic);
                    }
                    genericTypographic.Dispose();
                }
            }
        }

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);
        ~OperationInfo()
        {
            this.Dispose(false);
        }

        private static List<string> FindRegistryValue(RegistryKey myReg, string subkey, string item)
        {
            List<string> list = new List<string>();
            if ((subkey == "") && (myReg.GetValue(item) != null))
            {
                list.Add(myReg.GetValue(item).ToString());
            }
            RegistryKey key = myReg.OpenSubKey(subkey);
            if ((key == null) || (subkey == ""))
            {
                string[] subKeyNames = myReg.GetSubKeyNames();
                foreach (string str in subKeyNames)
                {
                    try
                    {
                        list.AddRange(FindRegistryValue(myReg.OpenSubKey(str), subkey, item));
                    }
                    catch
                    {
                    }
                }
                return list;
            }
            if (key.GetValue(item) == null)
            {
                list.AddRange(FindRegistryValue(key, "", item));
                return list;
            }
            list.Add(key.GetValue(item).ToString());
            return list;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private void Finish()
        {
            if (this.processList != null)
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (!this.processList.Contains(process.ProcessName))
                    {
                        try
                        {
                            if (process.Responding)
                            {
                                process.Kill();
                                process.WaitForExit();
                                Thread.Sleep(200);
                                for (int i = 0; !process.HasExited && (i < 5); i++)
                                {
                                    Thread.Sleep(200);
                                }
                                Thread.Sleep(200);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            IntPtr hWnd = FindWindow("CabinetWClass", null);
            if ((hWnd != IntPtr.Zero) && IsWindow(hWnd))
            {
                StringBuilder lpString = new StringBuilder();
                GetWindowText(hWnd, lpString, 0xff);
                if (lpString.ToString() == "考生目录")
                {
                    PostMessage(hWnd, 0x10, 0, 0);
                }
            }
            else
            {
                hWnd = IntPtr.Zero;
            }
            if (!this.Review)
            {
                this.fileList = new List<MemoryFile>();
                foreach (string str in Directory.GetFiles(this.dataPath))
                {
                    this.fileList.Add(new MemoryFile(str));
                }
                this.Opened = true;
            }
        }

        private void formFlow_Disposed(object sender, EventArgs e)
        {
            this.formDisposed = true;
            if (!this.processExited)
            {
                try
                {
                    this.openFileProcess.Kill();
                    this.openFileProcess.WaitForExit(0xbb8);
                }
                catch
                {
                }
            }
            else
            {
                this.Finish();
                if (base.DocumentContainer != null)
                {
                    base.DocumentContainer.OperateFinished(this);
                }
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        internal void LayOut(Graphics g)
        {
            this.imageRegion = new Region(new RectangleF(this.Location, imageSize));
            this.doButtonLocation = new PointF(this.Location.X, this.Location.Y + imageSize.Height);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            this.doButtonSize = g.MeasureString(doButtonText1, base.Font, 0, genericTypographic);
            this.redoButtonLocation = new PointF((this.Location.X + 20f) + this.doButtonSize.Width, this.Location.Y + imageSize.Height);
            this.doButtonRegion = new Region(new RectangleF(this.doButtonLocation, this.doButtonSize));
            this.redoButtonSize = g.MeasureString(redoButtonText, base.Font, 0, genericTypographic);
            this.redoButtonRegion = new Region(new RectangleF(this.redoButtonLocation, this.redoButtonSize));
            if (this.Review)
            {
                this.Size = imageSize;
            }
            else
            {
                this.Size = new SizeF(Math.Max(imageSize.Width, (this.doButtonSize.Width + this.redoButtonSize.Width) + 20f), imageSize.Height + Math.Max(this.doButtonSize.Height, this.redoButtonSize.Height));
            }
            genericTypographic.Dispose();
        }

        internal void LoadAnswer(List<byte[]> filebyets, List<string> filenames)
        {
            this.fileList = new List<MemoryFile>();
            this.backupFileList = new List<MemoryFile>();
            for (int i = 0; i < Math.Min(filebyets.Count, filenames.Count); i++)
            {
                this.fileList.Add(new MemoryFile(filenames[i], filebyets[i]));
                this.backupFileList.Add(new MemoryFile(filenames[i], filebyets[i]));
            }
        }

        private void Open(Image stuImg, string stuInfo, int examLeftTime, int tipTime, string openFile)
        {
            this.formDisposed = true;
            this.processExited = true;
            this.processList = new List<string>();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                this.processList.Add(process.ProcessName);
            }
            if (tipTime < 0)
            {
                this.formFlow = new FormFlow(stuImg, stuInfo, this.rtfpath, this.dataPath, examLeftTime, tipTime, this.stdAnswer, this.gifpath, 1);
            }
            else if (tipTime == 0)
            {
                this.formFlow = new FormFlow(stuImg, stuInfo, this.rtfpath, this.dataPath, examLeftTime, tipTime, this.stdAnswer, this.gifpath, 0);
            }
            else
            {
                this.formFlow = new FormFlow(stuImg, stuInfo, this.rtfpath, this.dataPath, examLeftTime, tipTime, this.stdAnswer, this.gifpath, -1);
            }
            this.formFlow.Disposed += new EventHandler(this.formFlow_Disposed);
            Process process2 = new Process {
                StartInfo = { FileName = "cmd.exe", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
            };
            process2.Start();
            process2.StandardInput.WriteLine("start  " + this.dataPath);
            bool flag = false;
            string[] strArray = new string[0];
            string[] strArray2 = new string[0];
            string[] strArray3 = new string[0];
            string str = "";
            if (this.operationType == OperationType.Access)
            {
                strArray = new string[] { @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office", @"SOFTWARE\Microsoft\Office" };
                strArray2 = new string[] { @"15.0\Access\InstallRoot", @"14.0\Access\InstallRoot", @"13.0\Access\InstallRoot", @"12.0\Access\InstallRoot", @"11.0\Access\InstallRoot", @"10.0\Access\InstallRoot", @"9.0\Access\InstallRoot" };
                strArray3 = new string[] { "Path", "Path", "Path", "Path", "Path", "Path", "Path" };
                str = "msaccess.exe";
            }
            else if (this.operationType == OperationType.Flash)
            {
                strArray = new string[] { @"SOFTWARE\Macromedia\Flash", @"SOFTWARE\Macromedia\Flash", @"SOFTWARE\Adobe\Flash", @"SOFTWARE\Adobe\Flash", @"SOFTWARE\Adobe\Flash", @"SOFTWARE\Adobe\Flash", @"SOFTWARE\Adobe\Flash", @"SOFTWARE\Macromedia\Flash" };
                strArray2 = new string[] { @"7\Installation", @"8\Installation", @"9.0\Installation", @"10.0\Installation", @"11.0\Installation", @"11.5\Installation", @"12\Installation", @"6\Installation" };
                strArray3 = new string[] { "InstallPath", "InstallPath", "InstallPath", "InstallPath", "InstallPath", "InstallPath", "InstallPath", "InstallPath" };
                str = "flash.exe";
            }
            else if (this.operationType == OperationType.PhotoShop)
            {
                strArray = new string[] { @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop", @"SOFTWARE\Adobe\Photoshop" };
                strArray2 = new string[] { "8.0", "9.0", "10.0", "11.0", "12.0", "55.0", "60.0" };
                strArray3 = new string[] { "ApplicationPath", "ApplicationPath", "ApplicationPath", "ApplicationPath", "ApplicationPath", "ApplicationPath", "ApplicationPath" };
                str = "photoshop.exe";
            }
            else if (this.operationType == OperationType.VisualBasic)
            {
                strArray = new string[] { @"SOFTWARE\Microsoft\VisualStudio" };
                strArray2 = new string[] { @"6.0\Setup\Microsoft Visual Basic" };
                strArray3 = new string[] { "ProductDir" };
                str = "VB6.exe";
            }
            for (int i = 0; i < strArray.Length; i++)
            {
                RegistryKey myReg = Registry.LocalMachine.OpenSubKey(strArray[i], false);
                if (myReg != null)
                {
                    List<string> list = FindRegistryValue(myReg, strArray2[i], strArray3[i]);
                    foreach (string str2 in list)
                    {
                        if (File.Exists((str2.EndsWith(@"\") ? str2 : (str2 + @"\")) + str))
                        {
                            this.openFileProcess = new Process();
                            this.openFileProcess.StartInfo.Arguments = "\"" + openFile + "\"";
                            this.openFileProcess.StartInfo.FileName = (str2.EndsWith(@"\") ? str2 : (str2 + @"\")) + str;
                            this.openFileProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                            flag = this.openFileProcess.Start();
                            Thread.Sleep(200);
                            if (!(!flag || this.openFileProcess.HasExited))
                            {
                                this.processExited = false;
                                SetWindowPos(this.openFileProcess.Handle, -2, 0, 0, 0, 0, 3);
                                this.openFileProcess.EnableRaisingEvents = true;
                                this.openFileProcess.Exited += new EventHandler(this.openFileProcess_Exited);
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        this.formDisposed = false;
                        this.formFlow.hasProcess = true;
                        this.formFlow.Show();
                        SetWindowPos(this.formFlow.Handle, -1, 0, 0, 0, 0, 3);
                        return;
                    }
                }
            }
            this.formFlow.Show();
        }

        private void openFileProcess_Exited(object sender, EventArgs e)
        {
            this.processExited = true;
            if (!this.formDisposed)
            {
                try
                {
                    this.pexit();
                }
                catch
                {
                }
            }
            else
            {
                this.Finish();
                if (base.DocumentContainer != null)
                {
                    base.DocumentContainer.OperateFinished(this);
                }
            }
        }

        private void pexit()
        {
            if ((this.formFlow != null) && !this.formFlow.IsDisposed)
            {
                if (this.formFlow.InvokeRequired)
                {
                    pexitcallback method = new pexitcallback(this.pexit);
                    try
                    {
                        this.formFlow.Invoke(method);
                    }
                    catch
                    {
                    }
                }
                else if (!((this.formFlow == null) || this.formFlow.IsDisposed))
                {
                    this.formFlow.Dispose();
                    this.formFlow = null;
                }
            }
        }

        [DllImport("User32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        internal void ReDo()
        {
            this.fileList = new List<MemoryFile>();
            foreach (MemoryFile file in this.backupFileList)
            {
                this.fileList.Add(new MemoryFile(file.FileName, file.FileByte));
            }
        }

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

        internal PointF DoButtonLocation
        {
            get
            {
                return this.doButtonLocation;
            }
        }

        internal Region DoButtonRegion
        {
            get
            {
                return this.doButtonRegion;
            }
        }

        internal Region ImageRegion
        {
            get
            {
                return this.imageRegion;
            }
        }

        internal override PointF Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                if (base.Location != value)
                {
                    base.Location = value;
                    this.imageRegion = new Region(new RectangleF(value, imageSize));
                    this.doButtonLocation = new PointF(value.X, value.Y + imageSize.Height);
                    this.redoButtonLocation = new PointF((value.X + 20f) + this.doButtonSize.Width, value.Y + imageSize.Height);
                    this.doButtonRegion = new Region(new RectangleF(this.doButtonLocation, this.doButtonSize));
                    this.redoButtonRegion = new Region(new RectangleF(this.redoButtonLocation, this.redoButtonSize));
                    this.Size = new SizeF(Math.Max(imageSize.Width, (this.doButtonSize.Width + this.redoButtonSize.Width) + 20f), imageSize.Height + Math.Max(this.doButtonSize.Height, this.redoButtonSize.Height));
                }
            }
        }

        internal bool Opened { get; set; }

        internal OperationType Operation
        {
            get
            {
                return this.operationType;
            }
            set
            {
                if (this.operationType != value)
                {
                    this.operationType = value;
                    if (this.operationType == OperationType.Flash)
                    {
                        this.buttonImage = Resources.flash;
                    }
                    else if (this.operationType == OperationType.PhotoShop)
                    {
                        this.buttonImage = Resources.photoshop;
                    }
                    else if (this.operationType == OperationType.Access)
                    {
                        this.buttonImage = Resources.access;
                    }
                    else if (this.operationType == OperationType.VisualBasic)
                    {
                        this.buttonImage = Resources.vb;
                    }
                }
            }
        }

        internal string OperationID { get; set; }

        internal PointF ReDoButtonLocation
        {
            get
            {
                return this.redoButtonLocation;
            }
        }

        internal Region ReDoButtonRegion
        {
            get
            {
                return this.redoButtonRegion;
            }
        }

        internal bool Review { get; set; }

        internal enum OperationType
        {
            Flash,
            PhotoShop,
            VisualBasic,
            Access
        }

        private delegate void pexitcallback();

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    }
}

