using HarmonyLib;
using System;

namespace KeyViewer.Patches
{
    [HarmonyPatch]
    public static class AssassinationPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "Hit")]
        public static void HitPatch(scrController __instance)
        {
            if (ADOBase.sceneName == "scnLevelSelect") return;
            if (__instance.state != States.PlayerControl) return;
            var listFloors = scrLevelMaker.instance.listFloors;
            var sub = listFloors.Count * UnityEngine.Random.Range(0.01f, 0.03f) * UnityEngine.Random.value;
            sub = Math.Max(2, sub);
            var index = listFloors.IndexOf(__instance.currFloor);
            if (index >= listFloors.Count - sub) Main.BlockInput = true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "FailAction")]
        public static void UnblockInputPatch()
        {
            Main.BlockInput = false;
        }
    }
}
