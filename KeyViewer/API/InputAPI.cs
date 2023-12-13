using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.API
{
    public static class InputAPI
    {
        public static bool Active
        {
            get => active;
            set
            {
                active = value;
                EventActive = value;
                APIFlags.Clear();
            }
        }
        public static bool EventActive { get; set; }
        public static void PressKey(KeyCode key)
            => APIFlags[key] = true;
        public static void ReleaseKey(KeyCode key)
            => APIFlags[key] = false;
        public static event Action<KeyCode> OnKeyPressed = delegate { };
        public static event Action<KeyCode> OnKeyReleased = delegate { };
        internal static readonly Dictionary<KeyCode, bool> APIFlags = new Dictionary<KeyCode, bool>();
        internal static void KeyPress(KeyCode key)
            => OnKeyPressed(key);
        internal static void KeyRelease(KeyCode key)
            => OnKeyReleased(key);
        private static bool active;
    }
}
