using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Models
{
    public class Settings
    {
        public SystemLanguage Language = SystemLanguage.English;
        public List<string> ActiveProfiles = new List<string>() { "Default.json" };
    }
}
