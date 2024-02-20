using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class EncryptedProfile : IModel, ICopyable<EncryptedProfile>
    {
        public Metadata Metadata;
        public byte[] RawProfile;
        public EncryptedProfile Copy()
        {
            var profile = new EncryptedProfile();
            profile.Metadata = Metadata.Copy();
            profile.RawProfile = (byte[])RawProfile.Clone();
            return profile;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Metadata)] = Metadata.Serialize();
            node[nameof(RawProfile)] = RawProfile;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Metadata = ModelUtils.Unbox<Metadata>(node[nameof(Metadata)]);
            RawProfile = node[nameof(RawProfile)];
        }
    }
}
