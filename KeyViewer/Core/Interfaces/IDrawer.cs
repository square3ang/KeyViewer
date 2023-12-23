using System;
using TMPro;
using UnityEngine;

namespace KeyViewer.Core.Interfaces
{
    public interface IDrawer
    {
        bool DrawBool(string label, ref bool value);
        bool DrawSByte(string label, ref sbyte value);
        bool DrawByte(string label, ref byte value);
        bool DrawInt16(string label, ref short value);
        bool DrawUInt16(string label, ref ushort value);
        bool DrawInt32(string label, ref int value);
        bool DrawUInt32(string label, ref uint value);
        bool DrawInt64(string label, ref long value);
        bool DrawUInt64(string label, ref ulong value);
        bool DrawSingle(string label, ref float value);
        bool DrawDouble(string label, ref double value);
        bool DrawString(string label, ref string value);
        bool DrawEnum<T>(string label, ref T @enum) where T : Enum;
        bool DrawArray(string label, ref object[] array);
        bool DrawToggleGroup(string[] labels, bool[] toggleGroup);
        bool DrawColor(string label, ref Color color);
        bool DrawColor(string label, ref VertexGradient color);
        void DrawObject(string label, object value);
        bool DrawObject(string label, ref object value);
    }
}
