using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Migration.V3;
// V3 To V4
public class V3Migrator {
    public static Models.Settings Migrate(V3Settings settings, out List<JObject> profiles) {
        var v4Settings = new Models.Settings();
        v4Settings.ActiveProfiles.AddRange(settings.Profiles.Select(p => new Models.ActiveProfile(p.Name, true)));
        profiles = [];
        foreach(var profile in settings.Profiles) {
            profiles.Add((JObject)MigrateProfile(profile).Serialize());
        }

        return v4Settings;
    }
    public static Models.Profile MigrateProfile(V3Profile profile) {
        var v4Profile = new Models.Profile {
            ViewOnlyGamePlay = profile.ViewerOnlyGameplay,
            LimitNotRegisteredKeys = profile.LimitNotRegisteredKeys,
            KPSUpdateRate = profile.KPSUpdateRateMs,
            ResetOnStart = profile.ResetWhenStart
        };
        var scale = profile.KeyViewerSize / 100f;
        v4Profile.VectorConfig.Offset.Set(new Vector3(profile.KeyViewerXPos / 2 * Screen.width, profile.KeyViewerYPos / 2 * Screen.height));
        v4Profile.VectorConfig.Scale.Set(new Vector3(scale, scale));
        List<Key_Config> specialV3Keys = [];
        List<Models.KeyConfig> specialKeys = [];
        float x = 0, dummyX = 0;
        foreach(var key in profile.ActiveKeys) {
            if(key.SpecialType != SpecialKeyType.None) {
                specialV3Keys.Add(key);
            } else {
                v4Profile.Keys.Add(MigrateKey(key, profile.ShowKeyPressTotal, profile.AnimateKeys, ref x));
            }
        }
        foreach(var key in specialV3Keys) {
            Models.KeyConfig v4Key = profile.MakeBarSpecialKeys
                ? MigrateKey(key, profile.ShowKeyPressTotal, profile.AnimateKeys, ref dummyX)
                : MigrateKey(key, profile.ShowKeyPressTotal, profile.AnimateKeys, ref x);

            v4Key.UpdateTextAlways = true;
            specialKeys.Add(v4Key);
            v4Profile.Keys.Add(v4Key);
        }
        if(profile.MakeBarSpecialKeys) {
            MakeBar(specialKeys, specialV3Keys, profile.AnimateKeys, x);
        }

        return v4Profile;
    }
    private static Models.KeyConfig MigrateKey(Key_Config keyConfig, bool showCountText, bool animateKeys, ref float x) {
        bool isSpecial = keyConfig.SpecialType != SpecialKeyType.None;
        var v4Config = new Models.KeyConfig {
            Code = keyConfig.Code,
            Font = keyConfig.Font,
            EnableCountText = showCountText
        };
        if(keyConfig.SpecialType != SpecialKeyType.None) {
            v4Config.DummyName = keyConfig.SpecialType.ToString();
        }

        v4Config.DoNotScaleText = true;
        v4Config.DisableSorting = true;
        v4Config.Count = (int)keyConfig.Count;
        if(isSpecial) {
            v4Config.CountText = keyConfig.SpecialType == SpecialKeyType.KPS ? (Models.PressReleaseBase<string>)"{CurKPS}" : (Models.PressReleaseBase<string>)"{Count}";
        }
        v4Config.Text = keyConfig.KeyTitle?.Replace("\\", "\\\\");
        if(keyConfig.RainEnabled) {
            v4Config.RainEnabled = true;
            v4Config.Rain = MigrateRain(keyConfig.RainConfig);
        }
        var keyHeight = showCountText ? 150 : 100;
        double rawHeight = keyConfig.Height - 100;
        double scaleUnit = 1d / keyHeight;
        var scale = new Vector2(keyConfig.Width / 100f, 1 + (float)(rawHeight * scaleUnit));
        var ease = new Models.EaseConfig(keyConfig.Ease, keyConfig.EaseDuration);
        var height = keyHeight * scale.y;
        var heightOffset = (keyHeight - height) / 4f;
        var offsetVector = new Vector2(keyConfig.OffsetX, keyConfig.OffsetY);

        v4Config.VectorConfig.Offset = new Vector2((keyConfig.Width / 2f) + x, height / 2f) + offsetVector;

        v4Config.TextFontSize = keyConfig.TextFontSize;
        v4Config.CountTextFontSize = keyConfig.CountTextFontSize;

        v4Config.BackgroundConfig.VectorConfig.Scale.SetEase(ease.Copy());
        v4Config.OutlineConfig.VectorConfig.Scale.SetEase(ease.Copy());

        v4Config.BackgroundConfig.VectorConfig.Scale.Set(scale * (animateKeys ? keyConfig.ShrinkFactor : 1), scale);
        v4Config.OutlineConfig.VectorConfig.Scale.Set(scale * (animateKeys ? keyConfig.ShrinkFactor : 1), scale);

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
        x += keyConfig.Width + 10;
        return v4Config;
    }
    private static Models.RainConfig MigrateRain(KeyRain_Config rainConfig) {
        var v4Config = new Models.RainConfig();
        v4Config.ObjectConfig.VectorConfig.Offset = new Vector2(rainConfig.OffsetX, rainConfig.OffsetY) / 5f;
        v4Config.Speed = rainConfig.RainSpeed;
        v4Config.PoolSize = rainConfig.RainPoolSize;
        v4Config.Softness = rainConfig.Softness;
        v4Config.ObjectConfig.Color.Set(rainConfig.RainColor);
        v4Config.Direction = (Models.Direction)rainConfig.Direction;
        v4Config.Length = rainConfig.RainLength;
        Vector2 newScale = new(rainConfig.RainWidth < 0 ? 1 : rainConfig.RainWidth / 100f,
                                        rainConfig.RainHeight < 0 ? 1 : rainConfig.RainHeight / 100f);
        v4Config.ObjectConfig.VectorConfig.Scale = newScale;
        v4Config.ImageDisplayMode = rainConfig.SequentialImages ? Models.RainImageDisplayMode.Sequential : Models.RainImageDisplayMode.Random;
        for(int i = 0; i < rainConfig.RainImages.Length; i++) {
            string img = rainConfig.RainImages[i];
            Models.RainImage ri = new() {
                Image = img,
                Count = rainConfig.RainImageCounts[i]
            };
            v4Config.RainImages.Add(ri);
        }
        return v4Config;
    }
    private static void MakeBar(List<Models.KeyConfig> keys, List<Key_Config> v3Keys, bool animateKeys, float lastX) {
        float tempX = 0;
        int updateCount = 0;
        for(int i = 0; i < keys.Count; i++) {
            var config = v3Keys[i];
            var v4Config = keys[i];
            int spacing = updateCount * 10;
            Vector2 size = new(0, 75 * (config.Height / 100)) {
                x = ((lastX - 10) / keys.Count * (config.Width / 100)) - (spacing / 2)
            };
            var scale = new Vector2(size.x / 100f, size.y / 150f);
            float heightOffset = config.Height / 5f;
            Vector2 position = new Vector2((size.x / 2) + tempX + (spacing / 2), -((config.Height / 2) - 10)) + new Vector2(config.OffsetX, config.OffsetY);
            v4Config.EnableCountText = true;
            v4Config.VectorConfig.Offset.Set(position);
            v4Config.BackgroundConfig.VectorConfig.Scale.Set(scale * (animateKeys ? config.ShrinkFactor : 1), scale);
            v4Config.OutlineConfig.VectorConfig.Scale.Set(scale * (animateKeys ? config.ShrinkFactor : 1), scale);
            v4Config.TextFontSize -= 15;
            v4Config.CountTextFontSize -= 6;
            v4Config.TextConfig.VectorConfig.Offset.Set(new Vector3(config.TextOffsetX, config.TextOffsetY - heightOffset));
            v4Config.CountTextConfig.VectorConfig.Offset.Set(new Vector3(config.CountTextOffsetX, config.CountTextOffsetY + heightOffset));
            tempX += size.x;
            updateCount++;
        }
    }
}
