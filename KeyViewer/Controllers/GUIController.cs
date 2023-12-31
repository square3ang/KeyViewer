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
        public static void Push(string backToStr, IDrawable drawable)
        {
            drawables.Push((backToStr, drawable));
        }
        public static void Pop()
        {
            if (!isAvailable) return;
            current = drawables.Pop().drawable;
        }
        public static void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            current.Draw(Drawer.Instance);
        }
    }
}
