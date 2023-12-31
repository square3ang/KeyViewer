using UnityEngine;
using DG.Tweening;
using KeyViewer.Core.Interfaces;
using JSON;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class KeyConfig : IModel
    {
        public uint Count = 0;
        public KeyCode Code = KeyCode.None;
        public SpecialKeyType SpecialKey = SpecialKeyType.None;
        public string Font = "Default";

        public PressRelease<string> Text = new PressRelease<string>(null);

        public PressRelease<float> TextSize = 75;
        public PressReleaseM<GColor> TextColor = new PressReleaseM<GColor>(Color.black, Color.white);
        public PressRelease<Vector2> TextOffset = Vector2.zero;
        public bool ChangeTextColorWithJudge = false;
        public JudgeM<GColor> TextJudgeColors = null;

        public PressRelease<float> CountTextSize = 50;
        public PressReleaseM<GColor> CountTextColor = new PressReleaseM<GColor>(Color.black, Color.white);
        public PressRelease<Vector2> CountTextOffset = Vector2.zero;
        public bool ChangeCountTextColorWithJudge = false;
        public JudgeM<GColor> CountTextJudgeColors = null;

        public PressRelease<string> Background = new PressRelease<string>(null);
        public PressReleaseM<GColor> BackgroundColor = new PressReleaseM<GColor>(Color.white, Color.black);
        public bool ChangeBackgroundColorWithJudge = false;
        public JudgeM<GColor> BackgroundJudgeColors = null;

        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<Vector2> Scale = Vector2.one;

        public PressReleaseM<EaseConfig> ScaleEasing = new EaseConfig(Ease.OutExpo, 0.1f, 0.9f);

        public bool RainEnabled = false;
        public RainConfig Rain = new RainConfig();

        public KeyConfig Copy()
        {
            KeyConfig newConfig = new KeyConfig();

            newConfig.Text = Text.Copy();

            newConfig.TextSize = TextSize.Copy();
            newConfig.TextColor = TextColor.Copy();
            newConfig.TextOffset = TextOffset.Copy();
            newConfig.ChangeTextColorWithJudge = ChangeTextColorWithJudge;
            newConfig.TextJudgeColors = TextJudgeColors?.Copy();

            newConfig.CountTextSize = CountTextSize.Copy();
            newConfig.CountTextColor = CountTextColor.Copy();
            newConfig.CountTextOffset = CountTextOffset.Copy();
            newConfig.ChangeCountTextColorWithJudge = ChangeCountTextColorWithJudge;
            newConfig.CountTextJudgeColors = CountTextJudgeColors?.Copy();

            newConfig.Background = Background.Copy();
            newConfig.BackgroundColor = BackgroundColor.Copy();
            newConfig.ChangeBackgroundColorWithJudge = ChangeBackgroundColorWithJudge;
            newConfig.BackgroundJudgeColors = BackgroundJudgeColors?.Copy();

            newConfig.Offset = Offset.Copy();
            newConfig.Scale = Scale.Copy();

            newConfig.ScaleEasing = ScaleEasing.Copy();

            newConfig.RainEnabled = RainEnabled;
            newConfig.Rain = Rain.Copy();

            return newConfig;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;

            node[nameof(Text)] = Text.Serialize();

            node[nameof(TextSize)] = TextSize.Serialize();
            node[nameof(TextColor)] = TextColor.Serialize();
            node[nameof(TextOffset)] = TextOffset.Serialize();
            node[nameof(ChangeTextColorWithJudge)] = ChangeTextColorWithJudge;
            node[nameof(TextJudgeColors)] = TextJudgeColors?.Serialize();

            node[nameof(CountTextSize)] = CountTextSize.Serialize();
            node[nameof(CountTextColor)] = CountTextColor.Serialize();
            node[nameof(CountTextOffset)] = CountTextOffset.Serialize();
            node[nameof(ChangeCountTextColorWithJudge)] = ChangeCountTextColorWithJudge;
            node[nameof(CountTextJudgeColors)] = CountTextJudgeColors?.Serialize();

            node[nameof(Background)] = Background.Serialize();
            node[nameof(BackgroundColor)] = BackgroundColor.Serialize();
            node[nameof(ChangeBackgroundColorWithJudge)] = ChangeBackgroundColorWithJudge;
            node[nameof(BackgroundJudgeColors)] = BackgroundJudgeColors?.Serialize();

            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Scale)] = Scale.Serialize();

            node[nameof(ScaleEasing)] = ScaleEasing.Serialize();

            node[nameof(RainEnabled)] = RainEnabled;
            node[nameof(Rain)] = Rain.Serialize();

            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Text = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Text)]);

            TextSize = ModelUtils.Unbox<PressRelease<float>>(node[nameof(TextSize)]);
            TextColor = ModelUtils.Unbox<PressReleaseM<GColor>>(node[nameof(TextColor)]);
            TextOffset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(TextOffset)]);
            ChangeTextColorWithJudge = node[nameof(ChangeTextColorWithJudge)];
            TextJudgeColors = ModelUtils.Unbox<JudgeM<GColor>>(node[nameof(TextJudgeColors)]);

            CountTextSize = ModelUtils.Unbox<PressRelease<float>>(node[nameof(CountTextSize)]);
            CountTextColor = ModelUtils.Unbox<PressReleaseM<GColor>>(node[nameof(CountTextColor)]);
            CountTextOffset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(CountTextOffset)]);
            ChangeCountTextColorWithJudge = node[nameof(ChangeCountTextColorWithJudge)];
            CountTextJudgeColors = ModelUtils.Unbox<JudgeM<GColor>>(node[nameof(CountTextJudgeColors)]);

            Background = ModelUtils.Unbox<PressRelease<string>>(node[nameof(Background)]);
            BackgroundColor = ModelUtils.Unbox<PressReleaseM<GColor>>(node[nameof(BackgroundColor)]);
            ChangeBackgroundColorWithJudge = node[nameof(ChangeBackgroundColorWithJudge)];
            BackgroundJudgeColors = ModelUtils.Unbox<JudgeM<GColor>>(node[nameof(BackgroundJudgeColors)]);

            Offset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Scale)]);

            ScaleEasing = ModelUtils.Unbox<PressReleaseM<EaseConfig>>(node[nameof(ScaleEasing)]);

            RainEnabled = node[nameof(RainEnabled)];
            Rain = ModelUtils.Unbox<RainConfig>(node[nameof(Rain)]);
        }
    }
}
