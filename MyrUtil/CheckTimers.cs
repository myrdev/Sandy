using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyrUtil
{
    class CheckTimers
    {
        private WinAPI Timers;
        private Dictionary<string, Dictionary<string, string>> TimerSettings;
        private List<string> suppressedEarlyWarnings = new List<string>();
        private DateTime LastAnnounce = DateTime.Now;
        public bool KeepGoing = true;
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
                foreach (var timer in activeTimers)
                {
                    //if (LastAnnounce.AddSeconds(5) < DateTime.Now)
                    //{
                    //    var currentTimer = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"];
                    //    var duration = Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"]);
                    //    var now = DateTime.Now;
                    //    var startedAt = timer.Value;

                    //    var activeAt = startedAt.AddSeconds(duration);

                    //    var secTill = Convert.ToInt32(Math.Floor((activeAt - now).TotalSeconds));
                    //    var MinAndSec = (secTill / 60).ToString() + "min " + (secTill % 60).ToString();
                    //    Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"] + " up in " + MinAndSec);
                    //}

                    if (timer.Value.AddSeconds(Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"])) < DateTime.Now)
                    {
                        Timers.RemoveTimer(timer.Key);
                        if (suppressedEarlyWarnings.Contains(timer.Key))
                        {
                            suppressedEarlyWarnings.Remove(timer.Key);
                        }
                        //Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"]);

                        var play = new Play();
                        play.fn = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["file"] + ".wav";
                        Thread PlayThread = new Thread(new ThreadStart(play.RunMe));
                        PlayThread.Start();

                        Thread.Sleep(3000);

                    }
                    else if (!suppressedEarlyWarnings.Contains(timer.Key) && timer.Value.AddSeconds(
                            Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["time"])
                            - Int32.Parse(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["earlywarn"])
                        ) < DateTime.Now)
                    {
                        suppressedEarlyWarnings.Add(timer.Key);
                        //Console.WriteLine(TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["message"] + " early warning");

                        var play = new Play();
                        play.fn = TimerSettings.Where(t => t.Key == timer.Key).SingleOrDefault().Value["file"] + "_EW.wav";
                        Thread PlayThread = new Thread(new ThreadStart(play.RunMe));
                        PlayThread.Start();

                        Thread.Sleep(3000);

                    }



                }
                if (LastAnnounce.AddSeconds(5) < DateTime.Now)
                {
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    LastAnnounce = DateTime.Now;
                }
            }
        }
    }
}
