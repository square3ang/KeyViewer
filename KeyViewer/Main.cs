using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;
using System.IO;
using TMPro;
using System.Threading;
using KeyViewer.API;
using KeyViewer.Migration;
using KeyViewer.Migration.V2;
using SFB;

namespace KeyViewer
{
    public static class Main
    {
        public static ModEntry Mod { get; private set; }
        public static ModEntry.ModLogger Log { get; private set; }
        public static Settings Settings { get; private set; }
        public static Harmony Harmony { get; private set; }
        public static Sprite KeyOutline { get; private set; }
        public static Sprite KeyBackground { get; private set; }
        public static KeyManager KeyManager { get; private set; }
        public static LangManager Lang { get; private set; }
        public static V2MigratorArgument v2Arg = new V2MigratorArgument();
        public static GUIStyle bold;
        public static GUIStyle Bigbold;
        public static string MigrateErrorString = string.Empty;
        public static bool IsMigrating = false;
        public static bool IsListening { get; private set; }
        public static readonly KeyCode[] KeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        public static Thread Wave;
        public static int WaveInterval = 100;

        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Log = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnHideGUI = OnHideGUI;
            AssetBundle assets = AssetBundle.LoadFromFile("Mods/KeyViewer/keyviewer.assets");
            KeyOutline = assets.LoadAsset<Sprite>("Assets/KeyOutline.png");
            KeyBackground = assets.LoadAsset<Sprite>("Assets/KeyBackground.png");
        }
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            if (value)
            {
                Settings = ModSettings.Load<Settings>(modEntry);
                List<Profile> profiles = Settings.Profiles;
                int profileIndex = Settings.ProfileIndex;
                if (!profiles.Any())
                    profiles.Add(new Profile());
                if (profiles.Count <= profileIndex || profileIndex < 0)
                    Settings.ProfileIndex = 0;
                Lang = new LangManager(File.ReadAllText("Mods/KeyViewer/Language.json"));
                Lang.ChangeLanguage(Settings.Language);
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
                KeyManager = new GameObject().AddComponent<KeyManager>();
                if (Settings.ResetWhenStart)
                {
                    foreach (var config in Settings.CurrentProfile.ActiveKeys)
                        config.Count = 0;
                }    
                KeyManager.Init(Settings.CurrentProfile);
            }
            else
            {
                OnSaveGUI(modEntry);
                KeyManager.Profile.calculator.Stop();
                KeyManager?.Dispose();
                KeyManager = null;
                Harmony.UnpatchAll(Harmony.Id);
                Harmony = null;
            }
            return true;
        }
        public static readonly Array languages = Enum.GetValues(typeof(LanguageType));
        public static void OnGUI(ModEntry modEntry)
        {
            if (IsMigrating)
                DrawMigrateMenu();
            else
            {
                GUILayout.BeginHorizontal();
                foreach (LanguageType language in languages)
                    if (GUILayout.Button(Lang.GetLanguageName(language)))
                    {
                        Lang.ChangeLanguage(language);
                        Settings.Language = language;
                    }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                DrawProfileSettingsGUI();
                GUILayout.Space(12f);
                MoreGUILayout.HorizontalLine(1f, 400f);
                GUILayout.BeginHorizontal();
                Settings.ResetWhenStart = GUILayout.Toggle(Settings.ResetWhenStart, Lang.GetString("RESET_WHEN_START"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (Settings.FunActivated)
                    DrawWaveMenu();
                GUILayout.Space(8f);
                DrawGroupSettingsGUI();
                GUILayout.Space(8f);
                DrawKeyRegisterSettingsGUI();
                GUILayout.Space(8f);
                DrawKeyViewerSettingsGUI();
            }
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {
            bool showViewer = true;
            if (scrController.instance && scrConductor.instance)
                KeyManager.isPlaying = !scrController.instance.paused && scrConductor.instance.isGameWorld;
            if (KeyManager.Profile.ViewerOnlyGameplay)
                showViewer = KeyManager.isPlaying;
            if (showViewer != KeyManager.gameObject.activeSelf)
                KeyManager.gameObject.SetActive(showViewer);

            if (!IsListening) return;
            bool changed = false;
            foreach (KeyCode code in KeyCodes)
            {
                int index = -1;
                if (!Input.GetKeyDown(code) || (SKIPPED_KEYS.Contains(code) && !KeyManager.Profile.IgnoreSkippedKeys)) continue;
                if ((index = KeyManager.Profile.ActiveKeys.FindIndex(c => c.Code == code)) != -1)
                {
                    KeyManager.Profile.ActiveKeys.RemoveAt(index);
                    changed = true;
                }
                else
                {
                    KeyManager.Profile.ActiveKeys.Add(new Key.Config(code));
                    changed = true;
                }
            }
            if (changed) KeyManager.UpdateKeys();
        }
        public static void OnHideGUI(ModEntry modEntry)
        {
            IsListening = false;
        }
        private static void DrawWaveMenu()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Wave Invertal:");
            int.TryParse(GUILayout.TextField(WaveInterval.ToString()), out WaveInterval);
            if (GUILayout.Button("Start"))
            {
                try
                {
                    Wave = new Thread(interval =>
                    {
                        InputAPI.Active = true;
                        var activeKeys = KeyManager.Keys;
                        int sleep = (int)interval;
                        while (true)
                        {
                            foreach (var key in activeKeys)
                            {
                                InputAPI.PressKey(key.Code);
                                Thread.Sleep(sleep);
                                InputAPI.ReleaseKey(key.Code);
                            }
                        }
                    });
                    Wave.Start(WaveInterval);
                }
                catch (Exception e) { Log.Log(e.ToString()); }
            }
            if (GUILayout.Button("Stop"))
            {
                try
                {
                    Wave.Abort();
                    InputAPI.Active = false;
                }
                catch (Exception e) { Log.Log(e.ToString()); }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void DrawMigrateMenu()
        {
            if (bold == null || Bigbold == null)
            {
                bold = new GUIStyle
                {
                    fontSize = 30,
                    fontStyle = FontStyle.Bold
                };
                bold.normal.textColor = Color.white;
                Bigbold = new GUIStyle
                {
                    fontSize = 40,
                    fontStyle = FontStyle.Bold
                };
                Bigbold.normal.textColor = Color.red;
            }
            GUILayout.Label(Lang.GetString("MIGRATION_WARNING"), Bigbold);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            GUILayout.Label(Lang.GetString("MIGRATE_FROM_V2_KEYVIEWER"), bold);

            GUILayout.BeginHorizontal();
            GUILayout.Label("KeyCounts.kc:");
            v2Arg.keyCountsPath = GUILayout.TextField(v2Arg.keyCountsPath);
            if (GUILayout.Button("Choose"))
                v2Arg.keyCountsPath = StandaloneFileBrowser.OpenFilePanel("Select KeyCounts", "", new ExtensionFilter[] { new ExtensionFilter("KeyCounts", "kc") }, false)[0];
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("KeySettings.ks:");
            v2Arg.keySettingsPath = GUILayout.TextField(v2Arg.keySettingsPath);
            if (GUILayout.Button("Choose"))
                v2Arg.keySettingsPath = StandaloneFileBrowser.OpenFilePanel("Select KeySettings", "", new ExtensionFilter[] { new ExtensionFilter("KeySettings", "ks") }, false)[0];
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Settings.xml (Must Not Be Null!):");
            v2Arg.settingsPath = GUILayout.TextField(v2Arg.settingsPath);
            if (GUILayout.Button("Choose"))
                v2Arg.settingsPath = StandaloneFileBrowser.OpenFilePanel("Select Settings", "", new ExtensionFilter[] { new ExtensionFilter("Settings", "xml") }, false)[0];
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.GetString("MIGRATE")))
            {
                try
                {
                    IMigrator migrator = Migrator.V2(v2Arg);
                    Settings = migrator.Migrate();
                    KeyManager.Profile.calculator.Stop();
                    Lang.ChangeLanguage(Settings.Language);
                    KeyManager.Profile = Settings.CurrentProfile;
                    MigrateErrorString = string.Empty;
                }
                catch (FileNotFoundException fe)
                {
                    MigrateErrorString = $"File Not Found: '{fe.FileName}'";
                }
                catch (InvalidOperationException ie)
                {
                    MigrateErrorString = ie.Message;
                }
            }
            GUILayout.Label(MigrateErrorString);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.GetString("GO_BACK")))
                IsMigrating = false;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void DrawGroupSettingsGUI()
        {
            if (KeyManager.Profile.EditingKeyGroups = GUILayout.Toggle(KeyManager.Profile.EditingKeyGroups, Lang.GetString("KEY_GROUPS")))
            {
                MoreGUILayout.BeginIndent();
                GUILayout.BeginHorizontal();
                var groups = KeyManager.Profile.KeyGroups;
                if (GUILayout.Button(Lang.GetString("NEW")))
                    groups.Add(new Group($"Group {groups.Count}"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    if (group.Editing = GUILayout.Toggle(group.Editing, group.Name))
                        group.RenderGUI();
                }
                MoreGUILayout.EndIndent();
            }
        }
        private static void DrawProfileSettingsGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.GetString("MIGRATE_MENU")))
                IsMigrating = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.GetString("NEW")))
            {
                Settings.Profiles.Add(new Profile());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += "Profile " + Settings.Profiles.Count;
                KeyManager.Profile = Settings.CurrentProfile;
            }
            if (GUILayout.Button(Lang.GetString("DUPLICATE")))
            {
                Settings.Profiles.Add(Settings.CurrentProfile.Copy());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += " Copy";
                KeyManager.Profile = Settings.CurrentProfile;
            }
            if (Settings.Profiles.Count > 1 && GUILayout.Button(Lang.GetString("DELETE")))
            {
                Settings.Profiles.RemoveAt(Settings.ProfileIndex);
                Settings.ProfileIndex = Math.Min(Settings.ProfileIndex, Settings.Profiles.Count - 1);
                KeyManager.Profile = Settings.CurrentProfile;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            Settings.CurrentProfile.Name = MoreGUILayout.NamedTextField(Lang.GetString("PROFILE_NAME"), Settings.CurrentProfile.Name, 400f);
            GUILayout.Label(Lang.GetString("PROFILES"));
            int selected = Settings.ProfileIndex;
            if (MoreGUILayout.ToggleList(Settings.Profiles, ref selected, p => p.Name))
            {
                KeyManager.Profile.calculator.Stop();
                Settings.ProfileIndex = selected;
                KeyManager.Profile = Settings.CurrentProfile;
                KeyManager.Profile.calculator.Start();
            }
        }
        private static void DrawKeyRegisterSettingsGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Lang.GetString("REGISTERED_KEYS"));
            KeyManager.Profile.IgnoreSkippedKeys = GUILayout.Toggle(KeyManager.Profile.IgnoreSkippedKeys, Lang.GetString("IGNORE_SKIPPED_KEYS"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            var configs = KeyManager.Profile.ActiveKeys;
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config.SpecialType != SpecialKeyType.None)
                {
                    if (GUILayout.Button(config.SpecialType.ToString()))
                    {
                        KeyManager.Profile.ActiveKeys.RemoveAt(i);
                        KeyManager.UpdateKeys();
                    }
                }
                else
                {
                    if (GUILayout.Button(config.Code.ToString()))
                    {
                        KeyManager.Profile.ActiveKeys.RemoveAt(i);
                        KeyManager.UpdateKeys();
                    }
                }
                GUILayout.Space(8f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            // Record keys toggle
            GUILayout.BeginHorizontal();
            if (IsListening)
            {
                if (GUILayout.Button(Lang.GetString("DONE")))
                    IsListening = false;
                GUILayout.Label(Lang.GetString("PRESS_KEY_REGISTER"));
            }
            else
            {
                if (GUILayout.Button(Lang.GetString("CHANGE_KEYS")))
                    IsListening = true;
            }
            if (GUILayout.Button(Lang.GetString("CLEAR_KEY_COUNT")))
                KeyManager.ClearCounts();
            if (!KeyManager.specialKeys.Keys.Contains(SpecialKeyType.KPS) && GUILayout.Button("Register KPS Key"))
            {
                KeyManager.Profile.ActiveKeys.Add(new Key.Config(SpecialKeyType.KPS));
                KeyManager.UpdateKeys();
            }
            if (!KeyManager.specialKeys.Keys.Contains(SpecialKeyType.Total) && GUILayout.Button("Register Total Key"))
            {
                KeyManager.Profile.ActiveKeys.Add(new Key.Config(SpecialKeyType.Total));
                KeyManager.UpdateKeys();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Profile pf = KeyManager.Profile;
            pf.MakeBarSpecialKeys = GUILayout.Toggle(pf.MakeBarSpecialKeys, Lang.GetString("MAKE_BAR_SPECIAL_KEYS"));
        }
        private static void DrawKeyViewerSettingsGUI()
        {
            MoreGUILayout.BeginIndent();
            KeyManager.Profile.ViewerOnlyGameplay = GUILayout.Toggle(KeyManager.Profile.ViewerOnlyGameplay, Lang.GetString("VIEWER_ONLY_GAMEPLAY"));
            KeyManager.Profile.AnimateKeys = GUILayout.Toggle(KeyManager.Profile.AnimateKeys, Lang.GetString("ANIMATE_KEYS"));
            bool newShowTotal = GUILayout.Toggle(KeyManager.Profile.ShowKeyPressTotal, Lang.GetString("SHOW_KEY_PRESS_TOTAL"));
            if (newShowTotal != KeyManager.Profile.ShowKeyPressTotal)
            {
                KeyManager.Profile.ShowKeyPressTotal = newShowTotal;
                KeyManager.UpdateLayout();
            }
            float newSize = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_SIZE"), KeyManager.Profile.KeyViewerSize, 10f, 200f, 300f, roundNearest: 1f);
            if (newSize != KeyManager.Profile.KeyViewerSize)
            {
                KeyManager.Profile.KeyViewerSize = newSize;
                KeyManager.UpdateLayout();
            }
            float newX = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_X_POS"), KeyManager.Profile.KeyViewerXPos, -1f, 1f, 300f, roundNearest: 0.01f, valueFormat: "{0:0.##}");
            if (newX != KeyManager.Profile.KeyViewerXPos)
            {
                KeyManager.Profile.KeyViewerXPos = newX;
                KeyManager.UpdateLayout();
            }
            float newY = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_Y_POS"), KeyManager.Profile.KeyViewerYPos, -1f, 1f, 300f, roundNearest: 0.01f, valueFormat: "{0:0.##}");
            if (newY != KeyManager.Profile.KeyViewerYPos)
            {
                KeyManager.Profile.KeyViewerYPos = newY;
                KeyManager.UpdateLayout();
            }
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Lang.GetString("KPS_UPDATE_RATE"));
            int.TryParse(GUILayout.TextField(KeyManager.Profile.KPSUpdateRateMs.ToString()), out KeyManager.Profile.KPSUpdateRateMs);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (Settings.EditSettingEachKeys = GUILayout.Toggle(Settings.EditSettingEachKeys, Lang.GetString("EDIT_SETTINGS_EACH_KEYS")))
                foreach (Key key in KeyManager.keys.Values)
                    key.RenderGUI();
            else Key.DrawGlobalConfig(KeyManager.Profile.GlobalConfig, ApplyEachKeys);
            foreach (Key key in KeyManager.specialKeys.Values)
                key.RenderGUI();
            MoreGUILayout.EndIndent();
        }
        public static void ApplyEachKeys(Key.Config keyConfig)
        {
            foreach (Key key in KeyManager.keys.Values)
                key.config.ApplyConfig(keyConfig);
            KeyManager.UpdateLayout();
        }
        public static bool Equals(this VertexGradient left, VertexGradient right)
            => left.topLeft == right.topLeft && left.topRight == right.topRight && left.bottomLeft == right.bottomLeft && left.bottomRight == right.bottomRight;
        public static bool Inequals(this VertexGradient left, VertexGradient right)
            => left.topLeft != right.topLeft || left.topRight != right.topRight || left.bottomLeft != right.bottomLeft || left.bottomRight != right.bottomRight;
        public static void CheckNull(this object obj, string identifier)
            => Log.Log($"{identifier} Is {(obj == null ? "Null" : "Not Null")}");
        public static T LogObject<T>(this T obj)
        {
            Log.Log(obj.ToString());
            return obj;
        }
        public static readonly ISet<KeyCode> SKIPPED_KEYS = new HashSet<KeyCode>()
        {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
            KeyCode.Escape,
        };
        public static uint Sum(this IEnumerable<uint> enumerable)
        {
            uint result = 0;
            foreach (uint u in enumerable)
                result += u;
            return result;
        }
    }
}
