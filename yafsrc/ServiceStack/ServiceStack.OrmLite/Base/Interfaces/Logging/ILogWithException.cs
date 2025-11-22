namespace ServiceStack.Logging;

using System;

/// <summary>
/// Interface ILogWithException
/// Implements the <see cref="ServiceStack.Logging.ILog" />
/// </summary>
/// <seealso cref="ServiceStack.Logging.ILog" />
public interface ILogWithException : ILog
{
    /// <summary>
    /// Logs a Debug format message and exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void Debug(Exception exception, string format, params object[] args);

    /// <summary>
    /// Logs an Info format message and exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void Info(Exception exception, string format, params object[] args);

    /// <summary>
    /// Logs a Warn format message and exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void Warn(Exception exception, string format, params object[] args);

    /// <summary>
    /// Logs an Error format message and exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void Error(Exception exception, string format, params object[] args);
}

/// <summary>
/// Class ILogWithExceptionExtensions.
/// </summary>
public static class ILogWithExceptionExtensions
{
    /// <param name="logger">The logger</param>
    extension(ILog logger)
    {
        /// <summary>
        /// Logs a Debug format message and exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Debug(Exception exception, string format, params object[] args)
        {
            if (logger is ILogWithException exceptionLogger)
            {
                exceptionLogger.Debug(exception, format, args);
            }
            else
            {
                logger.Debug(string.Format(format, args), exception);
            }
        }

        /// <summary>
        /// Logs an Info format message and exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Info(Exception exception, string format, params object[] args)
        {
            if (logger is ILogWithException exceptionLogger)
            {
                exceptionLogger.Info(exception, format, args);
            }
            else
            {
                logger.Info(string.Format(format, args), exception);
            }
        }

        /// <summary>
        /// Logs a Warn format message and exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Warn(Exception exception, string format, params object[] args)
        {
            if (logger is ILogWithException exceptionLogger)
            {
                exceptionLogger.Warn(exception, format, args);
            }
            else
            {
                logger.Warn(string.Format(format, args), exception);
            }
        }

        /// <summary>
        /// Logs an Error format message and exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Error(Exception exception, string format, params object[] args)
        {
            if (logger is ILogWithException exceptionLogger)
            {
                exceptionLogger.Error(exception, format, args);
            }
            else
            {
                logger.Error(string.Format(format, args), exception);
            }
        }
    }
}