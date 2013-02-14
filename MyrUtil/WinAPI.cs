using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Vigil
{
    internal class WinAPI
    {
        private static Dictionary<string, Dictionary<string, string>> Timers;
        public static Dictionary<string, DateTime> ActiveTimers;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                                        GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public WinAPI(Dictionary<string, Dictionary<string, string>> timers)
        {
            Timers = timers;
            ActiveTimers = new Dictionary<string, DateTime>();
        }

        public void RunMe()
        {
            _hookID = SetHook(_proc);
            Console.WriteLine("About to start hook");
            Application.Run();
            Console.WriteLine("Hook started, probably never see this");
        }

        public Dictionary<string, DateTime> GetTimers()
        {
            return ActiveTimers;
        }

        public void RemoveTimer(string t)
        {
            ActiveTimers.Remove(t);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string pressed_key = ((Keys)vkCode).ToString();
                if (Timers.ContainsKey(pressed_key))
                {
                    Console.WriteLine(Timers[pressed_key]["message"] + " timer started");
                    ActiveTimers.Add(pressed_key, DateTime.Now);
                }
                else
                {
                    //Console.WriteLine("Untracked key '" + pressed_key + "' was pressed");
                    //key not required
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                                                    IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
