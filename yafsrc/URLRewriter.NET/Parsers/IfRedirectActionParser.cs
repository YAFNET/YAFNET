// UrlRewriter - A .NET URL Rewriter module
// Version 1.7
//
// Copyright 2006 Intelligencia
// Copyright 2006 Seth Yates
// 

using System;
using System.Xml;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Parses the redirect portion of the IF and IFNOT elements.
	/// </summary>
	public sealed class IfRedirectActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public IfRedirectActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name
		{
			get
			{
				return String.Format(Constants.AttributeAction, Constants.ElementIf, Constants.ElementRedirect);
			}
		}

		/// <summary>
		/// Whether the action allows nested actions.
		/// </summary>
		public override bool AllowsNestedActions
		{
			get
			{
				return true;
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
			XmlNode redirectAttribute = node.Attributes.GetNamedItem(Constants.AttrRedirect);
			if (redirectAttribute != null)
			{
				bool permanent = true;
				XmlNode permanentNode = node.Attributes.GetNamedItem(Constants.AttrPermanent);
				if (permanentNode != null)
				{
					permanent = Convert.ToBoolean(permanentNode.Value);
				}

				return new RedirectAction(redirectAttribute.Value, permanent);
			}
			else
			{
				return null;
			}
		}
	}
}
