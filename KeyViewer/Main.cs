using HarmonyLib;
using JSON;
using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Core.Translation;
using KeyViewer.Migration.V3;
using KeyViewer.Models;
using KeyViewer.Patches;
using KeyViewer.Unity;
using KeyViewer.Utils;
using KeyViewer.Views;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

/* TODO List
 * 지원 가능한 모든 곳에 Gradient 기능 추가하기 => Completed
 * Rain이 위 아래로 늘어나는 버그 고치기 => Completed
 * 비동기 입력 Issue 고치기 => 99% Completed
 * 키 별 KPS 기능 추가하기 => Completed
 * 모든 텍스트에 Tag를 이용한 Text Replacing 지원하기 => Completed
 * 이미지 Rounding 지원하기 => Completed
 * 크기 조절 정렬 버그 고치기 => Completed
 * 
 * Maybe TODO List
 * Rain이 켜져있을 때 키를 누르면 파티클 효과 추가해보기
 */

namespace KeyViewer
{
    public static class Main
    {
        public static bool IsEnabled { get; private set; }
        public static bool IsPlaying { get; private set; }
        public static Language Lang { get; internal set; }
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Settings Settings { get; private set; }
        public static Dictionary<string, KeyManager> Managers { get; private set; }
        public static bool BlockInput { get; internal set; }
        public static ModelDrawable<Profile> ListeningDrawer { get; internal set; }
        public static Harmony Harmony { get; private set; }
        public static GUIController GUI { get; private set; }
        public static HttpClient HttpClient { get; private set; }
        public static System.Version LastestVersion { get; private set; }
        public static System.Version ModVersion { get; private set; }
        public static string DiscordLink { get; private set; }
        public static string DownloadLink { get; private set; }
        public static bool HasUpdate { get; private set; }
        public static bool WebAPIInitialized { get; private set; } = false;
        public static event System.Action OnManagersInitialized = delegate { };
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
            HttpClient = new HttpClient();
            modEntry.Info.Version = Constants.Version;
            typeof(ModEntry).GetField(nameof(ModEntry.Version)).SetValue(modEntry, ModVersion = System.Version.Parse(Constants.Version));
        }
        public static bool OnToggle(ModEntry modEntry, bool toggle)
        {
            if (toggle)
            {
                InitializeWebAPI();
                Tag.InitializeWrapperAssembly();
                FontManager.Initialize();
                AssetManager.Initialize();
                JudgementColorPatch.Initialize();
                GUI = new GUIController();
                Settings = new Settings();
                if (File.Exists(Constants.SettingsPath))
                    Settings.Deserialize(JsonNode.Parse(File.ReadAllText(Constants.SettingsPath)));
                Managers = new Dictionary<string, KeyManager>();
                List<string> notExistProfiles = new List<string>();
                foreach (var profile in Settings.ActiveProfiles)
                {
                    if (!AddManager(profile))
                        notExistProfiles.Add(profile.Name);
                }
                Settings.ActiveProfiles.RemoveAll(p => notExistProfiles.Contains(p.Name));
                if (!Settings.ActiveProfiles.Any())
                {
                    File.WriteAllText(Path.Combine(Mod.Path, "Default.json"),
                        new Profile().Serialize().ToString(4));
                    var def = new ActiveProfile("Default", true);
                    Settings.ActiveProfiles.Add(def);
                    AddManager(def);
                }
                Lang = Language.GetLanguage(Settings.Language);
                Language.OnInitialize += OnLanguageInitialize;
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
                StaticCoroutine.Run(InitializeManagersCo());
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
                ReleaseManagers();
                Harmony.UnpatchAll(Harmony.Id);
                Harmony = null;
                Language.OnInitialize -= OnLanguageInitialize;
                JudgementColorPatch.Release();
                //AssetManager.Release();
                FontManager.Release();
                Tag.ReleaseWrapperAssembly();
                System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced, true);
                Resources.UnloadUnusedAssets();
            }
            return true;
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {
            if (ListeningDrawer != null)
                foreach (var code in EnumHelper<KeyCode>.GetValues())
                    if (Input.GetKeyDown(code))
                        ListeningDrawer.OnKeyDown(code);
            bool showViewer = true;
            foreach (var manager in Managers.Values)
            {
                if (scrController.instance && scrConductor.instance)
                    IsPlaying = !scrController.instance.paused && scrConductor.instance.isGameWorld;
                if (manager.profile.ViewOnlyGamePlay)
                    showViewer = IsPlaying;
                if (showViewer != manager.gameObject.activeSelf)
                    manager.gameObject.SetActive(showViewer);
            }
        }
        public static void OnGUI(ModEntry modEntry)
        {
            if (!Lang.Initialized)
                Drawer.ButtonLabel("Preparing...", KeyViewerUtils.OpenDiscordUrl);
            else GUI.Draw();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            JsonNode settingsNode = Settings.Serialize();
            File.WriteAllText(Constants.SettingsPath, settingsNode.ToString(4));
            foreach (var (name, manager) in Managers)
            {
                Profile p = manager.profile;
                File.WriteAllText(Path.Combine(Mod.Path, $"{name}.json"), p.Serialize().ToString(4));
            }
        }
        public static void OnShowGUI(ModEntry modEntry)
        {
            BlockInput = true;
            GUI.Flush();
            ListeningDrawer = null;
        }
        public static void OnHideGUI(ModEntry modEntry)
        {
            GUI.Flush();
            ListeningDrawer = null;
            BlockInput = false;
        }

