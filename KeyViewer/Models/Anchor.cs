using System;

namespace KeyViewer.Models;

[Flags]
public enum Anchor {
    None = 0,

    Left = 1 << 0,
    Center = 1 << 1,
    Right = 1 << 2,
    HStretch = 1 << 3,

    Top = 1 << 4,
    Middle = 1 << 5,
    Bottom = 1 << 6,
    VStretch = 1 << 7,

    TopLeft = Top | Left,
    TopCenter = Top | Center,
    TopRight = Top | Right,

    MiddleLeft = Middle | Left,
    MiddleCenter = Middle | Center,
    MiddleRight = Middle | Right,

    BottomLeft = Bottom | Left,
    BottomCenter = Bottom | Center,
    BottomRight = Bottom | Right,

    HorizontalStretchTop = Top | HStretch,
    HorizontalStretchMiddle = Middle | HStretch,
    HorizontalStretchBottom = Bottom | HStretch,

    VerticalStretchLeft = Left | VStretch,
    VerticalStretchCenter = Center | VStretch,
    VerticalStretchRight = Right | VStretch,

    FullStretch = HStretch | VStretch
}