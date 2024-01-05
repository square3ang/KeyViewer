using KeyViewer.Utils;
using System;
using TMPro;
using UnityEngine;
using KeyViewer.Core.Interfaces;
using System.Linq;
using KeyViewer.Models;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;
using Newtonsoft.Json.Linq;
using KeyViewer.Controllers;
using System.Collections.Generic;

namespace KeyViewer.Core
{
    public delegate bool CustomDrawer<T>(T t);
    public delegate bool CustomDrawerRef<T>(ref T t);
    public static class Drawer
    {
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
         */

        public static bool CD_V_VEC2_0_1_300(ref Vector2 vec2)
        {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec2.x, 0, 1, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec2.y, 0, 1, 300f);
            return result;
        }
        public static bool CD_V_VEC3_0_1_300(ref Vector3 vec3)
        {
            bool result = false;
            result |= DrawSingleWithSlider("X:", ref vec3.x, 0, 1, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, 0, 1, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, 0, 1, 300f);
            return result;
        }
        public static bool CD_H_STR(ref string str)
        {
            string prev = str;
            GUILayout.BeginHorizontal();
            str = GUILayout.TextField(str);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != str;
        }
        public static bool CD_H_FLT(ref float val)
        {
            string str = val.ToString();
            bool result = CD_H_STR(ref str);
            if (result) val = StringConverter.ToFloat(str);
            return result;
        }
        public static bool CD_H_FLT_SIZEONLY(ref float val)
        {
            return DrawSingleWithSlider(Main.Lang[TKM.Size], ref val, 0, 500, 300f);
        }
        public static bool CD_H_FLT_SPEEDONLY(ref float val)
        {
            return DrawSingleWithSlider(Main.Lang[TKM.Speed], ref val, 0, 500, 300f);
        }
        public static bool CD_H_FLT_LENGTHONLY(ref float val)
        {
            return DrawSingleWithSlider(Main.Lang[TKM.Length], ref val, 0, 500, 300f);
        }
        public static bool CD_H_INT32_SOFTNESSONLY(ref int val)
        {
            float fVal = val;
            if (DrawSingleWithSlider(Main.Lang[TKM.Softness], ref fVal, 0, 500, 300f))
            {
                val = (int)Math.Round(fVal);
                return true;
            }
            return false;
        }
        public static bool CD_H_INT32_POOLSIZEONLY(ref int val)
        {
            float fVal = val;
            if (DrawSingleWithSlider(Main.Lang[TKM.PoolSize], ref fVal, 0, 500, 300f))
            {
                val = (int)Math.Round(fVal);
                return true;
            }
            return false;
        }
        public static bool CD_V_EASECONFIG(EaseConfig config)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Main.Lang[TKM.Ease]);
                result |= DrawEnum(Main.Lang[TKM.Ease], ref config.Ease, config.EaseUniqueObject.GetHashCode());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            result |= DrawSingleWithSlider(Main.Lang[TKM.Duration], ref config.Duration, 0, 5, 300);
            return result;
        }
        #endregion

        public static void DrawObjectConfig(string label, string objName, ObjectConfig objConfig, Action onChange)
        {
            TitleButton(label, Main.Lang[TKM.EditThis], () =>
            {
                string bts = string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ObjectConfigFrom], objName));
                GUIController.Push(new MethodDrawable(() =>
                {
                    GUILayoutEx.ExpandableGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(Main.Lang[TKM.Pressed]);
                            DrawGColor(ref objConfig.Color.Pressed).IfTrue(onChange);
                        }
                        GUILayout.EndVertical();
                        TitleButton(Main.Lang[TKM.CopyFromReleased], Main.Lang[TKM.Copy], () =>
                        {
                            objConfig.Color.Pressed = objConfig.Color.Released._color;
                            onChange();
                        });

                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(Main.Lang[TKM.Released]);
                            DrawGColor(ref objConfig.Color.Released).IfTrue(onChange);
                        }
                        GUILayout.EndVertical();
                        TitleButton(Main.Lang[TKM.CopyFromPressed], Main.Lang[TKM.Copy], () =>
                        {
                            objConfig.Color.Released = objConfig.Color.Pressed._color;
                            onChange();
                        });
                    }, Main.Lang[TKM.Color], ref objConfig.Color.Status.Expanded);
                    string title = string.Format(string.Format(Main.Lang[TKM.JudgeColorFrom], objName));
                    DrawObjectConfig(objConfig, j =>
                    {
                        bool judgeChanged = false;
                        TitleButton(string.Format(Main.Lang[TKM.Edit], Main.Lang[TKM.JudgeColor]), Main.Lang[TKM.EditThis], () =>
                        {
                            GUIController.Push(new MethodDrawable(() =>
                            {
                                var colors = objConfig.JudgeColors;
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.TooEarly]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.TooEarly).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.TooEarly])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.VeryEarly]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.VeryEarly).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.VeryEarly])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.EarlyPerfect]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.EarlyPerfect).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.EarlyPerfect])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.Perfect]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.Perfect).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.Perfect])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.LatePerfect]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.LatePerfect).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.LatePerfect])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.VeryLate]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.VeryLate).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.VeryLate])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.TooLate]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.TooLate).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.TooLate])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.Multipress]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.Multipress).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.Multipress])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.FailMiss]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.FailMiss).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.FailMiss])))));
                                TitleButton(string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.FailOverload]), Main.Lang[TKM.EditThis], () => GUIController.Push(new MethodDrawable(() => DrawGColor(ref colors.FailOverload).IfTrue(onChange), string.Format(Main.Lang[TKM.Edit], string.Format(Main.Lang[TKM.ThisColor], Main.Lang[TKM.FailOverload])))));
                            }, title));
                        });
                        return judgeChanged;
                    }).IfTrue(onChange);
                }, bts));
            });
        }
        public static void TitleButton(string label, string btnLabel, Action pressed)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            if (GUILayout.Button(btnLabel))
                pressed?.Invoke();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static bool DrawGColor(ref GColor color)
        {
            bool ge = color.gradientEnabled;
            if (DrawBool(Main.Lang[TKM.EnableGradient], ref ge))
                color = color with { gradientEnabled = ge };
            bool result = false;
            if (color.gradientEnabled)
            {
                Color tl = color.topLeft, tr = color.topRight,
                bl = color.bottomLeft, br = color.bottomRight;
                ExpandableGUI(color.topLeftStatus, Main.Lang[TKM.TopLeft], () => result |= DrawColor(ref tl));
                ExpandableGUI(color.topRightStatus, Main.Lang[TKM.TopRight], () => result |= DrawColor(ref tr));
                ExpandableGUI(color.bottomLeftStatus, Main.Lang[TKM.BottomLeft], () => result |= DrawColor(ref bl));
                ExpandableGUI(color.bottomRightStatus, Main.Lang[TKM.BottomRight], () => result |= DrawColor(ref br));
                if (result)
                {
                    color.topLeft = tl;
                    color.topRight = tr;
                    color.bottomLeft = bl;
                    color.bottomRight = br;
                }
            }
            else
            {
                Color dummy = color.topLeft;
                if (result = DrawColor(ref dummy)) color = dummy;
            }
            return result;
        }
        public static bool DrawColor(ref Color color)
        {
            bool result = false;
            result |= DrawSingleWithSlider("<color=#FF0000>R</color>", ref color.r, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#00FF00>G</color>", ref color.g, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#0000FF>B</color>", ref color.b, 0, 1, 300f);
            result |= DrawSingleWithSlider("A", ref color.a, 0, 1, 300f);
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            if (DrawString("Hex:", ref hex))
            {
                result = true;
                ColorUtility.TryParseHtmlString("#" + hex, out color);
            }
            return result;
        }
        public static bool DrawObjectConfig(ObjectConfig objConfig, CustomDrawer<JudgeM<GColor>> judgeColorDrawer)
        {
            bool result = DrawVectorConfig(objConfig.VectorConfig);
            if (DrawBool(Main.Lang[TKM.ChangeColorWithJudge], ref objConfig.ChangeColorWithJudge))
            {
                result = true;
                if (objConfig.ChangeColorWithJudge)
                {
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
                }
                else objConfig.JudgeColors = null;
            }    
            if (objConfig.ChangeColorWithJudge)
                result |= judgeColorDrawer?.Invoke(objConfig.JudgeColors) ?? false;
            return result;
        }
        public static bool DrawVectorConfig(VectorConfig vConfig)
        {
            bool result = false;
            if (vConfig.UseSize)
                result |= DrawPressReleaseH(Main.Lang[TKM.Size], vConfig.Size, CD_H_FLT_SIZEONLY);
            else result |= DrawPressReleaseV(Main.Lang[TKM.Scale], vConfig.Scale, CD_V_VEC2_0_1_300);
            result |= DrawPressReleaseV(Main.Lang[TKM.Offset], vConfig.Offset, CD_V_VEC2_0_1_300);
            result |= DrawPressReleaseV(Main.Lang[TKM.Rotation], vConfig.Rotation, CD_V_VEC3_0_1_300);
            TitleButton(Main.Lang[TKM.EditEaseConfig], Main.Lang[TKM.EditThis], () =>
            {
                GUIController.Push(new MethodDrawable(() =>
                {
                    if (vConfig.UseSize)
                        DrawPressReleaseV(Main.Lang[TKM.SizeEase], vConfig.SizeEase, CD_V_EASECONFIG);
                    else DrawPressReleaseV(Main.Lang[TKM.ScaleEase], vConfig.ScaleEase, CD_V_EASECONFIG);
                    DrawPressReleaseV(Main.Lang[TKM.OffsetEase], vConfig.OffsetEase, CD_V_EASECONFIG);
                    DrawPressReleaseV(Main.Lang[TKM.RotationEase], vConfig.RotationEase, CD_V_EASECONFIG);
                }, Main.Lang[TKM.EditEaseConfig]));
            });
            return result;
        }
        public static bool DrawVector2WithSlider(string label, ref Vector2 vec2, float lValue, float rValue)
        {
            bool result = false;
            GUILayout.Label($"<b>{label}</b>");
            result |= DrawSingleWithSlider("X:", ref vec2.x, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec2.y, lValue, rValue, 300f);
            return result;
        }
        public static bool DrawVector3WithSlider(string label, ref Vector3 vec3, float lValue, float rValue)
        {
            bool result = false;
            GUILayout.Label($"<b>{label}</b>");
            result |= DrawSingleWithSlider("X:", ref vec3.x, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Y:", ref vec3.y, lValue, rValue, 300f);
            result |= DrawSingleWithSlider("Z:", ref vec3.z, lValue, rValue, 300f);
            return result;
        }
        public static bool DrawPressReleaseH<T>(string label, PressRelease<T> pr, CustomDrawerRef<T> drawer)
        {
            GUIStatus status = pr.Status;

            bool changed = false;
            GUILayoutEx.ExpandableGUI(() =>
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Main.Lang[TKM.Pressed]);
                    changed = drawer(ref pr.Pressed);
                }
                GUILayout.EndHorizontal();
                TitleButton(Main.Lang[TKM.CopyFromReleased], Main.Lang[TKM.Copy], () =>
                {
                    object released = pr.Released;
                    if (released is ICopyable<T> copyable)
                        pr.Pressed = copyable.Copy();
                    else pr.Pressed = pr.Released;
                    changed = true;
                });

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(ref pr.Released);
                }
                GUILayout.EndHorizontal();
                TitleButton(Main.Lang[TKM.CopyFromPressed], Main.Lang[TKM.Copy], () =>
                {
                    object pressed = pr.Pressed;
                    if (pressed is ICopyable<T> copyable)
                        pr.Released = copyable.Copy();
                    else pr.Released = pr.Pressed;
                    changed = true;
                });
            }, label, ref status.Expanded);
            return changed;
        }
        public static bool DrawPressReleaseV<T>(string label, PressRelease<T> pr, CustomDrawerRef<T> drawer)
        {
            GUIStatus status = pr.Status;

            bool changed = false;
            GUILayoutEx.ExpandableGUI(() =>
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(Main.Lang[TKM.Pressed]);
                    changed = drawer(ref pr.Pressed);
                }
                GUILayout.EndVertical();
                TitleButton(Main.Lang[TKM.CopyFromReleased], Main.Lang[TKM.Copy], () =>
                {
                    object released = pr.Released;
                    if (released is ICopyable<T> copyable)
                        pr.Pressed = copyable.Copy();
                    else pr.Pressed = pr.Released;
                    changed = true;
                });

                GUILayout.BeginVertical();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(ref pr.Released);
                }
                GUILayout.EndVertical();
                TitleButton(Main.Lang[TKM.CopyFromPressed], Main.Lang[TKM.Copy], () =>
                {
                    object pressed = pr.Pressed;
                    if (pressed is ICopyable<T> copyable)
                        pr.Released = copyable.Copy();
                    else pr.Released = pr.Pressed;
                    changed = true;
                });
            }, label, ref status.Expanded);
            return changed;
        }
        public static bool DrawPressReleaseH<T>(string label, PressRelease<T> pr, CustomDrawer<T> drawer)
        {
            GUIStatus status = pr.Status;

            bool changed = false;
            GUILayoutEx.ExpandableGUI(() =>
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Main.Lang[TKM.Pressed]);
                    changed = drawer(pr.Pressed);
                }
                GUILayout.EndHorizontal();
                TitleButton(Main.Lang[TKM.CopyFromReleased], Main.Lang[TKM.Copy], () =>
                {
                    object released = pr.Released;
                    if (released is ICopyable<T> copyable)
                        pr.Pressed = copyable.Copy();
                    else pr.Pressed = pr.Released;
                    changed = true;
                });

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(pr.Released);
                }
                GUILayout.EndHorizontal();
                TitleButton(Main.Lang[TKM.CopyFromPressed], Main.Lang[TKM.Copy], () =>
                {
                    object pressed = pr.Pressed;
                    if (pressed is ICopyable<T> copyable)
                        pr.Released = copyable.Copy();
                    else pr.Released = pr.Pressed;
                    changed = true;
                });
            }, label, ref status.Expanded);
            return changed;
        }
        public static bool DrawPressReleaseV<T>(string label, PressRelease<T> pr, CustomDrawer<T> drawer)
        {
            GUIStatus status = pr.Status;

            bool changed = false;
            GUILayoutEx.ExpandableGUI(() =>
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(Main.Lang[TKM.Pressed]);
                    changed = drawer(pr.Pressed);
                }
                GUILayout.EndVertical();
                TitleButton(Main.Lang[TKM.CopyFromReleased], Main.Lang[TKM.Copy], () =>
                {
                    object released = pr.Released;
                    if (released is ICopyable<T> copyable)
                        pr.Pressed = copyable.Copy();
                    else pr.Pressed = pr.Released;
                    changed = true;
                });

                GUILayout.BeginVertical();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(pr.Released);
                }
                GUILayout.EndVertical();
                TitleButton(Main.Lang[TKM.CopyFromPressed], Main.Lang[TKM.Copy], () =>
                {
                    object pressed = pr.Pressed;
                    if (pressed is ICopyable<T> copyable)
                        pr.Released = copyable.Copy();
                    else pr.Released = pr.Pressed;
                    changed = true;
                });
            }, label, ref status.Expanded);
            return changed;
        }
        public static void ExpandableGUI(GUIStatus status, string label, Action drawer)
        {
            GUILayoutEx.ExpandableGUI(drawer, label, ref status.Expanded);
        }
        public static bool DrawSingleWithSlider(string label, ref float value, float lValue, float rValue, float width)
        {
            GUILayout.BeginHorizontal();
            float newValue = GUILayoutEx.NamedSliderContent(label, value, lValue, rValue, width);
            GUILayout.EndHorizontal();
            bool result = newValue != value;
            value = newValue;
            return result;
        }
        public static bool DrawStringArray(ref string[] array, Action<int> arrayResized = null, Action<int> elementRightGUI = null, Action<int, string> onElementChange = null)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                Array.Resize(ref array, array.Length + 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            if (array.Length > 0 && GUILayout.Button("-"))
            {
                Array.Resize(ref array, array.Length - 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < array.Length; i++)
            {
                string cache = array[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{i}: ");
                cache = GUILayout.TextField(cache);
                elementRightGUI?.Invoke(i);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (cache != array[i])
                {
                    array[i] = cache;
                    onElementChange?.Invoke(i, cache);
                }
            }
            return result;
        }
        public static bool DrawArray(string label, ref object[] array)
        {
            bool result = false;
            GUILayout.Label(label);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                Array.Resize(ref array, array.Length + 1);
            if (array.Length > 0 && GUILayout.Button("-"))
                Array.Resize(ref array, array.Length - 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < array.Length; i++)
                result |= DrawObject($"{i}: ", ref array[i]);
            GUILayout.EndVertical();
            return result;
        }
        public static bool DrawList<T>(List<T> list, CustomDrawerRef<T> drawer)
        {
            bool result = false;
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                list.Add(default);
            if (list.Count > 0 && GUILayout.Button("-"))
                list.RemoveAt(list.Count - 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (drawer(ref t))
                    list[i] = t;
                GUIStyle style = new GUIStyle()
                {
                    margin = new RectOffset(0, 0, 0, 10),
                    normal = new GUIStyleState()
                    {
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
        public static bool DrawBool(string label, ref bool value)
        {
            bool prev = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = GUILayout.Toggle(value, string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        public static bool DrawByte(string label, ref byte value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt8(str);
            return result;
        }
        public static bool DrawDouble(string label, ref double value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToDouble(str);
            return result;
        }
        public static bool DrawEnum<T>(string label, ref T @enum, int unique = 0) where T : Enum
        {
            int current = EnumHelper<T>.IndexOf(@enum);
            string[] names = EnumHelper<T>.GetNames();
            bool result = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref current, names, label, unique);
            @enum = EnumHelper<T>.GetValues()[current];
            return result;
        }
        public static bool DrawInt16(string label, ref short value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt16(str);
            return result;
        }
        public static bool DrawInt32(string label, ref int value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt32(str);
            return result;
        }
        public static bool DrawInt64(string label, ref long value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt64(str);
            return result;
        }
        public static void DrawObject(string label, object value)
        {
            if (value == null) return;
            if (value is IDrawable drawable)
            {
                drawable.Draw();
                return;
            }
            Type t = value.GetType();
            if (!t.IsPrimitive && t != typeof(string)) return;
            var fields = t.GetFields();
            foreach (var field in fields)
            {
                var fValue = field.GetValue(value);
                if (DrawObject(field.Name, ref fValue))
                    field.SetValue(value, fValue);
            }
            var props = t.GetProperties();
            foreach (var prop in props.Where(p => p.CanRead && p.CanWrite))
            {
                var pValue = prop.GetValue(value);
                if (DrawObject(prop.Name, ref pValue))
                    prop.SetValue(value, pValue);
            }
        }
        public static bool DrawObject(string label, ref object obj)
        {
            bool result = false;
            switch (obj)
            {
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
        public static bool DrawSByte(string label, ref sbyte value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt8(str);
            return result;
        }
        public static bool DrawSingle(string label, ref float value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToFloat(str);
            return result;
        }
        public static bool DrawString(string label, ref string value)
        {
            string prev = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = GUILayout.TextField(value);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        public static bool DrawToggleGroup(string[] labels, bool[] toggleGroup)
        {
            bool result = false;
            for (int i = 0; i < labels.Length; i++)
                if (DrawBool(labels[i], ref toggleGroup[i]))
                {
                    result = true;
                    for (int j = 0; j < toggleGroup.Length; j++)
                        if (j == i) continue;
                        else toggleGroup[j] = false;
                    break;
                }
            return result;
        }
        public static bool DrawUInt16(string label, ref ushort value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt16(str);
            return result;
        }
        public static bool DrawUInt32(string label, ref uint value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt32(str);
            return result;
        }
        public static bool DrawUInt64(string label, ref ulong value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt64(str);
            return result;
        }
    }
}
