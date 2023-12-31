using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace KeyViewer.Core.Interfaces
{
    public interface IDrawer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawBool(string label, ref bool value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawSByte(string label, ref sbyte value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawByte(string label, ref byte value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawInt16(string label, ref short value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawUInt16(string label, ref ushort value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawInt32(string label, ref int value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawUInt32(string label, ref uint value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawInt64(string label, ref long value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawUInt64(string label, ref ulong value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawSingle(string label, ref float value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawDouble(string label, ref double value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawString(string label, ref string value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawEnum<T>(string label, ref T @enum) where T : Enum;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawArray(string label, ref object[] array);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawToggleGroup(string[] labels, bool[] toggleGroup);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawColor(string label, ref Color color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawColor(string label, ref VertexGradient color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DrawObject(string label, object value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DrawObject(string label, ref object value);
    }
}
