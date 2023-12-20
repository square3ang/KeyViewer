using KeyViewer.Utils;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static OptionsPanelsCLS;

namespace KeyViewer.Types
{
    public class Drawer : IDrawer
    {
        public static Drawer Instance { get; private set; } = new Drawer();
        Drawer() => This = this;
        IDrawer This;
        bool IDrawer.DrawArray(string label, object[] array)
        {
            throw new NotImplementedException();
        }
        bool IDrawer.DrawBool(string label, ref bool value)
        {
            bool prev = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = GUILayout.Toggle(value, string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        bool IDrawer.DrawByte(string label, ref byte value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToUInt8(str);
            return result;
        }
        bool IDrawer.DrawColor(string label, ref Color color)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            result |= This.DrawSingle("<color=#FF0000>R</color>", ref color.r);
            result |= This.DrawSingle("<color=#00FF00>G</color>", ref color.g);
            result |= This.DrawSingle("<color=#0000FF>B</color>", ref color.b);
            result |= This.DrawSingle("A", ref color.a);
            GUILayout.EndHorizontal();
            return result;
        }
        bool IDrawer.DrawColor(string label, ref VertexGradient color)
        {
            bool result = false;  
            GUILayout.BeginHorizontal();
            result |= This.DrawColor("Top Left", ref color.topLeft);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= This.DrawColor("Top Right", ref color.topRight);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= This.DrawColor("Bottom Left", ref color.bottomLeft);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            result |= This.DrawColor("Bottom Right", ref color.bottomRight);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return result;
        }
        bool IDrawer.DrawDouble(string label, ref double value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToDouble(str);
            return result;
        }
        bool IDrawer.DrawEnum<T>(string label, ref T @enum)
        {
            int current = EnumHelper<T>.IndexOf(@enum);
            string[] names = EnumHelper<T>.GetNames();
            bool result = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref current, names, label);
            @enum = EnumHelper<T>.GetValues()[current];
            return result;
        }
        bool IDrawer.DrawInt16(string label, ref short value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToInt16(str);
            return result;
        }
        bool IDrawer.DrawInt32(string label, ref int value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToInt32(str);
            return result;
        }
        bool IDrawer.DrawInt64(string label, ref long value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToInt64(str);
            return result;
        }
        void IDrawer.DrawObject(string label, object value)
        {
            if (value == null) return;
            if (value is IDrawable drawable)
            {
                drawable.Draw(this);
                return;
            }
            Type t = value.GetType();
            if (t.IsPrimitive) return;
            var fields = t.GetFields();

            var props = t.GetProperties();
        }
        bool IDrawer.DrawObject(string label, ref object obj)
        {
            bool result = false;
            switch (obj)
            {
                case bool bb:
                    result = This.DrawBool(label, ref bb);
                    obj = bb;
                    break;
                case sbyte sb:
                    result = This.DrawSByte(label, ref sb);
                    obj = sb;
                    break;
                case byte b:
                    result = This.DrawByte(label, ref b);
                    obj = b;
                    break;
                case short s:
                    result = This.DrawInt16(label, ref s);
                    obj = s;
                    break;
                case ushort us:
                    result = This.DrawUInt16(label, ref us);
                    obj = us;
                    break;
                case int i:
                    result = This.DrawInt32(label, ref i);
                    obj = i;
                    break;
                case uint ui:
                    result = This.DrawUInt32(label, ref ui);
                    obj = ui;
                    break;
                case long l:
                    result = This.DrawInt64(label, ref l);
                    obj = l;
                    break;
                case ulong ul:
                    result = This.DrawUInt64(label, ref ul);
                    obj = ul;
                    break;
                case float f:
                    result = This.DrawSingle(label, ref f);
                    obj = f;
                    break;
                case double d:
                    result = This.DrawDouble(label, ref d);
                    obj = d;
                    break;
                case string str:
                    result = This.DrawString(label, ref str);
                    obj = str;
                    break;
            }
            return result;
        }
        bool IDrawer.DrawSByte(string label, ref sbyte value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToInt8(str);
            return result;
        }
        bool IDrawer.DrawSingle(string label, ref float value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToFloat(str);
            return result;
        }
        bool IDrawer.DrawString(string label, ref string value)
        {
            string prev = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = GUILayout.TextField(value);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        bool IDrawer.DrawToggleGroup(string[] labels, bool[] toggleGroup)
        {
            bool result = false;
            for (int i = 0; i < labels.Length; i++)
                result |= This.DrawBool(labels[i], ref toggleGroup[i]);
            return result;
        }
        bool IDrawer.DrawUInt16(string label, ref ushort value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToUInt16(str);
            return result;
        }
        bool IDrawer.DrawUInt32(string label, ref uint value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToUInt32(str);
            return result;
        }
        bool IDrawer.DrawUInt64(string label, ref ulong value)
        {
            string str = value.ToString();
            bool result = This.DrawString(label, ref str);
            value = StringConverter.ToUInt64(str);
            return result;
        }
    }
}
