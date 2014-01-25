// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Interface for conditions.  Note that Conditions must be thread-safe as there is a single
    /// instance of each condition.  This means that you must not make any changes to fields/properties
    /// on the condition once its created.
	/// </summary>
	public interface IRewriteCondition
	{
		/// <summary>
		/// Determines if the condition matches.
		/// </summary>
		/// <param name="context">The rewrite context.</param>
		/// <returns>True if the condition is met.</returns>
		bool IsMatch(RewriteContext context);
	}
}
