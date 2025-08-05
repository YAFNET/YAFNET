// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="ActionParserFactory.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Configuration;

using System;
using System.Collections.Generic;

using YAF.UrlRewriter.Parsers;

/// <summary>
/// Factory for creating the action parsers.
/// </summary>
public class ActionParserFactory
{
    /// <summary>
    /// Adds a parser.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public void Add(IRewriteActionParser parser)
    {
        if (parser == null)
        {
            throw new ArgumentNullException(nameof(parser));
        }

        IList<IRewriteActionParser> list;

        if (this._parsers.TryGetValue(parser.Name, out var parser1))
        {
            list = parser1;
        }
        else
        {
            list = new List<IRewriteActionParser>();
            this._parsers.Add(parser.Name, list);
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
        return this._parsers.TryGetValue(verb, out var parser)
                   ? parser
                   : null;
    }

    private readonly IDictionary<string, IList<IRewriteActionParser>> _parsers = new Dictionary<string, IList<IRewriteActionParser>>();
}