// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Parsers
{
    using System;
    using System.Configuration;
    using System.Xml;

    using YAF.UrlRewriter.Actions;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Extensions;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Parser for rewrite actions.
    /// </summary>
    public sealed class RewriteActionParser : RewriteActionParserBase
    {
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
        public override IRewriteAction Parse(XmlNode node, IRewriterConfiguration config)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            var to = node.GetRequiredAttribute(Constants.AttrTo, true);

            var processing = this.ParseProcessing(node);

            var action = new RewriteAction(to, processing);
            this.ParseConditions(node, action.Conditions, false, config);

            return action;
        }

        private RewriteProcessing ParseProcessing(XmlNode node)
        {
            var processing = node.GetOptionalAttribute(Constants.AttrProcessing);
            if (processing == null)
            {
                // Default to ContinueProcessing if processing attribute is missing.
                return RewriteProcessing.ContinueProcessing;
            }

            switch (processing)
            {
                case Constants.AttrValueRestart:
                    return RewriteProcessing.RestartProcessing;

                case Constants.AttrValueStop: 
                    return RewriteProcessing.StopProcessing;

                case Constants.AttrValueContinue:
                    return RewriteProcessing.ContinueProcessing;

                default:
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ValueOfProcessingAttribute, processing, Constants.AttrValueContinue, Constants.AttrValueRestart, Constants.AttrValueStop), node);
            }
        }
    }
}
