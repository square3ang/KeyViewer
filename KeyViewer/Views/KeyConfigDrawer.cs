using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using Overlayer.Core;
using System;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Views;

public class KeyConfigDrawer : ModelDrawable<KeyConfig> {
    public KeyManager manager;
    public KeyConfigDrawer(KeyManager manager, KeyConfig config) : base(config, string.Format(Main.Lang.Get("KEYCONFIG", "{0} Key Config"), config.DummyName == null ? config.Code : config.DummyName)) => this.manager = manager;

    public override void OnceCall() => NeoDrawer.StaticInstance.FieldResetDictById();

    public static bool IsOpenBoolSettings = false;

    private static readonly KeyCode[] KeyCodeValues = [.. Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().OrderBy(k => (int)k)];
    private static readonly string[] KeyCodeNames = [.. KeyCodeValues.Select(k => k.ToString())];

    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetId();

        if(model.DummyName != null) {
            if(Drawer.DrawString(Main.Lang.Get("DUMMY_KEY_NAME", "Dummy Key Name"), ref model.DummyName)) {
                Name = model.DummyName;
            }
        } else {
            GUILayout.BeginHorizontal();
            {
                int current = Array.IndexOf(KeyCodeValues, model.Code);
                bool result = Drawer.SelectionPopup(ref current, KeyCodeNames, $"{Main.Lang.Get("KEY_CODE", "Key Code")}");
                if(result) {
                    model.Code = KeyCodeValues[current]; 
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        bool prevBgBlurEnabled = model.BackgroundBlurEnabled;
        bool changed = false;
        {
            string tempFont = model.Font;
            bool tempChanged = Drawer.DrawString(Main.Lang.Get("TEXT_FONT", "Text Font"), ref tempFont);
            if(tempChanged) {
                model.Font = tempFont.TrimQuote();
                changed = true;
            }
        }
        GUILayout.BeginHorizontal();
        if(Drawer.Button(Main.Lang.Get("TOGGLE_SETTINGS", "Toggle Settings") + " " + (IsOpenBoolSettings ? "▼" : "▲"))) {
            IsOpenBoolSettings = !IsOpenBoolSettings;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(IsOpenBoolSettings) {
            if(model.DummyName == null) {
                if(Drawer.DrawBool(Main.Lang.Get("ENABLE_KPS_METER", "Enable KPS Meter"), ref model.EnableKPSMeter)) {
                    changed = true;
                    if(model.EnableKPSMeter) {
                        KPSCalculator.Sync(manager.keys.Select(k => k.Config.EnableKPSMeter ? k.KpsCalc : null).Where(c => c != null));
                    } else {
                        manager[model.Code.ToString()].KpsCalc.Stop();
                    }
                }
            }
            changed |= Drawer.DrawBool(Main.Lang.Get("ENABLE_COUNT_TEXT", "Enable Count Text"), ref model.EnableCountText);
            changed |= Drawer.DrawBool(Main.Lang.Get("ENABLE_OUTLINE_IMAGE", "Enable Outline Image"), ref model.EnableOutlineImage);
            changed |= Drawer.DrawBool(Main.Lang.Get("ENABLE_BACKGROUND_BLUR", "Enable Backgruond Blur"), ref model.BackgroundBlurEnabled);
            changed |= Drawer.DrawBool(Main.Lang.Get("ENABLE_RAIN", "Enable Rain"), ref model.RainEnabled);
            changed |= Drawer.DrawBool(Main.Lang.Get("UPDATE_TEXT_ALWAYS", "Update Text Always"), ref model.UpdateTextAlways);
            changed |= Drawer.DrawBool(Main.Lang.Get("DISABLE_SORTING", "Disable Sorting"), ref model.DisableSorting);
            changed |= Drawer.DrawBool(Main.Lang.Get("DO_NOT_SCALE_TEXT", "Do Not Scale Text"), ref model.DoNotScaleText);
        }
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("TEXT_FONT_SIZE", "Text Font Size"), ref model.TextFontSize, 0, 300, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("COUNT_TEXT_FONT_SIZE", "Count Text Font Size"), ref model.CountTextFontSize, 0, 300, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("BACKGROUND_IMAGE_ROUNDNESS", "Background Image Roundness"), ref model.BackgroundRoundness, 0, Constants.Rad2Deg100, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("OUTLINE_IMAGE_ROUNDNESS", "Outline Image Roundness"), ref model.OutlineRoundness, 0, Constants.Rad2Deg100, 300);

        GUILayout.BeginHorizontal();
        if(Drawer.Button($"<b>{Main.Lang.Get("TEXT", "Text")}</b>")) {
            Main.GUI.Push(new ObjectConfigDrawer(manager, model, model.TextConfig));
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        changed |= Drawer.DrawPressReleaseBase(model.Text);
        if(model.EnableCountText) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("COUNT_TEXT", "Count Text")}</b>")) {
                Main.GUI.Push(new ObjectConfigDrawer(manager, model, model.CountTextConfig));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            changed |= Drawer.DrawPressReleaseBase(model.CountText);
        }
        GUILayout.BeginHorizontal();
        if(Drawer.Button($"<b>{Main.Lang.Get("BACKGROUND_IMAGE", "Background Image")}</b>")) {
            Main.GUI.Push(new ObjectConfigDrawer(manager, model, model.BackgroundConfig));
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        changed |= Drawer.DrawPressReleaseBase(model.Background);
        if(model.EnableOutlineImage) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("OUTLINE_IMAGE", "Outline Image")}</b>")) {
                Main.GUI.Push(new ObjectConfigDrawer(manager, model, model.OutlineConfig));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            changed |= Drawer.DrawPressReleaseBase(model.Outline);
        }
        if(model.BackgroundBlurEnabled) {
            changed |= NeoDrawer.StaticInstance.DrawBlurConfig(model.BackgroundBlurConfig);
        }
        if(model.RainEnabled) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("RAIN", "Rain")}</b>")) {
                Main.GUI.Push(new RainConfigDrawer(manager, model));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.Label($"<b>{Main.Lang.Get("VECTOR", "Vector")}</b>");
        changed |= NeoDrawer.StaticInstance.DrawVectorConfig(model.VectorConfig);

        if(changed) {
            if(!prevBgBlurEnabled && model.BackgroundBlurEnabled) {
                KeyViewerUtils.ApplyBlurColorConfig(model);
            }
            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }
}
