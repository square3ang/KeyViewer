using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using System.IO;
using UnityEngine;

namespace KeyViewer.Views
{
    public class SettingsDrawer : ModelDrawable<Settings>
    {
        public SettingsDrawer(Settings settings) : base(settings) { }
        public override void Draw(IDrawer drawer)
        {
            GUILayout.BeginHorizontal();
            drawer.DrawEnum(L(TranslationKeys.Lorem_Ipsum), ref model.Language);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < model.ActiveProfiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                var profile = model.ActiveProfiles[i];
                if (drawer.DrawBool(profile.Name, ref profile.Active))
                {
                    if (profile.Active) 
                        Main.AddManager(profile);
                    else Main.RemoveManager(profile);
                    model.ActiveProfiles[i] = profile;
                }
                if (GUILayout.Button(L(TranslationKeys.Lorem_Ipsum)))
                {
                    var manager = Main.Managers[profile.Name];
                    GUIController.Push(L(TranslationKeys.Lorem_Ipsum), new ProfileDrawer(manager, manager.profile));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
