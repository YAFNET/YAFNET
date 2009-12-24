/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for Header.
  /// </summary>
  public class Header : BaseControl, IYafHeader
  {
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

    #region IYafHeader Members

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

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      RefreshURL = null;
      RefreshTime = 10;
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
        RenderRegular(ref writer);
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
      string themeHeader = PageContext.Theme.GetItem("THEME", "HEADER", string.Empty);

      if (!String.IsNullOrEmpty(themeHeader))
      {
        buildHeader.Append(themeHeader);
      }

      buildHeader.AppendFormat(@"<table width=""100%"" cellspacing=""0"" class=""content"" cellpadding=""0"" id=""yafheader""><tr>");

      MembershipUser user = UserMembershipHelper.GetUser();

      if (user != null)
      {
        buildHeader.AppendFormat(
          @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", 
          String.Format(PageContext.Localization.GetText("TOOLBAR", "LOGGED_IN_AS") + " ", HttpContext.Current.Server.HtmlEncode(PageContext.PageUserName)));
        buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");

        if (!PageContext.IsGuest && PageContext.BoardSettings.AllowPrivateMessages)
        {
          if (PageContext.UnreadPrivate > 0)
          {
            string unreadText = String.Format(PageContext.Localization.GetText("TOOLBAR", "NEWPM"), PageContext.UnreadPrivate);
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> <span class=\"unread\">{2}</span> | ", 
                YafBuildLink.GetLink(ForumPages.cp_pm), 
                PageContext.Localization.GetText("CP_PM", "INBOX"), 
                unreadText));
          }
          else
          {
            buildHeader.AppendFormat(
              String.Format(
                "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.cp_pm), PageContext.Localization.GetText("CP_PM", "INBOX")));
          }
        }

        if (!PageContext.IsGuest && YafContext.Current.BoardSettings.EnableBuddyList)
        {
            if (PageContext.PendingBuddies > 0)
            {
                string pendingBuddiesText = String.Format(PageContext.Localization.GetText("TOOLBAR", "BUDDYREQUEST"), PageContext.PendingBuddies);
                buildHeader.AppendFormat(
                  String.Format(
                    "	<a target='_top' href=\"{0}\">{1}</a> <span class=\"unread\">{2}</span> | ",
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    PageContext.Localization.GetText("TOOLBAR", "BUDDIES"),
                    pendingBuddiesText));
            }
            else
            {
                buildHeader.AppendFormat(
                  String.Format(
                    "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.cp_editbuddies), PageContext.Localization.GetText("TOOLBAR", "BUDDIES")));
            }
        }

        if (!PageContext.IsGuest && YafContext.Current.BoardSettings.EnableAlbum)
        {
            buildHeader.AppendFormat(
                  String.Format(
                    "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.albums, "u={0}", PageContext.PageUserID ), PageContext.Localization.GetText("TOOLBAR", "MyAlbums")));
        }

        /* TODO: help is currently useless...
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.help_index ), GetText( "TOOLBAR", "HELP" ) ) );
				*/
        if (YafServices.Permissions.Check(PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.search), PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        if (PageContext.IsAdmin)
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.admin_admin), PageContext.Localization.GetText("TOOLBAR", "ADMIN")));
        }

        if (PageContext.IsModerator || PageContext.IsForumModerator)
        {
          buildHeader.AppendFormat(
            String.Format(
              "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.moderate_index), PageContext.Localization.GetText("TOOLBAR", "MODERATE")));
        }

        if (!PageContext.IsGuest)
        {
            buildHeader.AppendFormat(
                String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.mytopics), PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")));
            buildHeader.AppendFormat(
              String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.cp_profile), PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")));
        }
        else
        {
            buildHeader.AppendFormat(
              String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.mytopics), PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        }

        if (YafServices.Permissions.Check(PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.members), PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            String.Format(
              " <a href=\"{0}\" onclick=\"return confirm('{2}');\">{1}</a>", 
              YafBuildLink.GetLink(ForumPages.logout), 
              PageContext.Localization.GetText("TOOLBAR", "LOGOUT"), 
              PageContext.Localization.GetText("TOOLBAR", "LOGOUT_QUESTION")));
        }
      }
      else
      {
        buildHeader.AppendFormat(
          String.Format(
            @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", PageContext.Localization.GetText("TOOLBAR", "WELCOME_GUEST")));

        buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");
        if (YafServices.Permissions.Check(PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.search), PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        buildHeader.AppendFormat(
          String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.mytopics), PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        if (YafServices.Permissions.Check(PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            String.Format("	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.members), PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        string returnUrl = GetReturnUrl();

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            String.Format(
              " <a href=\"{0}\">{1}</a>", 
              (returnUrl == string.Empty) ? YafBuildLink.GetLink(ForumPages.login) : YafBuildLink.GetLink(ForumPages.login, "ReturnUrl={0}", returnUrl), 
              PageContext.Localization.GetText("TOOLBAR", "LOGIN")));

          if (!PageContext.BoardSettings.DisableRegistrations)
          {
            buildHeader.AppendFormat(
              String.Format(
                " | <a href=\"{0}\">{1}</a>", 
                PageContext.BoardSettings.ShowRulesForRegistration ? YafBuildLink.GetLink(ForumPages.rules) : YafBuildLink.GetLink(ForumPages.register), 
                PageContext.Localization.GetText("TOOLBAR", "REGISTER")));
          }
        }
      }

      buildHeader.AppendFormat("</td></tr></table>");
      buildHeader.AppendFormat("<br />");

      // END HEADER
      writer.Write(buildHeader);
    }

    /// <summary>
    /// The get return url.
    /// </summary>
    /// <returns>
    /// The get return url.
    /// </returns>
    protected string GetReturnUrl()
    {
      string returnUrl = string.Empty;

      if (PageContext.ForumPageType != ForumPages.login)
      {
        returnUrl = HttpContext.Current.Server.UrlEncode(General.GetSafeRawUrl());
      }
      else
      {
        // see if there is already one since we are on the login page
        if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
        {
          returnUrl = HttpContext.Current.Server.UrlEncode(General.GetSafeRawUrl(HttpContext.Current.Request.QueryString["ReturnUrl"]));
        }
      }

      return returnUrl;
    }
  }
}