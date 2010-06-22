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
namespace YAF.Classes
{
  #region Using

  using System.Web;

  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The dot net nuke url builder.
  /// </summary>
  public class DotNetNukeUrlBuilder : BaseUrlBuilder
  {
    #region Implemented Interfaces

    #region IUrlBuilder

    /// <summary>
    /// The build url.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <returns>
    /// The build url.
    /// </returns>
    public override string BuildUrl(string url)
    {
      // escape & to &amp;
      url = url.Replace("&", "&amp;");

      string scriptname = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
      string tabid = HttpContext.Current.Request.QueryString.GetFirstOrDefault("tabid");

      string builturl = tabid != null
                          ? string.Format("{0}?tabid={1}&{2}", scriptname, tabid, url)
                          : string.Format("{0}?{1}", scriptname, url);

      return builturl;
    }

    #endregion

    #endregion
  }
}