using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Core.Translation;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable
    {
        public Profile profile;
        public List<KeyConfigDrawer> configs;
        public ProfileDrawer(KeyManager manager, Profile profile) : base(manager)
        {
            this.profile = profile;
            configs = profile.Keys.Select(k => new KeyConfigDrawer(manager, k)).ToList();
        }
        public override void Draw(IDrawer drawer)
        {
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.MakeBarSpecialKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.ViewOnlyGamePlay);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.AnimateKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.ShowKeyPressTotal);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.LimitNotRegisteredKeys);
            drawer.DrawBool(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.ResetOnStart);
            drawer.DrawInt32(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.KPSUpdateRate);
            drawer.DrawSingle(Main.Lang[TranslationKeys.Lorem_Ipsum], ref profile.Size);

        }
    }
}
