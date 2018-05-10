// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Parsers
{
    using Intelligencia.UrlRewriter.Utilities;

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
}
