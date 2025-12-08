using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class VectorConfig : IModel, ICopyable<VectorConfig>
    {
        public PressRelease<Vector3> Rotation = Vector3.zero;
        public PressRelease<Vector3> Offset = Vector3.zero;
        public PressRelease<Vector2> Scale = Vector2.one;
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
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Rotation)] = Rotation.Serialize();
            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Scale)] = Scale.Serialize();
            node[nameof(Pivot)] = Pivot.ToString();
            node[nameof(Anchor)] = Anchor.ToString();
            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new VectorConfig();

            Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Rotation)]);
            Offset = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Scale)]);
            Pivot = EnumHelper<Pivot>.Parse(
                node[nameof(Pivot)]?.Value<string>(),
                defaultSettings.Pivot
            );
            Anchor = EnumHelper<Anchor>.Parse(
                node[nameof(Anchor)]?.Value<string>(),
                defaultSettings.Anchor
            );
        }
    }
}
