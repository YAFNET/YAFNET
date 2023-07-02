// ***********************************************************************
// <copyright file="NullLogFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Logging;

/// <summary>
/// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
/// Made public so its testable
/// </summary>
public class NullLogFactory : ILogFactory
{
    /// <summary>
    /// The debug enabled
    /// </summary>
    private readonly bool debugEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullLogFactory" /> class.
    /// </summary>
    /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
    public NullLogFactory(bool debugEnabled = false)
    {
        this.debugEnabled = debugEnabled;
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ILog.</returns>
    public ILog GetLogger(Type type)
    {
        return new NullDebugLogger(type) { IsDebugEnabled = debugEnabled };
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>ILog.</returns>
    public ILog GetLogger(string typeName)
    {
        return new NullDebugLogger(typeName) { IsDebugEnabled = debugEnabled };
    }
}