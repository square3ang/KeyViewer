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
    //public bool BlurEnabled = false;
    //public BlurConfig BlurConfig = new BlurConfig();
    public ObjectConfig ObjectConfig = new(Vector2.one, Color.white, Color.white);
    public List<RainImage> RainImages = [];
    public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
    public Direction Direction = Direction.Up;
    public RainConfig Copy() {
        RainConfig newConfig = new() {
            Speed = Speed.Copy(),
            Length = Length.Copy(),
            Softness = Softness.Copy(),
            PoolSize = PoolSize,
            Roundness = Roundness,
            //newConfig.BlurEnabled = BlurEnabled;
            //newConfig.BlurConfig = BlurConfig.Copy();
            ObjectConfig = ObjectConfig.Copy(),
            RainImages = RainImages.ToList(),
            ImageDisplayMode = ImageDisplayMode,
            Direction = Direction
        };
        return newConfig;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Speed)] = Speed.Serialize(),
            [nameof(Length)] = Length.Serialize(),
            [nameof(Softness)] = Softness.Serialize(),
            [nameof(PoolSize)] = PoolSize,
            [nameof(Roundness)] = Roundness,
            //node[nameof(BlurEnabled)] = BlurEnabled;
            //node[nameof(BlurConfig)] = BlurConfig.Serialize();
            [nameof(ObjectConfig)] = ObjectConfig.Serialize(),
            [nameof(RainImages)] = ModelUtils.WrapCollection(RainImages),
            [nameof(ImageDisplayMode)] = ImageDisplayMode.ToString(),
            [nameof(Direction)] = Direction.ToString()
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new RainConfig();

        Speed = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Speed)]);
        Length = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Length)]);
        Softness = ModelUtils.Unbox<PressRelease<int>>(node[nameof(Softness)]);
        PoolSize = node[nameof(PoolSize)]?.Value<int>() ?? defaultSettings.PoolSize;
        Roundness = node[nameof(Roundness)]?.Value<float>() ?? defaultSettings.Roundness;
        //BlurEnabled = node[nameof(BlurEnabled)];
        //BlurConfig = ModelUtils.Unbox<BlurConfig>(node[nameof(BlurConfig)]) ?? new BlurConfig();
        ObjectConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(ObjectConfig)]);
        RainImages = ModelUtils.UnwrapList<RainImage>(node[nameof(RainImages)]);
        ImageDisplayMode = EnumHelper<RainImageDisplayMode>.Parse(
            node[nameof(ImageDisplayMode)]?.Value<string>(),
            defaultSettings.ImageDisplayMode);
        Direction = EnumHelper<Direction>.Parse(
            node[nameof(Direction)]?.Value<string>(),
            defaultSettings.Direction
        );
    }
}
