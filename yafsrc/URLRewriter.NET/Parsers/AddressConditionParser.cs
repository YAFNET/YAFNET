// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Parsers
{
    using System;
    using System.Xml;

    using Intelligencia.UrlRewriter.Conditions;
    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// Parser for address conditions.
    /// </summary>
    public sealed class AddressConditionParser : IRewriteConditionParser
    {
        /// <summary>
        /// Parses the condition.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <returns>The condition parsed, or null if nothing parsed.</returns>
        public IRewriteCondition Parse(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var addressAttr = node.Attributes.GetNamedItem(Constants.AttrAddress);
            if (addressAttr != null)
            {
                return new AddressCondition(addressAttr.Value);
            }

            return null;
        }
    }
}