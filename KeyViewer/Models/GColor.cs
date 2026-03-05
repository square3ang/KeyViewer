using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace KeyViewer.Models;

public struct GColor : IModel, ICopyable<GColor> {
    internal VertexGradient _color;

    private string _topLeftHex;
    private string _topRightHex;
    private string _bottomLeftHex;
    private string _bottomRightHex;

    public bool gradientEnabled = false;

    public Color topLeft { get => _color.topLeft; set => SetTopLeftColor(value); }
    public Color topRight { get => _color.topRight; set => SetTopRightColor(value); }
    public Color bottomLeft { get => _color.bottomLeft; set => SetBottomLeftColor(value); }
    public Color bottomRight { get => _color.bottomRight; set => SetBottomRightColor(value); }

    public string topLeftHex { get => _topLeftHex; set => SetTopLeftHex(value); }
    public string topRightHex { get => _topRightHex; set => SetTopRightHex(value); }
    public string bottomLeftHex { get => _bottomLeftHex; set => SetBottomLeftHex(value); }
    public string bottomRightHex { get => _bottomRightHex; set => SetBottomRightHex(value); }

    public float r { get => _color.topLeft.r; set => SetTopLeftColor(_color.topLeft with { r = value }); }
    public float g { get => _color.topLeft.g; set => SetTopLeftColor(_color.topLeft with { g = value }); }
    public float b { get => _color.topLeft.b; set => SetTopLeftColor(_color.topLeft with { b = value }); }
    public float a { get => _color.topLeft.a; set => SetTopLeftColor(_color.topLeft with { a = value }); }

    public bool isSame =>
        _color.topLeft == _color.topRight &&
        _color.topLeft == _color.bottomLeft &&
        _color.topLeft == _color.bottomRight;

    public GColor(Color color) {
        _color = new VertexGradient(color);
        var hex = ColorUtility.ToHtmlStringRGBA(color);
        _topLeftHex = hex;
        _topRightHex = hex;
        _bottomLeftHex = hex;
        _bottomRightHex = hex;
    }
    public GColor(VertexGradient color) {
        _color = color;
        _topLeftHex = ColorUtility.ToHtmlStringRGBA(color.topLeft);
        _topRightHex = ColorUtility.ToHtmlStringRGBA(color.topRight);
        _bottomLeftHex = ColorUtility.ToHtmlStringRGBA(color.bottomLeft);
        _bottomRightHex = ColorUtility.ToHtmlStringRGBA(color.bottomRight);
    }
    public GColor Copy() {
        var col = new GColor {
            gradientEnabled = gradientEnabled,
            topLeft = topLeft,
            topRight = topRight,
            bottomLeft = bottomLeft,
            bottomRight = bottomRight
        };
        return col;
    }
    public JToken Serialize() {
        return isSame
            ? new JObject {
                [nameof(topLeft)] = ModelUtils.ToNode(topLeft),
            }
            : new JObject {
                [nameof(topLeft)] = ModelUtils.ToNode(topLeft),
                [nameof(topRight)] = ModelUtils.ToNode(topRight),
                [nameof(bottomLeft)] = ModelUtils.ToNode(bottomLeft),
                [nameof(bottomRight)] = ModelUtils.ToNode(bottomRight),
            };
    }
    public void Deserialize(JToken node) {
        topLeft = node[nameof(topLeft)] != null
            ? ModelUtils.ToColor(node[nameof(topLeft)])
            : default;
        topRight = node[nameof(topRight)] != null
            ? ModelUtils.ToColor(node[nameof(topRight)])
            : topLeft;
        bottomLeft = node[nameof(bottomLeft)] != null
            ? ModelUtils.ToColor(node[nameof(bottomLeft)])
            : topLeft;
        bottomRight = node[nameof(bottomRight)] != null
            ? ModelUtils.ToColor(node[nameof(bottomRight)])
            : topLeft;

        gradientEnabled = !isSame;
    }

    private void SetTopLeftColor(Color color) {
        if(color == _color.topLeft) {
            return;
        }

        _color.topLeft = color;
        _topLeftHex = ColorUtility.ToHtmlStringRGBA(color);
    }
    private void SetTopLeftHex(string hex) {
        if(hex == _topLeftHex) {
            return;
        }

        if(ColorUtility.TryParseHtmlString($"#{hex}", out var parsed)) {
            _color.topLeft = parsed;
            _topLeftHex = hex;
        }
    }

    private void SetTopRightColor(Color color) {
        if(color == _color.topRight) {
            return;
        }

        _color.topRight = color;
        _topRightHex = ColorUtility.ToHtmlStringRGBA(color);
    }
    private void SetTopRightHex(string hex) {
        if(hex == _topRightHex) {
            return;
        }

        if(ColorUtility.TryParseHtmlString($"#{hex}", out var parsed)) {
            _color.topRight = parsed;
            _topRightHex = hex;
        }
    }

    private void SetBottomLeftColor(Color color) {
        if(color == _color.bottomLeft) {
            return;
        }

        _color.bottomLeft = color;
        _bottomLeftHex = ColorUtility.ToHtmlStringRGBA(color);
    }
    private void SetBottomLeftHex(string hex) {
        if(hex == _bottomLeftHex) {
            return;
        }

        if(ColorUtility.TryParseHtmlString($"#{hex}", out var parsed)) {
            _color.bottomLeft = parsed;
            _bottomLeftHex = hex;
        }
    }

    private void SetBottomRightColor(Color color) {
        if(color == _color.bottomRight) {
            return;
        }

        _color.bottomRight = color;
        _bottomRightHex = ColorUtility.ToHtmlStringRGBA(color);
    }
    private void SetBottomRightHex(string hex) {
        if(hex == _bottomRightHex) {
            return;
        }

        if(ColorUtility.TryParseHtmlString($"#{hex}", out var parsed)) {
            _color.bottomRight = parsed;
            _bottomRightHex = hex;
        }
    }

    public static implicit operator Color(GColor color) => color.topLeft;
    public static implicit operator GColor(Color color) => new(color);

    public static implicit operator VertexGradient(GColor color) => color.gradientEnabled ? new VertexGradient(color.topLeft, color.topRight, color.bottomLeft, color.bottomRight) : new VertexGradient(color);
    public static implicit operator GColor(VertexGradient color) => new(color);

    public static GColor operator +(GColor a, GColor b) {
        return new VertexGradient(
            a.topLeft + b.topLeft,
            a.topRight + b.topRight,
            a.bottomLeft + b.bottomLeft,
            a.bottomRight + b.bottomRight);
    }
    public static GColor operator -(GColor a, GColor b) {
        return new VertexGradient(
            a.topLeft - b.topLeft,
            a.topRight - b.topRight,
            a.bottomLeft - b.bottomLeft,
            a.bottomRight - b.bottomRight);
    }
}
