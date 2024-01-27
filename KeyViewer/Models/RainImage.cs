using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class RainImage : IModel, ICopyable<RainImage>
    {
        public int Count;
        public string Image;
        public float Roundness;
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
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Count)] = Count;
            node[nameof(Image)] = Image;
            node[nameof(Roundness)] = Roundness;
            //node[nameof(BlurEnabled)] = BlurEnabled;
            //node[nameof(BlurConfig)] = BlurConfig.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Count = node[nameof(Count)];
            Image = node[nameof(Image)].IfNotExist(null);
            Roundness = node[nameof(Roundness)];
            //BlurEnabled = node[nameof(BlurEnabled)];
            //BlurConfig = ModelUtils.Unbox<BlurConfig>(node[nameof(BlurConfig)]) ?? new BlurConfig();
        }
    }
}
