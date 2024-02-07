using HarmonyLib;
using KeyViewer.Unity;
using Overlayer.Tags;
using System.Linq;

namespace KeyViewer.OverlayerAdapter
{
    [HarmonyPatch(typeof(KeyManager), "Init")]
    public static class KeyManagerInitPatch
    {
        public static void Postfix(KeyManager __instance)
        {
            __instance.AllTags.AddRange(TagManager.All.Select(Main.InteropTag));
        }
    }
}
