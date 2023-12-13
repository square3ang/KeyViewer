using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KeyViewer.Models
{
    public class Settings
    {
        public SystemLanguage Language = SystemLanguage.English;
        public Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>() { { "Default", new Profile() } };
        public string CurrentProfile = "Default";
    }
}
