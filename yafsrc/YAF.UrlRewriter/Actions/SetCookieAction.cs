// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions
{
    using System;
    using System.Web;

    /// <summary>
    /// Action that sets a cookie.
    /// </summary>
    public class SetCookieAction : IRewriteAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cookieName">The cookie name.</param>
        /// <param name="cookieValue">The cookie value.</param>
        public SetCookieAction(string cookieName, string cookieValue)
        {
            this.Name = cookieName ?? throw new ArgumentNullException("cookieName");
            this.Value = cookieValue ?? throw new ArgumentNullException("cookieValue");
        }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewrite context.</param>
        public RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var cookie = new HttpCookie(this.Name, this.Value);
            context.ResponseCookies.Add(cookie);

            return RewriteProcessing.ContinueProcessing;
        }
    }
}
