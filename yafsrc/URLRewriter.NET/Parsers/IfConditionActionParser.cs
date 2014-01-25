// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
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
	/// Parses the IF node.
	/// </summary>
	public class IfConditionActionParser : RewriteActionParserBase
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
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            RewriterConfiguration rewriterConfig = config as RewriterConfiguration;
            ConditionalAction rule = new ConditionalAction();

			// Process the conditions on the element.
			bool negative = (node.LocalName == Constants.ElementUnless);
			ParseConditions(node, rule.Conditions, negative, config);

			// Next, process the actions on the element.
            ReadActions(node, rule.Actions, rewriterConfig);

			return rule;
		}

        private static void ReadActions(XmlNode node, IList actions, RewriterConfiguration config)
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
								actions.Add(action);
							}
						}

						if (!parsed)
						{
                            throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNotAllowed, node.FirstChild.Name), node);
						}
					}
				}

				childNode = childNode.NextSibling;
			}
		}
	}
}
