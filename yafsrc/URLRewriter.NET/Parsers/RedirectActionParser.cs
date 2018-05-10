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
	/// Summary description for RedirectActionParser.
	/// </summary>
	public sealed class RedirectActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RedirectActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name => Constants.ElementRedirect;

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
            
            var toNode = node.Attributes.GetNamedItem(Constants.AttrTo);
			if (toNode == null)
			{
				throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrTo), node);
			}

			var permanent = true;
			var permanentNode = node.Attributes.GetNamedItem(Constants.AttrPermanent);
			if (permanentNode != null)
			{
				permanent = Convert.ToBoolean(permanentNode.Value);
			}

			var action = new RedirectAction(toNode.Value, permanent);
		    this.ParseConditions(node, action.Conditions, false, config);
			return action;
		}
	}
}
