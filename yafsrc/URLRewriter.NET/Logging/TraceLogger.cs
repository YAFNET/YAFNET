// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Diagnostics;

namespace Intelligencia.UrlRewriter.Logging
{
    /// <summary>
    /// A logger which writes to Trace, which may be read by a Trace Listener.
    /// </summary>
    public class TraceLogger : IRewriteLogger
    {
        /// <summary>
        /// Writes a debug message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Debug(object message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes an informational message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Info(object message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Warn(object message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes an error.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Error(object message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes an error.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">The exception</param>
        public void Error(object message, Exception exception)
        {
            Trace.WriteLine(message);
            if (exception != null)
            {
                Trace.WriteLine(String.Format("Exception: {0}\r\nError Message: {1}", exception.GetType(), exception.Message));
            }
        }

        /// <summary>
        /// Writes a fatal error.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">The exception</param>
        public void Fatal(object message, Exception exception)
        {
            Trace.WriteLine(message);
            if (exception != null)
            {
                Trace.WriteLine(String.Format("Exception: {0}\r\nError Message: {1}", exception.GetType(), exception.Message));
            }
        }
    }
}
