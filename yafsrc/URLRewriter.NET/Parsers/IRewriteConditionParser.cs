// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using Intelligencia.UrlRewriter.Conditions;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Interface defining a parser which parses an XML node and returns the correct
	/// IRewriteCondition instance based on the node.
	/// </summary>
	public interface IRewriteConditionParser
	{
		/// <summary>
		/// Parses the node if possible.  The parser may be called on a condition
		/// that it cannot parse, if it is registered on a common verb
		/// which is shared by several condition parsers (e.g., and).
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <returns>The condition parsed.  If the parser could not parse the node,
		/// it <strong>must</strong> return null.</returns>
		IRewriteCondition Parse(XmlNode node);
	}
}
