// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using YAF.UrlRewriter.Conditions;
    using YAF.UrlRewriter.Extensions;

    /// <summary>
    /// Redirect using 302 temporary redirect.
    /// </summary>
    public sealed class RedirectAction : SetLocationAction, IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="location">The location to set.</param>
        /// <param name="permanent">Whether the redirection is permanent.</param>
        public RedirectAction(string location, bool permanent)
            : base(location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            this._permanent = permanent;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        public override RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.Execute(context);

            context.StatusCode = this._permanent
                ? HttpStatusCode.Moved
                : HttpStatusCode.Found;

            return RewriteProcessing.StopProcessing;
        }

        /// <summary>
        /// Determines if the rewrite rule matches.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsMatch(IRewriteContext context)
        {
            return this.Conditions.IsMatch(context);
        }

        /// <summary>
        /// Conditions that must hold for the rule to fire.
        /// </summary>
        public IList<IRewriteCondition> Conditions { get; } = new List<IRewriteCondition>();

        private readonly bool _permanent;
    }
}
