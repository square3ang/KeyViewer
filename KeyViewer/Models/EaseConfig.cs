using DG.Tweening;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class EaseConfig : IModel, ICopyable<EaseConfig> {
    public EaseConfig() {
        Ease = Ease.Unset;
        Duration = 0;
    }
    public EaseConfig(Ease ease, float duration) {
        Ease = ease;
        Duration = duration;
    }
    public Ease Ease = Ease.Unset;
    public float Duration = 0;
    public bool IsValid => Ease != Ease.Unset;
    public EaseConfig Copy() {
        var config = new EaseConfig {
            Ease = Ease,
            Duration = Duration
        };
        return config;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Ease)] = Ease.ToString(),
            [nameof(Duration)] = Duration
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new EaseConfig();

        Ease = EnumHelper<Ease>.Parse(node[nameof(Ease)]?.Value<string>() ?? defaultSettings.Ease.ToString());
        Duration = node[nameof(Duration)]?.Value<float>() ?? defaultSettings.Duration;
    }
}
