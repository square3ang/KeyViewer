using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace KeyViewer
{
    public static class EnumPatcher<T> where T : Enum
    {
        private static readonly Type thisType = typeof(T);
        private static readonly Dictionary<string, ulong> addedFields = new Dictionary<string, ulong>();
        public static void AddField(string name, ulong value)
        {
            addedFields[name] = value;
        }
        static EnumPatcher()
        {
            Main.Harmony.Patch(Main.GCVAN, postfix: new HarmonyMethod(typeof(EnumPatcher<T>).GetMethod(nameof(GCVAN_Patch), (BindingFlags)15420)));
        }
        static void GCVAN_Patch(Type enumType, object __result)
        {
            if (enumType != thisType) return;
            var names = Main.VAN_Names(__result).Concat(addedFields.Keys);
            var values = Main.VAN_Values(__result).Concat(addedFields.Values);
            Main.VAN_Names(__result) = names.ToArray();
            Main.VAN_Values(__result) = values.ToArray();
        }
    }
}
