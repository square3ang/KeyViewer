using HarmonyLib;
using KeyViewer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyViewer.Core
{
    public static class GUIManager
    {
        private static Stack<(string name, IDrawable drawable)> drawables = new Stack<(string, IDrawable)>();
        private static int depth => drawables.Count;
        private static bool isAvailable => depth > 0;
        private static IDrawable current;
        public static void Push(string name, IDrawable drawable)
        {
            drawables.AddItem((name, drawable));
        }
        public static void Pop()
        {
            if (!isAvailable) return;
            current = drawables.Pop().drawable;
        }
    }
}
