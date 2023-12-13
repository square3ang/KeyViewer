using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KeyViewer.Models
{
    public class KeyConfig
    {
        public const string SPECIAL_KEY_KPS = "KPS";
        public const string SPECIAL_KEY_TOTAL = "TOTAL";

        public uint Count = 0;
        public KeyCode Code = KeyCode.None;
        public string SpecialKey = null;
        public string Font = "Default";
        public bool RainEnabled = false;
        public RainConfig Rain = new RainConfig();
        public KeyConfig Copy()
        {
            KeyConfig newConfig = new KeyConfig();

            return newConfig;
        }
    }
}
