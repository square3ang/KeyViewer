using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using SkyHook;
using KeyViewer.API;

namespace KeyViewer.Patches
{
    [HarmonyPatch]
    public static class JudgementColorPatch
    {
        static bool initialized = false;
        static ShiftStack<KeyCode> keys;
        public static void Init()
        {
            if (initialized) return;
            keys = new ShiftStack<KeyCode>(() => Math.Max(Main.KeyManager?.keys.Count ?? 0, 2), KeyCode.None);
            SkyHookManager.KeyUpdated.AddListener(she =>
            {
                try
                {
                    if (!AsyncInputManager.isActive) return;
                    if (Main.KeyManager == null) return;
                    if (she.Type == SkyHook.EventType.KeyReleased) return;
                    if (Main.IsEnabled && (scrController.instance?.gameworld ?? false))
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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "ValidInputWasTriggered")]
        public static void SyncInput_StackPushPatch(scrController __instance)
        {
            if (AsyncInputManager.isActive) return;
            foreach (var code in Main.KeyManager.Codes)
                if (Input.GetKeyDown(code))
                    keys.Push(code);
        }
        static bool stackFlushed = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Update")]
        public static void SyncInput_StackFlushPatch(scrController __instance)
        {
            if (AsyncInputManager.isActive) return;
            if (!stackFlushed && __instance.state == States.PlayerControl)
            {
                keys.Clear();
                stackFlushed = true;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static void SyncInput_StackFlushPatch2(scrController __instance)
        {
            if (AsyncInputManager.isActive) return;
            stackFlushed = false;
        }
        [Comment("Change Key's Background Color")]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
        public static void ChangeColorPatch(HitMargin hit)
        {
            KeyCode code;
            while ((code = keys.Pop()) != KeyCode.None)
                Main.KeyManager[code].ChangeHitMarginColor(hit);
            keys.Clear();
        }
    }
}
