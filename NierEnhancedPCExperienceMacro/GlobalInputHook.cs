using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NierEnhancedPCExperienceMacro
{
    class GlobalInputHook
    {
        public GlobalInputHook()
        {
            Hook();
        }

        ~GlobalInputHook()
        {
            Unhook();
        }

        public void Hook()
        {
            _kbhook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            _mbhook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(_kbhook);
            UnhookWindowsHookEx(_mbhook);
        }


        public int KeyboardHookProc(int code, int wParam, IntPtr _lParam)
        {
            if (code >= 0)
            {

                var lParam = Marshal.PtrToStructure<KeyboardHookStruct>(_lParam);

                Keys key = (Keys)lParam.vkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs args = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && KeyDown != null)
                    {
                        KeyDown(this, args);
                    } else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && KeyUp != null)
                    {
                        KeyUp(this, args);
                    }
                    if (args.Handled)
                        return 1;

                }
            }

            return CallNextHookEx(_kbhook, code, wParam, _lParam);
        }

        private void KeyDispatch(Keys key, bool down)
        {
            if (!HookedKeys.Contains(key)) return;

            if (down)
                KeyDown?.Invoke(this, new KeyEventArgs(key));
            else
                KeyUp?.Invoke(this, new KeyEventArgs(key));
        }

        private void MouseDispatch(MouseButtons button, int clicks, POINT point, short mouseDelta)
        {
            MouseEvent?.Invoke(this, new MouseEventArgs(button, clicks, point.x, point.y, mouseDelta));
        }



        public int MouseHookProc(int code, int wParam, IntPtr _lParam)
        {

            if (code >= 0)
            {
                var lParam = (MouseHookStruct)Marshal.PtrToStructure(_lParam,typeof(MouseHookStruct));

                MouseButtons button = MouseButtons.None;
                short MouseDelta = 0;
                int clickCount = 0;
                bool down = true;
                bool doubleClick = false;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                    case WM_LBUTTONUP:
                    case WM_LBUTTONDBLCLK:
                        doubleClick = wParam == WM_LBUTTONDBLCLK;
                        down = wParam == WM_LBUTTONDOWN;
                        button = MouseButtons.Left;
                        KeyDispatch(Keys.LButton, down);
                        break;
                    case WM_RBUTTONDOWN:
                    case WM_RBUTTONUP:
                    case WM_RBUTTONDBLCLK:
                        doubleClick = wParam == WM_RBUTTONDBLCLK;
                        down = wParam == WM_RBUTTONDOWN;
                        button = MouseButtons.Right;
                        KeyDispatch(Keys.RButton, down);
                        break;
                    case WM_MBUTTONDOWN:
                    case WM_MBUTTONUP:
                    case WM_MBUTTONDBLCLK:
                        doubleClick = wParam == WM_MBUTTONDBLCLK;
                        down = wParam == WM_MBUTTONDOWN;
                        button = MouseButtons.Middle;
                        KeyDispatch(Keys.MButton, down);
                        break;
                    case WM_XBUTTONDOWN:
                    case WM_XBUTTONUP:
                    case WM_XBUTTONDBLCLK:
                        doubleClick = wParam == WM_XBUTTONDBLCLK;
                        int btncd = (int) ((lParam.mouseData >> 16) & 0xffff);
                        down = wParam == WM_XBUTTONDOWN;
                        switch (btncd)
                        {
                            case 0x1:
                                button = MouseButtons.XButton1;
                                KeyDispatch(Keys.XButton1, down);
                                break;
                            case 0x2:
                                button = MouseButtons.XButton2;
                                KeyDispatch(Keys.XButton2, down);
                                break;
                        }
                        break;
                    case WM_MOUSEWHEEL:
                        MouseDelta = (short) ((lParam.mouseData >> 16) & 0xffff);
                        break;
                }


                clickCount = down || doubleClick ? (doubleClick ? 2 : 1) : 0;

                MouseDispatch(button, clickCount, lParam.pt, MouseDelta);
            }




            return CallNextHookEx(_mbhook, code, wParam, _lParam);
        }


        private IntPtr _kbhook = IntPtr.Zero;
        private IntPtr _mbhook = IntPtr.Zero;

        public List<Keys> HookedKeys = new List<Keys>();

        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event MouseEventHandler MouseEvent;

        #endregion

        #region HookDefs

        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public delegate int HookProc(int code, int wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_XBUTTONDOWN = 0x020B;
        private const int WM_XBUTTONUP = 0x020C;
        private const int WM_XBUTTONDBLCLK = 0x020D;


        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class POINT
    {
        public int x;
        public int y;
    }
}
