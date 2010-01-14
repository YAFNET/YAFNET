/* Yet Another Forum.NET
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
using System;
using System.Collections.Generic;
using System.Web;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The query string id helper.
  /// </summary>
  public class QueryStringIDHelper
  {
    /// <summary>
    /// The _id dictionary.
    /// </summary>
    private Dictionary<string, long> _idDictionary = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringIDHelper"/> class. 
    /// False to ErrorOnInvalid
    /// </summary>
    /// <param name="idName">
    /// </param>
    public QueryStringIDHelper(string idName)
      : this(idName, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringIDHelper"/> class. 
    /// False on ErrorOnInvalid
    /// </summary>
    /// <param name="idNames">
    /// </param>
    public QueryStringIDHelper(string[] idNames)
      : this(idNames, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringIDHelper"/> class.
    /// </summary>
    /// <param name="idName">
    /// The id name.
    /// </param>
    /// <param name="errorOnInvalid">
    /// The error on invalid.
    /// </param>
    public QueryStringIDHelper(string idName, bool errorOnInvalid)
    {
      InitIDs(
        new[]
          {
            idName
          }, 
        new[]
          {
            errorOnInvalid
          });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringIDHelper"/> class.
    /// </summary>
    /// <param name="idNames">
    /// The id names.
    /// </param>
    /// <param name="errorOnInvalid">
    /// The error on invalid.
    /// </param>
    public QueryStringIDHelper(string[] idNames, bool errorOnInvalid)
    {
      var failInvalid = new bool[idNames.Length];

      for (int i = 0; i < failInvalid.Length; i++)
      {
        failInvalid[i] = errorOnInvalid;
      }

      InitIDs(idNames, failInvalid);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringIDHelper"/> class.
    /// </summary>
    /// <param name="idNames">
    /// The id names.
    /// </param>
    /// <param name="errorOnInvalid">
    /// The error on invalid.
    /// </param>
    public QueryStringIDHelper(string[] idNames, bool[] errorOnInvalid)
    {
      InitIDs(idNames, errorOnInvalid);
    }

    /// <summary>
    /// Gets Params.
    /// </summary>
    public Dictionary<string, long> Params
    {
      get
      {
        if (this._idDictionary == null)
        {
          this._idDictionary = new Dictionary<string, long>();
        }

        return this._idDictionary;
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="idName">
    /// The id name.
    /// </param>
    public long? this[string idName]
    {
      get
      {
        if (Params.ContainsKey(idName))
        {
          return Params[idName];
        }

        return null;
      }
    }

    /// <summary>
    /// The contains key.
    /// </summary>
    /// <param name="idName">
    /// The id name.
    /// </param>
    /// <returns>
    /// The contains key.
    /// </returns>
    public bool ContainsKey(string idName)
    {
      return Params.ContainsKey(idName);
    }

    /// <summary>
    /// The init i ds.
    /// </summary>
    /// <param name="idNames">
    /// The id names.
    /// </param>
    /// <param name="errorOnInvalid">
    /// The error on invalid.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    private void InitIDs(string[] idNames, bool[] errorOnInvalid)
    {
      if (idNames.Length != errorOnInvalid.Length)
      {
        throw new Exception("idNames and errorOnInvalid variables must be the same array length.");
      }

      for (int i = 0; i < idNames.Length; i++)
      {
        if (!Params.ContainsKey(idNames[i]))
        {
          long idConverted = -1;

          if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString[idNames[i]]) &&
              long.TryParse(HttpContext.Current.Request.QueryString[idNames[i]], out idConverted))
          {
            Params.Add(idNames[i], idConverted);
          }
          else
          {
            // fail, see if it should be valid...
            if (errorOnInvalid[i])
            {
              // redirect to invalid id information...
              YafBuildLink.Redirect(ForumPages.info, "i=6");
            }
          }
        }
      }
    }
  }
}