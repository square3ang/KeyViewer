using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models {
    public class BlurConfig : IModel, ICopyable<BlurConfig> {
        public float Spacing = 2f;
        public float Vibrancy = 0.3f;
        public GUIStatus Status = new GUIStatus();

        public BlurConfig Copy() {
            return new BlurConfig {
                Spacing = this.Spacing,
                Vibrancy = this.Vibrancy,
                Status = this.Status.Copy()
            };
        }

        public JToken Serialize() {
            var node = new JObject {
                [nameof(Spacing)] = Spacing,
                [nameof(Vibrancy)] = Vibrancy,
                [nameof(Status)] = Status.Serialize()
            };
            return node;
        }

        public void Deserialize(JToken node) {
            if(node == null) {
                return;
            }

            Spacing = node[nameof(Spacing)]?.Value<float>() ?? 2f;
            Vibrancy = node[nameof(Vibrancy)]?.Value<float>() ?? 0.3f;
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? new GUIStatus();
        }
    }
}