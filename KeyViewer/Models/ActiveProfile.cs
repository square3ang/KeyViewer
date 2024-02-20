using JSON;
using KeyViewer.Core.Interfaces;

namespace KeyViewer.Models
{
    public struct ActiveProfile : IModel, ICopyable<ActiveProfile>
    {
        public ActiveProfile(string name, bool active)
        {
            Name = name;
            Active = active;
            Key = null;
        }
        public ActiveProfile(string name, bool active, string key)
        {
            Name = name;
            Active = active;
            Key = key;
        }
        public string Name;
        public string Key;
        public bool Active;
        public ActiveProfile Copy()
        {
            var profile = new ActiveProfile();
            profile.Name = Name;
            profile.Key = Key;
            profile.Active = Active;
            return profile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Name)] = Name;
            node[nameof(Key)] = Key;
            node[nameof(Active)] = Active;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Name = node[nameof(Name)];
            Key = node[nameof(Key)];
            Active = node[nameof(Active)];
        }
    }
}
