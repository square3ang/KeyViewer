using KeyViewer.Models;
using KeyViewer.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity
{
    public class Rain : MonoBehaviour
    {
        public bool IsAlive { get; private set; }
        public Vector2 Position;
        public Vector2 DefaultSize;

        private bool stretching = false;
        private RainConfig config;
        private Key key;
        private RectTransform rt;
        private ObjectConfig objConfig;
        private int colorUpdateIgnores;
        private bool initialized = false;

        internal Image image;
        public void Init(Key key)
        {
            if (initialized) return;
            this.key = key;
            image = gameObject.AddComponent<Image>();
            rt = image.rectTransform;
            config = key.Config.Rain;
            objConfig = config.ObjectConfig;

            OnEnable();
            KeyViewerUtils.ApplyColorLayout(image, objConfig.Color.Released);
            KeyViewerUtils.ApplyConfigLayout(this, objConfig.VectorConfig);
            initialized = true;
        }
        public void Press()
        {
            stretching = true;
            var color = config.ObjectConfig.Color;
            if (colorUpdateIgnores == 0)
                KeyViewerUtils.ApplyColor(image, color.Released, color.Pressed, color.PressedEase);
            else colorUpdateIgnores--;
            KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, true, Position, false, DefaultSize);
        }
        public void Release()
        {
            stretching = false;
            var color = config.ObjectConfig.Color;
            if (colorUpdateIgnores == 0)
                KeyViewerUtils.ApplyColor(image, color.Pressed, color.Released, color.ReleasedEase);
            else colorUpdateIgnores--;
            Vector2 adjustedPosition = KeyViewerUtils.AdjustRainPosition(config.Direction, Position, objConfig.VectorConfig.Offset.Pressed);
            KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, false, adjustedPosition, false, DefaultSize);
        }
        public void OnEnable()
        {
            if (!initialized) return;
            colorUpdateIgnores = 0;
            image.sprite = key.RainImageManager.Get();
            rt.sizeDelta = DefaultSize = GetInitialSize();
            rt.anchoredPosition = GetPosition(config.Direction);
            Position = rt.localPosition;
            var lastRound = key.RainImageManager.GetLastRoundness();
            KeyViewerUtils.ApplyRoundnessLayout(image, lastRound == 0 ? config.Roundness : lastRound);
        }
        public void IgnoreColorUpdate()
        {
            colorUpdateIgnores++;
        }
        private void Update()
        {
            IsAlive = IsVisible(config.Direction);
            if (IsAlive)
            {
                var toMove = Time.deltaTime * config.Speed.Get(key.Pressed);
                var delta = GetDelta(config.Direction, toMove);
                if (stretching)
                {
                    rt.sizeDelta += delta.Abs();
                    rt.anchoredPosition += delta * 0.5f;
                    DefaultSize = rt.sizeDelta;
                }
                else rt.anchoredPosition += delta;
                Position = rt.localPosition;
            }
            else
            {
                stretching = false;
                OnEnable();
                gameObject.SetActive(false);
            }
        }
        private bool IsVisible(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return rt.anchoredPosition.y - rt.sizeDelta.y <= config.Length.Get(key.Pressed);
                case Direction.Down:
                    return -rt.anchoredPosition.y - rt.sizeDelta.y <= config.Length.Get(key.Pressed);
                case Direction.Left:
                    return -rt.anchoredPosition.x - rt.sizeDelta.x <= config.Length.Get(key.Pressed);
                case Direction.Right:
                    return rt.anchoredPosition.x - rt.sizeDelta.x <= config.Length.Get(key.Pressed);
                default: return false;
            }
        }
        private Vector2 GetInitialSize()
        {
            Vector2 scale = objConfig.VectorConfig.Scale.Get(key.Pressed);
            switch (config.Direction)
            {
                case Direction.Up:
                case Direction.Down:
                    return scale.x > 0 ?
                        new Vector2(key.Size.x * scale.x, 0) :
                        new Vector2(key.Size.x, 0);
                case Direction.Left:
                case Direction.Right:
                    return scale.y > 0 ?
                        new Vector2(0, key.Size.y * scale.y) :
                        new Vector2(0, key.Size.y);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetDelta(Direction dir, float value)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new Vector2(0, value);
                case Direction.Down:
                    return new Vector2(0, -value);
                case Direction.Left:
                    return new Vector2(-value, 0);
                case Direction.Right:
                    return new Vector2(value, 0);
                default: return Vector2.zero;
            }
        }
        private Vector2 GetPosition(Direction dir)
        {
            var sizeDelta = key.RainMaskRt.sizeDelta;
            switch (dir)
            {
                case Direction.Up:
                    return new Vector2(0, (-sizeDelta.y / 2) + config.Softness.Get(key.Pressed));
                case Direction.Down:
                    return new Vector2(0, (sizeDelta.y / 2) - config.Softness.Get(key.Pressed));
                case Direction.Left:
                    return new Vector2((sizeDelta.x / 2) - config.Softness.Get(key.Pressed), 0);
                case Direction.Right:
                    return new Vector2((-sizeDelta.x / 2) + config.Softness.Get(key.Pressed), 0);
                default: return Vector2.zero;
            }
        }
    }
}
