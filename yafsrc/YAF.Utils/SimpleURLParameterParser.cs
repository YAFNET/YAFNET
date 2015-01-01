/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
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
    ///   The _name values.
    /// </summary>
    private readonly NameValueCollection _nameValues = new NameValueCollection();

    /// <summary>
    ///   The _url parameters.
    /// </summary>
    private readonly string _urlParameters = string.Empty;

    /// <summary>
    ///   The _url anchor.
    /// </summary>
    private string _urlAnchor = string.Empty;

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
      this._urlParameters = urlParameters;
      this.ParseURLParameters();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Anchor.
    /// </summary>
    public string Anchor
    {
      get
      {
        return this._urlAnchor;
      }
    }

    /// <summary>
    ///   Gets Count.
    /// </summary>
    public int Count
    {
      get
      {
        return this._nameValues.Count;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether HasAnchor.
    /// </summary>
    public bool HasAnchor
    {
      get
      {
        return this._urlAnchor != string.Empty;
      }
    }

    /// <summary>
    ///   Gets Parameters.
    /// </summary>
    public NameValueCollection Parameters
    {
      get
      {
        return this._nameValues;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name = "name">
    ///   The name.
    /// </param>
    public string this[string name]
    {
      get
      {
        return this._nameValues[name];
      }
    }

    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name = "index">
    ///   The index.
    /// </param>
    public string this[int index]
    {
      get
      {
        return this._nameValues[index];
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The create query string.
    /// </summary>
    /// <param name="excludeValues">
    /// The exclude values.
    /// </param>
    /// <returns>
    /// The create query string.
    /// </returns>
    [NotNull]
    public string CreateQueryString([NotNull] string[] excludeValues)
    {
      CodeContracts.VerifyNotNull(excludeValues, "excludeValues");

      var queryBuilder = new StringBuilder();

      for (int i = 0; i < this._nameValues.Count; i++)
      {
        string key = this._nameValues.Keys[i].ToLower();
        string value = this._nameValues[i];

        if (!excludeValues.Contains(key))
        {
          if (queryBuilder.Length > 0)
          {
            queryBuilder.Append("&");
          }

          queryBuilder.AppendFormat("{0}={1}", key, value);
        }
      }

      return queryBuilder.ToString();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The parse url parameters.
    /// </summary>
    private void ParseURLParameters()
    {
      string urlTemp = this._urlParameters;

      // get the url end anchor (#blah) if there is one...
      this._urlAnchor = string.Empty;
      int index = urlTemp.LastIndexOf('#');

      if (index > 0)
      {
        // there's an anchor
        this._urlAnchor = urlTemp.Substring(index + 1);

        // remove the anchor from the url...
        urlTemp = urlTemp.Remove(index);
      }

      this._nameValues.Clear();

      string[] arrayPairs = urlTemp.Split('&');

      foreach (var nvalue in from pair in arrayPairs where pair.IsSet() select pair.Trim().Split('='))
      {
        if (nvalue.Length == 1)
        {
          this._nameValues.Add(nvalue[0], string.Empty);
        }
        else
        {
          // split again for .NET v4
          var chunks = nvalue[1].Split(',');
          this._nameValues.Add(nvalue[0], chunks.FirstOrDefault() ?? string.Empty);
        }
      }
    }

    #endregion
  }
}