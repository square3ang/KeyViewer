using DG.Tweening;
using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public struct EaseConfig : IModel
    {
        public EaseConfig(Ease ease, float duration, float shrinkFactor)
        {
            Ease = ease;
            Duration = duration;
            ShrinkFactor = shrinkFactor;
        }
        public Ease Ease;
        public float Duration;
        public float ShrinkFactor;
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Ease)] = Ease.ToString();
            node[nameof(Duration)] = Duration;
            node[nameof(ShrinkFactor)] = ShrinkFactor;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Ease = EnumHelper<Ease>.Parse(node[nameof(Ease)]);
            Duration = node[nameof(Duration)];
            ShrinkFactor = node[nameof(ShrinkFactor)];
        }
    }
}
