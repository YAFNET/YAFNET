// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
//

namespace YAF.UrlRewriter.Parsers
{
    using System;
    using System.Xml;

    using YAF.UrlRewriter.Conditions;
    using YAF.UrlRewriter.Extensions;
    using YAF.UrlRewriter.Utilities;

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
                throw new ArgumentNullException(nameof(node));
            }

            var address = node.GetOptionalAttribute(Constants.AttrAddress);

            return address == null ? null : new AddressCondition(address);
        }
    }
}
