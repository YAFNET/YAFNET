/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  #endregion

  /// <summary>
  /// The control extensions.
  /// </summary>
  public static class ControlExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public static string GetExtendedID(this Control currentControl, string prefix)
    {
      string createdID = null;

      if (currentControl.ID.IsSet())
      {
        createdID = currentControl.ID + "_";
      }

      if (prefix.IsSet())
      {
        createdID += prefix;
      }
      else
      {
        createdID += Guid.NewGuid().ToString().Substring(0, 5);
      }

      return createdID;
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public static string GetUniqueID(this Control currentControl, string prefix)
    {
      if (prefix.IsSet())
      {
        return prefix + Guid.NewGuid().ToString().Substring(0, 5);
      }
      else
      {
        return Guid.NewGuid().ToString().Substring(0, 10);
      }
    }

    /// <summary>
    /// The html encode.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The html encode.
    /// </returns>
    public static string HtmlEncode(this Control currentControl, object data)
    {
      return HttpContext.Current.Server.HtmlEncode(data.ToString());
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    public static YafContext PageContext(this Control currentControl)
    {
      if (currentControl.Site != null && currentControl.Site.DesignMode)
      {
        // design-time, return null...
        return null;
      }

      return YafContext.Current;
    }

    #endregion
  }
}