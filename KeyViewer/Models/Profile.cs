using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models
{
    public class Profile : IModel
    {
        public List<KeyConfig> Keys = new List<KeyConfig>();
        public bool ViewOnlyGamePlay = false;
        public bool AnimateKeys = true;
        public bool ShowKeyPressTotal = true;
        public bool LimitNotRegisteredKeys = false;
        public bool ResetOnStart = false;
        public VectorConfig VectorConfig = new VectorConfig();
        public int KPSUpdateRate = 1000;
        public Profile Copy()
        {
            Profile newProfile = new Profile();
            newProfile.Keys = Keys.Select(k => k.Copy()).ToList();
            newProfile.MakeBarSpecialKeys = MakeBarSpecialKeys;
            newProfile.ViewOnlyGamePlay = ViewOnlyGamePlay;
            newProfile.AnimateKeys = AnimateKeys;
            newProfile.ShowKeyPressTotal = ShowKeyPressTotal;
            newProfile.LimitNotRegisteredKeys = LimitNotRegisteredKeys;
            newProfile.ResetOnStart = ResetOnStart;
            newProfile.VectorConfig = VectorConfig.Copy();
            newProfile.KPSUpdateRate = KPSUpdateRate;
            return newProfile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Keys)] = ModelUtils.WrapList(Keys);
            node[nameof(MakeBarSpecialKeys)] = MakeBarSpecialKeys;
            node[nameof(ViewOnlyGamePlay)] = ViewOnlyGamePlay;
            node[nameof(AnimateKeys)] = AnimateKeys;
            node[nameof(ShowKeyPressTotal)] = ShowKeyPressTotal;
            node[nameof(LimitNotRegisteredKeys)] = LimitNotRegisteredKeys;
            node[nameof(ResetOnStart)] = ResetOnStart;
            node[nameof(VectorConfig)] = VectorConfig.Serialize();
            node[nameof(KPSUpdateRate)] = KPSUpdateRate;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Keys = ModelUtils.UnwrapList<KeyConfig>(node[nameof(Keys)].AsArray);
            MakeBarSpecialKeys = node[nameof(MakeBarSpecialKeys)];
            ViewOnlyGamePlay = node[nameof(ViewOnlyGamePlay)];
            AnimateKeys = node[nameof(AnimateKeys)];
            ShowKeyPressTotal = node[nameof(ShowKeyPressTotal)];
            LimitNotRegisteredKeys = node[nameof(LimitNotRegisteredKeys)];
            ResetOnStart = node[nameof(ResetOnStart)];
            VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);
            KPSUpdateRate = node[nameof(KPSUpdateRate)];
        }
    }
}
