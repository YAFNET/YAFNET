/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Header Control
    /// </summary>
    public class Header : BaseControl
    {
        #region Methods

        /// <summary>
        /// Gets the return URL.
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
                            General.GetSafeRawUrl(
                                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl")));
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
        protected override void Render([NotNull] HtmlTextWriter writer)
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
        protected void RenderRegular([NotNull] ref HtmlTextWriter writer)
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
                    this.GetText("TOOLBAR", "LOGGED_IN_AS").FormatWith(string.Empty),
                    this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.Get<YafBoardSettings>().EnableDisplayName && displayName.IsSet() ? displayName : this.PageContext.PageUserName),
                    this.Get<YafBoardSettings>().UseStyledNicks
                        ? this.Get<IStyleTransform>().DecodeStyleByString(this.PageContext.UserStyle, false)
                        : null);

                buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");

                if (!this.PageContext.IsGuest && this.Get<YafBoardSettings>().AllowPrivateMessages)
                {
                    if (this.PageContext.UnreadPrivate > 0)
                    {
                        string unreadText = this.GetText("TOOLBAR", "NEWPM").FormatWith(this.PageContext.UnreadPrivate);
                        buildHeader.AppendFormat("	<a target='_top' href=\"{0}\">{1}</a>&nbsp;<span class=\"unread\">{2}</span> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.cp_pm), this.GetText("CP_PM", "INBOX"), unreadText));
                    }
                    else
                    {
                        buildHeader.AppendFormat(
                            "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(
                                YafBuildLink.GetLink(ForumPages.cp_pm), this.GetText("CP_PM", "INBOX")));
                    }
                }

                if (!this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableBuddyList &&
                    this.PageContext.UserHasBuddies)
                {
                    if (this.PageContext.PendingBuddies > 0)
                    {
                        string pendingBuddiesText =
                            this.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(this.PageContext.PendingBuddies);
                        buildHeader.AppendFormat(
                            "	<a target='_top' href=\"{0}\">{1}</a>&nbsp;<span class=\"unread\">{2}</span> | ".FormatWith(
                                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                                    this.GetText("TOOLBAR", "BUDDIES"),
                                    pendingBuddiesText));
                    }
                    else
                    {
                        buildHeader.AppendFormat(
                            "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(
                                YafBuildLink.GetLink(ForumPages.cp_editbuddies), this.GetText("TOOLBAR", "BUDDIES")));
                    }
                }

                if (!this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableAlbum &&
                    (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
                {
                    buildHeader.AppendFormat(
                        "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                            this.GetText("TOOLBAR", "MYALBUMS")));
                }

                buildHeader.AppendFormat(
                    "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(
                        YafBuildLink.GetLink(ForumPages.help_index), this.GetText("TOOLBAR", "HELP")));

                if (this.PageContext.IsAdmin)
                {
                    buildHeader.AppendFormat(
                        "	<a target='_top' href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.admin_admin), this.GetText("TOOLBAR", "ADMIN")));
                }

                if (this.PageContext.IsModeratorInAnyForum)
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.moderate_index), this.GetText("TOOLBAR", "MODERATE")));
                }

                if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ExternalSearchPermissions) ||
                    this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.search), this.GetText("TOOLBAR", "SEARCH")));
                }

                if (!this.PageContext.IsGuest)
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.mytopics), this.GetText("TOOLBAR", "MYTOPICS")));
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.cp_profile), this.GetText("TOOLBAR", "MYPROFILE")));
                }
                else
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.mytopics), this.GetText("TOOLBAR", "ACTIVETOPICS")));
                }

                if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().MembersListViewPermissions))
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.members), this.GetText("TOOLBAR", "MEMBERS")));
                }

                if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
                {
                    buildHeader.AppendFormat(
                        " <a href=\"{0}\" onclick=\"return confirm('{2}');\">{1}</a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.logout),
                            this.GetText("TOOLBAR", "LOGOUT"),
                            this.GetText("TOOLBAR", "LOGOUT_QUESTION")));
                }
            }
            else
            {
                buildHeader.AppendFormat(
                    @"<td style=""padding:5px"" class=""post"" align=""left""><strong>{0}</strong></td>".FormatWith(
                        this.GetText("TOOLBAR", "WELCOME_GUEST")));

                buildHeader.AppendFormat(@"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">");
                if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ExternalSearchPermissions) ||
                    this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.search), this.GetText("TOOLBAR", "SEARCH")));
                }

                buildHeader.AppendFormat(
                    "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                        YafBuildLink.GetLink(ForumPages.mytopics), this.GetText("TOOLBAR", "ACTIVETOPICS")));
                if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().MembersListViewPermissions))
                {
                    buildHeader.AppendFormat(
                        "	<a href=\"{0}\">{1}</a> | ".FormatWith(
                            YafBuildLink.GetLink(ForumPages.members), this.GetText("TOOLBAR", "MEMBERS")));
                }

                string returnUrl = this.GetReturnUrl();

                if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
                {
                    buildHeader.AppendFormat(
                        " <a href=\"{0}\">{1}</a>".FormatWith(
                            (returnUrl == string.Empty)
                                ? (!this.Get<YafBoardSettings>().UseSSLToLogIn
                                       ? YafBuildLink.GetLink(ForumPages.login)
                                       : YafBuildLink.GetLink(ForumPages.login, true).Replace("http:", "https:"))
                                : (!this.Get<YafBoardSettings>().UseSSLToLogIn
                                       ? YafBuildLink.GetLink(ForumPages.login, "ReturnUrl={0}", returnUrl)
                                       : YafBuildLink.GetLink(ForumPages.login, true, "ReturnUrl={0}", returnUrl).Replace("http:", "https:")),
                            this.GetText("TOOLBAR", "LOGIN")));

                    if (!this.Get<YafBoardSettings>().DisableRegistrations)
                    {
                        buildHeader.AppendFormat(
                            " | <a href=\"{0}\">{1}</a>".FormatWith(
                                this.Get<YafBoardSettings>().ShowRulesForRegistration
                                    ? YafBuildLink.GetLink(ForumPages.rules)
                                    : (!this.Get<YafBoardSettings>().UseSSLToRegister
                                           ? YafBuildLink.GetLink(ForumPages.register)
                                           : YafBuildLink.GetLink(ForumPages.register, true).Replace("http:", "https:")),
                                this.GetText("TOOLBAR", "REGISTER")));
                    }
                }
            }

            buildHeader.ToString().TrimEnd(' ', '|');
            buildHeader.Append("</td></tr></table>");
            buildHeader.Append("<br />");

            // END HEADER
            writer.Write(buildHeader);
        }

        #endregion
    }
}