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
static internal class JsState
{
    //Exposing field for perf
    /// <summary>
    /// The writing key count
    /// </summary>
    [ThreadStatic]
    static internal int WritingKeyCount;

    /// <summary>
    /// The is writing value
    /// </summary>
    [ThreadStatic]
    static internal bool IsWritingValue;

    /// <summary>
    /// The is writing dynamic
    /// </summary>
    [ThreadStatic]
    static internal bool IsWritingDynamic;

    /// <summary>
    /// The is runtime type
    /// </summary>
    [ThreadStatic]
    static internal bool IsRuntimeType;

    /// <summary>
    /// The query string mode
    /// </summary>
    [ThreadStatic]
    static internal bool QueryStringMode;

    /// <summary>
    /// The depth
    /// </summary>
    [ThreadStatic]
    static internal int Depth;

    /// <summary>
    /// The is CSV
    /// </summary>
    [ThreadStatic]
    static internal bool IsCsv;

    /// <summary>
    /// The declaring type
    /// </summary>
    [ThreadStatic]
    static internal Type DeclaringType;

    /// <summary>
    /// The in serializer FNS
    /// </summary>
    [ThreadStatic]
    static internal HashSet<Type> InSerializerFns;

    /// <summary>
    /// Registers the serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static internal void RegisterSerializer<T>()
    {
        InSerializerFns ??= [];

        InSerializerFns.Add(typeof(T));
    }

    /// <summary>
    /// Uns the register serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static internal void UnRegisterSerializer<T>()
    {
        InSerializerFns?.Remove(typeof(T));
    }

    /// <summary>
    /// Ins the serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    static internal bool InSerializer<T>()
    {
        return InSerializerFns != null && InSerializerFns.Contains(typeof(T));
    }

    /// <summary>
    /// The in deserializer FNS
    /// </summary>
    [ThreadStatic]
    static internal HashSet<Type> InDeserializerFns;

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
    static internal void RegisterDeserializer<T>()
    {
        InDeserializerFns ??= [];

        InDeserializerFns.Add(typeof(T));
    }

    /// <summary>
    /// Uns the register deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static internal void UnRegisterDeserializer<T>()
    {
        InDeserializerFns?.Remove(typeof(T));
    }

    /// <summary>
    /// Ins the deserializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    static internal bool InDeserializer<T>()
    {
        return InDeserializerFns != null && InDeserializerFns.Contains(typeof(T));
    }

    /// <summary>
    /// Traverses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static internal bool Traverse(object value)
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
    static internal void UnTraverse()
    {
        --Depth;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    static internal void Reset()
    {
        InSerializerFns = null;
        InDeserializerFns = null;
    }
}