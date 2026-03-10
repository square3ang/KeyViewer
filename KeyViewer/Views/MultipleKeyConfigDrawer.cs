using KeyViewer.Core;
using KeyViewer.Core.Input;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Views;

public class MultipleKeyConfigDrawer : ModelDrawable<KeyConfig> {
    public KeyManager manager;
    public KeyConfig modelCopy;
    public List<KeyConfig> targets;
    public List<KeyConfig> targetsCopy;
    public List<string> relativeKeyNames;

    public MultipleKeyConfigDrawer(KeyManager manager, List<string> targets, KeyConfig criterion)
        : base(criterion ?? new KeyConfig(),
              string.Format(Main.Lang.Get("KEY_CONFIGURATION", "Key {0} Configuration"),
              KeyViewerUtils.AggregateComma(targets))) {
        this.manager = manager;
        modelCopy = model.Copy();
        this.targets = manager.keys
            .Where(k => targets.Contains(KeyViewerUtils.KeyName(k.Config)))
            .Select(k => k.Config)
            .ToList();

        targetsCopy = this.targets.Select(k => k.Copy()).ToList();
        relativeKeyNames = this.targets
            .Where(k => k.DisableSorting)
            .Select(KeyViewerUtils.KeyName)
            .ToList();
    }

    public override void Draw() {
        GUILayout.Label(Name);

        if(relativeKeyNames.Any()) {
            GUILayout.Label($"<color=#45FFCA>{Main.Lang.Get("RELATIVE_COORDINATE_MODE", "Relative Coordinate Mode Due To Disable Sorting")}: {KeyViewerUtils.AggregateComma(relativeKeyNames)}</color>");
        }

        bool changed = false;
        bool prevBgBlurEnabled = model.BackgroundBlurEnabled;

        string tempFont = model.Font;
        if(Drawer.DrawString(FormatText(Main.Lang.Get("TEXT_FONT", "Text Font"), "Font"), ref tempFont)) {
            model.Font = tempFont.TrimQuote();
            Set("Font");
            changed = true;
        }
        GUILayout.BeginHorizontal();
        if(Drawer.Button(Main.Lang.Get("TOGGLE_SETTINGS", "Toggle Settings") + " " + (KeyConfigDrawer.IsOpenBoolSettings ? "▼" : "▲"))) {
            KeyConfigDrawer.IsOpenBoolSettings = !KeyConfigDrawer.IsOpenBoolSettings;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(KeyConfigDrawer.IsOpenBoolSettings) {
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
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("UPDATE_TEXT_ALWAYS", "Update Text Always"), "UpdateTextAlways"), ref model.UpdateTextAlways).IfTrue(() => Set("UpdateTextAlways"));
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("ENABLE_COUNT_TEXT", "Enable Count Text"), "EnableCountText"), ref model.EnableCountText).IfTrue(() => Set("EnableCountText"));
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("ENABLE_OUTLINE_IMAGE", "Enable Outline Image"), "EnableOutlineImage"), ref model.EnableOutlineImage).IfTrue(() => Set("EnableOutlineImage"));
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("DO_NOT_SCALE_TEXT", "Do Not Scale Text"), "DoNotScaleText"), ref model.DoNotScaleText).IfTrue(() => Set("DoNotScaleText"));
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("ENABLE_BACKGROUND_BLUR", "Enable Backgruond Blur"), "BackgroundBlurEnabled"), ref model.BackgroundBlurEnabled).IfTrue(() => Set("BackgroundBlurEnabled"));
            changed |= Drawer.DrawBool(FormatText(Main.Lang.Get("ENABLE_RAIN", "Enable Rain"), "RainEnabled"), ref model.RainEnabled).IfTrue(() => Set("RainEnabled"));
        }
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("TEXT_FONT_SIZE", "Text Font Size"), ref model.TextFontSize, 0, 300, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("COUNT_TEXT_FONT_SIZE", "Count Text Font Size"), ref model.CountTextFontSize, 0, 300, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("BACKGROUND_IMAGE_ROUNDNESS", "Background Image Roundness"), ref model.BackgroundRoundness, 0, Constants.Rad2Deg100, 300);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("OUTLINE_IMAGE_ROUNDNESS", "Outline Image Roundness"), ref model.OutlineRoundness, 0, Constants.Rad2Deg100, 300);

        GUILayout.BeginHorizontal();
        if(Drawer.Button($"<b>{Main.Lang.Get("TEXT", "Text")}</b>")) {
            Main.GUI.Push(new MultipleObjectConfigDrawer(manager, targets.Select(KeyViewerUtils.KeyName).ToList(), model.TextConfig, k => k.TextConfig));
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        changed |= Drawer.DrawPressReleaseBase(model.Text);
        if(model.EnableCountText) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("COUNT_TEXT", "Count Text")}</b>")) {
                Main.GUI.Push(new MultipleObjectConfigDrawer(manager, targets.Select(KeyViewerUtils.KeyName).ToList(), model.CountTextConfig, k => k.CountTextConfig));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            changed |= Drawer.DrawPressReleaseBase(model.CountText);
        }
        GUILayout.BeginHorizontal();
        if(Drawer.Button($"<b>{Main.Lang.Get("BACKGROUND_IMAGE", "Background Image")}</b>")) {
            Main.GUI.Push(new MultipleObjectConfigDrawer(manager, targets.Select(KeyViewerUtils.KeyName).ToList(), model.BackgroundConfig, k => k.BackgroundConfig));
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        changed |= Drawer.DrawPressReleaseBase(model.Background);
        if(model.EnableOutlineImage) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("OUTLINE_IMAGE", "Outline Image")}</b>")) {
                Main.GUI.Push(new MultipleObjectConfigDrawer(manager, targets.Select(KeyViewerUtils.KeyName).ToList(), model.OutlineConfig, k => k.OutlineConfig));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            changed |= Drawer.DrawPressReleaseBase(model.Outline);
        }
        if(model.BackgroundBlurEnabled) {
            changed |= NeoDrawer.StaticInstance.DrawBlurConfig(model.BackgroundBlurConfig).IfTrue(() => SetBlurConfig("BackgroundBlurConfig"));
        }
        if(model.RainEnabled) {
            GUILayout.BeginHorizontal();
            if(Drawer.Button($"<b>{Main.Lang.Get("RAIN", "Rain")}</b>")) {
                Main.GUI.Push(new MultipleRainConfigDrawer(manager, targets.Select(KeyViewerUtils.KeyName).ToList(), model.Rain));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        changed |= NeoDrawer.StaticInstance.DrawVectorConfig(model.VectorConfig).IfTrue(() => SetVectorConfig("VectorConfig"));

        if(changed) {
            if(!prevBgBlurEnabled && model.BackgroundBlurEnabled) {
                KeyViewerUtils.ApplyBlurColorConfig(model);
            }

            manager.UpdateLayout();
        }
    }

    void Set(string fieldName) {
        KeyViewerUtils.SetMultiple(model, modelCopy, targets, targetsCopy,
            fieldName,
            (o, t) => KeyInput.Shift || (((KeyConfig)o).DisableSorting && KeyViewerUtils.IsVectorType(t)));
    }

    void SetPR<T>(string fieldName) {
        var field = typeof(KeyConfig).GetField(fieldName);
        var original = field.GetValue(model) as PressRelease<T>;
        var originalCopy = field.GetValue(modelCopy) as PressRelease<T>;
        var targets = this.targets.Select(k => field.GetValue(k) as PressRelease<T>);
        var targetsCopy = this.targetsCopy.Select(k => field.GetValue(k) as PressRelease<T>);

        KeyViewerUtils.SetMultiplePR(original, originalCopy, targets, targetsCopy,
            (i, o, t) => KeyInput.Shift);
    }

    void SetVectorConfig(string fieldName) {
        var field = typeof(KeyConfig).GetField(fieldName);
        var vOriginal = field.GetValue(model) as VectorConfig;
        var vOriginalCopy = field.GetValue(modelCopy) as VectorConfig;
        var vTargets = targets.Select(k => field.GetValue(k) as VectorConfig);
        var vTargetsCopy = targetsCopy.Select(k => field.GetValue(k) as VectorConfig);

        static bool IsRelative(object instance, object o, System.Type t) =>
            KeyInput.Shift || (((KeyConfig)o).DisableSorting && KeyViewerUtils.IsVectorType(t));

        KeyViewerUtils.SetMultiplePR(vOriginal.Offset, vOriginalCopy.Offset,
            vTargets.Select(t => t.Offset), vTargetsCopy.Select(t => t.Offset),
            IsRelative, targets.Cast<object>().ToList());
    }

    void SetBlurConfig(string fieldName) {
        var field = typeof(KeyConfig).GetField(fieldName);
        var original = field.GetValue(model) as BlurConfig;
        var originalCopy = field.GetValue(modelCopy) as BlurConfig;
        var targets = this.targets.Select(k => field.GetValue(k) as BlurConfig);
        var targetsCopy = this.targetsCopy.Select(k => field.GetValue(k) as BlurConfig);

        KeyViewerUtils.SetMultiple(original, originalCopy, targets, targetsCopy, "Spacing", (i, t) => KeyInput.Shift);
        KeyViewerUtils.SetMultiple(original, originalCopy, targets, targetsCopy, "Vibrancy", (i, t) => KeyInput.Shift);
    }

    string FormatText(string text, string fieldName, params object[] args) {
        if(!KeyViewerUtils.IsEquals(targets, fieldName)) {
            text += " <color=cyan>(Diff!)</color>";
        }

        return text;
    }
}
