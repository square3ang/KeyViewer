using KeyViewer.Utils;
using System;
using TMPro;
using UnityEngine;
using KeyViewer.Core.Interfaces;
using System.Linq;
using KeyViewer.Models;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;

namespace KeyViewer.Core
{
    public delegate bool CustomDrawer<T>(ref T t);
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
        #endregion

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
        public static bool DrawPressReleaseH<T>(string label, PressRelease<T> pr, CustomDrawer<T> drawer)
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

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(ref pr.Released);
                }
                GUILayout.EndHorizontal();
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
                    changed = drawer(ref pr.Pressed);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    GUILayout.Label(Main.Lang[TKM.Released]);
                    changed = drawer(ref pr.Released);
                }
                GUILayout.EndVertical();
            }, label, ref status.Expanded);
            return changed;
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
        public static bool DrawColor(string label, ref Color color)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            result |= DrawSingle("<color=#FF0000>R</color>", ref color.r);
            result |= DrawSingle("<color=#00FF00>G</color>", ref color.g);
            result |= DrawSingle("<color=#0000FF>B</color>", ref color.b);
            result |= DrawSingle("A", ref color.a);
            GUILayout.EndHorizontal();
            return result;
        }
        public static bool DrawColor(string label, ref VertexGradient color)
        {
            bool result = false;  
            GUILayout.BeginHorizontal();
            result |= DrawColor(Main.Lang[TKM.TopLeft], ref color.topLeft);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= DrawColor(Main.Lang[TKM.TopRight], ref color.topRight);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= DrawColor(Main.Lang[TKM.BottomLeft], ref color.bottomLeft);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= DrawColor(Main.Lang[TKM.BottomRight], ref color.bottomRight);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return result;
        }
        public static bool DrawDouble(string label, ref double value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToDouble(str);
            return result;
        }
        public static bool DrawEnum<T>(string label, ref T @enum) where T : Enum
        {
            int current = EnumHelper<T>.IndexOf(@enum);
            string[] names = EnumHelper<T>.GetNames();
            bool result = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref current, names, label);
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
