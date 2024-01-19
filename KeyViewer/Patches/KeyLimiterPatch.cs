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
            var profile = Main.Managers.FirstOrDefault().Value?.profile;
            if (profile == null) return;
            if (!profile.LimitNotRegisteredKeys) return;
            __result = GetPressedCount(profile, (MainStateCount)GetStateCountMethodSync(__instance, state), false);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDInputType_AsyncKeyboard), "Main")]
        public static void Async(RDInputType_AsyncKeyboard __instance, ref int __result, ButtonState state)
        {
            var profile = Main.Managers.FirstOrDefault().Value?.profile;
            if (profile == null) return;
            if (!profile.LimitNotRegisteredKeys) return;
            __result = GetPressedCount(profile, (MainStateCount)GetStateCountMethodAsync(__instance, state), true);
        }
        private static int GetPressedCount(Profile profile, MainStateCount stateCount, bool async)
        {
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
