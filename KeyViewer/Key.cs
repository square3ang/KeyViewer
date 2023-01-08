using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System.Threading;
using KeyViewer.API;

namespace KeyViewer
{
    public partial class Key : MonoBehaviour
    {
        public static uint Total;
        static LangManager lang => Main.Lang;
        public uint Count
        {
            get => config.Count;
            set => config.Count = value;
        }
        public KeyCode Code => config.Code;
        public SpecialKeyType SpecialType => config.SpecialType;
        public bool Pressed { get; private set; }
        public Image Outline { get; private set; }
        public string TweenID { get; private set; }
        public Image Background { get; private set; }
        public TextMeshProUGUI Text { get; private set; }
        public TextMeshProUGUI CountText { get; private set; }
        private bool initialized;
        private bool isSpecial;
        private bool prevPressed;
        private Vector2 position;
        private Vector2 offsetVec => new Vector2(config.OffsetX, config.OffsetY);
        private KeyManager keyManager;
        internal Config config;
        private Profile Profile => keyManager.Profile;
        public Key Init(KeyManager keyManager, Config config)
        {
            this.config = config ?? (config = new Config());
            Count = this.config.Count;
            this.keyManager = keyManager;
            transform.SetParent(keyManager.keysCanvas.transform);

            isSpecial = config.SpecialType != SpecialKeyType.None;

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            Background = bgObj.AddComponent<Image>();
            Background.sprite = Main.KeyBackground;
            Background.color = config.ReleasedBackgroundColor;
            Background.type = Image.Type.Sliced;

            GameObject olObj = new GameObject("Outline");
            olObj.transform.SetParent(transform);
            Outline = olObj.AddComponent<Image>();
            Outline.sprite = Main.KeyOutline;
            Outline.color = config.ReleasedOutlineColor;
            Outline.type = Image.Type.Sliced;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            ContentSizeFitter textCsf = textObj.AddComponent<ContentSizeFitter>();
            textCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text = textObj.AddComponent<TextMeshProUGUI>();
            Text.font = FontManager.GetFont(config.Font).fontTMP;
            Text.color = Color.white;
            Text.enableVertexGradient = true;
            Text.alignment = TextAlignmentOptions.Midline;
            if (isSpecial)
                Text.text = SpecialType.ToString();
            else
            {
                if (!KeyString.TryGetValue(Code, out string codeString))
                    codeString = Code.ToString();
                Text.text = codeString;
            }

            GameObject countTextObj = new GameObject("CountText");
            countTextObj.transform.SetParent(transform);
            ContentSizeFitter countTextCsf = countTextObj.AddComponent<ContentSizeFitter>();
            countTextCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            countTextCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            CountText = countTextObj.AddComponent<TextMeshProUGUI>();
            CountText.font = FontManager.GetFont(config.Font).fontTMP;
            CountText.color = Color.white;
            CountText.enableVertexGradient = true;
            CountText.alignment = TextAlignmentOptions.Midline;

            switch (SpecialType)
            {
                case SpecialKeyType.None:
                    CountText.text = config.Count.ToString();
                    break;
                case SpecialKeyType.KPS:
                    CountText.text = Profile.calculator.Kps.ToString();
                    break;
                case SpecialKeyType.Total:
                    CountText.text = (Total = Profile.ActiveKeys.Select(c => c.Count).Sum()).ToString();
                    break;
            }
            if (isSpecial)
                TweenID = $"KeyViewer.{SpecialType}Tween";
            else
                TweenID = $"KeyViewer.{Code}Tween";
            initialized = true;
            return this;
        }
        private void Update()
        {
            if (!initialized) return;
            switch (SpecialType)
            {
                case SpecialKeyType.KPS:
                    CountText.text = Profile.calculator.Kps.ToString();
                    return;
                case SpecialKeyType.Total:
                    CountText.text = Total.ToString();
                    return;
            }
            if (InputAPI.Active)
                Pressed = InputAPI.APIFlags.TryGetValue(Code, out bool flag) && flag;
            else
            {
                Pressed = Input.GetKey(Code);
                if (config.SpareCode != KeyCode.None)
                    Pressed |= Input.GetKey(config.SpareCode);
            }
            if (Pressed == prevPressed) return;
            prevPressed = Pressed;
            if (DOTween.IsTweening(TweenID))
                DOTween.Kill(TweenID);
            if (Pressed)
            {
                CountText.text = (++Count).ToString();
                Profile.calculator.PressCount++;
                Total++;
            }
            Color bgColor = Color.white, outlineColor;
            VertexGradient textColor, countTextColor;
            Vector3 scale = new Vector3(1, 1, 1);
            if (Pressed)
            {
                if (InputAPI.Active)
                    InputAPI.KeyPress(Code);
                bgColor = config.PressedBackgroundColor;
                outlineColor = config.PressedOutlineColor;
                textColor = config.PressedTextColor;
                countTextColor = config.PressedCountTextColor;
                if (Profile.AnimateKeys)
                    scale *= config.ShrinkFactor;
            }
            else
            {
                if (InputAPI.Active)
                    InputAPI.KeyRelease(Code);
                bgColor = config.ReleasedBackgroundColor;
                outlineColor = config.ReleasedOutlineColor;
                textColor = config.ReleasedTextColor;
                countTextColor = config.ReleasedCountTextColor;
            }
            Background.color = bgColor;
            Outline.color = outlineColor;
            Text.colorGradient = textColor;
            CountText.colorGradient = countTextColor;
            if (Profile.AnimateKeys)
            {
                Background.rectTransform.DOScale(scale, config.EaseDuration)
                    .SetId(TweenID)
                    .SetEase(config.Ease)
                    .SetUpdate(true)
                    .OnKill(() => Background.rectTransform.localScale = scale);
                Outline.rectTransform.DOScale(scale, config.EaseDuration)
                    .SetId(TweenID)
                    .SetEase(config.Ease)
                    .SetUpdate(true)
                    .OnKill(() => Outline.rectTransform.localScale = scale);
                Text.rectTransform.DOScale(scale, config.EaseDuration)
                    .SetId(TweenID)
                    .SetEase(config.Ease)
                    .SetUpdate(true)
                    .OnKill(() => Text.rectTransform.localScale = scale);
                CountText.rectTransform.DOScale(scale, config.EaseDuration)
                    .SetId(TweenID)
                    .SetEase(config.Ease)
                    .SetUpdate(true)
                    .OnKill(() => CountText.rectTransform.localScale = scale);
            }
            else
            {
                Background.rectTransform.localScale = scale;
                Outline.rectTransform.localScale = scale;
                Text.rectTransform.localScale = scale;
                CountText.rectTransform.localScale = scale;
            }
        }
        public void UpdateLayout(ref float x, ref float tempX, int updateCount)
        {
            if (FontManager.TryGetFont(config.Font, out var font))
            {
                Text.font = font.fontTMP;
                CountText.font = font.fontTMP;
            }
            if (isSpecial && Profile.MakeBarSpecialKeys)
            {
                int spacing = updateCount * 10;
                int specialCount = keyManager.specialKeys.Count;
                Vector2 size = new Vector2(0, 75 * (config.Height / 100));
                size.x = (x - 10) / specialCount * (config.Width / 100) - spacing / 2;
                this.position = new Vector2(size.x / 2 + tempX + spacing / 2, -(config.Height / 2 - 10));
                Vector2 position = this.position + offsetVec;
                tempX += size.x;
                Background.rectTransform.anchorMin = Vector2.zero;
                Background.rectTransform.anchorMax = Vector2.zero;
                Background.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                Background.rectTransform.sizeDelta = size;
                Background.rectTransform.anchoredPosition = position;

                Outline.rectTransform.anchorMin = Vector2.zero;
                Outline.rectTransform.anchorMax = Vector2.zero;
                Outline.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                Outline.rectTransform.sizeDelta = size;
                Outline.rectTransform.anchoredPosition = position;

                float hOffset = config.Height / 5f;
                Text.rectTransform.anchorMin = Vector2.zero;
                Text.rectTransform.anchorMax = Vector2.zero;
                Text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                Text.rectTransform.sizeDelta = size * new Vector2(1, size.y * 1.03f);
                Text.rectTransform.anchoredPosition = position + new Vector2(config.TextOffsetX, config.TextOffsetY + hOffset);
                Text.fontSize = config.TextFontSize - 15;
                Text.fontSizeMax = config.TextFontSize - 15;
                Text.enableAutoSizing = true;

                CountText.rectTransform.anchorMin = Vector2.zero;
                CountText.rectTransform.anchorMax = Vector2.zero;
                CountText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                CountText.rectTransform.sizeDelta = size * new Vector2(1, size.y * 0.8f);
                CountText.rectTransform.anchoredPosition = position + new Vector2(config.CountTextOffsetX, config.CountTextOffsetY - hOffset);
                CountText.fontSizeMin = 0;
                CountText.fontSize = config.CountTextFontSize - 5;
                CountText.fontSizeMax = config.CountTextFontSize - 5;
                CountText.enableAutoSizing = true;
                CountText.gameObject.SetActive(true);

                Background.color = config.ReleasedBackgroundColor;
                Outline.color = config.ReleasedOutlineColor;
                Text.colorGradient = config.ReleasedTextColor;
                CountText.colorGradient = config.ReleasedCountTextColor;
                return;
            }
            float keyWidth = config.Width, keyHeight = config.Height;
            DOTween.Kill(TweenID);
            if (Profile.ShowKeyPressTotal)
                keyHeight += 50;
            position = new Vector2(keyWidth / 2f + x, keyHeight / 2f);
            Vector2 anchoredPos = position + offsetVec;
            Background.rectTransform.anchorMin = Vector2.zero;
            Background.rectTransform.anchorMax = Vector2.zero;
            Background.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Background.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Background.rectTransform.anchoredPosition = anchoredPos;

            Outline.rectTransform.anchorMin = Vector2.zero;
            Outline.rectTransform.anchorMax = Vector2.zero;
            Outline.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Outline.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Outline.rectTransform.anchoredPosition = anchoredPos;

            float heightOffset = keyHeight / 4f;
            Text.rectTransform.anchorMin = Vector2.zero;
            Text.rectTransform.anchorMax = Vector2.zero;
            Text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Text.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 1.03f);
            if (Profile.ShowKeyPressTotal)
                Text.rectTransform.anchoredPosition = anchoredPos + new Vector2(config.TextOffsetX, config.TextOffsetY + heightOffset);
            else Text.rectTransform.anchoredPosition = anchoredPos + new Vector2(config.TextOffsetX, config.TextOffsetY);
            Text.fontSize = config.TextFontSize;
            Text.fontSizeMax = config.TextFontSize;
            Text.enableAutoSizing = true;

