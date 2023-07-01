using HarmonyLib;
using SkyHook;
using System;
using System.Linq;
using UnityEngine;

namespace KeyViewer
{
    [HarmonyPatch]
    public static class InputStack
    {
        static bool initialized = false;
        static ShiftStack<KeyCode> keys;
        public static void Init()
        {
            if (initialized) return;
            keys = new ShiftStack<KeyCode>(() => Math.Max(Main.KeyManager?.keys.Count ?? 0, 2));
            SkyHookManager.KeyUpdated.AddListener(she =>
            {
                try
                {
                    if (Main.KeyManager == null) return;
                    if (she.Type == SkyHook.EventType.KeyReleased) return;
                    if (Main.IsEnabled && AsyncInputManager.isActive && (scrController.instance?.gameworld ?? false))
                    {
                        var code = AsyncInputCompat.Convert(she.Label);
                        if (Main.KeyManager.Codes.Contains(code))
                            keys.Push(code);
                    }
                }
                finally { }
            });
            initialized = true;
        }
        public static KeyCode Pop()
        {
            if (keys.Count == 0) 
                return KeyCode.None;
            return keys.Pop();
        }
        public static KeyCode Peek()
        {
            if (keys.Count == 0)
                return KeyCode.None;
            return keys.Peek();
        }
        public static bool TryPop(out KeyCode code)
        {
            code = KeyCode.None;
            if (keys.Count == 0) return false;
            code = keys.Pop();
            return true;
        }
        public static int Count => keys.Count;
        public static void Flush() => keys.Clear();
        static bool flushed = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrCountdown), "ShowGetReady")]
        static void SGRPostfix() => flushed = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "UpdateInput")]
        static void UIPostfix(scrController __instance)
        {
            if (!flushed && __instance.state == States.PlayerControl)
            {
                Flush();
                flushed = true;
            }
            if (AsyncInputManager.isActive) return;
            foreach (var key in Main.KeyManager.Codes)
                if (Input.GetKeyDown(key)) keys.Push(key);
        }
    }
}
