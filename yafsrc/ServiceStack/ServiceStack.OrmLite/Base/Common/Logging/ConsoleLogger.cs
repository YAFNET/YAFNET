// ***********************************************************************
// <copyright file="ConsoleLogger.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.Logging;

/// <summary>
/// Default logger is to Console.WriteLine
/// Made public so its testable
/// </summary>
public class ConsoleLogger : ILog
{
    /// <summary>
    /// The debug
    /// </summary>
    const string DEBUG = "DEBUG: ";
    /// <summary>
    /// The error
    /// </summary>
    const string ERROR = "ERROR: ";

    /// <summary>
    /// The warn
    /// </summary>
    const string WARN = "WARN: ";

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public ConsoleLogger(string type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public ConsoleLogger(Type type)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is debug enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
    public bool IsDebugEnabled { get; set; }

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    private static void Log(object message, Exception exception)
    {
        var msg = message?.ToString() ?? string.Empty;
        if (exception != null)
        {
            msg += ", Exception: " + exception.Message + "\n" + exception;
        }

        Console.WriteLine(msg);
    }

    /// <summary>
    /// Logs the format.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    private static void LogFormat(object message, params object[] args)
    {
        var msg = message?.ToString() ?? string.Empty;
        Console.WriteLine(msg, args);
    }

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void Log(object message)
    {
        var msg = message?.ToString() ?? string.Empty;
        Console.WriteLine(msg);
    }

    /// <summary>
    /// Logs a Debug message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Debug(object message, Exception exception)
    {
        Log(DEBUG + message, exception);
    }

    /// <summary>
    /// Logs a Debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Debug(object message)
    {
        Log(DEBUG + message);
    }

    /// <summary>
    /// Logs a Debug format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void DebugFormat(string format, params object[] args)
    {
        LogFormat(DEBUG + format, args);
    }

    /// <summary>
    /// Logs a Error message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Error(object message, Exception exception)
    {
        Log(ERROR + message, exception);
    }

    /// <summary>
    /// Logs a Error message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Error(object message)
    {
        Log(ERROR + message);
    }

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void ErrorFormat(string format, params object[] args)
    {
        LogFormat(ERROR + format, args);
    }

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Info(object message, Exception exception)
    {
        Log(message, exception);
    }

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Info(object message)
    {
        Log(message);
    }

    /// <summary>
    /// Logs a Warning message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Warn(object message, Exception exception)
    {
        Log(WARN + message, exception);
    }

    /// <summary>
    /// Logs a Warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Warn(object message)
    {
        Log(WARN + message);
    }

    /// <summary>
    /// Logs a Warning format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void WarnFormat(string format, params object[] args)
    {
        LogFormat(WARN + format, args);
    }
}