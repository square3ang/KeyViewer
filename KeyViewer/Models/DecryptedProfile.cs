using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class DecryptedProfile : IModel, ICopyable<DecryptedProfile>
    {
        public Metadata Metadata;
        public Profile Profile;
        public DecryptedProfile Copy()
        {
            var profile = new DecryptedProfile();
            profile.Metadata = Metadata.Copy();
            profile.Profile = Profile.Copy();
            return profile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Metadata)] = Metadata.Serialize();
            node[nameof(Profile)] = Profile.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Metadata = ModelUtils.Unbox<Metadata>(node[nameof(Metadata)]);
            Profile = ModelUtils.Unbox<Profile>(node[nameof(Profile)]);
        }
    }
}
