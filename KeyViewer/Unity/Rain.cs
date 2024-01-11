using KeyViewer.Models;
using KeyViewer.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity
{
    public class Rain : MonoBehaviour
    {
        public bool IsAlive { get; private set; }
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
            image.sprite = key.RainImageManager.Get();

            KeyViewerUtils.ApplyColorLayout(image, objConfig.Color.Released);
            OnEnable();
            initialized = true;
        }
        public void Press()
        {
            stretching = true;
            var color = config.ObjectConfig.Color;
            if (colorUpdateIgnores == 0)
                KeyViewerUtils.ApplyColor(image, color.Released, color.Pressed, color.PressedEase);
            else colorUpdateIgnores--;
        }
        public void Release()
        {
            stretching = false;
            var color = config.ObjectConfig.Color;
            if (colorUpdateIgnores == 0)
                KeyViewerUtils.ApplyColor(image, color.Pressed, color.Released, color.ReleasedEase);
            else colorUpdateIgnores--;
        }
        public void OnEnable()
        {
            if (!initialized) return;   
            colorUpdateIgnores = 0;
            image.sprite = key.RainImageManager.Get();
            rt.sizeDelta = GetInitialSize();
            rt.anchoredPosition = GetPosition(config.Direction);
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
                    var yOffset = (key.Config.EnableCountText ? 50 : 0) + 5;
                    return scale.y > 0 ?
                        new Vector2(0, key.Size.y * scale.y + yOffset) :
                        new Vector2(0, key.Size.y + yOffset);
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
