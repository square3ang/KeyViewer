using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace KeyViewer
{
    public static class KPSCalculator
    {
        static Profile Profile;
        static Thread CalculatingThread;
        static int PressCount;
        public static int Kps;
        public static int Max;
        public static double Average;
        public static void Start(Profile profile)
        {
            try
            {
                Profile = profile;
                if (CalculatingThread == null)
                    (CalculatingThread = GetCalculateThread()).Start();
            }
            catch { }
        }
        public static void Stop()
        {
            try { CalculatingThread.Abort(); }
            catch { }
        }
        public static void Press() => PressCount++;
        static Thread GetCalculateThread()
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
                        if (watch.ElapsedMilliseconds >= Profile.KPSUpdateRateMs)
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
                            if (timePoints.Count >= 1000 / Profile.KPSUpdateRateMs)
                                timePoints.RemoveLast();
                            Kps = kps;
                            watch.Restart();
                            Thread.Sleep(Math.Max(Profile.KPSUpdateRateMs - 1, 0));
                        }
                    }
                }
                finally { CalculatingThread = null; }
            });
        }
    }
}
