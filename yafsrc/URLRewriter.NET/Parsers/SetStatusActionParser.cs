// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Net;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Action parser for the set-status action.
	/// </summary>
	public sealed class SetStatusActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SetStatusActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name
		{
			get
			{
				return Constants.ElementSet;
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
				return true;
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
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            XmlNode statusCodeNode = node.Attributes.GetNamedItem(Constants.AttrStatus);
			if (statusCodeNode == null)
			{
				return null;
			}

			return new SetStatusAction((HttpStatusCode)Convert.ToInt32(statusCodeNode.Value));
		}
	}
}
