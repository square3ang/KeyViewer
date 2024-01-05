using KeyViewer.Models;
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
        public static void ApplyColor(Image image, GColor color)
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
        public static void ApplyRoundness(Image image, float roundness)
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
        public static void ApplyColor(TextMeshProUGUI text, GColor color)
        {
            if (color.gradientEnabled) text.colorGradient = color;
            else text.colorGradient = new VertexGradient(color);
        }
        public static void ApplyConfig(Image image, ObjectConfig config)
        {
            ApplyColor(image, config.Color.Released);
            var vConfig = config.VectorConfig;
            var rt = image.rectTransform;
            rt.rotation = Quaternion.Euler(vConfig.Rotation.Released);
            rt.anchoredPosition += vConfig.Offset.Released;
            rt.localScale = vConfig.Scale.Released;
        }
        public static void ApplyConfig(TextMeshProUGUI text, ObjectConfig config)
        {
            ApplyColor(text, config.Color.Released);
            var vConfig = config.VectorConfig;
            var rt = text.rectTransform;
            rt.rotation = Quaternion.Euler(vConfig.Rotation.Released);
            rt.anchoredPosition += vConfig.Offset.Released;
            rt.localScale = vConfig.Scale.Released;
        }
    }
}
