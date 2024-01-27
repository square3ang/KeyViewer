using KeyViewer.API;
using KeyViewer.Core;
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
        private GameObject rainContainer;
        private RectMask2D rainMask;
        private int[] colorUpdateIgnores = new int[4];

        public KeyManager Manager;
        public bool Pressed;
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 DefaultSize;
        public KeyConfig Config;
        public Image Background;
        public Image Outline;
        public TextMeshProUGUI Text;
        public TextMeshProUGUI CountText;
        public KPSCalculator KpsCalc;
        public RectTransform RainMaskRt;
        public RainImageManager RainImageManager;

        internal Rain rain;
        internal EnsurePool<Rain> rainPool;

        public void Init(KeyManager manager, KeyConfig config)
        {
            if (initialized) return;
            Manager = manager;
            Config = config;
            textReplacerP = new Replacer(manager.AllTags);
            textReplacerR = new Replacer(manager.AllTags);
            countTextReplacerP = new Replacer(manager.AllTags);
            countTextReplacerR = new Replacer(manager.AllTags);
            KpsCalc = new KPSCalculator(manager.profile);
            RainImageManager = new RainImageManager(config.Rain);
            transform.SetParent(manager.keysCanvas.transform);

            rainContainer = new GameObject("Rain Container");
            var containerTransform = rainContainer.transform;
            containerTransform.position = transform.position;
            containerTransform.SetParent(transform);
            var image = rainContainer.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            rainMask = rainContainer.AddComponent<RectMask2D>();
            RainMaskRt = rainMask.rectTransform;
            var rainVecConfig = Config.Rain.ObjectConfig.VectorConfig;
            KeyViewerUtils.SetMaskAnchor(RainMaskRt, Config.Rain.Direction, rainVecConfig.Pivot, rainVecConfig.Anchor);

            rainPool = new EnsurePool<Rain>(() =>
            {
                GameObject rainObj = new GameObject($"Rain {Config.Code}");
                rainObj.transform.SetParent(rainMask.transform);
                var rain = rainObj.AddComponent<Rain>();
                rain.Init(this);
                rainObj.SetActive(false);
                return rain;
            }, kr => !kr.IsAlive, kr => kr.gameObject.SetActive(true), r => Destroy(r.gameObject));
            if (Config.RainEnabled) rainPool.Fill(Config.Rain.PoolSize);

            ObjectConfig bgConfig = config.BackgroundConfig;
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            Background = bgObj.AddComponent<Image>();
            Background.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyRoundnessBlurLayout(Background, ref config.BackgroundRoundness, config.BackgroundBlurConfig, config.BackgroundBlurEnabled);
            KeyViewerUtils.ApplyColorLayout(Background, bgConfig.Color.Released, config.BackgroundBlurEnabled);

            ObjectConfig olConfig = config.OutlineConfig;
            GameObject olObj = new GameObject("Outline");
            olObj.transform.SetParent(transform);
            Outline = olObj.AddComponent<Image>();
            Outline.type = Image.Type.Sliced;
            KeyViewerUtils.ApplyRoundnessBlurLayout(Outline, ref config.OutlineRoundness, null, false);
            KeyViewerUtils.ApplyColorLayout(Outline, olConfig.Color.Released, false);

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

            float defaultX = 100, defaultY = Config.EnableCountText ? 150 : 100;
            DefaultSize = new Vector2(defaultX, defaultY);

            VectorConfig vConfig = Config.VectorConfig;
            float keyWidth = vConfig.Scale.Released.x * 100;
            float keyHeight = Config.EnableCountText ? 150 : 100;
            keyHeight *= vConfig.Scale.Released.y;
            Size = new Vector2(keyWidth, keyHeight);
            float _x = Config.DisableSorting ? 0 : x + keyWidth / 2;
            Position = new Vector2(_x, 0) - Manager.centerOffset;
            Position += KeyViewerUtils.InjectPivot(this, KeyViewerUtils.GetPivot(vConfig.Pivot));
            KeyViewerUtils.ApplyConfigLayout(this, vConfig);

            Background.sprite = AssetManager.Get(Config.Background.Released, AssetManager.Background);
            Background.rectTransform.SetAnchor(Config.BackgroundConfig.VectorConfig.Anchor);
            Background.rectTransform.pivot = KeyViewerUtils.GetPivot(Config.BackgroundConfig.VectorConfig.Pivot);
            Background.rectTransform.sizeDelta = DefaultSize;
            KeyViewerUtils.ApplyRoundnessBlurLayout(Background, ref Config.BackgroundRoundness, Config.BackgroundBlurConfig, Config.BackgroundBlurEnabled);
            KeyViewerUtils.ApplyConfigLayout(Background, Config.BackgroundConfig, DefaultSize, Config.BackgroundBlurEnabled);

            Outline.sprite = AssetManager.Get(Config.Outline.Released, AssetManager.Outline);
            Outline.rectTransform.SetAnchor(Config.OutlineConfig.VectorConfig.Anchor);
            Outline.rectTransform.pivot = KeyViewerUtils.GetPivot(Config.OutlineConfig.VectorConfig.Pivot);
            Outline.rectTransform.sizeDelta = DefaultSize;
            KeyViewerUtils.ApplyRoundnessBlurLayout(Outline, ref Config.OutlineRoundness, null, false);
            KeyViewerUtils.ApplyConfigLayout(Outline, Config.OutlineConfig, DefaultSize, false);

            float heightOffset = defaultY / 4f;
            ObjectConfig textConfig = Config.TextConfig;
            Text.rectTransform.SetAnchor(Config.TextConfig.VectorConfig.Anchor);
            Text.rectTransform.pivot = KeyViewerUtils.GetPivot(Config.TextConfig.VectorConfig.Pivot);
            Text.rectTransform.sizeDelta = DefaultSize;
            Text.fontSize = Config.TextFontSize;
            Text.fontSizeMax = Config.TextFontSize;
            Text.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfigLayout(Text, textConfig, Config.EnableCountText ? heightOffset : 0, Config.DoNotScaleText);

            var defaultSource = Constants.KeyString.TryGetValue(Config.Code, out string codeStr) ? codeStr : KeyViewerUtils.KeyName(Config);
            textReplacerP.Source = string.IsNullOrEmpty(Config.Text.Pressed) ? defaultSource : Config.Text.Pressed;
            textReplacerR.Source = string.IsNullOrEmpty(Config.Text.Released) ? defaultSource : Config.Text.Released;
            Text.text = textReplacerR.Source;

            ObjectConfig cTextConfig = Config.CountTextConfig;
            CountText.rectTransform.SetAnchor(Config.CountTextConfig.VectorConfig.Anchor);
            CountText.rectTransform.pivot = KeyViewerUtils.GetPivot(Config.CountTextConfig.VectorConfig.Pivot);
            CountText.rectTransform.sizeDelta = DefaultSize;
            CountText.fontSizeMin = 0;
            CountText.fontSize = Config.CountTextFontSize;
            CountText.fontSizeMax = Config.CountTextFontSize;
            CountText.enableAutoSizing = true;
            KeyViewerUtils.ApplyConfigLayout(CountText, cTextConfig, Config.EnableCountText ? -heightOffset : 0, Config.DoNotScaleText);

            countTextReplacerP.Source = string.IsNullOrEmpty(Config.CountText.Pressed) ? $"{{Count:{KeyViewerUtils.KeyName(Config)}}}" : Config.CountText.Pressed;
            countTextReplacerR.Source = string.IsNullOrEmpty(Config.CountText.Released) ? $"{{Count:{KeyViewerUtils.KeyName(Config)}}}" : Config.CountText.Released;
            CountText.text = countTextReplacerR.Source;

            Outline.gameObject.SetActive(Config.EnableOutlineImage);
            CountText.gameObject.SetActive(Config.EnableCountText);

            RainImageManager.Refresh();
            var rainConfig = Config.Rain;
            rainMask.softness = GetSoftness(rainConfig.Direction);
            if (Config.RainEnabled)
            {
                rainPool.Clear();
                rainPool.Fill(Config.Rain.PoolSize);
            }
            var rainVecConfig = rainConfig.ObjectConfig.VectorConfig;
            KeyViewerUtils.SetMaskAnchor(RainMaskRt, rainConfig.Direction, rainVecConfig.Pivot, rainVecConfig.Anchor);
            RainUpdate();
            rainContainer.SetActive(Config.RainEnabled);

            if (!Config.DisableSorting)
                x += keyWidth + Manager.profile.KeySpacing;
            ReplaceText();
        }
        public void ResetRains()
        {
            if (!Config.RainEnabled) return;
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
                    return scale.y > 0 ?
                        new Vector2(rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed), Size.y * scale.y) :
                        new Vector2(rConfig.Softness.Get(Pressed) + rConfig.Length.Get(Pressed), Size.y);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetMaskPosition(Direction dir)
        {
            var rainVConfig = Config.Rain.ObjectConfig.VectorConfig;
            Vector2 vec = transform.position + rainVConfig.Offset.Get(Pressed);
            float x = Size.x, y = Size.y;
            Vector2 offset = rainVConfig.Offset.Get(Pressed);
            int softness = Config.Rain.Softness.Get(Pressed);
            float spacing = Manager.profile.KeySpacing;
            switch (dir)
            {
                case Direction.Up:
                    return new Vector2(vec.x + offset.x, vec.y + (y / 2 - softness) + spacing + offset.y);
                case Direction.Down:
                    return new Vector2(vec.x + offset.x, vec.y - (y / 2 - softness) - spacing + offset.y);
                case Direction.Left:
                    return new Vector2(vec.x + offset.x - (x / 2 - softness) - spacing, vec.y + offset.y);
                case Direction.Right:
                    return new Vector2(vec.x + offset.x + (x / 2 - softness) + spacing, vec.y + offset.y);
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
            else Pressed = Input.GetKey(Config.Code);
            if (prevPressed == Pressed) return;
            prevPressed = Pressed;
            if (Pressed)
            {
                if (InputAPI.EventActive)
                    InputAPI.KeyPress(Config.Code);
                Config.Count++;
                if (Config.EnableKPSMeter)
                    KpsCalc.Press();
                Manager.kpsCalc.Press();
            }
            else if (InputAPI.EventActive)
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
                KeyViewerUtils.ApplyColor(Background, bgColor.Get(!Pressed), bgColor.Get(Pressed), bgColor.GetEase(Pressed), Config.BackgroundBlurEnabled);
            }
            else colorUpdateIgnores[(int)Element.Background]--;

            if (colorUpdateIgnores[(int)Element.Outline] == 0)
            {
                var olColor = Config.OutlineConfig.Color;
                KeyViewerUtils.ApplyColor(Outline, olColor.Get(!Pressed), olColor.Get(Pressed), olColor.GetEase(Pressed), false);
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
            float heightOffset = Config.EnableCountText ? DefaultSize.y / 4f : 0;
            KeyViewerUtils.ApplyVectorConfig(this, Config.VectorConfig, Pressed);
            KeyViewerUtils.ApplyVectorConfig(Text.rectTransform, Config.TextConfig.VectorConfig, Pressed, heightOffset, Config.DoNotScaleText, DefaultSize, false);
            KeyViewerUtils.ApplyVectorConfig(CountText.rectTransform, Config.CountTextConfig.VectorConfig, Pressed, -heightOffset, Config.DoNotScaleText, DefaultSize, false);
            KeyViewerUtils.ApplyVectorConfig(Background.rectTransform, Config.BackgroundConfig.VectorConfig, Pressed, 0, false, DefaultSize);
            KeyViewerUtils.ApplyVectorConfig(Outline.rectTransform, Config.OutlineConfig.VectorConfig, Pressed, 0, false, DefaultSize);
        }
        private void RainUpdate()
        {
            if (!Config.RainEnabled) return;
            var rainConfig = Config.Rain;
            rainMask.softness = GetSoftness(rainConfig.Direction);
            RainMaskRt.sizeDelta = GetSizeDelta(rainConfig.Direction);
            RainMaskRt.position = GetMaskPosition(rainConfig.Direction);
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
