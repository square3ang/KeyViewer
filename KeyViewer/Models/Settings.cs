using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Models
{
    public class Settings : IModel, ICopyable<Settings>
    {
        public KeyViewerLanguage Language = KeyViewerLanguage.English;
        public List<ActiveProfile> ActiveProfiles = new List<ActiveProfile>();
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Language)] = Language.ToString();
            node[nameof(ActiveProfiles)] = ModelUtils.WrapCollection(ActiveProfiles);
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Language = EnumHelper<KeyViewerLanguage>.Parse(node[nameof(Language)]);
            ActiveProfiles = ModelUtils.UnwrapList<ActiveProfile>(node[nameof(ActiveProfiles)].AsArray);
        }
        public Settings Copy()
        {
            var newSettings = new Settings();
            newSettings.Language = Language;
            newSettings.ActiveProfiles = ActiveProfiles.Select(p => p.Copy()).ToList();
            return newSettings;
        }
    }
}
