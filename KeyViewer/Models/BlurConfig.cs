using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class BlurConfig : IModel, ICopyable<BlurConfig>
    {
        public float Spacing = 2f;
        public float Vibrancy = 0.3f;
        public GUIStatus Status = new GUIStatus();
        public BlurConfig Copy()
        {
            var newConfig = new BlurConfig();
            newConfig.Spacing = Spacing;
            newConfig.Vibrancy = Vibrancy;
            newConfig.Status = Status.Copy();
            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Spacing)] = Spacing;
            node[nameof(Vibrancy)] = Vibrancy;
            node[nameof(Status)] = Status.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Spacing = node[nameof(Spacing)];
            Vibrancy = node[nameof(Vibrancy)];
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? new GUIStatus();
        }
    }
}
