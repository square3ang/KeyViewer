using System.Collections.Generic;
using System.Xml.Serialization;

namespace KeyViewer.Migration.V3
{
    [XmlRoot("Profile")]
    public class V3Profile
    {
        public string Name = "Default Profile";
        [XmlElement("Config")]
        public Key_Config GlobalConfig = new Key_Config();
        [XmlArrayItem("Group")]
        public List<Group> KeyGroups = new List<Group>();
        [XmlArrayItem("Config")]
        public List<Key_Config> ActiveKeys = new List<Key_Config>();
        public bool MakeBarSpecialKeys = true;
        public bool IgnoreSkippedKeys;
        public bool ViewerOnlyGameplay;
        public bool EditingKeyGroups;
        public bool AnimateKeys = true;
        public bool ShowKeyPressTotal = true;
        public bool LimitNotRegisteredKeys = false;
        public float KeyViewerSize = 100f;
        public float KeyViewerXPos = 0.89f;
        public float KeyViewerYPos = 0.03f;
        public int KPSUpdateRateMs = 1000;
        public bool EditEachKeys = false;
        public bool ResetWhenStart = false;
        public bool ApplyWithOffset = false;
        public bool SequentialRainImage = true;
        public bool ShuffleRainImage = false;
    }
}
