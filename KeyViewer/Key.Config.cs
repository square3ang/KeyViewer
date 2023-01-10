using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Xml.Serialization;

namespace KeyViewer
{
    public partial class Key
    {
        public class Config
        {
            internal KeyManager keyManager;
            public Config() => Reset();
            public Config(KeyManager manager) : this() => keyManager = manager;
            public Config(KeyManager manager, KeyCode code) : this()
            {
                Code = code;
                Initialized = true;
                keyManager = manager;
            }
            public Config(KeyManager manager, SpecialKeyType type) : this()
            {
                SpecialType = type;
                Initialized = true;
                keyManager = manager;
            }
            private string font = "Default";
            private KeyCode code = KeyCode.None;
            private KeyCode spareCode = KeyCode.None;
            private SpecialKeyType specialType = SpecialKeyType.None;
            private uint count = 0;
            private bool gradient = false;
            private bool editing = false;
            private bool changeBgColorJudge = false;
            private float width = 100;
            private float height = 100;
            private float offsetX = 0;
            private float offsetY = 0;
            private float textOffsetX = 0;
            private float textOffsetY = 0;
            private float countTextOffsetX = 0;
            private float countTextOffsetY = 0;
            private float shrinkFactor = 0.9f;
            private float easeDuration = 0.1f;
            private float textFontSize = 75;
            private float countTextFontSize = 50;
            private Ease ease = Ease.OutExpo;
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
            public bool Initialized = false;
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
            [XmlIgnore]
            public LinkedList<Config> Backups = new LinkedList<Config>();
            [XmlIgnore]
            public LinkedListNode<Config> Current = null;
            internal bool CanUndo => Current?.Previous != null;
            internal bool CanRedo => Current?.Next != null;

