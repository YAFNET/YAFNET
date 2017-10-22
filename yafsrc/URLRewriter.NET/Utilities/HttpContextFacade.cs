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
    /// A naive pass-through implementation of the IHttpContext facade that proxys calls to HttpContext.Current.
    /// Mock implementations would want to do something more interesting like implement checks that the actions
    /// were called.
    /// </summary>
    internal class HttpContextFacade : IHttpContext
    {
        /// <summary>
        /// Maps the given URL to the absolute local path.
        /// </summary>
        /// <param name="url">The URL to map.</param>
        /// <returns>The absolute local file path relating to the URL.</returns>
        public string MapPath(string url)
        {
            return HttpContext.Current.Server.MapPath(url);
        }

        /// <summary>
        /// Retrieves the application path.
        /// </summary>
        public string ApplicationPath
        {
            get { return HttpContext.Current.Request.ApplicationPath; }
        }

        /// <summary>
        /// Retrieves the raw URL.
        /// </summary>
        public string RawUrl
        {
            get { return HttpContext.Current.Request.RawUrl; }
        }

        /// <summary>
        /// Retrieves the current request URL.
        /// </summary>
        public Uri RequestUrl
        {
            get { return HttpContext.Current.Request.Url; }
        }

        /// <summary>
        /// Sets the status code for the response.
        /// </summary>
        /// <param name="code">The status code.</param>
        public void SetStatusCode(HttpStatusCode code)
        {
            HttpContext.Current.Response.StatusCode = (int)code;
        }

        /// <summary>
        /// Rewrites the request to the new URL.
        /// </summary>
        /// <param name="url">The new URL to rewrite to.</param>
        public void RewritePath(string url)
        {
            HttpContext.Current.RewritePath(url, false);
        }

        /// <summary>
        /// Sets the redirection location to the given URL.
        /// </summary>
        /// <param name="url">The URL of the redirection location.</param>
        public void SetRedirectLocation(string url)
        {
            HttpContext.Current.Response.RedirectLocation = url;
        }

        /// <summary>
        /// Adds a header to the response.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public void SetResponseHeader(string name, string value)
        {
            HttpContext.Current.Response.AppendHeader(name, value);
        }

        /// <summary>
        /// Adds a cookie to the response.
        /// </summary>
        /// <param name="cookie">The cookie to add.</param>
        public void SetResponseCookie(HttpCookie cookie)
        {
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Handles an error with the error handler.
        /// </summary>
        /// <param name="handler">The error handler to use.</param>
        public void HandleError(IRewriteErrorHandler handler)
        {
            handler.HandleError(HttpContext.Current);
        }

        /// <summary>
        /// The Items collection for the current request.
        /// </summary>
        public IDictionary Items
        {
            get { return HttpContext.Current.Items; }
        }

        /// <summary>
        /// Retrieves the HTTP method used by the request (GET, POST, HEAD, PUT, DELETE).
        /// </summary>
        public string HttpMethod
        {
            get { return HttpContext.Current.Request.HttpMethod; }
        }

        /// <summary>
        /// Gets a collection of server variables.
        /// </summary>
        public NameValueCollection ServerVariables
        {
            get { return HttpContext.Current.Request.ServerVariables; }
        }

        /// <summary>
        /// Gets a collection of the request headers.
        /// </summary>
        public NameValueCollection RequestHeaders
        {
            get { return HttpContext.Current.Request.Headers; }
        }

        /// <summary>
        /// Gets a collection of request cookies.
        /// </summary>
        public HttpCookieCollection RequestCookies
        {
            get { return HttpContext.Current.Request.Cookies; }
        }
    }
}
