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
    /// Returns a 501 Not Implemented HTTP status code.
    /// </summary>
    public sealed class NotImplementedAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotImplementedAction()
            : base(HttpStatusCode.NotImplemented)
        {
        }
    }
}
