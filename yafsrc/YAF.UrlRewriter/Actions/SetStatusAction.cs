// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions
{
    using System;
    using System.Net;

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
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// The status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

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

            context.StatusCode = this.StatusCode;

            return (int)this.StatusCode >= 300
                    ? RewriteProcessing.StopProcessing
                    : RewriteProcessing.ContinueProcessing;
        }
    }
}
