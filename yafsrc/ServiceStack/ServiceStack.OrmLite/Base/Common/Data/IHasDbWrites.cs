// ***********************************************************************
// <copyright file="IHasDbWrites.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

namespace ServiceStack.Data;

/// <summary>
/// A DB connection that records whether it has run a statement that writes, e.g. OrmLiteConnection
/// </summary>
public interface IHasDbWrites
{
    /// <summary>
    /// Gets a value indicating whether the connection has run a statement that writes.
    /// </summary>
    bool HasWrites { get; }

    /// <summary>
    /// Called before the connection runs its first statement that writes
    /// </summary>
    System.Action? OnFirstWrite { get; set; }
}
