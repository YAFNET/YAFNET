// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Collections;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
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
			AddParser((IRewriteActionParser)TypeHelper.Activate(parserType, null));
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
			if (_parsers.ContainsKey(parser.Name))
			{
				list = (ArrayList)_parsers[parser.Name];
			}
			else
			{
				list = new ArrayList();
				_parsers.Add(parser.Name, list);
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
			if (_parsers.ContainsKey(verb))
			{
				return (ArrayList)_parsers[verb];
			}
			else
			{
				return null;
			}
		}

		private Hashtable _parsers = new Hashtable();
	}
}
