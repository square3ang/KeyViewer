using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models;

public class VectorConfig : IModel, ICopyable<VectorConfig> {
    public PressRelease<Vector3> Rotation = Vector3.zero;
    public PressRelease<Vector2> Offset = Vector2.zero;
    public PressRelease<Vector2> Scale = Vector2.one;
    public Pivot Pivot = Pivot.MiddleCenter;
    public Anchor Anchor = Anchor.MiddleCenter;

    public VectorConfig Copy() {
        VectorConfig newRos = new() {
            Rotation = Rotation.Copy(),
            Offset = Offset.Copy(),
            Scale = Scale.Copy(),
            Pivot = Pivot,
            Anchor = Anchor
        };
        return newRos;
    }
    public JToken Serialize() {
        var node = new JObject();
        if(Rotation.Pressed != Vector3.zero || Rotation.Released != Vector3.zero) {
            node[nameof(Rotation)] = Rotation.Serialize();
        }
        if(Offset.Pressed != Vector2.zero || Offset.Released != Vector2.zero) {
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
    public void Deserialize(JToken node) {
        var defaultSettings = new VectorConfig();

        JToken rotationRaw = node[nameof(Rotation)];
        Rotation = rotationRaw == null ? defaultSettings.Rotation : ModelUtils.Unbox<PressRelease<Vector3>>(rotationRaw);
        JToken offsetRaw = node[nameof(Offset)];
        Offset = offsetRaw == null ? defaultSettings.Offset : ModelUtils.Unbox<PressRelease<Vector2>>(offsetRaw);
        JToken scaleRaw = node[nameof(Scale)];
        Scale = scaleRaw == null ? defaultSettings.Scale : ModelUtils.Unbox<PressRelease<Vector2>>(scaleRaw);
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
