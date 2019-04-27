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
    /// Performs a property match.
    /// </summary>
    public sealed class PropertyMatchCondition : MatchCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="pattern">The pattern</param>
        public PropertyMatchCondition(string propertyName, string pattern)
            : base(pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            this.PropertyName = propertyName ?? throw new ArgumentNullException("propertyName");
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string PropertyName { get; } = string.Empty;

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public override bool IsMatch(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var property = context.Properties[this.PropertyName];
            if (property != null)
            {
                var match = this.Pattern.Match(property);
                if (match.Success)
                {
                    context.LastMatch = match;
                }

                return match.Success;
            }

            return false;
        }
    }
}
