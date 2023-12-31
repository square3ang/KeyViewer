using KeyViewer.Models;
using KeyViewer.Views;
using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Core.Translation;
using KeyViewer.Unity;
using System.Collections.Generic;
using System.IO;
using JSON;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace KeyViewer
{
    public static class Main
    {
        public static Language Lang { get; private set; }
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Settings Settings { get; private set; }
        public static Dictionary<string, KeyManager> Managers { get; private set; }
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
                Tag.InitializeWrapperAssembly();
                FontManager.Initialize();
                AssetManager.Initialize();
                Settings = new Settings();
                if (File.Exists(Constants.SettingsPath))
                    Settings.Deserialize(JsonNode.Parse(File.ReadAllText(Constants.SettingsPath)));
                Lang = Language.GetLanguage(Settings.Language);
                GUIController.Push(Lang[TranslationKeys.Lorem_Ipsum], new SettingsDrawer(Settings));
            }
            else
            {
                // TODO: Save Active Profiles And Others...
                Language.Release();
                AssetManager.Release();
                FontManager.Release();
                Tag.DisposeWrapperAssembly();
            }
            return true;
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {

        }
        public static void OnGUI(ModEntry modEntry)
        {
            GUIController.Draw();
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
