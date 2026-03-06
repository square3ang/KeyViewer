using HarmonyLib;
using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.Input;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Core.Translation;
using KeyViewer.Migration.V3;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using KeyViewer.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Overlayer.Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Object = UnityEngine.Object;

namespace KeyViewer;

public static class Main {
    public static bool IsEnabled { get; private set; }
    public static bool IsPlaying { get; private set; }
    public static Translator Lang { get; internal set; }
    public static ModEntry Mod { get; private set; }
    public static ModLogger Logger { get; private set; }
    public static Settings Settings { get; private set; }
    public static Dictionary<string, KeyManager> Managers { get; private set; }
    public static ModelDrawable<Profile> ListeningDrawer { get; internal set; }
    public static Harmony Harmony { get; private set; }
    public static GUIController GUI { get; private set; }
    public static HashSet<string> ToDeleteFiles { get; private set; }
    public static event Action OnManagersInitialized = delegate { };
    public static bool IsWindows { get; private set; }
    public static string ProfilePath;
    public static string Tooltip = "";
    public static void Load(ModEntry modEntry) {
        Mod = modEntry;
        ProfilePath = Path.Combine(Mod.Path, "profiles");
        Logger = modEntry.Logger;

        GUI = new GUIController();
        Lang = new Translator("0KTL_KEYVIEWER");

        modEntry.OnToggle = OnToggle;
        modEntry.OnUpdate = OnUpdate;
        modEntry.OnGUI = OnGUI;
        modEntry.OnSaveGUI = OnSaveGUI;
        modEntry.OnShowGUI = OnShowGUI;
        modEntry.OnHideGUI = OnHideGUI;
        IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
    public static bool OnToggle(ModEntry modEntry, bool toggle) {
        if(toggle) {
            Settings = new Settings();
            if(File.Exists(Constants.SettingsPath)) {
                var json = JToken.Parse(File.ReadAllText(Constants.SettingsPath));
                Settings.Deserialize(json);
            }

            if(IsWindows && Settings.UseWindowsAsyncInput) {
                WinInput.StartPolling(Settings.PollingRate);
            }

            WinInput.OnKeyDown += code => {
                MainThreadDispatcher.Enqueue(() => {
                    KeyCode k = WinInput.IntToKeyCode(code);
                    if(k != KeyCode.None) {
                        ListeningDrawer?.OnKeyDown(k);
                    }
                });
            };

            Tag.InitializeWrapperAssembly();
            FontManager.Initialize();
            AssetManager.Initialize();

            Managers = [];
            ToDeleteFiles = [];

            if(!Directory.Exists(ProfilePath)) {
                Directory.CreateDirectory(ProfilePath);
            }

            var profileFiles = Directory.GetFiles(ProfilePath, "*.json");
            List<string> notExistProfiles = [];

            foreach(var file in profileFiles) {
                try {
                    var profileName = Path.GetFileNameWithoutExtension(file);

                    var existingProfile = Settings.ActiveProfiles.FirstOrDefault(p => p.Name == profileName);
                    if(existingProfile.Name != default(ActiveProfile).Name) {
                        AddManager(existingProfile);
                        continue;
                    }

                    var profileJsonNew = File.ReadAllText(file);
                    var profileDataNew = JsonConvert.DeserializeObject<Profile>(profileJsonNew);

                    var newActiveProfile = new ActiveProfile(profileName, existingProfile.Active);
                    Settings.ActiveProfiles.Add(newActiveProfile);

                    if(!AddManager(newActiveProfile)) {
                        notExistProfiles.Add(profileName);
                    }
                } catch(Exception ex) {
                    Logger.Log($"Failed to load profile {file}: {ex.Message}");
                    notExistProfiles.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            Settings.ActiveProfiles.RemoveAll(p => notExistProfiles.Contains(p.Name));

            Lang.Language = Settings.Lang;
            Lang.OnInitialize += OnLanguageInitialize;
            var settingsDrawer = new SettingsDrawer(Settings);
            Lang.OnInitialize += () => settingsDrawer.NeedLangInit = true;
            _ = Lang.Load(Path.Combine(Mod.Path, "lang"));

            Harmony = new Harmony(modEntry.Info.Id);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            StaticCoroutine.Run(InitializeManagersCo());

            DllImporter.NCalcInitialize();

            GUI.Init(settingsDrawer);
            GUI.Flush();

            ListeningDrawer = null;
            IsEnabled = true;
        } else {
            IsEnabled = false;
            ReleaseManagers();
            ToDeleteFiles = null;
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
            if(IsWindows) {
                WinInput.Dispose();
            }
            //AssetManager.Release();
            FontManager.Release();
            Tag.ReleaseWrapperAssembly();
            Resources.UnloadUnusedAssets();
            Lang.Release();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }
        return true;
    }
    public static class MainThreadDispatcher {
        private static readonly ConcurrentQueue<Action> queue = new();
        public static void Enqueue(Action action) {
            queue.Enqueue(action);
        }
        public static void Update() {
            while(queue.TryDequeue(out var action)) {
                action.Invoke();
            }
        }
    }
    public static void OnUpdate(ModEntry modEntry, float deltaTime) {
        if(scrController.instance && scrConductor.instance) {
            IsPlaying = !scrController.instance.paused && scrConductor.instance.isGameWorld;
        }

        if(ListeningDrawer != null && (!IsWindows || !Settings.UseWindowsAsyncInput)) {
            foreach(KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                if(KeyInput.GetKeyDown(code)) {
                    ListeningDrawer.OnKeyDown(code);
                }
            }
        }

        foreach(var manager in Managers.Values) {
            bool showViewer = true;
            if(manager.profile.ViewOnlyGamePlay) {
                showViewer = IsPlaying;
            }

            if(showViewer != manager.gameObject.activeSelf) {
                manager.gameObject.SetActive(showViewer);
            }
        }

        MainThreadDispatcher.Update();
    }
    public static void OnGUI(ModEntry modEntry) {
        GUI.Draw();
        if(Settings.UseTooltip) {
            Drawer.Tooltip(Tooltip);
            Tooltip = null;
        }
    }
    public static void OnSaveGUI(ModEntry modEntry) {
        File.WriteAllText(Constants.SettingsPath, Settings.Serialize().ToString());
        foreach(var (name, manager) in Managers) {
            File.WriteAllText(Path.Combine(ProfilePath, $"{name}.json"), manager.profile.Serialize().ToString());
        }
        foreach(var path in ToDeleteFiles) {
            File.Delete(path);
        }
    }
    public static void OnShowGUI(ModEntry modEntry) {
        GUI.Flush();
        ListeningDrawer = null;
    }
    public static void OnHideGUI(ModEntry modEntry) {
        GUI.Flush();
        ListeningDrawer = null;
    }
    public static void OnLanguageInitialize() {
        string[] translatorLogs = Lang.Logs;
        if(translatorLogs != null && translatorLogs.Length > 0) {
            foreach(var log in translatorLogs) {
                Logger.Log(log);
            }
        }
    }
    public static bool AddManager(ActiveProfile profile, bool forceInit = false) {
        var profilePath = Path.Combine(ProfilePath, $"{profile.Name}.json");
        if(File.Exists(profilePath)) {
            if(profile.Active) {
                var profileJson = JToken.Parse(File.ReadAllText(profilePath));
                var p = ProfileImporter.Import(profileJson);

                if(Managers.TryGetValue(profile.Name, out var manager)) {
                    Object.Destroy(manager);
                }

                Managers[profile.Name] = KeyManager.CreateManager(profile.Name, p);

                if(forceInit) {
                    Managers[profile.Name].Init();
                    Managers[profile.Name].UpdateKeys();
                    Logger.Log($"Initialized Key Manager {profile.Name}.");
                }
            }
            return true;
        }
        return false;
    }
    public static void RemoveManager(ActiveProfile profile) {
        if(Managers.TryGetValue(profile.Name, out var manager)) {
            Object.Destroy(manager.gameObject);
            Managers.Remove(profile.Name);
            Logger.Log($"Released Key Manager {profile.Name}.");
        }
    }
    public static (KeyManager manager, ActiveProfile activeProfile) CreateManagerImmediate(string name, Profile p, string key = null) {
        var profile = new ActiveProfile(name, true, key);
        var manager = KeyManager.CreateManager(profile.Name, p);
        manager.Init();
        manager.UpdateKeys();
        Managers[name] = manager;
        Logger.Log($"Initialized Key Manager {profile.Name}.");
        return (manager, profile);
    }
    public static IEnumerator InitializeManagersCo() {
        if(!AssetManager.Initialized) {
            yield return new WaitUntil(() => !AssetManager.Initialized);
        }

        foreach(var (name, manager) in Managers) {
            var elapsed = MiscUtils.MeasureTime(() => {
                manager.Init();
                manager.UpdateKeys();
            });
            Logger.Log($"Initialized Key Manager {name}. ({elapsed.TotalMilliseconds}ms)");
            yield return null;
        }
        OnManagersInitialized();
        yield break;
    }
    public static void ReleaseManagers() {
        foreach(var (name, manager) in Managers) {
            Object.Destroy(manager.gameObject);
            Logger.Log($"Released Key Manager {name}.");
        }
        Managers = null;
    }
    public static void ResetKeys() {
        foreach(var manager in Managers.Values) {
            foreach(var key in manager.keys) {
                if(!key) {
                    continue;
                }

                key.Pressed = false;
                key.ResetRains();
            }
        }
    }
    public static void MigrateFromV3Xml(string path) {
        XmlSerializer serializer;
        try {
            serializer = new XmlSerializer(typeof(V3Settings), GetXAO(true));
            var v3s = serializer.Deserialize(File.OpenRead(path)) as V3Settings;
            var newSettings = V3Migrator.Migrate(v3s, out var profilesNode);
            foreach(var (name, manager) in Managers) {
                Object.Destroy(manager.gameObject);
                Logger.Log($"Released Key Manager {name}.");
            }
            Managers.Clear();
            for(int i = 0; i < newSettings.ActiveProfiles.Count; i++) {
                var profile = newSettings.ActiveProfiles[i];
                File.WriteAllText(Path.Combine(ProfilePath, $"{profile.Name}.json"), profilesNode[i].ToString());
                AddManager(profile, true);
            }
            GUI.Flush();
            GUI.Init(new SettingsDrawer(Settings = newSettings));
            Logger.Log($"Successfully Migrated Settings Xml '{path}'");
        } catch(Exception e) {
            try {
                serializer = new XmlSerializer(typeof(V3Profile), GetXAO(false));
                var v3p = serializer.Deserialize(File.OpenRead(path)) as V3Profile;
                var profile = V3Migrator.MigrateProfile(v3p);
                File.WriteAllText(Path.Combine(ProfilePath, $"{v3p.Name}.json"), profile.Serialize().ToString());
                var activeProfile = new ActiveProfile(v3p.Name, true);
                Settings.ActiveProfiles.Add(activeProfile);
                AddManager(activeProfile, true);
                Logger.Log($"Successfully Migrated Profile Xml '{path}'");
            } catch(Exception ee) { Logger.Log($"Failed To Migrate Xml..\n{e}\n\n{ee}"); }
        }
    }
    public static V3Settings ReadV3Settings(string path) {
        var serializer = new XmlSerializer(typeof(V3Settings), GetXAO(true));
        return serializer.Deserialize(File.OpenRead(path)) as V3Settings;
    }
    public static V3Profile ReadV3Profile(string path) {
        var serializer = new XmlSerializer(typeof(V3Profile), GetXAO(true));
        return serializer.Deserialize(File.OpenRead(path)) as V3Profile;
    }
    private static XmlAttributeOverrides GetXAO(bool settings) {
        XmlAttributeOverrides xao = new();

        if(settings) {
            XmlAttributes settingsAttr = new() {
                XmlRoot = new XmlRootAttribute("Settings")
            };
            xao.Add(typeof(V3Settings), settingsAttr);
        } else {
            XmlAttributes profileAttr = new() {
                XmlRoot = new XmlRootAttribute("Profile")
            };
            xao.Add(typeof(V3Profile), profileAttr);
        }

        return xao;
    }
}
