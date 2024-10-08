﻿using HarmonyLib;
using KeyViewer.Core.TextReplacing;
using Overlayer.Tags;
using System.Collections.Generic;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace KeyViewer.OverlayerAdapter
{
    public static class Main
    {
        public static Dictionary<MethodInfo, MethodInfo> patchedGetters = new Dictionary<MethodInfo, MethodInfo>();
        public static Harmony Harmony { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            Harmony = new Harmony(modEntry.Info.Id);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public static Tag InteropTag(OverlayerTag t)
        {
            var newTag = new Tag(t.Name);
            var go = t.Tag.GetterOriginal;
            if (go.Name == "Invoke")
                newTag.SetGetter(t.Tag.GetterOriginalDelegate);
            else newTag.SetGetter(go);
            if (!t.NotPlaying) PatchGetter(newTag.Getter);
            return newTag;
        }
        public static void PatchGetter(MethodInfo getter)
        {
            if (!patchedGetters.TryGetValue(getter, out _))
                patchedGetters[getter] = Harmony.Patch(getter, new HarmonyMethod(TP));
        }
        public static readonly MethodInfo TP = typeof(Main).GetMethod(nameof(TagPatcher));
        public static bool TagPatcher() => Overlayer.Main.IsPlaying;
    }
}
