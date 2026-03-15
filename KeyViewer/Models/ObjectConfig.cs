using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models;

public class ObjectConfig : IModel, ICopyable<ObjectConfig> {
    public ObjectConfig() { }
    public ObjectConfig(Vector2 defaultScale, Color defaultPressed, Color defaultReleased) {
        VectorConfig = new VectorConfig {
            Scale = defaultScale
        };
        Color = new PressReleaseModel<GColor>(defaultPressed, defaultReleased);
    }
    public ObjectConfig(Vector2 pressedScale, Vector2 releasedScale, Color defaultPressed, Color defaultReleased) {
        VectorConfig = new VectorConfig();
        VectorConfig.Scale.Pressed = pressedScale;
        VectorConfig.Scale.Released = releasedScale;
        Color = new PressReleaseModel<GColor>(defaultPressed, defaultReleased);
    }
    public ObjectConfig(PressRelease<Vector2> scale, Color defaultPressed, Color defaultReleased) {
        VectorConfig = new VectorConfig {
            Scale = scale
        };
        Color = new PressReleaseModel<GColor>(defaultPressed, defaultReleased);
    }
    public VectorConfig VectorConfig = new();
    public PressReleaseModel<GColor> Color = new();
    public ObjectConfig Copy() {
        return new ObjectConfig {
            VectorConfig = VectorConfig?.Copy(),
            Color = Color?.Copy()
        };
    }
    public JToken Serialize() {
        var node = new JObject();
        ModelUtils.PutIfNotEmpty(node, nameof(VectorConfig), VectorConfig?.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(Color), Color?.Serialize());
        return node;
    }
    public void Deserialize(JToken node) {
        if(node == null) {
            VectorConfig = new VectorConfig();
            Color = new PressReleaseModel<GColor>();
            return;
        }
        VectorConfig = node[nameof(VectorConfig)] != null
            ? ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)])
            : new VectorConfig();
        Color = node[nameof(Color)] != null
            ? ModelUtils.Unbox<PressReleaseModel<GColor>>(node[nameof(Color)])
            : new PressReleaseModel<GColor>();
    }
}
