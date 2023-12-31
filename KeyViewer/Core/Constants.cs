using System.IO;
using UnityEngine;

namespace KeyViewer.Core
{
    public static class Constants
    {
        public static string SettingsPath => Path.Combine(Main.Mod.Path, "Settings.json");

        public const string Version = "4.0.0";
        public static readonly Color TooEarlyColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
        public static readonly Color VeryEarlyColor = new Color(1.000f, 0.436f, 0.306f, 1.000f);
        public static readonly Color EarlyPerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
        public static readonly Color PerfectColor = new Color(0.376f, 1.000f, 0.307f, 1.000f);
        public static readonly Color LatePerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
        public static readonly Color VeryLateColor = new Color(1.000f, 0.435f, 0.306f, 1.000f);
        public static readonly Color TooLateColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
        public static readonly Color MultipressColor = new Color(0.000f, 1.000f, 0.930f, 1.000f);
        public static readonly Color FailMissColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
        public static readonly Color FailOverloadColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
    }
}
