namespace Qisi.General.Controls
{
    using Qisi.General.Controls.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public class imeBar : Control
    {
        private string _CurrentImeHandleStr = "";
        private ComboBox comboBox1;
        private IContainer components;
        private Form container;
        private const int DONT_RESOLVE_DLL_REFERENCES = 1;
        private static readonly int IMAGE_ICON = 1;
        private List<imeItem> imeList;
        private Label label1;
        private const int LOAD_LIBRARY_AS_DATAFILE = 2;
        private const int LOAD_WITH_ALTERED_SEARCH_PATH = 8;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private ComponentResourceManager resources = new ComponentResourceManager(typeof(Form));
        private const int RT_BITMAP = 2;
        private const int RT_GROUP_ICON = 14;
        private const int RT_ICON = 3;
        private Timer timer1;

        public imeBar()
        {
            this.InitializeComponent();
            this.InitMenus();
            this.timer1.Enabled = true;
        }

        private int Calc(int iMode)
        {
            if (iMode >= 0)
            {
                int num = 11;
                while ((iMode - this.Pow(2, num)) >= 0)
                {
                    num++;
                }
                if (num != 11)
                {
                    iMode -= this.Pow(2, num - 1);
                }
                return iMode;
            }
            iMode = (iMode + 0x7fffffff) + 1;
            int y = 11;
            while ((iMode - this.Pow(2, y)) >= 0)
            {
                y++;
            }
            if (y != 11)
            {
                iMode -= this.Pow(2, y - 1);
            }
            iMode = (iMode - 0x7fffffff) - 1;
            return iMode;
        }

        public void ChangeIme()
        {
            this._CurrentImeHandleStr = ImmGetContext(GetFocus()).ToString();
            foreach (imeItem item in this.imeList)
            {
                if (item.Tag.Handle.ToString() == this._CurrentImeHandleStr)
                {
                    this.comboBox1.SelectedIndex = this.imeList.IndexOf(item);
                }
            }
            this.InputLanauageChangedUI();
        }

        public void ChangeIme(IntPtr handle)
        {
            this._CurrentImeHandleStr = handle.ToString();
            foreach (imeItem item in this.imeList)
            {
                if (item.Tag.Handle.ToString() == this._CurrentImeHandleStr)
                {
                    this.comboBox1.SelectedIndex = this.imeList.IndexOf(item);
                }
            }
            this.InputLanauageChangedUI();
        }

        private void changeLabel()
        {
            IntPtr ptr;
            if ((this.container != null) && (this.container.ActiveControl != null))
            {
                ptr = ImmGetContext(this.container.ActiveControl.Handle);
            }
            else
            {
                ptr = ImmGetContext(GetFocus());
            }
            int conversion = 0;
            int sentence = 0;
            ImmGetConversionStatus(ptr, ref conversion, ref sentence);
            switch (this.Calc(conversion))
            {
                case 0x400:
                case -2147482624:
                    this.label1.Text = "英";
                    this.pictureBox2.Image = Resources.Half;
                    this.pictureBox3.Image = Resources.chs;
                    return;

                case 0x401:
                case -2147482623:
                    this.label1.Text = "中";
                    this.pictureBox2.Image = Resources.Half;
                    this.pictureBox3.Image = Resources.chs;
                    return;

                case 0x408:
                case -2147482616:
                    this.label1.Text = "英";
                    this.pictureBox2.Image = Resources.On;
                    this.pictureBox3.Image = Resources.chs;
                    return;

                case 0x409:
                case -2147482615:
                    this.label1.Text = "中";
                    this.pictureBox2.Image = Resources.On;
                    this.pictureBox3.Image = Resources.chs;
                    return;

                case 0:
                case -2147483648:
                    this.label1.Text = "英";
                    this.pictureBox2.Image = Resources.Half;
                    this.pictureBox3.Image = Resources.eng;
                    return;

                case 1:
                case -2147483647:
                    this.label1.Text = "中";
                    this.pictureBox2.Image = Resources.Half;
                    this.pictureBox3.Image = Resources.eng;
                    return;

                case 8:
                case -2147483640:
                    this.label1.Text = "英";
                    this.pictureBox2.Image = Resources.On;
                    this.pictureBox3.Image = Resources.eng;
                    return;

                case 9:
                case -2147483639:
                    this.label1.Text = "中";
                    this.pictureBox2.Image = Resources.On;
                    this.pictureBox3.Image = Resources.eng;
                    return;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.CurrentInputLanguage = this.imeList[this.comboBox1.SelectedIndex].Tag;
        }

        private void container_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            this.ChangeIme(Application.CurrentInputLanguage.Handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EnumIconResourceProc(IntPtr hModule, IntPtr nType, StringBuilder sName, IntPtr lParam)
        {
            Debug.WriteLine(string.Format("得到的资源名称：{0}", sName));
            Icon.FromHandle(LoadIcon(hModule, sName.ToString()));
            return true;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool EnumResourceNames(IntPtr hModule, IntPtr nType, EnumResNameProc lpEnumFunc, int lParam);
        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(IntPtr hInstance, string sExeFileName, int nIconIndex);
        [DllImport("Kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        private Image GetBitmapFromResource(string sFileName, string sBitmapFlag)
        {
            Bitmap bitmap = null;
            IntPtr hInstance = LoadLibraryEx(sFileName, IntPtr.Zero, 2);
            if (hInstance == IntPtr.Zero)
            {
                Debug.WriteLine("未能成功加载" + sFileName);
                return null;
            }
            string sID = "IMEICO";
            IntPtr zero = IntPtr.Zero;
            Debug.WriteLine("正在获取" + sFileName + "中所有图标。");
            zero = ExtractIcon(base.Handle, sFileName, 0);
            if (zero == IntPtr.Zero)
            {
                sID = "#101";
                zero = LoadImage(hInstance, sID, IMAGE_ICON, 0x10, 0x10, 0);
            }
            if (zero != IntPtr.Zero)
            {
                Debug.WriteLine(string.Format("Hicon:{0}", zero.ToString()));
                bitmap = Icon.FromHandle(zero).ToBitmap();
            }
            EnumResourceNames(hInstance, this.MAKEINTRESOURCE(14), new EnumResNameProc(this.EnumIconResourceProc), 0);
            FreeLibrary(hInstance);
            return bitmap;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern IntPtr GetFocus();
        private Image GetImeBitmap(InputLanguage ime)
        {
            int nBuffer = 0;
            Image bitmapFromResource = null;
            nBuffer = ImmGetIMEFileName(ime.Handle, null, nBuffer);
            StringBuilder sFileName = new StringBuilder(nBuffer);
            ImmGetIMEFileName(ime.Handle, sFileName, nBuffer);
            if (string.IsNullOrEmpty(sFileName.ToString()))
            {
                return Resources.input;
            }
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), sFileName.ToString());
            if (File.Exists(path))
            {
                bitmapFromResource = this.GetBitmapFromResource(path, "");
            }
            if (bitmapFromResource == null)
            {
                bitmapFromResource = Resources.input;
            }
            return bitmapFromResource;
        }

        private string GetImmDescription(InputLanguage inpt)
        {
            int nBuffer = 0;
            StringBuilder sName = new StringBuilder();
            string layoutName = "";
            nBuffer = ImmGetDescription(inpt.Handle, null, nBuffer);
            sName = new StringBuilder(nBuffer);
            ImmGetDescription(inpt.Handle, sName, nBuffer);
            layoutName = sName.ToString();
            if (string.IsNullOrEmpty(layoutName))
            {
                layoutName = inpt.LayoutName;
            }
            return layoutName;
        }

        private void imeBar_SizeChanged(object sender, EventArgs e)
        {
            Font font;
            this.pictureBox1.Size = new Size(base.ClientSize.Height, base.ClientSize.Height);
            this.pictureBox1.Location = new Point(0, 0);
            Graphics graphics = this.label1.CreateGraphics();
            int num = 1;
            for (font = new Font("宋体", (float) num, FontStyle.Regular, GraphicsUnit.Pixel); ((int) graphics.MeasureString("中", font).Height) < base.Height; font = new Font("宋体", (float) num, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                num++;
            }
            num--;
            font = new Font("宋体", (float) num, FontStyle.Regular, GraphicsUnit.Pixel);
            this.label1.Height = base.ClientSize.Height;
            this.label1.Width = (int) graphics.MeasureString("中", font).Width;
            this.label1.Top = 0;
            this.label1.Left = this.pictureBox1.Right;
            this.label1.Font = font;
            this.pictureBox2.Size = this.pictureBox1.Size;
            this.pictureBox3.Size = this.pictureBox2.Size;
            this.pictureBox2.Location = new Point(this.label1.Right, 0);
            this.pictureBox3.Location = new Point(this.pictureBox2.Right, 0);
            this.comboBox1.Top = 0;
            this.comboBox1.Left = this.pictureBox3.Right;
            this.comboBox1.Height = base.ClientSize.Height;
            this.comboBox1.Width = base.ClientSize.Width - this.comboBox1.Left;
            this.comboBox1.Font = font;
        }

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC, ref int conversion, ref int sentence);
        [DllImport("Imm32.dll")]
        public static extern int ImmGetDescription(IntPtr Hkl, StringBuilder sName, int nBuffer);
        [DllImport("Imm32.dll")]
        public static extern int ImmGetIMEFileName(IntPtr Hkl, StringBuilder sFileName, int nBuffer);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int conversion, int sentence);
        private void InitializeComponent()
        {
            this.components = new Container();
            this.pictureBox1 = new PictureBox();
            this.label1 = new Label();
            this.comboBox1 = new ComboBox();
            this.pictureBox2 = new PictureBox();
            this.pictureBox3 = new PictureBox();
            this.timer1 = new Timer(this.components);
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox3).BeginInit();
            base.SuspendLayout();
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Location = new Point(0x10, 3);
            this.pictureBox1.Margin = new Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(100, 50);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.label1.BorderStyle = BorderStyle.FixedSingle;
            this.label1.Cursor = Cursors.Hand;
            this.label1.Location = new Point(0x7a, 0x1a);
            this.label1.Margin = new Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(100, 0x17);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label1.Click += new EventHandler(this.label1_Click);
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new Point(0xbd, 0x17);
            this.comboBox1.Margin = new Padding(0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new Size(0x79, 20);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
            this.pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox2.Cursor = Cursors.Hand;
            this.pictureBox2.Location = new Point(0x93, 0x31);
            this.pictureBox2.Margin = new Padding(0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(100, 50);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new EventHandler(this.pictureBox2_Click);
            this.pictureBox3.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox3.Cursor = Cursors.Hand;
            this.pictureBox3.Location = new Point(0xbd, 0x1a);
            this.pictureBox3.Margin = new Padding(0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new Size(100, 50);
            this.pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new EventHandler(this.pictureBox3_Click);
            this.timer1.Interval = 0x3e8;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            base.Controls.Add(this.pictureBox3);
            base.Controls.Add(this.pictureBox2);
            base.Controls.Add(this.comboBox1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.pictureBox1);
            base.Margin = new Padding(0);
            base.Size = new Size(0x145, 0x41);
            base.SizeChanged += new EventHandler(this.imeBar_SizeChanged);
            ((ISupportInitialize) this.pictureBox1).EndInit();
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox3).EndInit();
            base.ResumeLayout(false);
        }

        private void InitMenus()
        {
            this.imeList = new List<imeItem>();
            this.comboBox1.Items.Clear();
            string immDescription = "";
            foreach (InputLanguage language in InputLanguage.InstalledInputLanguages)
            {
                immDescription = this.GetImmDescription(language);
                if (!string.IsNullOrEmpty(immDescription))
                {
                    imeItem item = new imeItem {
                        LayoutName = immDescription,
                        Tag = language,
                        image = this.GetImeBitmap(language)
                    };
                    this.comboBox1.Items.Add(item.LayoutName);
                    this.imeList.Add(item);
                }
            }
            this.InputLanauageChangedUI();
        }

        public void InputLanauageChangedUI()
        {
            foreach (imeItem item in this.imeList)
            {
                if (item.Tag.Handle.ToString() == this._CurrentImeHandleStr)
                {
                    this.pictureBox1.Image = item.image;
                }
            }
            this.changeLabel();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            IntPtr hIMC = ImmGetContext(GetFocus());
            int conversion = 0x409;
            int sentence = 0;
            ImmGetConversionStatus(hIMC, ref conversion, ref sentence);
            switch (this.Calc(conversion))
            {
                case -2147482623:
                case -2147482615:
                case -2147483647:
                case -2147483639:
                case 0x401:
                case 0x409:
                case 1:
                case 9:
                    conversion--;
                    break;

                default:
                    conversion++;
                    break;
            }
            if (ImmSetConversionStatus(ImmGetContext(GetFocus()), conversion, sentence))
            {
                this.changeLabel();
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string iID);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadImage(IntPtr hInstance, string sID, int nType, int cx, int cy, int fuLoad);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string sFileName, IntPtr hFile, int dwFlags);
        private IntPtr MAKEINTRESOURCE(int nID)
        {
            return new IntPtr((long) ((short) nID));
        }

        protected override void OnPrint(PaintEventArgs e)
        {
            base.OnPrint(e);
            if (this.container == null)
            {
                this.container = base.FindForm();
                if (this.container != null)
                {
                    this.container.InputLanguageChanged += new InputLanguageChangedEventHandler(this.container_InputLanguageChanged);
                    this.ChangeIme(Application.CurrentInputLanguage.Handle);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            IntPtr hIMC = ImmGetContext(GetFocus());
            int conversion = 0x409;
            int sentence = 0;
            ImmGetConversionStatus(hIMC, ref conversion, ref sentence);
            switch (this.Calc(conversion))
            {
                case -2147483640:
                case -2147483639:
                case -2147482616:
                case -2147482615:
                case 8:
                case 9:
                case 0x408:
                case 0x409:
                {
                    conversion -= 8;
                    Bitmap half = Resources.Half;
                    break;
                }
                default:
                {
                    conversion += 8;
                    Bitmap on = Resources.On;
                    break;
                }
            }
            if (ImmSetConversionStatus(ImmGetContext(GetFocus()), conversion, sentence))
            {
                this.changeLabel();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            IntPtr hIMC = ImmGetContext(GetFocus());
            int conversion = 0x409;
            int sentence = 0;
            ImmGetConversionStatus(hIMC, ref conversion, ref sentence);
            switch (this.Calc(conversion))
            {
                case -2147482624:
                case -2147482623:
                case -2147482616:
                case -2147482615:
                case 0x400:
                case 0x401:
                case 0x408:
                case 0x409:
                {
                    conversion -= 0x400;
                    Bitmap eng = Resources.eng;
                    break;
                }
                default:
                {
                    conversion += 0x400;
                    Bitmap chs = Resources.chs;
                    break;
                }
            }
            if (ImmSetConversionStatus(ImmGetContext(GetFocus()), conversion, sentence))
            {
                this.changeLabel();
            }
        }

        private int Pow(int x, int y)
        {
            if ((y < 0) || (y > 0x1f))
            {
                return 0x7fffffff;
            }
            switch (y)
            {
                case 0:
                    return 1;

                case 1:
                    return x;
            }
            return (x * this.Pow(x, y - 1));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.InputLanauageChangedUI();
        }

        public delegate bool EnumResNameProc(IntPtr hModule, IntPtr nType, StringBuilder sName, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct imeItem
        {
            public string LayoutName;
            public InputLanguage Tag;
            public Image image;
        }
    }
}

