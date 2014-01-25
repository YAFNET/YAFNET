// UrlRewriter - A .NET URL Rewriter module
// Version 1.7
//
// Copyright 2006 Intelligencia
// Copyright 2006 Seth Yates
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
	/// Parses the rewrite portion of the IF and IFNOT elements.
	/// </summary>
	public sealed class IfRewriteActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public IfRewriteActionParser()
		{
		}

		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name
		{
			get
			{
				return String.Format(Constants.AttributeAction, Constants.ElementIf, Constants.ElementRewrite);
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
			XmlNode rewriteAttribute = node.Attributes.GetNamedItem(Constants.AttrRewrite);
			if (rewriteAttribute != null)
			{
				RewriteProcessing processing = RewriteProcessing.ContinueProcessing;

				XmlNode processingNode = node.Attributes.GetNamedItem(Constants.AttrProcessing);
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
						throw new ConfigurationException(MessageProvider.FormatString(Message.ValueOfProcessingAttribute, processingNode.Value, Constants.AttrValueContinue, Constants.AttrValueRestart, Constants.AttrValueStop), node);
					}
				}

				return new RewriteAction(rewriteAttribute.Value, processing);
			}
			else
			{
				return null;
			}
		}
	}
}
