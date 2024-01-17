using JSON;
using KeyViewer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        public static Vector2 WithRelativeY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, vector.y + y);
        }
        public static void SetAnchor(this RectTransform source, Anchor allign)
        {
            switch (allign)
            {
                case Anchor.TopLeft:
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case Anchor.TopCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 1);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case Anchor.TopRight:
                    {
                        source.anchorMin = new Vector2(1, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case Anchor.MiddleLeft:
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case Anchor.MiddleCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0.5f);
                        source.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case Anchor.MiddleRight:
                    {
                        source.anchorMin = new Vector2(1, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case Anchor.BottomLeft:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case Anchor.BottomCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case Anchor.BottomRight:
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case Anchor.HorizontalStretchTop:
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case Anchor.HorizontalStretchMiddle:
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case Anchor.HorizontalStretchBottom:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case Anchor.VerticalStretchLeft:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case Anchor.VerticalStretchCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case Anchor.VerticalStretchRight:
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case Anchor.StretchAll:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
        }
    }
}
