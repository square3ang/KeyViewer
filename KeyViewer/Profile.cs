using System.Collections.Generic;
using System.Linq;

namespace KeyViewer
{
    public class Profile
    {
        public string Name = "Default Profile";
        public Key.Config GlobalConfig = new Key.Config();
        public List<Group> KeyGroups = new List<Group>();
        public List<Key.Config> ActiveKeys = new List<Key.Config>();
        public bool MakeBarSpecialKeys = true;
        public bool IgnoreSkippedKeys;
        public bool ViewerOnlyGameplay;
        public bool EditingKeyGroups;
        public bool AnimateKeys = true;
        public bool ShowKeyPressTotal = true;
        public float KeyViewerSize = 100f;
        public float KeyViewerXPos = 0.89f;
        public float KeyViewerYPos = 0.03f;
        public int ConfigBackupsCount = 100;
        public int KPSUpdateRateMs = 1000;
        public bool EditEachKeys = false;
        public bool ResetWhenStart = false;
        public Profile Copy()
        {
            Profile prof = new Profile();
            prof.Name = Name;
            prof.GlobalConfig = GlobalConfig.Copy();
            prof.IgnoreSkippedKeys = IgnoreSkippedKeys;
            prof.MakeBarSpecialKeys = MakeBarSpecialKeys;
            prof.KeyGroups = KeyGroups.Select(g => g.Copy()).ToList();
            prof.ActiveKeys = ActiveKeys.Select(c => c.Copy()).ToList();
            prof.ViewerOnlyGameplay = ViewerOnlyGameplay;
            prof.AnimateKeys = AnimateKeys;
            prof.ShowKeyPressTotal = ShowKeyPressTotal;
            prof.KeyViewerSize = KeyViewerSize;
            prof.KeyViewerXPos = KeyViewerXPos;
            prof.KeyViewerYPos = KeyViewerYPos;
            return prof;
        }
        public void Init(KeyManager manager)
        {
            GlobalConfig.keyManager = manager;
            KeyGroups.ForEach(g => g.keyManager = manager);
            ActiveKeys.ForEach(k => k.keyManager = manager);
        }
    }
}
