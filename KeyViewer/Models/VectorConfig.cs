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
        public Pivot Pivot = Pivot.MiddleCenter;
        public VectorConfig Copy()
        {
            VectorConfig newRos = new VectorConfig();
            newRos.Size = Size.Copy();
            newRos.Rotation = Rotation.Copy();
            newRos.Offset = Offset.Copy();
            newRos.Scale = Scale.Copy();
            newRos.Pivot = Pivot;
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
            node[nameof(Pivot)] = Pivot.ToString();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            UseSize = node[nameof(UseSize)];
            Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Rotation)]);
            Offset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Scale)]);
            Size = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Size)]);
            Pivot = EnumHelper<Pivot>.Parse(node[nameof(Pivot)]);
        }
    }
}
