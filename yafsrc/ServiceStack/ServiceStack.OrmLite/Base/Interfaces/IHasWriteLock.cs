// ***********************************************************************
// <copyright file="IHasWriteLock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

/// <summary>
/// Interface IHasWriteLock
/// </summary>
public interface IHasWriteLock
{
    /// <summary>
    /// Gets the write lock.
    /// </summary>
    /// <value>The write lock.</value>
    object WriteLock { get; }
}