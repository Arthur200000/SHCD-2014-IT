namespace Qisi.General.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
	/// <summary>
	/// Native methods with lots of magic numbers but seven.
	/// </summary>
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
        internal const int HTBOTTOM = 15;
        internal const int HTBOTTOMLEFT = 0x10;
        internal const int HTBOTTOMRIGHT = 0x11;
        internal const int HTLEFT = 10;
        internal const int HTRIGHT = 11;
        internal const int HTTOP = 12;
        internal const int HTTOPLEFT = 13;
        internal const int HTTOPRIGHT = 14;
        internal const int HTTRANSPARENT = -1;
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
        internal const int WM_GETMINMAXINFO = 0x24;
        internal const int WM_IME_STARTCOMPOSITION = 0x10d;
        internal const int WM_NCHITTEST = 0x84;

        internal static int HIWORD(int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int) ((long) n));
        }

        internal static int LOWORD(int n)
        {
            return (n & 0xffff);
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int) ((long) n));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public Point reserved;
            public Size maxSize;
            public Point maxPosition;
            public Size minTrackSize;
            public Size maxTrackSize;
        }
    }
}

