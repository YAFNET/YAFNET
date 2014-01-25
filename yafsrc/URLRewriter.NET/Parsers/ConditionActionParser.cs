// UrlRewriter - A .NET URL Rewriter module
// Version 1.7
//
// Copyright 2006 Intelligencia
// Copyright 2006 Seth Yates
// 

using System;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Conditions;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Parses the IF and IFNOT nodes.
	/// </summary>
	public sealed class ConditionActionParser : RewriteActionParserBase
	{
		/// <summary>
		/// The name of the action.
		/// </summary>
		public override string Name
		{
			get
			{
				return Constants.ElementIf;
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
		/// Parses the action.
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <param name="config">The rewriter configuration.</param>
		/// <returns>The parsed action, null if no action parsed.</returns>
		public override IRewriteAction Parse(XmlNode node, object config)
		{
			ConditionalAction rule = new ConditionalAction();

			// Process the conditions on the element.
			bool negative = (node.LocalName == Constants.ElementIfNot);
			ParseConditions(node, rule.Conditions, negative, config);

			// Process attribute-based actions.
			ReadAttributeActions(node, false, rule.Actions, (RewriterConfiguration)config);

			// Next, process the nested <if> and <ifnot> and non-final actions.
			ReadActions(node, false, rule.Actions, (RewriterConfiguration)config);

			// Finish off with any final actions.
			ReadAttributeActions(node, true, rule.Actions, (RewriterConfiguration)config);
			ReadActions(node, true, rule.Actions, (RewriterConfiguration)config);

			return rule;
		}

		private void ReadAttributeActions(XmlNode node, bool allowFinal, IList actions, RewriterConfiguration config)
		{
			int i = 0;
			while (i < node.Attributes.Count)
			{
				XmlNode attrNode = node.Attributes[i++];

				IList parsers = config.ActionParserFactory.GetParsers(String.Format(Constants.AttributeAction, node.LocalName, attrNode.LocalName));
				if (parsers != null)
				{
					foreach (IRewriteActionParser parser in parsers)
					{
						IRewriteAction action = parser.Parse(node, config);
						if (action != null)
						{
							if (action.Processing == RewriteProcessing.ContinueProcessing || allowFinal)
							{
								actions.Add(action);
								break;
							}
						}
					}
				}
			}
		}

		private void ReadActions(XmlNode node, bool allowFinal, IList actions, RewriterConfiguration config)
		{
			XmlNode childNode = node.FirstChild;
			while (childNode != null)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					IList parsers = config.ActionParserFactory.GetParsers(childNode.LocalName);
					if (parsers != null)
					{
						bool parsed = false;
						foreach (IRewriteActionParser parser in parsers)
						{
							IRewriteAction action = parser.Parse(childNode, config);
							if (action != null)
							{
								parsed = true;
								if ((action.Processing == RewriteProcessing.ContinueProcessing) ^ allowFinal)
								{
									actions.Add(action);
									break;
								}
							}
						}

						if (!parsed)
						{
							throw new ConfigurationException(MessageProvider.FormatString(Message.ElementNotAllowed, node.FirstChild.Name), node);
						}
					}
				}

				childNode = childNode.NextSibling;
			}
		}
	}
}
