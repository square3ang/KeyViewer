using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using SFB;
using System.IO;
using UnityEngine;
using TKP = KeyViewer.Core.Translation.TranslationKeys.Profile;
using TKS = KeyViewer.Core.Translation.TranslationKeys.Settings;

namespace KeyViewer.Views
{
    public class SettingsDrawer : ModelDrawable<Settings>
    {
        public SettingsDrawer(Settings settings) : base(settings, L(TKS.Prefix)) { }
        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(L(TKS.SelectLanguage));
                if (Drawer.DrawEnum(L(TKS.Language), ref model.Language))
                {
                    GUIController.Skip(() =>
                    {
                        Main.Lang = Language.GetLanguage(model.Language);
                        Main.OnLanguageInitialize();
                    });
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(L(TKS.ImportProfile)))
                {
                    var profiles = StandaloneFileBrowser.OpenFilePanel(L(TKS.SelectProfile), Persistence.GetLastUsedFolder(), "json", true);
                    foreach (var profile in profiles)
                    {
                        FileInfo file = new FileInfo(profile);
                        if (file.Directory.FullName.ToLower() != Main.Mod.Path.ToLower())
                            file.CopyTo(Path.Combine(Main.Mod.Path, file.Name));
                        var activeProfile = new ActiveProfile(Path.GetFileNameWithoutExtension(file.FullName), true);
                        model.ActiveProfiles.Add(activeProfile);
                        Main.AddManager(activeProfile, true);
                    }
                }
                if (GUILayout.Button(L(TKS.CreateProfile)))
                {
                    var profile = new ActiveProfile(GetNewProfileName(), true);
                    model.ActiveProfiles.Add(profile);
                    Profile newProfile = new Profile();
                    File.WriteAllText(Path.Combine(Main.Mod.Path, $"{profile.Name}.json"), newProfile.Serialize().ToString(4));
                    Main.AddManager(profile, true);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < model.ActiveProfiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    var profile = model.ActiveProfiles[i];
                    GUILayout.Label(profile.Name);
                    var newActive = GUILayout.Toggle(profile.Active, string.Empty);
                    if (profile.Active != newActive)
                    {
                        profile.Active = newActive;
                        if (Main.Managers.TryGetValue(profile.Name, out var newManager))
                            newManager.gameObject.SetActive(newActive);
                        model.ActiveProfiles[i] = profile;
                    }
                    if (profile.Active)
                    {
                        if (GUILayout.Button(L(TKP.ConfigurateProfile, profile.Name)))
                        {
                            var manager = Main.Managers[profile.Name];
                            GUIController.Push(new ProfileDrawer(manager, manager.profile, profile.Name));
                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private static int newProfileNum = 1;
        private static string GetNewProfileName()
        {
            string result = "Profile " + newProfileNum + ".json";
            while (File.Exists(Path.Combine(Main.Mod.Path, result)))
                result = "Profile " + ++newProfileNum + ".json";
            return $"Profile {newProfileNum++}";
        }
    }
}
