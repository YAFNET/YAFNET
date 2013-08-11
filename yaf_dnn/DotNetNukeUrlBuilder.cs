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

namespace YAF.DotNetNuke
{
  #region Using

    using System;
    using System.Text;
    using System.Web;

    using global::DotNetNuke.Common;

    using global::DotNetNuke.Entities.Portals;

    using global::DotNetNuke.Entities.Tabs;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

  /// <summary>
  /// The DotNetNuke url builder.
  /// </summary>
  public class DotNetNukeUrlBuilder : RewriteUrlBuilder
  {
    #region Public Methods

    /// <summary>
    /// The build url.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <returns>
    /// The new Url.
    /// </returns>
    public override string BuildUrl(string url)
    {
      var newUrl = new StringBuilder();

      PortalSettings curPortalSettings = PortalController.GetCurrentPortalSettings();

      TabInfo curTab = curPortalSettings.ActiveTab;

      if (!Config.EnableURLRewriting)
      {
        var scriptName = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];

        // escape & to &amp;
        url = url.Replace("&", "&amp;");

        return "{0}?tabid={1}&{2}".FormatWith(scriptName, curTab.TabID, url);
      }

     // try
        // {
        string oldUrl = Globals.NavigateURL(curTab.TabID);

        if (oldUrl.Contains("Default.aspx"))
        {
          oldUrl = oldUrl.Replace("Default.aspx", string.Empty);
        }

        // Fix For DNN 4
        if (oldUrl.Contains(".aspx"))
        {
          oldUrl = Globals.ResolveUrl("~/tabid/{0}/".FormatWith(curTab.TabID));
        }
          
        newUrl.Append(oldUrl);

        if (!newUrl.ToString().EndsWith("/"))
        {
            newUrl.Append("/");
        }

        var parser = new SimpleURLParameterParser(url);

        string useKey = string.Empty;

        switch (parser["g"])
        {
          case "topics":
            {
              useKey = "f";

              newUrl.AppendFormat("g/{2}/f/{0}/{1}", parser[useKey], this.GetForumName(parser[useKey].ToType<int>()), parser["g"]);
            }

            break;
          case "posts":
            {
              if (parser["t"].IsSet())
              {
                useKey = "t";

                  string topicName = this.GetTopicName(parser["t"].ToType<int>());

                  if (topicName.EndsWith("-"))
                  {
                      topicName = topicName.Remove(topicName.Length - 1, 1);
                  }

                newUrl.AppendFormat(
                  "g/{2}/t/{0}/{1}", parser["t"], topicName, parser["g"]);
              }
              else if (parser["m"].IsSet())
              {
                useKey = "m";

                  string topicName;

                  try
                  {
                      topicName = this.GetTopicNameFromMessage(parser["m"].ToType<int>());

                      if (topicName.EndsWith("-"))
                      {
                          topicName = topicName.Remove(topicName.Length - 1, 1);
                      }
                  }
                  catch (Exception)
                  {
                      topicName = parser["g"];
                  }

                newUrl.AppendFormat(
                  "g/{2}/m/{0}/{1}", 
                  parser["m"],
                  topicName, 
                  parser["g"]);
              }
            }

            break;
          case "profile":
            {
              useKey = "u";

              newUrl.AppendFormat(
                "g/{2}/u/{0}/{1}", parser[useKey], this.GetProfileName(parser[useKey].ToType<int>()), parser["g"]);
            }

            break;
          case "forum":
            {
              if (parser["c"].IsSet())
              {
                useKey = "c";

                newUrl.AppendFormat(
                  "g/{2}/c/{0}/{1}", parser[useKey], this.GetCategoryName(parser[useKey].ToType<int>()), parser["g"]);
              }
              else
              {
               newUrl.Append(YafContext.Current.Get<YafBoardSettings>().Name.Replace(" ", "-"));
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

        //newUrl.Append(".aspx");

        if (newUrl.Length >= 260)
        {
            newUrl.Remove(newUrl.ToString().LastIndexOf("/", StringComparison.Ordinal), newUrl.Length - newUrl.ToString().LastIndexOf("/", StringComparison.Ordinal));

            newUrl.Append("/Default.aspx");
        }

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
     /* }
      catch (Exception)
      {
        string sScriptName = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];

        return string.Format("{0}?tabid={1}&{2}", sScriptName, curTab.TabID, sUrl);
      }*/
      return newUrl.ToString();
    }
    #endregion
  }
}