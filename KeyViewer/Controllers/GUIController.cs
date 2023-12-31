using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Controllers
{
    public static class GUIController
    {
        private static Stack<(string backToStr, IDrawable drawable)> drawables = new Stack<(string, IDrawable)>();
        private static int depth => drawables.Count;
        private static bool isAvailable => depth > 0;
        private static IDrawable current;
        private static string bts;
        public static void Push(string backToStr, IDrawable drawable)
        {
            drawables.Push((backToStr, drawable));
            current = drawable;
            bts = backToStr;
        }
        public static void Pop()
        {
            if (!isAvailable) return;
            var t = drawables.Pop();
            current = t.drawable;
            bts = t.backToStr;
        }
        public static void Draw()
        {
            if (isAvailable)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("◀ " + bts))
                {
                    Pop();
                    return;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            current.Draw(Drawer.Instance);
        }
    }
}
