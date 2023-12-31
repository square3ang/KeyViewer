using JSON;
using KeyViewer.Core.Interfaces;
using System.Collections.Generic;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class Settings : IModel
    {
        public KeyViewerLanguage Language = KeyViewerLanguage.English;
        public List<ActiveProfile> ActiveProfiles = new List<ActiveProfile>();// { new ActiveProfile("Default", true) };
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Language)] = Language.ToString();
            node[nameof(ActiveProfiles)] = ModelUtils.WrapList(ActiveProfiles);
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Language = EnumHelper<KeyViewerLanguage>.Parse(node[nameof(Language)]);
            ActiveProfiles = ModelUtils.UnwrapList<ActiveProfile>(node[nameof(ActiveProfiles)].AsArray);
        }
    }
}
