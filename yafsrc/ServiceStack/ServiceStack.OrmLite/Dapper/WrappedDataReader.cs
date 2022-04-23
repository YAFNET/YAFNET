// ***********************************************************************
// <copyright file="WrappedDataReader.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Describes a reader that controls the lifetime of both a command and a reader,
/// exposing the downstream command/reader as properties.
/// </summary>
public interface IWrappedDataReader : IDataReader
{
    /// <summary>
    /// Obtain the underlying reader
    /// </summary>
    /// <value>The reader.</value>
    IDataReader Reader { get; }
    /// <summary>
    /// Obtain the underlying command
    /// </summary>
    /// <value>The command.</value>
    IDbCommand Command { get; }
}