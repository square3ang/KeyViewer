using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models
{
    public class RainImage : IModel, ICopyable<RainImage>
    {
        public int Count = 0;
        public string Image = null;
        public float Roundness = 0f;
        //public bool BlurEnabled = false;
        //public BlurConfig BlurConfig = new BlurConfig();
        public RainImage Copy()
        {
            var image = new RainImage();
            image.Count = Count;
            image.Image = Image;
            image.Roundness = Roundness;
            //image.BlurEnabled = BlurEnabled;
            //image.BlurConfig = BlurConfig.Copy();
            return image;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Count)] = Count;
            node[nameof(Image)] = Image;
            node[nameof(Roundness)] = Roundness;
            //node[nameof(BlurEnabled)] = BlurEnabled;
            //node[nameof(BlurConfig)] = BlurConfig.Serialize();
            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSetttings = new RainImage();

            Count = node[nameof(Count)]?.Value<int>() ?? defaultSetttings.Count;
            Image = node[nameof(Image)]?.Value<string>() ?? defaultSetttings.Image;
            Roundness = node[nameof(Roundness)]?.Value<float>() ?? defaultSetttings.Roundness;
            //BlurEnabled = node[nameof(BlurEnabled)];
            //BlurConfig = ModelUtils.Unbox<BlurConfig>(node[nameof(BlurConfig)]) ?? new BlurConfig();
        }
    }
}
