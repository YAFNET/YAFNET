// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Text.RegularExpressions;

namespace Intelligencia.UrlRewriter.Conditions
{
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
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            _pattern = pattern;
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
                throw new ArgumentNullException("context");
            }

            Regex regex = GetRegex(context);

            Match match = regex.Match(context.Location);
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
            if (_regex == null)
            {
                lock (this)
                {
                    if (_regex == null)
                    {
                        _regex = new Regex(context.ResolveLocation(_pattern), RegexOptions.IgnoreCase);
                    }
                }
            }

            return _regex;
        }

        /// <summary>
        /// The pattern to match.
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
        }

        private string _pattern;
        private Regex _regex;
    }
}