            CountText.rectTransform.anchorMin = Vector2.zero;
            CountText.rectTransform.anchorMax = Vector2.zero;
            CountText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            CountText.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 0.8f);
            CountText.rectTransform.anchoredPosition = anchoredPos + new Vector2(config.CountTextOffsetX, config.CountTextOffsetY - heightOffset);
            CountText.fontSizeMin = 0;
            CountText.fontSize = config.CountTextFontSize;
            CountText.fontSizeMax = config.CountTextFontSize;
            CountText.enableAutoSizing = true;
            CountText.gameObject.SetActive(Profile.ShowKeyPressTotal);

            if (isSpecial && Profile.MakeBarSpecialKeys) return;
            Vector3 scale = Vector3.one;
            if (Pressed)
            {
                Background.color = config.PressedBackgroundColor;
                Outline.color = config.PressedOutlineColor;
                Text.colorGradient = config.PressedTextColor;
                CountText.colorGradient = config.PressedCountTextColor;
                scale *= config.ShrinkFactor;
            }
            else
            {
                Background.color = config.ReleasedBackgroundColor;
                Outline.color = config.ReleasedOutlineColor;
                Text.colorGradient = config.ReleasedTextColor;
                CountText.colorGradient = config.ReleasedCountTextColor;
            }
            Background.rectTransform.localScale = scale;
            Outline.rectTransform.localScale = scale;
            Text.rectTransform.localScale = scale;
            CountText.rectTransform.localScale = scale;
            x += keyWidth + 10;
        }
        public void RenderGUI()
        {
            if (isSpecial)
            {
                if (!(config.Editing = GUILayout.Toggle(config.Editing, $"{SpecialType} Setting"))) return;
            }
            else if (!(config.Editing = GUILayout.Toggle(config.Editing, $"{Code} Setting"))) return;
            MoreGUILayout.BeginIndent();

            if (!isSpecial)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < Profile.KeyGroups.Count; i++)
                {
                    var group = Profile.KeyGroups[i];
                    if (!group.IsAdded(config))
                    {
                        if (GUILayout.Button($"Add This At {group.Name}"))
                            group.AddConfig(config);
                    }
                    else
                    {
                        if (GUILayout.Button($"Remove This At {group.Name}"))
                            group.RemoveConfig(config);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            float offsetX = MoreGUILayout.NamedSliderContent(lang.GetString("OFFSET_X"), config.OffsetX, -Screen.width, Screen.width, 300f);
            float offsetY = MoreGUILayout.NamedSliderContent(lang.GetString("OFFSET_Y"), config.OffsetY, -Screen.height, Screen.height, 300f);
            if (offsetX != config.OffsetX)
            {
                config.OffsetX = offsetX;
                keyManager.UpdateLayout();
            }
            if (offsetY != config.OffsetY)
            {
                config.OffsetY = offsetY;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string font = MoreGUILayout.NamedTextField(Main.Lang.GetString("FONT"), config.Font, 300f);
            if (font != config.Font)
            {
                config.Font = font;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float width = MoreGUILayout.NamedSliderContent(lang.GetString("WIDTH"), config.Width, -Screen.width, Screen.width, 300f);
            float height = MoreGUILayout.NamedSliderContent(lang.GetString("HEIGHT"), config.Height, -Screen.height, Screen.height, 300f);
            if (width != config.Width)
            {
                config.Width = width;
                keyManager.UpdateLayout();
            }
            if (height != config.Height)
            {
                config.Height = height;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float textXOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("TEXT_OFFSET_X"), config.TextOffsetX, -300f, 300f, 200f);
            float textYOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("TEXT_OFFSET_Y"), config.TextOffsetY, -300f, 300f, 200f);
            if (textXOffset != config.TextOffsetX)
            {
                config.TextOffsetX = textXOffset;
                keyManager.UpdateLayout();
            }
            if (textYOffset != config.TextOffsetY)
            {
                config.TextOffsetY = textYOffset;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float countTextXOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("COUNT_TEXT_OFFSET_X"), config.CountTextOffsetX, -300f, 300f, 200f);
            float countTextYOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("COUNT_TEXT_OFFSET_Y"), config.CountTextOffsetY, -300f, 300f, 200f);
            if (countTextXOffset != config.CountTextOffsetX)
            {
                config.CountTextOffsetX = countTextXOffset;
                keyManager.UpdateLayout();
            }
            if (countTextYOffset != config.CountTextOffsetY)
            {
                config.CountTextOffsetY = countTextYOffset;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float textSize = MoreGUILayout.NamedSliderContent(lang.GetString("TEXT_FONT_SIZE"), config.TextFontSize, 0, 300f, 200f);
            float countTextSize = MoreGUILayout.NamedSliderContent(lang.GetString("COUNT_TEXT_FONT_SIZE"), config.CountTextFontSize, 0, 300f, 200f);
            if (textSize != config.TextFontSize)
            {
                config.TextFontSize = textSize;
                keyManager.UpdateLayout();
            }
            if (countTextSize != config.CountTextFontSize)
            {
                config.CountTextFontSize = countTextSize;
                keyManager.UpdateLayout();
            }
            GUILayout.EndHorizontal();
            if (isSpecial)
            {
                Color newOutline, newBackground;
                string newOutlineHex, newBackgroundHex;
                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("OUTLINE_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("BACKGROUND_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();

                MoreGUILayout.BeginIndent();
                (newOutline, newBackground) = MoreGUILayout.ColorRgbaSlidersPair(config.ReleasedOutlineColor, config.ReleasedBackgroundColor);
                if (newOutline != config.ReleasedOutlineColor)
                {
                    config.ReleasedOutlineColor = newOutline;
                    keyManager.UpdateLayout();
                }
                if (newBackground != config.ReleasedBackgroundColor)
                {
                    config.ReleasedBackgroundColor = newBackground;
                    keyManager.UpdateLayout();
                }

                (newOutlineHex, newBackgroundHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.ReleasedOutlineColorHex, config.ReleasedBackgroundColorHex, 100, 40);
                if (newOutlineHex != config.ReleasedOutlineColorHex && ColorUtility.TryParseHtmlString($"#{newOutlineHex}", out newOutline))
                {
                    config.ReleasedOutlineColor = newOutline;
                    keyManager.UpdateLayout();
                }
                if (newBackgroundHex != config.ReleasedBackgroundColorHex && ColorUtility.TryParseHtmlString($"#{newBackgroundHex}", out newBackground))
                {
                    config.ReleasedBackgroundColor = newBackground;
                    keyManager.UpdateLayout();
                }
                MoreGUILayout.EndIndent();

                Color newTextColor, newCountTextColor;
                VertexGradient newTextColorG, newCountTextColorG;
                string newLeftHex, newRightHex;

                config.Gradient = GUILayout.Toggle(config.Gradient, "Gradient");

                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("COUNT_TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();
                if (config.Gradient)
                {
                    (newTextColorG, newCountTextColorG) = MoreGUILayout.VertexGradientSlidersPair(config.ReleasedTextColor, config.ReleasedCountTextColor);
                    if (newTextColorG.Inequals(config.ReleasedTextColor))
                    {
                        config.ReleasedTextColor = newTextColorG;
                        keyManager.UpdateLayout();
                    }
                    if (newCountTextColorG.Inequals(config.ReleasedCountTextColor))
                    {
                        config.ReleasedCountTextColor = newCountTextColorG;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newLeftHex, newRightHex) = MoreGUILayout.NamedTextFieldPair("Top Left Hex:", "Top Left Hex:", config.ReleasedTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newLeftHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newLeftHex}", out newTextColor))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(newTextColor, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newRightHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newRightHex}", out newCountTextColor))
                    {
                        var orig = config.ReleasedCountTextColor;
                        config.ReleasedCountTextColor = new VertexGradient(newCountTextColor, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newLeftHex, newRightHex) = MoreGUILayout.NamedTextFieldPair("Top Right Hex:", "Top Right Hex:", config.ReleasedTextColorHex[1], config.ReleasedCountTextColorHex[1], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newLeftHex != config.ReleasedTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newLeftHex}", out newTextColor))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, newTextColor, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newRightHex != config.ReleasedCountTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newRightHex}", out newCountTextColor))
                    {
                        var orig = config.ReleasedCountTextColor;
                        config.ReleasedCountTextColor = new VertexGradient(orig.topLeft, newCountTextColor, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newLeftHex, newRightHex) = MoreGUILayout.NamedTextFieldPair("Bottom Left Hex:", "Bottom Left Hex:", config.ReleasedTextColorHex[2], config.ReleasedCountTextColorHex[2], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newLeftHex != config.ReleasedTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newLeftHex}", out newTextColor))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newTextColor, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newRightHex != config.ReleasedCountTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newRightHex}", out newCountTextColor))
                    {
                        var orig = config.ReleasedCountTextColor;
                        config.ReleasedCountTextColor = new VertexGradient(orig.topLeft, orig.topRight, newCountTextColor, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newLeftHex, newRightHex) = MoreGUILayout.NamedTextFieldPair("Bottom Right Hex:", "Bottom Right Hex:", config.ReleasedTextColorHex[3], config.ReleasedCountTextColorHex[3], 100, 110);
                    GUILayout.EndHorizontal();

                    if (newLeftHex != config.ReleasedTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newLeftHex}", out newTextColor))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newTextColor);
                        keyManager.UpdateLayout();
                    }
                    if (newRightHex != config.ReleasedCountTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newRightHex}", out newCountTextColor))
                    {
                        var orig = config.ReleasedCountTextColor;
                        config.ReleasedCountTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newCountTextColor);
                        keyManager.UpdateLayout();
                    }
                }
                else
                {
                    (newTextColor, newCountTextColor) = MoreGUILayout.ColorRgbaSlidersPair(config.ReleasedTextColor.topLeft, config.ReleasedCountTextColor.topLeft);
                    if (newTextColor != config.ReleasedTextColor.topLeft)
                    {
                        config.ReleasedTextColor = new VertexGradient(newTextColor);
                        keyManager.UpdateLayout();
                    }
                    if (newCountTextColor != config.ReleasedCountTextColor.topLeft)
                    {
                        config.ReleasedCountTextColor = new VertexGradient(newCountTextColor);
                        keyManager.UpdateLayout();
                    }

                    (newLeftHex, newRightHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.ReleasedTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 40);
                    if (newLeftHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newLeftHex}", out newTextColor))
                    {
                        config.ReleasedTextColor = new VertexGradient(newTextColor);
                        keyManager.UpdateLayout();
                    }
                    if (newRightHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newRightHex}", out newCountTextColor))
                    {
                        config.ReleasedCountTextColor = new VertexGradient(newCountTextColor);
                        keyManager.UpdateLayout();
                    }
                }
                MoreGUILayout.EndIndent();
            }
            else
            {
                GUILayout.BeginHorizontal();
                config.ShrinkFactor = MoreGUILayout.NamedSliderContent("Shrink Factor", config.ShrinkFactor, 0, 10, 600f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                config.EaseDuration = MoreGUILayout.NamedSliderContent("Ease Duration", config.EaseDuration, 0, 10, 600f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Ease:");
                DrawEase(config.Ease, ease => config.Ease = ease);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("KEY_COUNT"));
                if (uint.TryParse(GUILayout.TextField(config.Count.ToString()), out uint count))
                {
                    config.Count = count;
                    CountText.text = config.Count.ToString();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("KeyCode:");
                DrawKeyCode(config.Code, code => config.Code = code);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Spare KeyCode:");
                DrawSpareKeyCode(config.SpareCode, code => config.SpareCode = code);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (config.ChangeBgColorJudge = GUILayout.Toggle(config.ChangeBgColorJudge, lang.GetString("CHANGE_BG_COLOR_FOLLOWING_HITMARGIN")))
                {
                    MoreGUILayout.BeginIndent();
                    string te, ve;
                    string ep, p;
                    string lp, vl;
                    string tl, mp;
                    string fm, fo;
                    GUILayout.BeginHorizontal();
                    (te, ve) = MoreGUILayout.NamedTextFieldPair("Too Early Hex:", "Very Early Hex:", config.HitMarginColorHex[0], config.HitMarginColorHex[1], 100, 120);
                    GUILayout.EndHorizontal();

                    if (te != config.HitMarginColorHex[0] && ColorUtility.TryParseHtmlString($"#{te}", out Color col))
                    {
                        config.TooEarlyColor = col;
                        keyManager.UpdateLayout();
                    }
                    if (ve != config.HitMarginColorHex[1] && ColorUtility.TryParseHtmlString($"#{ve}", out col))
                    {
                        config.VeryEarlyColor = col;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (ep, p) = MoreGUILayout.NamedTextFieldPair("Early Perfect Hex:", "Perfect Hex:", config.HitMarginColorHex[2], config.HitMarginColorHex[3], 100, 120);
                    GUILayout.EndHorizontal();

                    if (ep != config.HitMarginColorHex[2] && ColorUtility.TryParseHtmlString($"#{ep}", out col))
                    {
                        config.EarlyPerfectColor = col;
                        keyManager.UpdateLayout();
                    }
                    if (p != config.HitMarginColorHex[3] && ColorUtility.TryParseHtmlString($"#{p}", out col))
                    {
                        config.PerfectColor = col;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (lp, vl) = MoreGUILayout.NamedTextFieldPair("Late Perfect Hex:", "Very Late Hex:", config.HitMarginColorHex[4], config.HitMarginColorHex[5], 100, 120);
                    GUILayout.EndHorizontal();

                    if (lp != config.HitMarginColorHex[4] && ColorUtility.TryParseHtmlString($"#{lp}", out col))
                    {
                        config.LatePerfectColor = col;
                        keyManager.UpdateLayout();
                    }
                    if (vl != config.HitMarginColorHex[5] && ColorUtility.TryParseHtmlString($"#{vl}", out col))
                    {
                        config.VeryLateColor = col;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (tl, mp) = MoreGUILayout.NamedTextFieldPair("Too Late Hex:", "Multipress Hex:", config.HitMarginColorHex[6], config.HitMarginColorHex[7], 100, 120);
                    GUILayout.EndHorizontal();

                    if (tl != config.HitMarginColorHex[6] && ColorUtility.TryParseHtmlString($"#{tl}", out col))
                    {
                        config.TooLateColor = col;
                        keyManager.UpdateLayout();
                    }
                    if (mp != config.HitMarginColorHex[7] && ColorUtility.TryParseHtmlString($"#{mp}", out col))
                    {
                        config.MultipressColor = col;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (fm, fo) = MoreGUILayout.NamedTextFieldPair("Fail Miss Hex:", "Fail Overload Hex:", config.HitMarginColorHex[8], config.HitMarginColorHex[9], 100, 120);
                    GUILayout.EndHorizontal();

                    if (fm != config.HitMarginColorHex[8] && ColorUtility.TryParseHtmlString($"#{fm}", out col))
                    {
                        config.FailMissColor = col;
                        keyManager.UpdateLayout();
                    }
                    if (fo != config.HitMarginColorHex[9] && ColorUtility.TryParseHtmlString($"#{fo}", out col))
                    {
                        config.FailOverloadColor = col;
                        keyManager.UpdateLayout();
                    }
                    MoreGUILayout.EndIndent();
                }
                Color newPressed, newReleased;
                string newPressedHex, newReleasedHex;
                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("PRESSED_OUTLINE_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("RELEASED_OUTLINE_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();
                (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedOutlineColor, config.ReleasedOutlineColor);
                if (newPressed != config.PressedOutlineColor)
                {
                    config.PressedOutlineColor = newPressed;
                    keyManager.UpdateLayout();
                }
                if (newReleased != config.ReleasedOutlineColor)
                {
                    config.ReleasedOutlineColor = newReleased;
                    keyManager.UpdateLayout();
                }

                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedOutlineColorHex, config.ReleasedOutlineColorHex, 100, 40);
                if (newPressedHex != config.PressedOutlineColorHex && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    config.PressedOutlineColor = newPressed;
                    keyManager.UpdateLayout();
                }
                if (newReleasedHex != config.ReleasedOutlineColorHex && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    config.ReleasedOutlineColor = newReleased;
                    keyManager.UpdateLayout();
                }
                MoreGUILayout.EndIndent();
                GUILayout.Space(8f);
                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("PRESSED_BACKGROUND_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("RELEASED_BACKGROUND_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();
                (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedBackgroundColor, config.ReleasedBackgroundColor);
                if (newPressed != config.PressedBackgroundColor)
                {
                    config.PressedBackgroundColor = newPressed;
                    keyManager.UpdateLayout();
                }
                if (newReleased != config.ReleasedBackgroundColor)
                {
                    config.ReleasedBackgroundColor = newReleased;
                    keyManager.UpdateLayout();
                }
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedBackgroundColorHex, config.ReleasedBackgroundColorHex, 100, 40);
                if (newPressedHex != config.PressedBackgroundColorHex && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    config.PressedBackgroundColor = newPressed;
                    keyManager.UpdateLayout();
                }
                if (newReleasedHex != config.ReleasedBackgroundColorHex && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    config.ReleasedBackgroundColor = newReleased;
                    keyManager.UpdateLayout();
                }
                MoreGUILayout.EndIndent();
                GUILayout.Space(8f);

                config.Gradient = GUILayout.Toggle(config.Gradient, "Gradient");

                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("PRESSED_TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("RELEASED_TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();
                if (config.Gradient)
                {
                    VertexGradient newPressedG, newReleasedG;
                    (newPressedG, newReleasedG) = MoreGUILayout.VertexGradientSlidersPair(config.PressedTextColor, config.ReleasedTextColor);
                    if (newPressedG.Inequals(config.PressedTextColor))
                    {
                        config.PressedTextColor = newPressedG;
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedG.Inequals(config.ReleasedTextColor))
                    {
                        config.ReleasedTextColor = newReleasedG;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Left Hex:", "Top Left Hex:", config.PressedTextColorHex[0], config.ReleasedTextColorHex[0], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(newPressed, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(newReleased, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Right Hex:", "Top Right Hex:", config.PressedTextColorHex[1], config.ReleasedTextColorHex[1], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, newPressed, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, newReleased, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Left Hex:", "Bottom Left Hex:", config.PressedTextColorHex[2], config.ReleasedTextColorHex[2], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newPressed, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newReleased, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Right Hex:", "Bottom Right Hex:", config.PressedTextColorHex[3], config.ReleasedTextColorHex[3], 100, 110);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newReleased);
                        keyManager.UpdateLayout();
                    }
                }
                else
                {
                    (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedTextColor.topLeft, config.ReleasedTextColor.topLeft);
                    if (newPressed != config.PressedTextColor.topLeft)
                    {
                        config.PressedTextColor = new VertexGradient(newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleased != config.ReleasedTextColor.topLeft)
                    {
                        config.ReleasedTextColor = new VertexGradient(newReleased);
                        keyManager.UpdateLayout();
                    }

                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedTextColorHex[0], config.ReleasedTextColorHex[0], 100, 40);
                    if (newPressedHex != config.PressedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        config.PressedTextColor = new VertexGradient(newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        config.ReleasedTextColor = new VertexGradient(newReleased);
                        keyManager.UpdateLayout();
                    }
                }
                MoreGUILayout.EndIndent();

                GUILayout.BeginHorizontal();
                GUILayout.Label(lang.GetString("PRESSED_COUNT_TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(lang.GetString("RELEASED_COUNT_TEXT_COLOR"), GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();
                if (config.Gradient)
                {
                    VertexGradient newPressedG, newReleasedG;
                    (newPressedG, newReleasedG) = MoreGUILayout.VertexGradientSlidersPair(config.PressedCountTextColor, config.ReleasedCountTextColor);
                    if (newPressedG.Inequals(config.PressedCountTextColor))
                    {
                        config.PressedCountTextColor = newPressedG;
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedG.Inequals(config.ReleasedCountTextColor))
                    {
                        config.ReleasedCountTextColor = newReleasedG;
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Left Hex:", "Top Left Hex:", config.PressedCountTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(newPressed, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(newReleased, orig.topRight, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Right Hex:", "Top Right Hex:", config.PressedCountTextColorHex[1], config.ReleasedCountTextColorHex[1], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedCountTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, newPressed, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedCountTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, newReleased, orig.bottomLeft, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Left Hex:", "Bottom Left Hex:", config.PressedCountTextColorHex[2], config.ReleasedCountTextColorHex[2], 100, 100);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedCountTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newPressed, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedCountTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newReleased, orig.bottomRight);
                        keyManager.UpdateLayout();
                    }

                    GUILayout.BeginHorizontal();
                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Right Hex:", "Bottom Right Hex:", config.PressedCountTextColorHex[3], config.ReleasedCountTextColorHex[3], 100, 110);
                    GUILayout.EndHorizontal();

                    if (newPressedHex != config.PressedCountTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        var orig = config.PressedTextColor;
                        config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedCountTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        var orig = config.ReleasedTextColor;
                        config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newReleased);
                        keyManager.UpdateLayout();
                    }
                }
                else
                {
                    (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedCountTextColor.topLeft, config.ReleasedCountTextColor.topLeft);
                    if (newPressed != config.PressedCountTextColor.topLeft)
                    {
                        config.PressedCountTextColor = new VertexGradient(newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleased != config.ReleasedCountTextColor.topLeft)
                    {
                        config.ReleasedCountTextColor = new VertexGradient(newReleased);
                        keyManager.UpdateLayout();
                    }

                    (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedCountTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 40);
                    if (newPressedHex != config.PressedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                    {
                        config.PressedCountTextColor = new VertexGradient(newPressed);
                        keyManager.UpdateLayout();
                    }
                    if (newReleasedHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                    {
                        config.ReleasedCountTextColor = new VertexGradient(newReleased);
                        keyManager.UpdateLayout();
                    }
                }
                MoreGUILayout.EndIndent();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(lang.GetString("RESET")))
            {
                config.Reset();
                keyManager.UpdateLayout();
            }
            if (config.CanUndo)
            {
                if (GUILayout.Button(Main.Lang.GetString("UNDO")))
                {
                    if (config.Undo())
                        keyManager.UpdateLayout();
                }
            }
            if (config.CanRedo)
            {
                if (GUILayout.Button(Main.Lang.GetString("REDO")))
                {
                    if (config.Redo())
                        keyManager.UpdateLayout();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            MoreGUILayout.EndIndent();
        }
        public void ChangeHitMarginColor(HitMargin hit)
        {
            if (config.ChangeBgColorJudge)
            {
                var col = GetHitMarginColor(hit);
                //Main.Log.Log($"Changing Color {Code} HitMargin:{hit} Color:{col}");
                Background.color = col;
            }
        }
        public Color GetHitMarginColor(HitMargin hit)
        {
            switch (hit)
            {
                case HitMargin.TooEarly:
                    return config.TooEarlyColor;
                case HitMargin.VeryEarly:
                    return config.VeryEarlyColor;
                case HitMargin.EarlyPerfect:
                    return config.EarlyPerfectColor;
                case HitMargin.Perfect:
                    return config.PerfectColor;
                case HitMargin.LatePerfect:
                    return config.LatePerfectColor;
                case HitMargin.VeryLate:
                    return config.VeryLateColor;
                case HitMargin.TooLate:
                    return config.TooLateColor;
                case HitMargin.Multipress:
                    return config.MultipressColor;
                case HitMargin.FailMiss:
                    return config.FailMissColor;
                case HitMargin.FailOverload:
                    return config.FailOverloadColor;
                case HitMargin.Auto:
                    return config.PerfectColor;
                default:
                    return config.PressedBackgroundColor;
            }
        }
        internal static readonly Ease[] eases = (Ease[])Enum.GetValues(typeof(Ease));
        internal static readonly string[] easeNames = eases.Select(e => e.ToString()).ToArray();
        internal static readonly string[] keyNames = Main.KeyCodes.Select(c => c.ToString()).ToArray();
        internal static readonly Dictionary<KeyCode, int> codeIndex;
        internal static readonly Dictionary<KeyCode, ushort> KeyCodeToNative;
        static Key()
        {
            codeIndex = new Dictionary<KeyCode, int>();
            for (int i = 0; i < Main.KeyCodes.Length; i++)
                codeIndex[Main.KeyCodes[i]] = i;
            KeyCodeToNative = new Dictionary<KeyCode, ushort>();
            KeyCodeToNative[KeyCode.None] = 0;
            KeyCodeToNative[KeyCode.Escape] = 1;
            KeyCodeToNative[KeyCode.Alpha1] = 2;
            KeyCodeToNative[KeyCode.Alpha2] = 3;
            KeyCodeToNative[KeyCode.Alpha3] = 4;
            KeyCodeToNative[KeyCode.Alpha4] = 5;
            KeyCodeToNative[KeyCode.Alpha5] = 6;
            KeyCodeToNative[KeyCode.Alpha6] = 7;
            KeyCodeToNative[KeyCode.Alpha7] = 8;
            KeyCodeToNative[KeyCode.Alpha8] = 9;
            KeyCodeToNative[KeyCode.Alpha9] = 10;
            KeyCodeToNative[KeyCode.Alpha0] = 11;
            KeyCodeToNative[KeyCode.Minus] = 12;
            KeyCodeToNative[KeyCode.Equals] = 13;
            KeyCodeToNative[KeyCode.Backspace] = 14;
            KeyCodeToNative[KeyCode.Tab] = 15;
            KeyCodeToNative[KeyCode.Q] = 16;
            KeyCodeToNative[KeyCode.W] = 17;
            KeyCodeToNative[KeyCode.E] = 18;
            KeyCodeToNative[KeyCode.R] = 19;
            KeyCodeToNative[KeyCode.T] = 20;
            KeyCodeToNative[KeyCode.Y] = 21;
            KeyCodeToNative[KeyCode.U] = 22;
            KeyCodeToNative[KeyCode.I] = 23;
            KeyCodeToNative[KeyCode.O] = 24;
            KeyCodeToNative[KeyCode.P] = 25;
            KeyCodeToNative[KeyCode.LeftBracket] = 26;
            KeyCodeToNative[KeyCode.RightBracket] = 27;
            KeyCodeToNative[KeyCode.Return] = 28;
            KeyCodeToNative[KeyCode.LeftControl] = 29;
            KeyCodeToNative[KeyCode.A] = 30;
            KeyCodeToNative[KeyCode.S] = 31;
            KeyCodeToNative[KeyCode.D] = 32;
            KeyCodeToNative[KeyCode.F] = 33;
            KeyCodeToNative[KeyCode.G] = 34;
            KeyCodeToNative[KeyCode.H] = 35;
            KeyCodeToNative[KeyCode.J] = 36;
            KeyCodeToNative[KeyCode.K] = 37;
            KeyCodeToNative[KeyCode.L] = 38;
            KeyCodeToNative[KeyCode.Semicolon] = 39;
            KeyCodeToNative[KeyCode.Quote] = 40;
            KeyCodeToNative[KeyCode.BackQuote] = 41;
            KeyCodeToNative[KeyCode.LeftShift] = 42;
            KeyCodeToNative[KeyCode.Backslash] = 43;
            KeyCodeToNative[KeyCode.Z] = 44;
            KeyCodeToNative[KeyCode.X] = 45;
            KeyCodeToNative[KeyCode.C] = 46;
            KeyCodeToNative[KeyCode.V] = 47;
            KeyCodeToNative[KeyCode.B] = 48;
            KeyCodeToNative[KeyCode.N] = 49;
            KeyCodeToNative[KeyCode.M] = 50;
            KeyCodeToNative[KeyCode.Comma] = 51;
            KeyCodeToNative[KeyCode.Period] = 52;
            KeyCodeToNative[KeyCode.Slash] = 53;
            KeyCodeToNative[KeyCode.RightShift] = 54;
            KeyCodeToNative[KeyCode.KeypadMultiply] = 55;
            KeyCodeToNative[KeyCode.LeftAlt] = 56;
            KeyCodeToNative[KeyCode.Space] = 57;
            KeyCodeToNative[KeyCode.CapsLock] = 58;
            KeyCodeToNative[KeyCode.F1] = 59;
            KeyCodeToNative[KeyCode.F2] = 60;
            KeyCodeToNative[KeyCode.F3] = 61;
            KeyCodeToNative[KeyCode.F4] = 62;
            KeyCodeToNative[KeyCode.F5] = 63;
            KeyCodeToNative[KeyCode.F6] = 64;
            KeyCodeToNative[KeyCode.F7] = 65;
            KeyCodeToNative[KeyCode.F8] = 66;
            KeyCodeToNative[KeyCode.F9] = 67;
            KeyCodeToNative[KeyCode.F10] = 68;
            KeyCodeToNative[KeyCode.Numlock] = 69;
            KeyCodeToNative[KeyCode.ScrollLock] = 70;
            KeyCodeToNative[KeyCode.Keypad7] = 71;
            KeyCodeToNative[KeyCode.Keypad8] = 72;
            KeyCodeToNative[KeyCode.Keypad9] = 73;
            KeyCodeToNative[KeyCode.KeypadMinus] = 74;
            KeyCodeToNative[KeyCode.Keypad4] = 75;
            KeyCodeToNative[KeyCode.Keypad5] = 76;
            KeyCodeToNative[KeyCode.Keypad6] = 77;
            KeyCodeToNative[KeyCode.KeypadPlus] = 78;
            KeyCodeToNative[KeyCode.Keypad1] = 79;
            KeyCodeToNative[KeyCode.Keypad2] = 80;
            KeyCodeToNative[KeyCode.Keypad3] = 81;
            KeyCodeToNative[KeyCode.Keypad0] = 82;
            KeyCodeToNative[KeyCode.KeypadPeriod] = 83;
            KeyCodeToNative[KeyCode.F11] = 87;
            KeyCodeToNative[KeyCode.F12] = 88;
            KeyCodeToNative[KeyCode.F13] = 91;
            KeyCodeToNative[KeyCode.F14] = 92;
            KeyCodeToNative[KeyCode.F15] = 93;
            KeyCodeToNative[KeyCode.Underscore] = 115;
            KeyCodeToNative[KeyCode.Comma] = 126;
            KeyCodeToNative[KeyCode.KeypadEquals] = 3597;
            KeyCodeToNative[KeyCode.KeypadEnter] = 3612;
            KeyCodeToNative[KeyCode.RightControl] = 3613;
            KeyCodeToNative[KeyCode.KeypadDivide] = 3637;
            KeyCodeToNative[KeyCode.Print] = 3639;
            KeyCodeToNative[KeyCode.RightAlt] = 3640;
            KeyCodeToNative[KeyCode.Pause] = 3653;
            KeyCodeToNative[KeyCode.Home] = 3655;
            KeyCodeToNative[KeyCode.PageUp] = 3657;
            KeyCodeToNative[KeyCode.End] = 3663;
            KeyCodeToNative[KeyCode.PageDown] = 3665;
            KeyCodeToNative[KeyCode.Insert] = 3666;
            KeyCodeToNative[KeyCode.Delete] = 3667;
            KeyCodeToNative[KeyCode.LeftMeta] = 3675;
            KeyCodeToNative[KeyCode.RightMeta] = 3676;
            KeyCodeToNative[KeyCode.Menu] = 3677;
            KeyCodeToNative[KeyCode.UpArrow] = 57416;
            KeyCodeToNative[KeyCode.LeftArrow] = 57419;
            KeyCodeToNative[KeyCode.Clear] = 57420;
            KeyCodeToNative[KeyCode.RightArrow] = 57421;
            KeyCodeToNative[KeyCode.DownArrow] = 57424;
            KeyCodeToNative[KeyCode.Home] = 60999;
            KeyCodeToNative[KeyCode.UpArrow] = 61000;
            KeyCodeToNative[KeyCode.PageUp] = 61001;
            KeyCodeToNative[KeyCode.LeftArrow] = 61003;
            KeyCodeToNative[KeyCode.Clear] = 61004;
            KeyCodeToNative[KeyCode.RightArrow] = 61005;
            KeyCodeToNative[KeyCode.End] = 61007;
            KeyCodeToNative[KeyCode.DownArrow] = 61008;
            KeyCodeToNative[KeyCode.PageDown] = 61009;
            KeyCodeToNative[KeyCode.Insert] = 61010;
            KeyCodeToNative[KeyCode.Delete] = 61011;
            KeyCodeToNative[KeyCode.Mouse0] = 1001;
            KeyCodeToNative[KeyCode.Mouse1] = 1002;
            KeyCodeToNative[KeyCode.Mouse2] = 1003;
            KeyCodeToNative[KeyCode.Mouse3] = 1004;
            KeyCodeToNative[KeyCode.Mouse4] = 1005;
        }
        internal static bool DrawEase(Ease ease, Action<Ease> setter)
        {
            int index = (int)ease;
            bool changed = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref index, easeNames, "Ease");
            setter(eases[index]);
            return changed;
        }
        internal static bool DrawKeyCode(KeyCode code, Action<KeyCode> setter)
        {
            int index = codeIndex[code];
            bool changed = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref index, keyNames, $"{code} KeyCode");
            setter(Main.KeyCodes[index]);
            return changed;
        }
        internal static bool DrawSpareKeyCode(KeyCode code, Action<KeyCode> setter)
        {
            int index = codeIndex[code];
            bool changed = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref index, keyNames, $"{code} Spare KeyCode");
            setter(Main.KeyCodes[index]);
            return changed;
        }
        private static readonly Dictionary<KeyCode, string> KeyString =
            new Dictionary<KeyCode, string>() {
                { KeyCode.Alpha0, "0" },
                { KeyCode.Alpha1, "1" },
                { KeyCode.Alpha2, "2" },
                { KeyCode.Alpha3, "3" },
                { KeyCode.Alpha4, "4" },
                { KeyCode.Alpha5, "5" },
                { KeyCode.Alpha6, "6" },
                { KeyCode.Alpha7, "7" },
                { KeyCode.Alpha8, "8" },
                { KeyCode.Alpha9, "9" },
                { KeyCode.Keypad0, "0" },
                { KeyCode.Keypad1, "1" },
                { KeyCode.Keypad2, "2" },
                { KeyCode.Keypad3, "3" },
                { KeyCode.Keypad4, "4" },
                { KeyCode.Keypad5, "5" },
                { KeyCode.Keypad6, "6" },
                { KeyCode.Keypad7, "7" },
                { KeyCode.Keypad8, "8" },
                { KeyCode.Keypad9, "9" },
                { KeyCode.KeypadPlus, "+" },
                { KeyCode.KeypadMinus, "-" },
                { KeyCode.KeypadMultiply, "*" },
                { KeyCode.KeypadDivide, "/" },
                { KeyCode.KeypadEnter, "↵" },
                { KeyCode.KeypadEquals, "=" },
                { KeyCode.KeypadPeriod, "." },
                { KeyCode.Return, "↵" },
                { KeyCode.None, " " },
                { KeyCode.Tab, "⇥" },
                { KeyCode.Backslash, "\\" },
                { KeyCode.Slash, "/" },
                { KeyCode.Minus, "-" },
                { KeyCode.Equals, "=" },
                { KeyCode.LeftBracket, "[" },
                { KeyCode.RightBracket, "]" },
                { KeyCode.Semicolon, ";" },
                { KeyCode.Comma, "," },
                { KeyCode.Period, "." },
                { KeyCode.Quote, "'" },
                { KeyCode.UpArrow, "↑" },
                { KeyCode.DownArrow, "↓" },
                { KeyCode.LeftArrow, "←" },
                { KeyCode.RightArrow, "→" },
                { KeyCode.Space, "␣" },
                { KeyCode.BackQuote, "`" },
                { KeyCode.LeftShift, "L⇧" },
                { KeyCode.RightShift, "R⇧" },
                { KeyCode.LeftControl, "LCtrl" },
                { KeyCode.RightControl, "RCtrl" },
                { KeyCode.LeftAlt, "LAlt" },
                { KeyCode.RightAlt, "RAlt" },
                { KeyCode.Delete, "Del" },
                { KeyCode.PageDown, "Pg↓" },
                { KeyCode.PageUp, "Pg↑" },
                { KeyCode.Insert, "Ins" },
                { KeyCode.Mouse0, "M0" },
                { KeyCode.Mouse1, "M1" },
                { KeyCode.Mouse2, "M2" },
                { KeyCode.Mouse3, "M3" },
                { KeyCode.Mouse4, "M4" },
                { KeyCode.Mouse5, "M5" },
                { KeyCode.Mouse6, "M6" },
            };
        internal static void DrawGlobalConfig(Config config, Action<Config> onChange)
        {
            MoreGUILayout.BeginIndent();
            GUILayout.BeginHorizontal();
            string font = MoreGUILayout.NamedTextField(Main.Lang.GetString("FONT"), config.Font, 300f);
            if (font != config.Font)
            {
                config.Font = font;
                onChange(config);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float width = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("WIDTH"), config.Width, -Screen.width, Screen.width, 300f);
            float height = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("HEIGHT"), config.Height, -Screen.height, Screen.height, 300f);
            if (width != config.Width)
            {
                config.Width = width;
                onChange(config);
            }
            if (height != config.Height)
            {
                config.Height = height;
                onChange(config);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float textXOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("TEXT_OFFSET_X"), config.TextOffsetX, -300f, 300f, 200f);
            float textYOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("TEXT_OFFSET_Y"), config.TextOffsetY, -300f, 300f, 200f);
            if (textXOffset != config.TextOffsetX)
            {
                config.TextOffsetX = textXOffset;
                onChange(config);
            }
            if (textYOffset != config.TextOffsetY)
            {
                config.TextOffsetY = textYOffset;
                onChange(config);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float countTextXOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("COUNT_TEXT_OFFSET_X"), config.CountTextOffsetX, -300f, 300f, 200f);
            float countTextYOffset = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("COUNT_TEXT_OFFSET_Y"), config.CountTextOffsetY, -300f, 300f, 200f);
            if (countTextXOffset != config.CountTextOffsetX)
            {
                config.CountTextOffsetX = countTextXOffset;
                onChange(config);
            }
            if (countTextYOffset != config.CountTextOffsetY)
            {
                config.CountTextOffsetY = countTextYOffset;
                onChange(config);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float textSize = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("TEXT_FONT_SIZE"), config.TextFontSize, 0, 300f, 200f);
            float countTextSize = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("COUNT_TEXT_FONT_SIZE"), config.CountTextFontSize, 0, 300f, 200f);
            if (textSize != config.TextFontSize)
            {
                config.TextFontSize = textSize;
                onChange(config);
            }
            if (countTextSize != config.CountTextFontSize)
            {
                config.CountTextFontSize = countTextSize;
                onChange(config);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float shrinkFactor = MoreGUILayout.NamedSliderContent("Shrink Factor", config.ShrinkFactor, 0, 10, 600f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float easeDuration = MoreGUILayout.NamedSliderContent("Ease Duration", config.EaseDuration, 0, 10, 600f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (shrinkFactor != config.ShrinkFactor)
            {
                config.ShrinkFactor = shrinkFactor;
                onChange(config);
            }
            if (easeDuration != config.EaseDuration)
            {
                config.EaseDuration = easeDuration;
                onChange(config);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:");
            if (DrawEase(config.Ease, ease => config.Ease = ease))
                onChange(config);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            bool changeBgColorJudge;
            if (changeBgColorJudge = GUILayout.Toggle(config.ChangeBgColorJudge, Main.Lang.GetString("CHANGE_BG_COLOR_FOLLOWING_HITMARGIN")))
            {
                MoreGUILayout.BeginIndent();
                string te, ve;
                string ep, p;
                string lp, vl;
                string tl, mp;
                string fm, fo;
                GUILayout.BeginHorizontal();
                (te, ve) = MoreGUILayout.NamedTextFieldPair("Too Early Hex:", "Very Early Hex:", config.HitMarginColorHex[0], config.HitMarginColorHex[1], 100, 120);
                GUILayout.EndHorizontal();

                if (te != config.HitMarginColorHex[0] && ColorUtility.TryParseHtmlString($"#{te}", out Color col))
                {
                    config.TooEarlyColor = col;
                    onChange(config);
                }
                if (ve != config.HitMarginColorHex[1] && ColorUtility.TryParseHtmlString($"#{ve}", out col))
                {
                    config.VeryEarlyColor = col;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (ep, p) = MoreGUILayout.NamedTextFieldPair("Early Perfect Hex:", "Perfect Hex:", config.HitMarginColorHex[2], config.HitMarginColorHex[3], 100, 120);
                GUILayout.EndHorizontal();

                if (ep != config.HitMarginColorHex[2] && ColorUtility.TryParseHtmlString($"#{ep}", out col))
                {
                    config.EarlyPerfectColor = col;
                    onChange(config);
                }
                if (p != config.HitMarginColorHex[3] && ColorUtility.TryParseHtmlString($"#{p}", out col))
                {
                    config.PerfectColor = col;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (lp, vl) = MoreGUILayout.NamedTextFieldPair("Late Perfect Hex:", "Very Late Hex:", config.HitMarginColorHex[4], config.HitMarginColorHex[5], 100, 120);
                GUILayout.EndHorizontal();

                if (lp != config.HitMarginColorHex[4] && ColorUtility.TryParseHtmlString($"#{lp}", out col))
                {
                    config.LatePerfectColor = col;
                    onChange(config);
                }
                if (vl != config.HitMarginColorHex[5] && ColorUtility.TryParseHtmlString($"#{vl}", out col))
                {
                    config.VeryLateColor = col;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (tl, mp) = MoreGUILayout.NamedTextFieldPair("Too Late Hex:", "Multipress Hex:", config.HitMarginColorHex[6], config.HitMarginColorHex[7], 100, 120);
                GUILayout.EndHorizontal();

                if (tl != config.HitMarginColorHex[6] && ColorUtility.TryParseHtmlString($"#{tl}", out col))
                {
                    config.TooLateColor = col;
                    onChange(config);
                }
                if (mp != config.HitMarginColorHex[7] && ColorUtility.TryParseHtmlString($"#{mp}", out col))
                {
                    config.MultipressColor = col;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (fm, fo) = MoreGUILayout.NamedTextFieldPair("Fail Miss Hex:", "Fail Overload Hex:", config.HitMarginColorHex[8], config.HitMarginColorHex[9], 100, 120);
                GUILayout.EndHorizontal();

                if (fm != config.HitMarginColorHex[8] && ColorUtility.TryParseHtmlString($"#{fm}", out col))
                {
                    config.FailMissColor = col;
                    onChange(config);
                }
                if (fo != config.HitMarginColorHex[9] && ColorUtility.TryParseHtmlString($"#{fo}", out col))
                {
                    config.FailOverloadColor = col;
                    onChange(config);
                }
                MoreGUILayout.EndIndent();
            }
            if (changeBgColorJudge != config.ChangeBgColorJudge)
            {
                config.ChangeBgColorJudge = changeBgColorJudge;
                onChange(config);
            }
            Color newPressed, newReleased;
            string newPressedHex, newReleasedHex;
            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Lang.GetString("PRESSED_OUTLINE_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(Main.Lang.GetString("RELEASED_OUTLINE_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedOutlineColor, config.ReleasedOutlineColor);
            if (newPressed != config.PressedOutlineColor)
            {
                config.PressedOutlineColor = newPressed;
                onChange(config);
            }
            if (newReleased != config.ReleasedOutlineColor)
            {
                config.ReleasedOutlineColor = newReleased;
                onChange(config);
            }

            (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedOutlineColorHex, config.ReleasedOutlineColorHex, 100, 40);
            if (newPressedHex != config.PressedOutlineColorHex && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
            {
                config.PressedOutlineColor = newPressed;
                onChange(config);
            }
            if (newReleasedHex != config.ReleasedOutlineColorHex && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
            {
                config.ReleasedOutlineColor = newReleased;
                onChange(config);
            }
            MoreGUILayout.EndIndent();
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Lang.GetString("PRESSED_BACKGROUND_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(Main.Lang.GetString("RELEASED_BACKGROUND_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedBackgroundColor, config.ReleasedBackgroundColor);
            if (newPressed != config.PressedBackgroundColor)
            {
                config.PressedBackgroundColor = newPressed;
                onChange(config);
            }
            if (newReleased != config.ReleasedBackgroundColor)
            {
                config.ReleasedBackgroundColor = newReleased;
                onChange(config);
            }
            (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedBackgroundColorHex, config.ReleasedBackgroundColorHex, 100, 40);
            if (newPressedHex != config.PressedBackgroundColorHex && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
            {
                config.PressedBackgroundColor = newPressed;
                onChange(config);
            }
            if (newReleasedHex != config.ReleasedBackgroundColorHex && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
            {
                config.ReleasedBackgroundColor = newReleased;
                onChange(config);
            }
            MoreGUILayout.EndIndent();
            GUILayout.Space(8f);

            config.Gradient = GUILayout.Toggle(config.Gradient, "Gradient");

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Lang.GetString("PRESSED_TEXT_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(Main.Lang.GetString("RELEASED_TEXT_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            if (config.Gradient)
            {
                VertexGradient newPressedG, newReleasedG;
                (newPressedG, newReleasedG) = MoreGUILayout.VertexGradientSlidersPair(config.PressedTextColor, config.ReleasedTextColor);
                if (newPressedG.Inequals(config.PressedTextColor))
                {
                    config.PressedTextColor = newPressedG;
                    onChange(config);
                }
                if (newReleasedG.Inequals(config.ReleasedTextColor))
                {
                    config.ReleasedTextColor = newReleasedG;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Left Hex:", "Top Left Hex:", config.PressedTextColorHex[0], config.ReleasedTextColorHex[0], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(newPressed, orig.topRight, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(newReleased, orig.topRight, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Right Hex:", "Top Right Hex:", config.PressedTextColorHex[1], config.ReleasedTextColorHex[1], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, newPressed, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, newReleased, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Left Hex:", "Bottom Left Hex:", config.PressedTextColorHex[2], config.ReleasedTextColorHex[2], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newPressed, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newReleased, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Right Hex:", "Bottom Right Hex:", config.PressedTextColorHex[3], config.ReleasedTextColorHex[3], 100, 110);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newPressed);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newReleased);
                    onChange(config);
                }
            }
            else
            {
                (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedTextColor.topLeft, config.ReleasedTextColor.topLeft);
                if (newPressed != config.PressedTextColor.topLeft)
                {
                    config.PressedTextColor = new VertexGradient(newPressed);
                    onChange(config);
                }
                if (newReleased != config.ReleasedTextColor.topLeft)
                {
                    config.ReleasedTextColor = new VertexGradient(newReleased);
                    onChange(config);
                }

                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedTextColorHex[0], config.ReleasedTextColorHex[0], 100, 40);
                if (newPressedHex != config.PressedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    config.PressedTextColor = new VertexGradient(newPressed);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    config.ReleasedTextColor = new VertexGradient(newReleased);
                    onChange(config);
                }
            }
            MoreGUILayout.EndIndent();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Lang.GetString("PRESSED_COUNT_TEXT_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(Main.Lang.GetString("RELEASED_COUNT_TEXT_COLOR"), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            if (config.Gradient)
            {
                VertexGradient newPressedG, newReleasedG;
                (newPressedG, newReleasedG) = MoreGUILayout.VertexGradientSlidersPair(config.PressedCountTextColor, config.ReleasedCountTextColor);
                if (newPressedG.Inequals(config.PressedCountTextColor))
                {
                    config.PressedCountTextColor = newPressedG;
                    onChange(config);
                }
                if (newReleasedG.Inequals(config.ReleasedCountTextColor))
                {
                    config.ReleasedCountTextColor = newReleasedG;
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Left Hex:", "Top Left Hex:", config.PressedCountTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(newPressed, orig.topRight, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(newReleased, orig.topRight, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Top Right Hex:", "Top Right Hex:", config.PressedCountTextColorHex[1], config.ReleasedCountTextColorHex[1], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedCountTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, newPressed, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedCountTextColorHex[1] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, newReleased, orig.bottomLeft, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Left Hex:", "Bottom Left Hex:", config.PressedCountTextColorHex[2], config.ReleasedCountTextColorHex[2], 100, 100);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedCountTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newPressed, orig.bottomRight);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedCountTextColorHex[2] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, newReleased, orig.bottomRight);
                    onChange(config);
                }

                GUILayout.BeginHorizontal();
                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Bottom Right Hex:", "Bottom Right Hex:", config.PressedCountTextColorHex[3], config.ReleasedCountTextColorHex[3], 100, 110);
                GUILayout.EndHorizontal();

                if (newPressedHex != config.PressedCountTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    var orig = config.PressedTextColor;
                    config.PressedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newPressed);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedCountTextColorHex[3] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    var orig = config.ReleasedTextColor;
                    config.ReleasedTextColor = new VertexGradient(orig.topLeft, orig.topRight, orig.bottomLeft, newReleased);
                    onChange(config);
                }
            }
            else
            {
                (newPressed, newReleased) = MoreGUILayout.ColorRgbaSlidersPair(config.PressedCountTextColor.topLeft, config.ReleasedCountTextColor.topLeft);
                if (newPressed != config.PressedCountTextColor.topLeft)
                {
                    config.PressedCountTextColor = new VertexGradient(newPressed);
                    onChange(config);
                }
                if (newReleased != config.ReleasedCountTextColor.topLeft)
                {
                    config.ReleasedCountTextColor = new VertexGradient(newReleased);
                    onChange(config);
                }

                (newPressedHex, newReleasedHex) = MoreGUILayout.NamedTextFieldPair("Hex:", "Hex:", config.PressedCountTextColorHex[0], config.ReleasedCountTextColorHex[0], 100, 40);
                if (newPressedHex != config.PressedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
                {
                    config.PressedCountTextColor = new VertexGradient(newPressed);
                    onChange(config);
                }
                if (newReleasedHex != config.ReleasedCountTextColorHex[0] && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
                {
                    config.ReleasedCountTextColor = new VertexGradient(newReleased);
                    onChange(config);
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Main.Lang.GetString("RESET")))
            {
                config.Reset();
                onChange(config);
            }
            if (config.CanUndo)
            {
                if (GUILayout.Button(Main.Lang.GetString("UNDO")))
                {
                    if (config.Undo())
                        onChange(config);
                }
            }
            if (config.CanRedo)
            {
                if (GUILayout.Button(Main.Lang.GetString("REDO")))
                {
                    if (config.Redo())
                        onChange(config);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            MoreGUILayout.EndIndent();
            MoreGUILayout.EndIndent();
        }
    }
}
