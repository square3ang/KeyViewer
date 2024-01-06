using KeyViewer.Models;
using System;

namespace KeyViewer.Utils
{
    public static class CanEase<T>
    {
        public static readonly bool Value;
        static CanEase()
        {
            Type t = typeof(T);
            Value =
                t == ModelUtils.quat_t ||
                t == ModelUtils.col_t ||
                t == ModelUtils.col32_t ||
                t == ModelUtils.vec2_t ||
                t == ModelUtils.vec3_t ||
                t == ModelUtils.vec4_t ||
                t == typeof(GColor) ||
                t == typeof(float);
        }
    }
}
