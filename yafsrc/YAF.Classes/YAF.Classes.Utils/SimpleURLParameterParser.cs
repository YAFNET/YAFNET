/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Utils
{
  #region Using

  using System.Collections.Specialized;

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
    public string CreateQueryString(string[] excludeValues)
    {
      string queryString = string.Empty;
      bool bFirst = true;

      for (int i = 0; i < this._nameValues.Count; i++)
      {
        string key = this._nameValues.Keys[i].ToLower();
        string value = this._nameValues[i];
        if (!this.KeyInsideArray(excludeValues, key))
        {
          if (bFirst)
          {
            bFirst = false;
          }
          else
          {
            queryString += "&";
          }

          queryString += key + "=" + value;
        }
      }

      return queryString;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The key inside array.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The key inside array.
    /// </returns>
    private bool KeyInsideArray(string[] array, string key)
    {
      foreach (string tmp in array)
      {
        if (tmp.Equals(key))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// The parse url parameters.
    /// </summary>
    private void ParseURLParameters()
    {
      string urlTemp = this._urlParameters;
      int index;

      // get the url end anchor (#blah) if there is one...
      this._urlAnchor = string.Empty;
      index = urlTemp.LastIndexOf('#');

      if (index > 0)
      {
        // there's an anchor
        this._urlAnchor = urlTemp.Substring(index + 1);

        // remove the anchor from the url...
        urlTemp = urlTemp.Remove(index);
      }

      this._nameValues.Clear();
      string[] arrayPairs = urlTemp.Split(new[] { '&' });

      foreach (string tValue in arrayPairs)
      {
        if (tValue.Trim().Length > 0)
        {
          // parse...
          string[] nvalue = tValue.Trim().Split(new[] { '=' });
          if (nvalue.Length == 1)
          {
            this._nameValues.Add(nvalue[0], string.Empty);
          }
          else if (nvalue.Length > 1)
          {
            this._nameValues.Add(nvalue[0], nvalue[1]);
          }
        }
      }
    }

    #endregion
  }
}