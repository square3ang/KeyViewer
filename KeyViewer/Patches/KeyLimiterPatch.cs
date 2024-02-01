using HarmonyLib;
using KeyViewer.Core.Input;
using KeyViewer.Models;
using System.Linq;
using UnityEngine;
using static RDInputType;

namespace KeyViewer.Patches
{
    [HarmonyPatch]
    public static class KeyLimiterPatch
    {
        private static readonly FastInvokeHandler GetStateCountMethodSync = MethodInvoker.GetHandler(AccessTools.Method(typeof(RDInputType_Keyboard), "GetStateCount"));
        private static readonly FastInvokeHandler GetStateCountMethodAsync = MethodInvoker.GetHandler(AccessTools.Method(typeof(RDInputType_AsyncKeyboard), "GetStateCount"));
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDInputType_Keyboard), "Main")]
        public static void Sync(RDInputType_Keyboard __instance, ref int __result, ButtonState state)
        {
            var result = GetPressedCounts((MainStateCount)GetStateCountMethodSync(__instance, state), false);
            if (result < 0) return;
            __result = result;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDInputType_AsyncKeyboard), "Main")]
        public static void Async(RDInputType_AsyncKeyboard __instance, ref int __result, ButtonState state)
        {
            var result = GetPressedCounts((MainStateCount)GetStateCountMethodAsync(__instance, state), true);
            if (result < 0) return;
            __result = result;
        }
        private static int GetPressedCounts(MainStateCount stateCount, bool async)
        {
            var profiles = Main.Managers.Select(m => m.Value.profile);
            profiles = profiles.Where(p => p.LimitNotRegisteredKeys);
            if (!profiles.Any()) return -1;
            return profiles.Sum(p => GetPressedCount(p, stateCount, async));
        }
        private static int GetPressedCount(Profile profile, MainStateCount stateCount, bool async)
        {
            if (Main.BlockInput) return 0;
            var activeKeys = profile.Keys
                .Select(kc => kc.Code)
                .Where(k => k != KeyCode.None)
                .Distinct();
            if (async)
                return stateCount.keys.Where(k => activeKeys.Contains(AsyncInputCompat.Convert(((AsyncKeyCode)k.value).label))).Count();
            return stateCount.keys.Where(k => activeKeys.Contains((KeyCode)k.value)).Count();
        }
    }
}
