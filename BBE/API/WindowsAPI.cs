using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace BBE.API
{
    public enum MessageBoxButtons
    {
        OK = 0,
        OkCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5,
        CancelTryContinue = 6
    }
    public enum OperationSystem
    {
        MacOS,
        Linux,
        Windows11,
        Windows10,
        Windows8_1,
        Windows8,
        Windows7,
        WindowsVista,
        Windows2003,
        WindowsXP,
        Windows2000,
        WindowsMe,
        Windows98,
        Windows95,
        WindowsNT,
        None
    }
    public static class WindowsAPI
    {
        public static Dictionary<string, byte> Keys => keys;
        private readonly static Dictionary<string, byte> keys = new Dictionary<string, byte>()
        {
            { "A", 0x41 },
            { "B", 0x42 },
            { "C", 0x43 },
            { "D", 0x44 },
            { "E", 0x45 },
            { "F", 0x46 },
            { "G", 0x47 },
            { "H", 0x48 },
            { "I", 0x49 },
            { "J", 0x4A },
            { "K", 0x4B },
            { "L", 0x4C },
            { "M", 0x4D },
            { "N", 0x4E },
            { "O", 0x4F },
            { "P", 0x50 },
            { "Q", 0x51 },
            { "R", 0x52 },
            { "S", 0x53 },
            { "T", 0x54 },
            { "U", 0x55 },
            { "V", 0x56 },
            { "W", 0x57 },
            { "X", 0x58 },
            { "Y", 0x59 },
            { "Z", 0x5A },
            { "0", 0x30 },
            { "1", 0x31 },
            { "2", 0x32 },
            { "3", 0x33 },
            { "4", 0x34 },
            { "5", 0x35 },
            { "6", 0x36 },
            { "7", 0x37 },
            { "8", 0x38 },
            { "9", 0x39 },
            { "ENTER", 0x0D },
            { "ESCAPE", 0x1B },
            { "BACKSPACE", 0x08 },
            { "TAB", 0x09 },
            { "SPACE", 0x20 },
            { " ", 0x20 },
            { "LEFT", 0x25 },
            { "UP", 0x26 },
            { "RIGHT", 0x27 },
            { "DOWN", 0x28 },
            { "SHIFT", 0x10 },
            { "CONTROL", 0x11 },
            { "ALT", 0x12 },
            { "CAPSLOCK", 0x14 },
            { "DELETE", 0x2E },
            { "INSERT", 0x2D },
            { "HOME", 0x24 },
            { "END", 0x23 },
            { "PAGEUP", 0x21 },
            { "PAGEDOWN", 0x22 },
            { "F1", 0x70 },
            { "F2", 0x71 },
            { "F3", 0x72 },
            { "F4", 0x73 },
            { "F5", 0x74 },
            { "F6", 0x75 },
            { "F7", 0x76 },
            { "F8", 0x77 },
            { "F9", 0x78 },
            { "F10", 0x79 },
            { "F11", 0x7A },
            { "F12", 0x7B },
            { "F13", 0x7C },
            { "F14", 0x7D },
            { "F15", 0x7E },
            { "F16", 0x7F },
            { "F17", 0x80 },
            { "F18", 0x81 },
            { "F19", 0x82 },
            { "F20", 0x83 },
            { "F21", 0x84 },
            { "F22", 0x85 },
            { "F23", 0x86 },
            { "F24", 0x87 },
            { "SLASH", 0xBF },
            { "/", 0xBF },
            {"WIN", 0x5B }
        };
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

        }
        [StructLayout(LayoutKind.Sequential)]
        struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
        }
        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("ntdll.dll")]
        private static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);
        [DllImport("ntdll.dll")]
        private static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);
        [DllImport("user32.dll")]
        private static extern int MessageBox(HandleRef hWnd, string text, string caption, int type);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private static bool IsWindows10OrGreater(int build = -1) => Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        private struct POINT
        {
            public int X;
            public int Y;
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        /// <summary>
        /// Find window by class name or window name
        /// </summary>
        /// <param name="className">Class name (Notepad.exe, calc.exe etc)</param>
        /// <param name="windowName">Full window name (title)</param>
        /// <returns><see cref="IntPtr"/> of window</returns>
        public static IntPtr GetWindow(string className = null, string windowName = null) => FindWindow(className, windowName);
        /// <summary>
        /// Find window by title name of window
        /// </summary>
        /// <param name="titleName">Name to find</param>
        /// <returns><see cref="IntPtr"/> of window</returns>
        public static IntPtr GetWindowByTitleName(string titleName) => GetWindow(null, titleName);
        /// <summary>
        /// Find window by class name
        /// </summary>
        /// <param name="classsName">Name of class</param>
        /// <returns><see cref="IntPtr"/> of window</returns>
        public static IntPtr GetWindowByClassName(string classsName) => GetWindow(classsName, null);
        /// <summary>
        /// Set window position
        /// </summary>
        /// <param name="window">Window to set</param>
        /// <param name="X">X coordinate</param>
        /// <param name="Y">Y coordinate</param>
        public static void SetWindowPosition(IntPtr window, int X, int Y) => SetWindowPos(window, IntPtr.Zero, X, Y, 0, 0, 0x0004 | 0x0001);
        /// <summary>
        /// Set window position
        /// </summary>
        /// <param name="window">Window to set</param>
        /// <param name="position">Position to set (only int)</param>
        public static void SetWindowPosition(IntPtr window, Vector2Int position) => SetWindowPosition(window, position.x, position.y);
        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="x">Coordinate by x</param>
        /// <param name="y">Coordinate by y</param>
        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="pos">Position to set</param>
        public static void SetCursorPosition(Vector2Int pos) => SetCursorPosition(pos.x, pos.y);
        /// <summary>
        /// Get window position
        /// </summary>
        /// <param name="window">Window to find position</param>
        /// <param name="x">Window coordinate by X</param>
        /// <param name="y">Window coordinate by Y</param>
        public static void GetWindowPosition(IntPtr window, out int x, out int y)
        {
            Vector2Int v = GetWindowPosition(window);
            x = v.x; 
            y = v.y;
        }
        /// <summary>
        /// Check if caps lock is active
        /// </summary>
        /// <returns>Caps lock state</returns>
        public static bool CapsLockIsActive()
        {
            return (GetKeyState(0x14) & 0x0001) != 0;
        }
        /// <summary>
        /// Change caps lock state
        /// </summary>
        /// <param name="state">State to set</param>
        public static void SetCapsLockState(bool state)
        {
            if (CapsLockIsActive() != state)
            {
                PressKey(0x14);
                ReleaseKey(0x14);
            }
        }
        /// <summary>
        /// Press key on keyboard
        /// </summary>
        /// <param name="key">Key to press, you can find info in https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes</param>
        public static void PressKey(byte key)
        {
            keybd_event(key, 0, 0x0000, UIntPtr.Zero);
        }
        /// <summary>
        /// Press key on keyboard
        /// </summary>
        /// <param name="key">Key to press</param>
        public static void PressKey(char key) => PressKey(key.ToString());
        /// <summary>
        /// Press key on keyboard
        /// </summary>
        /// <param name="key">Key to press</param>
        public static void PressKey(string key)
        {
            if (keys.ContainsKey(key.ToUpper()))
            {
                PressKey(keys[key.ToUpper()]);
            }
            else
            {
                UnityEngine.Debug.Log("Unknown key " + key);
            }
        }
        /// <summary>
        /// Release key on keyboard
        /// </summary>
        /// <param name="key">Key to release, you can find info in https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes</param>
        public static void ReleaseKey(byte key)
        {
            keybd_event(key, 0, 0x0002, UIntPtr.Zero);
        }
        /// <summary>
        /// Release key on keyboard
        /// </summary>
        /// <param name="key">Key to release</param>
        public static void ReleaseKey(char key) => ReleaseKey(key.ToString());
        /// <summary>
        /// Release key on keyboard
        /// </summary>
        /// <param name="key">Key to release</param>
        public static void ReleaseKey(string key)
        {
            if (keys.ContainsKey(key.ToUpper()))
            {
                ReleaseKey(keys[key.ToUpper()]);
            }
            else
            {
                UnityEngine.Debug.Log("Unknown key " + key);
            }
        }
        /// <summary>
        /// Use <see cref="WindowsAPI.PressKey(string)"/> and <see cref="WindowsAPI.ReleaseKey(string)"/> to type text
        /// </summary>
        /// <param name="text">Text to type</param>
        public static void TypeTextOnKeyboard(string text)
        {
            foreach (char c in text)
            {
                PressKey(c.ToString());
                ReleaseKey(c.ToString());
            }
        }
        /// <summary>
        /// Get window position
        /// </summary>
        /// <param name="window">Window to find position</param>
        /// <returns>Window position as <see cref="Vector2Int"/></returns>
        public static Vector2Int GetWindowPosition(IntPtr window)
        {
            if (GetWindowRect(window, out RECT windowRect))
            {
                return new Vector2Int(windowRect.Left, windowRect.Top);
            }
            return Vector2Int.zero;
        }
        /// <summary>
        /// Change window alpha channel (make it transperent)
        /// </summary>
        /// <param name="window">Window to make</param>
        /// <param name="transparency">Level of transparency (0 for fully transparent, 255 for fully opaque)</param>
        public static void SetWindowTransparent(IntPtr window, byte transparency)
        {
            int exStyle = GetWindowLong(window, -20);
            SetWindowLong(window, -20, exStyle | 0x00080000);
            SetLayeredWindowAttributes(window, 0, transparency, 0x00000002);    
        }
        /// <summary>
        /// Change title bar color to black or white
        /// </summary>
        /// <param name="window">Window which title bar need be changed</param>
        /// <param name="enabled">If true title bar will black, else white</param>
        public static void UseImmersiveDarkMode(IntPtr window, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                int attribute = 19;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = 20;
                }
                int useImmersiveDarkMode = enabled ? 1 : 0;
                DwmSetWindowAttribute(window, attribute, ref useImmersiveDarkMode, sizeof(int));
            }
        }
        /// <summary>
        /// Change window title bar to black
        /// </summary>
        /// <param name="window">Window which title bar need be changed</param>
        public static void ChangeTitleBarToBlack(IntPtr window) => UseImmersiveDarkMode(window, true);
        /// <summary>
        /// Change window title bar to white
        /// </summary>
        /// <param name="window">Window which title bar need be changed</param>
        public static void ChangeTitleBarToWhite(IntPtr window) => UseImmersiveDarkMode(window, false);
        /// <summary>
        /// Use CMD via code
        /// </summary>
        /// <param name="command">Command to excute</param>
        public static void UseCmd(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            Process process = Process.Start(processInfo);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();
        }
        /// <summary>
        /// Get screen size with "out" parameters
        /// </summary>
        /// <param name="width">Width of screen</param>
        /// <param name="height">Height of screen</param>
        public static void GetScreenSize(out int width, out int height)
        {
            Vector2Int vector2 = GetScreenSize();
            width = vector2.x;
            height = vector2.y;
        }
        /// <summary>
        /// Get screen size as <see cref="Vector2Int"/>
        /// </summary>
        /// <returns>Screen size as <see cref="Vector2Int"/> or <see cref="Vector2.zero"/> if something goes wrong</returns>
        public static Vector2Int GetScreenSize()
        {
            try
            {
                int width = 0;
                int height = 0;
                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                if (EnumDisplaySettings(null, -1, ref devMode))
                {
                    width = devMode.dmPelsWidth;
                    height = devMode.dmPelsHeight;
                }
                return new Vector2Int(width, height);
            }
            catch
            {
                return Vector2Int.zero;
            }
        }
        /// <summary>
        /// Get IntPtr of current process
        /// </summary>
        /// <returns>Hadle of current process</returns>
        public static IntPtr GetWindow() => GetCurrentProcess().Handle;
        /// <summary>
        /// Get current process
        /// </summary>
        /// <returns>Current process</returns>
        public static Process GetCurrentProcess() => Process.GetCurrentProcess();
        /// <summary>
        /// Open link in default browser
        /// </summary>
        /// <param name="link">Link to open</param>
        public static void OpenLink(string link) => Process.Start(link);
        /// <summary>
        /// Raname file
        /// </summary>
        /// <param name="fullPath">Full path to file</param>
        /// <param name="newName">New file name (ONLY FILE NAME, NO PATH)</param>
        public static void RenameFiles(string fullPath, string newName) => UseCmd("rename \"" + fullPath + "\" \"" + newName + "\"");
        /// <summary>
        /// Is program active or process active
        /// </summary>
        /// <param name="name">Name of program to find</param>
        /// <param name="searchInWindowClass">If true proces will sheared in class name of windows</param>
        /// <param name="searchInWindowTitle">If true proces will sheared in title name of windows</param>
        /// <param name="searchInProcesses">If true proces will sheared machine name in all actived process</param>
        /// <returns>Check if program is opened</returns>
        public static bool ProgramIsOpened(string name, bool searchInWindowClass = true, bool searchInWindowTitle = true, bool searchInProcesses = true)
        {
            bool result = false;
            if (searchInWindowClass) result = result || FindWindow(name, null) != IntPtr.Zero;
            if (searchInWindowTitle) result = result || FindWindow(null, name) != IntPtr.Zero;
            if (searchInProcesses) result = result || Process.GetProcessesByName(name).Any();
            return result;
        }
        /// <summary>
        /// Show window with text
        /// </summary>
        /// <param name="text">Text to show</param>
        /// <param name="buttons"></param>
        /// <param name="title">Buttons that message will contains</param>
        /// <returns>Result of pressing</returns>
        public static int ShowDialogWindow(string title, string text, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            bool currsor = Cursor.visible;
            CursorLockMode mode = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            int res = MessageBox(new HandleRef(), text, title, (int)buttons);
            Cursor.visible = currsor;
            Cursor.lockState = mode;
            return res;
        }
        /// <summary>
        /// Show window with text and do action if user press OK button
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="text">Text to show</param>
        /// <param name="onOk">Action to do when user press OK</param>
        /// <param name="onCancel">Action to do when user press cancel button</param>
        /// <returns>Result of pressing</returns>
        public static int ShowDialogWindow(string title, string text, Action onOk, Action onCancel)
        {
            int result = ShowDialogWindow(title, text, MessageBoxButtons.OkCancel);
            if (result == 1)
                onOk();
            else
                onCancel();
            Cursor.visible = !Cursor.visible;
            Cursor.visible = !Cursor.visible;
            return result;
        }
        /// <summary>
        /// Get user name on PC
        /// </summary>
        /// <returns>Name of user</returns>
        public static string GetUserName() => Environment.MachineName;
        /// <summary>
        /// Get OS version
        /// </summary>
        /// <returns><see cref="API.OperationSystem"/> of user system</returns>
        public static OperationSystem OperationSystem
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return OperationSystem.MacOS;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return OperationSystem.Linux;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Version version = Environment.OSVersion.Version;
                    if (version.Major == 10)
                    {
                        if (version.Build >= 22000)
                            return OperationSystem.Windows11;
                        return OperationSystem.Windows10;
                    }
                    else if (version.Major == 6)
                    {
                        switch (version.Minor)
                        {
                            case 3:
                                return OperationSystem.Windows8_1;
                            case 2:
                                return OperationSystem.Windows8;
                            case 1:
                                return OperationSystem.Windows7;
                            case 0:
                                return OperationSystem.WindowsVista;
                        }
                    }
                    else if (version.Major == 5)
                    {
                        switch (version.Minor)
                        {
                            case 2:
                                return OperationSystem.Windows2003;
                            case 1:
                                return OperationSystem.WindowsXP;
                            case 0:
                                return OperationSystem.Windows2000;
                        }
                    }
                    else if (version.Major == 4)
                    {
                        switch (version.Minor)
                        {
                            case 90:
                                return OperationSystem.WindowsMe;
                            case 10:
                                return OperationSystem.Windows98;
                            case 0:
                                return OperationSystem.Windows95;
                        }
                    }
                    else if (version.Major == 3 && version.Minor == 51)
                    {
                        return OperationSystem.WindowsNT;
                    }
                }
                return OperationSystem.None;
            }
        }
        /// <summary>
        /// Check if system is Linux
        /// </summary>
        /// <param name="system">System to check</param>
        /// <returns>True if system is is Linux</returns>
        public static bool IsLinux(this OperationSystem system) => system == OperationSystem.Linux;
        /// <summary>
        /// Check if system is Windows
        /// </summary>
        /// <param name="system">System to check</param>
        /// <returns>True if system is is Windows</returns>
        public static bool IsWindows(this OperationSystem system) => !system.IsLinux() && !system.IsMacOS() && system == OperationSystem.None;
        /// <summary>
        /// Check if system is MacOS
        /// </summary>
        /// <param name="system">System to check</param>
        /// <returns>True if system is is MacOS</returns>
        public static bool IsMacOS(this OperationSystem system) => system == OperationSystem.MacOS;
        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="window">Window to close</param>
        public static void CloseWindow(IntPtr window)
        {
            SendMessage(window, 0x0010, IntPtr.Zero, IntPtr.Zero);
        }
        /// <summary>
        /// Show window with text
        /// </summary>
        /// <param name="text">Text to show</param>
        /// <param name="x">Position by X</param>
        /// <param name="y">Position by Y</param>
        /// <param name="width">Width of window</param>
        /// <param name="height">Height of window</param>
        /// <returns>Window as <see cref="IntPtr"/></returns>
        public static IntPtr ShowTextWindow(string text, int x, int y, int width, int height)
        {
            IntPtr window = CreateWindowEx(0, "STATIC", text, 0x10CF0000, x, y, width, height, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (window != IntPtr.Zero)
                ShowWindow(window, 0);
            return window;
        }
    }
}
