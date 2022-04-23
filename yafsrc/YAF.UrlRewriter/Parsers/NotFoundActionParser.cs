// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Parsers;

using System.Xml;

using YAF.UrlRewriter.Actions;
using YAF.UrlRewriter.Configuration;
using YAF.UrlRewriter.Utilities;

/// <summary>
/// Parser for not found actions.
/// </summary>
public sealed class NotFoundActionParser : RewriteActionParserBase
{
    /// <summary>
    /// The name of the action.
    /// </summary>
    public override string Name => Constants.ElementNotFound;

    /// <summary>
    /// Whether the action allows nested actions.
    /// </summary>
    public override bool AllowsNestedActions => false;

    /// <summary>
    /// Whether the action allows attributes.
    /// </summary>
    public override bool AllowsAttributes => false;

    /// <summary>
    /// Parses the node.
    /// </summary>
    /// <param name="node">The node to parse.</param>
    /// <param name="config">The rewriter configuration.</param>
    /// <returns>The parsed action, or null if no action parsed.</returns>
    public override IRewriteAction Parse(XmlNode node, IRewriterConfiguration config)
    {
        return new NotFoundAction();
    }
}