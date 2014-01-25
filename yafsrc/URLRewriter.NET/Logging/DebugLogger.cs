using System;

namespace Intelligencia.UrlRewriter.Logging
{
	/// <summary>
	/// A logger which writes out to the Debug window.
	/// </summary>
	public class DebugLogger : IRewriteLogger
	{
		/// <summary>
		/// Writes a debug message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Debug(object message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
		
		/// <summary>
		/// Writes an informational message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Info(object message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
		
		/// <summary>
		/// Writes a warning message.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Warn(object message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
		
		/// <summary>
		/// Writes an error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public void Error(object message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
		
		/// <summary>
		/// Writes an error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="exception">The exception</param>
		public void Error(object message, Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
		
		/// <summary>
		/// Writes a fatal error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="exception">The exception</param>
		public void Fatal(object message, Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
	}
}
