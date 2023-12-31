using System;
using System.Collections.Generic;
using System.Threading;
using KeyViewer.Models;
using System.Diagnostics;

namespace KeyViewer.Core
{
    public class KPSCalculator
    {
        public bool Running { get; private set; }
        public int Kps;
        public int Max;
        public double Average;

        private int pressCount;
        private Profile profile;
        private CancellationTokenSource cts;
        private CancellationToken token;
        private Thread current;

        public KPSCalculator(Profile profile)
        {
            this.profile = profile;
        }
        public void Start()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            current = GetCalculateThread();
            current.Start();
        }
        public void Stop()
        {
            try
            {
                cts.Cancel();
                current.Abort();
            }
            catch { }
            finally
            {
                current = null;
                cts = null;
            }
        }
        public void Press()
        {
            pressCount++;
        }

        Thread GetCalculateThread()
        {
            return new Thread(() =>
            {
                try
                {
                    Running = true;
                    LinkedList<int> timePoints = new LinkedList<int>();
                    int prev = 0, total = 0;
                    long n = 0;
                    Stopwatch watch = Stopwatch.StartNew();
                    while (!token.IsCancellationRequested)
                    {
                        if (watch.ElapsedMilliseconds >= profile.KPSUpdateRate)
                        {
                            int temp = pressCount;
                            pressCount = 0;
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
                            if (timePoints.Count >= 1000 / profile.KPSUpdateRate)
                                timePoints.RemoveLast();
                            Kps = kps;
                            watch.Restart();
                            Thread.Sleep(Math.Max(profile.KPSUpdateRate - 1, 0));
                        }
                    }
                }
                catch { }
                finally { Running = false; }
            });
        }
    }
}
