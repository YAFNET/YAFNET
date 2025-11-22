// ***********************************************************************
// <copyright file="ILog.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Logging;

using System;

/// <summary>
/// Logs a message in a running application
/// </summary>
public interface ILog
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is debug enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Logs a Debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Debug(object message);

    /// <summary>
    /// Logs a Debug message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Debug(object message, Exception exception);

    /// <summary>
    /// Logs a Debug format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    [JetBrains.Annotations.StringFormatMethod("format")]
    void DebugFormat(string format, params object[] args);

    /// <summary>
    /// Logs a Error message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Error(object message);

    /// <summary>
    /// Logs a Error message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Error(object message, Exception exception);

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    [JetBrains.Annotations.StringFormatMethod("format")]
    void ErrorFormat(string format, params object[] args);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    void Info(object message);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Info(object message, Exception exception);

    /// <summary>
    /// Logs a Warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Warn(object message);

    /// <summary>
    /// Logs a Warning message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Warn(object message, Exception exception);

    /// <summary>
    /// Logs a Warning format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    [JetBrains.Annotations.StringFormatMethod("format")]
    void WarnFormat(string format, params object[] args);


}/// <summary>
/// When implemented will log as TRACE otherwise as DEBUG
/// </summary>
public interface ILogTrace
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is trace enabled.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.
    /// </value>
    bool IsTraceEnabled { get; }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Trace(object message);

    /// <summary>
    /// Logs a trace message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Trace(object message, Exception exception);

    /// <summary>
    /// Logs a trace format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    [JetBrains.Annotations.StringFormatMethod("format")]
    void TraceFormat(string format, params object[] args);
}

/// <summary>
/// Class LogUtils.
/// </summary>
public static class LogUtils
{
    /// <param name="log">The log.</param>
    extension(ILog log)
    {
        /// <summary>
        /// Determines whether [is trace enabled] [the specified log].
        /// </summary>
        /// <returns><c>true</c> if [is trace enabled] [the specified log]; otherwise, <c>false</c>.</returns>
        public bool IsTraceEnabled()
        {
            return log is ILogTrace traceLog
                ? traceLog.IsTraceEnabled
                : log.IsDebugEnabled;
        }

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Trace(object message)
        {
            if (log is ILogTrace traceLog)
            {
                traceLog.Trace(message);
            }
            else
            {
                log.Debug(message);
            }
        }

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Trace(object message, Exception exception)
        {
            if (log is ILogTrace traceLog)
            {
                traceLog.Trace(message, exception);
            }
            else
            {
                log.Debug(message, exception);
            }
        }

        /// <summary>
        /// Traces the format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void TraceFormat(string format, params object[] args)
        {
            if (log is ILogTrace traceLog)
            {
                traceLog.TraceFormat(format, args);
            }
            else
            {
                log.DebugFormat(format, args);
            }
        }
    }
}