using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using static UnityModManagerNet.UnityModManager;

namespace KeyViewer.Migration.V3
{
    public class V3Settings : ModSettings
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
