// ***********************************************************************
// <copyright file="DeserializeTypeUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Reflection;

namespace ServiceStack.Text.Common;

using System.Linq;

/// <summary>
/// Class DeserializeTypeUtils.
/// </summary>
public class DeserializeTypeUtils
{
    /// <summary>
    /// Gets the parse method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseMethod(Type type) => v => GetParseStringSpanMethod(type)(v.AsSpan());

    /// <summary>
    /// Gets the parse string span method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanMethod(Type type)
    {
        var typeConstructor = GetTypeStringConstructor(type);
        if (typeConstructor != null)
        {
            return value => typeConstructor.Invoke(new object[] { value.ToString() });
        }

        return null;
    }

    /// <summary>
    /// Get the type(string) constructor if exists
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ConstructorInfo.</returns>
    public static ConstructorInfo GetTypeStringConstructor(Type type)
    {
        return (from ci in type.GetConstructors()
                let paramInfos = ci.GetParameters()
                let matchFound = paramInfos.Length == 1 && paramInfos[0].ParameterType == typeof(string)
                where matchFound
                select ci).FirstOrDefault();
    }
}