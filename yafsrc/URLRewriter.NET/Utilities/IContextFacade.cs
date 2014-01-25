using System;
using System.Web;
using System.Collections.Specialized;

namespace Intelligencia.UrlRewriter.Utilities
{
    /// <summary>
    /// Map path delegate
    /// </summary>
    /// <param name="url">The url to map</param>
    /// <returns>The physical path.</returns>
	public delegate string MapPath(string url);

	/// <summary>
	/// Interface for a facade to the context.  Useful for plugging out the HttpContext object
	/// in unit tests.
	/// </summary>
	public interface IContextFacade
	{
		/// <summary>
		/// Retrieves the application path.
		/// </summary>
		/// <returns>The application path.</returns>
		string GetApplicationPath();

		/// <summary>
		/// Retrieves the raw url.
		/// </summary>
		/// <returns>The raw url.</returns>
		string GetRawUrl();

		/// <summary>
		/// Retrieves the current request url.
		/// </summary>
		/// <returns>The request url.</returns>
		Uri GetRequestUrl();

		/// <summary>
		/// Delegate to use for mapping paths.
		/// </summary>
		MapPath MapPath
		{
			get;
		}

		/// <summary>
		/// Sets the status code for the response.
		/// </summary>
		/// <param name="code">The status code.</param>
		void SetStatusCode(int code);

		/// <summary>
		/// Rewrites the request to the new url.
		/// </summary>
		/// <param name="url">The new url to rewrite to.</param>
		void RewritePath(string url);

		/// <summary>
		/// Sets the redirection location to the given url.
		/// </summary>
		/// <param name="url">The url of the redirection location.</param>
		void SetRedirectLocation(string url);

		/// <summary>
		/// Appends a header to the response.
		/// </summary>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		void AppendHeader(string name, string value);

		/// <summary>
		/// Adds a cookie to the response.
		/// </summary>
		/// <param name="cookie">The cookie to add.</param>
		void AppendCookie(HttpCookie cookie);

		/// <summary>
		/// Handles an error with the error handler.
		/// </summary>
		/// <param name="handler">The error handler to use.</param>
		void HandleError(IRewriteErrorHandler handler);

		/// <summary>
		/// Sets a context item.
		/// </summary>
		/// <param name="item">The item key</param>
		/// <param name="value">The item value</param>
		void SetItem(object item, object value);

		/// <summary>
		/// Retrieves a context item.
		/// </summary>
		/// <param name="item">The item key.</param>
		/// <returns>The item value.</returns>
		object GetItem(object item);

		/// <summary>
		/// Retrieves the HTTP method used by the request.
		/// </summary>
		/// <returns>The HTTP method.</returns>
		string GetHttpMethod();

		/// <summary>
		/// Gets a collection of server variables.
		/// </summary>
		/// <returns></returns>
		NameValueCollection GetServerVariables();

		/// <summary>
		/// Gets a collection of headers.
		/// </summary>
		/// <returns></returns>
		NameValueCollection GetHeaders();

		/// <summary>
		/// Gets a collection of cookies.
		/// </summary>
		/// <returns></returns>
		HttpCookieCollection GetCookies();
	}
}
