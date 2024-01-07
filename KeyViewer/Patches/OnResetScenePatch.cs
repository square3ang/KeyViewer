using HarmonyLib;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scnGame), "ResetScene")]
    public static class OnResetScenePatch
    {
        public static void Postfix()
        {
            foreach (var manager in Main.Managers.Values)
            {
                foreach (var key in manager.keys)
                {
                    key.Pressed = false;
                    key.ResetRains();
                }
            }
        }
    }
}
