using KeyViewer.Core.Input;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models;

public class Settings : IModel, ICopyable<Settings> {
    public string Lang = "Default";
    public bool UseLegacyTheme = false;
    public bool UseTooltip = true;
    public bool UseWindowsAsyncInput = true;
    public PollingRate PollingRate = PollingRate.Hz1000;
    public List<ActiveProfile> ActiveProfiles = [];

    public Settings Copy() {
        var newSettings = new Settings {
            Lang = Lang,
            ActiveProfiles = [.. ActiveProfiles.Select(p => p.Copy())],
            UseLegacyTheme = UseLegacyTheme,
            UseTooltip = UseTooltip,
            UseWindowsAsyncInput = UseWindowsAsyncInput,
            PollingRate = PollingRate
        };
        return newSettings;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Lang)] = Lang,
            [nameof(ActiveProfiles)] = ModelUtils.WrapCollection(ActiveProfiles),
            [nameof(UseLegacyTheme)] = UseLegacyTheme,
            [nameof(UseTooltip)] = UseTooltip,
            [nameof(UseWindowsAsyncInput)] = UseWindowsAsyncInput,
            [nameof(PollingRate)] = ((int)PollingRate).ToString(),
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new Settings();
        Lang = node[nameof(Lang)]?.Value<string>() ?? defaultSettings.Lang;
        ActiveProfiles = node[nameof(ActiveProfiles)] is JArray profilesArray ? ModelUtils.UnwrapList<ActiveProfile>(profilesArray) : [];
        UseLegacyTheme = node[nameof(UseLegacyTheme)]?.Value<bool>() ?? defaultSettings.UseLegacyTheme;
        UseTooltip = node[nameof(UseTooltip)]?.Value<bool>() ?? defaultSettings.UseTooltip;
        UseWindowsAsyncInput = node[nameof(UseWindowsAsyncInput)]?.Value<bool>() ?? defaultSettings.UseWindowsAsyncInput;
        PollingRate = node[nameof(PollingRate)]?.Value<int>() is int pollingRateInt ? (PollingRate)pollingRateInt : defaultSettings.PollingRate;
    }
}
