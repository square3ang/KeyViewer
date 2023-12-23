using UnityEngine;
using DG.Tweening;

namespace KeyViewer.Models
{
    public class KeyConfig
    {
        public uint Count = 0;
        public KeyCode Code = KeyCode.None;
        public SpecialKeyType SpecialKey = SpecialKeyType.None;
        public string Font = "Default";

        public PressRelease<string> Text = new PressRelease<string>(null);

        public PressRelease<float> TextSize = 75;
        public PressRelease<GColor> TextColor = new PressRelease<GColor>(Color.black, Color.white);
        public PressRelease<Vector2> TextOffset = Vector2.zero;
        public bool ChangeTextColorWithJudge = false;
        public Judge<GColor> TextJudgeColors = null;

        public PressRelease<float> CountTextSize = 50;
        public PressRelease<GColor> CountTextColor = new PressRelease<GColor>(Color.black, Color.white);
        public PressRelease<Vector2> CountTextOffset = Vector2.zero;
        public bool ChangeCountTextColorWithJudge = false;
        public Judge<GColor> CountTextJudgeColors = null;

        public string Background = null;
        public PressRelease<GColor> BackgroundColor = new PressRelease<GColor>(Color.white, Color.black);
        public bool ChangeBackgroundColorWithJudge = false;
        public Judge<GColor> BackgroundJudgeColors = null;

        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<Vector2> Scale = Vector2.one;

        public PressRelease<EaseConfig> ScaleEasing = new EaseConfig(Ease.OutExpo, 0.1f, 0.9f);

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

            newConfig.Background = Background;
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
    }
}
