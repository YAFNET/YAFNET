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
  using System.Text;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for Header.
  /// </summary>
  public class Header : BaseControl, IYafHeader
  {
    #region Constants and Fields

    /// <summary>
    /// The _refresh time.
    /// </summary>
    private int _refreshTime = 10;

    /// <summary>
    /// The _refresh url.
    /// </summary>
    private string _refreshURL = null;

    /// <summary>
    /// The _simple render.
    /// </summary>
    private bool _simpleRender = false;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets RefreshTime.
    /// </summary>
    public int RefreshTime
    {
      get
      {
        return this._refreshTime;
      }

      set
      {
        this._refreshTime = value;
      }
    }

    /// <summary>
    /// Gets or sets RefreshURL.
    /// </summary>
    public string RefreshURL
    {
      get
      {
        return this._refreshURL;
      }

      set
      {
        this._refreshURL = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether SimpleRender.
    /// </summary>
    public bool SimpleRender
    {
      get
      {
        return this._simpleRender;
      }

      set
      {
        this._simpleRender = value;
      }
    }

    /// <summary>
    /// Gets ThisControl.
    /// </summary>
    public Control ThisControl
    {
      get
      {
        return this;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this.RefreshURL = null;
      this.RefreshTime = 10;
    }

    #endregion

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
        if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
        {
          returnUrl =
            HttpContext.Current.Server.UrlEncode(
              General.GetSafeRawUrl(HttpContext.Current.Request.QueryString["ReturnUrl"]));
        }
      }

      return returnUrl;
    }

    /// <summary>
    /// Renders the header.
    /// </summary>
    /// <param name="writer">
    /// The HtmlTextWriter that we are using.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
      if (!this._simpleRender)
      {
        this.RenderRegular(ref writer);
      }
    }

    /// <summary>
    /// The render regular.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderRegular(ref HtmlTextWriter writer)
    {
      // BEGIN HEADER
      var buildHeader = new StringBuilder();

      // get the theme header -- usually used for javascript
      string themeHeader = this.PageContext.Theme.GetItem("THEME", "HEADER", string.Empty);

      if (!String.IsNullOrEmpty(themeHeader))
      {
        buildHeader.Append(themeHeader);
      }

      buildHeader.AppendFormat(
        @"<table width=""100%"" cellspacing=""0"" class=""content"" cellpadding=""0"" id=""yafheader""><tr>");

      MembershipUser user = UserMembershipHelper.GetUser();

      if (user != null)
      {
        string displayName = this.PageContext.CurrentUserData.DisplayName;
        buildHeader.AppendFormat(
          @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", 
          String.Format(
            this.PageContext.Localization.GetText("TOOLBAR", "LOGGED_IN_AS") + " ",
            HttpContext.Current.Server.HtmlEncode(!string.IsNullOrEmpty(displayName) ? displayName : this.PageContext.PageUserName)));
        buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");

        if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.AllowPrivateMessages)
        {
          if (this.PageContext.UnreadPrivate > 0)
          {
            string unreadText = String.Format(
              this.PageContext.Localization.GetText("TOOLBAR", "NEWPM"), this.PageContext.UnreadPrivate);
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> <span class=\"unread\">{2}</span> | ", 
                YafBuildLink.GetLink(ForumPages.cp_pm), 
                this.PageContext.Localization.GetText("CP_PM", "INBOX"), 
                unreadText));
          }
          else
          {
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> | ", 
                YafBuildLink.GetLink(ForumPages.cp_pm), 
                this.PageContext.Localization.GetText("CP_PM", "INBOX")));
          }
        }

        if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableBuddyList)
        {
          if (this.PageContext.PendingBuddies > 0)
          {
            string pendingBuddiesText = String.Format(
              this.PageContext.Localization.GetText("TOOLBAR", "BUDDYREQUEST"), this.PageContext.PendingBuddies);
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> <span class=\"unread\">{2}</span> | ", 
                YafBuildLink.GetLink(ForumPages.cp_editbuddies), 
                this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES"), 
                pendingBuddiesText));
          }
          else
          {
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> | ", 
                YafBuildLink.GetLink(ForumPages.cp_editbuddies), 
                this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES")));
          }
        }

        if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableAlbum)
        {
          int albumCount = DB.album_getstats(this.PageContext.PageUserID, null)[0];

          bool addAlbumLink = false;

          // Check if the user already has albums.
          if (albumCount > 0)
          {
            addAlbumLink = true;
          }
          else
          {
            // Check if a user have permissions to have albums, even if he has no albums at all.
            var usrAlbums =
              DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID).
                GetFirstRowColumnAsValue<int>("UsrAlbums", 0);

            if (usrAlbums > 0 || albumCount > 0)
            {
              addAlbumLink = true;
            }
          }

          if (addAlbumLink)
          {
            buildHeader.Append(this.AlbumLink());
          }
        }

        /* TODO: help is currently useless...
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.help_index ), GetText( "TOOLBAR", "HELP" ) ) );
				*/
        if (YafServices.Permissions.Check(this.PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.search), 
              this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        if (this.PageContext.IsAdmin)
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a target='_top' href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.admin_admin), 
              this.PageContext.Localization.GetText("TOOLBAR", "ADMIN")));
        }

        if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.moderate_index), 
              this.PageContext.Localization.GetText("TOOLBAR", "MODERATE")));
        }

        if (!this.PageContext.IsGuest)
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.mytopics), 
              this.PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")));
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.cp_profile), 
              this.PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")));
        }
        else
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.mytopics), 
              this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        }

        if (YafServices.Permissions.Check(this.PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.members), 
              this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            String.Format(
              " <a href=\"{0}\" onclick=\"return confirm('{2}');\">{1}</a>", 
              YafBuildLink.GetLink(ForumPages.logout), 
              this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT"), 
              this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT_QUESTION")));
        }
      }
      else
      {
        buildHeader.AppendFormat(
          String.Format(
            @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", 
            this.PageContext.Localization.GetText("TOOLBAR", "WELCOME_GUEST")));

        buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");
        if (YafServices.Permissions.Check(this.PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.search), 
              this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        buildHeader.AppendFormat(
          String.Format(
            "	<a href=\"{0}\">{1}</a> | ", 
            YafBuildLink.GetLink(ForumPages.mytopics), 
            this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        if (YafServices.Permissions.Check(this.PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", 
              YafBuildLink.GetLink(ForumPages.members), 
              this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        string returnUrl = this.GetReturnUrl();

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            String.Format(
              " <a href=\"{0}\">{1}</a>", 
              (returnUrl == string.Empty)
                ? YafBuildLink.GetLink(ForumPages.login)
                : YafBuildLink.GetLink(ForumPages.login, "ReturnUrl={0}", returnUrl), 
              this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")));

          if (!this.PageContext.BoardSettings.DisableRegistrations)
          {
            buildHeader.AppendFormat(
              String.Format(
                " | <a href=\"{0}\">{1}</a>", 
                this.PageContext.BoardSettings.ShowRulesForRegistration
                  ? YafBuildLink.GetLink(ForumPages.rules)
                  : YafBuildLink.GetLink(ForumPages.register), 
                this.PageContext.Localization.GetText("TOOLBAR", "REGISTER")));
          }
        }
      }
      buildHeader.ToString().TrimEnd(' ', '|');
      buildHeader.AppendFormat("</td></tr></table>");
      buildHeader.AppendFormat("<br />");

      // END HEADER
      writer.Write(buildHeader);
    }

    /// <summary>
    /// The album link.
    /// </summary>
    /// <returns>
    /// The album link.
    /// </returns>
    private string AlbumLink()
    {
      return string.Format(
        "	<a target='_top' href=\"{0}\">{1}</a> | ", 
        YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID), 
        this.PageContext.Localization.GetText("TOOLBAR", "MYALBUMS"));
    }

    #endregion
  }
}