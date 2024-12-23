// ***********************************************************************
// <copyright file="AssemblyUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text.Support;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Utils to load types
/// </summary>
public static partial class AssemblyUtils
{
    /// <summary>
    /// The type cache
    /// </summary>
    private static Dictionary<string, Type> TypeCache = [];

    /// <summary>
    /// Gets or sets the name of the validate type.
    /// </summary>
    /// <value>The name of the validate type.</value>
    public static Func<string, bool> ValidateTypeName { get; set; } = DefaultValidateTypeName;

    /// <summary>
    /// Gets or sets the validate type regex.
    /// </summary>
    /// <value>The validate type regex.</value>
    public static Regex ValidateTypeRegex { get; set; } =
        ValidateType();

    /// <summary>
    /// Defaults the name of the validate type.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool DefaultValidateTypeName(string typeName)
    {
        return ValidateTypeRegex.IsMatch(typeName);
    }

    /// <summary>
    /// Find the type from the name supplied
    /// </summary>
    /// <param name="typeName">[typeName] or [typeName, assemblyName]</param>
    /// <returns>System.Type.</returns>
    public static Type FindType(string typeName)
    {
        if (!ValidateTypeName(typeName))
        {
            throw new NotSupportedException($"Invalid Type Name: {typeName}");
        }

        return UncheckedFindType(typeName);
    }


    /// <summary>
    /// Uncheckeds the type of the find.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>Type.</returns>
    public static Type UncheckedFindType(string typeName)
    {
        if (TypeCache.TryGetValue(typeName, out var type))
        {
            return type;
        }

        type = Type.GetType(typeName);
        if (type == null)
        {
            var typeDef = new AssemblyTypeDefinition(typeName);
            type = !string.IsNullOrEmpty(typeDef.AssemblyName)
                ? FindType(typeDef.TypeName, typeDef.AssemblyName)
                : FindTypeFromLoadedAssemblies(typeDef.TypeName);
        }

        Dictionary<string, Type> snapshot, newCache;
        do
        {
            snapshot = TypeCache;
            newCache = new Dictionary<string, Type>(TypeCache) { [typeName] = type };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref TypeCache, newCache, snapshot), snapshot));

        return type;
    }

    /// <summary>
    /// Find type if it exists
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="assemblyName">Name of the assembly.</param>
    /// <returns>The type if it exists</returns>
    public static Type FindType(string typeName, string assemblyName)
    {
        var type = FindTypeFromLoadedAssemblies(typeName);
        return type ?? PclExport.Instance.FindType(typeName, assemblyName);
    }

    /// <summary>
    /// The top-most interface of the given type, if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.Type.</returns>
    public static Type MainInterface<T>()
    {
        var t = typeof(T);
        if (t.BaseType != typeof(object))
        {
            return t; // not safe to use interface, as it might be a superclass one.
        }

        // on Windows, this can be just "t.GetInterfaces()" but Mono doesn't return in order.
        var interfaceType = t.GetInterfaces().FirstOrDefault(i => !t.GetInterfaces().Exists(i2 => i2.GetInterfaces().Contains(i)));
        return interfaceType ?? t;
    }

    /// <summary>
    /// Finds the type from loaded assemblies.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.Type.</returns>
    public static Type FindTypeFromLoadedAssemblies(string typeName)
    {
        var assemblies = PclExport.Instance.GetAllAssemblies();
        return assemblies.Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type != null);
    }

    /// <summary>
    /// The version reg ex
    /// </summary>
    private readonly static Regex versionRegEx = new(", Version=[^\\]]+", PclExport.Instance.RegexOptions,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Converts to typestring.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>string.</returns>
    public static string ToTypeString(this Type type)
    {
        return versionRegEx.Replace(type.AssemblyQualifiedName, "");
    }

    /// <summary>
    /// Writes the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>string.</returns>
    public static string WriteType(Type type)
    {
        return type.ToTypeString();
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_.,+`\[\]\s]+$", RegexOptions.Compiled)]
    private static partial Regex ValidateType();
}