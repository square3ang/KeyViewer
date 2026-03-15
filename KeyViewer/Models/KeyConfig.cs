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

        ModelUtils.PutIfNotEmpty(node, nameof(Text), Text.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(CountText), CountText.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(Background), Background.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(Outline), Outline.Serialize());

        ModelUtils.PutIfNotEmpty(node, nameof(TextConfig), TextConfig.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(CountTextConfig), CountTextConfig.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(BackgroundConfig), BackgroundConfig.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(OutlineConfig), OutlineConfig.Serialize());

        node[nameof(BackgroundRoundness)] = BackgroundRoundness;
        node[nameof(OutlineRoundness)] = OutlineRoundness;

        ModelUtils.PutIfNotEmpty(node, nameof(BackgroundBlurConfig), BackgroundBlurConfig.Serialize());
        ModelUtils.PutIfNotEmpty(node, nameof(VectorConfig), VectorConfig.Serialize());

        node[nameof(RainEnabled)] = RainEnabled;
        ModelUtils.PutIfNotEmpty(node, nameof(Rain), Rain.Serialize());

        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new KeyConfig();

        Count = node?[nameof(Count)]?.Value<int>() ?? defaultSettings.Count;
        Code = EnumHelper<KeyCode>.Parse(node?[nameof(Code)]?.Value<string>() ?? defaultSettings.Code.ToString());
        DummyName = node?[nameof(DummyName)]?.Value<string>() ?? defaultSettings.DummyName;
        Font = node?[nameof(Font)]?.Value<string>() ?? defaultSettings.Font;

        EnableKPSMeter = node?[nameof(EnableKPSMeter)]?.Value<bool>() ?? defaultSettings.EnableKPSMeter;
        UpdateTextAlways = node?[nameof(UpdateTextAlways)]?.Value<bool>() ?? defaultSettings.UpdateTextAlways;
        EnableCountText = node?[nameof(EnableCountText)]?.Value<bool>() ?? defaultSettings.EnableCountText;
        EnableOutlineImage = node?[nameof(EnableOutlineImage)]?.Value<bool>() ?? defaultSettings.EnableOutlineImage;
        DisableSorting = node?[nameof(DisableSorting)]?.Value<bool>() ?? defaultSettings.DisableSorting;
        DoNotScaleText = node?[nameof(DoNotScaleText)]?.Value<bool>() ?? defaultSettings.DoNotScaleText;
        BackgroundBlurEnabled = node?[nameof(BackgroundBlurEnabled)]?.Value<bool>() ?? defaultSettings.BackgroundBlurEnabled;

        TextFontSize = node?[nameof(TextFontSize)]?.Value<float>() ?? defaultSettings.TextFontSize;
        CountTextFontSize = node?[nameof(CountTextFontSize)]?.Value<float>() ?? defaultSettings.CountTextFontSize;

        Text = node?[nameof(Text)] != null
            ? ModelUtils.Unbox<PressRelease<string>>(node[nameof(Text)])
            : defaultSettings.Text;

        CountText = node?[nameof(CountText)] != null
            ? ModelUtils.Unbox<PressRelease<string>>(node[nameof(CountText)])
            : defaultSettings.CountText;

        Background = node?[nameof(Background)] != null
            ? ModelUtils.Unbox<PressRelease<string>>(node[nameof(Background)])
            : defaultSettings.Background;

        Outline = node?[nameof(Outline)] != null
            ? ModelUtils.Unbox<PressRelease<string>>(node[nameof(Outline)])
            : defaultSettings.Outline;

        TextConfig = node?[nameof(TextConfig)] != null
            ? ModelUtils.Unbox<ObjectConfig>(node[nameof(TextConfig)])
            : defaultSettings.TextConfig;

        CountTextConfig = node?[nameof(CountTextConfig)] != null
            ? ModelUtils.Unbox<ObjectConfig>(node[nameof(CountTextConfig)])
            : defaultSettings.CountTextConfig;

        BackgroundConfig = node?[nameof(BackgroundConfig)] != null
            ? ModelUtils.Unbox<ObjectConfig>(node[nameof(BackgroundConfig)])
            : defaultSettings.BackgroundConfig;

        OutlineConfig = node?[nameof(OutlineConfig)] != null
            ? ModelUtils.Unbox<ObjectConfig>(node[nameof(OutlineConfig)])
            : defaultSettings.OutlineConfig;

        BackgroundRoundness = node?[nameof(BackgroundRoundness)]?.Value<float>() ?? defaultSettings.BackgroundRoundness;
        OutlineRoundness = node?[nameof(OutlineRoundness)]?.Value<float>() ?? defaultSettings.OutlineRoundness;

        BackgroundBlurConfig = node?[nameof(BackgroundBlurConfig)] != null
            ? ModelUtils.Unbox<BlurConfig>(node[nameof(BackgroundBlurConfig)])
            : new BlurConfig();

        VectorConfig = node?[nameof(VectorConfig)] != null
            ? ModelUtils.Unbox<VectorConfig>(node[nameof(VectorConfig)])
            : defaultSettings.VectorConfig;

        RainEnabled = node?[nameof(RainEnabled)]?.Value<bool>() ?? defaultSettings.RainEnabled;

        Rain = node?[nameof(Rain)] != null
            ? ModelUtils.Unbox<RainConfig>(node[nameof(Rain)])
            : defaultSettings.Rain;
    }
}
