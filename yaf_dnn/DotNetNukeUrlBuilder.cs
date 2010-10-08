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

using System.Text;

namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Web;

  using global::DotNetNuke.Common;
  using global::DotNetNuke.Entities.Portals;
  using global::DotNetNuke.Entities.Tabs;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The dotnetnuke url builder.
  /// </summary>
  public class DotNetNukeUrlBuilder : RewriteUrlBuilder
  {
    #region Public Methods

    /// <summary>
    /// The build url.
    /// </summary>
    /// <param name="sUrl">
    /// The url.
    /// </param>
    /// <returns>
    /// The build url.
    /// </returns>
    public override string BuildUrl(string sUrl)
    {
      StringBuilder newUrl = null;

      PortalSettings curPortalSettings = PortalController.GetCurrentPortalSettings();

      TabInfo curTab = curPortalSettings.ActiveTab;

      if (!Config.EnableURLRewriting)
      {
        string sScriptName = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];

        // escape & to &amp;
        sUrl = sUrl.Replace("&", "&amp;");

        return string.Format("{0}?tabid={1}&{2}", sScriptName, curTab.TabID, sUrl);
      }

      try
      {
        string oldUrl = Globals.NavigateURL(curTab.TabID);

        if (oldUrl.Contains("Default.aspx"))
        {
          oldUrl = oldUrl.Replace("Default.aspx", string.Empty);
        }

        // Fix For DNN 4
        if (oldUrl.Contains(".aspx"))
        {
          oldUrl = Globals.ResolveUrl(string.Format("~/tabid/{0}/", curTab.TabID));
        }
          
          newUrl.Append(oldUrl);

        var parser = new SimpleURLParameterParser(sUrl);

        string useKey = string.Empty;

        switch (parser["g"])
        {
          case "topics":
            {
              useKey = "f";

              newUrl.AppendFormat("g/{2}/f/{0}/{1}", parser[useKey], this.GetForumName(Convert.ToInt32(parser[useKey])), parser["g"]);
            }

            break;
          case "posts":
            {
              if (!String.IsNullOrEmpty(parser["t"]))
              {
                useKey = "t";

                newUrl.AppendFormat(
                  "g/{2}/t/{0}/{1}", parser["t"], this.GetTopicName(Convert.ToInt32(parser["t"])), parser["g"]);
              }
              else if (!String.IsNullOrEmpty(parser["m"]))
              {
                useKey = "m";

                newUrl.AppendFormat(
                  "g/{2}/m/{0}/{1}", 
                  parser["m"], 
                  this.GetTopicNameFromMessage(Convert.ToInt32(parser["m"])), 
                  parser["g"]);
              }
            }

            break;
          case "profile":
            {
              useKey = "u";

              newUrl.AppendFormat(
                "g/{2}/u/{0}/{1}", parser[useKey], this.GetProfileName(Convert.ToInt32(parser[useKey])), parser["g"]);
            }

            break;
          case "forum":
            {
              if (!String.IsNullOrEmpty(parser["c"]))
              {
                useKey = "c";

                newUrl.AppendFormat(
                  "g/{2}/c/{0}/{1}", parser[useKey], this.GetCategoryName(Convert.ToInt32(parser[useKey])), parser["g"]);
              }
              else
              {
               newUrl.Append(YafContext.Current.BoardSettings.Name.Replace(" ", "-"));
              }
            }

            break;
          case "activeusers":
            {
              useKey = "v";

              newUrl.AppendFormat("g/{1}/v/{0}/{1}", parser[useKey], parser["g"]);
            }

            break;
          case "admin_editforum":
            {
              useKey = "f";

              if (parser[useKey] != null)
              {
                  newUrl.AppendFormat("g/{1}/f/{0}/{1}", parser[useKey], parser["g"]);
              }
              else
              {
                  newUrl.AppendFormat("g/{0}/{0}", parser["g"]);
              }
            }

            break;

          default:
            {
                newUrl.AppendFormat("g/{0}/{0}", parser["g"]);
            }

            break;
        }

         newUrl.Append(".aspx");

        string restURL = parser.CreateQueryString(new[] { "g", useKey });

        // append to the url if there are additional (unsupported) parameters
        if (restURL.Length > 0)
        {
            newUrl.AppendFormat("?{0}", restURL);
        }

        // add anchor
        if (parser.HasAnchor)
        {
            newUrl.AppendFormat("#{0}", parser.Anchor);
        }

        // just make sure & is &amp; ...
       // sNewURL = sNewURL.Replace("&amp;", "&").Replace("&", "&amp;");
      }
      catch (Exception)
      {
        string sScriptName = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];

        return string.Format("{0}?tabid={1}&{2}", sScriptName, curTab.TabID, sUrl);
      }

      return newUrl.ToString();
    }

    #endregion
  }
}