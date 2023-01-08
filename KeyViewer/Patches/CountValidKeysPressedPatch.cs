using HarmonyLib;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
    public static class CountValidKeysPressedPatch
    {
        public static bool Prefix(ref int __result)
        {
            if (Main.IsListening)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
