using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Controllers
{
    public static class GUIController
    {
        private static List<IDrawable> drawables = new List<IDrawable>();
        private static int depth;
        private static bool isUndoAvailable => depth > 0;
        private static bool isRedoAvailable => depth < drawables.Count;
        private static IDrawable current;
        private static IDrawable first;
        private static int skipFrames = 0;
        private static Stack<Action> onSkipCallbacks = new Stack<Action>();
        public static void Init(IDrawable drawable)
        {
            first = current = drawable;
        }
        public static void Push(IDrawable drawable)
        {
            if (drawables.Count == depth)
            {
                drawables.Add(current);
                depth++;
            }
            else
            {
                drawables[depth++] = current;
            }
            current = drawable;
        }
        public static void Pop()
        {
            if (!isUndoAvailable) return;
            var cache = current;
            current = drawables[--depth];
            drawables[depth] = cache;
        }
        public static void Draw()
        {
            //GUILayout.Label($"DEPTH:{depth}, COUNT:{drawables.Count}");
            //for (int i = 0; i < drawables.Count; i++)
            //    GUILayout.Label($"{i}:{drawables[i].Name}");
            if (skipFrames > 0)
            {
                skipFrames--;
                if (onSkipCallbacks.Count > 0)
                    onSkipCallbacks.Pop()?.Invoke();
                return;
            }
            GUILayout.BeginHorizontal();
            {
                if (isUndoAvailable)
                {
                    if (GUILayout.Button("◀ " + drawables[depth - 1].Name))
                        Pop();
                }
                if (isRedoAvailable)
                {
                    var draw = drawables[depth];
                    if (GUILayout.Button(draw.Name + " ▶"))
                        Push(draw);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            current.Draw();
        }
        public static void Skip(Action onSkip = null, int frames = 1)
        {
            skipFrames += frames;
            onSkipCallbacks.Push(onSkip);
        }

        public static void Flush()
        {
            current = first;
            drawables = new List<IDrawable>();
            depth = 0;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
        }
    }
}
