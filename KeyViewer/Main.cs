using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace KeyViewer
{
    public static class Main
    {
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnHideGUI = OnHideGUI;
        }
        public static bool OnToggle(ModEntry modEntry, bool toggle)
        {
            if (toggle)
            {

            }
            else
            {

            }
            return true;
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {

        }
        public static void OnGUI(ModEntry modEntry)
        {

        }
        public static void OnSaveGUI(ModEntry modEntry)
        {

        }
        public static void OnShowGUI(ModEntry modEntry)
        {

        }
        public static void OnHideGUI(ModEntry modEntry)
        {

        }
    }
}
