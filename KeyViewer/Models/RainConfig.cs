using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Models
{
    public class RainConfig
    {
        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<float> Speed = 400f;
        public PressRelease<float> Length = 400f;
        public PressRelease<float> Width = -1f;
        public PressRelease<float> Height = -1f;
        public PressRelease<int> Softness = 100;
        public PressRelease<int> PoolSize = 25;
        public PressRelease<GColor> Color = new GColor(UnityEngine.Color.white);
        public bool ChangeRainColorWithJudge = false;
        public Judge<GColor> RainJudgeColors = null;
        public List<RainImage> RainImages = new List<RainImage>();
        public RainImageDisplayMode ImageDisplayMode = RainImageDisplayMode.Sequential;
        public Direction Direction = Direction.U;
        public RainConfig Copy()
        {
            RainConfig newConfig = new RainConfig();
            newConfig.Offset = Offset.Copy();
            newConfig.Speed = Speed.Copy();
            newConfig.Length = Length.Copy();
            newConfig.Width = Width.Copy();
            newConfig.Height = Height.Copy();
            newConfig.Softness = Softness.Copy();
            newConfig.PoolSize = PoolSize.Copy();
            newConfig.Color = Color.Copy();
            newConfig.ChangeRainColorWithJudge = ChangeRainColorWithJudge;
            newConfig.RainJudgeColors = RainJudgeColors?.Copy();
            newConfig.RainImages = new List<RainImage>(RainImages);
            newConfig.ImageDisplayMode = ImageDisplayMode;
            newConfig.Direction = Direction;
            return newConfig;
        }
    }
}
