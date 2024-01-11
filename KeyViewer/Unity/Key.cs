using KeyViewer.API;
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
        private KeyManager manager;
        private Replacer textReplacerP;
        private Replacer textReplacerR;
        private Replacer countTextReplacerP;
        private Replacer countTextReplacerR;
        private GameObject rainContainer;
        private RectMask2D rainMask;
        private EnsurePool<Rain> rainPool;
        private Vector3 initialPosition;
        private int[] colorUpdateIgnores = new int[4];

        public bool Pressed;
        public Vector2 Size;
        public Vector2 Position;
        public KeyConfig Config;
        public Image Background;
        public Image Outline;
        public TextMeshProUGUI Text;
        public TextMeshProUGUI CountText;
        public KPSCalculator KpsCalc;
        public RectTransform RainMaskRt;
        public RainImageManager RainImageManager;

        internal Rain rain;

        public void Init(KeyManager manager, KeyConfig config)
        {
            if (initialized) return;
            this.manager = manager;
            Config = config;
            textReplacerP = new Replacer(manager.AllTags);
            textReplacerR = new Replacer(manager.AllTags);
            countTextReplacerP = new Replacer(manager.AllTags);
            countTextReplacerR = new Replacer(manager.AllTags);
            KpsCalc = new KPSCalculator(manager.profile);
            RainImageManager = new RainImageManager(config.Rain);
            transform.SetParent(manager.keysCanvas.transform);
            initialPosition = transform.localPosition;
            Config.VectorConfig.anchorCache = initialPosition;

            rainContainer = new GameObject("Rain Container");
            var containerTransform = rainContainer.transform;
            containerTransform.position = transform.position;
            containerTransform.SetParent(transform);
            var image = rainContainer.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            rainMask = rainContainer.AddComponent<RectMask2D>();
            RainMaskRt = rainMask.rectTransform;
            RainMaskRt.anchorMin = RainMaskRt.anchorMax = Vector2.zero;
            rainPool = new EnsurePool<Rain>(() =>
            {
                GameObject rainObj = new GameObject($"Rain {config.Code}");
                rainObj.transform.SetParent(rainMask.transform);
                var rain = rainObj.AddComponent<Rain>();
                rain.Init(this);
                rainObj.SetActive(false);
                return rain;
            }, kr => !kr.IsAlive, kr => kr.gameObject.SetActive(true), Destroy);
            rainPool.Fill(Config.Rain.PoolSize);

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
                KpsCalc.Start();

            initialized = true;
        }
        public void UpdateLayout(ref float x)
        {
            Pressed = false;
            if (FontManager.TryGetFont(Config.Font, out var font))
            {
                Text.font = font.fontTMP;
                CountText.font = font.fontTMP;
            }

            VectorConfig vConfig = Config.VectorConfig;
            float keyWidth = vConfig.Scale.Released.x * 100, keyHeight = (Config.EnableCountText ? 50 : 0) + 100;
            keyHeight *= vConfig.Scale.Released.y;
            Size = new Vector2(keyWidth, keyHeight);
            float _x = Config.DisableSorting ? 0 : x;
            Vector2 position = new Vector2(keyWidth / 2f + _x, keyHeight / 2f);
            Vector2 anchoredPos = Position = position + vConfig.Offset.Released - manager.centerOffset, releasedOffset;
            KeyViewerUtils.ApplyConfigLayout(this, Config.VectorConfig);

            Background.sprite = AssetManager.Get(Config.Background.Released, AssetManager.Background);
            Background.rectTransform.anchorMin = Vector2.zero;
            Background.rectTransform.anchorMax = Vector2.zero;
            Background.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Background.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Background.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfigLayout(Background, Config.BackgroundConfig);
            KeyViewerUtils.ApplyRoundnessLayout(Background, Config.BackgroundRoundness);

            Outline.sprite = AssetManager.Get(Config.Outline.Released, AssetManager.Outline);
            Outline.rectTransform.anchorMin = Vector2.zero;
            Outline.rectTransform.anchorMax = Vector2.zero;
            Outline.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Outline.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
            Outline.rectTransform.anchoredPosition = anchoredPos;
            KeyViewerUtils.ApplyConfigLayout(Outline, Config.OutlineConfig);
            KeyViewerUtils.ApplyRoundnessLayout(Outline, Config.OutlineRoundness);

            ObjectConfig textConfig = Config.TextConfig;
            float heightOffset = keyHeight / 4f;
            Text.rectTransform.anchorMin = Vector2.zero;
            Text.rectTransform.anchorMax = Vector2.zero;
            Text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            Text.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 1.03f);
            releasedOffset = textConfig.VectorConfig.Offset.Released;
            if (Config.EnableCountText)
                Text.rectTransform.anchoredPosition = anchoredPos + new Vector2(releasedOffset.x, releasedOffset.y + heightOffset);
            else Text.rectTransform.anchoredPosition = anchoredPos + releasedOffset;
            Text.fontSize = 75;
            Text.fontSizeMax = 75;
            Text.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfigLayout(Text, textConfig);

            var defaultSource = Constants.KeyString.TryGetValue(Config.Code, out string codeStr) ? codeStr : KeyViewerUtils.KeyName(Config);
            textReplacerP.Source = string.IsNullOrEmpty(Config.Text.Pressed) ? defaultSource : Config.Text.Pressed;
            textReplacerR.Source = string.IsNullOrEmpty(Config.Text.Released) ? defaultSource : Config.Text.Released;
            Text.text = textReplacerR.Source;

            ObjectConfig cTextConfig = Config.CountTextConfig;
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

            countTextReplacerP.Source = string.IsNullOrEmpty(Config.CountText.Pressed) ? $"{{Count:{KeyViewerUtils.KeyName(Config)}}}" : Config.CountText.Pressed;
            countTextReplacerR.Source = string.IsNullOrEmpty(Config.CountText.Released) ? $"{{Count:{KeyViewerUtils.KeyName(Config)}}}" : Config.CountText.Released;
            CountText.text = countTextReplacerR.Source;

            Outline.gameObject.SetActive(Config.EnableOutlineImage);
            CountText.gameObject.SetActive(Config.EnableCountText);

            RainImageManager.Refresh();
            var rainConfig = Config.Rain;
            rainMask.softness = GetSoftness(rainConfig.Direction);
            rainPool.Clear();
            rainPool.Fill(Config.Rain.PoolSize);
            KeyViewerUtils.SetAnchor(RainMaskRt, rainConfig.Direction);
            RainUpdate();
            rainContainer.SetActive(Config.RainEnabled);

            if (!Config.DisableSorting)
                x += keyWidth + 10;
            ReplaceText();
        }
        public void ResetRains()
        {
            rainPool.ForEach(r =>
            {
                r.Release();
                r.OnEnable();
                r.gameObject.SetActive(false);
            });
        }
        public void IgnoreColorUpdate(Element e)
        {
            colorUpdateIgnores[(int)e]++;
        }

        #region Privates

        #region Rain
        private Vector2 GetSizeDelta(Direction dir)
        {
            var rConfig = Config.Rain;
            var scale = rConfig.ObjectConfig.VectorConfig.Scale.Get(Pressed);
            switch (dir)
            {
                case Direction.Up:
                case Direction.Down:
                    return scale.x > 0 ?
                        new Vector2(Size.x * scale.x, rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed)) :
                        new Vector2(Size.x, rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed));
                case Direction.Left:
                case Direction.Right:
                    var yOffset = (Config.EnableCountText ? 50 : 0) + 5;
                    return scale.y > 0 ?
                        new Vector2(rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed), Size.y * scale.y + yOffset) :
                        new Vector2(rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed), Size.y + yOffset);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetMaskPosition(Direction dir)
        {
            Vector2 vec = Position + Config.VectorConfig.Offset.Get(Pressed);
            float x = Size.x, y = Size.y + (Config.EnableCountText ? 50 : 0);
            Vector2 offset = Config.Rain.ObjectConfig.VectorConfig.Offset.Get(Pressed);
            int softness = Config.Rain.Softness.Get(Pressed);
            switch (dir)
            {
                case Direction.Up:
                    return new Vector2(vec.x + offset.x, vec.y + (y / 2 - softness) + 10 + offset.y);
                case Direction.Down:
                    return new Vector2(vec.x + offset.x, vec.y - (y / 2 - softness) - 10 + offset.y);
                case Direction.Left:
                    return new Vector2(vec.x + offset.x - (x / 2 - softness) - 10, vec.y + offset.y);
                case Direction.Right:
                    return new Vector2(vec.x + offset.x + (x / 2 - softness) + 10, vec.y + offset.y);
                default: return Vector2.zero;
            }
        }
        private Vector2Int GetSoftness(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                case Direction.Down:
                    return new Vector2Int(0, Config.Rain.Softness.Get(Pressed));
                case Direction.Left:
                case Direction.Right:
                    return new Vector2Int(Config.Rain.Softness.Get(Pressed), 0);
                default: return Vector2Int.zero;
            }
        }
        #endregion

        #region Update
        private void Update()
        {
            if (!initialized) return;
            if (Config.UpdateTextAlways) ReplaceText();
            if (!string.IsNullOrEmpty(Config.DummyName)) return;
            if (InputAPI.Active)
                Pressed = InputAPI.APIFlags.TryGetValue(Config.Code, out var p) ? p : false;
            else Pressed = KeyInput.GetKey(Config.Code);
            if (prevPressed == Pressed) return;
            prevPressed = Pressed;
            if (Pressed)
            {
                if (InputAPI.Active)
                    InputAPI.KeyPress(Config.Code);
                Config.Count++;
                if (Config.EnableKPSMeter)
                    KpsCalc.Press();
                manager.kpsCalc.Press();
            }
            else if (InputAPI.Active)
                InputAPI.KeyRelease(Config.Code);
            if (!Config.UpdateTextAlways)
                ReplaceText();

            RainUpdate();
            ApplyColor();
            ApplySprite();
            ApplyVectorConfig();
        }
        private void ReplaceText()
        {
            if (string.IsNullOrEmpty(Config.DummyName))
            {
                if (Pressed)
                {
                    Text.text = textReplacerP.Replace();
                    if (Config.EnableCountText)
                        CountText.text = countTextReplacerP.Replace();
                }
                else
                {
                    Text.text = textReplacerR.Replace();
                    if (Config.EnableCountText)
                        CountText.text = countTextReplacerR.Replace();
                }
            }
            else
            {
                Text.text = textReplacerR.Replace();
                if (Config.EnableCountText)
                    CountText.text = countTextReplacerR.Replace();
            }
        }
        private void ApplyColor()
        {
            if (colorUpdateIgnores[(int)Element.Text] == 0)
            {
                var textColor = Config.TextConfig.Color;
                KeyViewerUtils.ApplyColor(Text, textColor.Get(!Pressed), textColor.Get(Pressed), textColor.GetEase(Pressed));
            }
            else colorUpdateIgnores[(int)Element.Text]--;

            if (colorUpdateIgnores[(int)Element.CountText] == 0)
            {
                var countTextColor = Config.CountTextConfig.Color;
                KeyViewerUtils.ApplyColor(CountText, countTextColor.Get(!Pressed), countTextColor.Get(Pressed), countTextColor.GetEase(Pressed));
            }
            else colorUpdateIgnores[(int)Element.CountText]--;

            if (colorUpdateIgnores[(int)Element.Background] == 0)
            {
                var bgColor = Config.BackgroundConfig.Color;
                KeyViewerUtils.ApplyColor(Background, bgColor.Get(!Pressed), bgColor.Get(Pressed), bgColor.GetEase(Pressed));
            }
            else colorUpdateIgnores[(int)Element.Background]--;

            if (colorUpdateIgnores[(int)Element.Outline] == 0)
            {
                var olColor = Config.OutlineConfig.Color;
                KeyViewerUtils.ApplyColor(Outline, olColor.Get(!Pressed), olColor.Get(Pressed), olColor.GetEase(Pressed));
            }
            else colorUpdateIgnores[(int)Element.Outline]--;
        }
        private void ApplySprite()
        {
            Background.sprite = AssetManager.Get(Config.Background.Get(Pressed), AssetManager.Background);
            Outline.sprite = AssetManager.Get(Config.Outline.Get(Pressed), AssetManager.Outline);
        }
        private void ApplyVectorConfig()
        {
            KeyViewerUtils.ApplyVectorConfig(transform, Config.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Text.rectTransform, Config.TextConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(CountText.rectTransform, Config.CountTextConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Background.rectTransform, Config.BackgroundConfig.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Outline.rectTransform, Config.OutlineConfig.VectorConfig, Pressed);
        }
        private void RainUpdate()
        {
            if (!Config.RainEnabled) return;
            var rainConfig = Config.Rain;
            rainMask.softness = GetSoftness(rainConfig.Direction);
            RainMaskRt.sizeDelta = GetSizeDelta(rainConfig.Direction);
            RainMaskRt.anchoredPosition = GetMaskPosition(rainConfig.Direction);
            if (Pressed)
            {
                rain = rainPool.Get();
                rain.Press();
            }
            else if (rain != null)
            {
                rain.Release();
            }
        }
        #endregion

        #endregion

        public enum Element
        {
            Background,
            Outline,
            Text,
            CountText
        }
    }
}
