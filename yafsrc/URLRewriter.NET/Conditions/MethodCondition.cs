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
	/// Matches on the current method.
	/// </summary>
	public sealed class MethodCondition : MatchCondition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="pattern"></param>
		public MethodCondition(string pattern) : base(GetMethodPattern(pattern))
		{
		}

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context">The rewriting context.</param>
		/// <returns>True if the condition is met.</returns>
		public override bool IsMatch(RewriteContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
			return Pattern.IsMatch(context.Method);
		}

		private static string GetMethodPattern(string method)
		{
			// Convert the GET,  POST, *   to ^GET|POST|.+$
			return String.Format("^{0}$", Regex.Replace(method, @"[^a-zA-Z,\*]+", "").Replace(",", "|").Replace("*", ".+"));
		}
	}
}
