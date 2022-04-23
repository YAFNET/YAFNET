// ***********************************************************************
// <copyright file="IContainer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack;

/// <summary>
/// Interface IContainer
/// </summary>
public interface IContainer
{
    /// <summary>
    /// Creates the factory.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Func&lt;System.Object&gt;.</returns>
    Func<object> CreateFactory(Type type);

    /// <summary>
    /// Adds the singleton.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>IContainer.</returns>
    IContainer AddSingleton(Type type, Func<object> factory);

    /// <summary>
    /// Adds the transient.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>IContainer.</returns>
    IContainer AddTransient(Type type, Func<object> factory);

    /// <summary>
    /// Resolves the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    object Resolve(Type type);

    /// <summary>
    /// Existses the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool Exists(Type type);
}