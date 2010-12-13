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
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for Header.
  /// </summary>
  public class Header : BaseControl
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
        returnUrl = this.Get<HttpServerUtilityBase>().UrlEncode(General.GetSafeRawUrl());
      }
      else
      {
        // see if there is already one since we are on the login page
        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl").IsSet())
        {
          returnUrl =
            this.Get<HttpServerUtilityBase>().UrlEncode(
              General.GetSafeRawUrl(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl")));
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
      this.RenderRegular(ref writer);
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

      buildHeader.AppendFormat(
        @"<table width=""100%"" cellspacing=""0"" class=""content"" cellpadding=""0"" id=""yafheader""><tr>");

      MembershipUser user = UserMembershipHelper.GetUser();

      if (user != null)
      {
        string displayName = this.PageContext.CurrentUserData.DisplayName;
        buildHeader.AppendFormat(
          @"<td style=""padding:5px"" class=""post"" align=""left""><strong>{0}&nbsp;<span id=""nick_{1}"" style =""{2}"" >{1}</span></strong></td>",         
          this.PageContext.Localization.GetText("TOOLBAR", "LOGGED_IN_AS").FormatWith(string.Empty),
            this.Get<HttpServerUtilityBase>().HtmlEncode(!string.IsNullOrEmpty(displayName) ? displayName : this.PageContext.PageUserName),
            this.PageContext.BoardSettings.UseStyledNicks
                                  ? new StyleTransform(this.PageContext.Theme).DecodeStyleByString(this.PageContext.UserStyle, false)
                                  : null);
       
          buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");

        if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.AllowPrivateMessages)
        {
          if (this.PageContext.UnreadPrivate > 0)
          {
            string unreadText = this.PageContext.Localization.GetText("TOOLBAR", "NEWPM").FormatWith(this.PageContext.UnreadPrivate);
            buildHeader.AppendFormat(
              "	<a target='_top' href=\"{0}\">{1}</a>&nbsp;<span class=\"unread\">{2}</span> | ".FormatWith(YafBuildLink.GetLink(ForumPages.cp_pm), this.PageContext.Localization.GetText("CP_PM", "INBOX"), unreadText));
          }
          else
          {
            buildHeader.AppendFormat(
              "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.cp_pm), this.PageContext.Localization.GetText("CP_PM", "INBOX")));
          }
        }

        if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableBuddyList && this.PageContext.UserHasBuddies)
        {
          if (this.PageContext.PendingBuddies > 0)
          {
            string pendingBuddiesText = this.PageContext.Localization.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(this.PageContext.PendingBuddies);
            buildHeader.AppendFormat(
              "	<a target='_top' href=\"{0}\">{1}</a>&nbsp;<span class=\"unread\">{2}</span> | ".FormatWith(YafBuildLink.GetLink(ForumPages.cp_editbuddies), this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES"), pendingBuddiesText));
          }
          else
          {             
                  buildHeader.AppendFormat(
                    "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.cp_editbuddies), this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES")));              
          }
        }

        if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableAlbum && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
        {
            buildHeader.AppendFormat("	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID), this.PageContext.Localization.GetText("TOOLBAR", "MYALBUMS")));          
        }

        buildHeader.AppendFormat(String.Format("	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink(ForumPages.help_index), this.PageContext.Localization.GetText("TOOLBAR", "HELP")));

        if (this.PageContext.IsAdmin)
        {
            buildHeader.AppendFormat(
              "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.admin_admin), this.PageContext.Localization.GetText("TOOLBAR", "ADMIN")));
        }

        if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
        {
            buildHeader.AppendFormat(
              "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.moderate_index), this.PageContext.Localization.GetText("TOOLBAR", "MODERATE")));
        }

        if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || this.Get<IPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.search), this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        if (!this.PageContext.IsGuest)
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.mytopics), this.PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")));
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.cp_profile), this.PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")));
        }
        else
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.mytopics), this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        }

        if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.members), this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            " <a href=\"{0}\" onclick=\"return confirm('{2}');\">{1}</a>".FormatWith(YafBuildLink.GetLink(ForumPages.logout), this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT"), this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT_QUESTION")));
        }
      }
      else
      {
        buildHeader.AppendFormat(
          @"<td style=""padding:5px"" class=""post"" align=""left""><strong>{0}</strong></td>".FormatWith(this.PageContext.Localization.GetText("TOOLBAR", "WELCOME_GUEST")));

        buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");
        if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || this.Get<IPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions))
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.search), this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")));
        }

        buildHeader.AppendFormat(
          "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.mytopics), this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")));
        if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.MembersListViewPermissions))
        {
          buildHeader.AppendFormat(
            "	<a href=\"{0}\">{1}</a> | ".FormatWith(YafBuildLink.GetLink(ForumPages.members), this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")));
        }

        string returnUrl = this.GetReturnUrl();

        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
          buildHeader.AppendFormat(
            " <a href=\"{0}\">{1}</a>".FormatWith((returnUrl == string.Empty)
                                                    ? (!this.PageContext.BoardSettings.UseSSLToLogIn ? YafBuildLink.GetLink(ForumPages.login) : YafBuildLink.GetLink(ForumPages.login).Replace("http:","https:"))
                                                    : (!this.PageContext.BoardSettings.UseSSLToLogIn ? YafBuildLink.GetLink(ForumPages.login, "ReturnUrl={0}", returnUrl) : YafBuildLink.GetLink(ForumPages.login, "ReturnUrl={0}", returnUrl).Replace("http:", "https:")), this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")));

          if (!this.PageContext.BoardSettings.DisableRegistrations)
          {
            buildHeader.AppendFormat(
              " | <a href=\"{0}\">{1}</a>".FormatWith(this.PageContext.BoardSettings.ShowRulesForRegistration
                                                        ? YafBuildLink.GetLink(ForumPages.rules)
                                                        : (!this.PageContext.BoardSettings.UseSSLToRegister ? YafBuildLink.GetLink(ForumPages.register) : YafBuildLink.GetLink(ForumPages.register).Replace("http:", "https:")), this.PageContext.Localization.GetText("TOOLBAR", "REGISTER")));
          }
        }
      }

      buildHeader.ToString().TrimEnd(' ', '|');
      buildHeader.AppendFormat("</td></tr></table>");
      buildHeader.AppendFormat("<br />");

      // END HEADER
      writer.Write(buildHeader);
    } 

    #endregion
  }
}