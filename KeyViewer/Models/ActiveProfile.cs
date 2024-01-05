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
        }
        public string Name;
        public bool Active;
        public ActiveProfile Copy()
        {
            var profile = new ActiveProfile();
            profile.Name = Name;
            profile.Active = Active;
            return profile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Name)] = Name;
            node[nameof(Active)] = Active;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Name = node[nameof(Name)];
            Active = node[nameof(Active)];
        }
    }
}
