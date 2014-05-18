namespace Qisi.General.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class ReadOnlyRichTextBox : RichTextBox
    {
        private static IntPtr moduleHandle;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_SETCURSOR = 0x20;
        private const int WM_SETFOCUS = 7;
        private const int WS_BORDER = 0x800000;
        private const long WS_CAPTION = 0xc00000L;
        private const long WS_CHILD = 0x40000000L;
        private const long WS_CLIPCHILDREN = 0x2000000L;
        private const long WS_CLIPSIBLINGS = 0x4000000L;
        private const long WS_DISABLED = 0x8000000L;
        private const long WS_DLGFRAME = 0x400000L;
        private const int WS_EX_ACCEPTFILES = 0x40000000;
        private const int WS_EX_LEFT = 0x40000000;
        private const int WS_EX_LTRREADING = 0x40000000;
        private const int WS_EX_RIGHTSCROLLBAR = 0x40000000;
        private const long WS_GROUP = 0x20000L;
        private const int WS_HSCROLL = 0x100000;
        private const long WS_MAXIMIZE = 0x1000000L;
        private const long WS_MAXIMIZEBOX = 0x10000L;
        private const long WS_MINIMIZE = 0x20000000L;
        private const long WS_MINIMIZEBOX = 0x20000L;
        private const long WS_OVERLAPPED = 0L;
        private const long WS_POPUP = 0x80000000L;
        private const long WS_SYSMENU = 0x80000L;
        private const long WS_TABSTOP = 0x10000L;
        private const long WS_THICKFRAME = 0x40000L;
        private const long WS_VISIBLE = 0x10000000L;
        private const int WS_VSCROLL = 0x200000;

        public ReadOnlyRichTextBox()
        {
            this.Cursor = Cursors.Arrow;
            base.TabStop = true;
            base.ReadOnly = true;
            this.BackColor = Color.White;
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr LoadLibrary(string path);
        protected override void WndProc(ref Message m)
        {
            if ((((m.Msg != 7) && (m.Msg != 0x100)) && ((m.Msg != 0x101) && (m.Msg != 0x201))) && ((((m.Msg != 0x202) && (m.Msg != 0x203)) && ((m.Msg != 0x204) && (m.Msg != 0x205))) && (((m.Msg != 0x206) && (m.Msg != 0x200)) && (m.Msg != 0x20))))
            {
                base.WndProc(ref m);
            }
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                if (moduleHandle == IntPtr.Zero)
                {
                    moduleHandle = LoadLibrary("msftedit.dll");
                    if (((long) moduleHandle) < 0x20L)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "无法加载Msftedit.dll");
                    }
                }
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ClassName = "RICHEDIT50W";
                if (this.Multiline)
                {
                    if (((base.ScrollBars & RichTextBoxScrollBars.Horizontal) != RichTextBoxScrollBars.None) && !base.WordWrap)
                    {
                        createParams.Style |= 0x100000;
                        if ((base.ScrollBars & ((RichTextBoxScrollBars) 0x10)) != RichTextBoxScrollBars.None)
                        {
                            createParams.Style |= 0x2000;
                        }
                    }
                    if ((base.ScrollBars & RichTextBoxScrollBars.Vertical) != RichTextBoxScrollBars.None)
                    {
                        createParams.Style |= 0x200000;
                        if ((base.ScrollBars & ((RichTextBoxScrollBars) 0x10)) != RichTextBoxScrollBars.None)
                        {
                            createParams.Style |= 0x2000;
                        }
                    }
                }
                if ((BorderStyle.FixedSingle == base.BorderStyle) && ((createParams.Style & 0x800000) != 0))
                {
                    createParams.ExStyle |= 0x200;
                }
                if ((createParams.Style & 0x81c4) != 0x81c4)
                {
                    createParams.Style &= 0x7ffff0ff;
                    createParams.Style |= 0x100;
                }
                return createParams;
            }
        }
    }
}

