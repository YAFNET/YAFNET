/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

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
    /// The url.
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
    /// Do Logout Dialog
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LogOutClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      var notification = (DialogBox)this.PageContext.CurrentForumPage.Notification;

      notification.Show(
        this.GetText("TOOLBAR", "LOGOUT_QUESTION"), 
        "Logout?", 
        DialogBox.DialogIcon.Question, 
        new DialogBox.DialogButton
          {
            Text = "Yes", 
            CssClass = "StandardButton", 
            ForumPageLink = new DialogBox.ForumLink { ForumPage = ForumPages.logout }
          }, 
        new DialogBox.DialogButton { Text = "No", CssClass = "StandardButton" });
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      var searchIcon = this.PageContext.Get<ITheme>().GetItem("ICONS", "SEARCH");

      if (!string.IsNullOrEmpty(searchIcon))
      {
        this.doQuickSearch.Text = @"<img alt=""{1}"" title=""{1}"" src=""{0}"" /> {1}".FormatWith(
          searchIcon, this.GetText("SEARCH", "BTNSEARCH"));
      }
      else
      {
        this.doQuickSearch.Text = this.GetText("SEARCH", "BTNSEARCH");
      }

      this.searchInput.Attributes["onkeydown"] =
        "if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{0}').click();return false;}}}} else {{return true}}; "
          .FormatWith(this.doQuickSearch.ClientID);
      this.searchInput.Attributes["onfocus"] =
        "if (this.value == '{0}') {{this.value = '';}}".FormatWith(
          this.GetText("TOOLBAR", "SEARCHKEYWORD"));
      this.searchInput.Attributes["onblur"] =
        "if (this.value == '') {{this.value = '{0}';}}".FormatWith(
          this.GetText("TOOLBAR", "SEARCHKEYWORD"));

      this.searchInput.Text = this.GetText("TOOLBAR", "SEARCHKEYWORD");

      base.OnPreRender(e);
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
    protected void QuickSearchClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (string.IsNullOrEmpty(this.searchInput.Text))
      {
        return;
      }

      YafBuildLink.Redirect(ForumPages.search, "search={0}", this.searchInput.Text);
    }

    #endregion
  }
}