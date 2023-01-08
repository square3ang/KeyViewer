using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using static UnityModManagerNet.UnityModManager;

namespace KeyViewer
{
    public class Settings : ModSettings
    {
        public override void Save(ModEntry modEntry) => Save(this, modEntry);
        public int ProfileIndex = 0;
        public List<Profile> Profiles = new List<Profile>();
        public LanguageType Language = LanguageType.English;
        public bool EditSettingEachKeys = false;
        public bool FunActivated = false;
        public bool ResetWhenStart = false;
        [XmlIgnore]
        public Profile CurrentProfile => Profiles[ProfileIndex];
    }
}
