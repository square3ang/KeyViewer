using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TKP = KeyViewer.Core.Translation.TranslationKeys.Profile;

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
            drawer.DrawBool(L(TKP.MakeBarSpecialKeys), ref model.MakeBarSpecialKeys);
            drawer.DrawBool(L(TKP.ViewOnlyGamePlay), ref model.ViewOnlyGamePlay);
            drawer.DrawBool(L(TKP.AnimateKeys), ref model.AnimateKeys);
            drawer.DrawBool(L(TKP.ShowKeyPressTotal), ref model.ShowKeyPressTotal);
            drawer.DrawBool(L(TKP.LimitNotRegisteredKeys), ref model.LimitNotRegisteredKeys);
            drawer.DrawBool(L(TKP.ResetOnStart), ref model.ResetOnStart);
            drawer.DrawInt32(L(TKP.KPSUpdateRate), ref model.KPSUpdateRate);
            drawer.DrawSingle(L(TKP.Size), ref model.Size);

        }
    }
}
