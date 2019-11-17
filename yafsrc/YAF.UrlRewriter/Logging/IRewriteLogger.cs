// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Logging
{
    using System;

    /// <summary>
    /// Interface for logging info from the Rewriter.
    /// </summary>
    public interface IRewriteLogger
    {
        /// <summary>
        /// Writes a debug message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Debug(object message);

        /// <summary>
        /// Writes an informational message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Info(object message);

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Warn(object message);

        /// <summary>
        /// Writes an error.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Error(object message);

        /// <summary>
        /// Writes an error.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">The exception</param>
        void Error(object message, Exception exception);
    }
}
