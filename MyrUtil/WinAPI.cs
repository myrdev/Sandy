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
        private static Dictionary<string, Dictionary<string, string>> Commands;
        private static string lastPressedKey = "";
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

        public WinAPI(Dictionary<string, Dictionary<string, string>> timers, Dictionary<string,Dictionary<string,string>> commands)
        {
            Timers = timers;
            Commands = commands;
            ActiveTimers = new Dictionary<string, DateTime>();
        }

        public void RunMe()
        {
            _hookID = SetHook(_proc);
            //Console.WriteLine("About to start hook");
            Application.Run();
            //Console.WriteLine("Hook started, probably never see this");
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

        private static void RunBatFile(string key)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = Commands[key]["file"];
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit();
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string pressed_key = ((Keys)vkCode).ToString();

                
                if (Commands.ContainsKey(lastPressedKey + "-" + pressed_key))
                {
                    Console.WriteLine("Command key combo pressed: " + lastPressedKey + "-" + pressed_key);

                    RunBatFile(lastPressedKey + "-" + pressed_key);
                }else if (Commands.ContainsKey(pressed_key))
                {
                    Console.WriteLine("Command key pressed: " + pressed_key);

                    RunBatFile(pressed_key);
                }

                if (Timers.ContainsKey(pressed_key))
                {
                    Console.WriteLine(Timers[pressed_key]["message"] + " timer started");
                    ActiveTimers.Add(pressed_key, DateTime.Now);
                }
         
                //Console.WriteLine("key '" + pressed_key + "' was pressed");

                lastPressedKey = pressed_key!=lastPressedKey ? pressed_key : lastPressedKey;

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
