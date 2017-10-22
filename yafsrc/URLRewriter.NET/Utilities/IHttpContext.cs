// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace Intelligencia.UrlRewriter.Utilities
{
    /// <summary>
    /// Interface for the HTTP context.
    /// Useful for plugging out the HttpContext.Current object in unit tests.
    /// </summary>
    public interface IHttpContext
    {
        /// <summary>
        /// Retrieves the application path.
        /// </summary>
        string ApplicationPath { get; }

        /// <summary>
        /// Retrieves the raw URL.
        /// </summary>
        string RawUrl { get; }

        /// <summary>
        /// Retrieves the current request URL.
        /// </summary>
        Uri RequestUrl { get; }

        /// <summary>
        /// Maps the given URL to the absolute local path.
        /// </summary>
        /// <param name="url">The URL to map.</param>
        /// <returns>The absolute local file path relating to the URL.</returns>
        string MapPath(string url);

        /// <summary>
        /// Sets the status code for the response.
        /// </summary>
        /// <param name="code">The status code.</param>
        void SetStatusCode(HttpStatusCode code);

        /// <summary>
        /// Rewrites the request to the new URL.
        /// </summary>
        /// <param name="url">The new URL to rewrite to.</param>
        void RewritePath(string url);

        /// <summary>
        /// Sets the redirection location to the given URL.
        /// </summary>
        /// <param name="url">The URL of the redirection location.</param>
        void SetRedirectLocation(string url);

        /// <summary>
        /// Adds a header to the response.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        void SetResponseHeader(string name, string value);

        /// <summary>
        /// Adds a cookie to the response.
        /// </summary>
        /// <param name="cookie">The cookie to add.</param>
        void SetResponseCookie(HttpCookie cookie);

        /// <summary>
        /// Handles an error with the error handler.
        /// </summary>
        /// <param name="handler">The error handler to use.</param>
        void HandleError(IRewriteErrorHandler handler);

        /// <summary>
        /// The Items collection for the current request.
        /// </summary>
        IDictionary Items { get; }

        /// <summary>
        /// Retrieves the HTTP method used by the request (GET, POST, HEAD, PUT, DELETE).
        /// </summary>
        string HttpMethod { get; }

        /// <summary>
        /// Gets a collection of server variables.
        /// </summary>
        NameValueCollection ServerVariables { get; }

        /// <summary>
        /// Gets a collection of request headers.
        /// </summary>
        NameValueCollection RequestHeaders { get; }

        /// <summary>
        /// Gets a collection of request cookies.
        /// </summary>
        HttpCookieCollection RequestCookies { get; }
    }
}
