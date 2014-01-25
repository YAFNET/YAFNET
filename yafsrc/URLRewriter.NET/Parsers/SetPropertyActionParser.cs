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
	/// Action parser for the set-Property action.
	/// </summary>
	public sealed class SetPropertyActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SetPropertyActionParser()
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
            
            XmlNode propertyNameNode = node.Attributes.GetNamedItem(Constants.AttrProperty);
			if (propertyNameNode == null)
			{
				return null;
			}

			XmlNode propertyValueNode = node.Attributes.GetNamedItem(Constants.AttrValue);
			if (propertyValueNode == null)
			{
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrValue), node);
			}

			return new SetPropertyAction(propertyNameNode.Value, propertyValueNode.Value);
		}
	}
}
