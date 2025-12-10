// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogFactory.cs" company="ServiceStack, Inc.">
//   Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>
//   Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ServiceStack.Logging;

/// <summary>
/// Creates a Console Logger, that logs all messages to: System.Console
/// Made public so its testable
/// </summary>
public class ConsoleLogFactory : ILogFactory
{
    /// <summary>
    /// The debug enabled
    /// </summary>
    private readonly bool debugEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogFactory" /> class.
    /// </summary>
    /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
    public ConsoleLogFactory(bool debugEnabled = true)
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
        return new ConsoleLogger(type) { IsDebugEnabled = this.debugEnabled };
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>ILog.</returns>
    public ILog GetLogger(string typeName)
    {
        return new ConsoleLogger(typeName) { IsDebugEnabled = this.debugEnabled };
    }

    /// <summary>
    /// Configures the specified debug enabled.
    /// </summary>
    /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
    public static void Configure(bool debugEnabled = true)
    {
        LogManager.LogFactory = new ConsoleLogFactory();
    }
}