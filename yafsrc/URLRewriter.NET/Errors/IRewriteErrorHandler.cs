/* UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
*/

namespace Intelligencia.UrlRewriter
{
    using System.Web;

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