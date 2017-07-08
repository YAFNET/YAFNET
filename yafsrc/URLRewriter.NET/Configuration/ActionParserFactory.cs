// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Configuration
{
    using System;
    using System.Collections;

    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// Factory for creating the action parsers.
    /// </summary>
    public class ActionParserFactory
    {
        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parserType">The parser type.</param>
        public void AddParser(string parserType)
        {
            this.AddParser((IRewriteActionParser)TypeHelper.Activate(parserType, null));
        }

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public void AddParser(IRewriteActionParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            ArrayList list;
            if (this._parsers.ContainsKey(parser.Name))
            {
                list = (ArrayList)this._parsers[parser.Name];
            }
            else
            {
                list = new ArrayList();
                this._parsers.Add(parser.Name, list);
            }

            list.Add(parser);
        }

        /// <summary>
        /// Returns a list of parsers for the given verb.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns>A list of parsers</returns>
        public IList GetParsers(string verb)
        {
            if (this._parsers.ContainsKey(verb))
            {
                return (ArrayList)this._parsers[verb];
            }

            return null;
        }

        private Hashtable _parsers = new Hashtable();
    }
}