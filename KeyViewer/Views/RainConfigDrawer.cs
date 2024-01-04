using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using UnityEngine;
using TKRC = KeyViewer.Core.Translation.TranslationKeys.RainConfig;

namespace KeyViewer.Views
{
    public class RainConfigDrawer : ModelDrawable<RainConfig>
    {
        public KeyManager manager;
        public KeyConfig config;
        private bool imageListExpanded = false;
        public RainConfigDrawer(KeyManager manager, KeyConfig config) : base(config.Rain, L(TKRC.KeyConfiguration, config.DummyName != null ? config.DummyName : config.Code))
        {
            this.manager = manager;
            this.config = config;
        }
        public override void Draw()
        {
            string name = config.DummyName ?? config.Code.ToString();
            Drawer.DrawPressReleaseH(L(TKRC.RainSpeed), model.Speed, Drawer.CD_H_FLT_SPEEDONLY);
            Drawer.DrawPressReleaseH(L(TKRC.RainLength), model.Length, Drawer.CD_H_FLT_LENGTHONLY);
            Drawer.DrawPressReleaseH(L(TKRC.RainSoftness), model.Softness, Drawer.CD_H_INT32_SOFTNESSONLY);
            Drawer.DrawPressReleaseH(L(TKRC.RainPoolSize), model.PoolSize, Drawer.CD_H_INT32_POOLSIZEONLY);

            GUILayoutEx.ExpandableGUI(() =>
            {
                Drawer.DrawList(model.RainImages, (ref RainImage i) =>
                {
                    bool result = false;
                    result |= Drawer.DrawString(L(TKRC.RainImagePath), ref i.Image);
                    result |= Drawer.DrawInt32(L(TKRC.RainImageCount), ref i.Count);
                    return result;
                });
            }, L(TKRC.RainImages), ref imageListExpanded);

            Drawer.DrawObjectConfig(L(TKRC.EditRainConfig), L(TKRC.KeyRain, name), model.ObjectConfig);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(L(TKRC.Direction, name));
                Drawer.DrawEnum(L(TKRC.Direction, name), ref model.Direction);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(L(TKRC.ImageDisplayMode, name));
                Drawer.DrawEnum(L(TKRC.ImageDisplayMode, name), ref model.ImageDisplayMode);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
