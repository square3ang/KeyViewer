using DG.Tweening;
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
            kpsCalc = new KPSCalculator(manager.profile);
            transform.SetParent(manager.keysCanvas.transform);

            ObjectConfig bgConfig = config.BackgroundConfig;
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            Background = bgObj.AddComponent<Image>();
            Background.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyColor(Background, bgConfig.Color.Released);
            KeyViewerUtils.ApplyRoundness(Background, config.BackgroundRoundness);

            ObjectConfig olConfig = config.OutlineConfig;
            GameObject olObj = new GameObject("Outline");
            olObj.transform.SetParent(transform);
            Outline = olObj.AddComponent<Image>();
            Outline.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyColor(Outline, olConfig.Color.Released);
            KeyViewerUtils.ApplyRoundness(Outline, config.OutlineRoundness);

            ObjectConfig textConfig = config.TextConfig;
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            ContentSizeFitter textCsf = textObj.AddComponent<ContentSizeFitter>();
            textCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text = textObj.AddComponent<TextMeshProUGUI>();
            Text.font = FontManager.GetFont(config.Font).fontTMP;
            Text.enableVertexGradient = true;
            KeyViewerUtils.ApplyColor(Text, textConfig.Color.Released);
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
            KeyViewerUtils.ApplyColor(Text, cTextConfig.Color.Released);
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

            Background.sprite = AssetManager.Get(config.Background.Released, AssetManager.Background);
            Background.rectTransform.anchorMin = Vector2.zero;
            Background.rectTransform.anchorMax = Vector2.zero;
            Background.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Background.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Background.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfig(Background, config.BackgroundConfig);
            KeyViewerUtils.ApplyRoundness(Background, config.BackgroundRoundness);

            Outline.sprite = AssetManager.Get(config.Outline.Released, AssetManager.Outline);
            Outline.rectTransform.anchorMin = Vector2.zero;
            Outline.rectTransform.anchorMax = Vector2.zero;
            Outline.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Outline.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Outline.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfig(Outline, config.OutlineConfig);
            KeyViewerUtils.ApplyRoundness(Outline, config.OutlineRoundness);

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
            Text.fontSize = textConfig.VectorConfig.Size.Released;
            Text.fontSizeMax = textConfig.VectorConfig.Size.Released;
            Text.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfig(Text, textConfig);

            if (!string.IsNullOrEmpty(config.Text.Released))
            {
                Text.text = config.Text.Released;
                textReplacerR = new Replacer(config.Text.Released, manager.AllTags);
            }
            else
            {
                if (!Constants.KeyString.TryGetValue(config.Code, out string codeString))
                    codeString = KeyViewerUtils.KeyName(config);
                Text.text = codeString;
                textReplacerR = null;
            }
            textReplacerP = !string.IsNullOrEmpty(config.Text.Pressed) ? new Replacer(config.Text.Pressed, manager.AllTags) : null;

            ObjectConfig cTextConfig = config.CountTextConfig;
            CountText.rectTransform.anchorMin = Vector2.zero;
            CountText.rectTransform.anchorMax = Vector2.zero;
            CountText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            CountText.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 0.8f);
            releasedOffset = cTextConfig.VectorConfig.Offset.Released;
            CountText.rectTransform.anchoredPosition = anchoredPos + new Vector2(releasedOffset.x, releasedOffset.y - heightOffset);
            CountText.fontSizeMin = 0;
            CountText.fontSize = cTextConfig.VectorConfig.Size.Released;
            CountText.fontSizeMax = cTextConfig.VectorConfig.Size.Released;
            CountText.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfig(CountText, cTextConfig);

            if (!string.IsNullOrEmpty(config.CountText.Released))
            {
                CountText.text = config.CountText.Released;
                countTextReplacerR = new Replacer(config.CountText.Released, manager.AllTags);
            }
            else
            {
                CountText.text = $"{{Count:{KeyViewerUtils.KeyName(config)}}}";
                countTextReplacerR = new Replacer(CountText.text, manager.AllTags);
            }
            countTextReplacerP = !string.IsNullOrEmpty(config.CountText.Pressed) ? new Replacer(config.CountText.Pressed, manager.AllTags) : null;

            CountText.gameObject.SetActive(config.EnableCountText);

            x += keyWidth + 10;
            ReplaceText();
        }

        private void Update()
        {
            if (!initialized) return;
            Pressed = KeyInput.GetKey(config.Code);
            if (prevPressed == Pressed) return;
            prevPressed = Pressed;
            config.Count++;
            kpsCalc.Press();
            manager.kpsCalc.Press();
            ReplaceText();

        }
        private void ReplaceText()
        {
            if (string.IsNullOrEmpty(config.DummyName))
            {
                if (Pressed)
                {
                    if (textReplacerP != null)
                        Text.text = textReplacerP.Replace();
                    if (countTextReplacerP != null)
                        CountText.text = countTextReplacerP.Replace();
                }
                else
                {
                    if (textReplacerR != null)
                        Text.text = textReplacerR.Replace();
                    if (countTextReplacerR != null)
                        CountText.text = countTextReplacerR.Replace();
                }
            }
            else
            {
                if (textReplacerR != null)
                    Text.text = textReplacerR.Replace();
                if (countTextReplacerR != null)
                    CountText.text = countTextReplacerR.Replace();
            }
        }
    }
}
