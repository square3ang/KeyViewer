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
        public int BackupInterval = 10;
        [XmlIgnore]
        public Profile CurrentProfile => Profiles[ProfileIndex];
    }
}
