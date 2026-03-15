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
        return new Profile {
            Keys = Keys?.Select(k => k.Copy()).ToList() ?? [],
            ViewOnlyGamePlay = ViewOnlyGamePlay,
            LimitNotRegisteredKeys = LimitNotRegisteredKeys,
            ResetOnStart = ResetOnStart,
            KeySpacing = KeySpacing,
            VectorConfig = VectorConfig?.Copy(),
            KPSUpdateRate = KPSUpdateRate
        };
    }
    public JToken Serialize() {
        var node = new JObject();
        if(Keys.Count > 0) {
            node[nameof(Keys)] = ModelUtils.WrapCollection(Keys);
        }
        if(ViewOnlyGamePlay) {
            node[nameof(ViewOnlyGamePlay)] = true;
        }
        if(LimitNotRegisteredKeys) {
            node[nameof(LimitNotRegisteredKeys)] = true;
        }
        if(ResetOnStart) {
            node[nameof(ResetOnStart)] = true;
        }
        if(KeySpacing != 10f) {
            node[nameof(KeySpacing)] = KeySpacing;
        }
        node[nameof(VectorConfig)] = VectorConfig.Serialize();
        if(KPSUpdateRate != 1000) {
            node[nameof(KPSUpdateRate)] = KPSUpdateRate;
        }
        return node;
    }
    public void Deserialize(JToken node) {
        var defaults = new Profile();
        if(node == null) {
            return;
        }
        Keys = ModelUtils.UnwrapList<KeyConfig>(node[nameof(Keys)]) ?? [];
        ViewOnlyGamePlay = node[nameof(ViewOnlyGamePlay)]?.Value<bool>() ?? defaults.ViewOnlyGamePlay;
        LimitNotRegisteredKeys = node[nameof(LimitNotRegisteredKeys)]?.Value<bool>() ?? defaults.LimitNotRegisteredKeys;
        ResetOnStart = node[nameof(ResetOnStart)]?.Value<bool>() ?? defaults.ResetOnStart;
        KeySpacing = node[nameof(KeySpacing)]?.Value<float>() ?? defaults.KeySpacing;
        VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]) ?? new VectorConfig();
        KPSUpdateRate = node[nameof(KPSUpdateRate)]?.Value<int>() ?? defaults.KPSUpdateRate;
    }
}