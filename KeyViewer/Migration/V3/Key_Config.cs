using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Xml.Serialization;
using KeyViewer.Models;

namespace KeyViewer.Migration.V3
{
    public class Key_Config
    {
        public Key_Config() => Reset();
        public Key_Config(KeyCode code) : this()
        {
            Code = code;
        }
        public Key_Config(SpecialKeyType type) : this()
        {
            SpecialType = type;
        }
        public KeyRain_Config RainConfig = new KeyRain_Config();
        public bool RainEnabled = false;
        public string Font = "Default";
        public KeyCode Code = KeyCode.None;
        public KeyCode SpareCode = KeyCode.None;
        public SpecialKeyType SpecialType = SpecialKeyType.None;
        public uint Count = 0;
        public bool Gradient = false;
        public bool Editing = false;
        public string KeyTitle = null;
        public bool ChangeBgColorJudge = false;
        public float Width = 100;
        public float Height = 100;
        [XmlIgnore]
        public float offsetX = 0;
        [XmlIgnore]
        public float offsetY = 0;
        public float TextOffsetX = 0;
        public float TextOffsetY = 0;
        public float CountTextOffsetX = 0;
        public float CountTextOffsetY = 0;
        public float ShrinkFactor = 0.9f;
        public float EaseDuration = 0.1f;
        public float TextFontSize = 75;
        public float CountTextFontSize = 50;
        public Ease Ease = Ease.OutExpo;
        private Color pressedOutlineColor = Color.white;
        private Color releasedOutlineColor = Color.white;
        private Color pressedBackgroundColor = Color.white;
        private Color releasedBackgroundColor = Color.black.WithAlpha(0.4f);
        private Color tooEarlyColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
        private Color veryEarlyColor = new Color(1.000f, 0.436f, 0.306f, 1.000f);
        private Color earlyPerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
        private Color perfectColor = new Color(0.376f, 1.000f, 0.307f, 1.000f);
        private Color latePerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
        private Color veryLateColor = new Color(1.000f, 0.435f, 0.306f, 1.000f);
        private Color tooLateColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
        private Color multipressColor = new Color(0.000f, 1.000f, 0.930f, 1.000f);
        private Color failMissColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
        private Color failOverloadColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
        private VertexGradient pressedTextColor = new VertexGradient(Color.black);
        private VertexGradient releasedTextColor = new VertexGradient(Color.white);
        private VertexGradient pressedCountTextColor = new VertexGradient(Color.black);
        private VertexGradient releasedCountTextColor = new VertexGradient(Color.white);

        [XmlIgnore]
        public string PressedOutlineColorHex;
        [XmlIgnore]
        public string ReleasedOutlineColorHex;
        [XmlIgnore]
        public string PressedBackgroundColorHex;
        [XmlIgnore]
        public string ReleasedBackgroundColorHex;
        [XmlIgnore]
        public string[] PressedTextColorHex = new string[4];
        [XmlIgnore]
        public string[] ReleasedTextColorHex = new string[4];
        [XmlIgnore]
        public string[] PressedCountTextColorHex = new string[4];
        [XmlIgnore]
        public string[] ReleasedCountTextColorHex = new string[4];
        [XmlIgnore]
        public string[] HitMarginColorHex = new string[10];

