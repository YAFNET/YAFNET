// ***********************************************************************
// <copyright file="LogManager.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Logging;

/// <summary>
/// Logging API for this library. You can inject your own implementation otherwise
/// will use the DebugLogFactory to write to System.Diagnostics.Debug
/// </summary>
public class LogManager
{
    /// <summary>
    /// Gets or sets the log factory.
    /// Use this to override the factory that is used to create loggers
    /// </summary>
    /// <value>The log factory.</value>
    public static ILogFactory LogFactory {
        get => field ?? new NullLogFactory();
        set;
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ILog.</returns>
    public static ILog GetLogger(Type type)
    {
        return LogFactory.GetLogger(type);
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>ILog.</returns>
    public static ILog GetLogger(string typeName)
    {
        return LogFactory.GetLogger(typeName);
    }
}