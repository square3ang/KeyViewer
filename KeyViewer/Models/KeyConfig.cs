using DG.Tweening;
using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using UnityEngine;

namespace KeyViewer.Models
{
    public class KeyConfig : IModel, ICopyable<KeyConfig>
    {
        public int Count = 0;
        public KeyCode Code = KeyCode.None;
        public string DummyName = null;
        public string Font = "Default";
        public bool EnableKPSMeter = false;
        public bool EnableCountText = true;

        public PressRelease<string> Text = new PressRelease<string>(null);
        public PressRelease<string> CountText = new PressRelease<string>(null);
        public PressRelease<string> Background = new PressRelease<string>(null);
        public PressRelease<string> Outline = new PressRelease<string>(null);

        public ObjectConfig TextConfig = new ObjectConfig(75, Color.black, Color.white);
        public ObjectConfig CountTextConfig = new ObjectConfig(50, Color.black, Color.white);
        public ObjectConfig BackgroundConfig = new ObjectConfig(Vector2.one, Color.white, Color.black.WithAlpha(0.4f));
        public ObjectConfig OutlineConfig = new ObjectConfig(Vector2.one, Color.white, Color.white);
        public float BackgroundRoundness = 0f;
        public float OutlineRoundness = 0f;

        public VectorConfig VectorConfig = new VectorConfig() { Scale = new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one), ScaleEase = new PressRelease<EaseConfig>(new EaseConfig(Ease.OutQuad, 0.1f)) };

        public PressReleaseM<EaseConfig> ScaleEasing = new EaseConfig(Ease.OutExpo, 0.1f);

        public bool RainEnabled = false;
        public RainConfig Rain = new RainConfig();

        public KeyConfig Copy()
        {
            KeyConfig newConfig = new KeyConfig();

            newConfig.Count = Count;
            newConfig.Code = Code;
            newConfig.DummyName = DummyName;
            newConfig.Font = Font;
            newConfig.EnableKPSMeter = EnableKPSMeter;
            newConfig.EnableCountText = EnableCountText;

            newConfig.Text = Text.Copy();
            newConfig.CountText = CountText.Copy();
            newConfig.Background = Background.Copy();
            newConfig.Outline = Outline.Copy();

            newConfig.TextConfig = TextConfig.Copy();
            newConfig.CountTextConfig = CountTextConfig.Copy();
            newConfig.BackgroundConfig = BackgroundConfig.Copy();
            newConfig.OutlineConfig = OutlineConfig.Copy();
            newConfig.BackgroundRoundness = BackgroundRoundness;
            newConfig.OutlineRoundness = OutlineRoundness;

            newConfig.VectorConfig = VectorConfig.Copy();

            newConfig.ScaleEasing = ScaleEasing.Copy();

            newConfig.RainEnabled = RainEnabled;
            newConfig.Rain = Rain.Copy();

            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;

            node[nameof(Count)] = Count;
            node[nameof(Code)] = Code.ToString();
            node[nameof(DummyName)] = DummyName;
            node[nameof(Font)] = Font;
            node[nameof(EnableKPSMeter)] = EnableKPSMeter;
            node[nameof(EnableCountText)] = EnableCountText;

            node[nameof(Text)] = Text.Serialize();
            node[nameof(CountText)] = CountText.Serialize();
            node[nameof(Background)] = Background.Serialize();
            node[nameof(Outline)] = Outline.Serialize();

            node[nameof(TextConfig)] = TextConfig.Serialize();
            node[nameof(CountTextConfig)] = CountTextConfig.Serialize();
            node[nameof(BackgroundConfig)] = BackgroundConfig.Serialize();
            node[nameof(OutlineConfig)] = OutlineConfig.Serialize();
            node[nameof(BackgroundRoundness)] = BackgroundRoundness;
            node[nameof(OutlineRoundness)] = OutlineRoundness;

            node[nameof(VectorConfig)] = VectorConfig.Serialize();

            node[nameof(ScaleEasing)] = ScaleEasing.Serialize();

            node[nameof(RainEnabled)] = RainEnabled;
            node[nameof(Rain)] = Rain.Serialize();

            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Count = node[nameof(Count)];
            Code = EnumHelper<KeyCode>.Parse(node[nameof(Code)]);
            var dum = node[nameof(DummyName)];
            DummyName = dum == null ? null : dum.Value;
            Font = node[nameof(Font)];
            EnableKPSMeter = node[nameof(EnableKPSMeter)];
            EnableCountText = node[nameof(EnableCountText)];

            Text = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Text)]);
            CountText = ModelUtils.Unbox<PressRelease<string>>(node[nameof(CountText)]);
            Background = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Background)]);
            Outline = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Outline)]);

            TextConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(TextConfig)]);
            CountTextConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(CountTextConfig)]);
            BackgroundConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(BackgroundConfig)]);
            OutlineConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(OutlineConfig)]);
            BackgroundRoundness = node[nameof(BackgroundRoundness)];
            OutlineRoundness = node[nameof(OutlineRoundness)];

            VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);

            ScaleEasing = ModelUtils.Unbox<PressReleaseM<EaseConfig>>(node[nameof(ScaleEasing)]);

            RainEnabled = node[nameof(RainEnabled)];
            Rain = ModelUtils.Unbox<RainConfig>(node[nameof(Rain)]);
        }
    }
}
