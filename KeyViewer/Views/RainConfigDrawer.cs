﻿using KeyViewer.Core;
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
            bool changed = false;
            string name = config.DummyName ?? config.Code.ToString();
            changed |= Drawer.DrawInt32(L(TKRC.RainPoolSize), ref model.PoolSize);
            changed |= Drawer.DrawPressReleaseH(L(TKRC.RainSpeed), model.Speed, Drawer.CD_H_FLT_SPEEDONLY);
            changed |= Drawer.DrawPressReleaseH(L(TKRC.RainLength), model.Length, Drawer.CD_H_FLT_LENGTHONLY);
            changed |= Drawer.DrawPressReleaseH(L(TKRC.RainSoftness), model.Softness, Drawer.CD_H_INT32_SOFTNESSONLY);

            GUILayoutEx.ExpandableGUI(() =>
            {
                changed |= Drawer.DrawList(model.RainImages, (ref RainImage i) =>
                {
                    bool result = false;
                    result |= Drawer.DrawString(L(TKRC.RainImagePath), ref i.Image);
                    result |= Drawer.DrawInt32(L(TKRC.RainImageCount), ref i.Count);
                    return result;
                });
            }, L(TKRC.RainImages), ref imageListExpanded);

            Drawer.DrawObjectConfig(L(TKRC.EditRainConfig), L(TKRC.KeyRain, name), model.ObjectConfig, () => manager.UpdateLayout());

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(L(TKRC.Direction, name));
                changed |= Drawer.DrawEnum(L(TKRC.Direction, name), ref model.Direction);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(L(TKRC.ImageDisplayMode, name));
                changed |= Drawer.DrawEnum(L(TKRC.ImageDisplayMode, name), ref model.ImageDisplayMode);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (changed) manager.UpdateLayout();
        }
    }
}
