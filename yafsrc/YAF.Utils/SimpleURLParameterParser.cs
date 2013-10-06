/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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