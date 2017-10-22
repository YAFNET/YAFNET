// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections;
using System.Collections.Generic;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
    /// <summary>
    /// Pipeline for creating the Condition parsers.
    /// </summary>
    public class ConditionParserPipeline : List<IRewriteConditionParser>
    {
        /*
        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parserType">The parser type.</param>
        public void AddParser(string parserType)
        {
            AddParser((IRewriteConditionParser)TypeHelper.Activate(parserType, null));
        }

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public void AddParser(IRewriteConditionParser parser)
        {
            Add(parser);
        }
         */
    }
}
