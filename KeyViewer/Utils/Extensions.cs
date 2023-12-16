using System;
using System.Collections.Generic;
using KeyViewer.Types;

namespace KeyViewer.Utils
{
    public static class Extensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
        public static void Draw(this IDrawable drawable)
        {
            if (GUIUtils.Draw(drawable.Context))
                drawable.OnChange();
        }
    }
}
