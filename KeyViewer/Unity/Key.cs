using KeyViewer.Core;
using KeyViewer.Core.Input;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Models;
using KeyViewer.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity
{
    public class Key : MonoBehaviour
    {
        private bool initialized;
        private bool prevPressed;
        private Replacer textReplacerP;
        private Replacer textReplacerR;
        private Replacer countTextReplacerP;
        private Replacer countTextReplacerR;
        internal KPSCalculator kpsCalc;

        public bool Pressed;
        public KeyManager manager;
        public KeyConfig config;
        public Image Background;
        public Image Outline;
        public TextMeshProUGUI Text;
        public TextMeshProUGUI CountText;

        public void Init(KeyManager manager, KeyConfig config)
        {
            if (initialized) return;
            this.manager = manager;
            this.config = config;
            textReplacerP = new Replacer(manager.AllTags);
            textReplacerR = new Replacer(manager.AllTags);
            countTextReplacerP = new Replacer(manager.AllTags);
            countTextReplacerR = new Replacer(manager.AllTags);
            kpsCalc = new KPSCalculator(manager.profile);
            transform.SetParent(manager.keysCanvas.transform);

            ObjectConfig bgConfig = config.BackgroundConfig;
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            Background = bgObj.AddComponent<Image>();
            Background.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyColorLayout(Background, bgConfig.Color.Released);
            KeyViewerUtils.ApplyRoundnessLayout(Background, config.BackgroundRoundness);

            ObjectConfig olConfig = config.OutlineConfig;
            GameObject olObj = new GameObject("Outline");
            olObj.transform.SetParent(transform);
            Outline = olObj.AddComponent<Image>();
            Outline.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyColorLayout(Outline, olConfig.Color.Released);
            KeyViewerUtils.ApplyRoundnessLayout(Outline, config.OutlineRoundness);

            ObjectConfig textConfig = config.TextConfig;
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            ContentSizeFitter textCsf = textObj.AddComponent<ContentSizeFitter>();
            textCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text = textObj.AddComponent<TextMeshProUGUI>();
            Text.font = FontManager.GetFont(config.Font).fontTMP;
            Text.enableVertexGradient = true;
            KeyViewerUtils.ApplyColorLayout(Text, textConfig.Color.Released);
            Text.alignment = TextAlignmentOptions.Midline;

            ObjectConfig cTextConfig = config.CountTextConfig;
            GameObject countTextObj = new GameObject("CountText");
            countTextObj.transform.SetParent(transform);
            ContentSizeFitter countTextCsf = countTextObj.AddComponent<ContentSizeFitter>();
            countTextCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            countTextCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            CountText = countTextObj.AddComponent<TextMeshProUGUI>();
            CountText.font = FontManager.GetFont(config.Font).fontTMP;
            CountText.enableVertexGradient = true;
            KeyViewerUtils.ApplyColorLayout(CountText, cTextConfig.Color.Released);
            CountText.alignment = TextAlignmentOptions.Midline;

            if (config.EnableKPSMeter)
                kpsCalc.Start();

            initialized = true;
        }
        public void UpdateLayout(ref float x)
        {
            if (FontManager.TryGetFont(config.Font, out var font))
            {
                Text.font = font.fontTMP;
                CountText.font = font.fontTMP;
            }

            VectorConfig vConfig = config.VectorConfig;
            float keyWidth = vConfig.Scale.Released.x * 100, keyHeight = vConfig.Scale.Released.y * 100;
            if (config.EnableCountText)
                keyHeight += 50;
            Vector2 position = new Vector2(keyWidth / 2f + x, keyHeight / 2f);
            Vector2 anchoredPos = position + vConfig.Offset.Released;
            Vector2 releasedOffset;
            transform.localRotation = Quaternion.Euler(vConfig.Rotation.Released);

            Background.sprite = AssetManager.Get(config.Background.Released, AssetManager.Background);
            Background.rectTransform.anchorMin = Vector2.zero;
            Background.rectTransform.anchorMax = Vector2.zero;
            Background.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Background.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Background.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfigLayout(Background, config.BackgroundConfig);
            KeyViewerUtils.ApplyRoundnessLayout(Background, config.BackgroundRoundness);

            Outline.sprite = AssetManager.Get(config.Outline.Released, AssetManager.Outline);
            Outline.rectTransform.anchorMin = Vector2.zero;
            Outline.rectTransform.anchorMax = Vector2.zero;
            Outline.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Outline.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Outline.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfigLayout(Outline, config.OutlineConfig);
            KeyViewerUtils.ApplyRoundnessLayout(Outline, config.OutlineRoundness);

            ObjectConfig textConfig = config.TextConfig;
            float heightOffset = keyHeight / 4f;
            Text.rectTransform.anchorMin = Vector2.zero;
            Text.rectTransform.anchorMax = Vector2.zero;
            Text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Text.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 1.03f);
            releasedOffset = textConfig.VectorConfig.Offset.Released;
            if (config.EnableCountText)
                Text.rectTransform.anchoredPosition = anchoredPos + new Vector2(releasedOffset.x, releasedOffset.y + heightOffset);
            else Text.rectTransform.anchoredPosition = anchoredPos + releasedOffset;
            Text.fontSize = 75;
            Text.fontSizeMax = 75;
            Text.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfigLayout(Text, textConfig);

            var defaultSource = Constants.KeyString.TryGetValue(config.Code, out string codeStr) ? codeStr : KeyViewerUtils.KeyName(config);
            textReplacerP.Source = string.IsNullOrEmpty(config.Text.Pressed) ? defaultSource : config.Text.Pressed;
            textReplacerR.Source = string.IsNullOrEmpty(config.Text.Released) ? defaultSource : config.Text.Released;
            Text.text = textReplacerR.Source;

            ObjectConfig cTextConfig = config.CountTextConfig;
            CountText.rectTransform.anchorMin = Vector2.zero;
            CountText.rectTransform.anchorMax = Vector2.zero;
            CountText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            CountText.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 0.8f);
            releasedOffset = cTextConfig.VectorConfig.Offset.Released;
            CountText.rectTransform.anchoredPosition = anchoredPos + new Vector2(releasedOffset.x, releasedOffset.y - heightOffset);
            CountText.fontSizeMin = 0;
            CountText.fontSize = 50;
            CountText.fontSizeMax = 50;
            CountText.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfigLayout(CountText, cTextConfig);

            countTextReplacerP.Source = string.IsNullOrEmpty(config.CountText.Pressed) ? $"{{Count:{KeyViewerUtils.KeyName(config)}}}" : config.CountText.Pressed;
            countTextReplacerR.Source = string.IsNullOrEmpty(config.CountText.Released) ? $"{{Count:{KeyViewerUtils.KeyName(config)}}}" : config.CountText.Released;
            CountText.text = countTextReplacerR.Source;

            CountText.gameObject.SetActive(config.EnableCountText);

            x += keyWidth + 10;
            ReplaceText();
        }

        private void Update()
        {
            if (!initialized) return;
            if (config.UpdateTextAlways) ReplaceText();
            Pressed = KeyInput.GetKey(config.Code);
            if (prevPressed == Pressed) return;
            prevPressed = Pressed;
            if (Pressed)
            {
                config.Count++;
                kpsCalc.Press();
                manager.kpsCalc.Press();
            }
            if (!config.UpdateTextAlways)
                ReplaceText();
            ApplyColor();
            ApplyVectorConfig();
        }
        private void ReplaceText()
        {
            if (string.IsNullOrEmpty(config.DummyName))
            {
                if (Pressed)
                {
                    Text.text = textReplacerP.Replace();
                    if (config.EnableCountText)
                        CountText.text = countTextReplacerP.Replace();
                }
                else
                {
                    Text.text = textReplacerR.Replace();
                    if (config.EnableCountText)
                        CountText.text = countTextReplacerR.Replace();
                }
            }
            else
            {
                Text.text = textReplacerR.Replace();
                if (config.EnableCountText)
                    CountText.text = countTextReplacerR.Replace();
            }
        }
        private void ApplyColor()
        {
            var textColor = config.TextConfig.Color;
            KeyViewerUtils.ApplyColor(Text, textColor.Get(!Pressed), textColor.Get(Pressed), textColor.GetEase(Pressed));
            var countTextColor = config.CountTextConfig.Color;
            KeyViewerUtils.ApplyColor(CountText, countTextColor.Get(!Pressed), countTextColor.Get(Pressed), countTextColor.GetEase(Pressed));
            var bgColor = config.BackgroundConfig.Color;
            KeyViewerUtils.ApplyColor(Background, bgColor.Get(!Pressed), bgColor.Get(Pressed), bgColor.GetEase(Pressed));
            var olColor = config.OutlineConfig.Color;
            KeyViewerUtils.ApplyColor(Outline, olColor.Get(!Pressed), olColor.Get(Pressed), olColor.GetEase(Pressed));
        }
        private void ApplyVectorConfig()
        {
            KeyViewerUtils.ApplyVectorConfig(transform, config.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Text.rectTransform, config.TextConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(CountText.rectTransform, config.CountTextConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Background.rectTransform, config.BackgroundConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Outline.rectTransform, config.OutlineConfig.VectorConfig, Pressed);
        }
    }
}
