﻿using JSON;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static bool IfTrue(this bool b, Action a)
        {
            if (b) a();
            return b;
        }
        public static string ToStringN(this JsonNode node)
        {
            if (node == null) return null;
            return node.Value;
        }
        /// <summary>
        /// For Avoid Warning
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async void Await(this Task task)
        {
            await task;
        }
    }
}
