﻿using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class RainConfig : IModel, ICopyable<RainConfig>
    {
        public PressRelease<float> Speed = 400f;
        public PressRelease<float> Length = 400f;
        public PressRelease<int> Softness = 100;
        public int PoolSize = 25;
        public float Roundness = 0;
        //public bool BlurEnabled = false;
        //public BlurConfig BlurConfig = new BlurConfig();
        public ObjectConfig ObjectConfig = new ObjectConfig(Vector2.one, Color.white, Color.white);
        public List<RainImage> RainImages = new List<RainImage>();
        public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
        public Direction Direction = Direction.Up;
        public RainConfig Copy()
        {
            RainConfig newConfig = new RainConfig();
            newConfig.Speed = Speed.Copy();
            newConfig.Length = Length.Copy();
            newConfig.Softness = Softness.Copy();
            newConfig.PoolSize = PoolSize;
            newConfig.Roundness = Roundness;
            //newConfig.BlurEnabled = BlurEnabled;
            //newConfig.BlurConfig = BlurConfig.Copy();
            newConfig.ObjectConfig = ObjectConfig.Copy();
            newConfig.RainImages = RainImages.ToList();
            newConfig.ImageDisplayMode = ImageDisplayMode;
            newConfig.Direction = Direction;
            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Speed)] = Speed.Serialize();
            node[nameof(Length)] = Length.Serialize();
            node[nameof(Softness)] = Softness.Serialize();
            node[nameof(PoolSize)] = PoolSize;
            node[nameof(Roundness)] = Roundness;
            //node[nameof(BlurEnabled)] = BlurEnabled;
            //node[nameof(BlurConfig)] = BlurConfig.Serialize();
            node[nameof(ObjectConfig)] = ObjectConfig.Serialize();
            node[nameof(RainImages)] = ModelUtils.WrapCollection(RainImages);
            node[nameof(ImageDisplayMode)] = ImageDisplayMode.ToString();
            node[nameof(Direction)] = Direction.ToString();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Speed = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Speed)]);
            Length = ModelUtils.Unbox<PressRelease<float>>(node[nameof(Length)]);
            Softness = ModelUtils.Unbox<PressRelease<int>>(node[nameof(Softness)]);
            PoolSize = node[nameof(PoolSize)];
            Roundness = node[nameof(Roundness)];
            //BlurEnabled = node[nameof(BlurEnabled)];
            //BlurConfig = ModelUtils.Unbox<BlurConfig>(node[nameof(BlurConfig)]) ?? new BlurConfig();
            ObjectConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(ObjectConfig)]);
            RainImages = ModelUtils.UnwrapList<RainImage>(node[nameof(RainImages)].AsArray) ?? new List<RainImage>();
            ImageDisplayMode = EnumHelper<RainImageDisplayMode>.Parse(node[nameof(ImageDisplayMode)]);
            Direction = EnumHelper<Direction>.Parse(node[nameof(Direction)]);
        }
    }
}
