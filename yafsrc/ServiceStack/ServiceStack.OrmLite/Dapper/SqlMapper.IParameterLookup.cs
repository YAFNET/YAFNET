// ***********************************************************************
// <copyright file="SqlMapper.IParameterLookup.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Extends IDynamicParameters providing by-name lookup of parameter values
    /// </summary>
    public interface IParameterLookup : IDynamicParameters
    {
        /// <summary>
        /// Get the value of the specified parameter (return null if not found)
        /// </summary>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>System.Object.</returns>
        object this[string name] { get; }
    }
}