using KeyViewer.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class RainImage : IModel, ICopyable<RainImage> {
    public int Count = 0;
    public string Image = null;
    public float Roundness = 0f;
    public RainImage Copy() {
        var image = new RainImage {
            Count = Count,
            Image = Image,
            Roundness = Roundness
        };
        return image;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Count)] = Count,
            [nameof(Image)] = Image,
            [nameof(Roundness)] = Roundness
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSetttings = new RainImage();
        Count = node[nameof(Count)]?.Value<int>() ?? defaultSetttings.Count;
        Image = node[nameof(Image)]?.Value<string>() ?? defaultSetttings.Image;
        Roundness = node[nameof(Roundness)]?.Value<float>() ?? defaultSetttings.Roundness;
    }
}
