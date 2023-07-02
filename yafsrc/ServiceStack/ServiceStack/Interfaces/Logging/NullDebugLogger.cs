// ***********************************************************************
// <copyright file="NullDebugLogger.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Logging;

using System;

/// <summary>
/// Default logger is to System.Diagnostics.Debug.Print
/// Made public so its testable
/// </summary>
public class NullDebugLogger : ILog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullDebugLogger" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public NullDebugLogger(string type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullDebugLogger" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public NullDebugLogger(Type type)
    {
    }

    /// <summary>
    /// Logs a Debug message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Debug(object message, Exception exception)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is debug enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
    public bool IsDebugEnabled { get; set; }

    /// <summary>
    /// Logs a Debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Debug(object message)
    {
    }

    /// <summary>
    /// Logs a Debug format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void DebugFormat(string format, params object[] args)
    {
    }

    /// <summary>
    /// Logs a Error message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Error(object message, Exception exception)
    {
    }

    /// <summary>
    /// Logs a Error message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Error(object message)
    {
    }

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void ErrorFormat(string format, params object[] args)
    {
    }

    /// <summary>
    /// Logs a Fatal message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Fatal(object message, Exception exception)
    {
    }

    /// <summary>
    /// Logs a Fatal message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Fatal(object message)
    {
    }

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void FatalFormat(string format, params object[] args)
    {
    }

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Info(object message, Exception exception)
    {
    }

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Info(object message)
    {
    }

    /// <summary>
    /// Logs an Info format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void InfoFormat(string format, params object[] args)
    {
    }

    /// <summary>
    /// Logs a Warning message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Warn(object message, Exception exception)
    {
    }

    /// <summary>
    /// Logs a Warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Warn(object message)
    {
    }

    /// <summary>
    /// Warns the format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The arguments.</param>
    public void WarnFormat(string format, params object[] args)
    {
    }
}