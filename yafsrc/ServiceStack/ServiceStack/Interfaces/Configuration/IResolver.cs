// ***********************************************************************
// <copyright file="IResolver.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Configuration;

/// <summary>
/// Interface IResolver
/// </summary>
public interface IResolver
{
    /// <summary>
    /// Resolve a dependency from the AppHost's IOC
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    T TryResolve<T>();
}