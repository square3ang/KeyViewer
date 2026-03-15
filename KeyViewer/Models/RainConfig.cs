using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Models;

public class RainConfig : IModel, ICopyable<RainConfig> {

    public PressRelease<float> Speed = 400f;
    public PressRelease<float> Length = 400f;
    public PressReleaseBase<int> Softness = 100;

    public int PoolSize = 32;
    public float Roundness = 0;

    public ObjectConfig ObjectConfig = new(Vector2.one, Color.white, Color.white);

    public List<RainImage> RainImages = [];

    public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
    public Direction Direction = Direction.Up;

    public RainConfig Copy() {
        return new RainConfig {
            Speed = Speed?.Copy(),
            Length = Length?.Copy(),
            Softness = Softness?.Copy(),
            PoolSize = PoolSize,
            Roundness = Roundness,
            ObjectConfig = ObjectConfig?.Copy(),
            RainImages = RainImages?.ToList() ?? [],
            ImageDisplayMode = ImageDisplayMode,
            Direction = Direction
        };
    }

    public JToken Serialize() {
        var node = new JObject();
        var defaults = new RainConfig();
        if(!Speed.IsSame || Speed.Pressed != defaults.Speed.Pressed) {
            node[nameof(Speed)] = Speed.Serialize();
        }
        if(!Length.IsSame || Length.Pressed != defaults.Length.Pressed) {
            node[nameof(Length)] = Length.Serialize();
        }
        if(!Softness.IsSame || Softness.Pressed != defaults.Softness.Pressed) {
            node[nameof(Softness)] = Softness.Serialize();
        }
        if(PoolSize != defaults.PoolSize) {
            node[nameof(PoolSize)] = PoolSize;
        }
        if(Roundness != defaults.Roundness) {
            node[nameof(Roundness)] = Roundness;
        }
        node[nameof(ObjectConfig)] = ObjectConfig.Serialize();
        if(RainImages.Count > 0) {
            node[nameof(RainImages)] = ModelUtils.WrapCollection(RainImages);
        }
        if(ImageDisplayMode != defaults.ImageDisplayMode) {
            node[nameof(ImageDisplayMode)] = ImageDisplayMode.ToString();
        }
        if(Direction != defaults.Direction) {
            node[nameof(Direction)] = Direction.ToString();
        }
        return node;
    }
    public void Deserialize(JToken node) {
        var defaults = new RainConfig();
        if(node == null) {
            return;
        }
        Speed = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Speed)]) ?? defaults.Speed.Copy();
        Length = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Length)]) ?? defaults.Length.Copy();
        Softness = ModelUtils.Unbox<PressReleaseBase<int>>(node[nameof(Softness)]) ?? defaults.Softness.Copy();
        PoolSize = node[nameof(PoolSize)]?.Value<int>() ?? defaults.PoolSize;
        Roundness = node[nameof(Roundness)]?.Value<float>() ?? defaults.Roundness;
        ObjectConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(ObjectConfig)]) ?? defaults.ObjectConfig.Copy();
        RainImages = ModelUtils.UnwrapList<RainImage>(node[nameof(RainImages)]) ?? [];
        ImageDisplayMode = EnumHelper<RainImageDisplayMode>.Parse(
            node[nameof(ImageDisplayMode)]?.Value<string>(),
            defaults.ImageDisplayMode
        );
        Direction = EnumHelper<Direction>.Parse(
            node[nameof(Direction)]?.Value<string>(),
            defaults.Direction
        );
    }
}
