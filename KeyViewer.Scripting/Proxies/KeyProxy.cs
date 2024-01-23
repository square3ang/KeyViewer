using JSNet.API;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Unity.UI;
using KeyViewer.Utils;
using TMPro;
using UnityEngine;

namespace KeyViewer.Scripting.Proxies
{
    [Alias("Key")]
    public class KeyProxy
    {
        [NotVisible]
        public Key key;
        public KeyConfig Config;
        public KeyProxy(Key key)
        {
            this.key = key;
            Config = key.Config;
        }
        public void SetBackgroundImage(string imagePath)
        {
            key.Background.sprite = AssetManager.Get(imagePath, AssetManager.Background);
        }
        public void SetOutlineImage(string imagePath)
        {
            key.Outline.sprite = AssetManager.Get(imagePath, AssetManager.Outline);
        }
        public string Text { get => key.Text.text; set => key.Text.text = value; }
        public string CountText { get => key.CountText.text; set => key.CountText.text = value; }
        public GColor TextColor { get => key.Text.colorGradient; set => key.Text.colorGradient = value; }
        public GColor CountTextColor { get => key.CountText.colorGradient; set => key.CountText.colorGradient = value; }
        public GColor BackgroundColor
        {
            get
            {
                var grad = key.Background.GetComponent<UICornersGradient>();
                if (grad != null) return new VertexGradient(grad.topLeft, grad.topRight, grad.bottomLeft, grad.bottomRight);
                return key.Background.color;
            }
            set => KeyViewerUtils.ApplyColorLayout(key.Background, value, key.Config.BackgroundBlurEnabled);
        }
        public GColor OutlineColor
        {
            get
            {
                var grad = key.Outline.GetComponent<UICornersGradient>();
                if (grad != null) return new VertexGradient(grad.topLeft, grad.topRight, grad.bottomLeft, grad.bottomRight);
                return key.Outline.color;
            }
            set => KeyViewerUtils.ApplyColorLayout(key.Outline, value, false);
        }
        public Vector3 Rotation { get => key.transform.localRotation.eulerAngles; set => key.transform.localRotation = Quaternion.Euler(value); }
        public Vector3 TextRotation { get => key.Text.rectTransform.localRotation.eulerAngles; set => key.Text.rectTransform.localRotation = Quaternion.Euler(value); }
        public Vector3 CountTextRotation { get => key.CountText.rectTransform.localRotation.eulerAngles; set => key.CountText.rectTransform.localRotation = Quaternion.Euler(value); }
        public Vector3 BackgroundRotation { get => key.Background.rectTransform.localRotation.eulerAngles; set => key.Background.rectTransform.localRotation = Quaternion.Euler(value); }
        public Vector3 OutlineRotation { get => key.Outline.rectTransform.localRotation.eulerAngles; set => key.Outline.rectTransform.localRotation = Quaternion.Euler(value); }
        public Vector3 Offset { get => key.transform.localPosition - (Vector3)key.Position; set => key.transform.localPosition = (Vector3)key.Position + value; }
        public Vector3 TextOffset { get => key.Text.rectTransform.localPosition; set => key.Text.rectTransform.localPosition = value; }
        public Vector3 CountTextOffset { get => key.CountText.rectTransform.localPosition; set => key.CountText.rectTransform.localPosition = value; }
        public Vector3 BackgroundOffset { get => key.Background.rectTransform.localPosition; set => key.Background.rectTransform.localPosition = value; }
        public Vector3 OutlineOffset { get => key.Outline.rectTransform.localPosition; set => key.Outline.rectTransform.localPosition = value; }
        public Vector2 Scale { get => key.transform.localScale; set => key.transform.localScale = value; }
        public Vector2 TextScale { get => key.Text.rectTransform.localScale; set => key.Text.rectTransform.localScale = value; }
        public Vector2 CountTextScale { get => key.CountText.rectTransform.localScale; set => key.CountText.rectTransform.localScale = value; }
        public Vector2 BackgroundScale { get => key.Background.rectTransform.sizeDelta / key.DefaultSize; set => key.Background.rectTransform.sizeDelta = key.DefaultSize * value; }
        public Vector2 OutlineScale { get => key.Outline.rectTransform.sizeDelta / key.DefaultSize; set => key.Outline.rectTransform.sizeDelta = key.DefaultSize * value; }
    }
}
