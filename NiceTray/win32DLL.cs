using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;

namespace NiceTray
{
    static public class win32DLL_native
    {
        [DllImport("user32.dll", EntryPoint = "mouse_event", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern void user32_mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        public static extern void user32_mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static void DoMouseClick()
        {
            user32_mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(100);
            user32_mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        public static void DoMouseDoubleClick()
        {
            int s = 50;
            user32_mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(s);
            user32_mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(s);
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        static extern bool user32_SetCursorPos(int X, int Y);

        public static void MoveCursorToPoint(int x, int y)
        {
            user32_SetCursorPos(x, y);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        private static extern bool user32_GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            user32_GetCursorPos(out lpPoint);
            return lpPoint;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int user32_GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength")]
        public static extern int user32_GetWindowTextLength(IntPtr hWnd);

        //[DllImport("user32.dll", EntryPoint = "EnumWindows")]
        //public static extern bool user32_EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        public static extern bool user32_EnumWindows(EnumWindowsProc enumFunc, int lParam);


        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowRect")]
        public static extern bool user32_GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr user32_GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool user32_SetForegroundWindow(IntPtr hWnd);


        public static string _GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = user32_GetForegroundWindow();

            if (user32_GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public static IDictionary<IntPtr, string> _GetOpenWindows()
        {
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            user32_EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                int length = user32_GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                user32_GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
    }
}
