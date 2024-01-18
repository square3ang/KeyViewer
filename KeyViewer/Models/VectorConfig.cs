using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using UnityEngine;

namespace KeyViewer.Models
{
    public class VectorConfig : IModel, ICopyable<VectorConfig>
    {
        public PressRelease<Vector3> Rotation = Vector3.zero;
        public PressRelease<Vector3> Offset = Vector3.zero;
        public PressRelease<Vector3> Scale = Vector3.one;
        public Pivot Pivot = Pivot.MiddleCenter;
        public Anchor Anchor = Anchor.MiddleCenter;
        public VectorConfig Copy()
        {
            VectorConfig newRos = new VectorConfig();
            newRos.Rotation = Rotation.Copy();
            newRos.Offset = Offset.Copy();
            newRos.Scale = Scale.Copy();
            newRos.Pivot = Pivot;
            newRos.Anchor = Anchor;
            return newRos;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Rotation)] = Rotation.Serialize();
            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Scale)] = Scale.Serialize();
            node[nameof(Pivot)] = Pivot.ToString();
            node[nameof(Anchor)] = Anchor.ToString();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Rotation)]);
            Offset = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Scale)]);
            Pivot = EnumHelper<Pivot>.Parse(node[nameof(Pivot)].IfNotExist(nameof(Pivot.MiddleCenter)));
            Anchor = EnumHelper<Anchor>.Parse(node[nameof(Anchor)].IfNotExist(nameof(Anchor.MiddleCenter)));
        }
    }
}
