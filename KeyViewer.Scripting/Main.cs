using JSNet;
using System;
using System.IO;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace KeyViewer.Scripting
{
    public static class Main
    {
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            Mod.OnToggle = OnToggle;
        }
        public static bool OnToggle(ModEntry modEntry, bool toggle)
        {
            if (toggle)
            {
                KeyViewer.Main.OnManagersInitialized += RunScripts;
                API.Initialize();
            }
            else
            {
                API.Release();
                KeyViewer.Main.OnManagersInitialized -= RunScripts;
            }
            return true;
        }
        public static void RunScripts()
        {
            foreach (var script in Directory.GetFiles(Path.Combine(Mod.Path, "Scripts"), "*.js"))
            {
                if (Path.GetFileName(script) == "Impl.js") continue;
                Script s = null;
                try
                {
                    s = Script.InterpretAPI(API.api, File.ReadAllText(script));
                    s.Exec();
                }
                catch (Exception e) { Logger.Log($"Error On Executing Script ({Path.GetFileName(script)}):\n{e}"); }
                finally { s?.Dispose(); }
            }
        }
    }
}
