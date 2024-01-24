using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using System.Linq;
using UnityEngine;
using TKKC = KeyViewer.Core.Translation.TranslationKeys.KeyConfig;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;
using System.Collections.Generic;

namespace KeyViewer.Views
{
    public class MultipleKeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyManager manager;
        public KeyConfig original;
        public List<string> targets;
        public MultipleKeyConfigDrawer(KeyManager manager, KeyConfig original, List<string> targets) : base(original, L(TKKC.KeyConfiguration, AggregateTargets(targets)))
        {
            this.manager = manager;
            this.original = original.Copy();
            this.targets = targets;
        }
        public override void Draw()
        {
            Drawer.ButtonLabel(Name, KeyViewerUtils.OpenDiscordUrl);

            bool changed = false;
            bool prevBgBlurEnabled = model.BackgroundBlurEnabled;
            changed |= Drawer.DrawString(LD(TKKC.TextFont, "Font"), ref model.Font).IfTrue(() => Set("Font"));
            if (model.DummyName == null)
            {
                if (Drawer.DrawBool(LD(TKKC.EnableKPSMeter, "EnableKPSMeter"), ref model.EnableKPSMeter))
                {
                    changed = true;
                    if (model.EnableKPSMeter)
                        KPSCalculator.Sync(manager.keys.Select(k => k.Config.EnableKPSMeter ? k.KpsCalc : null).Where(c => c != null));
                    else manager[model.Code.ToString()].KpsCalc.Stop();
                }
            }
            changed |= Drawer.DrawBool(LD(TKKC.UpdateTextAlways, "UpdateTextAlways"), ref model.UpdateTextAlways).IfTrue(() => Set("UpdateTextAlways"));
            changed |= Drawer.DrawBool(LD(TKKC.EnableCountText, "EnableCountText"), ref model.EnableCountText).IfTrue(() => Set("EnableCountText"));
            changed |= Drawer.DrawBool(LD(TKKC.EnableOutlineImage, "EnableOutlineImage"), ref model.EnableOutlineImage).IfTrue(() => Set("EnableOutlineImage"));
            changed |= Drawer.DrawBool(LD(TKKC.DisableSorting, "DisableSorting"), ref model.DisableSorting).IfTrue(() => Set("DisableSorting"));
            changed |= Drawer.DrawBool(LD(TKKC.DoNotScaleText, "DoNotScaleText"), ref model.DoNotScaleText).IfTrue(() => Set("DoNotScaleText"));
            changed |= Drawer.DrawBool(LD(TKKC.EnableBackgroundBlur, "BackgroundBlurEnabled"), ref model.BackgroundBlurEnabled).IfTrue(() => Set("BackgroundBlurEnabled"));
            changed |= Drawer.DrawSingleWithSlider(LD(TKKC.TextFontSize, "TextFontSize"), ref model.TextFontSize, 0, 300, 300).IfTrue(() => Set("TextFontSize"));
            changed |= Drawer.DrawSingleWithSlider(LD(TKKC.CountTextFontSize, "CountTextFontSize"), ref model.CountTextFontSize, 0, 300, 300).IfTrue(() => Set("CountTextFontSize"));

            changed |= Drawer.DrawPressReleaseH(L(TKKC.Text), model.Text, Drawer.CD_H_STR);
            if (model.EnableCountText)
                changed |= Drawer.DrawPressReleaseH(L(TKKC.CountText), model.CountText, Drawer.CD_H_STR);
            changed |= Drawer.DrawPressReleaseH(L(TKKC.BackgroundImage), model.Background, Drawer.CD_H_STR);
            if (model.EnableOutlineImage)
                changed |= Drawer.DrawPressReleaseH(L(TKKC.OutlineImage), model.Outline, Drawer.CD_H_STR);
            if (model.BackgroundBlurEnabled)
                changed |= Drawer.DrawBlurConfig(L(TKKC.KeyBackground, KeyViewerUtils.KeyName(model)), model.BackgroundBlurConfig);

            changed |= Drawer.DrawVectorConfig(model.VectorConfig);

            Drawer.DrawObjectConfig(L(TKKC.EditTextConfig), L(TKKC.KeyText, model.DummyName != null ? model.DummyName : model.Code), model.TextConfig, () => manager.UpdateLayout());
            if (model.EnableCountText)
                Drawer.DrawObjectConfig(L(TKKC.EditCountTextConfig), L(TKKC.KeyCountText, model.DummyName != null ? model.DummyName : model.Code), model.CountTextConfig, () => manager.UpdateLayout());
            Drawer.DrawObjectConfig(L(TKKC.EditBackgroundConfig), L(TKKC.KeyBackground, model.DummyName != null ? model.DummyName : model.Code), model.BackgroundConfig, () => manager.UpdateLayout());
            if (model.EnableOutlineImage)
                Drawer.DrawObjectConfig(L(TKKC.EditOutlineConfig), L(TKKC.KeyOutline, model.DummyName != null ? model.DummyName : model.Code), model.OutlineConfig, () => manager.UpdateLayout());

            changed |= Drawer.DrawSingleWithSlider(LD(TKKC.BackgroundImageRoundness, "BackgroundRoundness"), ref model.BackgroundRoundness, 0, Constants.Rad2Deg100, 300);
            changed |= Drawer.DrawSingleWithSlider(LD(TKKC.OutlineImageRoundness, "OutlineRoundness"), ref model.OutlineRoundness, 0, Constants.Rad2Deg100, 300);

            changed |= Drawer.DrawBool(LD(TKKC.EnableRain, "RainEnabled"), ref model.RainEnabled).IfTrue(() => Set("RainEnabled"));
            if (model.RainEnabled)
                Drawer.TitleButton(L(TKKC.EditRainConfig), L(TKM.EditThis), () => Main.GUI.Push(new RainConfigDrawer(manager, model)));

            if (changed)
            {
                if (!prevBgBlurEnabled && model.BackgroundBlurEnabled)
                    KeyViewerUtils.ApplyBlurColorConfig(model);
                manager.UpdateLayout();
            }
        }
        bool IsDifferent(string fieldName)
        {
            var field = typeof(KeyConfig).GetField(fieldName);
            return !IsEquals(manager.keys.Where(k => targets.Contains(KeyViewerUtils.KeyName(k.Config)))
                .Select(k => field.GetValue(k.Config)));
        }
        void Set(string fieldName)
        {
            var field = typeof(KeyConfig).GetField(fieldName);
            var modelValue = field.GetValue(model);
            foreach (var key in manager.keys.Where(k => targets.Contains(KeyViewerUtils.KeyName(k.Config))))
                field.SetValue(key.Config, modelValue);
        }
        string LD(string tk, string fieldName, params object[] args)
        {
            string l = L(tk, args);
            if (IsDifferent(fieldName))
                l += " <color=cyan>(Diff!)</color>";
            return l;
        }
        static bool IsEquals(IEnumerable<object> objects)
        {
            object first = objects.First();
            return objects.All(o => o.Equals(first));
        }
        static string AggregateTargets(List<string> keyNames)
        {
            var result = keyNames.Aggregate("", (c, n) => $"{c}{n},");
            return result.Remove(result.Length - 1, 1);
        }
    }
}
