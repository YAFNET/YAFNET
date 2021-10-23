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
    using System.Configuration;
    using System.Xml;

    using YAF.UrlRewriter.Actions;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Parses the IF node.
    /// </summary>
    public class IfConditionActionParser : RewriteActionParserBase
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name => Constants.ElementIf;

        /// <summary>
        /// Whether the action allows nested actions.
        /// </summary>
        public override bool AllowsNestedActions => true;

        /// <summary>
        /// Whether the action allows attributes.
        /// </summary>
        public override bool AllowsAttributes => true;

        /// <summary>
        /// Parses the action.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <param name="config">The rewriter configuration.</param>
        /// <returns>The parsed action, null if no action parsed.</returns>
        public override IRewriteAction Parse(XmlNode node, IRewriterConfiguration config)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var rule = new ConditionalAction();

            // Process the conditions on the element.
            var negative = node.LocalName == Constants.ElementUnless;
            this.ParseConditions(node, rule.Conditions, negative, config);

            // Next, process the actions on the element.
            ReadActions(node, rule.Actions, config);

            return rule;
        }

        private static void ReadActions(XmlNode node, ICollection<IRewriteAction> actions, IRewriterConfiguration config)
        {
            var childNode = node.FirstChild;
            while (childNode != null)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    var parsers = config.ActionParserFactory.GetParsers(childNode.LocalName);
                    if (parsers != null)
                    {
                        var parsed = false;
                        foreach (var parser in parsers)
                        {
                            var action = parser.Parse(childNode, config);

                            if (action == null)
                            {
                                continue;
                            }

                            parsed = true;
                            actions.Add(action);
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
