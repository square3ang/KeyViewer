using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace KeyViewer
{
    public class KPSCalculator
    {
        public Profile profile;
        public KPSCalculator(Profile profile)
        {
            CalculatingThread = GetCalculateThread();
            this.profile = profile;
        }
        Thread CalculatingThread;
        public int PressCount;
        public int Kps;
        public int Max;
        public double Average;
        public void Start()
        {
            try { CalculatingThread.Start(); }
            catch  (Exception e) { Main.Log.Log($"Exception At Starting KPSCalculator. Report This Exception To CSNB. ({e})"); }
        }
        public void Stop()
        {
            try { CalculatingThread.Abort(); }
            finally { CalculatingThread = GetCalculateThread(); }
        }
        Thread GetCalculateThread()
        {
            return new Thread(() =>
            {
                try
                {
                    LinkedList<int> timePoints = new LinkedList<int>();
                    int prev = 0, total = 0;
                    long n = 0;
                    Stopwatch watch = Stopwatch.StartNew();
                    while (true)
                    {
                        if (watch.ElapsedMilliseconds >= profile.KPSUpdateRateMs)
                        {
                            int temp = PressCount;
                            PressCount = 0;
                            int kps = temp;
                            foreach (int i in timePoints)
                                kps += i;
                            Max = Math.Max(kps, Max);
                            if (kps != 0)
                            {
                                Average = (Average * n + kps) / (n + 1.0);
                                n += 1L;
                                total += temp;
                            }
                            prev = kps;
                            timePoints.AddFirst(temp);
                            if (timePoints.Count >= 1000 / profile.KPSUpdateRateMs)
                                timePoints.RemoveLast();
                            Kps = kps;
                            watch.Restart();
                            Thread.Sleep(Math.Max(profile.KPSUpdateRateMs - 1, 0));
                        }
                    }
                }
                catch
                {
                    Start();
                    return;
                }
            });
        }
    }
}
