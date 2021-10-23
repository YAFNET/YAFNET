// ***********************************************************************
// <copyright file="AssemblyUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace ServiceStack.Text
{
    using ServiceStack.Text.Support;

    /// <summary>
    /// Utils to load types
    /// </summary>
    public static class AssemblyUtils
    {
        /// <summary>
        /// The file URI
        /// </summary>
        private const string FileUri = "file:///";
        /// <summary>
        /// The URI seperator
        /// </summary>
        private const char UriSeperator = '/';

        /// <summary>
        /// The type cache
        /// </summary>
        private static Dictionary<string, Type> TypeCache = new();

        /// <summary>
        /// Find the type from the name supplied
        /// </summary>
        /// <param name="typeName">[typeName] or [typeName, assemblyName]</param>
        /// <returns>System.Type.</returns>
        public static Type FindType(string typeName)
        {
            if (TypeCache.TryGetValue(typeName, out var type)) return type;

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
        /// The top-most interface of the given type, if any.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Type.</returns>
        public static Type MainInterface<T>()
        {
            var t = typeof(T);
            if (t.BaseType != typeof(object)) return t; // not safe to use interface, as it might be a superclass one.
            // on Windows, this can be just "t.GetInterfaces()" but Mono doesn't return in order.
            var interfaceType = t.GetInterfaces().FirstOrDefault(i => !t.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i)));
            return interfaceType ?? t;
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
        /// Loads the assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>System.Reflection.Assembly.</returns>
        public static Assembly LoadAssembly(string assemblyPath)
        {
            return PclExport.Instance.LoadAssembly(assemblyPath);
        }

        /// <summary>
        /// Gets the assembly bin path.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>string.</returns>
        public static string GetAssemblyBinPath(Assembly assembly)
        {
            var codeBase = PclExport.Instance.GetAssemblyCodeBase(assembly);
            var binPathPos = codeBase.LastIndexOf(UriSeperator);
            var assemblyPath = codeBase.Substring(0, binPathPos + 1);
            if (assemblyPath.StartsWith(FileUri, StringComparison.OrdinalIgnoreCase))
            {
                assemblyPath = assemblyPath.Remove(0, FileUri.Length);
            }
            return assemblyPath;
        }

        /// <summary>
        /// The version reg ex
        /// </summary>
        static readonly Regex versionRegEx = new(", Version=[^\\]]+", PclExport.Instance.RegexOptions);

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
    }
}