﻿using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
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
        public bool DoNotAssAss = true;
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
            newProfile.DoNotAssAss = DoNotAssAss;
            newProfile.KeySpacing = KeySpacing;
            newProfile.VectorConfig = VectorConfig.Copy();
            newProfile.KPSUpdateRate = KPSUpdateRate;
            return newProfile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Keys)] = ModelUtils.WrapCollection(Keys);
            node[nameof(ViewOnlyGamePlay)] = ViewOnlyGamePlay;
            node[nameof(LimitNotRegisteredKeys)] = LimitNotRegisteredKeys;
            node[nameof(ResetOnStart)] = ResetOnStart;
            node[nameof(DoNotAssAss)] = DoNotAssAss;
            node[nameof(KeySpacing)] = KeySpacing;
            node[nameof(VectorConfig)] = VectorConfig.Serialize();
            node[nameof(KPSUpdateRate)] = KPSUpdateRate;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Keys = ModelUtils.UnwrapList<KeyConfig>(node[nameof(Keys)].AsArray);
            ViewOnlyGamePlay = node[nameof(ViewOnlyGamePlay)];
            LimitNotRegisteredKeys = node[nameof(LimitNotRegisteredKeys)];
            ResetOnStart = node[nameof(ResetOnStart)];
            DoNotAssAss = true;// node[nameof(DoNotAssAss)].IfNotExist(true);
            KeySpacing = node[nameof(KeySpacing)];
            VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);
            KPSUpdateRate = node[nameof(KPSUpdateRate)];
        }
    }
}
