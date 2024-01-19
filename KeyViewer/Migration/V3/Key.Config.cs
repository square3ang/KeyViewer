using DG.Tweening;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

namespace KeyViewer.Migration.V3
{
    [XmlRoot("Config")]
    public class Key_Config
    {
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
        public bool ChangeRainColorJudge = false;
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
        [XmlIgnore]
        public bool RelativeOffsetApplied = false;
        [XmlIgnore]
        public float RelativeOffsetX = 0;
        [XmlIgnore]
        public float RelativeOffsetY = 0;
    }
}
