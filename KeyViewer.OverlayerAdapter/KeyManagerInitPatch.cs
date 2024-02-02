using HarmonyLib;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Unity;
using Overlayer.Core;
using System.Linq;

namespace KeyViewer.OverlayerAdapter
{
    [HarmonyPatch(typeof(KeyManager), "Init")]
    public static class KeyManagerInitPatch
    {
        public static void Postfix(KeyManager __instance)
        {
            __instance.AllTags.AddRange(
                TagManager.All.Select(t =>
                {
                    var tag = new Tag(t.Name);
                    var go = t.GetterOriginal;
                    if (go.Name == "Invoke")
                        return tag.SetGetter(t.GetterOriginalDelegate);
                    else return tag.SetGetter(go);
                }));
        }
    }
}
