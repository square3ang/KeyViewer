using HarmonyLib;
using Overlayer.Tags;
using System;
using System.Linq;

namespace KeyViewer.OverlayerAdapter
{
    [HarmonyPatch(typeof(TagManager))]
    public static class TagManagerSetRemovePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetTag")]
        public static void Set(OverlayerTag tag)
        {
            foreach (var manager in KeyViewer.Main.Managers.Values)
            {
                if (!manager.initialized) continue;
                manager.AllTags.Add(Main.InteropTag(tag));
                manager.UpdateLayout();
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("RemoveTag")]
        public static void Remove(string name)
        {
            foreach (var manager in KeyViewer.Main.Managers.Values)
            {
                if (!manager.initialized) continue;
                manager.AllTags.RemoveAll(t => t.Name == name);
                manager.UpdateLayout();
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("Load", new Type[] { typeof(Type) })]
        public static void Load(Type type)
        {
            if (type.FullName != "Overlayer.Scripting.Tags.Expression") return;
            foreach (var manager in KeyViewer.Main.Managers.Values)
            {
                if (!manager.initialized) continue;
                var notAddedTags = TagManager.All.Where(t => manager.AllTags.Count(kt => kt.Name == t.Name) == 0);
                manager.AllTags.AddRange(notAddedTags.Select(Main.InteropTag));
                manager.UpdateLayout();
            }
        }
    }
}
