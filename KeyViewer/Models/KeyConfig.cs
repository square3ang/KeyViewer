using DG.Tweening;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models
{
    public class KeyConfig : IModel, ICopyable<KeyConfig>
    {
        public int Count = 0;
        public KeyCode Code = KeyCode.None;
        //public KeyCode[] Codes = new KeyCode[0];
        public string DummyName = null;
        public string Font = "Default";
        public bool EnableKPSMeter = false;
        public bool UpdateTextAlways = false;
        public bool EnableCountText = true;
        public bool EnableOutlineImage = true;
        public bool DisableSorting = false;
        public bool DoNotScaleText = true;
        public bool BackgroundBlurEnabled = false;
        public float TextFontSize = 75;
        public float CountTextFontSize = 50;

        public PressRelease<string> Text = new PressRelease<string>(null);
        public PressRelease<string> CountText = new PressRelease<string>(null);
        public PressRelease<string> Background = new PressRelease<string>(null);
        public PressRelease<string> Outline = new PressRelease<string>(null);

        public ObjectConfig TextConfig = new ObjectConfig(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.black, Color.white);
        public ObjectConfig CountTextConfig = new ObjectConfig(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.black, Color.white);
        public ObjectConfig BackgroundConfig = new ObjectConfig(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.white, Color.black.WithAlpha(0.4f));
        public ObjectConfig OutlineConfig = new ObjectConfig(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.white, Color.white);
        public float BackgroundRoundness = 0f;
        public float OutlineRoundness = 0f;
        public BlurConfig BackgroundBlurConfig = new BlurConfig();

        public VectorConfig VectorConfig = new VectorConfig();

        public bool RainEnabled = false;
        public RainConfig Rain = new RainConfig();

        public KeyConfig Copy()
        {
            KeyConfig newConfig = new KeyConfig();

            newConfig.Count = Count;
            newConfig.Code = Code;
            //newConfig.Codes = (KeyCode[])Codes.Clone();
            newConfig.DummyName = DummyName;
            newConfig.Font = Font;
            newConfig.EnableKPSMeter = EnableKPSMeter;
            newConfig.UpdateTextAlways = UpdateTextAlways;
            newConfig.EnableCountText = EnableCountText;
            newConfig.EnableOutlineImage = EnableOutlineImage;
            newConfig.DisableSorting = DisableSorting;
            newConfig.DoNotScaleText = DoNotScaleText;
            newConfig.BackgroundBlurEnabled = BackgroundBlurEnabled;
            newConfig.TextFontSize = TextFontSize;
            newConfig.CountTextFontSize = CountTextFontSize;

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
            newConfig.BackgroundBlurConfig = BackgroundBlurConfig.Copy();

            newConfig.VectorConfig = VectorConfig.Copy();

            newConfig.RainEnabled = RainEnabled;
            newConfig.Rain = Rain.Copy();
            return newConfig;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Count)] = Count;
            node[nameof(Code)] = Code.ToString();
            //node[nameof(Codes)] = Codes.Select(k => k.ToString()).ToArray();
            node[nameof(DummyName)] = DummyName;
            node[nameof(Font)] = Font;
            node[nameof(EnableKPSMeter)] = EnableKPSMeter;
            node[nameof(UpdateTextAlways)] = UpdateTextAlways;
            node[nameof(EnableCountText)] = EnableCountText;
            node[nameof(EnableOutlineImage)] = EnableOutlineImage;
            node[nameof(DisableSorting)] = DisableSorting;
            node[nameof(DoNotScaleText)] = DoNotScaleText;
            node[nameof(BackgroundBlurEnabled)] = BackgroundBlurEnabled;
            node[nameof(TextFontSize)] = TextFontSize;
            node[nameof(CountTextFontSize)] = CountTextFontSize;

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
            node[nameof(BackgroundBlurConfig)] = BackgroundBlurConfig.Serialize();

            node[nameof(VectorConfig)] = VectorConfig.Serialize();

            node[nameof(RainEnabled)] = RainEnabled;
            node[nameof(Rain)] = Rain.Serialize();

            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new KeyConfig();

            Count = node[nameof(Count)]?.Value<int>() ?? defaultSettings.Count;
            Code = EnumHelper<KeyCode>.Parse(node[nameof(Code)]?.Value<string>() ?? defaultSettings.Code.ToString());
            //Codes = node[nameof(Codes)].IfNotExist(new JsonArray()).AsArray.Values.Select(n => EnumHelper<KeyCode>.Parse(n.Value)).ToArray();
            DummyName = node[nameof(DummyName)]?.Value<string>() ?? defaultSettings.DummyName;
            Font = node[nameof(Font)]?.Value<string>() ?? defaultSettings.Font;
            EnableKPSMeter = node[nameof(EnableKPSMeter)]?.Value<bool>() ?? defaultSettings.EnableKPSMeter;
            UpdateTextAlways = node[nameof(UpdateTextAlways)]?.Value<bool>() ?? defaultSettings.UpdateTextAlways;
            EnableCountText = node[nameof(EnableCountText)]?.Value<bool>() ?? defaultSettings.EnableCountText;
            EnableOutlineImage = node[nameof(EnableOutlineImage)]?.Value<bool>() ?? defaultSettings.EnableOutlineImage;
            DisableSorting = node[nameof(DisableSorting)]?.Value<bool>() ?? defaultSettings.DisableSorting;
            DoNotScaleText = node[nameof(DoNotScaleText)]?.Value<bool>() ?? defaultSettings.DoNotScaleText;
            BackgroundBlurEnabled = node[nameof(BackgroundBlurEnabled)]?.Value<bool>() ?? defaultSettings.BackgroundBlurEnabled;
            TextFontSize = node[nameof(TextFontSize)]?.Value<float>() ?? defaultSettings.TextFontSize;
            CountTextFontSize = node[nameof(CountTextFontSize)]?.Value<float>() ?? defaultSettings.CountTextFontSize;

            Text = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Text)]);
            CountText = ModelUtils.Unbox<PressRelease<string>>(node[nameof(CountText)]);
            Background = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Background)]);
            Outline = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Outline)]);

            TextConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(TextConfig)]);
            CountTextConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(CountTextConfig)]);
            BackgroundConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(BackgroundConfig)]);
            OutlineConfig = ModelUtils.Unbox<ObjectConfig>(node[nameof(OutlineConfig)]);
            BackgroundRoundness = node[nameof(BackgroundRoundness)]?.Value<float>() ?? defaultSettings.BackgroundRoundness;
            OutlineRoundness = node[nameof(OutlineRoundness)]?.Value<float>() ?? defaultSettings.OutlineRoundness;
            BackgroundBlurConfig = ModelUtils.Unbox<BlurConfig>(node[nameof(BackgroundBlurConfig)]) ?? new BlurConfig();

            VectorConfig = ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)]);

            RainEnabled = node[nameof(RainEnabled)]?.Value<bool>() ?? defaultSettings.RainEnabled;
            Rain = ModelUtils.Unbox<RainConfig>(node[nameof(Rain)]);
        }
    }
}
