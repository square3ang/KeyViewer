using DG.Tweening;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KeyViewer.Models;

public class KeyConfig : IModel, ICopyable<KeyConfig> {
    public int Count = 0;
    public KeyCode Code = KeyCode.None;
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

    public PressReleaseBase<string> Text = new(null);
    public PressReleaseBase<string> CountText = new(null);
    public PressReleaseBase<string> Background = new(null);
    public PressReleaseBase<string> Outline = new(null);

    public ObjectConfig TextConfig = new(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.black, Color.white);
    public ObjectConfig CountTextConfig = new(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.black, Color.white);
    public ObjectConfig BackgroundConfig = new(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.white, Color.black.WithAlpha(0.4f));
    public ObjectConfig OutlineConfig = new(new PressRelease<Vector2>(new Vector2(0.9f, 0.9f), Vector2.one).SetEase(new EaseConfig(Ease.OutQuad, 0.1f)), Color.white, Color.white);
    public float BackgroundRoundness = 0f;
    public float OutlineRoundness = 0f;
    public BlurConfig BackgroundBlurConfig = new();

    public VectorConfig VectorConfig = new();

    public bool RainEnabled = false;
    public RainConfig Rain = new();

    public KeyConfig Copy() {
        KeyConfig newConfig = new() {
            Count = Count,
            Code = Code,
            //newConfig.Codes = (KeyCode[])Codes.Clone();
            DummyName = DummyName,
            Font = Font,
            EnableKPSMeter = EnableKPSMeter,
            UpdateTextAlways = UpdateTextAlways,
            EnableCountText = EnableCountText,
            EnableOutlineImage = EnableOutlineImage,
            DisableSorting = DisableSorting,
            DoNotScaleText = DoNotScaleText,
            BackgroundBlurEnabled = BackgroundBlurEnabled,
            TextFontSize = TextFontSize,
            CountTextFontSize = CountTextFontSize,

            Text = Text.Copy(),
            CountText = CountText.Copy(),
            Background = Background.Copy(),
            Outline = Outline.Copy(),

            TextConfig = TextConfig.Copy(),
            CountTextConfig = CountTextConfig.Copy(),
            BackgroundConfig = BackgroundConfig.Copy(),
            OutlineConfig = OutlineConfig.Copy(),
            BackgroundRoundness = BackgroundRoundness,
            OutlineRoundness = OutlineRoundness,
            BackgroundBlurConfig = BackgroundBlurConfig.Copy(),

            VectorConfig = VectorConfig.Copy(),

            RainEnabled = RainEnabled,
            Rain = Rain.Copy()
        };
        return newConfig;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Count)] = Count,
            [nameof(Code)] = Code.ToString()
        };
        if(!string.IsNullOrEmpty(DummyName)) {
            node[nameof(DummyName)] = DummyName;
        }
        if(Font != "Default") {
            node[nameof(Font)] = Font;
        }
        if(EnableKPSMeter) {
            node[nameof(EnableKPSMeter)] = EnableKPSMeter;
        }
        if(UpdateTextAlways) {
            node[nameof(UpdateTextAlways)] = UpdateTextAlways;
        }
        if(EnableCountText) {
            node[nameof(EnableCountText)] = EnableCountText;
        }
        if(EnableOutlineImage) {
            node[nameof(EnableOutlineImage)] = EnableOutlineImage;
        }
        if(DisableSorting) {
            node[nameof(DisableSorting)] = DisableSorting;
        }
        if(DoNotScaleText) {
            node[nameof(DoNotScaleText)] = DoNotScaleText;
        }
        if(BackgroundBlurEnabled) {
            node[nameof(BackgroundBlurEnabled)] = BackgroundBlurEnabled;
        }
        node[nameof(TextFontSize)] = TextFontSize;
        node[nameof(CountTextFontSize)] = CountTextFontSize;

        var text = Text.Serialize();
        if(!string.IsNullOrEmpty(text.ToString())) {
            node[nameof(Text)] = Text.Serialize();
        }
        var countText = CountText.Serialize();
        if(!string.IsNullOrEmpty(countText.ToString())) {
            node[nameof(CountText)] = CountText.Serialize();
        }
        var backGround = Background.Serialize();
        if(!string.IsNullOrEmpty(backGround.ToString())) {
            node[nameof(Background)] = Background.Serialize();
        }
        var outline = Outline.Serialize();
        if(!string.IsNullOrEmpty(outline.ToString())) {
            node[nameof(Outline)] = Outline.Serialize();
        }

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
    public void Deserialize(JToken node) {
        var defaultSettings = new KeyConfig();

        Count = node[nameof(Count)]?.Value<int>() ?? defaultSettings.Count;
        Code = EnumHelper<KeyCode>.Parse(node[nameof(Code)]?.Value<string>() ?? defaultSettings.Code.ToString());
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
