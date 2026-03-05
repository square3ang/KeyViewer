namespace KeyViewer.Models;

[System.Flags]
public enum Pivot {
    None = 0,

    Top = 1 << 0,
    MiddleV = 1 << 1,
    Bottom = 1 << 2,

    Left = 1 << 3,
    CenterH = 1 << 4,
    Right = 1 << 5,

    TopLeft = Top | Left,
    TopCenter = Top | CenterH,
    TopRight = Top | Right,

    MiddleLeft = MiddleV | Left,
    MiddleCenter = MiddleV | CenterH,
    MiddleRight = MiddleV | Right,

    BottomLeft = Bottom | Left,
    BottomCenter = Bottom | CenterH,
    BottomRight = Bottom | Right
}