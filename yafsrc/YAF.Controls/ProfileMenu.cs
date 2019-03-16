/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// User Profile Menu in the User Control Panel
    /// </summary>
    public class ProfileMenu : BaseControl
    {
        #region Methods

        /// <summary>
        /// Render the Profile Menu
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var html = new StringBuilder();

            html.Append(@"<div class=""dropdown"">");

            html.Append(@"<button class=""btn btn-secondary dropdown-toggle"" type=""button"" id=""dropdownMenuButton"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">");
            html.AppendFormat(@"<i class=""fa fa-cogs fa-fw""></i>&nbsp;{0}", this.GetText("CONTROL_PANEL"));
            html.Append(@"</button>");

            html.Append(@"<div class=""dropdown-menu scrollable-dropdown"" aria-labelledby=""dropdownMenuButton"">");

            // Render Mailbox Items
            if (this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                html.AppendFormat(@"<h6 class=""dropdown-header"">{0}</h6>", this.GetText("MESSENGER"));

                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{3}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=in"),
                    this.GetText("INBOX"),
                    ForumPages.cp_pm,
                    this.GetText("TOOLBAR", "INBOX_TITLE"));
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"),
                    this.GetText("SENTITEMS"),
                    ForumPages.cp_pm);
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"),
                    this.GetText("ARCHIVE"), 
                    ForumPages.cp_pm);
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.pmessage),
                    this.GetText("NEW_MESSAGE"),
                    ForumPages.pmessage);

                html.AppendFormat(@"</ul></td></tr>");
            }

            // Render Personal Profile Items
            html.AppendFormat(@"<h6 class=""dropdown-header"">{0}</h6>", this.GetText("PERSONAL_PROFILE"));
            
            html.AppendFormat(
                @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.PageContext.PageUserID),
                this.GetText("VIEW_PROFILE"),
                    ForumPages.profile);

            if (!Config.IsDotNetNuke)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_editprofile),
                    this.GetText("EDIT_PROFILE"),
                    ForumPages.cp_editprofile);
            }

            if (!this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableThanksMod)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.viewthanks, "u={0}", this.PageContext.PageUserID),
                    this.GetText("ViewTHANKS", "TITLE"),
                    ForumPages.viewthanks);
            }
            
            if (!this.PageContext.IsGuest)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.attachments),
                    this.GetText("ATTACHMENTS", "TITLE"),
                    ForumPages.attachments);
            }

            if (!this.PageContext.IsGuest
                && this.Get<YafBoardSettings>().EnableBuddyList & this.PageContext.UserHasBuddies)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{3}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    this.GetText("EDIT_BUDDIES"),
                    ForumPages.cp_editbuddies,
                    this.GetText("TOOLBAR", "BUDDIES_TITLE"));
            }

            if (!this.PageContext.IsGuest
                && (this.Get<YafBoardSettings>().EnableAlbum || (this.PageContext.NumAlbums > 0)))
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                    this.GetText("EDIT_ALBUMS"),
                    ForumPages.albums);
            }

            if (!Config.IsDotNetNuke && (this.Get<YafBoardSettings>().AvatarRemote || this.Get<YafBoardSettings>().AvatarUpload
                || this.Get<YafBoardSettings>().AvatarGallery || this.Get<YafBoardSettings>().AvatarGravatar))
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_editavatar),
                    this.GetText("EDIT_AVATAR"),
                    ForumPages.cp_editavatar);
            }

            if (this.Get<YafBoardSettings>().AllowSignatures)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_signature),
                    this.GetText("SIGNATURE"),
                    ForumPages.cp_signature);
            }

            html.AppendFormat(
                @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                YafBuildLink.GetLink(ForumPages.cp_subscriptions),
                this.GetText("SUBSCRIPTIONS"),
                    ForumPages.cp_subscriptions);

            if (!Config.IsDotNetNuke && this.Get<YafBoardSettings>().AllowPasswordChange)
            {
                html.AppendFormat(
                    @"<a class=""dropdown-item"" href=""{0}"" title=""{1}"">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.cp_changepassword),
                    this.GetText("CHANGE_PASSWORD"),
                    ForumPages.cp_changepassword);
            }

            html.Append(@"</div></div>");

            writer.Write(html.ToString());
        }

        #endregion
    }
}
 