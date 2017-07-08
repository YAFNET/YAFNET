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
	/// Summary description for RewriteActionParser.
	/// </summary>
	public sealed class RewriteActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RewriteActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name => Constants.ElementRewrite;

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

            XmlNode toNode = node.Attributes[Constants.AttrTo];
			if (toNode.Value == null)
			{
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrTo), node);
			}

			XmlNode processingNode = node.Attributes[Constants.AttrProcessing];

			var processing = RewriteProcessing.ContinueProcessing;
			if (processingNode != null)
			{
				if (processingNode.Value == Constants.AttrValueRestart)
				{
					processing = RewriteProcessing.RestartProcessing;
				}
				else if (processingNode.Value == Constants.AttrValueStop)
				{
					processing = RewriteProcessing.StopProcessing;
				}
				else if (processingNode.Value != Constants.AttrValueContinue)
				{
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ValueOfProcessingAttribute, processingNode.Value, Constants.AttrValueContinue, Constants.AttrValueRestart, Constants.AttrValueStop), node);
				}
			}

			var action = new RewriteAction(toNode.Value, processing);
		    this.ParseConditions(node, action.Conditions, false, config);
			return action;
		}
	}
}
