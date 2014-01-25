// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Interface defining a parser which parses an XML node and returns the correct
	/// IRewriteAction instance based on the node.
	/// </summary>
	public interface IRewriteActionParser
	{
		/// <summary>
		/// Parses the node if possible.  The parser may be called on an action node
		/// that it cannot parse, if it is registered on a common verb
		/// which is shared by several action parsers (e.g., set).
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <param name="config">The rewriter configuration.</param>
		/// <returns>The action parsed.  If the parser could not parse the node,
		/// it <strong>must</strong> return null.</returns>
		IRewriteAction Parse(XmlNode node, object config);

		/// <summary>
		/// The name of the action.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Whether the action allows nested actions.
		/// </summary>
		bool AllowsNestedActions
		{
			get;
		}

		/// <summary>
		/// Whether the action allows attributes.
		/// </summary>
		bool AllowsAttributes
		{
			get;
		}
	}
}
