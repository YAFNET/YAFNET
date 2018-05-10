// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Configuration;
using System.Xml;

using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Action parser for the add-header action.
	/// </summary>
	public sealed class AddHeaderActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AddHeaderActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name => Constants.ElementAdd;

	    /// <summary>
		/// Whether the action allows nested actions.
		/// </summary>
		public override bool AllowsNestedActions => false;

	    /// <summary>
		/// Whether the action allows attributes.
		/// </summary>
		public override bool AllowsAttributes => true;

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

            var headerNameNode = node.Attributes.GetNamedItem(Constants.AttrHeader);
			if (headerNameNode == null)
			{
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrHeader), node);
			}

			var headerValueNode = node.Attributes.GetNamedItem(Constants.AttrValue);
			if (headerValueNode == null)
			{
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrValue), node);
			}

			return new AddHeaderAction(headerNameNode.Value, headerValueNode.Value);
		}
	}
}
