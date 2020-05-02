// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions
{
    using System.Collections.Generic;

    using YAF.UrlRewriter.Conditions;
    using YAF.UrlRewriter.Extensions;

    /// <summary>
    /// Rewrites in-place.
    /// </summary>
    public sealed class RewriteAction : SetLocationAction, IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="location">The location to set.</param>
        /// <param name="processing">The processing directive.</param>
        public RewriteAction(string location, RewriteProcessing processing)
            : base(location)
        {
            this._processing = processing;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewrite context.</param>
        public override RewriteProcessing Execute(IRewriteContext context)
        {
            base.Execute(context);
            return this._processing;
        }

        /// <summary>
        /// Determines if the rewrite rule matches.
        /// </summary>
        /// <param name="context">The rewrite context.</param>
        /// <returns>True if the rule matches.</returns>
        public bool IsMatch(IRewriteContext context)
        {
             return this.Conditions.IsMatch(context);
        }

        /// <summary>
        /// Conditions that must hold for the rule to fire.
        /// </summary>
        public IList<IRewriteCondition> Conditions { get; } = new List<IRewriteCondition>();

        private readonly RewriteProcessing _processing;
    }
}
