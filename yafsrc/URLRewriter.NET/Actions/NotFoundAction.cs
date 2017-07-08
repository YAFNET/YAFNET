// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Actions
{
    using System.Net;

    /// <summary>
    /// Returns a 404 Not Found HTTP status code.
    /// </summary>
    public sealed class NotFoundAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotFoundAction()
            : base(HttpStatusCode.NotFound)
        {
        }
    }
}