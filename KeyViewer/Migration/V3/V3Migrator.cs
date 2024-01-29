using JSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KeyViewer.Utils;

namespace KeyViewer.Migration.V3
{
    // V3 To V4
    public class V3Migrator
    {
        public static Models.Settings Migrate(V3Settings settings, out List<JsonNode> profiles)
        {
            var v4Settings = new Models.Settings();
            v4Settings.ActiveProfiles.AddRange(settings.Profiles.Select(p => new Models.ActiveProfile(p.Name, true)));
            profiles = new List<JsonNode>();
            foreach (var profile in settings.Profiles)
                profiles.Add(MigrateProfile(profile).Serialize());
            return v4Settings;
        }
        public static Models.Profile MigrateProfile(V3Profile profile)
        {
            var v4Profile = new Models.Profile();
            v4Profile.ViewOnlyGamePlay = profile.ViewerOnlyGameplay;
            v4Profile.LimitNotRegisteredKeys = profile.LimitNotRegisteredKeys;
            v4Profile.KPSUpdateRate = profile.KPSUpdateRateMs;
            v4Profile.ResetOnStart = profile.ResetWhenStart;
            var scale = profile.KeyViewerSize / 100f;
            v4Profile.VectorConfig.Offset.Set(new Vector3(profile.KeyViewerXPos / 2 * Screen.width, profile.KeyViewerYPos / 2 * Screen.height));
            v4Profile.VectorConfig.Scale.Set(new Vector3(scale, scale));
            List<Models.KeyConfig> specialBars = new List<Models.KeyConfig>();
            float x = 0;
            foreach (var key in profile.ActiveKeys)
            {
                var k = MigrateKey(key, profile.ShowKeyPressTotal, ref x);
                if (k.DummyName != null)
                {
                    specialBars.Add(k);
                    k.UpdateTextAlways = true;
                    k.DisableSorting = profile.MakeBarSpecialKeys;
                }
                else v4Profile.Keys.Add(k);
            }
            v4Profile.Keys.AddRange(specialBars);
            if (profile.MakeBarSpecialKeys)
                KeyViewerUtils.MakeBar(v4Profile, specialBars);
            return v4Profile;
        }
        private static Models.KeyConfig MigrateKey(Key_Config keyConfig, bool showCountText, ref float x)
        {
            var v4Config = new Models.KeyConfig();
            v4Config.Code = keyConfig.Code;
            v4Config.Font = keyConfig.Font;
            v4Config.EnableCountText = showCountText;
            if (keyConfig.SpecialType != SpecialKeyType.None)
                v4Config.DummyName = keyConfig.SpecialType.ToString();
            v4Config.DoNotScaleText = true;
            v4Config.Count = (int)keyConfig.Count;
            if (keyConfig.SpecialType != SpecialKeyType.None)
            {
                if (keyConfig.SpecialType == SpecialKeyType.KPS)
                    v4Config.CountText = "{CurKPS}";
                else v4Config.CountText = "{Count}";
            }
            v4Config.Text = keyConfig.KeyTitle?.Replace("\\", "\\\\");
            if (keyConfig.RainEnabled)
            {
                v4Config.RainEnabled = true;
                v4Config.Rain = MigrateRain(keyConfig.RainConfig);
            }
            if (keyConfig.ChangeBgColorJudge)
            {
                var bgConfig = v4Config.BackgroundConfig;
                bgConfig.ChangeColorWithJudge = true;
                var jc = bgConfig.JudgeColors = new Models.JudgeM<Models.GColor>();
                jc.TooEarly = keyConfig.TooEarlyColor;
                jc.VeryEarly = keyConfig.VeryEarlyColor;
                jc.EarlyPerfect = keyConfig.EarlyPerfectColor;
                jc.Perfect = keyConfig.PerfectColor;
                jc.LatePerfect = keyConfig.LatePerfectColor;
                jc.VeryLate = keyConfig.VeryLateColor;
                jc.TooLate = keyConfig.TooLateColor;
                jc.Multipress = keyConfig.MultipressColor;
                jc.FailMiss = keyConfig.FailMissColor;
                jc.FailOverload = keyConfig.FailOverloadColor;
            }
            if (v4Config.RainEnabled && keyConfig.ChangeRainColorJudge)
            {
                var rainConfig = v4Config.Rain;
                rainConfig.ObjectConfig.ChangeColorWithJudge = true;
                var jc = rainConfig.ObjectConfig.JudgeColors = new Models.JudgeM<Models.GColor>();
                jc.TooEarly = keyConfig.TooEarlyColor;
                jc.VeryEarly = keyConfig.VeryEarlyColor;
                jc.EarlyPerfect = keyConfig.EarlyPerfectColor;
                jc.Perfect = keyConfig.PerfectColor;
                jc.LatePerfect = keyConfig.LatePerfectColor;
                jc.VeryLate = keyConfig.VeryLateColor;
                jc.TooLate = keyConfig.TooLateColor;
                jc.Multipress = keyConfig.MultipressColor;
                jc.FailMiss = keyConfig.FailMissColor;
                jc.FailOverload = keyConfig.FailOverloadColor;
            }
            var keyHeight = showCountText ? 150 : 100;
            double rawHeight = keyConfig.Height - 100;
            double scaleUnit = 1d / keyHeight;
            var scale = new Vector2(keyConfig.Width / 100f, 1 + (float)(rawHeight * scaleUnit));
            var ease = new Models.EaseConfig(keyConfig.Ease, keyConfig.EaseDuration);
            var height = keyHeight * scale.y;
            var heightOffset = (keyHeight - height) / 4f;
            Main.Logger.Log($"Code:{keyConfig.Code}, width:{keyConfig.Width}");

            v4Config.VectorConfig.Offset = new Vector3(keyConfig.OffsetX + ((keyConfig.Width - 100) / 2f), keyConfig.OffsetY);

            v4Config.TextFontSize = keyConfig.TextFontSize;
            v4Config.CountTextFontSize = keyConfig.CountTextFontSize;

            v4Config.BackgroundConfig.VectorConfig.Scale.SetEase(ease.Copy());
            v4Config.OutlineConfig.VectorConfig.Scale.SetEase(ease.Copy());

            v4Config.BackgroundConfig.VectorConfig.Scale.Set(scale * keyConfig.ShrinkFactor, scale);
            v4Config.OutlineConfig.VectorConfig.Scale.Set(scale * keyConfig.ShrinkFactor, scale);

            v4Config.TextConfig.VectorConfig.Offset.Set(new Vector3(keyConfig.TextOffsetX, keyConfig.TextOffsetY - heightOffset));
            v4Config.CountTextConfig.VectorConfig.Offset.Set(new Vector3(keyConfig.CountTextOffsetX, keyConfig.CountTextOffsetY + heightOffset));

            v4Config.TextConfig.Color.Pressed = keyConfig.PressedTextColor;
            v4Config.TextConfig.Color.Released = keyConfig.ReleasedTextColor;

            v4Config.CountTextConfig.Color.Pressed = keyConfig.PressedCountTextColor;
            v4Config.CountTextConfig.Color.Released = keyConfig.ReleasedCountTextColor;

            v4Config.BackgroundConfig.Color.Pressed = keyConfig.PressedBackgroundColor;
            v4Config.BackgroundConfig.Color.Released = keyConfig.ReleasedBackgroundColor;

            v4Config.OutlineConfig.Color.Pressed = keyConfig.PressedOutlineColor;
            v4Config.OutlineConfig.Color.Released = keyConfig.ReleasedOutlineColor;
            return v4Config;
        }
        private static Models.RainConfig MigrateRain(KeyRain_Config rainConfig)
        {
            var v4Config = new Models.RainConfig();
            v4Config.ObjectConfig.VectorConfig.Offset = new Vector3(rainConfig.OffsetX, rainConfig.OffsetY);
            v4Config.Speed = rainConfig.RainSpeed;
            v4Config.PoolSize = rainConfig.RainPoolSize;
            v4Config.Softness = rainConfig.Softness;
            v4Config.ObjectConfig.Color.Set(rainConfig.RainColor);
            v4Config.Direction = (Models.Direction)rainConfig.Direction;
            v4Config.Length = rainConfig.RainLength;
            Vector2 newScale = new Vector2(rainConfig.RainWidth < 0 ? 1 : rainConfig.RainWidth / 100f,
                                            rainConfig.RainHeight < 0 ? 1 : rainConfig.RainHeight / 100f);
            v4Config.ObjectConfig.VectorConfig.Scale = newScale;
            v4Config.ImageDisplayMode = rainConfig.SequentialImages ? Models.RainImageDisplayMode.Sequential : Models.RainImageDisplayMode.Random;
            for (int i = 0; i < rainConfig.RainImages.Length; i++)
            {
                string img = rainConfig.RainImages[i];
                Models.RainImage ri = new Models.RainImage();
                ri.Image = img;
                ri.Count = rainConfig.RainImageCounts[i];
                v4Config.RainImages.Add(ri);
            }
            return v4Config;
        }
    }
}
