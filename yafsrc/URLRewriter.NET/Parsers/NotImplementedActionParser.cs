// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Summary description for ForbiddenActionParser.
	/// </summary>
	public sealed class NotImplementedActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NotImplementedActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name
		{
			get
			{
				return Constants.ElementNotImplemented;
			}
		}

		/// <summary>
		/// Whether the action allows nested actions.
		/// </summary>
		public override bool AllowsNestedActions
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Whether the action allows attributes.
		/// </summary>
		public override bool AllowsAttributes
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Parses the node.
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <param name="config">The rewriter configuration.</param>
		/// <returns>The parsed action, or null if no action parsed.</returns>
		public override IRewriteAction Parse(XmlNode node, object config)
		{
			return new NotImplementedAction();
		}
	}
}
