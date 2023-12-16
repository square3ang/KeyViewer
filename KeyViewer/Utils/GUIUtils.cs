using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.UI;
using KeyViewer.Types;

namespace KeyViewer.Utils
{
    public static class GUIUtils
    {
        #region Draw
        public static bool Draw(DrawContext context)
        {
            object container = context.instance;
            Type type = context.declaringType;
            int unique = context.unique;
            DrawFieldMask defaultMask = context.mask;
            bool changed = false;
            var options = new List<GUILayoutOption>();
            DrawFieldMask mask = type.GetCustomAttribute<DrawFieldsAttribute>(false)?.Mask ?? defaultMask;
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                DrawAttribute a = f.GetCustomAttribute<DrawAttribute>(false);
                if (a != null)
                {
                    a.Width = a.Width != 0 ? Scale(a.Width) : 0;
                    a.Height = a.Height != 0 ? Scale(a.Height) : 0;
                    if (a.Type == DrawType.Ignore) continue;
                    if (!string.IsNullOrEmpty(a.VisibleOn))
                    {
                        if (!DependsOn(a.VisibleOn, container, type))
                            continue;
                    }
                    else if (!string.IsNullOrEmpty(a.InvisibleOn))
                    {
                        if (DependsOn(a.InvisibleOn, container, type))
                            continue;
                    }
                }
                else
                {
                    a = new DrawAttribute();
                    if ((mask & DrawFieldMask.OnlyDrawAttr) == 0 && ((mask & DrawFieldMask.SkipNotSerialized) == 0 || !f.IsNotSerialized)
                        && ((mask & DrawFieldMask.Public) > 0 && f.IsPublic
                        || (mask & DrawFieldMask.Serialized) > 0 && f.GetCustomAttributes(typeof(SerializeField), false).Length > 0
                        || (mask & DrawFieldMask.Public) == 0 && (mask & DrawFieldMask.Serialized) == 0))
                    {
                        RangeAttribute range = f.GetCustomAttribute<RangeAttribute>();
                        if (range != null)
                        {
                            //a.Type = DrawType.Slider;
                            a.Min = range.min;
                            a.Max = range.max;
                        }
                    }
                    else continue;
                }
                foreach (SpaceAttribute a_ in f.GetCustomAttributes(typeof(SpaceAttribute), false))
                    GUILayout.Space(Scale((int)a_.height));
                foreach (HeaderAttribute a_ in f.GetCustomAttributes(typeof(HeaderAttribute), false))
                    GUILayout.Label(a_.header, bold, GUILayout.ExpandWidth(false));
                var fieldName = a.Label == null ? f.Name : a.Label;
                if ((f.FieldType.IsClass && !f.FieldType.IsArray || f.FieldType.IsValueType && !f.FieldType.IsPrimitive && !f.FieldType.IsEnum) && !Array.Exists(specialTypes, x => x == f.FieldType))
                {
                    defaultMask = f.GetCustomAttribute<DrawFieldsAttribute>()?.Mask ?? mask;
                    var box = a.Box || a.Collapsible && collapsibleStates.Exists(x => x == f.MetadataToken);
                    var horizontal = f.GetCustomAttribute<HorizontalAttribute>(false) != null || f.FieldType.GetCustomAttributes<HorizontalAttribute>(false) != null;
                    if (horizontal)
                    {
                        GUILayout.BeginHorizontal(box ? "box" : "");
                        box = false;
                    }
                    if (a.Collapsible)
                        GUILayout.BeginHorizontal();
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        BeginHorizontalTooltip(null, a);
                        GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                        EndHorizontalTooltip(null, a);
                    }
                    var visible = true;
                    if (a.Collapsible)
                    {
                        if (!string.IsNullOrEmpty(fieldName))
                            GUILayout.Space(5);
                        visible = collapsibleStates.Exists(x => x == f.MetadataToken);
                        if (GUILayout.Button(visible ? "Hide" : "Show", GUILayout.ExpandWidth(false)))
                        {
                            if (visible)
                                collapsibleStates.Remove(f.MetadataToken);
                            else
                                collapsibleStates.Add(f.MetadataToken);
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (visible)
                    {
                        if (box) GUILayout.BeginVertical("box");
                        var val = f.GetValue(container);
                        if (typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType) && val is UnityEngine.Object obj)
                            GUILayout.Label(obj.name, GUILayout.ExpandWidth(false));
                        else
                        {
                            if (Draw(context.GetInnerContext(f.Name, val, defaultMask, f.Name.GetHashCode() + unique)))
                            {
                                changed = true;
                                f.SetValue(container, val);
                            }
                        }
                        if (box) GUILayout.EndVertical();
                    }
                    if (horizontal) GUILayout.EndHorizontal();
                    continue;
                }
                options.Clear();
                if (a.Type == DrawType.Auto)
                {
                    if (Array.Exists(fieldTypes, x => x == f.FieldType))
                        a.Type = DrawType.Field;
                    else if (Array.Exists(toggleTypes, x => x == f.FieldType))
                        a.Type = DrawType.Toggle;
                    else if (f.FieldType.IsEnum)
                    {
                        if (f.GetCustomAttribute<FlagsAttribute>(false) != null)
                            a.Type = DrawType.PopupList;
                    }
                    else if (f.FieldType == typeof(KeyBinding))
                        a.Type = DrawType.KeyBinding;
                }
                if (a.Type == DrawType.Field)
                {
                    if (!Array.Exists(fieldTypes, x => x == f.FieldType) && !f.FieldType.IsArray)
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.Field}");
                    options.Add(a.Width != 0 ? GUILayout.Width(a.Width) : GUILayout.Width(Scale(100)));
                    options.Add(a.Height != 0 ? GUILayout.Height(a.Height) : GUILayout.Height(Scale(a.TextArea ? (int)drawHeight * 3 : (int)drawHeight)));
                    if (f.FieldType == typeof(Vector2))
                    {
                        if (a.Vertical)
                            GUILayout.BeginVertical();
                        else
                            GUILayout.BeginHorizontal();

                        BeginHorizontalTooltip(null, a);
                        GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                        EndHorizontalTooltip(null, a);

                        if (!a.Vertical)
                            GUILayout.Space(Scale(5));
                        var vec = (Vector2)f.GetValue(container);
                        if (DrawVector(ref vec, null, options.ToArray()))
                        {
                            f.SetValue(container, vec);
                            changed = true;
                        }
                        if (a.Vertical)
                        {
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    else if (f.FieldType == typeof(Vector3))
                    {
                        if (a.Vertical)
                            GUILayout.BeginVertical();
                        else
                            GUILayout.BeginHorizontal();

                        BeginHorizontalTooltip(null, a);
                        GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                        EndHorizontalTooltip(null, a);

                        if (!a.Vertical)
                            GUILayout.Space(Scale(5));
                        var vec = (Vector3)f.GetValue(container);
                        if (DrawVector(ref vec, null, options.ToArray()))
                        {
                            f.SetValue(container, vec);
                            changed = true;
                        }
                        if (a.Vertical)
                        {
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    else if (f.FieldType == typeof(Vector4))
                    {
                        if (a.Vertical)
                            GUILayout.BeginVertical();
                        else
                            GUILayout.BeginHorizontal();

                        BeginHorizontalTooltip(null, a);
                        GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                        EndHorizontalTooltip(null, a);

                        if (!a.Vertical)
                            GUILayout.Space(Scale(5));
                        var vec = (Vector4)f.GetValue(container);
                        if (DrawVector(ref vec, null, options.ToArray()))
                        {
                            f.SetValue(container, vec);
                            changed = true;
                        }
                        if (a.Vertical)
                        {
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    else if (f.FieldType == typeof(Color))
                    {
                        if (a.Vertical)
                            GUILayout.BeginVertical();
                        else
                            GUILayout.BeginHorizontal();

                        BeginHorizontalTooltip(null, a);
                        GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                        EndHorizontalTooltip(null, a);

                        if (!a.Vertical)
                            GUILayout.Space(Scale(5));
                        var vec = (Color)f.GetValue(container);
                        if (DrawColor(ref vec, null, options.ToArray()))
                        {
                            f.SetValue(container, vec);
                            changed = true;
                        }
                        if (a.Vertical)
                        {
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        //var val = f.GetValue(container).ToString();
                        var obj = f.GetValue(container);
                        Type elementType = null;
                        object[] values = null;
                        if (f.FieldType.IsArray)
                        {
                            if (obj is IEnumerable array)
                            {
                                values = array.Cast<object>().ToArray();
                                elementType = obj.GetType().GetElementType();
                            }
                        }
                        else
                        {
                            values = new object[] { obj };
                            elementType = obj.GetType();
                        }

                        if (values == null)
                            continue;

                        var _changed = false;

                        a.Vertical = a.Vertical || f.FieldType.IsArray;
                        if (a.Vertical)
                            GUILayout.BeginVertical();
                        else
                            GUILayout.BeginHorizontal();
                        if (f.FieldType.IsArray)
                        {
                            GUILayout.BeginHorizontal();
                            BeginTooltip(null, a.Tooltip);
                            GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                            EndTooltip(null, a.Tooltip);
                            GUILayout.Space(Scale(5));
                            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            {
                                Array.Resize(ref values, Math.Min(values.Length + 1, int.MaxValue));
                                values[values.Length - 1] = Convert.ChangeType("0", elementType);
                                _changed = true;
                                changed = true;
                            }
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Array.Resize(ref values, Math.Max(values.Length - 1, 0));
                                _changed = true;
                                changed = true;
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            BeginHorizontalTooltip(null, a);
                            GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                            EndHorizontalTooltip(null, a);
                        }
                        if (!a.Vertical)
                            GUILayout.Space(Scale(5));

                        if (values.Length > 0)
                        {
                            var isFloat = elementType == typeof(float) || elementType == typeof(double);
                            for (int i = 0; i < values.Length; i++)
                            {
                                var val = values[i].ToString();
                                if (a.Precision >= 0 && isFloat)
                                {
                                    if (Double.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out var num))
                                    {
                                        val = num.ToString($"f{a.Precision}");
                                    }
                                }
                                if (f.FieldType.IsArray)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label($"  [{i}] ", GUILayout.ExpandWidth(false));
                                }
                                if (elementType == typeof(string))
                                {
                                    options.Add(GUILayout.ExpandWidth(true));
                                }
                                string result;
                                if (elementType == typeof(string))
                                {
                                    if (a.TextArea)
                                    {
                                        result = GUILayout.TextArea(val, a.MaxLength, options.ToArray());
                                    }
                                    else
                                    {
                                        result = GUILayout.TextField(val, a.MaxLength, options.ToArray());
                                    }
                                }
                                else
                                {
                                    result = GUILayout.TextField(val, options.ToArray());
                                }
                                if (f.FieldType.IsArray)
                                {
                                    GUILayout.EndHorizontal();
                                }
                                if (result != val)
                                {
                                    if (elementType == typeof(string))
                                    {
                                    }
                                    else if (string.IsNullOrEmpty(result))
                                    {
                                        result = "0";
                                    }
                                    else
                                    {
                                        if (Double.TryParse(result, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out var num))
                                        {
                                            num = Math.Max(num, a.Min);
                                            num = Math.Min(num, a.Max);
                                            result = num.ToString();
                                        }
                                        else
                                        {
                                            result = "0";
                                        }
                                    }
                                    values[i] = Convert.ChangeType(result, elementType);
                                    changed = true;
                                    _changed = true;
                                }
                            }
                        }
                        if (_changed)
                        {
                            if (f.FieldType.IsArray)
                            {
                                if (elementType == typeof(float))
                                    f.SetValue(container, Array.ConvertAll(values, x => (float)x));
                                else if (elementType == typeof(int))
                                    f.SetValue(container, Array.ConvertAll(values, x => (int)x));
                                else if (elementType == typeof(long))
                                    f.SetValue(container, Array.ConvertAll(values, x => (long)x));
                                else if (elementType == typeof(double))
                                    f.SetValue(container, Array.ConvertAll(values, x => (double)x));
                                else if (elementType == typeof(string))
                                    f.SetValue(container, Array.ConvertAll(values, x => (string)x));
                            }
                            else
                            {
                                f.SetValue(container, values[0]);
                            }
                        }
                        if (a.Vertical)
                            GUILayout.EndVertical();
                        else
                            GUILayout.EndHorizontal();
                    }
                }
                else if (a.Type == DrawType.Slider)
                {
                    if (!Array.Exists(sliderTypes, x => x == f.FieldType))
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.Slider}");

                    options.Add(a.Width != 0 ? GUILayout.Width(a.Width) : GUILayout.Width(Scale(200)));
                    options.Add(a.Height != 0 ? GUILayout.Height(a.Height) : GUILayout.Height(Scale((int)drawHeight)));
                    if (a.Vertical)
                        GUILayout.BeginVertical();
                    else
                        GUILayout.BeginHorizontal();

                    BeginHorizontalTooltip(null, a);
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                    EndHorizontalTooltip(null, a);

                    if (!a.Vertical)
                        GUILayout.Space(Scale(5));
                    var val = f.GetValue(container).ToString();
                    if (!Double.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out var num))
                    {
                        num = 0;
                    }
                    if (a.Vertical)
                        GUILayout.BeginHorizontal();
                    var fnum = (float)num;
                    var result = GUILayout.HorizontalSlider(fnum, (float)a.Min, (float)a.Max, options.ToArray());
                    if (!a.Vertical)
                        GUILayout.Space(Scale(5));
                    GUILayout.Label(result.ToString(), GUILayout.ExpandWidth(false), GUILayout.Height(Scale((int)drawHeight)));
                    if (a.Vertical)
                        GUILayout.EndHorizontal();
                    if (a.Vertical)
                        GUILayout.EndVertical();
                    else
                        GUILayout.EndHorizontal();
                    if (result != fnum)
                    {
                        if ((f.FieldType == typeof(float) || f.FieldType == typeof(double)) && a.Precision >= 0)
                            result = (float)Math.Round(result, a.Precision);
                        f.SetValue(container, Convert.ChangeType(result, f.FieldType));
                        changed = true;
                    }
                }
                else if (a.Type == DrawType.Toggle)
                {
                    if (!Array.Exists(toggleTypes, x => x == f.FieldType))
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.Toggle}");

                    options.Add(GUILayout.ExpandWidth(false));
                    options.Add(a.Height != 0 ? GUILayout.Height(a.Height) : GUILayout.Height(Scale((int)drawHeight)));
                    if (a.Vertical)
                        GUILayout.BeginVertical();
                    else
                        GUILayout.BeginHorizontal();

                    BeginHorizontalTooltip(null, a);
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                    EndHorizontalTooltip(null, a);

                    var val = (bool)f.GetValue(container);
                    var result = GUILayout.Toggle(val, "", options.ToArray());
                    if (a.Vertical)
                        GUILayout.EndVertical();
                    else
                        GUILayout.EndHorizontal();
                    if (result != val)
                    {
                        f.SetValue(container, Convert.ChangeType(result, f.FieldType));
                        changed = true;
                    }
                }
                else if (a.Type == DrawType.ToggleGroup)
                {
                    if (!f.FieldType.IsEnum)
                    {
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.ToggleGroup}");
                    }

                    options.Add(GUILayout.ExpandWidth(false));
                    options.Add(a.Height != 0 ? GUILayout.Height(a.Height) : GUILayout.Height(Scale((int)drawHeight)));
                    if (a.Vertical)
                        GUILayout.BeginVertical();
                    else
                        GUILayout.BeginHorizontal();

                    BeginHorizontalTooltip(null, a);
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                    EndHorizontalTooltip(null, a);

                    if (!a.Vertical)
                        GUILayout.Space(Scale(5));
                    var names = Enum.GetNames(f.FieldType);
                    var values = Enum.GetValues(f.FieldType);
                    var val = f.GetValue(container);
                    var index = Array.IndexOf(values, val);
                    if (ToggleGroup(ref index, names, null, options.ToArray()))
                    {
                        var v = Enum.Parse(f.FieldType, names[index]);
                        f.SetValue(container, v);
                        changed = true;
                    }
                    if (a.Vertical)
                        GUILayout.EndVertical();
                    else
                        GUILayout.EndHorizontal();
                }
                else if (a.Type == DrawType.PopupList)
                {
                    if (!f.FieldType.IsEnum)
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.PopupList}");

                    options.Add(GUILayout.ExpandWidth(false));
                    options.Add(a.Height != 0 ? GUILayout.Height(a.Height) : GUILayout.Height(Scale((int)drawHeight)));
                    if (a.Vertical)
                        GUILayout.BeginVertical();
                    else
                        GUILayout.BeginHorizontal();

                    BeginHorizontalTooltip(null, a);
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                    EndHorizontalTooltip(null, a);

                    if (!a.Vertical)
                        GUILayout.Space(Scale(5));
                    var names = Enum.GetNames(f.FieldType);
                    var values = Enum.GetValues(f.FieldType);
                    var val = f.GetValue(container);
                    var index = Array.IndexOf(values, val);
                    if (PopupToggleGroup(ref index, names, fieldName, unique, null, options.ToArray()))
                    {
                        var v = Enum.Parse(f.FieldType, names[index]);
                        f.SetValue(container, v);
                        changed = true;
                    }
                    if (a.Vertical)
                        GUILayout.EndVertical();
                    else
                        GUILayout.EndHorizontal();
                }
                else if (a.Type == DrawType.KeyBinding || a.Type == DrawType.KeyBindingNoMod)
                {
                    if (f.FieldType != typeof(KeyBinding))
                        throw new Exception($"Type {f.FieldType} can't be drawn as {DrawType.KeyBinding}");

                    if (a.Vertical)
                        GUILayout.BeginVertical();
                    else
                        GUILayout.BeginHorizontal();

                    BeginHorizontalTooltip(null, a);
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));
                    EndHorizontalTooltip(null, a);

