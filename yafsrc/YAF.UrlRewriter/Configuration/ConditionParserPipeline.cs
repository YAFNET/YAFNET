// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Configuration;

using System.Collections.Generic;

using YAF.UrlRewriter.Parsers;

/// <summary>
/// Pipeline for creating the Condition parsers.
/// </summary>
public class ConditionParserPipeline : List<IRewriteConditionParser>;