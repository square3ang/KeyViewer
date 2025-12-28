using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Utils;
using RapidGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Core
{
    public delegate bool CustomDrawer<T>(T t);
    public delegate bool CustomDrawerRef<T>(ref T t);
    public static class Drawer {
        #region Custom Drawers
        /*
         * CD_[H/V]_[TYPE][[Additional Attributes]]
         * CD => Custom Drawer
         * H/V => Horizontal Or Vertical
         * TYPE => Drawing Type
         * 
         * Additional Attributes
         * A_B => A to B
         * A_B_C => A to B & Width Is C
         * *ONLY => * Only Drawer
         */
        public static bool CD_V_VEC2_0_10_300(ref Vector2 vec2) {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec2.x, 0, 10, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec2.y, 0, 10, 300f);
            return result;
        }
        public static bool CD_V_VEC3_0_10_300(ref Vector3 vec3) {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec3.x, 0, 10, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, 0, 10, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, 0, 10, 300f);
            return result;
        }
        public static bool CD_V_VEC3_WIDTH_HEIGHT_Z_300(ref Vector3 vec3) {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec3.x, -Screen.width, Screen.width, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, -Screen.height, Screen.height, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, -10, 10, 300f);
            return result;
        }
        public static bool CD_V_VEC3_M180_180_300(ref Vector3 vec3) {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec3.x, -180, 180, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, -180, 180, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, -180, 180, 300f);
            return result;
        }
        public static bool CD_H_STR(ref string str) {
            string prev = str;
            GUILayout.BeginHorizontal();
            str = GUILayout.TextField(str);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != str;
        }
        public static bool CD_H_STR_TRIMQUOTE(ref string str) {
            string prev = str;
            GUILayout.BeginHorizontal();
            str = GUILayout.TextField(str);
            str = str.TrimQuote();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != str;
        }
        public static bool CD_H_FLT(ref float val) {
            string str = val.ToString();
            bool result = CD_H_STR(ref str);
            if(result)
                val = StringConverter.ToFloat(str);
            return result;
        }
        public static bool CD_H_FLT_SPEEDONLY(ref float val) {
            return DrawSingleWithSlider(Main.Lang.Get("MISC_SPEED", "Speed"), ref val, 0, 500, 300f);
        }
        public static bool CD_H_FLT_LENGTHONLY(ref float val) {
            return DrawSingleWithSlider(Main.Lang.Get("MISC_LENGTH", "Length"), ref val, 0, 500, 300f);
        }
        public static bool CD_H_INT32_SOFTNESSONLY(ref int val) {
            float fVal = val;
            if(DrawSingleWithSlider(Main.Lang.Get("MISC_SOFTNESS", "Softness"), ref fVal, 0, 500, 300f)) {
                val = (int)Math.Round(fVal);
                return true;
            }
            return false;
        }
        public static bool CD_H_INT32_POOLSIZEONLY(ref int val) {
            float fVal = val;
            if(DrawSingleWithSlider(Main.Lang.Get("MISC_SIZE", "Size"), ref fVal, 0, 500, 300f)) {
                val = (int)Math.Round(fVal);
                return true;
            }
            return false;
        }
        public static bool CD_V_EASECONFIG(EaseConfig config) {
            bool result = false;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Main.Lang.Get("MISC_EASE", "Ease"));
                result |= DrawEnum(Main.Lang.Get("MISC_EASE", "Ease"), ref config.Ease, config.GetHashCode());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            result |= DrawSingleWithSlider(Main.Lang.Get("MISC_DURATION", "Duration"), ref config.Duration, 0, 5, 300);
            return result;
        }
        #endregion

        public static bool DrawBlurConfig(string objName, BlurConfig blurConfig) {
            bool result = false;
            bool force = true;
            GUILayoutEx.ExpandableGUI(() => {
                GUILayout.BeginHorizontal();
                result |= DrawSingleWithSlider(Main.Lang.Get("MISC_SIZE", "Size"), ref blurConfig.Spacing, 0, 40, 300f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                result |= DrawSingleWithSlider(Main.Lang.Get("MISC_VIBRANCY", "Vibrancy"), ref blurConfig.Vibrancy, 0, 2, 300f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }, string.Format(Main.Lang.Get("MISC_BLUR_CONFIG", "{0} Blur Config"), objName), ref force);
            return result;
        }
        public static void DrawObjectConfig(string label, string objName, ObjectConfig objConfig, Action onChange) {
            bool force = true;
            TitleButton(label, Main.Lang.Get("MISC_EDIT", "Edit"), () => {
                string bts = string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_OBJECTCONFIG_FROM", "Object Config From {0}"), objName));
                Main.GUI.Push(new MethodDrawable(() => {
                    GUILayoutEx.ExpandableGUI((() => {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(Main.Lang.Get("MISC_PRESSED", "Pressed"));
                            DrawGColor(ref objConfig.Color.Pressed).IfTrue(onChange);
                        }
                        GUILayout.EndVertical();
                        if(CanEase<GColor>.Value)
                            DrawEaseConfig(Main.Lang.Get("MISC_PRESSED_EASE", "Pressed Ease"), objConfig.Color.PressedEase);
                        TitleButton(Main.Lang.Get("MISC_COPY_FROM_RELEASED", "Copy From Released"), Main.Lang.Get("MISC_COPY", "Copy"), () => {
                            objConfig.Color.Pressed = objConfig.Color.Released.Copy();
                            objConfig.Color.PressedEase = objConfig.Color.ReleasedEase.Copy();
                            onChange();
                        });

                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(Main.Lang.Get("MISC_RELEASED", "Released"));
                            DrawGColor(ref objConfig.Color.Released).IfTrue(onChange);
                        }
                        GUILayout.EndVertical();
                        if(CanEase<GColor>.Value)
                            DrawEaseConfig(Main.Lang.Get("MISC_PRESSED_EASE", "Pressed Ease"), objConfig.Color.ReleasedEase);
                        TitleButton(Main.Lang.Get("MISC_COPY_FROM_PRESSED", "Copy From Pressed"), Main.Lang.Get("MISC_COPY", "Copy"), () => {
                            objConfig.Color.Released = objConfig.Color.Pressed;
                            objConfig.Color.ReleasedEase = objConfig.Color.PressedEase.Copy();
                        });
                    }), Main.Lang.Get("MISC_COLOR", "Color"), ref force);
                    string title = string.Format(string.Format(Main.Lang.Get("MISC_JUDGECOLOR_FROM", "Judge Color From {0}"), objName));
                    DrawObjectConfig(objConfig, j => {
                        bool judgeChanged = false;
                        TitleButton(string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), Main.Lang.Get("MISC_JUDGECOLOR", "Judge Color")), Main.Lang.Get("MISC_EDIT", "Edit"), () => {
                            Main.GUI.Push(new MethodDrawable(() => {
                                var colors = objConfig.JudgeColors;
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_TOO_EARLY", "<color=#ED3E3E>Too Early</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.TooEarly), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_TOO_EARLY", "<color=#ED3E3E>Too Early</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_VERY_EARLY", "<color=#EB9A46>Very Early</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.VeryEarly), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_VERY_EARLY", "<color=#EB9A46>Very Early</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_EARLY_PERFECT", "<color=#E3E370>Early Perfect</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.EarlyPerfect), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_EARLY_PERFECT", "<color=#E3E370>Early Perfect</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_PERFECT", "<color=#86E370>Perfect</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.Perfect), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_PERFECT", "<color=#86E370>Perfect</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_LATE_PERFECT", "<color=#E3E370>Late Perfect</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.LatePerfect), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_LATE_PERFECT", "<color=#E3E370>Late Perfect</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_VERY_LATE", "<color=#EB9A46>Very Late</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.VeryLate), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_VERY_LATE", "<color=#EB9A46>Very Late</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_TOO_LATE", "<color=#ED3E3E>Too Late</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.TooLate), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_TOO_LATE", "<color=#ED3E3E>Too Late</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_MULTI_PRESS", "<color=#00FFED>Multipress</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.Multipress), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_MULTI_PRESS", "<color=#00FFED>Multipress</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_FAIL_MISS", "<color=#DA59FF>Miss</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.FailMiss), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_FAIL_MISS", "<color=#DA59FF>Miss</color>"))))));
                                TitleButton(string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_FAIL_OVERLOAD", "<color=#DA59FF>Overload</color>")), Main.Lang.Get("MISC_EDIT", "Edit"), () => Main.GUI.Push(new MethodDrawable(() => DrawGColor(ref colors.FailOverload), string.Format(Main.Lang.Get("MISC_EDIT_THIS", "Edit {0}"), string.Format(Main.Lang.Get("MISC_THIS_COLOR", "{0} Color"), Main.Lang.Get("MISC_FAIL_OVERLOAD", "<color=#DA59FF>Overload</color>"))))));
                                if(CanEase<GColor>.Value)
                                    DrawEaseConfig(Main.Lang.Get("MISC_EASE", "Ease"), objConfig.JudgeColorEase);
                            }, title));
                        });
                        return judgeChanged;
                    }).IfTrue(onChange);
                }, bts));
            });
        }
        public static void TitleButton(string label, string btnLabel, Action pressed) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            if(Drawer.Button(btnLabel))
                pressed?.Invoke();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static bool DrawGColor(ref GColor color) {
            bool ge = color.gradientEnabled;
            if(DrawBool(Main.Lang.Get("MISC_ENABLE_GRADIENT", "Enable Gradient"), ref ge))
                color = color with { gradientEnabled = ge };
            bool result = false;
            if(color.gradientEnabled) {
                Color tl = color.topLeft, tr = color.topRight,
                bl = color.bottomLeft, br = color.bottomRight;
                ExpandableGUI(Main.Lang.Get("MISC_TOP_LEFT", "Top Left"), () => result |= DrawColor(ref tl));
                ExpandableGUI(Main.Lang.Get("MISC_TOP_RIGHT", "Top Right"), () => result |= DrawColor(ref tr));
                ExpandableGUI(Main.Lang.Get("MISC_BOTTOM_LEFT", "Bottom Left"), () => result |= DrawColor(ref bl));
                ExpandableGUI(Main.Lang.Get("MISC_BOTTOM_RIGHT", "Bottom Right"), () => result |= DrawColor(ref br));
                if(result) {
                    color.topLeft = tl;
                    color.topRight = tr;
                    color.bottomLeft = bl;
                    color.bottomRight = br;
                }
            } else {
                Color dummy = color.topLeft;
                if(result = DrawColor(ref dummy))
                    color = dummy;
            }
            return result;
        }
        public static bool DrawColor(ref Color color) {
            bool result = false;
            result |= DrawSingleWithSlider("<color=#FF0000>R</color>", ref color.r, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#00FF00>G</color>", ref color.g, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#0000FF>B</color>", ref color.b, 0, 1, 300f);
            result |= DrawSingleWithSlider("A", ref color.a, 0, 1, 300f);
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            if(DrawString("Hex:", ref hex)) {
                result = true;
                ColorUtility.TryParseHtmlString("#" + hex, out color);
            }
            return result;
        }
        public static bool DrawObjectConfig(ObjectConfig objConfig, CustomDrawer<JudgeM<GColor>> judgeColorDrawer) {
            bool result = DrawVectorConfig(objConfig.VectorConfig);
            if(DrawBool(Main.Lang.Get("MISC_CHANGE_COLOR_WITH_JUDGE", "Change Color With Judge"), ref objConfig.ChangeColorWithJudge)) {
                result = true;
                if(objConfig.ChangeColorWithJudge) {
                    var jc = objConfig.JudgeColors = new JudgeM<GColor>();
                    jc.TooEarly = Constants.TooEarlyColor;
                    jc.VeryEarly = Constants.VeryEarlyColor;
                    jc.EarlyPerfect = Constants.EarlyPerfectColor;
                    jc.Perfect = Constants.PerfectColor;
                    jc.LatePerfect = Constants.LatePerfectColor;
                    jc.VeryLate = Constants.VeryLateColor;
                    jc.TooLate = Constants.TooLateColor;
                    jc.Multipress = Constants.MultipressColor;
                    jc.FailMiss = Constants.FailMissColor;
                    jc.FailOverload = Constants.FailOverloadColor;
                } else
                    objConfig.JudgeColors = null;
            }
            if(objConfig.ChangeColorWithJudge)
                result |= judgeColorDrawer?.Invoke(objConfig.JudgeColors) ?? false;
            return result;
        }
        public static bool DrawVectorConfig(VectorConfig vConfig) {
            bool result = false;
            result |= DrawPressReleaseV(Main.Lang.Get("MISC_SCALE", "Scale"), vConfig.Scale, CD_V_VEC2_0_10_300);
            result |= DrawPressReleaseV(Main.Lang.Get("MISC_OFFSET", "Offset"), vConfig.Offset, CD_V_VEC3_WIDTH_HEIGHT_Z_300);
            result |= DrawPressReleaseV(Main.Lang.Get("MISC_ROTATION", "Rotation"), vConfig.Rotation, CD_V_VEC3_M180_180_300);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Main.Lang.Get("MISC_PIVOT", "Pivot"));
                result |= DrawEnum(Main.Lang.Get("MISC_PIVOT", "Pivot"), ref vConfig.Pivot, vConfig.GetHashCode());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Main.Lang.Get("MISC_ANCHOR", "Anchor"));
                result |= DrawEnum(Main.Lang.Get("MISC_ANCHOR", "Anchor"), ref vConfig.Anchor, vConfig.GetHashCode());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return result;
        }
        public static bool DrawVector2WithSlider(string label, ref Vector2 vec2, float lValue, float rValue) {
            bool result = false;
            GUILayout.Label($"<b>{label}</b>");
            result |= DrawSingleWithSlider("X:", ref vec2.x, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec2.y, lValue, rValue, 300f);
            return result;
        }
        public static bool DrawVector3WithSlider(string label, ref Vector3 vec3, float lValue, float rValue) {
            bool result = false;
            GUILayout.Label($"<b>{label}</b>");
            result |= DrawSingleWithSlider("X:", ref vec3.x, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, lValue, rValue, 300f);
            return result;
        }
        public static bool DrawPressReleaseH<T>(string label, PressRelease<T> pr, CustomDrawerRef<T> drawer) {
            return DrawPressReleaseBase(label, pr, drawer, GUILayout.BeginHorizontal, GUILayout.EndHorizontal);
        }
        public static bool DrawPressReleaseV<T>(string label, PressRelease<T> pr, CustomDrawerRef<T> drawer) {
            return DrawPressReleaseBase(label, pr, drawer, GUILayout.BeginVertical, GUILayout.EndVertical);
        }
        public static bool DrawPressReleaseBase<T>(string label, PressRelease<T> pr, CustomDrawerRef<T> drawer, Action<GUILayoutOption[]> begin, Action end) {
            var emptyOptions = Array.Empty<GUILayoutOption>();
            bool changed = false;
            bool force = true;
            GUILayoutEx.ExpandableGUI((Action)(() => {
                begin(emptyOptions);
                {
                    GUILayout.Label(Main.Lang.Get("MISC_PRESSED", "Pressed"));
                    changed |= drawer(ref pr.Pressed);
                }
                end();
                if(CanEase<T>.Value)
                    DrawEaseConfig(Main.Lang.Get("MISC_PRESSED_EASE", "Pressed Ease"), pr.PressedEase);
                TitleButton(Main.Lang.Get("MISC_COPY_FROM_RELEASED", "Copy From Released"), Main.Lang.Get("MISC_COPY", "Copy"), () => {
                    object released = pr.Released;
                    if(released is ICopyable<T> copyable)
                        pr.Pressed = copyable.Copy();
                    else
                        pr.Pressed = pr.Released;
                    pr.PressedEase = pr.ReleasedEase.Copy();
                    changed = true;
                });

                begin(emptyOptions);
                {
                    GUILayout.Label(Main.Lang.Get("MISC_RELEASED", "Released"));
                    changed |= drawer(ref pr.Released);
                }
                end();
                if(CanEase<T>.Value)
                    DrawEaseConfig(Main.Lang.Get("MISC_RELEASED_EASE", "Released Ease"), pr.ReleasedEase);
                TitleButton(Main.Lang.Get("MISC_COPY_FROM_PRESSED", "Copy From Pressed"), Main.Lang.Get("MISC_COPY", "Copy"), () => {
                    object pressed = pr.Pressed;
                    if(pressed is ICopyable<T> copyable)
                        pr.Released = copyable.Copy();
                    else
                        pr.Released = pr.Pressed;
                    pr.ReleasedEase = pr.PressedEase.Copy();
                    changed = true;
                });
            }), label, ref force);
            return changed;
        }
        public static bool DrawEaseConfig(string label, EaseConfig easeConfig) {
            bool changed = false;
            bool force = true;
            GUILayoutEx.ExpandableGUI(() => {
                changed = CD_V_EASECONFIG(easeConfig);
            }, label, ref force);
            return changed;
        }
        public static void ExpandableGUI(string label, Action drawer) {
            bool force = true;
            GUILayoutEx.ExpandableGUI(drawer, label, ref force);
        }
        public static bool DrawSingleWithSlider(string label, ref float value, float lValue, float rValue, float width) {
            GUILayout.BeginHorizontal();
            float newValue = GUILayoutEx.NamedSliderContent(label, value, lValue, rValue, width);
            GUILayout.EndHorizontal();
            bool result = newValue != value;
            value = newValue;
            return result;
        }
        public static bool DrawStringArray(ref string[] array, Action<int> arrayResized = null, Action<int> elementRightGUI = null, Action<int, string> onElementChange = null) {
            bool result = false;
            GUILayout.BeginHorizontal();
            if(Drawer.Button("+")) {
                Array.Resize(ref array, array.Length + 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            if(array.Length > 0 && Drawer.Button("-")) {
                Array.Resize(ref array, array.Length - 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for(int i = 0; i < array.Length; i++) {
                string cache = array[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{i}: ");
                cache = GUILayout.TextField(cache);
                elementRightGUI?.Invoke(i);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if(cache != array[i]) {
                    array[i] = cache;
                    onElementChange?.Invoke(i, cache);
                }
            }
            return result;
        }
        public static bool DrawArray(string label, ref object[] array) {
            bool result = false;
            GUILayout.Label(label);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if(Drawer.Button("+"))
                Array.Resize(ref array, array.Length + 1);
            if(array.Length > 0 && Drawer.Button("-"))
                Array.Resize(ref array, array.Length - 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for(int i = 0; i < array.Length; i++)
                result |= DrawObject($"{i}: ", ref array[i]);
            GUILayout.EndVertical();
            return result;
        }
        public static bool DrawList<T>(List<T> list, CustomDrawerRef<T> drawer) where T : new() {
            bool result = false;
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if(Drawer.Button("+")) {
                list.Add(new T());
                result = true;
            }
            if(list.Count > 0 && Drawer.Button("-")) {
                list.RemoveAt(list.Count - 1);
                result = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for(int i = 0; i < list.Count; i++) {
                T t = list[i];
                if(drawer(ref t)) {
                    list[i] = t;
                    result = true;
                }
                GUIStyle style = new GUIStyle() {
                    margin = new RectOffset(0, 0, 0, 10),
                    normal = new GUIStyleState() {
                        textColor = Color.white,
                    }
                };
                GUILayout.BeginHorizontal();
                GUILayoutEx.HorizontalLine(1, 95);
                GUILayout.Label(i.ToString(), style);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            return result;
        }
        public static bool DrawBool(string label, ref bool value) {
            bool prev = value;

            GUILayout.BeginHorizontal();

            if(Main.Settings.useLegacyTheme) {
                value = GUILayout.Toggle(value, "");
            } else {
                var old = GUI.backgroundColor;
                GUI.backgroundColor = Color.clear;
                var newskin = new GUIStyle(GUI.skin.button);
                newskin.fontSize = 16;
                newskin.margin = new RectOffset(0, 0, 4, 0);
                newskin.padding = new RectOffset(0, 0, 0, 0);

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

            if(Main.Settings.useLegacyTheme) {
                value = GUILayout.Toggle(value, "");
            } else {
                var old = GUI.backgroundColor;
                GUI.backgroundColor = Color.clear;
                var newskin = new GUIStyle(GUI.skin.button);
                newskin.fontSize = 16;
                newskin.margin = new RectOffset(0, 0, 4, 0);
                newskin.padding = new RectOffset(0, 0, 0, 0);

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

            if(Main.Settings.useLegacyTheme) {
                value = GUILayout.Toggle(value, "");
            } else {
                var old = GUI.backgroundColor;
                GUI.backgroundColor = Color.clear;
                var newskin = new GUIStyle(GUI.skin.button);
                newskin.fontSize = 16;
                newskin.margin = new RectOffset(0, 0, 4, 0);
                newskin.padding = new RectOffset(0, 0, 0, 0);

                if(GUILayout.Button(value ? Icon_Active : Icon_Inactive, newskin)) {
                    value = !value;
                }

                GUI.backgroundColor = old;
            }

            return prev != value;
        }
        public static bool DrawByte(string label, ref byte value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt8(str);
            return result;
        }
        public static bool DrawDouble(string label, ref double value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToDouble(str);
            return result;
        }
        public static bool DrawEnum<T>(string label, ref T @enum, int unique = 0) where T : Enum {
            int current = EnumHelper<T>.IndexOf(@enum);
            string[] names = EnumHelper<T>.GetNames();
            bool result = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref current, names, label, unique);
            @enum = EnumHelper<T>.GetValues()[current];
            return result;
        }
        public static bool DrawInt16(string label, ref short value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt16(str);
            return result;
        }
        public static bool DrawInt32(string label, ref int value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt32(str);
            return result;
        }
        public static bool DrawInt64(string label, ref long value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt64(str);
            return result;
        }
        public static void DrawObject(string label, object value) {
            if(value == null)
                return;
            if(value is IDrawable drawable) {
                drawable.Draw();
                return;
            }
            Type t = value.GetType();
            if(!t.IsPrimitive && t != typeof(string))
                return;
            var fields = t.GetFields();
            foreach(var field in fields) {
                var fValue = field.GetValue(value);
                if(DrawObject(field.Name, ref fValue))
                    field.SetValue(value, fValue);
            }
            var props = t.GetProperties();
            foreach(var prop in props.Where(p => p.CanRead && p.CanWrite)) {
                var pValue = prop.GetValue(value);
                if(DrawObject(prop.Name, ref pValue))
                    prop.SetValue(value, pValue);
            }
        }
        public static bool DrawObject(string label, ref object obj) {
            bool result = false;
            switch(obj) {
                case bool bb:
                    result = DrawBool(label, ref bb);
                    obj = bb;
                    break;
                case sbyte sb:
                    result = DrawSByte(label, ref sb);
                    obj = sb;
                    break;
                case byte b:
                    result = DrawByte(label, ref b);
                    obj = b;
                    break;
                case short s:
                    result = DrawInt16(label, ref s);
                    obj = s;
                    break;
                case ushort us:
                    result = DrawUInt16(label, ref us);
                    obj = us;
                    break;
                case int i:
                    result = DrawInt32(label, ref i);
                    obj = i;
                    break;
                case uint ui:
                    result = DrawUInt32(label, ref ui);
                    obj = ui;
                    break;
                case long l:
                    result = DrawInt64(label, ref l);
                    obj = l;
                    break;
                case ulong ul:
                    result = DrawUInt64(label, ref ul);
                    obj = ul;
                    break;
                case float f:
                    result = DrawSingle(label, ref f);
                    obj = f;
                    break;
                case double d:
                    result = DrawDouble(label, ref d);
                    obj = d;
                    break;
                case string str:
                    result = DrawString(label, ref str);
                    obj = str;
                    break;
                default:
                    GUILayout.Label($"{label}{obj}");
                    break;
            }
            return result;
        }
        public static bool DrawSByte(string label, ref sbyte value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt8(str);
            return result;
        }
        public static bool DrawSingle(string label, ref float value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToFloat(str);
            return result;
        }
        public static bool DrawString(string label, ref string value, bool textArea = false) {
            string prev = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            if(!textArea)
                value = GUILayout.TextField(value, myTextField);
            else
                value = GUILayout.TextArea(value, myTextField);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        public static bool DrawToggleGroup(string[] labels, bool[] toggleGroup) {
            bool result = false;
            for(int i = 0; i < labels.Length; i++)
                if(DrawBool(labels[i], ref toggleGroup[i])) {
                    result = true;
                    for(int j = 0; j < toggleGroup.Length; j++)
                        if(j == i)
                            continue;
                        else
                            toggleGroup[j] = false;
                    break;
                }
            return result;
        }
        public static bool DrawUInt16(string label, ref ushort value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt16(str);
            return result;
        }
        public static bool DrawUInt32(string label, ref uint value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt32(str);
            return result;
        }
        public static bool DrawUInt64(string label, ref ulong value) {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt64(str);
            return result;
        }
        public static bool Button(string str, params GUILayoutOption[] options) {
            return GUILayout.Button(str, myButton, options);
        }

        public static bool SelectionPopup(ref int selected, string[] options, string label,
            params GUILayoutOption[] layoutOptions) {
            if(label != "") {
                GUILayout.BeginHorizontal();
                GUILayout.Label(label);
            }

            var news = RGUI.SelectionPopup(selected, options, null, layoutOptions);
            var c = selected != news;

            selected = news;
            if(label != "")
                GUILayout.EndHorizontal();
            return c;
        }

        private static bool isImageInited = false;

        public static Texture2D Icon_Active;
        public static Texture2D Icon_Inactive;

        public static Texture2D Icon_Down;
        public static Texture2D Icon_Gradation;
        public static Texture2D Icon_Up;
        public static Texture2D Icon_UpDown;
        public static Texture2D Icon_LeftRight;
        public static Texture2D Icon_XRotate;
        public static Texture2D Icon_YRotate;
        public static Texture2D Icon_ZRotate;

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

            outlineimg = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
            outlineimg.filterMode = FilterMode.Point;
            outlineimg.LoadImage(ImageManager.GetResourceBytes("RGUIoutline.png"));

            black = new Texture2D(1, 1);
            black.SetPixel(0, 0, Color.black);
            black.Apply();

            Icon_Active = CreateTextureFromByte(ImageManager.GetResourceBytes("active.png"));
            Icon_Inactive = CreateTextureFromByte(ImageManager.GetResourceBytes("inactive.png"));
            Icon_Gradation = CreateTextureFromByte(ImageManager.GetResourceBytes("gradation.png"));
            Icon_UpDown = CreateTextureFromByte(ImageManager.GetResourceBytes("updown.png"));
            Icon_LeftRight = RotateTexture90(Icon_UpDown);
            Icon_XRotate = CreateTextureFromByte(ImageManager.GetResourceBytes("xrotate.png"));
            Icon_YRotate = RotateTexture90(Icon_XRotate);
            Icon_ZRotate = CreateTextureFromByte(ImageManager.GetResourceBytes("zrotate.png"));
            Icon_Up = CreateTextureFromByte(ImageManager.GetResourceBytes("up.png"));
            Icon_Down = RotateTexture180(Icon_Up);


            isImageInited = true;
        }

        public static Texture2D RotateTexture90(Texture2D tex) {
            int w = tex.width;
            int h = tex.height;

            Texture2D rotTex = new Texture2D(h, w, tex.format, false);
            Color[] original = tex.GetPixels();
            Color[] rotated = new Color[original.Length];

            for(int y = 0; y < h; y++) {
                for(int x = 0; x < w; x++) {
                    rotated[x * h + (h - y - 1)] = original[y * w + x];
                }
            }

            rotTex.SetPixels(rotated);
            rotTex.Apply();
            return rotTex;
        }

        public static Texture2D RotateTexture180(Texture2D tex) {
            int w = tex.width;
            int h = tex.height;

            Texture2D rotTex = new Texture2D(w, h, tex.format, false);
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
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
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

        static Drawer() {
            InitializeImages();

            myButton = new GUIStyle(GUI.skin.button);
            myTextField = new GUIStyle(GUI.skin.textField);
            myTextFieldNoPad = new GUIStyle(myTextField);
            myTextField.padding.right = 40;
            mySlider = new GUIStyle(GUI.skin.horizontalSlider);
            myThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
            SetStyle(Main.Settings.useLegacyTheme);
        }
    }
}
