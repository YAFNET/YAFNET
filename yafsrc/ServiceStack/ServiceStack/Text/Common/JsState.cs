// ***********************************************************************
// <copyright file="JsState.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServiceStack.Text.Common;

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
    internal static int WritingKeyCount = 0;

    /// <summary>
    /// The is writing value
    /// </summary>
    [ThreadStatic]
    internal static bool IsWritingValue = false;

    /// <summary>
    /// The is writing dynamic
    /// </summary>
    [ThreadStatic]
    internal static bool IsWritingDynamic = false;

    /// <summary>
    /// The is runtime type
    /// </summary>
    [ThreadStatic]
    internal static bool IsRuntimeType = false;

    /// <summary>
    /// The query string mode
    /// </summary>
    [ThreadStatic]
    internal static bool QueryStringMode = false;

    /// <summary>
    /// The depth
    /// </summary>
    [ThreadStatic]
    internal static int Depth = 0;

    /// <summary>
    /// The is CSV
    /// </summary>
    [ThreadStatic]
    internal static bool IsCsv = false;

    [ThreadStatic]
    internal static Type DeclaringType;

    /// <summary>
    /// The in serializer FNS
    /// </summary>
    [ThreadStatic]
    internal static HashSet<Type> InSerializerFns = new();

    /// <summary>
    /// Registers the serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void RegisterSerializer<T>()
    {
        InSerializerFns ??= new HashSet<Type>();

        InSerializerFns.Add(typeof(T));
    }

    /// <summary>
    /// Uns the register serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void UnRegisterSerializer<T>()
    {
        if (InSerializerFns == null)
            return;

        InSerializerFns.Remove(typeof(T));
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

    /// <summary>
    /// Registers the deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static void RegisterDeserializer<T>()
    {
        InDeserializerFns ??= new HashSet<Type>();

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
            return true;

        Tracer.Instance.WriteError(
            $"Exceeded MaxDepth limit of {JsConfig.MaxDepth} attempting to serialize {value.GetType().Name}");
        return false;
    }

    /// <summary>
    /// Uns the traverse.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void UnTraverse() => --Depth;

    /// <summary>
    /// Resets this instance.
    /// </summary>
    internal static void Reset()
    {
        InSerializerFns = null;
        InDeserializerFns = null;
    }
}