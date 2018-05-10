// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Conditions
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
            if (chainedCondition == null)
            {
                throw new ArgumentNullException("chainedCondition");
            }

            this._chainedCondition = chainedCondition;
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public bool IsMatch(RewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return !this._chainedCondition.IsMatch(context);
        }

        private IRewriteCondition _chainedCondition;
    }
}