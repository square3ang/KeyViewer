using HarmonyLib;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace KeyViewer.OverlayerAdapter
{
    public static class Main
    {
        public static void Load(ModEntry modEntry)
        {
            new Harmony(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
