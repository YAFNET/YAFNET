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
    /// Matches on the current URL.
    /// </summary>
    public sealed class UrlMatchCondition : IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pattern"></param>
        public UrlMatchCondition(string pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
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

            var regex = this.GetRegex(context);

            var match = regex.Match(context.Location);
            if (match.Success)
            {
                context.LastMatch = match;
            }

            return match.Success;
        }

        /// <summary>
        /// Gets regular expression to evaluate.
        /// </summary>
        private Regex GetRegex(IRewriteContext context)
        {
            // Use double-checked locking pattern to synchronise access to the regex.
            if (this._regex != null)
            {
                return this._regex;
            }

            lock (this)
            {
                this._regex ??= new Regex(context.ResolveLocation(this.Pattern), RegexOptions.IgnoreCase);
            }

            return this._regex;
        }

        /// <summary>
        /// The pattern to match.
        /// </summary>
        public string Pattern { get; }

        private Regex _regex;
    }
}
