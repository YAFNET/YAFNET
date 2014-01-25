using System;
using System.Web;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Interface for rewriter error handlers.
	/// </summary>
	public interface IRewriteErrorHandler
	{
		/// <summary>
		/// Handles the error.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		void HandleError(HttpContext context);
	}
}
