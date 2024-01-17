﻿using Esprima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using System;
using System.Linq;
using System.Reflection;

namespace KeyViewer.Scripting
{
    public class FIWrapper
    {
        public readonly Engine engine;
        public readonly FunctionInstance fi;
        public readonly string[] args;
        public FIWrapper(FunctionInstance fi)
        {
            this.fi = fi;
            engine = fi.Engine;
            args = fi.FunctionDeclaration.Params.Select(n => ((Identifier)n).Name).ToArray();
        }
        public object Call(params object[] args) => fi.Call(null, args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]).ToObject();
        public JsValue CallRaw(params object[] args) => fi.Call(null, args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]);
        public static readonly MethodInfo CallMethod = typeof(FIWrapper).GetMethod("Call");
        public static readonly MethodInfo CallRawMethod = typeof(FIWrapper).GetMethod("CallRaw");
    }
}