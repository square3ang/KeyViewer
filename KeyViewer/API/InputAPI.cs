using KeyViewer.Unity;
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
        public static event Action<Key> OnKeyPressed = delegate { };
        public static event Action<Key> OnKeyReleased = delegate { };
        internal static readonly Dictionary<KeyCode, bool> APIFlags = new Dictionary<KeyCode, bool>();
        internal static void KeyPress(Key key)
            => OnKeyPressed(key);
        internal static void KeyRelease(Key key)
            => OnKeyReleased(key);
        private static bool active;
    }
}
