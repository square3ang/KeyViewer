using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Core.Translation;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable<Profile>
    {
        public KeyManager manager;
        public List<KeyConfigDrawer> configs;
        public ProfileDrawer(KeyManager manager, Profile profile) : base(profile)
        {
            this.manager = manager;
            configs = profile.Keys.Select(k => new KeyConfigDrawer(manager, k)).ToList();
        }
        public override void Draw(IDrawer drawer)
        {
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.MakeBarSpecialKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.ViewOnlyGamePlay);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.AnimateKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.ShowKeyPressTotal);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.LimitNotRegisteredKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.ResetOnStart);
            drawer.DrawInt32(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.KPSUpdateRate);
            drawer.DrawSingle(Main.Lang[TranslationKeys.Lorem_Ipsum], ref model.Size);
        }
    }
}
