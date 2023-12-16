using System;
using UnityEngine;
using KeyViewer.Types;

namespace KeyViewer.Models
{
    public class KeyConfig
    {
        public uint Count = 0;
        public KeyCode Code = KeyCode.None;
        public SpecialKeyType SpecialKey = SpecialKeyType.None;
        public string Font = "Default";
        public string Background = null;
        public bool RainEnabled = false;
        public RainConfig Rain = new RainConfig();
        public KeyConfig Copy()
        {
            KeyConfig newConfig = new KeyConfig();

            return newConfig;
        }
    }
}
