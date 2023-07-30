using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityModManagerNet.UnityModManager;

namespace KeyViewer
{
    public partial class KeyRain : MonoBehaviour
    {
        public bool IsAlive { get; private set; }
        private bool stretching = false;
        private RainConfig config => key.config.RainConfig;
        internal Image image;
        private Key key;
        private RectTransform rt;
        void Awake()
        {
            image = gameObject.AddComponent<Image>();
            rt = image.rectTransform;
            rt.anchoredPosition = Vector2.zero;
        }
        public void Init(Key key)
        {
            this.key = key;
            if (File.Exists(config.RainImage))
                image.sprite = Main.GetSprite(config.RainImage);
            image.color = config.RainColor;
            ResetSizePos();
        }
        public void Press()
        {
            stretching = true;
        }
        public void ResetSizePos()
        {
            rt.sizeDelta = GetInitialSize();
            rt.anchoredPosition = GetPosition(config.Direction);
        }
        public void Release()
        {
            stretching = false;
        }
        private void Update()
        {
            IsAlive = IsVisible(config.Direction);
            if (IsAlive)
            {
                var toMove = Time.deltaTime * config.RainSpeed;
                var delta = GetDelta(config.Direction, toMove);
                var sizeDelta = rt.sizeDelta;
                if (stretching)
                {
                    var vec = Vector2.zero;
                    if (delta.x != 0) vec.x += Mathf.Abs(delta.x);
                    else if (delta.y != 0) vec.y += Mathf.Abs(delta.y);
                    sizeDelta += vec;
                    rt.sizeDelta = sizeDelta;
                    rt.anchoredPosition += delta * 0.5f;
                }
                else rt.anchoredPosition += delta;
            }
            else
            {
                stretching = false;
                ResetSizePos();
                gameObject.SetActive(false);
            }
        }
        internal static bool DrawConfigGUI(KeyCode code, RainConfig config)
        {
            bool changed = false;
            var newX = MoreGUILayout.NamedSlider("Offset X", config.OffsetX, -100, 100, 300);
            if (changed |= newX != config.OffsetX) config.OffsetX = newX;
            var newY = MoreGUILayout.NamedSlider("Offset Y", config.OffsetY, -100, 100, 300);
            if (changed |= newY != config.OffsetY) config.OffsetY = newY;
            var newSpeed = MoreGUILayout.NamedSlider("Rain Speed", config.RainSpeed, 0, 1000, 300);
            if (changed |= newSpeed != config.RainSpeed) config.RainSpeed = newSpeed;
            var newLength = MoreGUILayout.NamedSlider("Rain Length", config.RainLength, 0, 1000, 300);
            if (changed |= newLength != config.RainLength) config.RainLength = newLength;
            var newWidth = MoreGUILayout.NamedSlider("Rain Width", config.RainWidth, -1, 1000, 300);
            if (changed |= newWidth != config.RainWidth) config.RainWidth = newWidth;
            var newHeight = MoreGUILayout.NamedSlider("Rain Height", config.RainHeight, -1, 1000, 300);
            if (changed |= newHeight != config.RainHeight) config.RainHeight = newHeight;
            var newPoolSize = MoreGUILayout.NamedSlider("Rain Pool Size", config.RainPoolSize, 1, 100, 300, 1);
            if (newPoolSize != config.RainPoolSize)
            {
                config.RainPoolSize = (int)newPoolSize;
                Main.KeyManager.UpdateKeys();
            }
            var newSoftness = MoreGUILayout.NamedSlider("Disappear Softness", config.Softness, 0, 500, 300, 1);
            if (changed |= newSoftness != config.Softness) config.Softness = (int)newSoftness;
            if (config.ColorExpanded = GUILayout.Toggle(config.ColorExpanded, "Rain Color"))
            {
                MoreGUILayout.BeginIndent();
                var newColor = MoreGUILayout.ColorRgbaSliders(config.RainColor);
                if (changed |= newColor != config.RainColor) config.RainColor = newColor;

                Color newHexColor;
                var hexString = ColorUtility.ToHtmlStringRGBA(config.RainColor);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Hex");
                if (ColorUtility.TryParseHtmlString(GUILayout.TextField(hexString), out newHexColor))
                    config.RainColor = newHexColor;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                MoreGUILayout.EndIndent();
            }
            var newImage = MoreGUILayout.NamedTextField("Rain Image", config.RainImage);
            if (changed |= newImage != config.RainImage) config.RainImage = newImage;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rain Direction");
            if (DrawDirection($"{code} Rain Direction", ref config.Direction))
                Main.KeyManager.UpdateKeys();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return changed;
        }
        private bool IsVisible(Direction dir)
        {
            switch (dir)
            {
                case Direction.U:
                    return rt.anchoredPosition.y - rt.sizeDelta.y <= config.RainLength;
                case Direction.D:
                    return -rt.anchoredPosition.y - rt.sizeDelta.y <= config.RainLength;
                case Direction.L:
                    return -rt.anchoredPosition.x - rt.sizeDelta.x <= config.RainLength;
                case Direction.R:
                    return rt.anchoredPosition.x - rt.sizeDelta.x <= config.RainLength;
                default: return false;
            }
        }
        private Vector2 GetInitialSize()
        {
            switch (config.Direction)
            {
                case Direction.U:
                case Direction.D:
                    return config.RainWidth > 0 ?
                        new Vector2(config.RainWidth, 0) :
                        new Vector2(key.config.Width, 0);
                case Direction.L:
                case Direction.R:
                    var yOffset = (key.config.keyManager.Profile.ShowKeyPressTotal ? 50 : 0) + 5;
                    return config.RainHeight > 0 ?
                        new Vector2(0, config.RainHeight) :
                        new Vector2(0, key.config.Height + yOffset);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetDelta(Direction dir, float value)
        {
            switch (dir)
            {
                case Direction.U:
                    return new Vector2(0, value);
                case Direction.D:
                    return new Vector2(0, -value);
                case Direction.L:
                    return new Vector2(-value, 0);
                case Direction.R:
                    return new Vector2(value, 0);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetPosition(Direction dir)
        {
            var sizeDelta = key.rainMaskRt.sizeDelta;
            switch (dir)
            {
                case Direction.U:
                    return new Vector2(0, (-sizeDelta.y / 2) + config.Softness);
                case Direction.D:
                    return new Vector2(0, (sizeDelta.y / 2) - config.Softness);
                case Direction.L:
                    return new Vector2((sizeDelta.x / 2) - config.Softness, 0);
                case Direction.R:
                    return new Vector2((-sizeDelta.x / 2) + config.Softness, 0);
                default: return Vector2.zero;
            }
        }
        static readonly string[] dirNames = new string[] { "Up", "Down", "Left", "Right" };
        static readonly Direction[] dirValues = (Direction[])Enum.GetValues(typeof(Direction));
        private static bool DrawDirection(string title, ref Direction direction)
        {
            int selected = Array.IndexOf(dirValues, direction);
            bool result = UI.PopupToggleGroup(ref selected, dirNames, title);
            direction = dirValues[selected];
            return result;
        }
    }
}
