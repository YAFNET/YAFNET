// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Action parser for the set-cookie action.
	/// </summary>
	public sealed class SetCookieActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SetCookieActionParser()
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
            
            XmlNode cookieNameNode = node.Attributes.GetNamedItem(Constants.AttrCookie);
			if (cookieNameNode == null)
			{
				return null;
			}

			XmlNode cookieValueNode = node.Attributes.GetNamedItem(Constants.AttrValue);
			if (cookieValueNode == null)
			{
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrValue), node);
			}

			return new SetCookieAction(cookieNameNode.Value, cookieValueNode.Value);
		}
	}
}
