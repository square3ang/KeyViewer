using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Controllers
{
    public static class GUIController
    {
        private static List<(string bts, IDrawable drawable)> drawables = new List<(string, IDrawable)>();
        private static int depth;
        private static bool isAvailable => depth > 0;
        private static IDrawable current;
        private static string bts;
        public static void Init(string backToStr, IDrawable drawable)
        {
            current = drawable;
            bts = backToStr;
        }
        public static void Push(string backToStr, IDrawable drawable)
        {
            drawables.Add((bts, current));
            current = drawable;
            bts = backToStr;
            depth++;
        }
        public static void Pop()
        {
            if (!isAvailable) return;
            (bts, current) = drawables[--depth];
            drawables.RemoveAt(depth);
        }
        public static void Draw()
        {
            if (depth > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("◀ " + drawables[depth - 1].bts))
                {
                    Pop();
                    return;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            if (drawables.Count > depth + 1)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(drawables[depth + 1].bts + " ▶"))
                    (bts, current) = drawables[++depth];
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            current.Draw(Drawer.Instance);
        }
        public static void Flush()
        {
            drawables = new List<(string, IDrawable)>();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
        }
    }
}
