using KeyViewer.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public struct ActiveProfile : IModel, ICopyable<ActiveProfile> {
    public ActiveProfile(string name, bool active) {
        Name = name;
        Active = active;
    }
    public ActiveProfile(string name, bool active, string key) {
        Name = name;
        Active = active;
    }
    public string Name = "None";
    public bool Active = false;
    public ActiveProfile Copy() {
        var profile = new ActiveProfile {
            Name = Name,
            Active = Active
        };
        return profile;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Name)] = Name,
            [nameof(Active)] = Active
        };
        return node;
    }

    public void Deserialize(JToken node) {
        if(node == null) {
            return;
        }
        var defaultSettings = new ActiveProfile();
        Name = node[nameof(Name)]?.Value<string>() ?? defaultSettings.Name;
        Active = node[nameof(Active)]?.Value<bool>() ?? defaultSettings.Active;
    }
}
