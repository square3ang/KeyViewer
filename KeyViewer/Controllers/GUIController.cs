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
        private static int skipFrames = 0;
        private static Stack<Action> onSkipCallbacks = new Stack<Action>();
        private static (string bts, IDrawable drawable) prev;
        public static void Init(string backToStr, IDrawable drawable)
        {
            current = drawable;
            bts = backToStr;
        }
        public static void Push(string backToStr, IDrawable drawable)
        {
            if (drawables.Count <= depth)
            {
                drawables.Add((bts, current));
                depth++;
            }
            else drawables[depth++] = (bts, current);
            current = drawable;
            bts = backToStr;
        }
        public static void Pop()
        {
            if (!isAvailable) return;
            prev = (bts, current);
            (bts, current) = drawables[--depth];
        }
        public static void Draw()
        {
#if DEBUG
            GUILayout.Label($"DEPTH:{depth}, COUNT:{drawables.Count}");
            GUILayout.Label($"PREV BTS:{prev.bts}, CURRENT BTS:{bts}");
#endif
            if (skipFrames > 0)
            {
                skipFrames--;
                if (onSkipCallbacks.Count > 0)
                    onSkipCallbacks.Pop()?.Invoke();
                return;
            }
            if (depth > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("◀ " + drawables[depth - 1].bts))
                    Pop();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            if (drawables.Count > depth)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(prev.bts + " ▶"))
                    Push(prev.bts, prev.drawable);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            current.Draw(Drawer.Instance);
        }
        public static void Skip(Action onSkip = null, int frames = 1)
        {
            skipFrames += frames;
            onSkipCallbacks.Push(onSkip);
        }

        public static void Flush()
        {
            drawables = new List<(string, IDrawable)>();
            depth = 0;
            prev = (null, null);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
        }
    }
}
