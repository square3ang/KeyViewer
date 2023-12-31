using JSON;
using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using KeyViewer.Views;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace KeyViewer
{
    public static class Main
    {
        public static Language Lang { get; internal set; }
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
                Managers = new Dictionary<string, KeyManager>();
                if (!Settings.ActiveProfiles.Any())
                {
                    File.WriteAllText(Path.Combine(Mod.Path, "Default.json"), 
                        new Profile().Serialize().ToString(4));
                    Settings.ActiveProfiles.Add(new ActiveProfile("Default", true));
                }
                List<string> notExistProfiles = new List<string>();
                foreach (var profile in Settings.ActiveProfiles)
                {
                    if (!AddManager(profile))
                        notExistProfiles.Add(profile.Name);
                }
                Settings.ActiveProfiles.RemoveAll(p => notExistProfiles.Contains(p.Name));
                Lang = Language.GetLanguage(Settings.Language);
                Lang.OnDownload(() => GUIController.Init(Lang[TranslationKeys.Settings.Prefix], new SettingsDrawer(Settings)));
            }
            else
            {
                // TODO: Save Active Profiles And Others...
                Language.Release();
                AssetManager.Release();
                FontManager.Release();
                Tag.DisposeWrapperAssembly();
                Resources.UnloadUnusedAssets();
            }
            return true;
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {

        }
        public static void OnGUI(ModEntry modEntry)
        {
            if (!Lang.Initialized)
                GUILayout.Label("Preparing...");
            else GUIController.Draw();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {

        }
        public static void OnShowGUI(ModEntry modEntry)
        {
            GUIController.Flush();
        }
        public static void OnHideGUI(ModEntry modEntry)
        {
            GUIController.Flush();
        }

        public static bool AddManager(ActiveProfile profile)
        {
            var profilePath = Path.Combine(Mod.Path, $"{profile.Name}.json");
            if (File.Exists(profilePath))
            {
                if (profile.Active)
                {
                    var profileNode = JsonNode.Parse(File.ReadAllText(profilePath));
                    var p = ModelUtils.Unbox<Profile>(profileNode);
                    Managers[profile.Name] = KeyManager.CreateManager(profile.Name, p);
                }
                return true;
            }
            return false;
        }
        public static void RemoveManager(ActiveProfile profile)
        {
            if (Managers.TryGetValue(profile.Name, out var manager))
            {
                Object.Destroy(manager);
                Managers.Remove(profile.Name);   
            }
        }
    }
}
