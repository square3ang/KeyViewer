using DG.Tweening;
using HarmonyLib;
using KeyViewer.Models;
using RapidGUI;
using System;
using UnityEngine;

namespace KeyViewer.Core;

public delegate bool CustomDrawer<T>(T t);
public delegate bool CustomDrawerRef<T>(ref T t);
public static class Drawer {
    public static bool DrawBool(string label, ref bool value) {
        bool prev = value;

        GUILayout.BeginHorizontal();

        if(Main.Settings.UseLegacyTheme) {
            value = GUILayout.Toggle(value, "");
        } else {
            var old = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            var newskin = new GUIStyle(GUI.skin.button) {
                fontSize = 16,
                margin = new RectOffset(0, 0, 4, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            if(GUILayout.Button(value ? Icon_Active : Icon_Inactive, newskin)) {
                value = !value;
            }

            GUI.backgroundColor = old;
        }

        if(GUILayout.Button(label, GUI.skin.label)) {
            value = !value;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        return prev != value;
    }
    public static bool DrawBool(Texture2D icon, string label, ref bool value) {
        bool prev = value;

        GUILayout.BeginHorizontal();

        if(Main.Settings.UseLegacyTheme) {
            value = GUILayout.Toggle(value, "");
        } else {
            var old = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            var newskin = new GUIStyle(GUI.skin.button) {
                fontSize = 16,
                margin = new RectOffset(0, 0, 4, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            if(GUILayout.Button(value ? Icon_Active : Icon_Inactive, newskin)) {
                value = !value;
            }

            GUI.backgroundColor = old;
        }

        bool buttonPressed = false;
        buttonPressed |= GUILayout.Button(icon, GUI.skin.label);
        buttonPressed |= GUILayout.Button(label, GUI.skin.label);

        if(buttonPressed) {
            value = !value;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        return prev != value;
    }
    public static bool DrawOnlyBool(ref bool value) {
        bool prev = value;

        if(Main.Settings.UseLegacyTheme) {
            value = GUILayout.Toggle(value, "");
        } else {
            var old = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            var newskin = new GUIStyle(GUI.skin.button) {
                fontSize = 16,
                margin = new RectOffset(0, 0, 4, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            if(GUILayout.Button(value ? Icon_Active : Icon_Inactive, newskin)) {
                value = !value;
            }

            GUI.backgroundColor = old;
        }

        return prev != value;
    }

    public static bool DrawEase(ref Ease ease) {
        string[] names = Enum.GetNames(typeof(Ease));
        int current = (int)ease;
        Texture2D[] easeImages = new Texture2D[] { null, Icon_EaseLinear, Icon_EaseInSine, Icon_EaseOutSine, Icon_EaseInOutSine, Icon_EaseInQuad, Icon_EaseOutQuad, Icon_EaseInOutQuad, Icon_EaseInCubic, Icon_EaseOutCubic, Icon_EaseInOutCubic, Icon_EaseInQuart, Icon_EaseOutQuart, Icon_EaseInOutQuart, Icon_EaseInQuint, Icon_EaseOutQuint, Icon_EaseInOutQuint, Icon_EaseInExpo, Icon_EaseOutExpo, Icon_EaseInOutExpo, Icon_EaseInCirc, Icon_EaseOutCirc, Icon_EaseInOutCirc, Icon_EaseInElastic, Icon_EaseOutElastic, Icon_EaseInOutElastic, Icon_EaseInBack, Icon_EaseOutBack, Icon_EaseInOutBack, Icon_EaseInBounce, Icon_EaseOutBounce, Icon_EaseInOutBounce };
        bool result = SelectionPopup(ref current, names, easeImages, "");
        if(result) {
            ease = (Ease)current;
            return true;
        }
        return false;
    }

    public static bool DrawString(string label, ref string value, bool textArea = false) {
        string prev = value;
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        value = !textArea ? GUILayout.TextField(value, myTextField) : GUILayout.TextArea(value, myTextField);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        return prev != value;
    }

    public static bool Button(string str, params GUILayoutOption[] options) => GUILayout.Button(str, myButton, options);
    public static bool Button(Texture2D texture, params GUILayoutOption[] options) => GUILayout.Button(texture, myButton, options);

    public static bool SelectionPopup(ref int selected, string[] options, string label,
        params GUILayoutOption[] layoutOptions) {
        if(label != "") {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
        }

        var news = RGUI.SelectionPopup(selected, options, null, layoutOptions);
        var c = selected != news;

        selected = news;
        if(label != "") {
            GUILayout.EndHorizontal();
        }

        return c;
    }

    public static bool SelectionPopup(ref int selected, string[] options, Texture2D[] images, string label,
        params GUILayoutOption[] layoutOptions) {
        if(label != "") {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
        }

        var news = RGUI.SelectionPopup(selected, options, images, null, layoutOptions);
        var c = selected != news;

        selected = news;
        if(label != "") {
            GUILayout.EndHorizontal();
        }

        return c;
    }

    public static bool DrawPressReleaseBase(PressReleaseBase<string> prb) {
        bool changed = false;
        Color old = GUI.color;

        GUILayout.BeginHorizontal();
        GUILayout.Label(Icon_Down);
        GUI.color = new Color(0.5f, 1f, 0.5f);
        if(Button(Icon_Copy, GUILayout.Width(34))) {
            prb.Pressed = prb.Released;
            changed = true;
        }
        GUI.color = old;
        string newPressed = GUILayout.TextField(prb.Pressed, myTextField);
        if(newPressed != prb.Pressed) {
            prb.Pressed = newPressed;
            changed = true;
        }
        GUILayout.Space(14);
        GUILayout.Label(Icon_Up);
        GUI.color = new Color(0.5f, 1f, 0.5f);
        if(Button(Icon_Copy, GUILayout.Width(34))) {
            prb.Released = prb.Pressed;
            changed = true;
        }
        GUI.color = old;
        string newReleased = GUILayout.TextField(prb.Released, myTextField);
        if(newReleased != prb.Released) {
            prb.Released = newReleased;
            changed = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        return changed;
    }

    public static bool DrawAnchor(ref Anchor anchor) {
        bool changed = false;

        Color old = GUI.color;
        GUILayout.BeginHorizontal();
        GUI.color = anchor == Anchor.TopLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorTopLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.TopLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.TopCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorTopCenter, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.TopCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.TopRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorTopRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.TopRight; changed = true; }
        GUILayout.Space(8);
        GUI.color = anchor == Anchor.HorizontalStretchTop ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorHorizontalStretchTop, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.HorizontalStretchTop; changed = true; }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUI.color = anchor == Anchor.MiddleLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorMiddleLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.MiddleLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.MiddleCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorMiddleCenter, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.MiddleCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.MiddleRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorMiddleRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.MiddleRight; changed = true; }
        GUILayout.Space(8);
        GUI.color = anchor == Anchor.HorizontalStretchMiddle ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorHorizontalStretchMiddle, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.HorizontalStretchMiddle; changed = true; }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUI.color = anchor == Anchor.BottomLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorBottomLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.BottomLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.BottomCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorBottomCenter, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.BottomCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.BottomRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorBottomRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.BottomRight; changed = true; }
        GUILayout.Space(8);
        GUI.color = anchor == Anchor.HorizontalStretchBottom ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorHorizontalStretchBottom, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.HorizontalStretchBottom; changed = true; }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUI.color = anchor == Anchor.VerticalStretchLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorVerticalStretchLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.VerticalStretchLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.VerticalStretchCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorVerticalStretchCenter, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.VerticalStretchCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = anchor == Anchor.VerticalStretchRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorVerticalStretchRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.VerticalStretchRight; changed = true; }
        GUILayout.Space(8);
        GUI.color = anchor == Anchor.FullStretch ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_AnchorFullStretch, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { anchor = Anchor.FullStretch; changed = true; }
        GUILayout.EndHorizontal();

        GUI.color = old;
        return changed;
    }

    public static bool DrawPivot(ref Pivot pivot) {
        bool changed = false;
        Color old = GUI.color;

        GUILayout.BeginHorizontal();
        GUI.color = pivot == Pivot.TopLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_UpLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.TopLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.TopCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_Up, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.TopCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.TopRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_UpRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.TopRight; changed = true; }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUI.color = pivot == Pivot.MiddleLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_Left, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.MiddleLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.MiddleCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_Center, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.MiddleCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.MiddleRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_Right, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.MiddleRight; changed = true; }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUI.color = pivot == Pivot.BottomLeft ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_DownLeft, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.BottomLeft; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.BottomCenter ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_Down, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.BottomCenter; changed = true; }
        GUILayout.Space(4);
        GUI.color = pivot == Pivot.BottomRight ? Color.cyan : Color.white;
        if(GUILayout.Button(Icon_DownRight, nopadButton, GUILayout.Width(26), GUILayout.Height(26))) { pivot = Pivot.BottomRight; changed = true; }
        GUILayout.EndHorizontal();

