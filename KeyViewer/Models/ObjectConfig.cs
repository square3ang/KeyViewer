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
    public VectorConfig VectorConfig;
    public PressReleaseModel<GColor> Color;
    public ObjectConfig Copy() {
        ObjectConfig newConfig = new() {
            VectorConfig = VectorConfig.Copy(),
            Color = Color.Copy(),
        };
        return newConfig;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(VectorConfig)] = VectorConfig.Serialize(),
            [nameof(Color)] = Color.Serialize()
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new ObjectConfig();
        VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);
        Color = ModelUtils.Unbox<PressReleaseModel<GColor>>(node[nameof(Color)]);
    }
}
