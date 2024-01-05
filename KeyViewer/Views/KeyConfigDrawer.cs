using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using UnityEngine;
using TKKC = KeyViewer.Core.Translation.TranslationKeys.KeyConfig;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;

namespace KeyViewer.Views
{
    public class KeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyManager manager;
        public KeyConfigDrawer(KeyManager manager, KeyConfig config) : base(config, L(TKKC.KeyConfiguration, config.DummyName != null ? config.DummyName : config.Code))
        {
            this.manager = manager;
        }
        public override void Draw()
        {
            if (model.DummyName != null)
            {
                if (Drawer.DrawString(L(TKKC.DummyKeyName), ref model.DummyName))
                    Name = model.DummyName;
            }
            else GUILayout.Label(L(TKKC.KeyCode) + ":" + model.Code);

            bool changed = false;
            changed |= Drawer.DrawString(L(TKKC.TextFont), ref model.Font);
            if (model.DummyName == null)
                changed |= Drawer.DrawBool(L(TKKC.EnableKPSMeter), ref model.EnableKPSMeter);
            changed |= Drawer.DrawBool(L(TKKC.EnableCountText), ref model.EnableCountText);

            changed |= Drawer.DrawPressReleaseH(L(TKKC.Text), model.Text, Drawer.CD_H_STR);
            if (model.EnableCountText)
                changed |= Drawer.DrawPressReleaseH(L(TKKC.CountText), model.CountText, Drawer.CD_H_STR);
            changed |= Drawer.DrawPressReleaseH(L(TKKC.BackgroundImage), model.Background, Drawer.CD_H_STR);
            changed |= Drawer.DrawPressReleaseH(L(TKKC.OutlineImage), model.Outline, Drawer.CD_H_STR);

            Drawer.DrawObjectConfig(L(TKKC.EditTextConfig), L(TKKC.KeyText, model.DummyName != null ? model.DummyName : model.Code), model.TextConfig, () => manager.UpdateLayout());
            if (model.EnableCountText)
                Drawer.DrawObjectConfig(L(TKKC.EditCountTextConfig), L(TKKC.KeyCountText, model.DummyName != null ? model.DummyName : model.Code), model.CountTextConfig, () => manager.UpdateLayout());
            Drawer.DrawObjectConfig(L(TKKC.EditBackgroundConfig), L(TKKC.KeyBackground, model.DummyName != null ? model.DummyName : model.Code), model.BackgroundConfig, () => manager.UpdateLayout());
            Drawer.DrawObjectConfig(L(TKKC.EditOutlineConfig), L(TKKC.KeyOutline, model.DummyName != null ? model.DummyName : model.Code), model.OutlineConfig, () => manager.UpdateLayout());

            changed |= Drawer.DrawSingleWithSlider(L(TKKC.BackgroundImageRoundness), ref model.BackgroundRoundness, 0, 1, 300);
            changed |= Drawer.DrawSingleWithSlider(L(TKKC.OutlineImageRoundness), ref model.OutlineRoundness, 0, 1, 300);

            changed |= Drawer.DrawVectorConfig(model.VectorConfig);

            changed |= Drawer.DrawBool(L(TKKC.EnableRain), ref model.RainEnabled);
            if (model.RainEnabled)
                Drawer.TitleButton(L(TKKC.EditRainConfig), L(TKM.EditThis), () => GUIController.Push(new RainConfigDrawer(manager, model)));

            if (changed)
                manager.UpdateLayout();
        }
    }
}
