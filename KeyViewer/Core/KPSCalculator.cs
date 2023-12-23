using System;
using System.Collections.Generic;
using System.Threading;
using KeyViewer.Models;
using System.Diagnostics;

namespace KeyViewer.Core
{
    public static class KPSCalculator
    {
        static Profile Profile;
        static Thread CalculatingThread;
        static int PressCount;
        static CancellationTokenSource cts;
        static CancellationToken token;
        public static int Kps;
        public static int Max;
        public static double Average;
        public static void Start(Profile profile)
        {
            try
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                token = cts.Token;
                Profile = profile;
                Kps = Max = 0;
                Average = 0;
                if (CalculatingThread == null)
                    (CalculatingThread = GetCalculateThread()).Start();
            }
            catch { }
        }
        public static void Stop()
        {
            try { cts?.Cancel(); CalculatingThread.Abort(); }
            catch { }
            finally { CalculatingThread = null; }
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
                    while (!token.IsCancellationRequested)
                    {
                        if (watch.ElapsedMilliseconds >= Profile.KPSUpdateRate)
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
                            if (timePoints.Count >= 1000 / Profile.KPSUpdateRate)
                                timePoints.RemoveLast();
                            Kps = kps;
                            watch.Restart();
                            Thread.Sleep(Math.Max(Profile.KPSUpdateRate - 1, 0));
                        }
                    }
                }
                finally { CalculatingThread = null; }
            });
        }
    }
}
