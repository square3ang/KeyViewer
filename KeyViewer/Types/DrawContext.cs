using System;
using System.Collections.Generic;
using System.Reflection;
using UnityModManagerNet;

namespace KeyViewer.Types
{
    public class DrawContext
    {
        public Type declaringType;
        public object instance;
        public DrawFieldMask mask;
        public int unique;
        private Dictionary<string, bool[]> toggleStates;
        private Dictionary<string, DrawContext> innerContext;
        private Dictionary<DrawType, Action<DrawContext, FieldInfo>> customDrawer;
        public DrawContext(object instance, DrawFieldMask mask = DrawFieldMask.Any, int unique = 0)
        {
            declaringType = instance.GetType();
            this.instance = instance;
            this.mask = mask;
            this.unique = unique;
            toggleStates = new Dictionary<string, bool[]>();
            innerContext = new Dictionary<string, DrawContext>();
            customDrawer = new Dictionary<DrawType, Action<DrawContext, FieldInfo>>();
        }
        public DrawContext GetInnerContext(string name, object innerInstance, DrawFieldMask mask, int unique)
        {
            if (innerContext.TryGetValue(name, out var context)) return context;
            return innerContext[name] = new DrawContext(innerInstance, mask, unique) { customDrawer = customDrawer };
        }
        public bool[] GetToggleStates(string name, int length)
        {
            bool[] result;
            if (!toggleStates.TryGetValue(name, out result))
                toggleStates[name] = result = Array.Empty<bool>();
            if (result.Length != length)
            {
                Array.Resize(ref result, length);
                toggleStates[name] = result;
            }
            return result;
        }
        public void AddDrawer(DrawType customDrawType, Action<DrawContext, FieldInfo> drawer)
        {
            customDrawer.Add(customDrawType, drawer);
        }
        public Action<DrawContext, FieldInfo> ResolveDrawer(DrawType dt)
        {
            customDrawer.TryGetValue(dt, out var drawer);
            return drawer;
        }
    }
}
