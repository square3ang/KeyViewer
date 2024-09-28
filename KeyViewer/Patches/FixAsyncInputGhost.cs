using HarmonyLib;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(AsyncInputManager), "Update")]
    public static class FixAsyncInputGhost
    {
        public static void Postfix()
        { 
            if (Persistence.GetChosenAsynchronousInput())
                RDInput.asyncKeyboardMouseInput.isActive = true;
        }
    }
}
