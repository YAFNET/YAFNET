// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Parsers
{
    /// <summary>
    /// Parser for rewrite actions.
    /// </summary>
    public sealed class RewriteActionParser : RewriteActionParserBase
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name
        {
            get { return Constants.ElementRewrite; }
        }

        /// <summary>
        /// Whether the action allows nested actions.
        /// </summary>
        public override bool AllowsNestedActions
        {
            get { return false; }
        }

        /// <summary>
        /// Whether the action allows attributes.
        /// </summary>
        public override bool AllowsAttributes
        {
            get { return true; }
        }

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

            string to = node.GetRequiredAttribute(Constants.AttrTo, true);

            RewriteProcessing processing = ParseProcessing(node);

            RewriteAction action = new RewriteAction(to, processing);
            ParseConditions(node, action.Conditions, false, config);

            return action;
        }

        private RewriteProcessing ParseProcessing(XmlNode node)
        {
            string processing = node.GetOptionalAttribute(Constants.AttrProcessing);
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
