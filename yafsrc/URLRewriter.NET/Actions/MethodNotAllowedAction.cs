// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Net;

namespace Intelligencia.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 405 Method Not Allowed HTTP status code.
    /// </summary>
    public sealed class MethodNotAllowedAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MethodNotAllowedAction()
            : base(HttpStatusCode.MethodNotAllowed)
        {
        }
    }
}
