/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.Controls
{
    #region Using

    using System.Text;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
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
            var htmlDropDown = new StringBuilder();

            htmlDropDown.Append(@"<div class=""dropdown d-lg-none mb-3"">");

            htmlDropDown.Append(
                @"<button class=""btn btn-secondary dropdown-toggle w-100 text-left mb-3"" type=""button"" id=""dropdownMenuButton"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">");
            htmlDropDown.AppendFormat(@"<i class=""fa fa-cogs fa-fw""></i>&nbsp;{0}", this.GetText("CONTROL_PANEL"));
            htmlDropDown.Append(@"</button>");

            htmlDropDown.Append(
                @"<div class=""dropdown-menu scrollable-dropdown"" aria-labelledby=""dropdownMenuButton"">");

            html.Append(@"<div class=""list-group d-none d-md-block"">");

            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.cp_profile,
                this.GetText("YOUR_ACCOUNT"));

            this.RenderMenuItem(htmlDropDown, "dropdown-item", ForumPages.cp_profile, this.GetText("YOUR_ACCOUNT"));

            var unreadActivity =
                this.PageContext.Mention + this.PageContext.Quoted + this.PageContext.ReceivedThanks;

           this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.profile,
                this.GetText("VIEW_PROFILE"),
                $"u={this.PageContext.PageUserID}");

            htmlDropDown.AppendFormat(@"<h6 class=""dropdown-header"">{0}</h6>", this.GetText("PERSONAL_PROFILE"));

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.profile,
                this.GetText("VIEW_PROFILE"),
                $"u={this.PageContext.PageUserID}");

            if (!Config.IsDotNetNuke)
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_editprofile,
                    this.GetText("EDIT_PROFILE"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_editprofile,
                    this.GetText("EDIT_PROFILE"));

                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_editsettings,
                    this.GetText("EDIT_SETTINGS"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_editsettings,
                    this.GetText("EDIT_SETTINGS"));
            }

            if (!this.PageContext.IsGuest)
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.attachments,
                    this.GetText("ATTACHMENTS", "TITLE"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.attachments,
                    this.GetText("ATTACHMENTS", "TITLE"));
            }

            if (!this.PageContext.IsGuest
                && this.Get<BoardSettings>().EnableBuddyList & this.PageContext.UserHasBuddies)
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_editbuddies,
                    this.GetText("EDIT_BUDDIES"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_editbuddies,
                    this.GetText("EDIT_BUDDIES"));
            }

            if (!this.PageContext.IsGuest && this.Get<BoardSettings>().EnableAlbum)
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.albums,
                    this.GetText("EDIT_ALBUMS"),
                    $"u={this.PageContext.PageUserID}");

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.albums,
                    this.GetText("EDIT_ALBUMS"),
                    $"u={this.PageContext.PageUserID}");
            }

            if (!Config.IsDotNetNuke && (this.Get<BoardSettings>().AvatarRemote
                                         || this.Get<BoardSettings>().AvatarUpload
                                         || this.Get<BoardSettings>().AvatarGallery
                                         || this.Get<BoardSettings>().AvatarGravatar))
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_editavatar,
                    this.GetText("EDIT_AVATAR"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_editavatar,
                    this.GetText("EDIT_AVATAR"));
            }

            if (this.Get<BoardSettings>().AllowSignatures)
            {
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_signature,
                    this.GetText("CP_PROFILE", "SIGNATURE"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_signature,
                    this.GetText("CP_PROFILE", "SIGNATURE"));
            }

            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.cp_subscriptions,
                this.GetText("SUBSCRIPTIONS"));

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.cp_subscriptions,
                this.GetText("SUBSCRIPTIONS"));

            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.cp_blockoptions,
                this.GetText("CP_BLOCKOPTIONS", "TITLE"));

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.cp_blockoptions,
                this.GetText("CP_BLOCKOPTIONS", "TITLE"));

            if (!Config.IsDotNetNuke && this.Get<BoardSettings>().AllowPasswordChange)
            {
                // Render Change Password Item
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_changepassword,
                    this.GetText("CHANGE_PASSWORD"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_changepassword,
                    this.GetText("CHANGE_PASSWORD"));
            }

            if (!Config.IsDotNetNuke && !this.PageContext.IsAdmin  && !this.PageContext.IsHostAdmin)
            {
                // Render Delete Account Item
                this.RenderMenuItem(
                    html,
                    "list-group-item list-group-item-action",
                    ForumPages.cp_deleteaccount,
                    this.GetText("DELETE_ACCOUNT"));

                this.RenderMenuItem(
                    htmlDropDown,
                    "dropdown-item",
                    ForumPages.cp_deleteaccount,
                    this.GetText("DELETE_ACCOUNT"));
            }

            htmlDropDown.Append("</div></div>");

            html.Append("</div>");

            writer.Write(html.ToString());
            writer.Write(htmlDropDown.ToString());
        }

        /// <summary>
        /// The render menu item.
        /// </summary>
        /// <param name="stringBuilder">
        /// The string builder.
        /// </param>
        /// <param name="cssClass">
        /// The CSS class.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="getText">
        /// The get text.
        /// </param>
        /// <param name="parameter">
        /// The URL Parameter
        /// </param>
        private void RenderMenuItem(
            StringBuilder stringBuilder,
            string cssClass,
            ForumPages page,
            string getText,
            string parameter = null)
        {
            stringBuilder.AppendFormat(
                this.PageContext.ForumPageType == page
                    ? @"<a class=""{3} active"" href=""{0}"" title=""{2}"" data-toggle=""tooltip"">{1}</a>"
                    : @"<a class=""{3}"" href=""{0}"" title=""{2}"" data-toggle=""tooltip"">{1}</a>",
                parameter.IsSet() ? BuildLink.GetLink(page, parameter) : BuildLink.GetLink(page),
                getText,
                getText,
                cssClass);
        }

        #endregion
    }
}