/* UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
*/

namespace Intelligencia.UrlRewriter.Errors
{
    using System;
    using System.Web;

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

            this._url = url;
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

            context.Server.Execute(this._url);
        }

        private string _url;
    }
}