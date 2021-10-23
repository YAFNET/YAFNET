// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Errors
{
    using System;
    using System.Web;

    /// <summary>
    /// The default error handler.
    /// </summary>
    public class DefaultErrorHandler : IRewriteErrorHandler
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">URL of the error page.</param>
        public DefaultErrorHandler(string url)
        {
            this._url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// Handles the error by rewriting to the error page URL.
        /// </summary>
        /// <param name="context">The context.</param>
        public void HandleError(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Server.Execute(this._url);
        }

        private readonly string _url;
    }
}
