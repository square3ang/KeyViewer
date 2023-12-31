using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using UnityEngine;
using TKS = KeyViewer.Core.Translation.TranslationKeys.Settings;

namespace KeyViewer.Views
{
    public class SettingsDrawer : ModelDrawable<Settings>
    {
        public SettingsDrawer(Settings settings) : base(settings) { }
        public override void Draw(IDrawer drawer)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(L(TKS.SelectLanguage));
            if (drawer.DrawEnum(L(TKS.Language), ref model.Language))
                Main.Lang = Language.GetLanguage(model.Language);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label(L(TKS.ActiveProfiles));

            for (int i = 0; i < model.ActiveProfiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                var profile = model.ActiveProfiles[i];
                GUILayout.Label(profile.Name);
                var newActive = GUILayout.Toggle(profile.Active, string.Empty);
                if (profile.Active != newActive)
                {
                    profile.Active = newActive;
                    if (profile.Active) 
                        Main.AddManager(profile);
                    else Main.RemoveManager(profile);
                    model.ActiveProfiles[i] = profile;
                }
                if (GUILayout.Button(L(TKS.ConfigurateProfile)))
                {
                    var manager = Main.Managers[profile.Name];
                    GUIController.Push(L(TKS.ConfigurateProfile), new ProfileDrawer(manager, manager.profile));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
