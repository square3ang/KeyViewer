using KeyViewer.Core;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using RapidGUI;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace KeyViewer.Views
{
    public class SettingsDrawer : ModelDrawable<Settings>
    {
        public SettingsDrawer(Settings settings) : base(settings, Main.Lang.Get("SETTINGS", "Settings")) { }

        private bool isOpenedExtraMenu = false;
        private string[] languages;
        private string[] userLanguages;
        internal bool NeedLangInit = true;

        public static float preparinglastUpdateTime = 0f;
        public static string[] preparingsymbols = { "|", "/", "-", "\\" };
        public static int preparingsymbolIndex = 0;
        public static float helptime = 0f;

        private void LanguageInit() {
            helptime = 0f;
            preparingsymbolIndex = 0;
            languages = Main.Lang.GetLanguages();
            userLanguages = Main.Lang.GetLanguageNativeNames();
        }

        private void LanguageUpdate(int index) {
            Main.Lang.Language = languages[index];
            model.Lang = Main.Lang.Language;
        }

        public override void OnceCall() {
            LanguageInit();
            destroyConfirm = [];
        }

        private static HashSet<string> destroyConfirm;

        public override void Draw()
        {
            bool reaction = false;

            if(Main.Lang.IsLoading) {
                float elapsedTime = Time.time - preparinglastUpdateTime;

                if(elapsedTime >= 0.05f) {
                    preparingsymbolIndex++;
                    if(preparingsymbolIndex >= preparingsymbols.Length) {
                        preparingsymbolIndex = 0;
                    }
                    preparinglastUpdateTime = Time.time;
                }

                helptime += Time.deltaTime;
                if(helptime >= 8f) {
                    GUILayout.Label("Is the Preparing is taking too long?? please get in touch with the developer for assistance!!");
                } else {
                    GUILayout.Label("");
                }
                GUILayout.BeginHorizontal();
                Drawer.Button("Loading translations for you, hang tight...", GUILayout.Width(480));
                GUILayout.Space(10);
                GUILayout.Label(preparingsymbols[preparingsymbolIndex]);
                GUILayout.EndHorizontal();
            } else {
                string languageDesc;
                if(Main.Lang.IsDefault) {
                    languageDesc = $"! {Translator.FALLBACK_LANGUAGE} by KEYVIEWER";
                } else {
                    int translatorsCount = Main.Lang.GetArrCount("0TRANSLATORS");

                    if(translatorsCount > 0) {
                        var names = new List<string>();
                        for(int i = 0; i < translatorsCount; i++) {
                            names.Add(Main.Lang.GetArr("0TRANSLATORS", i, "[UNKNOWN]"));
                        }
                        string translatorsText = string.Join(" & ", names);
                        languageDesc = $"| {Main.Lang.Get("0NATIVELANG", Main.Lang.Language)} by {translatorsText}";
                    } else {
                        languageDesc = $"| {Main.Lang.Language}";
                    }
                }

                GUILayout.Label($"{Main.Lang.Get("SELECTLANGUAGE", "Select Language")} {languageDesc}");
                if(Main.Lang.IsSomeFail) {
                    GUILayout.Label("<color=#FFFF00>Some translations are failed to load, See the log for details.</color>");
                } else if(Main.Lang.IsFail) {
                    GUILayout.Label($"<color=#FF0000>All translations failed to load: {Main.Lang.FailState}</color>");
                }
                GUILayout.BeginHorizontal();
                int selectedIndex = Array.IndexOf(languages, Main.Lang.Language);

                if(Drawer.Button("◀", GUILayout.Width(40))) {
                    reaction = true;
                    selectedIndex = (selectedIndex - 1 + languages.Length) % languages.Length;
                    LanguageUpdate(selectedIndex);
                }

                if(Drawer.SelectionPopup(ref selectedIndex, userLanguages, "", GUILayout.Width(400))) {
                    reaction = true;
                    LanguageUpdate(selectedIndex);
                }
                if(Drawer.Button("▶", GUILayout.Width(40))) {
                    reaction = true;
                    selectedIndex = (selectedIndex + 1) % languages.Length;
                    LanguageUpdate(selectedIndex);
                }

                bool reloadLang = false;
                try {
                    if(Drawer.Button(Main.Lang.Get("RELOADLANG", "Reload Language Pack"), GUILayout.Width(320))) {
                        reaction = true;
                        reloadLang = true; 
                    }
                } catch {
                } finally {
                    GUILayout.EndHorizontal();
                }
                
                if(reloadLang) {
                    _ = Task.Run(async () => {
                        await Main.Lang.Load(Path.Combine(Main.Mod.Path, "lang"));
                        NeedLangInit = true;
                    });
                }
            }
            GUILayout.BeginHorizontal();
            if(Drawer.Button(Main.Lang.Get("EXTRA_MENU", "Extra Menu") + " " + (isOpenedExtraMenu ? "▼" : "▲"))) {
                reaction = true;
                isOpenedExtraMenu = !isOpenedExtraMenu;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if(isOpenedExtraMenu) {
                if(Drawer.DrawBool(string.Format(Main.Lang.Get("USE_THIS", "Use {0}"), Main.Lang.Get("LEGACY_THEME", "Legacy Theme")), ref model.useLegacyTheme)) {
                    reaction = true;
                    Drawer.SetStyle(model.useLegacyTheme);
                    RGUIStyle.CreateStyles();
                }
            }
            GUILayout.BeginHorizontal();
            if(Drawer.Button(Main.Lang.Get("SETTINGS_IMPORT_PROFILE", "Import Profile"))) {
                reaction = true;
                var profiles = StandaloneFileBrowser.OpenFilePanel(Main.Lang.Get("SETTINGS_SELECT_PROFILE", "Select Profile"), Main.ProfilePath, new[] { new ExtensionFilter("V4", "json"), new ExtensionFilter("V3", "xml"), }, true);
                foreach(var profile in profiles) {
                    FileInfo file = new FileInfo(profile);
                    if(file.Extension == ".json") {
                        if(!File.Exists(Path.Combine(Main.ProfilePath, file.Name)))
                            file.CopyTo(Path.Combine(Main.ProfilePath, file.Name));
                        var activeProfile = new ActiveProfile(Path.GetFileNameWithoutExtension(file.FullName), true);
                        model.ActiveProfiles.Add(activeProfile);
                        Main.AddManager(activeProfile, true);
                    } else if(file.Extension == ".xml")
                        Main.MigrateFromV3Xml(file.FullName);
                }
            }
            if(Drawer.Button(Main.Lang.Get("SETTINGS_CREATE_PROFILE", "Create New Profile"))) {
                reaction = true;
                var profile = new ActiveProfile(GetNewProfileName(), true);
                model.ActiveProfiles.Add(profile);
                Profile newProfile = new Profile();
                File.WriteAllText(Path.Combine(Main.ProfilePath, $"{profile.Name}.json"), newProfile.Serialize().ToString());
                Main.AddManager(profile, true);
            }
            if(Drawer.Button(Main.Lang.Get("SETTINGS_OPEN_MOD_DIR", "Open Mod Directory"))) {
                reaction = true;
                Application.OpenURL(Path.GetFullPath(Main.Mod.Path));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < model.ActiveProfiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                var profile = model.ActiveProfiles[i];
                bool profileActiveDiff = Drawer.DrawOnlyBool(ref profile.Active);
                if(profileActiveDiff) {
                    if(profile.Active && !Main.Managers.TryGetValue(profile.Name, out _))
                        Main.AddManager(profile, true);
                    if(!profile.Active && Main.Managers.TryGetValue(profile.Name, out var m))
                        Main.RemoveManager(profile);
                    model.ActiveProfiles[i] = profile;
                }
                GUI.color = profile.Active ? new Color(0.8f, 0.8f, 1f) : Color.gray;
                if(Drawer.Button(Main.Lang.Get("EDIT", "Edit")) && profile.Active) {
                    var manager = Main.Managers[profile.Name];
                    Main.GUI.Push(new ProfileDrawer(manager, manager.profile, profile.Name));
                }
                GUI.color = new Color(1f, 0.8f, 0.8f);
                bool isConfirm = destroyConfirm.Contains(profile.Name);

                if(isConfirm) {
                    if(Drawer.Button(Main.Lang.Get("ONE_MORE", "One More!"))) {
                        Main.RemoveManager(profile);
                        string path = Path.Combine(Main.ProfilePath, $"{profile.Name}.json");

                        if(File.Exists(path)) {
                            File.Delete(path);
                        }

                        Main.ToDeleteFiles.Add(path);
                        model.ActiveProfiles.RemoveAll(p => p.Name == profile.Name);

                        destroyConfirm.Remove(profile.Name);
                        break;
                    }
                } else {
                    if(Drawer.Button(Main.Lang.Get("DESTROY", "Destroy"))) {
                        destroyConfirm.Add(profile.Name);
                    }
                }
                GUI.color = new Color(1f, 0.8f, 1f);
                if(Drawer.Button(Main.Lang.Get("EXPORT", "Export"))) {
                    reaction = true;
                    string target = StandaloneFileBrowser.SaveFilePanel(Main.Lang.Get("SETTINGS_SELECT_PROFILE", "Select Profile"), Persistence.GetLastUsedFolder(), $"{profile.Name}.json", "json");
                    if(!string.IsNullOrWhiteSpace(target)) {
                        Profile p = Main.Managers[profile.Name].profile;
                        var node = p.Serialize();
                        node["References"] = ProfileImporter.GetReferencesAsJson(p);
                        File.WriteAllText(target, node.ToString());
                    }
                }
                GUI.color = Color.white;
                GUILayout.Label(profile.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if(reaction) {
                    destroyConfirm = [];
                }

                if(NeedLangInit) {
                    NeedLangInit = false;
                    languages = null;
                    userLanguages = null;
                    LanguageInit();
                }
            }
        }
        private static string GetNewProfileName() {
            int num = 0;
            while(File.Exists(Path.Combine(Main.ProfilePath, $"Profile {num}.json"))) {
                num++;
            }
            return $"Profile {num}";
        }
    }
}
