// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Conditions
{
    using System;

    /// <summary>
    /// Performs a negation of the given conditions.
    /// </summary>
    public sealed class NegativeCondition : IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="chainedCondition"></param>
        public NegativeCondition(IRewriteCondition chainedCondition)
        {
            this._chainedCondition = chainedCondition ?? throw new ArgumentNullException(nameof(chainedCondition));
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public bool IsMatch(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return !this._chainedCondition.IsMatch(context);
        }

        private readonly IRewriteCondition _chainedCondition;
    }
}
