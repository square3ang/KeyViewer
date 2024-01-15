using DG.Tweening;
using KeyViewer.Core.Translation;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Unity.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Utils
{
    public static class KeyViewerUtils
    {
        public static string KeyName(KeyConfig config)
        {
            return config.DummyName ?? config.Code.ToString();
        }
        public static Vector2 InjectPivot(Key key, Vector2 pivot)
        {
            var k = key.transform;
            var origin = k.position;
            Vector3 offset = key.Size * pivot;
            foreach (Transform child in k)
                child.position += offset;
            k.position = origin;
            return offset;
        }
        public static void ApplyColorLayout(Image image, GColor color)
        {
            UICornersGradient grad = image.GetComponent<UICornersGradient>();
            if (color.gradientEnabled)
            {
                image.color = Color.white;
                if (!grad) grad = image.gameObject.AddComponent<UICornersGradient>();
                grad.topLeft = color.topLeft;
                grad.topRight = color.topRight;
                grad.bottomLeft = color.bottomLeft;
                grad.bottomRight = color.bottomRight;
                image.SetVerticesDirty();
            }
            else
            {
                if (grad) Object.Destroy(grad);
                image.color = color;
            }
        }
        public static void ApplyRoundnessLayout(Image image, float roundness)
        {
            ImageWithRoundedCorners rounder = image.GetComponent<ImageWithRoundedCorners>();
            if (roundness <= 0)
            {
                if (rounder) Object.Destroy(rounder);
                return;
            }
            if (!rounder) rounder = image.gameObject.AddComponent<ImageWithRoundedCorners>();
            rounder.radius = roundness * 90;
            rounder.Validate();
            rounder.Refresh();
        }
        public static void ApplyColorLayout(TextMeshProUGUI text, GColor color)
        {
            if (color.gradientEnabled) text.colorGradient = color;
            else text.colorGradient = new VertexGradient(color);
        }
        public static void ApplyConfigLayout(Image image, ObjectConfig config)
        {
            ApplyColorLayout(image, config.Color.Released);
            var vConfig = config.VectorConfig;
            var rt = image.rectTransform;
            rt.localRotation = Quaternion.Euler(vConfig.Rotation.Released);
            rt.anchoredPosition = vConfig.Offset.Released;
            rt.localScale = vConfig.Scale.Released;
        }
        public static void ApplyConfigLayout(Key k, VectorConfig vConfig)
        {
            var t = k.transform;
            t.localRotation = Quaternion.Euler(vConfig.Rotation.Released);
            t.localScale = vConfig.Scale.Released;
            t.localPosition = vConfig.Offset.Released + k.Position;
        }
        public static void ApplyConfigLayout(Rain r, VectorConfig vConfig)
        {
            var t = r.transform;
            t.localRotation = Quaternion.Euler(vConfig.Rotation.Released);
            t.localScale = vConfig.Scale.Released;
            t.localPosition = vConfig.Offset.Released + r.Position;
        }
        public static void ApplyConfigLayout(TextMeshProUGUI text, ObjectConfig config, float heightOffset)
        {
            ApplyColorLayout(text, config.Color.Released);
            var vConfig = config.VectorConfig;
            var rt = text.rectTransform;
            rt.localRotation = Quaternion.Euler(vConfig.Rotation.Released);
            rt.anchoredPosition = vConfig.Offset.Released.WithRelativeY(heightOffset);
            rt.localScale = vConfig.Scale.Released;
        }
        public static void ApplyColor(Image image, GColor from, GColor to, EaseConfig easeConfig)
        {
            DOTween.Kill(image, true);
            if (!easeConfig.IsValid)
            {
                ApplyColorLayout(image, to);
                return;
            }
            UICornersGradient grad = image.GetComponent<UICornersGradient>();
            var gradEnabled = from.gradientEnabled || to.gradientEnabled;
            if (gradEnabled)
            {
                image.color = Color.white;
                if (!grad) grad = image.gameObject.AddComponent<UICornersGradient>();
                var fromGrad = from.gradientEnabled ? from._color : new VertexGradient(from);
                var toGrad = to.gradientEnabled ? to._color : new VertexGradient(to);
                DOVirtual.Float(0, 1, easeConfig.Duration, f =>
                {
                    grad.topLeft = EasedColor(fromGrad.topLeft, toGrad.topLeft, f);
                    grad.topRight = EasedColor(fromGrad.topRight, toGrad.topRight, f);
                    grad.bottomLeft = EasedColor(fromGrad.bottomLeft, toGrad.bottomLeft, f);
                    grad.bottomRight = EasedColor(fromGrad.bottomRight, toGrad.bottomRight, f);
                    image.SetVerticesDirty();
                }).SetEase(easeConfig.Ease).SetAutoKill(false).SetTarget(image);
            }
            else
            {
                if (grad) Object.Destroy(grad);
                image.DOColor(to, easeConfig.Duration).SetEase(easeConfig.Ease).SetAutoKill(false);
            }
        }
        public static void ApplyColor(TextMeshProUGUI text, GColor from, GColor to, EaseConfig easeConfig)
        {
            DOTween.Kill(text, true);
            if (!easeConfig.IsValid)
            {
                text.color = Color.white;
                ApplyColorLayout(text, to);
                return;
            }
            var gradEnabled = from.gradientEnabled || to.gradientEnabled;
            if (gradEnabled)
            {
                var fromGrad = from.gradientEnabled ? from._color : new VertexGradient(from);
                var toGrad = to.gradientEnabled ? to._color : new VertexGradient(to);
                DOVirtual.Float(0, 1, easeConfig.Duration, f =>
                {
                    var tl = EasedColor(fromGrad.topLeft, toGrad.topLeft, f);
                    var tr = EasedColor(fromGrad.topRight, toGrad.topRight, f);
                    var bl = EasedColor(fromGrad.bottomLeft, toGrad.bottomLeft, f);
                    var br = EasedColor(fromGrad.bottomRight, toGrad.bottomRight, f);
                    text.colorGradient = new VertexGradient(tl, tr, bl, br);
                }).SetEase(easeConfig.Ease).SetAutoKill(false).SetTarget(text);
            }
            else
            {
                text.DOColor(to, easeConfig.Duration).SetEase(easeConfig.Ease).SetAutoKill(false);
            }
        }
        public static Color EasedColor(Color from, Color to, float lifetime)
        {
            var r = to.r - from.r;
            var g = to.g - from.g;
            var b = to.b - from.b;
            var a = to.a - from.a;
            return new Color(from.r + r * lifetime, from.g + g * lifetime, from.b + b * lifetime, from.a + a * lifetime);
        }
        public static void ApplyVectorConfig(Key k, VectorConfig vConfig, bool pressed)
        {
            Transform t = k.transform;

            DOTween.Kill(t, true);

            var rEase = vConfig.Rotation.GetEase(pressed);
            if (rEase.IsValid)
                t.DOLocalRotate(vConfig.Rotation.Get(pressed), rEase.Duration)
                .SetEase(rEase.Ease)
                .SetAutoKill(false);
            else t.localRotation = Quaternion.Euler(vConfig.Rotation.Get(pressed));

            var oEase = vConfig.Offset.GetEase(pressed);
            if (oEase.IsValid)
                t.DOLocalMove(vConfig.Offset.Get(pressed) + k.Position, oEase.Duration)
                .SetEase(oEase.Ease)
                .SetAutoKill(false);
            else t.localPosition = vConfig.Offset.Get(pressed) + k.Position;

            var sEase = vConfig.Scale.GetEase(pressed);
            if (sEase.IsValid)
                t.DOScale(vConfig.Scale.Get(pressed), sEase.Duration)
                .SetEase(sEase.Ease)
                .SetAutoKill(false);
            else t.localScale = vConfig.Scale.Get(pressed);
        }
        public static void ApplyVectorConfig(RectTransform rt, VectorConfig vConfig, bool pressed, float heightOffset)
        {
            ApplyVectorConfig(rt, vConfig, pressed, new Vector2(0, heightOffset));
        }
        public static void ApplyVectorConfig(RectTransform rt, VectorConfig vConfig, bool pressed, Vector2 offset)
        {
            DOTween.Kill(rt, true);

            var rEase = vConfig.Rotation.GetEase(pressed);
            if (rEase.IsValid)
                rt.DOLocalRotate(vConfig.Rotation.Get(pressed), rEase.Duration)
                .SetEase(rEase.Ease)
                .SetAutoKill(false);
            else rt.localRotation = Quaternion.Euler(vConfig.Rotation.Get(pressed));

            var oEase = vConfig.Offset.GetEase(pressed);
            if (oEase.IsValid)
                rt.DOAnchorPos(vConfig.Offset.Get(pressed) + offset, oEase.Duration)
                .SetEase(oEase.Ease)
                .SetAutoKill(false);
            else rt.anchoredPosition = vConfig.Offset.Get(pressed) + offset;

            var sEase = vConfig.Scale.GetEase(pressed);
            if (sEase.IsValid)
                rt.DOScale(vConfig.Scale.Get(pressed), sEase.Duration)
                .SetEase(sEase.Ease)
                .SetAutoKill(false);
            else rt.localScale = vConfig.Scale.Get(pressed);
        }
        public static void SetMaskAnchor(RectTransform rt, Direction dir, Pivot pivot = Pivot.MiddleCenter, Anchor anchor = Anchor.MiddleCenter)
        {
            switch (dir)
            {
                case Direction.Up:
                    rt.pivot = new Vector2(0.5f, 0);
                    rt.anchorMin = new Vector2(0.5f, 0);
                    rt.anchorMax = new Vector2(0.5f, 0);
                    break;
                case Direction.Down:
                    rt.pivot = new Vector2(0.5f, 1);
                    rt.anchorMin = new Vector2(0.5f, 1);
                    rt.anchorMax = new Vector2(0.5f, 1);
                    break;
                case Direction.Right:
                    rt.pivot = new Vector2(0, 0.5f);
                    rt.anchorMin = new Vector2(0, 0.5f);
                    rt.anchorMax = new Vector2(0, 0.5f);
                    break;
                case Direction.Left:
                    rt.pivot = new Vector2(1, 0.5f);
                    rt.anchorMin = new Vector2(1, 0.5f);
                    rt.anchorMax = new Vector2(1, 0.5f);
                    break;
            }
            if (pivot != Pivot.MiddleCenter)
                rt.pivot = GetPivot(pivot).InversePivot();
            if (anchor != Anchor.MiddleCenter)
                rt.SetAnchor(anchor);
        }
        public static void OpenDiscordUrl()
        {
            Application.OpenURL(Main.Lang[TranslationKeys.DiscordLink]);
        }
        public static void OpenDownloadUrl()
        {
            if (Main.HasUpdate)
                Application.OpenURL(Main.Lang[TranslationKeys.DownloadLink]);
        }
        public static Vector2 GetPivot(Pivot pivot)
        {
            switch (pivot)
            {
                case Pivot.TopLeft: return new Vector2(0, 1);
                case Pivot.TopCenter: return new Vector2(0.5f, 1);
                case Pivot.TopRight: return new Vector2(1, 1);

                case Pivot.MiddleLeft: return new Vector2(0, 0.5f);
                case Pivot.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case Pivot.MiddleRight: return new Vector2(1, 0.5f);

                case Pivot.BottomLeft: return new Vector2(0, 0);
                case Pivot.BottomCenter: return new Vector2(0.5f, 0);
                case Pivot.BottomRight: return new Vector2(1, 0);
            }
            return Vector2.zero;
        }
    }
}
