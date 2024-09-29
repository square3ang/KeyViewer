using HarmonyLib;
using KeyViewer.Core.Input;
using KeyViewer.Migration.V2;
using System.Collections.Generic;
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
            var result = GetPressedCounts((MainStateCount)GetStateCountMethodSync(__instance, state), false, state);
            if (result < 0) return;
            __result = result;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDInputType_AsyncKeyboard), "Main")]
        public static void Async(RDInputType_AsyncKeyboard __instance, ref int __result, ButtonState state)
        {
            var result = GetPressedCounts((MainStateCount)GetStateCountMethodAsync(__instance, state), true, state);
            if (result < 0) return;
            __result = result;
        }
        private static int GetPressedCounts(MainStateCount stateCount, bool async, ButtonState bs)
        {
            var profiles = Main.Managers.Select(m => m.Value.profile);
            profiles = profiles.Where(p => p.LimitNotRegisteredKeys);
            if (!profiles.Any()) return -1; 
            var activeKeys = profiles.SelectMany(p => p.Keys).Select(k => k.Code).Where(c => c != KeyCode.None).Distinct();
            int count = 0;
            if (async)
                count = stateCount.keys.Where(k => activeKeys.Contains(AsyncInputCompat.Convert(((AsyncKeyCode)k.value).label))).Count();
            else count = stateCount.keys.Where(k => activeKeys.Contains((KeyCode)k.value)).Count();
            if (Main.IsWindows)
            {
                foreach (var mapping in WinInput.GetMappings().Where(k => activeKeys.Contains(k)))
                {
                    if (bs == ButtonState.IsDown && WinInput.GetState(mapping)) count++;
                    if (bs == ButtonState.WentDown && WinInput.IsDown(mapping)) count++;
                    if (bs == ButtonState.WentUp && WinInput.IsUp(mapping)) count++;
                }
            }
            return count;
        }
    }
}