                    if (!a.Vertical)
                        GUILayout.Space(Scale(5));
                    var key = (KeyBinding)f.GetValue(container);
                    if (key == null)
                        key = new KeyBinding();

                    DrawKeybindingSmart(key, fieldName, (k) =>
                    {
                        f.SetValue(container, k);
                        changed = true;
                    }, a.Type == DrawType.KeyBindingNoMod, null, options.ToArray());

                    if (a.Vertical)
                    {
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }
                else context.ResolveDrawer(a.Type)?.Invoke(context, f);
            }
            return changed;
        }
        private static bool DependsOn(string str, object container, Type type)
        {
            var param = str.Split('|');
            if (param.Length != 2)
            {
                throw new Exception($"VisibleOn/InvisibleOn({str}) must have 2 params, name and value, e.g (FieldName|True) or (#PropertyName|True).");
            }

            var isField = !str.StartsWith("#");
            if (isField)
            {
                var dependsOnField = type.GetField(param[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (dependsOnField == null)
                    throw new Exception($"Field '{param[0]}' not found. Insert # at the beginning for properties.");
                if (!dependsOnField.FieldType.IsPrimitive && !dependsOnField.FieldType.IsEnum)
                    throw new Exception($"Type '{dependsOnField.FieldType.Name}' is not supported.");
                object dependsOnValue;
                if (dependsOnField.FieldType.IsEnum)
                {
                    dependsOnValue = Enum.Parse(dependsOnField.FieldType, param[1]);
                    if (dependsOnValue == null)
                        throw new Exception($"Value '{param[1]}' cannot be parsed.");
                }
                else if (dependsOnField.FieldType == typeof(string))
                {
                    dependsOnValue = param[1];
                }
                else
                {
                    dependsOnValue = Convert.ChangeType(param[1], dependsOnField.FieldType);
                    if (dependsOnValue == null)
                        throw new Exception($"Value '{param[1]}' cannot be parsed.");
                }
                var value = dependsOnField.GetValue(container);
                return value.GetHashCode() == dependsOnValue.GetHashCode();
            }
            else
            {
                param[0] = param[0].TrimStart('#');
                var dependsOnProperty = type.GetProperty(param[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (dependsOnProperty == null)
                    throw new Exception($"Property '{param[0]}' not found.");
                if (!dependsOnProperty.PropertyType.IsPrimitive && !dependsOnProperty.PropertyType.IsEnum)
                    throw new Exception($"Type '{dependsOnProperty.PropertyType.Name}' is not supported.");
                object dependsOnValue;
                if (dependsOnProperty.PropertyType.IsEnum)
                {
                    dependsOnValue = Enum.Parse(dependsOnProperty.PropertyType, param[1]);
                    if (dependsOnValue == null)
                        throw new Exception($"Value '{param[1]}' cannot be parsed.");
                }
                else if (dependsOnProperty.PropertyType == typeof(string))
                {
                    dependsOnValue = param[1];
                }
                else
                {
                    dependsOnValue = Convert.ChangeType(param[1], dependsOnProperty.PropertyType);
                    if (dependsOnValue == null)
                        throw new Exception($"Value '{param[1]}' cannot be parsed.");
                }
                var value = dependsOnProperty.GetValue(container, null);
                return value.GetHashCode() == dependsOnValue.GetHashCode();
            }
        }
        #endregion
        #region UMM.UI Resolves
        private static FastInvokeHandler BeginTooltip = MethodInvoker.GetHandler(AccessTools.Method(typeof(UI), "BeginTooltip"));
        private static FastInvokeHandler EndTooltip = MethodInvoker.GetHandler(AccessTools.Method(typeof(UI), "EndTooltip"));
        private static FastInvokeHandler BeginHorizontalTooltip = MethodInvoker.GetHandler(AccessTools.Method(typeof(UI), "BeginHorizontalTooltip"));
        private static FastInvokeHandler EndHorizontalTooltip = MethodInvoker.GetHandler(AccessTools.Method(typeof(UI), "EndHorizontalTooltip"));
        private static AccessTools.FieldRef<UI, float> drawHeight_f = AccessTools.FieldRefAccess<UI, float>(AccessTools.Field(typeof(UI), "drawHeight"));
        private static AccessTools.FieldRef<UI, List<int>> collapsibleStates_f = AccessTools.FieldRefAccess<UI, List<int>>(AccessTools.Field(typeof(UI), "collapsibleStates"));
        private static AccessTools.FieldRef<UI, Type[]> fieldTypes_f = AccessTools.FieldRefAccess<UI, Type[]>(AccessTools.Field(typeof(UI), "fieldTypes"));
        private static AccessTools.FieldRef<UI, Type[]> sliderTypes_f = AccessTools.FieldRefAccess<UI, Type[]>(AccessTools.Field(typeof(UI), "sliderTypes"));
        private static AccessTools.FieldRef<UI, Type[]> toggleTypes_f = AccessTools.FieldRefAccess<UI, Type[]>(AccessTools.Field(typeof(UI), "toggleTypes"));
        private static AccessTools.FieldRef<UI, Type[]> specialTypes_f = AccessTools.FieldRefAccess<UI, Type[]>(AccessTools.Field(typeof(UI), "specialTypes"));
        private static float drawHeight { get => drawHeight_f(); set => drawHeight_f() = value; }
        private static List<int> collapsibleStates { get => collapsibleStates_f(); set => collapsibleStates_f() = value; }
        private static Type[] fieldTypes { get => fieldTypes_f(); set => fieldTypes_f() = value; }
        private static Type[] sliderTypes { get => sliderTypes_f(); set => sliderTypes_f() = value; }
        private static Type[] toggleTypes { get => toggleTypes_f(); set => toggleTypes_f() = value; }
        private static Type[] specialTypes { get => specialTypes_f(); set => specialTypes_f() = value; }
        #endregion
    }
}
