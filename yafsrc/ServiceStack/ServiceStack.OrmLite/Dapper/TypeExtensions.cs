// ***********************************************************************
// <copyright file="TypeExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Reflection;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class TypeExtensions.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Gets the public instance method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="types">The types.</param>
    /// <returns>MethodInfo.</returns>
    public static MethodInfo GetPublicInstanceMethod(this Type type, string name, Type[] types)
        => type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public, null, types, null);
}