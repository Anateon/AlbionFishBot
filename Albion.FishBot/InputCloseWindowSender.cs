using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AutoFishWPF
{
    public class InputCloseWindowSender
    {
        public static IntPtr handle;

        public static int MAKELPARAM(int p, int p_2)
        {
            return ((p_2 << 16) | (p & 0xFFFF));
        }
        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        static extern bool PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,   //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton do

            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101
        }

        public static void ClickKey(ushort scanCode)
        {

            SendMessage(handle, 0x6, 0x00000001, 0);
            PostMessage(handle, (int)WMessages.WM_KEYDOWN, scanCode, 0x00000001 | (scanCode << 16));
            SendMessage(handle, 0x6, 0x00000001, 0);
            PostMessage(handle, (int)WMessages.WM_KEYUP, scanCode, 0x00000001 | (scanCode << 16));
        }

        public static void ClickKeyDown(ushort scanCode)
        {
            SendMessage(handle, 0x6, 0x00000001, 0);
            PostMessage(handle, (int)WMessages.WM_KEYDOWN, scanCode, 0x00000001 | (scanCode << 16));
        }

        public static void ClickKeyUp(ushort scanCode)
        {
            SendMessage(handle, 0x6, 0x00000001, 0);
            PostMessage(handle, (int)WMessages.WM_KEYUP, scanCode, 0x00000001 | (scanCode << 16));
        }

        public static void ClickMouse(int x, int y)
        {
            SendMessage(handle, 0x6, 0x00000001, MAKELPARAM(x, y));
            PostMessage(handle, (int)WMessages.WM_LBUTTONDOWN, 0x00000001, MAKELPARAM(x, y));
            SendMessage(handle, 0x6, 0x00000001, MAKELPARAM(x, y));
            PostMessage(handle, (int)WMessages.WM_LBUTTONUP, 0x00000001, MAKELPARAM(x, y));
        }

        public static void ClickMouseDown(int x, int y)
        {
            SendMessage(handle, 0x6, 0x00000001, MAKELPARAM(x, y));
            PostMessage(handle, (int)WMessages.WM_LBUTTONDOWN, 0x00000001, MAKELPARAM(x, y));
        }
        public static void ClickMouseUp(int x, int y)
        {
            SendMessage(handle, 0x6, 0x00000001, MAKELPARAM(x, y));
            PostMessage(handle, (int)WMessages.WM_LBUTTONUP, 0x00000001, MAKELPARAM(x, y));
        }
    }
}