        public float OffsetX
        {
            get
            {
                if (RelativeOffsetApplied)
                    return offsetX + RelativeOffsetX;
                return offsetX;
            }
            set => offsetX = value;
        }
        public float OffsetY
        {
            get
            {
                if (RelativeOffsetApplied)
                    return offsetY + RelativeOffsetY;
                return offsetY;
            }
            set => offsetY = value;
        }
        public Color PressedOutlineColor
        {
            get => pressedOutlineColor;
            set
            {
                pressedOutlineColor = value;
                PressedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color ReleasedOutlineColor
        {
            get => releasedOutlineColor;
            set
            {
                releasedOutlineColor = value;
                ReleasedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color PressedBackgroundColor
        {
            get => pressedBackgroundColor;
            set
            {
                pressedBackgroundColor = value;
                PressedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color ReleasedBackgroundColor
        {
            get => releasedBackgroundColor;
            set
            {
                releasedBackgroundColor = value;
                ReleasedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public VertexGradient PressedTextColor
        {
            get => pressedTextColor;
            set
            {
                pressedTextColor = value;
                PressedTextColorHex[0] = ColorUtility.ToHtmlStringRGBA(value.topLeft);
                PressedTextColorHex[1] = ColorUtility.ToHtmlStringRGBA(value.topRight);
                PressedTextColorHex[2] = ColorUtility.ToHtmlStringRGBA(value.bottomLeft);
                PressedTextColorHex[3] = ColorUtility.ToHtmlStringRGBA(value.bottomRight);
            }
        }
        public VertexGradient ReleasedTextColor
        {
            get => releasedTextColor;
            set
            {
                releasedTextColor = value;
                ReleasedTextColorHex[0] = ColorUtility.ToHtmlStringRGBA(value.topLeft);
                ReleasedTextColorHex[1] = ColorUtility.ToHtmlStringRGBA(value.topRight);
                ReleasedTextColorHex[2] = ColorUtility.ToHtmlStringRGBA(value.bottomLeft);
                ReleasedTextColorHex[3] = ColorUtility.ToHtmlStringRGBA(value.bottomRight);
            }
        }
        public VertexGradient PressedCountTextColor
        {
            get => pressedCountTextColor;
            set
            {
                pressedCountTextColor = value;
                PressedCountTextColorHex[0] = ColorUtility.ToHtmlStringRGBA(value.topLeft);
                PressedCountTextColorHex[1] = ColorUtility.ToHtmlStringRGBA(value.topRight);
                PressedCountTextColorHex[2] = ColorUtility.ToHtmlStringRGBA(value.bottomLeft);
                PressedCountTextColorHex[3] = ColorUtility.ToHtmlStringRGBA(value.bottomRight);
            }
        }
        public VertexGradient ReleasedCountTextColor
        {
            get => releasedCountTextColor;
            set
            {
                releasedCountTextColor = value;
                ReleasedCountTextColorHex[0] = ColorUtility.ToHtmlStringRGBA(value.topLeft);
                ReleasedCountTextColorHex[1] = ColorUtility.ToHtmlStringRGBA(value.topRight);
                ReleasedCountTextColorHex[2] = ColorUtility.ToHtmlStringRGBA(value.bottomLeft);
                ReleasedCountTextColorHex[3] = ColorUtility.ToHtmlStringRGBA(value.bottomRight);
            }
        }
        public Color TooEarlyColor
        {
            get => tooEarlyColor;
            set
            {
                tooEarlyColor = value;
                HitMarginColorHex[0] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color VeryEarlyColor
        {
            get => veryEarlyColor;
            set
            {
                veryEarlyColor = value;
                HitMarginColorHex[1] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color EarlyPerfectColor
        {
            get => earlyPerfectColor;
            set
            {
                earlyPerfectColor = value;
                HitMarginColorHex[2] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color PerfectColor
        {
            get => perfectColor;
            set
            {
                perfectColor = value;
                HitMarginColorHex[3] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color LatePerfectColor
        {
            get => latePerfectColor;
            set
            {
                latePerfectColor = value;
                HitMarginColorHex[4] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color VeryLateColor
        {
            get => veryLateColor;
            set
            {
                veryLateColor = value;
                HitMarginColorHex[5] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color TooLateColor
        {
            get => tooLateColor;
            set
            {
                tooLateColor = value;
                HitMarginColorHex[6] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color MultipressColor
        {
            get => multipressColor;
            set
            {
                multipressColor = value;
                HitMarginColorHex[7] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }
        public Color FailMissColor
        {
            get => failMissColor;
            set
            {
                failMissColor = value;
                HitMarginColorHex[8] = ColorUtility.ToHtmlStringRGBA(value);

            }
        }
        public Color FailOverloadColor
        {
            get => failOverloadColor;
            set
            {
                failOverloadColor = value;
                HitMarginColorHex[9] = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        public void Reset()
        {
            RainEnabled = false;
            RainConfig = new KeyRain_Config();
            Width = 100;
            Height = 100;
            OffsetX = 0;
            OffsetY = 0;
            ShrinkFactor = 0.9f;
            EaseDuration = 0.1f;
            Ease = Ease.OutExpo;
            SpareCode = Code;
            Count = 0;
            TextFontSize = 75;
            CountTextFontSize = 50;
            TextOffsetX = 0;
            TextOffsetY = 0;
            CountTextOffsetX = 0;
            CountTextOffsetY = 0;
            PressedOutlineColor = Color.white;
            ReleasedOutlineColor = Color.white;
            PressedBackgroundColor = Color.white;
            ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
            PressedTextColor = new VertexGradient(Color.black);
            ReleasedTextColor = new VertexGradient(Color.white);
            PressedCountTextColor = new VertexGradient(Color.black);
            ReleasedCountTextColor = new VertexGradient(Color.white);
            ChangeBgColorJudge = false;
            TooEarlyColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
            VeryEarlyColor = new Color(1.000f, 0.436f, 0.306f, 1.000f);
            EarlyPerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
            PerfectColor = new Color(0.376f, 1.000f, 0.307f, 1.000f);
            LatePerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f);
            VeryLateColor = new Color(1.000f, 0.435f, 0.306f, 1.000f);
            TooLateColor = new Color(1.000f, 0.000f, 0.000f, 1.000f);
            MultipressColor = new Color(0.000f, 1.000f, 0.930f, 1.000f);
            FailMissColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
            FailOverloadColor = new Color(0.851f, 0.346f, 1.000f, 1.000f);
        }
        public void ApplyConfig(Key_Config config)
        {
            RainEnabled = config.RainEnabled;
            RainConfig = config.RainConfig.Copy();
            Font = config.Font;
            Width = config.Width;
            Height = config.Height;
            OffsetX = config.OffsetX;
            OffsetY = config.OffsetY;
            ShrinkFactor = config.ShrinkFactor;
            EaseDuration = config.EaseDuration;
            Ease = config.Ease;
            TextOffsetX = config.TextOffsetX;
            TextOffsetY = config.TextOffsetY;
            CountTextOffsetX = config.CountTextOffsetX;
            CountTextOffsetY = config.CountTextOffsetY;
            TextFontSize = config.TextFontSize;
            CountTextFontSize = config.CountTextFontSize;
            PressedOutlineColor = config.PressedOutlineColor;
            ReleasedOutlineColor = config.ReleasedOutlineColor;
            PressedBackgroundColor = config.PressedBackgroundColor;
            ReleasedBackgroundColor = config.ReleasedBackgroundColor;
            PressedTextColor = config.PressedTextColor;
            ReleasedTextColor = config.ReleasedTextColor;
            PressedCountTextColor = config.PressedCountTextColor;
            ReleasedCountTextColor = config.ReleasedCountTextColor;
            ChangeBgColorJudge = config.ChangeBgColorJudge;
            TooEarlyColor = config.TooEarlyColor;
            VeryEarlyColor = config.VeryEarlyColor;
            EarlyPerfectColor = config.EarlyPerfectColor;
            PerfectColor = config.PerfectColor;
            LatePerfectColor = config.LatePerfectColor;
            VeryLateColor = config.VeryLateColor;
            TooLateColor = config.TooLateColor;
            MultipressColor = config.MultipressColor;
            FailMissColor = config.FailMissColor;
            FailOverloadColor = config.FailOverloadColor;
        }
        [XmlIgnore]
        public bool RelativeOffsetApplied = false;
        [XmlIgnore]
        public float RelativeOffsetX = 0;
        [XmlIgnore]
        public float RelativeOffsetY = 0;
        public void ApplyOffsetRelative(Key_Config config)
        {
            RelativeOffsetApplied = true;
            RelativeOffsetX = config.OffsetX;
            RelativeOffsetY = config.OffsetY;
        }
        public void ApplyConfigWithoutOffset(Key_Config config)
        {
            RainEnabled = config.RainEnabled;
            RainConfig = config.RainConfig.Copy();
            Font = config.Font;
            Width = config.Width;
            Height = config.Height;
            ShrinkFactor = config.ShrinkFactor;
            EaseDuration = config.EaseDuration;
            Ease = config.Ease;
            TextOffsetX = config.TextOffsetX;
            TextOffsetY = config.TextOffsetY;
            CountTextOffsetX = config.CountTextOffsetX;
            CountTextOffsetY = config.CountTextOffsetY;
            TextFontSize = config.TextFontSize;
            CountTextFontSize = config.CountTextFontSize;
            PressedOutlineColor = config.PressedOutlineColor;
            ReleasedOutlineColor = config.ReleasedOutlineColor;
            PressedBackgroundColor = config.PressedBackgroundColor;
            ReleasedBackgroundColor = config.ReleasedBackgroundColor;
            PressedTextColor = config.PressedTextColor;
            ReleasedTextColor = config.ReleasedTextColor;
            PressedCountTextColor = config.PressedCountTextColor;
            ReleasedCountTextColor = config.ReleasedCountTextColor;
            ChangeBgColorJudge = config.ChangeBgColorJudge;
            TooEarlyColor = config.TooEarlyColor;
            VeryEarlyColor = config.VeryEarlyColor;
            EarlyPerfectColor = config.EarlyPerfectColor;
            PerfectColor = config.PerfectColor;
            LatePerfectColor = config.LatePerfectColor;
            VeryLateColor = config.VeryLateColor;
            TooLateColor = config.TooLateColor;
            MultipressColor = config.MultipressColor;
            FailMissColor = config.FailMissColor;
            FailOverloadColor = config.FailOverloadColor;
        }
        public void ApplyConfigAll(Key_Config config)
        {
            RainEnabled = config.RainEnabled;
            RainConfig = config.RainConfig.Copy();
            Font = config.Font;
            Code = config.Code;
            SpecialType = config.SpecialType;
            Width = config.Width;
            Height = config.Height;
            OffsetX = config.OffsetX;
            OffsetY = config.OffsetY;
            ShrinkFactor = config.ShrinkFactor;
            EaseDuration = config.EaseDuration;
            Ease = config.Ease;
            SpareCode = config.SpareCode;
            Count = config.Count;
            TextFontSize = config.TextFontSize;
            CountTextFontSize = config.CountTextFontSize;
            TextOffsetX = config.TextOffsetX;
            TextOffsetY = config.TextOffsetY;
            CountTextOffsetX = config.CountTextOffsetX;
            CountTextOffsetY = config.CountTextOffsetY;
            PressedOutlineColor = config.PressedOutlineColor;
            ReleasedOutlineColor = config.ReleasedOutlineColor;
            PressedBackgroundColor = config.PressedBackgroundColor;
            ReleasedBackgroundColor = config.ReleasedBackgroundColor;
            PressedTextColor = config.PressedTextColor;
            ReleasedTextColor = config.ReleasedTextColor;
            PressedCountTextColor = config.PressedCountTextColor;
            ReleasedCountTextColor = config.ReleasedCountTextColor;
            ChangeBgColorJudge = config.ChangeBgColorJudge;
            TooEarlyColor = config.TooEarlyColor;
            VeryEarlyColor = config.VeryEarlyColor;
            EarlyPerfectColor = config.EarlyPerfectColor;
            PerfectColor = config.PerfectColor;
            LatePerfectColor = config.LatePerfectColor;
            VeryLateColor = config.VeryLateColor;
            TooLateColor = config.TooLateColor;
            MultipressColor = config.MultipressColor;
            FailMissColor = config.FailMissColor;
            FailOverloadColor = config.FailOverloadColor;
        }
        public Key_Config Copy()
        {
            Key_Config conf = new Key_Config();
            conf.RainEnabled = RainEnabled;
            conf.RainConfig = RainConfig.Copy();
            conf.Font = Font;
            conf.Code = Code;
            conf.SpecialType = SpecialType;
            conf.Width = Width;
            conf.Height = Height;
            conf.OffsetX = OffsetX;
            conf.OffsetY = OffsetY;
            conf.ShrinkFactor = ShrinkFactor;
            conf.EaseDuration = EaseDuration;
            conf.Ease = Ease;
            conf.SpareCode = SpareCode;
            conf.Count = Count;
            conf.TextFontSize = TextFontSize;
            conf.CountTextFontSize = CountTextFontSize;
            conf.TextOffsetX = TextOffsetX;
            conf.TextOffsetY = TextOffsetY;
            conf.CountTextOffsetX = CountTextOffsetX;
            conf.CountTextOffsetY = CountTextOffsetY;
            conf.PressedOutlineColor = PressedOutlineColor;
            conf.ReleasedOutlineColor = ReleasedOutlineColor;
            conf.PressedBackgroundColor = PressedBackgroundColor;
            conf.ReleasedBackgroundColor = ReleasedBackgroundColor;
            conf.PressedTextColor = PressedTextColor;
            conf.ReleasedTextColor = ReleasedTextColor;
            conf.PressedCountTextColor = PressedCountTextColor;
            conf.ReleasedCountTextColor = ReleasedCountTextColor;
            conf.ChangeBgColorJudge = ChangeBgColorJudge;
            conf.TooEarlyColor = TooEarlyColor;
            conf.VeryEarlyColor = VeryEarlyColor;
            conf.EarlyPerfectColor = EarlyPerfectColor;
            conf.PerfectColor = PerfectColor;
            conf.LatePerfectColor = LatePerfectColor;
            conf.VeryLateColor = VeryLateColor;
            conf.TooLateColor = TooLateColor;
            conf.MultipressColor = MultipressColor;
            conf.FailMissColor = FailMissColor;
            conf.FailOverloadColor = FailOverloadColor;
            return conf;
        }
    }
}