        GUI.color = old;
        return changed;
    }

    public static void TooltipAuto(string text, bool ignoreWidth = false) {
        Rect lastRect = GUILayoutUtility.GetLastRect();
        if(lastRect.Contains(Event.current.mousePosition)) {
            Tooltip(text, ignoreWidth);
        }
    }

    public static void Tooltip(string text, bool ignoreWidth = false) {
        if(string.IsNullOrEmpty(text)) {
            GUI.Box(new Rect(0, 0, 0, 0), "");
        } else {
            Vector2 mousePosition = Event.current.mousePosition;

            Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(text));
            Rect labelPosition = new(mousePosition.x, mousePosition.y - 40, 0, 0);

            if(!ignoreWidth) {
                var windowwidth = ((Rect)AccessTools.Field(typeof(UnityModManagerNet.UnityModManager.UI), "mWindowRect")
                        .GetValue(UnityModManagerNet.UnityModManager.UI.Instance))
                    .width;
                var scroll = (Vector2[])AccessTools.Field(typeof(UnityModManagerNet.UnityModManager.UI), "mScrollPosition")
                    .GetValue(UnityModManagerNet.UnityModManager.UI.Instance);
                windowwidth += scroll[UnityModManagerNet.UnityModManager.UI.Instance.tabId].x;
                if(labelPosition.x + textSize.x + 20 + 20 > windowwidth) {
                    labelPosition.x = windowwidth - textSize.x - 20 - 20;
                }
            }

            labelPosition.width = textSize.x + 20;
            labelPosition.height = textSize.y + 20;
            GUI.Box(labelPosition, "", RGUIStyle.darkWindow);

            labelPosition.x += 10;
            labelPosition.y += 10;
            GUI.Label(labelPosition, text);
        }
    }

    private static bool isImageInited = false;

    public static Texture2D Icon_Active;
    public static Texture2D Icon_Inactive;

    public static Texture2D Icon_Copy;
    public static Texture2D Icon_Gradation;
    public static Texture2D Icon_UpDown;
    public static Texture2D Icon_LeftRight;
    public static Texture2D Icon_XRotate;
    public static Texture2D Icon_YRotate;
    public static Texture2D Icon_ZRotate;
    public static Texture2D Icon_Sun;
    public static Texture2D Icon_Blur;
    public static Texture2D Icon_Scale;
    public static Texture2D Icon_Offset;
    public static Texture2D Icon_Rotate;
    public static Texture2D Icon_Duration;
    public static Texture2D Icon_Speed;
    public static Texture2D Icon_Lenght;
    public static Texture2D Icon_Softness;
    public static Texture2D Icon_Roundness;
    public static Texture2D Icon_PoolSize;
    public static Texture2D Icon_Color;
    public static Texture2D Icon_Image;
    public static Texture2D Icon_Sequential;
    public static Texture2D Icon_Random;

    public static Texture2D Icon_Up;
    public static Texture2D Icon_Down;
    public static Texture2D Icon_Left;
    public static Texture2D Icon_Right;
    public static Texture2D Icon_UpLeft;
    public static Texture2D Icon_UpRight;
    public static Texture2D Icon_DownLeft;
    public static Texture2D Icon_DownRight;
    public static Texture2D Icon_Center;

    public static Texture2D Icon_AnchorTopLeft;
    public static Texture2D Icon_AnchorTopCenter;
    public static Texture2D Icon_AnchorTopRight;
    public static Texture2D Icon_AnchorMiddleLeft;
    public static Texture2D Icon_AnchorMiddleCenter;
    public static Texture2D Icon_AnchorMiddleRight;
    public static Texture2D Icon_AnchorBottomLeft;
    public static Texture2D Icon_AnchorBottomCenter;
    public static Texture2D Icon_AnchorBottomRight;
    public static Texture2D Icon_AnchorHorizontalStretchTop;
    public static Texture2D Icon_AnchorHorizontalStretchMiddle;
    public static Texture2D Icon_AnchorHorizontalStretchBottom;
    public static Texture2D Icon_AnchorVerticalStretchLeft;
    public static Texture2D Icon_AnchorVerticalStretchCenter;
    public static Texture2D Icon_AnchorVerticalStretchRight;
    public static Texture2D Icon_AnchorFullStretch;

    public static Texture2D Icon_EaseLinear;
    public static Texture2D Icon_EaseInSine;
    public static Texture2D Icon_EaseOutSine;
    public static Texture2D Icon_EaseInOutSine;
    public static Texture2D Icon_EaseInQuad;
    public static Texture2D Icon_EaseOutQuad;
    public static Texture2D Icon_EaseInOutQuad;
    public static Texture2D Icon_EaseInCubic;
    public static Texture2D Icon_EaseOutCubic;
    public static Texture2D Icon_EaseInOutCubic;
    public static Texture2D Icon_EaseInQuart;
    public static Texture2D Icon_EaseOutQuart;
    public static Texture2D Icon_EaseInOutQuart;
    public static Texture2D Icon_EaseInQuint;
    public static Texture2D Icon_EaseOutQuint;
    public static Texture2D Icon_EaseInOutQuint;
    public static Texture2D Icon_EaseInExpo;
    public static Texture2D Icon_EaseOutExpo;
    public static Texture2D Icon_EaseInOutExpo;
    public static Texture2D Icon_EaseInCirc;
    public static Texture2D Icon_EaseOutCirc;
    public static Texture2D Icon_EaseInOutCirc;
    public static Texture2D Icon_EaseInElastic;
    public static Texture2D Icon_EaseOutElastic;
    public static Texture2D Icon_EaseInOutElastic;
    public static Texture2D Icon_EaseInBack;
    public static Texture2D Icon_EaseOutBack;
    public static Texture2D Icon_EaseInOutBack;
    public static Texture2D Icon_EaseInBounce;
    public static Texture2D Icon_EaseOutBounce;
    public static Texture2D Icon_EaseInOutBounce;

    public static void InitializeImages() {
        if(isImageInited) {
            return;
        }

        dulgray = new Texture2D(1, 1);
        dulgray.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f));
        dulgray.Apply();

        gray = new Texture2D(1, 1);
        gray.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
        gray.Apply();

        jittengray = new Texture2D(1, 1);
        jittengray.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f));
        jittengray.Apply();

        tfgray = new Texture2D(1, 1);
        tfgray.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
        tfgray.Apply();

        veryjittengray = new Texture2D(1, 1);
        veryjittengray.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
        veryjittengray.Apply();

        outlineimg = new Texture2D(1, 1, TextureFormat.RGBA32, false, true) {
            filterMode = FilterMode.Point
        };
        outlineimg.LoadImage(ImageManager.GetResourceBytes("RGUIoutline.png"));

        black = new Texture2D(1, 1);
        black.SetPixel(0, 0, Color.black);
        black.Apply();

        Icon_Copy = CreateTextureFromByte(ImageManager.GetResourceBytes("copy.png"));
        Icon_Active = CreateTextureFromByte(ImageManager.GetResourceBytes("active.png"));
        Icon_Inactive = CreateTextureFromByte(ImageManager.GetResourceBytes("inactive.png"));
        Icon_Gradation = CreateTextureFromByte(ImageManager.GetResourceBytes("gradation.png"));
        Icon_UpDown = CreateTextureFromByte(ImageManager.GetResourceBytes("updown.png"));
        Icon_LeftRight = RotateTexture90(Icon_UpDown);
        Icon_XRotate = CreateTextureFromByte(ImageManager.GetResourceBytes("xrotate.png"));
        Icon_YRotate = RotateTexture90(Icon_XRotate);
        Icon_ZRotate = CreateTextureFromByte(ImageManager.GetResourceBytes("zrotate.png"));
        Icon_Sun = CreateTextureFromByte(ImageManager.GetResourceBytes("sun.png"));
        Icon_Blur = CreateTextureFromByte(ImageManager.GetResourceBytes("blur.png"));
        Icon_Scale = CreateTextureFromByte(ImageManager.GetResourceBytes("scale.png"));
        Icon_Offset = CreateTextureFromByte(ImageManager.GetResourceBytes("offset.png"));
        Icon_Rotate = CreateTextureFromByte(ImageManager.GetResourceBytes("rotate.png"));
        Icon_Duration = CreateTextureFromByte(ImageManager.GetResourceBytes("duration.png"));
        Icon_Speed = CreateTextureFromByte(ImageManager.GetResourceBytes("speed.png"));
        Icon_Lenght = CreateTextureFromByte(ImageManager.GetResourceBytes("length.png"));
        Icon_Softness = CreateTextureFromByte(ImageManager.GetResourceBytes("softness.png"));
        Icon_Roundness = CreateTextureFromByte(ImageManager.GetResourceBytes("roundness.png"));
        Icon_PoolSize = CreateTextureFromByte(ImageManager.GetResourceBytes("poolsize.png"));
        Icon_Color = CreateTextureFromByte(ImageManager.GetResourceBytes("color.png"));
        Icon_Image = CreateTextureFromByte(ImageManager.GetResourceBytes("image.png"));
        Icon_Sequential = CreateTextureFromByte(ImageManager.GetResourceBytes("sequential.png"));
        Icon_Random = CreateTextureFromByte(ImageManager.GetResourceBytes("random.png"));

        Icon_Up = CreateTextureFromByte(ImageManager.GetResourceBytes("up.png"));
        Icon_Left = RotateTexture90(Icon_Up);
        Icon_Down = RotateTexture90(Icon_Left);
        Icon_Right = RotateTexture90(Icon_Down);
        Icon_UpLeft = CreateTextureFromByte(ImageManager.GetResourceBytes("upleft.png"));
        Icon_DownLeft = RotateTexture90(Icon_UpLeft);
        Icon_DownRight = RotateTexture90(Icon_DownLeft);
        Icon_UpRight = RotateTexture90(Icon_DownRight);
        Icon_Center = CreateTextureFromByte(ImageManager.GetResourceBytes("center.png"));

        Icon_AnchorTopLeft = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchortopleft.png")));
        Icon_AnchorBottomLeft = RotateTexture90(Icon_AnchorTopLeft);
        Icon_AnchorBottomRight = RotateTexture90(Icon_AnchorBottomLeft);
        Icon_AnchorTopRight = RotateTexture90(Icon_AnchorBottomRight);
        Icon_AnchorMiddleLeft = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchormiddleleft.png")));
        Icon_AnchorBottomCenter = RotateTexture90(Icon_AnchorMiddleLeft);
        Icon_AnchorMiddleRight = RotateTexture90(Icon_AnchorBottomCenter);
        Icon_AnchorTopCenter = RotateTexture90(Icon_AnchorMiddleRight);
        Icon_AnchorMiddleCenter = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchormiddlecenter.png")));
        Icon_AnchorHorizontalStretchTop = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchorhorizontalstretchtop.png")));
        Icon_AnchorVerticalStretchLeft = RotateTexture90(Icon_AnchorHorizontalStretchTop);
        Icon_AnchorHorizontalStretchBottom = RotateTexture90(Icon_AnchorVerticalStretchLeft);
        Icon_AnchorVerticalStretchRight = RotateTexture90(Icon_AnchorHorizontalStretchBottom);
        Icon_AnchorHorizontalStretchMiddle = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchorhorizontalstretchmiddle.png")));
        Icon_AnchorVerticalStretchCenter = RotateTexture90(Icon_AnchorHorizontalStretchMiddle);
        Icon_AnchorFullStretch = DoubleTexture(CreateTextureFromByte(ImageManager.GetResourceBytes("anchorfullstretch.png")));

        Icon_EaseLinear = CreateTextureFromByte(ImageManager.GetResourceBytes("easelinear.png"));
        Icon_EaseInSine = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinsine.png"));
        Icon_EaseOutSine = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutsine.png"));
        Icon_EaseInOutSine = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutsine.png"));
        Icon_EaseInQuad = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinquad.png"));
        Icon_EaseOutQuad = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutquad.png"));
        Icon_EaseInOutQuad = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutquad.png"));
        Icon_EaseInCubic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeincubic.png"));
        Icon_EaseOutCubic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutcubic.png"));
        Icon_EaseInOutCubic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutcubic.png"));
        Icon_EaseInQuart = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinquart.png"));
        Icon_EaseOutQuart = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutquart.png"));
        Icon_EaseInOutQuart = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutquart.png"));
        Icon_EaseInQuint = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinquint.png"));
        Icon_EaseOutQuint = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutquint.png"));
        Icon_EaseInOutQuint = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutquint.png"));
        Icon_EaseInExpo = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinexpo.png"));
        Icon_EaseOutExpo = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutexpo.png"));
        Icon_EaseInOutExpo = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutexpo.png"));
        Icon_EaseInCirc = CreateTextureFromByte(ImageManager.GetResourceBytes("easeincirc.png"));
        Icon_EaseOutCirc = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutcirc.png"));
        Icon_EaseInOutCirc = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutcirc.png"));
        Icon_EaseInElastic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinelastic.png"));
        Icon_EaseOutElastic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutelastic.png"));
        Icon_EaseInOutElastic = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutelastic.png"));
        Icon_EaseInBack = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinback.png"));
        Icon_EaseOutBack = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutback.png"));
        Icon_EaseInOutBack = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutback.png"));
        Icon_EaseInBounce = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinbounce.png"));
        Icon_EaseOutBounce = CreateTextureFromByte(ImageManager.GetResourceBytes("easeoutbounce.png"));
        Icon_EaseInOutBounce = CreateTextureFromByte(ImageManager.GetResourceBytes("easeinoutbounce.png"));

        isImageInited = true;
    }

    public static Texture2D DoubleTexture(Texture2D src) {
        int w = src.width * 2;
        int h = src.height * 2;
        Texture2D dst = new(w, h, src.format, false) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        for(int y = 0; y < h; y++) {
            for(int x = 0; x < w; x++) {
                dst.SetPixel(x, y, src.GetPixel(x / 2, y / 2));
            }
        }

        dst.Apply();
        return dst;
    }

    public static Texture2D RotateTexture90(Texture2D tex) {
        int w = tex.width;
        int h = tex.height;

        Texture2D rotTex = new(h, w, tex.format, false);
        Color[] original = tex.GetPixels();
        Color[] rotated = new Color[original.Length];

        for(int y = 0; y < h; y++) {
            for(int x = 0; x < w; x++) {
                rotated[(x * h) + (h - y - 1)] = original[(y * w) + x];
            }
        }

        rotTex.SetPixels(rotated);
        rotTex.Apply();
        return rotTex;
    }

    public static Texture2D RotateTexture180(Texture2D tex) {
        int w = tex.width;
        int h = tex.height;

        Texture2D rotTex = new(w, h, tex.format, false);
        Color[] original = tex.GetPixels();
        Color[] rotated = new Color[original.Length];

        for(int i = 0; i < original.Length; i++) {
            rotated[original.Length - 1 - i] = original[i];
        }

        rotTex.SetPixels(rotated);
        rotTex.Apply();
        return rotTex;
    }

    public static Texture2D CreateTextureFromByte(byte[] bytes) {
        Texture2D texture = new(1, 1, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes);
        return texture;
    }

    public static GUIStyle myButton;
    public static GUIStyle myTextField;
    public static GUIStyle myTextFieldNoPad;
    public static GUIStyle mySlider;
    public static GUIStyle myThumb;

    public static void SetStyle(bool legacy) {
        if(legacy) {
            myButton.normal.background = GUI.skin.button.normal.background;
            myButton.active.background = GUI.skin.button.active.background;
            myButton.hover.background = GUI.skin.button.hover.background;
            myTextField.normal.background = GUI.skin.textField.normal.background;
            myTextField.focused.background = GUI.skin.textField.focused.background;
            myTextField.hover.background = GUI.skin.textField.hover.background;
            myTextFieldNoPad.normal.background = GUI.skin.textField.normal.background;
            myTextFieldNoPad.focused.background = GUI.skin.textField.focused.background;
            myTextFieldNoPad.hover.background = GUI.skin.textField.hover.background;
            mySlider.normal.background = GUI.skin.horizontalSlider.normal.background;
            myThumb.normal.background = GUI.skin.horizontalSliderThumb.normal.background;
            myThumb.active.background = GUI.skin.horizontalSliderThumb.active.background;
            myThumb.hover.background = GUI.skin.horizontalSliderThumb.hover.background;
        } else if(isImageInited) {
            myButton.normal.background = gray;
            myButton.active.background = dulgray;
            myButton.hover.background = dulgray;
            myTextField.normal.background = tfgray;
            myTextField.focused.background = tfgray;
            myTextField.hover.background = tfgray;
            myTextFieldNoPad.normal.background = tfgray;
            myTextFieldNoPad.focused.background = tfgray;
            myTextFieldNoPad.hover.background = tfgray;
            mySlider.normal.background = jittengray;
            myThumb.normal.background = gray;
            myThumb.active.background = dulgray;
            myThumb.hover.background = dulgray;
        }
    }

    public static Texture2D veryjittengray;
    public static Texture2D gray;
    public static Texture2D dulgray;
    public static Texture2D jittengray;
    public static Texture2D tfgray;
    public static Texture2D outlineimg;
    public static Texture2D black;

    private static GUIStyle nopadButton;

    static Drawer() {
        InitializeImages();

        myButton = new GUIStyle(GUI.skin.button);
        myTextField = new GUIStyle(GUI.skin.textField);
        myTextFieldNoPad = new GUIStyle(myTextField);
        myTextField.padding.right = 40;
        mySlider = new GUIStyle(GUI.skin.horizontalSlider);
        myThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
        SetStyle(Main.Settings.UseLegacyTheme);

        nopadButton = new GUIStyle(myButton) {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0)
        };
    }
}
