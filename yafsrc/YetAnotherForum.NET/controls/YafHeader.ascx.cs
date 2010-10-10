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

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The yaf header.
  /// </summary>
  public partial class YafHeader : BaseUserControl
  {
    #region Methods

    /// <summary>
    /// The get return url.
    /// </summary>
    /// <returns>
    /// The get return url.
    /// </returns>
    protected string GetReturnUrl()
    {
      string returnUrl = string.Empty;

      if (this.PageContext.ForumPageType != ForumPages.login)
      {
        returnUrl = HttpContext.Current.Server.UrlEncode(General.GetSafeRawUrl());
      }
      else
      {
        // see if there is already one since we are on the login page
        if (HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl").IsSet())
        {
          returnUrl =
            HttpContext.Current.Server.UrlEncode(
              General.GetSafeRawUrl(HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl")));
        }
      }

      return returnUrl;
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        doQuickSearch.Text = this.PageContext.Localization.GetText("SEARCH", "BTNSEARCH");

        searchInput.Attributes["onfocus"] = string.Format("if (this.value == '{0}') {{this.value = '';}}", this.PageContext.Localization.GetText("TOOLBAR", "SEARCHKEYWORD"));
        searchInput.Attributes["onblur"] = string.Format("if (this.value == '') {{this.value = '{0}';}}", this.PageContext.Localization.GetText("TOOLBAR", "SEARCHKEYWORD"));

         searchInput.Text = this.PageContext.Localization.GetText("TOOLBAR", "SEARCHKEYWORD");
    }

    /// <summary>
    /// Do Quick Search
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void QuickSearchClick(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(searchInput.Text)) return;

        YafBuildLink.Redirect(
            ForumPages.search,
            "search={0}",
            searchInput.Text);
    }

    #endregion
  }
}