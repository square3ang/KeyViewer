using KeyViewer.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class BlurConfig : IModel, ICopyable<BlurConfig> {
    public float Spacing = 2f;
    public float Vibrancy = 0.3f;

    public BlurConfig Copy() {
        return new BlurConfig {
            Spacing = this.Spacing,
            Vibrancy = this.Vibrancy,
        };
    }

    public JToken Serialize() {
        var node = new JObject {
            [nameof(Spacing)] = Spacing,
            [nameof(Vibrancy)] = Vibrancy,
        };
        return node;
    }

    public void Deserialize(JToken node) {
        if(node == null) {
            return;
        }

        Spacing = node[nameof(Spacing)]?.Value<float>() ?? 2f;
        Vibrancy = node[nameof(Vibrancy)]?.Value<float>() ?? 0.3f;
    }
}