using System;
using System.Web;

namespace Intelligencia.UrlRewriter.Errors
{
	/// <summary>
	/// Summary description for DefaultErrorHandler.
	/// </summary>
	public class DefaultErrorHandler : IRewriteErrorHandler
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="url">Url of the error page.</param>
		public DefaultErrorHandler(string url)
		{
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            _url = url;
		}

		/// <summary>
		/// Handles the error by rewriting to the error page url.
		/// </summary>
		/// <param name="context">The context.</param>
		public void HandleError(HttpContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.Server.Execute(_url);
		}

		private string _url;
	}
}
