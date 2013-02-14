using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace Vigil
{
    class CheckTimers
    {
        private WinAPI Timers;
        private Dictionary<string, Dictionary<string, string>> TimerSettings;
        private Dictionary<string, Label> timerUI = new Dictionary<string, Label>();
        private List<string> suppressedEarlyWarnings = new List<string>();
        private DateTime LastAnnounce = DateTime.Now;
        private DateTime LastUIUpdate = DateTime.Now;
        private bool UIUpdated = false;
        public bool KeepGoing = true;


        public void AddTimerUI(string k, Label v){
            timerUI.Add(k, v);
        }

        public CheckTimers(WinAPI timers, Dictionary<string, Dictionary<string, string>> timerSettings)
        {
            Timers = timers;
            TimerSettings = timerSettings;
        }

        public void RunMe()
        {
            while (KeepGoing)
            {
                Thread.Sleep(100);
                var activeTimers = Timers.GetTimers().ToList();
                UIUpdated = false;

                foreach (var timer in activeTimers)
                {
                    if (timerUI.ContainsKey(timer.Key) && timerUI[timer.Key].SafeInvoke(pb => pb.BackColor)==System.Drawing.Color.Transparent)
                    {
                        timerUI[timer.Key].SafeInvoke(pb => pb.BackColor = System.Drawing.Color.Red);
                        
                    }
                    if (LastUIUpdate.AddSeconds(0.5) < DateTime.Now)
                    {
                        var currentTimer = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"];
                        var duration = Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"]);
                        var now = DateTime.Now;
                        var startedAt = timer.Value;

                        var activeAt = startedAt.AddSeconds(duration);

                        var secTill = Convert.ToInt32(Math.Floor((activeAt - now).TotalSeconds));
                        var MinAndSec = secTill > 0 ? ((secTill / 60)>0?(secTill / 60).ToString() + "min ":"") + (secTill % 60).ToString() + "sec" : timer.Key;
                        //Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"] + " up in " + MinAndSec);


                        if (timerUI.ContainsKey(timer.Key) && timerUI[timer.Key].SafeInvoke(pb => pb.Text)!= MinAndSec)
                        {
                            timerUI[timer.Key].SafeInvoke(pb => pb.Text = MinAndSec);

                        }
                        UIUpdated = true;
                    }

                    if (LastAnnounce.AddSeconds(3) < DateTime.Now && 
                        timer.Value.AddSeconds(Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"])) < DateTime.Now)
                    {
                        Timers.RemoveTimer(timer.Key);
                        if (suppressedEarlyWarnings.Contains(timer.Key))
                        {
                            suppressedEarlyWarnings.Remove(timer.Key);
                        }
                        //Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"]);

                        if (timerUI.ContainsKey(timer.Key))
                        {
                            timerUI[timer.Key].SafeInvoke(pb => pb.BackColor = System.Drawing.Color.Transparent);
                        }

                        var play = new Play();
                        play.fn = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["file"] + ".wav";
                        Thread PlayThread = new Thread(new ThreadStart(play.RunMe));
                        PlayThread.IsBackground = true;
                        PlayThread.Start();

                        LastAnnounce = DateTime.Now;

                    }
                    else if (LastAnnounce.AddSeconds(3) < DateTime.Now &&
                        !suppressedEarlyWarnings.Contains(timer.Key) && timer.Value.AddSeconds(
                            Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"])
                            - Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["earlywarn"])
                        ) < DateTime.Now)
                    {
                        if (timerUI.ContainsKey(timer.Key))
                        {
                            timerUI[timer.Key].SafeInvoke(pb => pb.BackColor = System.Drawing.Color.Green);
                        }
                        suppressedEarlyWarnings.Add(timer.Key);
                        //Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"] + " early warning");

                        var play = new Play();
                        play.fn = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["file"] + "_EW.wav";
                        Thread PlayThread = new Thread(new ThreadStart(play.RunMe));
                        PlayThread.IsBackground = true;
                        PlayThread.Start();

                        LastAnnounce = DateTime.Now;
                    }
                }
                if (UIUpdated)
                {
                    LastUIUpdate = DateTime.Now;
                }
            }
        }
    }
}
