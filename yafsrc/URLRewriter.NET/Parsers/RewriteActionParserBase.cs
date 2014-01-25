using System;
using System.Xml;
using System.Collections;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Conditions;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Summary description for RewriteActionParserBase.
	/// </summary>
	public abstract class RewriteActionParserBase : IRewriteActionParser
	{
		/// <summary>
		/// Parses the action.
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <param name="config">The rewriter configuration.</param>
		/// <returns>The parsed action, null if no action parsed.</returns>
		public abstract IRewriteAction Parse(XmlNode node, object config);
		
		/// <summary>
		/// The name of the action.
		/// </summary>
		public abstract string Name
		{
			get;
		}
		
		/// <summary>
		/// Whether the action allows nested actions.
		/// </summary>
		public abstract bool AllowsNestedActions
		{
			get;
		}

		/// <summary>
		/// Whether the action allows attributes.
		/// </summary>
		public abstract bool AllowsAttributes
		{
			get;
		}

		/// <summary>
		/// Parses conditions from the node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="conditions">Conditions list to add new conditions to.</param>
		/// <param name="negative">Whether the conditions should be negated.</param>
		/// <param name="config">Rewriter configuration</param>
		protected void ParseConditions(XmlNode node, IList conditions, bool negative, object config)
		{
            if (config == null)
            {
                return;
            }
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (conditions == null)
            {
                throw new ArgumentNullException("conditions");
            }

            RewriterConfiguration rewriterConfig = config as RewriterConfiguration;

			// Parse attribute-based conditions.
            foreach (IRewriteConditionParser parser in rewriterConfig.ConditionParserPipeline)
			{
				IRewriteCondition condition = parser.Parse(node);
				if (condition != null)
				{
                    if (negative)
                    {
                        condition = new NegativeCondition(condition);
                    }

					conditions.Add(condition);
				}
			}

			// Now, process the nested <and> conditions.
			XmlNode childNode = node.FirstChild;
			while (childNode != null)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					if (childNode.LocalName == Constants.ElementAnd)
					{
						ParseConditions(childNode, conditions, negative, config);

						XmlNode childNode2 = childNode.NextSibling;
						node.RemoveChild(childNode);
						childNode = childNode2;
						continue;
					}
				}

				childNode = childNode.NextSibling;
			}
		}
	}
}
