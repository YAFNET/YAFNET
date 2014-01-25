// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Text.RegularExpressions;

namespace Intelligencia.UrlRewriter.Conditions
{
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
            _pattern = new Regex(pattern, RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// The pattern to match.
		/// </summary>
		public Regex Pattern
		{
			get
			{
				return _pattern;
			}
		}

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context">The rewriting context.</param>
		/// <returns>True if the condition is met.</returns>
		public abstract bool IsMatch(RewriteContext context);

		private Regex _pattern;
	}
}
