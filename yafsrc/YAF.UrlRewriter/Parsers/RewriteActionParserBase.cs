// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using YAF.UrlRewriter.Actions;
    using YAF.UrlRewriter.Conditions;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Base class for rewrite actions.
    /// </summary>
    public abstract class RewriteActionParserBase : IRewriteActionParser
    {
        /// <summary>
        /// Parses the action.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <param name="config">The rewriter configuration.</param>
        /// <returns>The parsed action, null if no action parsed.</returns>
        public abstract IRewriteAction Parse(XmlNode node, IRewriterConfiguration config);

        /// <summary>
        /// The name of the action.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Whether the action allows nested actions.
        /// </summary>
        public abstract bool AllowsNestedActions { get; }

        /// <summary>
        /// Whether the action allows attributes.
        /// </summary>
        public abstract bool AllowsAttributes { get; }

        /// <summary>
        /// Parses conditions from the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="conditions">Conditions list to add new conditions to.</param>
        /// <param name="negative">Whether the conditions should be negated.</param>
        /// <param name="config">Rewriter configuration</param>
        protected void ParseConditions(XmlNode node, IList<IRewriteCondition> conditions, bool negative, IRewriterConfiguration config)
        {
            if (config == null)
            {
                return;
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }

            // Parse attribute-based conditions.
            config.ConditionParserPipeline.ForEach(parser =>
            {
                var condition = parser.Parse(node);

                if (condition == null)
                {
                    return;
                }

                if (negative)
                {
                    condition = new NegativeCondition(condition);
                }

                conditions.Add(condition);
            });

            // Now, process the nested <and> conditions.
            var childNode = node.FirstChild;
            while (childNode != null)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    if (childNode.LocalName == Constants.ElementAnd)
                    {
                        this.ParseConditions(childNode, conditions, negative, config);

                        var childNode2 = childNode.NextSibling;
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
