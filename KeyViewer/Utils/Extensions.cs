using JSON;
using KeyViewer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace KeyViewer.Utils
{
    public static class Extensions
    {
        public static string TrimQuote(this string s) => s?.Trim('\'', '"', '\\');
        public static string Stringify(this byte[] array)
        {
            StringBuilder buffer = new StringBuilder();
            int length = array.Length;
            for (int i = 0; i < length; i++)
                if (i + 1 < length)
                    buffer.Append((char)(array[i] << 8 | array[++i]));
                else buffer.Append((char)(array[i] << 8));
            return buffer.ToString();
        }
        public static byte[] ToBytes(this string str)
        {
            char[] chars = str.ToCharArray();
            int charsLength = chars.Length;
            bool isOdd = (chars[charsLength - 1] & 0xff) == 0;
            byte[] buffer = new byte[charsLength * 2 - (isOdd ? 1 : 0)];
            int length = buffer.Length;
            for (int i = 0; i < length; i += 2)
            {
                char c = chars[i / 2];
                buffer[i] = (byte)(c >> 8);
                if (i + 1 < length)
                    buffer[i + 1] = (byte)(c & 0xff);
            }
            return buffer;
        }
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
        public static Vector2 WithRelativeX(this Vector2 vector, float x)
        {
            return new Vector2(vector.x + x, vector.y);
        }
        public static Vector2 WithRelativeY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, vector.y + y);
        }
        public static Vector3 WithRelativeX(this Vector3 vector, float x)
        {
            return new Vector3(vector.x + x, vector.y, vector.z);
        }
        public static Vector3 WithRelativeY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, vector.y + y, vector.z);
        }
        public static Vector3 WithRelativeZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, vector.z + z);
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
        public static JsonNode IfNotExist(this JsonNode node, JsonNode other) => node == null ? other : node;
        public static byte[] Compress(this byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                    dstream.Write(data, 0, data.Length);
                return output.ToArray();
            }
        }
        public static byte[] Decompress(this byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (MemoryStream input = new MemoryStream(data))
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                    dstream.CopyTo(output);
                return output.ToArray();
            }
        }
        public static string GetHashSHA1(this byte[] data) => string.Concat(SHA1.Create().ComputeHash(data).Select(x => x.ToString("X2")));
    }
}
