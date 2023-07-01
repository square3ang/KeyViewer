using HarmonyLib;
using UnityEngine;
using System;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scrMistakesManager))]
    public static class JudgementColorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddHit")]
        public static void AHPrefix(HitMargin hit)
        {
            while (InputStack.TryPop(out KeyCode code))
            {
                Main.KeyManager[code].ChangeHitMarginColor(hit);
            }
            InputStack.Flush();
        }
        [HarmonyPostfix]
        [HarmonyPatch("Reset")]
        public static void RPostfix() => InputStack.Flush();
    }
}
