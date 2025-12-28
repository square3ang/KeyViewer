using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class ObjectConfig : IModel, ICopyable<ObjectConfig>
    {
        public ObjectConfig() { }
        public ObjectConfig(Vector2 defaultScale, Color defaultPressed, Color defaultReleased)
        {
            VectorConfig = new VectorConfig();
            VectorConfig.Scale = defaultScale;
            Color = new PressReleaseM<GColor>(defaultPressed, defaultReleased);
        }
        public ObjectConfig(Vector2 pressedScale, Vector2 releasedScale, Color defaultPressed, Color defaultReleased)
        {
            VectorConfig = new VectorConfig();
            VectorConfig.Scale.Pressed = pressedScale;
            VectorConfig.Scale.Released = releasedScale;
            Color = new PressReleaseM<GColor>(defaultPressed, defaultReleased);
        }
        public ObjectConfig(PressRelease<Vector2> scale, Color defaultPressed, Color defaultReleased)
        {
            VectorConfig = new VectorConfig();
            VectorConfig.Scale = scale;
            Color = new PressReleaseM<GColor>(defaultPressed, defaultReleased);
        }
        public VectorConfig VectorConfig;
        public PressReleaseM<GColor> Color;
        public bool ChangeColorWithJudge = false;
        public JudgeM<GColor> JudgeColors = null;
        public EaseConfig JudgeColorEase = new EaseConfig();
        public ObjectConfig Copy()
        {
            ObjectConfig newConfig = new ObjectConfig();
            newConfig.VectorConfig = VectorConfig.Copy();
            newConfig.Color = Color.Copy();
            newConfig.ChangeColorWithJudge = ChangeColorWithJudge;
            newConfig.JudgeColors = JudgeColors?.Copy();
            newConfig.JudgeColorEase = JudgeColorEase.Copy();
            return newConfig;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(VectorConfig)] = VectorConfig.Serialize();
            node[nameof(Color)] = Color.Serialize();
            if(ChangeColorWithJudge) {
                node[nameof(ChangeColorWithJudge)] = ChangeColorWithJudge;
            }
            if(JudgeColors != null) {
                node[nameof(JudgeColors)] = JudgeColors.Serialize();
            }
            if(JudgeColorEase != null && JudgeColorEase.IsValid) {
                node[nameof(JudgeColorEase)] = JudgeColorEase.Serialize();
            }
            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new ObjectConfig();
            VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);
            Color = ModelUtils.Unbox<PressReleaseM<GColor>>(node[nameof(Color)]);
            ChangeColorWithJudge = node[nameof(ChangeColorWithJudge)]?.Value<bool>() ?? defaultSettings.ChangeColorWithJudge;
            JudgeColors = ModelUtils.Unbox<JudgeM<GColor>>(node[nameof(JudgeColors)]);
            JudgeColorEase = ModelUtils.Unbox<EaseConfig>(node[nameof(JudgeColorEase)]) ?? new EaseConfig();
        }
    }
}
