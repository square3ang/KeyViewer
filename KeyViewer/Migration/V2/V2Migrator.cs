using KeyViewer.Migration.V3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

namespace KeyViewer.Migration.V2;
// V2 To V3
public class V2Migrator {
    public Dictionary<KeyCode, int> KeyCounts;
    public Dictionary<KeyCode, KeySetting> KeySettings;
    public KeyViewerSettings Settings;
    public V2Migrator(string keyCountsPath, string keySettingsPath, string settingsPath) {
        KeyCounts = !string.IsNullOrWhiteSpace(keyCountsPath)
            ? JsonConvert.DeserializeObject<Dictionary<KeyCode, int>>(File.ReadAllText(keyCountsPath))
            : [];

        KeySettings = !string.IsNullOrWhiteSpace(keySettingsPath)
            ? JsonConvert.DeserializeObject<Dictionary<KeyCode, KeySetting>>(File.ReadAllText(keySettingsPath))
            : [];

        if(!string.IsNullOrWhiteSpace(settingsPath)) {
            XmlSerializer serializer = new(typeof(KeyViewerSettings));
            Settings = (KeyViewerSettings)serializer.Deserialize(File.Open(settingsPath, FileMode.Open));
        } else {
            throw new InvalidOperationException("Settings Path Cannot Be Null!");
        }
    }
    public V3Settings Migrate() {
        List<V3Profile> profiles = [];
        foreach(KeyViewerProfile pf in Settings.Profiles) {
            V3Profile newProfile = new() {
                Name = pf.Name,
                MakeBarSpecialKeys = false,
                ViewerOnlyGameplay = pf.ViewerOnlyGameplay,
                KeyViewerSize = pf.KeyViewerSize,
                KeyViewerXPos = pf.KeyViewerXPos,
                KeyViewerYPos = pf.KeyViewerYPos,
                AnimateKeys = pf.AnimateKeys,
                ShowKeyPressTotal = pf.ShowKeyPressTotal,
                IgnoreSkippedKeys = Settings.IgnoreSkippedKeys,
                KPSUpdateRateMs = Settings.UpdateRate,
                ActiveKeys = pf.ActiveKeys.Select(code => {
                    return code switch {
                        KeyCode.None => new Key_Config() { SpecialType = SpecialKeyType.KPS },
                        KeyCode.Joystick1Button0 => new Key_Config() { SpecialType = SpecialKeyType.Total },
                        _ => new Key_Config() { Code = code },
                    };
                }).ToList()
            };
            MigrateProfile(pf, newProfile.ActiveKeys);
            profiles.Add(newProfile);
        }
        V3Settings settings = new() {
            Profiles = profiles,
            ProfileIndex = Settings.ProfileIndex
        };
        return settings;
    }
    void MigrateProfile(KeyViewerProfile pf, List<Key_Config> keyConfs) {
        foreach(var conf in keyConfs) {
            if(KeyCounts.TryGetValue(conf.Code, out int count)) {
                conf.Count = (uint)count;
            }

            if(KeySettings.TryGetValue(conf.SpecialType switch {
                SpecialKeyType.KPS => KeyCode.None,
                SpecialKeyType.Total => KeyCode.Joystick1Button0,
                _ => conf.Code
            }, out KeySetting keySetting)) {
                PoSize posSize = keySetting.ps;

                Point offset = posSize.Pos;
                conf.OffsetX = offset.x;
                conf.OffsetY = offset.y;

                Point size = posSize.Size;
                conf.Width = size.x;
                conf.Height = size.y;

                Point tOffset = keySetting.TextPos;
                conf.TextOffsetX = tOffset.x;
                conf.TextOffsetY = tOffset.y;

                Point ctOffset = keySetting.CTextPos;
                conf.CountTextOffsetX = ctOffset.x;
                conf.CountTextOffsetY = ctOffset.y;

                conf.TextFontSize = keySetting.TextF;
                conf.CountTextFontSize = keySetting.CTextF;
            }
            conf.TooEarlyColor = Settings.TE;
            conf.VeryEarlyColor = Settings.VE;
            conf.EarlyPerfectColor = Settings.EP;
            conf.PerfectColor = Settings.P;
            conf.LatePerfectColor = Settings.LP;
            conf.VeryLateColor = Settings.VL;
            conf.TooLateColor = Settings.TL;

            conf.PressedBackgroundColor = pf.PressedBackgroundColor;
            conf.ReleasedBackgroundColor = pf.ReleasedBackgroundColor;

            conf.PressedOutlineColor = pf.PressedOutlineColor;
            conf.ReleasedOutlineColor = pf.ReleasedOutlineColor;

            conf.PressedTextColor = new VertexGradient(pf.PressedTextColor);
            conf.ReleasedTextColor = new VertexGradient(pf.ReleasedTextColor);

            conf.Ease = Settings.ease;
            conf.EaseDuration = Settings.ed;
            conf.ShrinkFactor = Settings.sf;
        }
    }
}
