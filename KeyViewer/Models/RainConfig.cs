using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Models
{
    public class RainConfig : IModel
    {
        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<float> Speed = 400f;
        public PressRelease<float> Length = 400f;
        public PressRelease<float> Width = -1f;
        public PressRelease<float> Height = -1f;
        public PressRelease<int> Softness = 100;
        public PressRelease<int> PoolSize = 25;
        public PressReleaseM<GColor> Color = new GColor(UnityEngine.Color.white);
        public bool ChangeRainColorWithJudge = false;
        public JudgeM<GColor> RainJudgeColors = null;
        public List<RainImage> RainImages = new List<RainImage>();
        public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
        public Direction Direction = Direction.U;
        public RainConfig Copy()
        {
            RainConfig newConfig = new RainConfig();
            newConfig.Offset = Offset.Copy();
            newConfig.Speed = Speed.Copy();
            newConfig.Length = Length.Copy();
            newConfig.Width = Width.Copy();
            newConfig.Height = Height.Copy();
            newConfig.Softness = Softness.Copy();
            newConfig.PoolSize = PoolSize.Copy();
            newConfig.Color = Color.Copy();
            newConfig.ChangeRainColorWithJudge = ChangeRainColorWithJudge;
            newConfig.RainJudgeColors = RainJudgeColors?.Copy();
            newConfig.RainImages = new List<RainImage>(RainImages);
            newConfig.ImageDisplayMode = ImageDisplayMode;
            newConfig.Direction = Direction;
            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Speed)] = Speed.Serialize();
            node[nameof(Length)] = Length.Serialize();
            node[nameof(Width)] = Width.Serialize();
            node[nameof(Height)] = Height.Serialize();
            node[nameof(Softness)] = Softness.Serialize();
            node[nameof(PoolSize)] = PoolSize.Serialize();
            node[nameof(Color)] = Color.Serialize();
            node[nameof(ChangeRainColorWithJudge)] = ChangeRainColorWithJudge;
            node[nameof(RainJudgeColors)] = RainJudgeColors?.Serialize();
            node[nameof(RainImages)] = ModelUtils.WrapList(RainImages);
            node[nameof(ImageDisplayMode)] = ImageDisplayMode.ToString();
            node[nameof(Direction)] = Direction.ToString();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Offset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Offset)]);
            Speed = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Speed)]);
            Length = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Length)]);
            Width = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Width)]);
            Height = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Height)]);
            Softness = ModelUtils.Unbox<PressRelease<int>>(node[nameof(Softness)]);
            PoolSize = ModelUtils.Unbox<PressRelease<int>>(node[nameof(PoolSize)]);
            Color = ModelUtils.Unbox<PressReleaseM<GColor>>(node[nameof(Color)]);
            ChangeRainColorWithJudge = node[nameof(ChangeRainColorWithJudge)];
            RainJudgeColors = ModelUtils.Unbox<JudgeM<GColor>>(node[nameof(RainJudgeColors)]);
            RainImages = ModelUtils.UnwrapList<RainImage>(node[nameof(RainImages)].AsArray);
            ImageDisplayMode = EnumHelper<RainImageDisplayMode>.Parse(node[nameof(ImageDisplayMode)]);
            Direction = EnumHelper<Direction>.Parse(node[nameof(Direction)]);
        }
    }
}
