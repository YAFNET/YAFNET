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
    /// Sets the StatusCode.
    /// </summary>
    public class SetStatusAction : IRewriteAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="statusCode">The status code to set.</param>
        public SetStatusAction(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        /// <summary>
        /// The status code.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        public virtual RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.StatusCode = StatusCode;

            return ((int)StatusCode >= 300)
                    ? RewriteProcessing.StopProcessing
                    : RewriteProcessing.ContinueProcessing;
        }

        private HttpStatusCode _statusCode;
    }
}
