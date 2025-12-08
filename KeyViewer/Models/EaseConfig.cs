using DG.Tweening;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models
{
    public class EaseConfig : IModel, ICopyable<EaseConfig>
    {
        public EaseConfig() { }
        public EaseConfig(Ease ease, float duration)
        {
            Ease = ease;
            Duration = duration;
        }
        public Ease Ease = Ease.Unset;
        public float Duration = 0;
        public bool IsValid => Ease != Ease.Unset && Duration != 0;
        public GUIStatus Status = new GUIStatus();
        public EaseConfig Copy()
        {
            var config = new EaseConfig();
            config.Ease = Ease;
            config.Duration = Duration;
            config.Status = Status.Copy();
            return config;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Ease)] = Ease.ToString();
            node[nameof(Duration)] = Duration;
            node[nameof(Status)] = Status.Serialize();
            return node;
        }
        public void Deserialize(JToken node) {
            var defaultSettings = new EaseConfig();

            Ease = EnumHelper<Ease>.Parse(node[nameof(Ease)]?.Value<string>() ?? defaultSettings.Ease.ToString());
            Duration = node[nameof(Duration)]?.Value<float>() ?? defaultSettings.Duration;
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? defaultSettings.Status.Copy();
        }
    }
}
