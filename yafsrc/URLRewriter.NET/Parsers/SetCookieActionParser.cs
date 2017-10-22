// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Actions;

namespace Intelligencia.UrlRewriter.Parsers
{
    /// <summary>
    /// Action parser for the set-cookie action.
    /// </summary>
    public sealed class SetCookieActionParser : RewriteActionParserBase
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name
        {
            get { return Constants.ElementSet; }
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

            string cookieName = node.GetOptionalAttribute(Constants.AttrCookie);
            if (String.IsNullOrEmpty(cookieName))
            {
                return null;
            }

            string cookieValue = node.GetRequiredAttribute(Constants.AttrValue, true);

            return new SetCookieAction(cookieName, cookieValue);
        }
    }
}
