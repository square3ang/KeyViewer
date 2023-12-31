using JSON;
using KeyViewer.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class Settings : IModel
    {
        public SystemLanguage Language = SystemLanguage.English;
        public List<string> ActiveProfiles = new List<string>() { "Default.json" };
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Language)] = Language.ToString();
            node[nameof(ActiveProfiles)] = ActiveProfiles;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Language = EnumHelper<SystemLanguage>.Parse(node[nameof(Language)]);
            ActiveProfiles = node[nameof(ActiveProfiles)].AsStringList;
        }
    }
}
