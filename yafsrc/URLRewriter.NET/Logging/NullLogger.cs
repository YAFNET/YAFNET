using System;

namespace Intelligencia.UrlRewriter.Logging
{
	/// <summary>
	/// A logger which does nothing.
	/// </summary>
	public class NullLogger : IRewriteLogger
	{
		/// <summary>
		/// Writes a debug message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Debug(object message) {}

		/// <summary>
		/// Writes an informational message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Info(object message) {}

		/// <summary>
		/// Writes a warning message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Warn(object message) {}
		
		/// <summary>
		/// Writes an error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Error(object message) {}
		
		/// <summary>
		/// Writes an error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="exception">The exception</param>
		public void Error(object message, Exception exception) {}
		
		/// <summary>
		/// Writes a fatal error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="exception">The exception</param>
		public void Fatal(object message, Exception exception) {}
	}
}
