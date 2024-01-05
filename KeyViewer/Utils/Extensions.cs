using System;
using System.Collections.Generic;
using KeyViewer.Core.Interfaces;

namespace KeyViewer.Utils
{
    public static class Extensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
        public static double Round(this double value, int digits = -1) => digits < 0 ? value : Math.Round(value, digits);
        public static unsafe IntPtr GetAddress<T>(ref T obj)
        {
            TypedReference tr = __makeref(obj);
#pragma warning disable CS8500
            return *(IntPtr*)&tr;
#pragma warning restore CS8500
        }
        public static int GetUnique<T>(this T obj) where T : class => (int)GetAddress(ref obj);
        public static int GetUnique<T>(this ref T obj) where T : struct => (int)GetAddress(ref obj);
    }
}
