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
    /// Matches on the current method.
    /// </summary>
    public sealed class MethodCondition : MatchCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pattern"></param>
        public MethodCondition(string pattern)
            : base(GetMethodPattern(pattern))
        {
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public override bool IsMatch(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return this.Pattern.IsMatch(context.HttpContext.HttpMethod);
        }

        private static string GetMethodPattern(string method)
        {
            // Convert the "GET,POST,*" pattern to a regex, e.g. "^GET|POST|.+$".
            return $"^{Regex.Replace(method, @"[^a-zA-Z,\*]+", string.Empty).Replace(",", "|").Replace("*", ".+")}$";
        }
    }
}
