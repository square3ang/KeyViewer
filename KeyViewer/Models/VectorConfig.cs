using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class VectorConfig : IModel, ICopyable<VectorConfig>
    {
        public PressRelease<Vector3> Rotation = Vector3.zero;
        public PressRelease<Vector2> Offset = Vector2.zero;
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
            if(Rotation.Pressed != Vector3.zero && Rotation.Released != Vector3.zero) {
                node[nameof(Rotation)] = Rotation.Serialize();
            }
            if(Offset.Pressed != Vector2.zero && Offset.Released != Vector2.zero) {
                node[nameof(Offset)] = Offset.Serialize();
            }
            if(Scale.Pressed != Vector2.one || Scale.Released != Vector2.one) {
                node[nameof(Scale)] = Scale.Serialize();
            }
            if(Pivot != Pivot.MiddleCenter) {
                node[nameof(Pivot)] = Pivot.ToString();
            }
            if(Anchor != Anchor.MiddleCenter) {
                node[nameof(Anchor)] = Anchor.ToString();
            }
            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new VectorConfig();

            JToken rotationRaw = node[nameof(Rotation)];
            if(rotationRaw == null) {
                Rotation = defaultSettings.Rotation;
            } else {
                Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(rotationRaw);
            }
            JToken offsetRaw = node[nameof(Offset)];
            if(offsetRaw == null) {
                Offset = defaultSettings.Offset;
            } else {
                Offset = ModelUtils.Unbox<PressRelease<Vector2>>(offsetRaw);
            }
            JToken scaleRaw = node[nameof(Scale)];
            if(scaleRaw == null) {
                Scale = defaultSettings.Scale;
            } else {
                Scale = ModelUtils.Unbox<PressRelease<Vector2>>(scaleRaw);
            }
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
