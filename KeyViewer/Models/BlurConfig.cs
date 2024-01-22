using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class BlurConfig : IModel, ICopyable<BlurConfig>
    {
        public float Size = 10f;
        public GUIStatus Status = new GUIStatus();
        public BlurConfig Copy()
        {
            var newConfig = new BlurConfig();
            newConfig.Size = Size;
            newConfig.Status = Status.Copy();
            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Size)] = Size;
            node[nameof(Status)] = Status.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Size = node[nameof(Size)];
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? new GUIStatus();
        }
    }
}
