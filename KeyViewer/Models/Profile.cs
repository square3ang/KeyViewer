using KeyViewer.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class Profile : IModel
    {
        public List<KeyConfig> Keys = new List<KeyConfig>();
        public bool MakeBarSpecialKeys = true;
        public bool ViewOnlyGamePlay = false;
        public bool AnimateKeys = true;
        public bool ShowKeyPressTotal = true;
        public bool LimitNotRegisteredKeys = true;
        public bool ResetOnStart = false;
        public float Size = 100;
        public Vector2 Position = Vector2.zero;
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
            newProfile.Position = Position;
            newProfile.KPSUpdateRate = KPSUpdateRate;
            return newProfile;
        }
    }
}
