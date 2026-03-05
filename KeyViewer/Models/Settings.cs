using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models;

public class Settings : IModel, ICopyable<Settings> {
    public string Lang = "Default";
    public bool useLegacyTheme = false;
    public List<ActiveProfile> ActiveProfiles = [];
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Lang)] = Lang,
            [nameof(ActiveProfiles)] = ModelUtils.WrapCollection(ActiveProfiles)
        };

        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new Settings();

        Lang = node[nameof(Lang)]?.Value<string>() ?? defaultSettings.Lang;
        ActiveProfiles = node[nameof(ActiveProfiles)] is JArray profilesArray ? ModelUtils.UnwrapList<ActiveProfile>(profilesArray) : [];
    }
    public Settings Copy() {
        var newSettings = new Settings {
            Lang = Lang,
            ActiveProfiles = ActiveProfiles.Select(p => p.Copy()).ToList()
        };
        return newSettings;
    }
}
