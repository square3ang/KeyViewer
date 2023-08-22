using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
            if (Main.IsListening)
            {
                __result = 0;
                return false;
            }
            var profile = Main.KeyManager.Profile;
            if (!profile.LimitNotRegisteredKeys) return true;
            if (!profile.LimitNotRegisteredKeysOnCLS && ADOBase.isCLS) return true;
            if (!profile.LimitNotRegisteredKeysOnMain && !scrController.instance.gameworld && !ADOBase.isCLS) return true;

            // From AdofaiTweaks.KeyLimiterTweak
            int keysPressed = 0;
            if (AsyncInputManager.isActive)
            {
                // Check registered keys
                keysPressed += profile.ActiveKeys.Count(k => AsyncInputCompat.GetKeyDown(k.Code))
                               // Always account for certain keys
                               + AlwaysBoundKeys.Count(AsyncInputCompat.GetKeyDown);
            }
            else
            {
                // Check registered keys
                keysPressed += profile.ActiveKeys.Count(k => Input.GetKeyDown(k.Code))
                               // Always account for certain keys
                               + AlwaysBoundKeys.Count(Input.GetKeyDown);
            }

            // Limit keys pressed
            __result = Mathf.Min(4, keysPressed);
            return false;
        }
    }
}
