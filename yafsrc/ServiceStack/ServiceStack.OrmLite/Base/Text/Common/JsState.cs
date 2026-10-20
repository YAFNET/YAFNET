// ***********************************************************************
// <copyright file="JsState.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class JsState.
/// </summary>
internal static class JsState
{
    //Exposing field for perf
    /// <summary>
    /// The writing key count
    /// </summary>
    [ThreadStatic]
    internal static int WritingKeyCount;

    /// <summary>
    /// The is writing value
    /// </summary>
    [ThreadStatic]
    internal static bool IsWritingValue;

    /// <summary>
    /// The is writing dynamic
    /// </summary>
    [ThreadStatic]
    internal static bool IsWritingDynamic;

    /// <summary>
    /// The is runtime type
    /// </summary>
    [ThreadStatic]
    internal static bool IsRuntimeType;

    /// <summary>
    /// The query string mode
    /// </summary>
    [ThreadStatic]
    internal static bool QueryStringMode;

    /// <summary>
    /// The depth
    /// </summary>
    [ThreadStatic]
    internal static int Depth;

    /// <summary>
    /// The is CSV
    /// </summary>
    [ThreadStatic]
    internal static bool IsCsv;

    /// <summary>
    /// The declaring type
    /// </summary>
    [ThreadStatic]
    internal static Type DeclaringType;

    /// <summary>
    /// The in serializer FNS
    /// </summary>
    [ThreadStatic]
    internal static HashSet<Type> InSerializerFns;

    /// <summary>
    /// Registers the serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void RegisterSerializer<T>()
    {
        InSerializerFns ??= [];

        InSerializerFns.Add(typeof(T));
    }

    /// <summary>
    /// Uns the register serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void UnRegisterSerializer<T>()
    {
        InSerializerFns?.Remove(typeof(T));
    }

    /// <summary>
    /// Ins the serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool InSerializer<T>()
    {
        return InSerializerFns != null && InSerializerFns.Contains(typeof(T));
    }

    /// <summary>
    /// The in deserializer FNS
    /// </summary>
    [ThreadStatic]
    internal static HashSet<Type> InDeserializerFns;

    static JsState()
    {
        IsWritingValue = false;
        IsWritingDynamic = false;
        IsRuntimeType = false;
        QueryStringMode = false;
        InSerializerFns = [];
        WritingKeyCount = 0;
    }

    /// <summary>
    /// Registers the deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void RegisterDeserializer<T>()
    {
        InDeserializerFns ??= [];

        InDeserializerFns.Add(typeof(T));
    }

    /// <summary>
    /// Uns the register deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void UnRegisterDeserializer<T>()
    {
        InDeserializerFns?.Remove(typeof(T));
    }

    /// <summary>
    /// Ins the deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool InDeserializer<T>()
    {
        return InDeserializerFns != null && InDeserializerFns.Contains(typeof(T));
    }

    /// <summary>
    /// Traverses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Traverse(object value)
    {
        if (++Depth <= JsConfig.MaxDepth)
        {
            return true;
        }

        Tracer.Instance.WriteError(
            $"Exceeded MaxDepth limit of {JsConfig.MaxDepth} attempting to serialize {value.GetType().Name}");
        return false;
    }

    /// <summary>
    /// Uns the traverse.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void UnTraverse()
    {
        --Depth;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    internal static void Reset()
    {
        InSerializerFns = null;
        InDeserializerFns = null;
    }
}