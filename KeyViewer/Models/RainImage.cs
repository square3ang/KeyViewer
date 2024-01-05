using JSON;
using KeyViewer.Core.Interfaces;

namespace KeyViewer.Models
{
    public struct RainImage : IModel, ICopyable<RainImage>
    {
        public int Count;
        public string Image;
        public RainImage Copy()
        {
            var image = new RainImage();
            image.Count = Count;
            image.Image = Image;
            return image;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Count)] = Count;
            node[nameof(Image)] = Image;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Count = node[nameof(Count)];
            var img = node[nameof(Image)];
            if (img == null)
                Image = null;
            else Image = img.Value;
        }
    }
}
