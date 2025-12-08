using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models
{
    public class Settings : IModel, ICopyable<Settings>
    {
        public string Lang = "Default";
        public bool useLegacyTheme = false;
        public List<ActiveProfile> ActiveProfiles = new List<ActiveProfile>();
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Lang)] = Lang;
            node[nameof(ActiveProfiles)] = ModelUtils.WrapCollection(ActiveProfiles);

            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new Settings();

            Lang = node[nameof(Lang)]?.Value<string>() ?? defaultSettings.Lang;
            var profilesArray = node[nameof(ActiveProfiles)] as JArray;
            if(profilesArray != null) {
                ActiveProfiles = ModelUtils.UnwrapList<ActiveProfile>(profilesArray);
            } else {
                ActiveProfiles = new List<ActiveProfile>();
            }
        }
        public Settings Copy()
        {
            var newSettings = new Settings();
            newSettings.Lang = Lang;
            newSettings.ActiveProfiles = ActiveProfiles.Select(p => p.Copy()).ToList();
            return newSettings;
        }
    }
}
