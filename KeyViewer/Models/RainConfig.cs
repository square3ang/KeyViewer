using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Models
{
    public class RainConfig : IModel, ICopyable<RainConfig>
    {
        public PressRelease<float> Speed = 400f;
        public PressRelease<float> Length = 400f;
        public PressRelease<int> Softness = 100;
        public int PoolSize = 25;
        public float Roundness = 0;
        public ObjectConfig ObjectConfig = new ObjectConfig(Vector2.one, Color.white, Color.white);
        public List<RainImage> RainImages = new List<RainImage>();
        public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
        public Direction Direction = Direction.Up;
        public RainConfig Copy()
        {
            RainConfig newConfig = new RainConfig();
            newConfig.Speed = Speed.Copy();
            newConfig.Length = Length.Copy();
            newConfig.Softness = Softness.Copy();
            newConfig.PoolSize = PoolSize;
            newConfig.Roundness = Roundness;
            newConfig.ObjectConfig = ObjectConfig.Copy();
            newConfig.RainImages = new List<RainImage>(RainImages);
            newConfig.ImageDisplayMode = ImageDisplayMode;
            newConfig.Direction = Direction;
            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Speed)] = Speed.Serialize();
            node[nameof(Length)] = Length.Serialize();
            node[nameof(Softness)] = Softness.Serialize();
            node[nameof(PoolSize)] = PoolSize;
            node[nameof(Roundness)] = Roundness;
            node[nameof(ObjectConfig)] = ObjectConfig.Serialize();
            node[nameof(RainImages)] = ModelUtils.WrapList(RainImages);
            node[nameof(ImageDisplayMode)] = ImageDisplayMode.ToString();
            node[nameof(Direction)] = Direction.ToString();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Speed = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Speed)]);
            Length = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Length)]);
            Softness = ModelUtils.Unbox<PressRelease<int>>(node[nameof(Softness)]);
            PoolSize = node[nameof(PoolSize)];
            Roundness = node[nameof(Roundness)];
            ObjectConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(ObjectConfig)]);
            RainImages = ModelUtils.UnwrapList<RainImage>(node[nameof(RainImages)].AsArray);
            ImageDisplayMode = EnumHelper<RainImageDisplayMode>.Parse(node[nameof(ImageDisplayMode)]);
            Direction = EnumHelper<Direction>.Parse(node[nameof(Direction)]);
        }
    }
}
