using TMPro;
using UnityEngine;

namespace KeyViewer.Models
{
    public struct GColor
    {
        private VertexGradient _color;
        private string topLeftHex;
        private string topRightHex;
        private string bottomLeftHex;
        private string bottomRightHex;

        public Color topLeft { get => _color.topLeft; set => topLeftHex = ColorUtility.ToHtmlStringRGBA(_color.topLeft = value); }
        public Color topRight { get => _color.topRight; set => topRightHex = ColorUtility.ToHtmlStringRGBA(_color.topRight = value); }
        public Color bottomLeft { get => _color.bottomLeft; set => bottomLeftHex = ColorUtility.ToHtmlStringRGBA(_color.bottomLeft = value); }
        public Color bottomRight { get => _color.bottomRight; set => bottomRightHex = ColorUtility.ToHtmlStringRGBA(_color.bottomRight = value); }

        public GColor(Color color)
        {
            _color = new VertexGradient(color);
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            topLeftHex = hex;
            topRightHex = hex;
            bottomLeftHex = hex;
            bottomRightHex = hex;
        }
        public GColor(VertexGradient color)
        {
            _color = color;
            topLeftHex = ColorUtility.ToHtmlStringRGBA(color.topLeft);
            topRightHex = ColorUtility.ToHtmlStringRGBA(color.topRight);
            bottomLeftHex = ColorUtility.ToHtmlStringRGBA(color.bottomLeft);
            bottomRightHex = ColorUtility.ToHtmlStringRGBA(color.bottomRight);
        }

        public void SyncHex()
        {
            ColorUtility.TryParseHtmlString($"#{topLeftHex}", out _color.topLeft);
            ColorUtility.TryParseHtmlString($"#{topRightHex}", out _color.topRight);
            ColorUtility.TryParseHtmlString($"#{bottomLeftHex}", out _color.bottomLeft);
            ColorUtility.TryParseHtmlString($"#{bottomRightHex}", out _color.bottomRight);
        }

        public static implicit operator Color(GColor color) => color.topLeft;
        public static implicit operator GColor(Color color) => new GColor(color);

        public static implicit operator VertexGradient(GColor color) => new VertexGradient(color.topLeft, color.topRight, color.bottomLeft, color.bottomRight);
        public static implicit operator GColor(VertexGradient color) => new GColor(color);
    }
}
