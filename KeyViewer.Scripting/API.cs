using DG.Tweening;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Interop;
using JSNet.API;
using KeyViewer.API;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Migration.V3;
using KeyViewer.Models;
using KeyViewer.Scripting.Proxies;
using KeyViewer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;

namespace KeyViewer.Scripting
{
    public static class API
    {
        [Api("GetAllProfileNames",
            Comment = new string[]
            {
                "Get All Profile Names"
            })]
        public static string[] GetAllProfileNames() => KeyViewer.Main.Managers.Keys.ToArray();
        [Api("GetManager",
            Comment = new string[]
            {
                "Get Key Manager Object"
            },
            ParamComment = new string[]
            {
                "Key Viewer Profile Name (File Name Without Extension)"
            },
            RequireTypes = new Type[]
            {
                typeof(KeyCode),
                typeof(Ease),
                typeof(RainImageDisplayMode),
                typeof(Pivot),
                typeof(Anchor),
                typeof(Models.Direction),
                typeof(Color),
                typeof(VertexGradient),
                typeof(GColor),
                typeof(RainImage),
                typeof(ActiveProfile),
                typeof(Vector2),
                typeof(Vector3),
                typeof(PressRelease<string>),
                typeof(PressRelease<float>),
                typeof(PressRelease<int>),
                typeof(PressRelease<Vector2>),
                typeof(PressRelease<Vector3>),
                typeof(PressRelease<GColor>),
                typeof(Judge<GColor>),
                typeof(GUIStatus),
                typeof(EaseConfig),
                typeof(VectorConfig),
                typeof(ObjectConfig),
                typeof(KeyConfig),
                typeof(RainConfig),
                typeof(Models.Profile),
                typeof(KeyManagerProxy),
                typeof(KeyProxy),
                typeof(ListProxy),
            },
            RequireTypesAliases = new string[]
            {
                null,
                null,
                null,
                null,
                null,
                null,
                "Color",
                "Gradient",
                "GradientColor",
                null,
                null,
                null,
                null,
                "PressRelease_String",
                "PressRelease_Float",
                "PressRelease_Int",
                "PressRelease_Vector2",
                "PressRelease_Vector3",
                "PressRelease_GradientColor",
                "JudgeColor",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                "KeyManager",
                "Key",
                "List",
            })]
        public static KeyManagerProxy GetManager(string profileName) => KeyViewer.Main.Managers.TryGetValue(profileName, out var manager) ? new KeyManagerProxy(manager) : null;
        [Api("GetKey",
            Comment = new string[]
            {
                "Get Key From Key Manager Object"
            },
            ParamComment = new string[]
            {
                "Key Manager Object",
                "Key Name (KeyCode Or Dummy Key Name) ex) KeyCode.Semicolon, KPS"
            })]
        public static KeyProxy GetKey(KeyManagerProxy manager, object keyName)
        {
            Unity.Key result = null;
            if (keyName is double d)
                result = manager.manager.keys.Find(k => k.Config.Code == (KeyCode)(int)d);
            else if (keyName is string str)
                result = manager.manager.keys.Find(k => KeyViewerUtils.KeyName(k.Config) == str);
            if (result != null) return new KeyProxy(result);
            return null;
        }
        [Api("Apply",
            Comment = new string[]
            {
                "Apply Config Changes"
            },
            ParamComment = new string[]
            {
                "Key Manager Object"
            })]
        public static void UpdateLayout(KeyManagerProxy manager) => manager.manager.UpdateLayout();
        [Api("RegisterTag",
            Comment = new string[]
            {
                "Register Tag (Like Overlayer)"
            },
            ParamComment = new string[]
            {
                "To Register Key Manager",
                "Tag Name",
                "Tag Function"
            })]
        public static void RegisterTag(KeyManagerProxy manager, string name, JsValue func)
        {
            if (!(func is FunctionInstance fi)) return;
            Tag tag = new Tag(name);
            tag.SetGetter(GenerateTagWrapper(new FIWrapper(fi)));
            manager.manager.AllTags.Add(tag);
        }
        [Api("OnKeyPressed",
            Comment = new string[]
            {
                "On Key Pressed Event"
            },
            ParamComment = new string[]
            {
                "Pressed Key Code",
                "Event Callback"
            })]
        public static void OnKeyPressed(KeyCode code, JsValue func)
        {
            if (!(func is FunctionInstance fi)) return;
            InputAPI.EventActive = true;
            FIWrapper wrapper = new FIWrapper(fi);
            InputAPI.OnKeyPressed += c =>
            {
                if (code == c.Config.Code)
                    wrapper.CallRaw();
            };
        }
        [Api("OnKeyReleased",
            Comment = new string[]
            {
                "On Key Released Event"
            },
            ParamComment = new string[]
            {
                "Released Key Code",
                "Event Callback"
            })]
        public static void OnKeyReleased(KeyCode code, JsValue func)
        {
            if (!(func is FunctionInstance fi)) return;
            InputAPI.EventActive = true;
            FIWrapper wrapper = new FIWrapper(fi);
            InputAPI.OnKeyReleased += c =>
            {
                if (code == c.Config.Code)
                    wrapper.CallRaw();
            };
        }
        [Api("EasedValue",
            Comment = new string[]
            {
                "Eased Value"
            },
            ParamComment = new string[]
            {
                "Ease Type",
                "Lifetime (0.0 ~ 1.0)"
            },
            ReturnComment = "Eased Value")]
        public static float EasedValue(Ease ease, float lifetime)
        {
            return DOVirtual.EasedValue(0, 1, lifetime, ease);
        }
        [Api("DelayedCall",
            Comment = new string[]
            {
                "Delayed Call"
            },
            ParamComment = new string[]
            {
                "Delay Seconds",
                "Callback"
            },
            ReturnComment = "Delayed Caller Handle")]
        public static object DelayedCall(float seconds, JsValue func)
        {
            if (!(func is FunctionInstance fi)) return null;
            FIWrapper wrapper = new FIWrapper(fi);
            return DOVirtual.DelayedCall(seconds, () => wrapper.CallRaw(), false);
        }
        [Api("CancelDelayedCall",
            Comment = new string[]
            {
                "Cancel Delayed Call"
            },
            ParamComment = new string[]
            {
                "Delayed Caller Handle",
            })]
        public static void CancelDelayedCall(object handle)
        {
            if (!(handle is Sequence t)) return;
            t.Kill(false);
        }
        public static object[] StrArrayToObjArray(string[] arr)
        {
            object[] newArr = new object[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                newArr[i] = arr[i];
            return newArr;
        }
        [Api("Resolve",
            Comment = new string[]
            {
                "Resolve CLR Type"
            },
            ParamComment = new string[]
            {
                "CLR Type Full Name"
            },
            ReturnComment = "Resolved CLR Type")]
        public static TypeReference Resolve(Engine engine, string clrType)
        {
            if (jsTypes.TryGetValue(engine, out var dict))
                if (dict.TryGetValue(clrType, out var t))
                    return t;
                else return dict[clrType] = TypeReference.CreateTypeReference(engine, MiscUtils.TypeByName(clrType));
            dict = jsTypes[engine] = new Dictionary<string, TypeReference>();
            return dict[clrType] = TypeReference.CreateTypeReference(engine, MiscUtils.TypeByName(clrType));
        }
        [Api("ReadV3Settings",
            Comment = new string[]
            {
                "Read V3 Settings.xml"
            },
            ParamComment = new string[]
            {
                "Settings.xml Path"
            },
            ReturnComment = "V3 Settings Object",
            RequireTypes = new Type[]
            {
                typeof(Migration.V3.Direction),
                typeof(SpecialKeyType),
                typeof(LanguageType),
                typeof(KeyRain_Config),
                typeof(Key_Config),
                typeof(Group),
                typeof(V3Profile),
                typeof(V3Settings),
            },
            RequireTypesAliases = new string[]
            {
                "V3Direction",
                "V3SpecialKeyType",
                "V3LanguageType",
                "V3RainConfig",
                "V3KeyConfig",
                "V3Group",
                null,
                null
            })]
        public static V3Settings ReadV3Settings(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return null;
            return KeyViewer.Main.ReadV3Settings(xmlPath);
        }
        [Api("ReadV3Profile",
            Comment = new string[]
            {
                "Read V3 Profile Xml File"
            },
            ParamComment = new string[]
            {
                "Profile Xml File Path"
            },
            ReturnComment = "V3 Profile Object")]
        public static V3Profile ReadV3Profile(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return null;
            return KeyViewer.Main.ReadV3Profile(xmlPath);
        }
        static MethodInfo GenerateTagWrapper(FIWrapper wrapper)
        {
            Type[] paramTypes = wrapper.args.Select(t => typeof(string)).ToArray();
            var name = wrapper.fi.ToString().Replace("function ", string.Empty).Replace("() { [native code] }", string.Empty);
            if (string.IsNullOrWhiteSpace(name)) name = "Anonymous";
            TypeBuilder wrapperType = mod.DefineType($"{name}_WrapperType${uniqueId++}", TypeAttributes.Public);
            MethodBuilder wrapperMethod = wrapperType.DefineMethod($"{name}_WrapperMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(object), paramTypes);
            FieldBuilder wrapperField = wrapperType.DefineField("wrapper", typeof(FIWrapper), FieldAttributes.Public | FieldAttributes.Static);
            for (int i = 0; i < wrapper.args.Length; i++)
                wrapperMethod.DefineParameter(i + 1, ParameterAttributes.None, wrapper.args[i]);
            ILGenerator il = wrapperMethod.GetILGenerator();
            if (paramTypes.Length > 0)
            {
                LocalBuilder strArray = il.MakeArray<string>(paramTypes.Length);
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, strArray);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i);
                    il.Emit(OpCodes.Stelem_Ref);
                }
                il.Emit(OpCodes.Ldsfld, wrapperField);
                il.Emit(OpCodes.Ldloc, strArray);
                il.Emit(OpCodes.Call, satoa);
            }
            else
            {
                il.Emit(OpCodes.Ldsfld, wrapperField);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Newarr, typeof(object));
            }
            il.Emit(OpCodes.Call, call_fi);
            il.Emit(OpCodes.Ret);
            var resultT = wrapperType.CreateType();
            resultT.GetField("wrapper").SetValue(null, wrapper);
            return resultT.GetMethod($"{name}_WrapperMethod");
        }
        static LocalBuilder MakeArray<T>(this ILGenerator il, int length)
        {
            LocalBuilder array = il.DeclareLocal(typeof(T[]));
            il.Emit(OpCodes.Ldc_I4, length);
            il.Emit(OpCodes.Newarr, typeof(T));
            il.Emit(OpCodes.Stloc, array);
            return array;
        }
        public static Api api { get; private set; }
        static int uniqueId;
        static Dictionary<Engine, Dictionary<string, TypeReference>> jsTypes;
        static MethodInfo satoa;
        static MethodInfo call_fi;
        static AssemblyBuilder ass;
        static System.Reflection.Emit.ModuleBuilder mod;
        public static void Initialize()
        {
            uniqueId = 0;
            jsTypes = new Dictionary<Engine, Dictionary<string, TypeReference>>();
            satoa = typeof(API).GetMethod(nameof(StrArrayToObjArray), (BindingFlags)15420);
            call_fi = typeof(FIWrapper).GetMethod("Call");
            ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("KeyViewer.Scripting.TagWrapper"), AssemblyBuilderAccess.RunAndCollect);
            mod = ass.DefineDynamicModule("KeyViewer.Scripting.TagWrapper");
            api = new Api();
            api.RegisterType(typeof(API));
            api.RegisterType(typeof(ListProxy));
            var scriptsFolder = Path.Combine(Main.Mod.Path, "Scripts");
            Directory.CreateDirectory(scriptsFolder);
            File.WriteAllText(Path.Combine(scriptsFolder, "Impl.js"), api.Generate());
        }
        public static void Release()
        {
            jsTypes = null;
            satoa = null;
            call_fi = null;
            ass = null;
            mod = null;
            api = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }
    }
}
