// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="TypeHelper.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Utilities;

using System;

/// <summary>
/// Helper class for dealing with types.
/// </summary>
static internal class TypeHelper
{
    /// <summary>
    /// Loads and activates a type
    /// </summary>
    /// <param name="fullTypeName">The full name of the type to activate "TypeName, AssemblyName"</param>
    /// <param name="args">Arguments to pass to the constructor</param>
    /// <returns>The object</returns>
    public static object Activate(string fullTypeName, object[] args)
    {
        var components = fullTypeName.Split([','], 2);
        if (components.Length != 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fullTypeName),
                fullTypeName,
                MessageProvider.FormatString(Message.FullTypeNameRequiresAssemblyName));
        }

        return Activate(components[1].Trim(), components[0].Trim(), args);
    }

    /// <summary>
    /// Loads and activates a type
    /// </summary>
    /// <param name="assemblyName">The assembly name</param>
    /// <param name="typeName">The type name</param>
    /// <param name="args">Arguments to pass to the constructor</param>
    /// <returns>The object</returns>
    public static object Activate(string assemblyName, string typeName, object[] args)
    {
        if (assemblyName.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(assemblyName),
                assemblyName,
                MessageProvider.FormatString(Message.AssemblyNameRequired));
        }

        if (typeName.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(typeName),
                typeName,
                MessageProvider.FormatString(Message.TypeNameRequired));
        }

        return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
            assemblyName,
            typeName,
            false,
            0,
            null,
            args,
            null,
            null);
    }
}