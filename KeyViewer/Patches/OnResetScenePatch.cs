using HarmonyLib;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scnGame), "ResetScene")]
    public static class OnResetScenePatch
    {
        public static void Postfix()
        {
            Main.ResetKeys();
        }
    }
}
