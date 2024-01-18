﻿using JSON;
using KeyViewer.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public static Models.Profile MigrateProfile(Profile profile)
        {
            var v4Profile = new Models.Profile();
            v4Profile.ViewOnlyGamePlay = profile.ViewerOnlyGameplay;
            v4Profile.LimitNotRegisteredKeys = profile.LimitNotRegisteredKeys;
            var scale = profile.KeyViewerSize / 100f;
            v4Profile.VectorConfig.Scale = new Vector3(scale, scale);
            v4Profile.KPSUpdateRate = profile.KPSUpdateRateMs;
            v4Profile.ResetOnStart = profile.ResetWhenStart;
            foreach (var key in profile.ActiveKeys)
            {
                var k = MigrateKey(key);
                v4Profile.Keys.Add(k);
            }
            AdjustProfile(v4Profile);
            return v4Profile;
        }
        private static Models.KeyConfig MigrateKey(Key_Config keyConfig)
        {
            var v4Config = new Models.KeyConfig();
            v4Config.Code = keyConfig.Code;
            v4Config.Font = keyConfig.Font;
            if (keyConfig.SpecialType != SpecialKeyType.None)
                v4Config.DummyName = keyConfig.SpecialType.ToString();
            v4Config.DoNotScaleText = true;
            v4Config.Count = (int)keyConfig.Count;
            v4Config.Text = keyConfig.KeyTitle;
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
            v4Config.VectorConfig.Scale = new Vector3(keyConfig.Width / 100f, keyConfig.Height / 100f);
            v4Config.VectorConfig.Offset = new Vector3(keyConfig.OffsetX, keyConfig.OffsetY);
            v4Config.TextConfig.VectorConfig.Offset = new Vector3(keyConfig.TextOffsetX, keyConfig.TextOffsetY);
            v4Config.CountTextConfig.VectorConfig.Offset = new Vector3(keyConfig.CountTextOffsetX, keyConfig.CountTextOffsetY);

            var ease = new Models.EaseConfig(keyConfig.Ease, keyConfig.EaseDuration);
            v4Config.TextConfig.VectorConfig.Scale.SetEase(ease.Copy());
            v4Config.CountTextConfig.VectorConfig.Scale.SetEase(ease.Copy());
            v4Config.BackgroundConfig.VectorConfig.Scale.SetEase(ease.Copy());
            v4Config.OutlineConfig.VectorConfig.Scale.SetEase(ease.Copy());

            v4Config.TextConfig.VectorConfig.Scale.Pressed = new Vector2(keyConfig.ShrinkFactor, keyConfig.ShrinkFactor);
            v4Config.CountTextConfig.VectorConfig.Scale.Pressed = new Vector2(keyConfig.ShrinkFactor, keyConfig.ShrinkFactor);
            v4Config.BackgroundConfig.VectorConfig.Scale.Pressed = new Vector2(keyConfig.ShrinkFactor, keyConfig.ShrinkFactor);
            v4Config.OutlineConfig.VectorConfig.Scale.Pressed = new Vector2(keyConfig.ShrinkFactor, keyConfig.ShrinkFactor);

            v4Config.TextConfig.VectorConfig.Scale.Released = Vector2.one;
            v4Config.CountTextConfig.VectorConfig.Scale.Released = Vector2.one;
            v4Config.BackgroundConfig.VectorConfig.Scale.Released = Vector2.one;
            v4Config.OutlineConfig.VectorConfig.Scale.Released = Vector2.one;

            v4Config.TextFontSize = keyConfig.TextFontSize;
            v4Config.CountTextFontSize = keyConfig.CountTextFontSize;

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
            Vector3 newScale = new Vector3(rainConfig.RainWidth < 0 ? 1 : rainConfig.RainWidth,
                                            rainConfig.RainHeight < 0 ? 1 : rainConfig.RainHeight);
            v4Config.ObjectConfig.VectorConfig.Scale = newScale;
            v4Config.ImageDisplayMode = rainConfig.SequentialImages ? Models.RainImageDisplayMode.Sequential : Models.RainImageDisplayMode.Random;
            for (int i = 0; i < rainConfig.RainImages.Length; i++)
            {
                string img = rainConfig.RainImages[i];
                Models.RainImage ri = default;
                ri.Image = img;
                ri.Count = rainConfig.RainImageCounts[i];
                v4Config.RainImages.Add(ri);
            }
            return v4Config;
        }
        private static void AdjustProfile(Models.Profile profile)
        {
            profile.VectorConfig.Offset.Set(profile.VectorConfig.Offset.Released.WithRelativeX(-.5f).WithRelativeY(-.5f));
        }
    }
}