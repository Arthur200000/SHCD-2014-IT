namespace ExamClientControlsLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    [SecurityCritical]
    internal static class NativeMethods
    {
        private const int WM_CLOSE = 0x10;
        private const int WM_DESTORY = 2;
        private const int WM_QUIT = 0x12;

        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern uint GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        internal static void ShutdownForms(string closeList)
        {
            Hashtable hashtable = new Hashtable();
            List<string> list = new List<string>();
            list.AddRange(closeList.Split(new char[] { '|' }));
            list.Add("DV2ControlHost");
            list.Add("BaseBar");
            foreach (string str in list)
            {
                IntPtr hWnd = FindWindow(str, null);
                if ((hWnd != IntPtr.Zero) && IsWindow(hWnd))
                {
                    PostMessage(hWnd, 0x10, 0, 0);
                }
                else
                {
                    hWnd = IntPtr.Zero;
                }
            }
        }

        internal static void ShutdownForms(string closeList, string exceptClass)
        {
            Hashtable hashtable = new Hashtable();
            List<string> list = new List<string>();
            list.AddRange(closeList.Split(new char[] { '|' }));
            list.Add("DV2ControlHost");
            list.Add("BaseBar");
            foreach (string str in list)
            {
                IntPtr hWnd = FindWindow(str, null);
                if ((hWnd != IntPtr.Zero) && IsWindow(hWnd))
                {
                    StringBuilder lpString = new StringBuilder(0x400);
                    GetWindowText(hWnd, lpString, 0xff);
                    if (!lpString.ToString().Contains(exceptClass))
                    {
                        PostMessage(hWnd, 0x10, 0, 0);
                    }
                }
                else
                {
                    hWnd = IntPtr.Zero;
                }
            }
        }

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    }
}

