using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KeyViewer
{
    public class Capture
    {
        static bool RequireSkip(CaptureMode mode, Type t, string name, Predicate<string> exclude)
        {
            if (t == null) return true;
            if (exclude?.Invoke(name) ?? false) return true;
            switch (mode)
            {
                case CaptureMode.Class:
                    return !t.IsClass;
                case CaptureMode.Struct:
                    return !t.IsValueType && !t.IsPrimitive;
                default: return false;
            }
        }
        public static Capture<T> CaptureValues<T>(T t, bool includePrivate = false, CaptureMode captureMode = CaptureMode.ClassAndStruct, Predicate<string> exclude = null)
        {
            var bf = BindingFlags.Public | BindingFlags.Instance;
            if (includePrivate) bf |= BindingFlags.NonPublic;
            if (Equals(t, null)) return null;
            var values = new Dictionary<string, object>();
            foreach (var field in Capture<T>.Type.GetFields(bf))
            {
                if (RequireSkip(captureMode, field.FieldType, field.Name, exclude)) continue;
                values.Add(field.Name, field.GetValue(t));
            }
            foreach (var prop in Capture<T>.Type.GetProperties(bf))
            {
                if (RequireSkip(captureMode, prop.PropertyType, prop.Name, exclude)) continue;
                if (prop.GetGetMethod(includePrivate) == null) continue;
                if (prop.GetIndexParameters().Length > 0) continue;
                values.Add(prop.Name, prop.GetValue(t));
            }
            return new Capture<T>(t, includePrivate, captureMode, exclude, values);
        }
        public static Capture<T> CaptureClasses<T>(T t, bool includePrivate = false, Predicate<string> exclude = null) => CaptureValues(t, includePrivate, CaptureMode.Class, exclude);
        public static Capture<T> CaptureStructs<T>(T t, bool includePrivate = false, Predicate<string> exclude = null) => CaptureValues(t, includePrivate, CaptureMode.Struct, exclude);
        public static void UncaptureValues<T>(Capture<T> capture, T target)
        {
            var bf = BindingFlags.Public | BindingFlags.Instance;
            if (capture.includePrivate) bf |= BindingFlags.NonPublic;
            foreach (var field in Capture<T>.Type.GetFields(bf))
                if (capture.values.TryGetValue(field.Name, out object value))
                    field.SetValue(target, value);
            foreach (var prop in Capture<T>.Type.GetProperties(bf))
            {
                if (prop.GetSetMethod(capture.includePrivate) == null) continue;
                if (prop.GetIndexParameters().Length > 0) continue;
                if (capture.values.TryGetValue(prop.Name, out object value))
                    prop.SetValue(target, value);
            }
        }
    }
    public class Capture<T>
    {
        public static readonly Type Type = typeof(T);
        public readonly T original;
        public readonly bool includePrivate;
        public readonly CaptureMode captureMode;
        public readonly Predicate<string> excludePredicate;
        public readonly Dictionary<string, object> values;
        internal Capture(T original, bool includePrivate, CaptureMode captureMode, Predicate<string> excludePredicate, Dictionary<string, object> values)
        {
            this.original = original;
            this.includePrivate = includePrivate;
            this.captureMode = captureMode;
            this.excludePredicate = excludePredicate;
            this.values = values;
        }
        public static implicit operator Capture<T>(T t) => Capture.CaptureValues(t);
        public static implicit operator T(Capture<T> capture)
        {
            T t;
            var ctor = Type.GetConstructor((BindingFlags)15420, null, Type.EmptyTypes, null);
            if (ctor != null) t = (T)ctor.Invoke(null);
            else t = (T)FormatterServices.GetUninitializedObject(Type);
            Capture.UncaptureValues(capture, t);
            return t;
        }
    }
    public enum CaptureMode
    {
        Class,
        Struct,
        ClassAndStruct,
    }
}
