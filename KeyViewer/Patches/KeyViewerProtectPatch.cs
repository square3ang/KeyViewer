using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(Memory), "DetourMethod")]
    public static class KeyViewerProtectPatch
    {
        static List<(MethodBase, MethodBase)> origRepl = new List<(MethodBase, MethodBase)>();
        public static bool Prefix(MethodBase original, MethodBase replacement)
        {
            bool result = true;
            Assembly ass = original.DeclaringType.Assembly;
            if (ass.GetName().Name.IndexOf(nameof(KeyViewer), StringComparison.OrdinalIgnoreCase) >= 0) result = false;
            if (ass.GetName().Name.IndexOf(nameof(Harmony), StringComparison.OrdinalIgnoreCase) >= 0) result = false;
            if (!result) Main.Logger.Log($"KeyViewer Was Protected From Patch => Target:{original}, Patch:{replacement}");
            return result;
        }
    }
}