        public static void OnLanguageInitialize()
        {
            GUI.Flush();
            ListeningDrawer = null;
            GUI.Init(new SettingsDrawer(Settings));
        }

        public static bool AddManager(ActiveProfile profile, bool forceInit = false)
        {
            var profilePath = Path.Combine(Mod.Path, $"{profile.Name}.json");
            if (File.Exists(profilePath))
            {
                if (profile.Active)
                {
                    var profileNode = JsonNode.Parse(File.ReadAllText(profilePath));
                    var p = ProfileImporter.Import(profileNode);
                    if (Managers.TryGetValue(profile.Name, out var manager))
                        Object.Destroy(manager);
                    Managers[profile.Name] = KeyManager.CreateManager(profile.Name, p);
                    if (forceInit)
                    {
                        Managers[profile.Name].Init();
                        Managers[profile.Name].UpdateKeys();
                        Logger.Log($"Initialized Key Manager {profile.Name}.");
                    }
                }
                return true;
            }
            return false;
        }
        public static void RemoveManager(ActiveProfile profile)
        {
            if (Managers.TryGetValue(profile.Name, out var manager))
            {
                Object.Destroy(manager.gameObject);
                Managers.Remove(profile.Name);
                Logger.Log($"Released Key Manager {profile.Name}.");
            }
        }
        public static IEnumerator InitializeManagersCo()
        {
            if (AssetManager.Initialized)
            {
                foreach (var (name, manager) in Managers)
                {
                    manager.Init();
                    manager.UpdateKeys();
                    Logger.Log($"Initialized Key Manager {name}.");
                    yield return null;
                }
                OnManagersInitialized();
                yield break;
            }
            else
            {
                yield return new WaitUntil(() => !AssetManager.Initialized);
                foreach (var (name, manager) in Managers)
                {
                    manager.Init();
                    manager.UpdateKeys();
                    Logger.Log($"Initialized Key Manager {name}.");
                    yield return null;
                }
                OnManagersInitialized();
                yield break;
            }
        }
        public static void ReleaseManagers()
        {
            foreach (var (name, manager) in Managers)
            {
                Object.Destroy(manager.gameObject);
                Logger.Log($"Released Key Manager {name}.");
            }
            Managers = null;
        }
        public static void ResetKeys()
        {
            foreach (var manager in Managers.Values)
            {
                foreach (var key in manager.keys)
                {
                    if (!key) continue;
                    key.Pressed = false;
                    key.ResetRains();
                }
            }
        }
        public static async void InitializeWebAPI()
        {
            if (WebAPIInitialized) return;
            Logger.Log($"Handshake Response:{await KeyViewerWebAPI.Handshake()}");
            LastestVersion = await KeyViewerWebAPI.GetVersion();
            DiscordLink = await KeyViewerWebAPI.GetDiscordLink();
            DownloadLink = await KeyViewerWebAPI.GetDownloadLink();
            StaticCoroutine.Queue(StaticCoroutine.SyncRunner(EnsureKeyViewerVersion));
            WebAPIInitialized = true;
        }
        public static void EnsureKeyViewerVersion()
        {
            if (LastestVersion > ModVersion)
            {
                Lang.ActivateUpdateMode();
                ErrorCanvasContext ecc = new ErrorCanvasContext();
                ecc.titleText = "WOW YOUR KEYVIEWER VERSION IS BEAUTIFUL!";
                ecc.errorMessage =
                    $"Current KeyViewer Version v{ModVersion}.\n" +
                    $"But Latest KeyViewer Is v{LastestVersion}.\n" +
                    $"PlEaSe UpDaTe YoUr KeYvIeWeR!";
                ecc.ignoreBtnCallback = () =>
                {
                    ADOUtils.HideError(ecc);
                    KeyViewerUtils.OpenDownloadUrl();
                };
                ADOUtils.ShowError(ecc);
            }
        }
        public static void MigrateFromV3Xml(string path)
        {
            XmlSerializer serializer;
            try
            {
                serializer = new XmlSerializer(typeof(V3Settings), GetXAO(true));
                var v3s = serializer.Deserialize(File.OpenRead(path)) as V3Settings;
                var newSettings = V3Migrator.Migrate(v3s, out var profilesNode);
                foreach (var (name, manager) in Managers)
                {
                    Object.Destroy(manager.gameObject);
                    Logger.Log($"Released Key Manager {name}.");
                }
                Managers.Clear();
                for (int i = 0; i < newSettings.ActiveProfiles.Count; i++)
                {
                    var profile = newSettings.ActiveProfiles[i];
                    File.WriteAllText(Path.Combine(Mod.Path, $"{profile.Name}.json"), profilesNode[i].ToString(4));
                    AddManager(profile, true);
                }
                GUI.Flush();
                GUI.Init(new SettingsDrawer(Settings = newSettings));
                Logger.Log($"Successfully Migrated Settings Xml '{path}'");
            }
            catch (System.Exception e)
            {
                try
                {
                    serializer = new XmlSerializer(typeof(V3Profile), GetXAO(false));
                    var v3p = serializer.Deserialize(File.OpenRead(path)) as V3Profile;
                    var profile = V3Migrator.MigrateProfile(v3p);
                    File.WriteAllText(Path.Combine(Mod.Path, $"{v3p.Name}.json"), profile.Serialize().ToString(4));
                    var activeProfile = new ActiveProfile(v3p.Name, true);
                    Settings.ActiveProfiles.Add(activeProfile);
                    AddManager(activeProfile, true);
                    Logger.Log($"Successfully Migrated Profile Xml '{path}'");
                }
                catch (System.Exception ee) { Logger.Log($"Failed To Migrate Xml..\n{e}\n\n{ee}"); }
            }
        }
        public static V3Settings ReadV3Settings(string path)
        {
            var serializer = new XmlSerializer(typeof(V3Settings), GetXAO(true));
            return serializer.Deserialize(File.OpenRead(path)) as V3Settings;
        }
        public static V3Profile ReadV3Profile(string path)
        {
            var serializer = new XmlSerializer(typeof(V3Profile), GetXAO(true));
            return serializer.Deserialize(File.OpenRead(path)) as V3Profile;
        }
        private static XmlAttributeOverrides GetXAO(bool settings)
        {
            XmlAttributeOverrides xao = new XmlAttributeOverrides();

            if (settings)
            {
                XmlAttributes settingsAttr = new XmlAttributes();
                settingsAttr.XmlRoot = new XmlRootAttribute("Settings");
                xao.Add(typeof(V3Settings), settingsAttr);
            }
            else
            {
                XmlAttributes profileAttr = new XmlAttributes();
                profileAttr.XmlRoot = new XmlRootAttribute("Profile");
                xao.Add(typeof(V3Profile), profileAttr);
            }

            return xao;
        }
    }
}