            public string Font { get => font; set => font = value; }
            public KeyCode Code { get => code; set => code = value; }
            public SpecialKeyType SpecialType { get => specialType; set => specialType = value; }
            public uint Count { get => count; set => count = value; }
            public bool Editing { get => editing; set => editing = value; }
            public KeyCode SpareCode
            {
                get => spareCode;
                set
                {
                    if (spareCode != value)
                        Backup();
                    spareCode = value;
                }
            }
            public bool Gradient
            {
                get => gradient;
                set
                {
                    if (gradient != value)
                        Backup();
                    gradient = value;
                }
            }
            public bool ChangeBgColorJudge 
            { 
                get => changeBgColorJudge; 
                set 
                {
                    if (changeBgColorJudge != value)
                        Backup();
                    changeBgColorJudge = value;
                } 
            }
            public float Width 
            { 
                get => width; 
                set 
                {
                    if (width != value)
                        Backup();
                    width = value;
                } 
            }
            public float Height 
            { 
                get => height; 
                set
                {
                    if (height != value)
                        Backup();
                    height = value;
                }
            }
            public float OffsetX 
            { 
                get => offsetX; 
                set 
                {
                    if (offsetX != value)
                        Backup();
                    offsetX = value;
                } 
            }
            public float OffsetY 
            { 
                get => offsetY;
                set
                {
                    if (offsetY != value)
                        Backup();
                    offsetY = value;
                }
            }
            public float TextOffsetX 
            { 
                get => textOffsetX; 
                set 
                {
                    if (textOffsetX != value)
                        Backup();
                    textOffsetX = value;
                } 
            }
            public float TextOffsetY 
            { 
                get => textOffsetY; 
                set
                {
                    if (textOffsetY != value)
                        Backup();
                    textOffsetY = value;
                }
            }
            public float CountTextOffsetX 
            { 
                get => countTextOffsetX; 
                set 
                {
                    if (countTextOffsetX != value)
                        Backup();
                    countTextOffsetX = value;
                } 
            }
            public float CountTextOffsetY 
            { 
                get => countTextOffsetY; 
                set 
                {
                    if (countTextOffsetY != value)
                        Backup();
                    countTextOffsetY = value;
                }
            }
            public float ShrinkFactor 
            { 
                get => shrinkFactor; 
                set 
                {
                    if (shrinkFactor != value)
                        Backup();
                    shrinkFactor = value;
                } 
            }
            public float EaseDuration 
            { 
                get => easeDuration; 
                set 
                {
                    if (easeDuration != value)
                        Backup();
                    easeDuration = value;
                } 
            }
            public float TextFontSize 
            { 
                get => textFontSize; 
                set 
                {
                    if (textFontSize != value)
                        Backup();
                    textFontSize = value;
                } 
            }
            public float CountTextFontSize 
            { 
                get => countTextFontSize; 
                set
                {
                    if (countTextFontSize != value)
                        Backup();
                    countTextFontSize = value;
                }
            }
            public Ease Ease 
            { 
                get => ease;
                set 
                {
                    if (ease != value)
                        Backup();
                    ease = value;
                } 
            }
            public Color PressedOutlineColor
            {
                get => pressedOutlineColor;
                set
                {
                    if (pressedOutlineColor != value)
                        Backup();
                    pressedOutlineColor = value;
                    PressedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color ReleasedOutlineColor
            {
                get => releasedOutlineColor;
                set
                {
                    if (releasedOutlineColor != value)
                        Backup();
                    releasedOutlineColor = value;
                    ReleasedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color PressedBackgroundColor
            {
                get => pressedBackgroundColor;
                set
                {
                    if (pressedBackgroundColor != value)
                        Backup();
                    pressedBackgroundColor = value;
                    PressedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color ReleasedBackgroundColor
            {
                get => releasedBackgroundColor;
                set
                {
                    if (releasedBackgroundColor != value)
                        Backup();
                    releasedBackgroundColor = value;
                    ReleasedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public VertexGradient PressedTextColor
            {
                get => pressedTextColor;
                set
                {
                    if (pressedTextColor.Inequals(value))
                        Backup();
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
                    if (releasedTextColor.Inequals(value))
                        Backup();
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
                    if (pressedCountTextColor.Inequals(value))
                        Backup();
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
                    if (ReleasedCountTextColor.Inequals(value))
                        Backup();
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
                    if (tooEarlyColor != value)
                        Backup();
                    tooEarlyColor = value;
                    HitMarginColorHex[0] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color VeryEarlyColor
            {
                get => veryEarlyColor;
                set
                {
                    if (veryEarlyColor != value)
                        Backup();
                    veryEarlyColor = value;
                    HitMarginColorHex[1] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color EarlyPerfectColor
            {
                get => earlyPerfectColor;
                set
                {
                    if (earlyPerfectColor != value)
                        Backup();
                    earlyPerfectColor = value;
                    HitMarginColorHex[2] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color PerfectColor
            {
                get => perfectColor;
                set
                {
                    if (perfectColor != value)
                        Backup();
                    perfectColor = value;
                    HitMarginColorHex[3] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color LatePerfectColor
            {
                get => latePerfectColor;
                set
                {
                    if (latePerfectColor != value)
                        Backup();
                    latePerfectColor = value;
                    HitMarginColorHex[4] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color VeryLateColor
            {
                get => veryLateColor;
                set
                {
                    if (veryLateColor != value)
                        Backup();
                    veryLateColor = value;
                    HitMarginColorHex[5] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color TooLateColor
            {
                get => tooLateColor;
                set
                {
                    if (tooLateColor != value)
                        Backup();
                    tooLateColor = value;
                    HitMarginColorHex[6] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color MultipressColor
            {
                get => multipressColor;
                set
                {
                    if (multipressColor != value)
                        Backup();
                    multipressColor = value;
                    HitMarginColorHex[7] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }
            public Color FailMissColor
            {
                get => failMissColor;
                set
                {
                    if (failMissColor != value)
                        Backup();
                    failMissColor = value;
                    HitMarginColorHex[8] = ColorUtility.ToHtmlStringRGBA(value);
                    
                }
            }
            public Color FailOverloadColor
            {
                get => failOverloadColor;
                set
                {
                    if (failOverloadColor != value)
                        Backup();
                    failOverloadColor = value;
                    HitMarginColorHex[9] = ColorUtility.ToHtmlStringRGBA(value);
                }
            }

            public void Reset()
            {
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
            public void Backup()
            {
                if (!Initialized) return;
                if (keyManager.Profile.ConfigBackupsCount < Backups.Count)
                    Backups.RemoveFirst();
                if (Current != null)
                    Current = Backups.AddAfter(Current, Copy());
                else Current = Backups.AddLast(Copy());
            }
            public bool Undo()
            {
                var prev = Current.Previous;
                if (prev == null)
                    return false;
                Current = prev;
                ApplyConfigAll(prev.Value);
                return true;
            }
            public bool Redo()
            {
                var next = Current.Next;
                if (next == null)
                    return false;
                Current = next;
                ApplyConfigAll(next.Value);
                return true;
            }
            public void ApplyConfig(Config config)
            {
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
            public void ApplyConfigAll(Config config)
            {
                keyManager = config.keyManager;
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
            public Config Copy()
            {
                Config conf = new Config();
                conf.keyManager = keyManager;
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
}
