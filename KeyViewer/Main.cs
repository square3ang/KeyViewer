using HarmonyLib;
using JSON;
using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using KeyViewer.Patches;
using KeyViewer.Unity;
using KeyViewer.Utils;
using KeyViewer.Views;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

/* TODO List
 * 지원 가능한 모든 곳에 Gradient 기능 추가하기 => Completed
 * Rain이 위 아래로 늘어나는 버그 고치기 => Completed
 * 비동기 입력 Issue 고치기 => FUCK
 * 키 별 KPS 기능 추가하기 => Completed
 * 모든 텍스트에 Tag를 이용한 Text Replacing 지원하기 => Completed
 * 이미지 Rounding 지원하기 => Completed
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
                JudgementColorPatch.Initialize();
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
                Language.Release();
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
            else GUIController.Draw();
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
            GUIController.Flush();
        }
        public static void OnHideGUI(ModEntry modEntry)
        {
            GUIController.Flush();
            BlockInput = false;
        }

        public static void OnLanguageInitialize()
        {
            GUIController.Flush();
            GUIController.Init(new SettingsDrawer(Settings));
        }

        public static bool AddManager(ActiveProfile profile, bool forceInit = false)
        {
            var profilePath = Path.Combine(Mod.Path, $"{profile.Name}.json");
            if (File.Exists(profilePath))
            {
                if (profile.Active)
                {
                    var profileNode = JsonNode.Parse(File.ReadAllText(profilePath));
                    var p = ModelUtils.Unbox<Profile>(profileNode);
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
    }
}
