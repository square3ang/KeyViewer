using HarmonyLib;
using KeyViewer.Core.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
    public static class CountValidKeysPressedPatch
    {
        public static readonly List<KeyCode> AlwaysBoundKeys = new List<KeyCode>()
        {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
        };
        public static bool Prefix(ref int __result)
        {
            if (Main.BlockInput)
            {
                __result = 0;
                return false;
            }
            var profile = Main.Managers.FirstOrDefault().Value?.profile;
            if (profile == null || !profile.LimitNotRegisteredKeys) return true;

            // From AdofaiTweaks.KeyLimiterTweak
            int keysPressed = 0;
            if (AsyncInputManager.isActive)
            {
                // Check registered keys
                keysPressed += profile.Keys.Count(k => AsyncInputCompat.GetKeyDown(k.Code))
                               // Always account for certain keys
                               + AlwaysBoundKeys.Count(AsyncInputCompat.GetKeyDown);
            }
            else
            {
                // Check registered keys
                keysPressed += profile.Keys.Count(k => Input.GetKeyDown(k.Code))
                               // Always account for certain keys
                               + AlwaysBoundKeys.Count(Input.GetKeyDown);
            }

            // Limit keys pressed
            __result = Mathf.Min(4, keysPressed);
            return false;
        }
    }
}
