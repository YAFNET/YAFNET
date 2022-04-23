/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Utilities;

#region Using

using System.Collections.Specialized;
using System.Linq;
using System.Text;

using YAF.Types;
using YAF.Types.Extensions;

#endregion

/// <summary>
/// Helps parse URLs
/// </summary>
public class SimpleURLParameterParser
{
    #region Constants and Fields

    /// <summary>
    ///   The _url parameters.
    /// </summary>
    private readonly string urlParameters;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleURLParameterParser"/> class.
    /// </summary>
    /// <param name="urlParameters">
    /// The url parameters.
    /// </param>
    public SimpleURLParameterParser(string urlParameters)
    {
        this.urlParameters = urlParameters;
        this.ParseURLParameters();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Anchor.
    /// </summary>
    public string Anchor { get; private set; } = string.Empty;

    /// <summary>
    ///   Gets Count.
    /// </summary>
    public int Count => this.Parameters.Count;

    /// <summary>
    ///   Gets a value indicating whether HasAnchor.
    /// </summary>
    public bool HasAnchor => this.Anchor != string.Empty;

    /// <summary>
    ///   Gets Parameters.
    /// </summary>
    public NameValueCollection Parameters { get; } = new();

    #endregion

    #region Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <value>
    /// The <see cref="System.string"/>.
    /// </value>
    /// <param name="name">The name.</param>
    public string this[string name] => this.Parameters[name];

    /// <summary>
    /// The this.
    /// </summary>
    /// <value>
    /// The <see cref="System.String"/>.
    /// </value>
    /// <param name="index">The index.</param>
    public string this[int index] => this.Parameters[index];

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates the query string.
    /// </summary>
    /// <param name="excludeValues">The exclude values.</param>
    /// <returns>
    /// Returns the created query string.
    /// </returns>
    [NotNull]
    public string CreateQueryString([NotNull] string[] excludeValues)
    {
        CodeContracts.VerifyNotNull(excludeValues);

        var queryBuilder = new StringBuilder();

        this.Parameters.Cast<string>().Where(k => !excludeValues.Contains(k)).ForEach(
            key =>
                {
                    var value = this.Parameters[key];

                    if (queryBuilder.Length > 0)
                    {
                        queryBuilder.Append("&");
                    }

                    queryBuilder.AppendFormat("{0}={1}", key, value);
                });

        return queryBuilder.ToString();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Parses the URL parameters.
    /// </summary>
    private void ParseURLParameters()
    {
        var urlTemp = this.urlParameters;

        // get the URL end anchor (#blah) if there is one...
        this.Anchor = string.Empty;
        var index = urlTemp.LastIndexOf('#');

        if (index > 0)
        {
            // there's an anchor
            this.Anchor = urlTemp.Substring(index + 1);

            // remove the anchor from the URL...
            urlTemp = urlTemp.Remove(index);
        }

        this.Parameters.Clear();

        var arrayPairs = urlTemp.Split('&');

        (from pair in arrayPairs where pair.IsSet() select pair.Trim().Split('=')).ForEach(nvalue =>
            {
                if (nvalue.Length == 1)
                {
                    this.Parameters.Add(nvalue[0], string.Empty);
                }
                else
                {
                    // split again for .NET v4
                    var chunks = nvalue[1].Split(',');
                    this.Parameters.Add(nvalue[0], chunks.FirstOrDefault() ?? string.Empty);
                }
            });
    }

    #endregion
}