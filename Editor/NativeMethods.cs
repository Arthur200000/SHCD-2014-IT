namespace Qisi.Editor
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    internal static class NativeMethods
    {
        internal const int ASPECTX = 40;
        internal const int ASPECTXY = 0x2c;
        internal const int ASPECTY = 0x2a;
        internal const int BITSPIXEL = 12;
        internal const int BLTALIGNMENT = 0x77;
        internal const int CFS_POINT = 2;
        internal const int CLIPCAPS = 0x24;
        internal const int COLORRES = 0x6c;
        internal const int CURVECAPS = 0x1c;
        internal const int DESKTOPHORZRES = 0x76;
        internal const int DESKTOPVERTRES = 0x75;
        internal const int DRIVERVERSION = 0;
        internal const int HORZRES = 8;
        internal const int HORZSIZE = 4;
        internal const int LINECAPS = 30;
        internal const int LOGPIXELSX = 0x58;
        internal const int LOGPIXELSY = 90;
        internal const int NUMBRUSHES = 0x10;
        internal const int NUMCOLORS = 0x18;
        internal const int NUMFONTS = 0x16;
        internal const int NUMMARKERS = 20;
        internal const int NUMPENS = 0x12;
        internal const int NUMRESERVED = 0x6a;
        internal const int PDEVICESIZE = 0x1a;
        internal const int PHYSICALHEIGHT = 0x6f;
        internal const int PHYSICALOFFSETX = 0x70;
        internal const int PHYSICALOFFSETY = 0x71;
        internal const int PHYSICALWIDTH = 110;
        internal const int PLANES = 14;
        internal const int POLYGONALCAPS = 0x20;
        internal const int RASTERCAPS = 0x26;
        internal const int SCALINGFACTORX = 0x72;
        internal const int SCALINGFACTORY = 0x73;
        internal const int SHADEBLENDCAPS = 0x2d;
        internal const int SIZEPALETTE = 0x68;
        internal const int TECHNOLOGY = 2;
        internal const int TEXTCAPS = 0x22;
        internal const int VERTRES = 10;
        internal const int VERTSIZE = 6;
        internal const int VREFRESH = 0x74;
        internal const int WM_IME_STARTCOMPOSITION = 0x10d;

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern void CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("gdi32.dll", CharSet=CharSet.Unicode)]
        internal static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
        [DllImport("user32.dll")]
        internal static extern int DestroyCaret();
        [DllImport("User32.dll")]
        internal static extern bool GetCaretPos(out Point point);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public static string GetMD5HashFromFile(string fileName)
        {
            string str;
            try
            {
                FileStream inputStream = new FileStream(fileName, FileMode.Open);
                byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(inputStream);
                inputStream.Close();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(buffer[i].ToString("x2"));
                }
                str = builder.ToString();
            }
            catch (Exception exception)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + exception.Message);
            }
            return str;
        }

        public static Color getRevColor(Color c)
        {
            return Color.FromArgb(0xff - c.R, 0xff - c.G, 0xff - c.B);
        }

        [DllImport("kernel32.dll")]
        public static extern int GlobalAddAtom(string lpString);
        [DllImport("kernel32.dll")]
        public static extern int GlobalDeleteAtom(uint nAtom);
        [DllImport("user32.dll")]
        internal static extern bool HideCaret(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        internal static extern bool ImmReleaseContext(IntPtr hWnd, int hIMC);
        [DllImport("imm32.dll")]
        internal static extern int ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompositionForm);
        public static Color MixColor(Color c1, Color c2)
        {
            return Color.FromArgb((c1.A + c2.A) / 2, (c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
        }

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("user32")]
        internal static extern int SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        internal static extern bool SetProcessDPIAware();
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern void ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COMPOSITIONFORM
        {
            public int dwStyle;
            public Point ptCurrentPos;
            public Qisi.Editor.NativeMethods.Rect rcArea;
        }

        [Flags]
        public enum KeyModifiers
        {
            Alt = 1,
            Ctrl = 2,
            CtrlAndShift = 6,
            None = 0,
            Shift = 4,
            WindowsKey = 8
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Rect
        {
            [FieldOffset(12)]
            public int bottom;
            [FieldOffset(0)]
            public int left;
            [FieldOffset(8)]
            public int right;
            [FieldOffset(4)]
            public int top;
        }
    }
}

