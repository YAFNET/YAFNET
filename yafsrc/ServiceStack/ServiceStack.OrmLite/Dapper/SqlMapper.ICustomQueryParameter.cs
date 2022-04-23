// ***********************************************************************
// <copyright file="SqlMapper.ICustomQueryParameter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Implement this interface to pass an arbitrary db specific parameter to Dapper
    /// </summary>
    public interface ICustomQueryParameter
    {
        /// <summary>
        /// Add the parameter needed to the command before it executes
        /// </summary>
        /// <param name="command">The raw command prior to execution</param>
        /// <param name="name">Parameter name</param>
        void AddParameter(IDbCommand command, string name);
    }
}