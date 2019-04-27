// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Conditions
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base class for MatchConditions.
    /// </summary>
    public abstract class MatchCondition : IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pattern">Pattern to match.</param>
        protected MatchCondition(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            this.Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// The pattern to match.
        /// </summary>
        public Regex Pattern { get; }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public abstract bool IsMatch(IRewriteContext context);
    }
}
