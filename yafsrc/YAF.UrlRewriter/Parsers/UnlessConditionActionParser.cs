// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Parsers;

using YAF.UrlRewriter.Utilities;

/// <summary>
/// Parses the IFNOT node.
/// </summary>
public class UnlessConditionActionParser : IfConditionActionParser
{
    /// <summary>
    /// The name of the action.
    /// </summary>
    public override string Name => Constants.ElementUnless;
}