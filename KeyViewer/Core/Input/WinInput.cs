#pragma warning disable IDE0079
#pragma warning disable IDE1006
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace KeyViewer.Core.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    public static class WinInput
    {
        private static bool firstInit = true;
        private static HHook hInstance;
        private static bool ralt = false;
        private static bool rctrl = false;
        public static void Initialize()
        {
            if (!Main.IsWindows) return;
            if (firstInit)
            {
                Application.quitting += Release;
                firstInit = false;
            }
            hInstance = KBDHooker.HookThis((nCode, wParam, lParam) =>
            {
                bool isDown = ((int)wParam - 256) == 0;
                WinKeyCode code = KBDHooker.GetKeyCode(lParam);

                if (code == WinKeyCode.RALT) ralt = isDown;
                if (code == WinKeyCode.RCTRL) rctrl = isDown;

                return KBDHooker.CallNextHookEx(hInstance, nCode, wParam, lParam);
            });
        }
        public static void Release()
        {
            if (hInstance == null) return;
            KBDHooker.UnHook(hInstance);
            hInstance = null;
        }

        public static bool RAlt => ralt;
        public static bool RCtrl => rctrl;

        static WinInput()
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT();
            input.Data.Keyboard.Scan = 0;
            input.Data.Keyboard.Flags = 0;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            DOWN = new INPUT[] { input };

            INPUT input2 = new INPUT
            {
                Type = 1
            };
            input2.Data.Keyboard = new KEYBDINPUT();
            input2.Data.Keyboard.Scan = 0;
            input2.Data.Keyboard.Flags = 0;
            input2.Data.Keyboard.Time = 0;
            input2.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            UP = new INPUT[] { input2 };

            INPUT input3 = new INPUT
            {
                Type = 1
            };
            input3.Data.Keyboard = new KEYBDINPUT();
            input3.Data.Keyboard.Scan = 0;
            input3.Data.Keyboard.Flags = 2;
            input3.Data.Keyboard.Time = 0;
            input3.Data.Keyboard.ExtraInfo = IntPtr.Zero;

            PRESS = new INPUT[] { input, input3 };
        }
        static readonly INPUT[] DOWN;
        static readonly INPUT[] UP;
        static readonly INPUT[] PRESS;
        public static void SendKeyPress(this WinKeyCode keyCode)
        {
            PRESS[0].Data.Keyboard.Vk = (ushort)keyCode;
            PRESS[1].Data.Keyboard.Vk = (ushort)keyCode;
            if (Extern.SendInput(2, PRESS, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(this WinKeyCode keyCode)
        {
            DOWN[0].Data.Keyboard.Vk = (ushort)keyCode;
            if (Extern.SendInput(1, DOWN, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();
        }

        /// <summary>
        /// Release a key that is being hold down
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(this WinKeyCode keyCode)
        {
            UP[0].Data.Keyboard.Vk = (ushort)keyCode;
            if (Extern.SendInput(1, UP, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();

        }
    }
    /// <summary>
    /// Hook Instance
    /// </summary>
    public class HHook
    {
        IntPtr ptr;
        public HHook(IntPtr ptr) => this.ptr = ptr;
        public static explicit operator HHook(IntPtr ptr) => new HHook(ptr);
        public static implicit operator IntPtr(HHook ptr) => ptr.ptr;
    }
    public static class KBDHooker
    {
        /// <summary>
        /// KeyCode를 반환합니다. (lParam)
        /// </summary>
        /// <param name="lParam">IntPtr</param>
        /// <returns>KeyCode</returns>
        public static WinKeyCode GetKeyCode(IntPtr lParam) => (WinKeyCode)Marshal.ReadInt32(lParam);
        /// <summary>
        /// 키보드를 후킹합니다. (현재 프로세스)
        /// </summary>
        /// <param name="HookProc">LowLevelKeyboardProc Params:(Int32 nCode, IntPtr wParam, IntPtr lParam) Returns:(IntPtr)</param>
        /// <returns>Hooked Ptr</returns>
        public static HHook HookThis(LowLevelKeyboardProc HookProc) => HookProcess(HookProc, Process.GetCurrentProcess());
        /// <summary>
        /// 키보드를 후킹합니다. (프로세스)
        /// </summary>
        /// <param name="HookProc">LowLevelKeyboardProc Params:(Int32 nCode, IntPtr wParam, IntPtr lParam) Returns:(IntPtr)</param>
        /// <returns>Hooked Ptr</returns>
        public static HHook HookProcess(LowLevelKeyboardProc HookProc, Process proc) => (HHook)Extern.SetWindowsHookEx((int)Extern.HookType.WH_KEYBOARD_LL, HookProc, Extern.GetModuleHandle(proc.MainModule.ModuleName), 0);
        /// <summary>
        /// 입력을 막지 않고 흘려보내줍니다.
        /// </summary>
        /// <param name="HHOOK">Hooked Ptr</param>
        /// <param name="nCode">Determine Invalid Key or Normal Key</param>
        /// <param name="wParam">Determine Released or Pressesd</param>
        /// <param name="lParam">Get KeyCode</param>
        /// <returns>IntPtr</returns>
        public static IntPtr CallNextHookEx(HHook HHOOK, int nCode, IntPtr wParam, IntPtr lParam) => Extern.CallNextHookEx(HHOOK, nCode, wParam, lParam);
        /// <summary>
        /// 입력을 막습니다.
        /// </summary>
        /// <returns>IntPtr</returns>
        public static IntPtr Return() => (IntPtr)1;
        /// <summary>
        /// 키가 눌려있는지 확인합니다.
        /// </summary>
        /// <param name="keyCode">KeyCode</param>
        /// <param name="nCode">Determine Invalid Key or Normal Key</param>
        /// <param name="wParam">Determine Released or Pressesd</param>
        /// <param name="lParam">Get KeyCode</param>
        /// <returns>IntPtr</returns>
        public static bool IsKeyDown(WinKeyCode keyCode, int nCode, IntPtr wParam, IntPtr lParam) => nCode >= 0 && (int)wParam == 256 && (WinKeyCode)Marshal.ReadInt32(lParam) == keyCode;
        /// <summary>
        /// 키가 떼져있는지 확인합니다.
        /// </summary>
        /// <param name="keyCode">KeyCode</param>
        /// <param name="nCode">Determine Invalid Key or Normal Key</param>
        /// <param name="wParam">Determine Released or Pressesd</param>
        /// <param name="lParam">Get KeyCode</param>
        /// <returns>IntPtr</returns>
        public static bool IsKeyUp(WinKeyCode keyCode, int nCode, IntPtr wParam, IntPtr lParam) => nCode >= 0 && (int)wParam == 257 && (WinKeyCode)Marshal.ReadInt32(lParam) == keyCode;
        /// <summary>
        /// 키보드를 후킹합니다. (전역)
        /// </summary>
        /// <param name="HookProc">LowLevelKeyboardProc Params:(Int32 nCode, IntPtr wParam, IntPtr lParam) Returns:(IntPtr)</param>
        /// <returns>Hooked Ptr</returns>
        public static HHook Hook(this LowLevelKeyboardProc HookProc) => (HHook)Extern.SetWindowsHookEx((int)Extern.HookType.WH_KEYBOARD_LL, HookProc, Extern.LoadLibrary("User32"), 0);
        /// <summary>
        /// 키보드 후킹을 해제합니다.
        /// </summary>
        /// <param name="HHOOK"></param>
        /// <returns></returns>
        public static bool UnHook(this HHook HHOOK) => Extern.UnhookWindowsHookEx(HHOOK);
    }
    public enum WinKeyCode : ushort
    {
        RALT = 21,
        RCTRL = 25,

        #region Media

        /// <summary>
        /// Next track if a song is playing
        /// </summary>
        MEDIA_NEXT_TRACK = 0xb0,

        /// <summary>
        /// Play pause
        /// </summary>
        MEDIA_PLAY_PAUSE = 0xb3,

        /// <summary>
        /// Previous track
        /// </summary>
        MEDIA_PREV_TRACK = 0xb1,

        /// <summary>
        /// Stop
        /// </summary>
        MEDIA_STOP = 0xb2,

        #endregion

        #region math

        /// <summary>Key "+"</summary>
        ADD = 0x6b,
        /// <summary>
        /// "*" key
        /// </summary>
        MULTIPLY = 0x6a,

        /// <summary>
        /// "/" key
        /// </summary>
        DIVIDE = 0x6f,

        /// <summary>
        /// Subtract key "-"
        /// </summary>
        SUBTRACT = 0x6d,

        #endregion

        #region Browser
        /// <summary>
        /// Go Back
        /// </summary>
        BROWSER_BACK = 0xa6,
        /// <summary>
        /// Favorites
        /// </summary>
        BROWSER_FAVORITES = 0xab,
        /// <summary>
        /// Forward
        /// </summary>
        BROWSER_FORWARD = 0xa7,
        /// <summary>
        /// Home
        /// </summary>
        BROWSER_HOME = 0xac,
        /// <summary>
        /// Refresh
        /// </summary>
        BROWSER_REFRESH = 0xa8,
        /// <summary>
        /// browser search
        /// </summary>
        BROWSER_SEARCH = 170,
        /// <summary>
        /// Stop
        /// </summary>
        BROWSER_STOP = 0xa9,
        #endregion

        #region Numpad numbers
        /// <summary>
        /// 
        /// </summary>
        NUMPAD0 = 0x60,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD1 = 0x61,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD2 = 0x62,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD3 = 0x63,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD4 = 100,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD5 = 0x65,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD6 = 0x66,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD7 = 0x67,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD8 = 0x68,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD9 = 0x69,

        #endregion

        #region Fkeys
        /// <summary>
        /// F1
        /// </summary>
        F1 = 0x70,
        /// <summary>
        /// F10
        /// </summary>
        F10 = 0x79,
        /// <summary>
        /// 
        /// </summary>
        F11 = 0x7a,
        /// <summary>
        /// 
        /// </summary>
        F12 = 0x7b,
        /// <summary>
        /// 
        /// </summary>
        F13 = 0x7c,
        /// <summary>
        /// 
        /// </summary>
        F14 = 0x7d,
        /// <summary>
        /// 
        /// </summary>
        F15 = 0x7e,
        /// <summary>
        /// 
        /// </summary>
        F16 = 0x7f,
        /// <summary>
        /// 
        /// </summary>
        F17 = 0x80,
        /// <summary>
        /// 
        /// </summary>
        F18 = 0x81,
        /// <summary>
        /// 
        /// </summary>
        F19 = 130,
        /// <summary>
        /// 
        /// </summary>
        F2 = 0x71,
        /// <summary>
        /// 
        /// </summary>
        F20 = 0x83,
        /// <summary>
        /// 
        /// </summary>
        F21 = 0x84,
        /// <summary>
        /// 
        /// </summary>
        F22 = 0x85,
        /// <summary>
        /// 
        /// </summary>
        F23 = 0x86,
        /// <summary>
        /// 
        /// </summary>
        F24 = 0x87,
        /// <summary>
        /// 
        /// </summary>
        F3 = 0x72,
        /// <summary>
        /// 
        /// </summary>
        F4 = 0x73,
        /// <summary>
        /// 
        /// </summary>
        F5 = 0x74,
        /// <summary>
        /// 
        /// </summary>
        F6 = 0x75,
        /// <summary>
        /// 
        /// </summary>
        F7 = 0x76,
        /// <summary>
        /// 
        /// </summary>
        F8 = 0x77,
        /// <summary>
        /// 
        /// </summary>
        F9 = 120,

        #endregion

        #region Other
        /// <summary>
        /// 
        /// </summary>
        OEM_1 = 0xba,
        /// <summary>
        /// 
        /// </summary>
        OEM_102 = 0xe2,
        /// <summary>
        /// 
        /// </summary>
        OEM_2 = 0xbf,
        /// <summary>
        /// 
        /// </summary>
        OEM_3 = 0xc0,
        /// <summary>
        /// 
        /// </summary>
        OEM_4 = 0xdb,
        /// <summary>
        /// 
        /// </summary>
        OEM_5 = 220,
        /// <summary>
        /// 
        /// </summary>
        OEM_6 = 0xdd,
        /// <summary>
        /// 
        /// </summary>
        OEM_7 = 0xde,
        /// <summary>
        /// 
        /// </summary>
        OEM_8 = 0xdf,
        /// <summary>
        /// 
        /// </summary>
        OEM_CLEAR = 0xfe,
        /// <summary>
        /// 
        /// </summary>
        OEM_COMMA = 0xbc,
        /// <summary>
        /// 
        /// </summary>
        OEM_MINUS = 0xbd,
        /// <summary>
        /// 
        /// </summary>
        OEM_PERIOD = 190,
        /// <summary>
        /// 
        /// </summary>
        OEM_PLUS = 0xbb,

        #endregion

        #region KEYS

        /// <summary>
        /// 
        /// </summary>
        KEY_0 = 0x30,
        /// <summary>
        /// 
        /// </summary>
        KEY_1 = 0x31,
        /// <summary>
        /// 
        /// </summary>
        KEY_2 = 50,
        /// <summary>
        /// 
        /// </summary>
        KEY_3 = 0x33,
        /// <summary>
        /// 
        /// </summary>
        KEY_4 = 0x34,
        /// <summary>
        /// 
        /// </summary>
        KEY_5 = 0x35,
        /// <summary>
        /// 
        /// </summary>
        KEY_6 = 0x36,
        /// <summary>
        /// 
        /// </summary>
        KEY_7 = 0x37,
        /// <summary>
        /// 
        /// </summary>
        KEY_8 = 0x38,
        /// <summary>
        /// 
        /// </summary>
        KEY_9 = 0x39,
        /// <summary>
        /// 
        /// </summary>
        KEY_A = 0x41,
        /// <summary>
        /// 
        /// </summary>
        KEY_B = 0x42,
        /// <summary>
        /// 
        /// </summary>
        KEY_C = 0x43,
        /// <summary>
        /// 
        /// </summary>
        KEY_D = 0x44,
        /// <summary>
        /// 
        /// </summary>
        KEY_E = 0x45,
        /// <summary>
        /// 
        /// </summary>
        KEY_F = 70,
        /// <summary>
        /// 
        /// </summary>
        KEY_G = 0x47,
        /// <summary>
        /// 
        /// </summary>
        KEY_H = 0x48,
        /// <summary>
        /// 
        /// </summary>
        KEY_I = 0x49,
        /// <summary>
        /// 
        /// </summary>
        KEY_J = 0x4a,
        /// <summary>
        /// 
        /// </summary>
        KEY_K = 0x4b,
        /// <summary>
        /// 
        /// </summary>
        KEY_L = 0x4c,
        /// <summary>
        /// 
        /// </summary>
        KEY_M = 0x4d,
        /// <summary>
        /// 
        /// </summary>
        KEY_N = 0x4e,
        /// <summary>
        /// 
        /// </summary>
        KEY_O = 0x4f,
        /// <summary>
        /// 
        /// </summary>
        KEY_P = 80,
        /// <summary>
        /// 
        /// </summary>
        KEY_Q = 0x51,
        /// <summary>
        /// 
        /// </summary>
        KEY_R = 0x52,
        /// <summary>
        /// 
        /// </summary>
        KEY_S = 0x53,
        /// <summary>
        /// 
        /// </summary>
        KEY_T = 0x54,
        /// <summary>
        /// 
        /// </summary>
        KEY_U = 0x55,
        /// <summary>
        /// 
        /// </summary>
        KEY_V = 0x56,
        /// <summary>
        /// 
        /// </summary>
        KEY_W = 0x57,
        /// <summary>
        /// 
        /// </summary>
        KEY_X = 0x58,
        /// <summary>
        /// 
        /// </summary>
        KEY_Y = 0x59,
        /// <summary>
        /// 
        /// </summary>
        KEY_Z = 90,

        #endregion

        #region volume
        /// <summary>
        /// Decrese volume
        /// </summary>
        VOLUME_DOWN = 0xae,

        /// <summary>
        /// Mute volume
        /// </summary>
        VOLUME_MUTE = 0xad,

        /// <summary>
        /// Increase volue
        /// </summary>
        VOLUME_UP = 0xaf,

        #endregion


        /// <summary>
        /// Take snapshot of the screen and place it on the clipboard
        /// </summary>
        SNAPSHOT = 0x2c,

        /// <summary>Send right click from keyboard "key that is 2 keys to the right of space bar"</summary>
        RightClick = 0x5d,

        /// <summary>
        /// Go Back or delete
        /// </summary>
        BACKSPACE = 8,

        /// <summary>
        /// Control + Break "When debuging if you step into an infinite loop this will stop debug"
        /// </summary>
        CANCEL = 3,
        /// <summary>
        /// Caps lock key to send cappital letters
        /// </summary>
        CAPS_LOCK = 20,
        /// <summary>
        /// Ctlr key
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// Alt key
        /// </summary>
        ALT = 18,

        /// <summary>
        /// "." key
        /// </summary>
        DECIMAL = 110,

        /// <summary>
        /// Delete Key
        /// </summary>
        DELETE = 0x2e,


        /// <summary>
        /// Arrow down key
        /// </summary>
        DOWN = 40,

        /// <summary>
        /// End key
        /// </summary>
        END = 0x23,

        /// <summary>
        /// Escape key
        /// </summary>
        ESC = 0x1b,

        /// <summary>
        /// Home key
        /// </summary>
        HOME = 0x24,

        /// <summary>
        /// Insert key
        /// </summary>
        INSERT = 0x2d,

        /// <summary>
        /// Open my computer
        /// </summary>
        LAUNCH_APP1 = 0xb6,
        /// <summary>
        /// Open calculator
        /// </summary>
        LAUNCH_APP2 = 0xb7,

        /// <summary>
        /// Open default email in my case outlook
        /// </summary>
        LAUNCH_MAIL = 180,

        /// <summary>
        /// Opend default media player (itunes, winmediaplayer, etc)
        /// </summary>
        LAUNCH_MEDIA_SELECT = 0xb5,

        /// <summary>
        /// Left control
        /// </summary>
        LCONTROL = 0xa2,

        /// <summary>
        /// Left arrow
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// Left shift
        /// </summary>
        LSHIFT = 160,

        /// <summary>
        /// left windows key
        /// </summary>
        LWIN = 0x5b,


        /// <summary>
        /// Next "page down"
        /// </summary>
        PAGEDOWN = 0x22,

        /// <summary>
        /// Num lock to enable typing numbers
        /// </summary>
        NUMLOCK = 0x90,

        /// <summary>
        /// Page up key
        /// </summary>
        PAGE_UP = 0x21,

        /// <summary>
        /// Right control
        /// </summary>
        RCONTROL = 0xa3,

        /// <summary>
        /// Return key
        /// </summary>
        ENTER = 13,

        /// <summary>
        /// Right arrow key
        /// </summary>
        RIGHT = 0x27,

        /// <summary>
        /// Right shift
        /// </summary>
        RSHIFT = 0xa1,

        /// <summary>
        /// Right windows key
        /// </summary>
        RWIN = 0x5c,

        /// <summary>
        /// Shift key
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// Space back key
        /// </summary>
        SPACE_BAR = 0x20,

        /// <summary>
        /// Tab key
        /// </summary>
        TAB = 9,

        /// <summary>
        /// Up arrow key
        /// </summary>
        UP = 0x26,

    }
    public static class Extern
    {
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int pid);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("user32.dll")]
        public static extern ushort GetAsyncKeyState(Int32 vKey);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] // used for button-down & button-up
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
    }
}