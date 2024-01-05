using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using UnityEngine;

namespace KeyViewer.Models
{
    public class VectorConfig : IModel, ICopyable<VectorConfig>
    {
        public bool UseSize = false;
        public PressRelease<Vector3> Rotation = Vector3.zero;
        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<Vector2> Scale = Vector2.one;
        public PressRelease<float> Size = 1;

        public PressRelease<EaseConfig> RotationEase = new EaseConfig();
        public PressRelease<EaseConfig> OffsetEase = new EaseConfig();
        public PressRelease<EaseConfig> ScaleEase = new EaseConfig();
        public PressRelease<EaseConfig> SizeEase = new EaseConfig();
        public VectorConfig Copy()
        {
            VectorConfig newRos = new VectorConfig();
            newRos.Size = Size.Copy();
            newRos.Rotation = Rotation.Copy();
            newRos.Offset = Offset.Copy();
            newRos.Scale = Scale.Copy();

            newRos.SizeEase = SizeEase.Copy();
            newRos.RotationEase = RotationEase.Copy();
            newRos.OffsetEase = OffsetEase.Copy();
            newRos.ScaleEase = ScaleEase.Copy();
            return newRos;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(UseSize)] = UseSize;    
            node[nameof(Rotation)] = Rotation.Serialize();
            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Scale)] = Scale.Serialize();
            node[nameof(Size)] = Size.Serialize();
            node[nameof(RotationEase)] = RotationEase.Serialize();
            node[nameof(OffsetEase)] = OffsetEase.Serialize();
            node[nameof(ScaleEase)] = ScaleEase.Serialize();
            node[nameof(SizeEase)] = SizeEase.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            UseSize = node[nameof(UseSize)];
            Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Rotation)]);
            Offset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Scale)]);
            Size = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Size)]);
            RotationEase = ModelUtils.Unbox<PressRelease<EaseConfig>>(node[nameof(RotationEase)]);
            OffsetEase = ModelUtils.Unbox<PressRelease<EaseConfig>>(node[nameof(OffsetEase)]);
            ScaleEase = ModelUtils.Unbox<PressRelease<EaseConfig>>(node[nameof(ScaleEase)]);
            SizeEase = ModelUtils.Unbox<PressRelease<EaseConfig>>(node[nameof(SizeEase)]);
        }
    }
}
