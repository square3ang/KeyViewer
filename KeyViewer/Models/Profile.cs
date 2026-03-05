using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models;

public class Profile : IModel, ICopyable<Profile> {
    public List<KeyConfig> Keys = [];
    public bool ViewOnlyGamePlay = false;
    public bool LimitNotRegisteredKeys = false;
    public bool ResetOnStart = false;
    public float KeySpacing = 10f;
    public VectorConfig VectorConfig = new();
    public int KPSUpdateRate = 1000;
    public Profile Copy() {
        Profile newProfile = new() {
            Keys = Keys.Select(k => k.Copy()).ToList(),
            ViewOnlyGamePlay = ViewOnlyGamePlay,
            LimitNotRegisteredKeys = LimitNotRegisteredKeys,
            ResetOnStart = ResetOnStart,
            KeySpacing = KeySpacing,
            VectorConfig = VectorConfig.Copy(),
            KPSUpdateRate = KPSUpdateRate
        };
        return newProfile;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Keys)] = ModelUtils.WrapCollection(Keys),
            [nameof(ViewOnlyGamePlay)] = ViewOnlyGamePlay,
            [nameof(LimitNotRegisteredKeys)] = LimitNotRegisteredKeys,
            [nameof(ResetOnStart)] = ResetOnStart,
            [nameof(KeySpacing)] = KeySpacing,
            [nameof(VectorConfig)] = VectorConfig.Serialize(),
            [nameof(KPSUpdateRate)] = KPSUpdateRate
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new Profile();

        Keys = ModelUtils.UnwrapList<KeyConfig>(node[nameof(Keys)]);
        ViewOnlyGamePlay = node[nameof(ViewOnlyGamePlay)]?.Value<bool>() ?? defaultSettings.ViewOnlyGamePlay;
        LimitNotRegisteredKeys = node[nameof(LimitNotRegisteredKeys)]?.Value<bool>() ?? defaultSettings.LimitNotRegisteredKeys;
        ResetOnStart = node[nameof(ResetOnStart)]?.Value<bool>() ?? defaultSettings.ResetOnStart;
        KeySpacing = node[nameof(KeySpacing)]?.Value<float>() ?? defaultSettings.KeySpacing;
        VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);
        KPSUpdateRate = node[nameof(KPSUpdateRate)]?.Value<int>() ?? defaultSettings.KPSUpdateRate;
    }
}
