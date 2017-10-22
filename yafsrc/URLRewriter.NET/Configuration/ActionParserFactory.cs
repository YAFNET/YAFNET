// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
    /// <summary>
    /// Factory for creating the action parsers.
    /// </summary>
    public class ActionParserFactory
    {
        /*
        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parserType">The parser type.</param>
        public void Add(string parserType)
        {
            Add((IRewriteActionParser)TypeHelper.Activate(parserType, null));
        }
         */

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public void Add(IRewriteActionParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            IList<IRewriteActionParser> list;

            if (_parsers.ContainsKey(parser.Name))
            {
                list = _parsers[parser.Name];
            }
            else
            {
                list = new List<IRewriteActionParser>();
                _parsers.Add(parser.Name, list);
            }

            list.Add(parser);
        }

        /// <summary>
        /// Returns a list of parsers for the given verb.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns>A list of parsers</returns>
        public IList<IRewriteActionParser> GetParsers(string verb)
        {
            return (_parsers.ContainsKey(verb))
                    ? _parsers[verb]
                    : null;
        }

        private IDictionary<string, IList<IRewriteActionParser>> _parsers = new Dictionary<string, IList<IRewriteActionParser>>();
    }
}
