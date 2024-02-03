using HarmonyLib;
using KeyViewer.Unity;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.OverlayerAdapter
{
    [HarmonyPatch(typeof(KeyManager), "Init")]
    public static class KeyManagerInitPatch
    {
        public static void Postfix(KeyManager __instance)
        {
            __instance.AllTags.AddRange(TagManager.NP.Select(Main.InteropTag));
            __instance.AllTags.AddRange(TagManager.All.Except(TagManager.NP).Select(t => Main.InteropTag(t, false)));
        }
    }
}
