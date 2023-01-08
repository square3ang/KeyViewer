using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scrController))]
    public static class JudgementColorPatch
    {
		static List<KeyCode> inputCodes = new List<KeyCode>();
        [HarmonyPatch("ValidInputWasTriggered")]
        [HarmonyPrefix]
		public static void VIWTPrefix()
        {
            inputCodes = new List<KeyCode>();
            foreach (var key in Main.KeyManager.Codes)
            {
                if (Input.GetKeyDown(key))
                    inputCodes.Add(key);
            }
        }
		[HarmonyPatch("ShowHitText")]
        [HarmonyPostfix]
        public static void SHTPrefix(HitMargin hitMargin)
        {
            for (int i = 0; i < inputCodes.Count; i++)
                Main.KeyManager[inputCodes[i]].ChangeHitMarginColor(hitMargin);
        }
    }
}
