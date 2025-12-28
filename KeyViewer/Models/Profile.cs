using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models
{
    public class Profile : IModel, ICopyable<Profile>
    {
        public List<KeyConfig> Keys = new List<KeyConfig>();
        public bool ViewOnlyGamePlay = false;
        public bool LimitNotRegisteredKeys = false;
        public bool ResetOnStart = false;
        public float KeySpacing = 10f;
        public VectorConfig VectorConfig = new VectorConfig();
        public int KPSUpdateRate = 1000;
        public Profile Copy()
        {
            Profile newProfile = new Profile();
            newProfile.Keys = Keys.Select(k => k.Copy()).ToList();
            newProfile.ViewOnlyGamePlay = ViewOnlyGamePlay;
            newProfile.LimitNotRegisteredKeys = LimitNotRegisteredKeys;
            newProfile.ResetOnStart = ResetOnStart;
            newProfile.KeySpacing = KeySpacing;
            newProfile.VectorConfig = VectorConfig.Copy();
            newProfile.KPSUpdateRate = KPSUpdateRate;
            return newProfile;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Keys)] = ModelUtils.WrapCollection(Keys);
            node[nameof(ViewOnlyGamePlay)] = ViewOnlyGamePlay;
            node[nameof(LimitNotRegisteredKeys)] = LimitNotRegisteredKeys;
            node[nameof(ResetOnStart)] = ResetOnStart;
            node[nameof(KeySpacing)] = KeySpacing;
            node[nameof(VectorConfig)] = VectorConfig.Serialize();
            node[nameof(KPSUpdateRate)] = KPSUpdateRate;
            return node;
        }
        public void Deserialize(JToken node)
        {
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
}
