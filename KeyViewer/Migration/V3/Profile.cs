using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Migration.V3
{
    public class Profile
    {
        public string Name = "Default Profile";
        public Key_Config GlobalConfig = new Key_Config();
        public List<Group> KeyGroups = new List<Group>();
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
    }
}
