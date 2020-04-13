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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// User Page To Delete (deactivate) his account
    /// </summary>
    public partial class DeleteAccount : ForumPageRegistered
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAccount"/> class.
        /// </summary>
        public DeleteAccount()
            : base("DELETE_ACCOUNT")
        {
        }

        #region Methods

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));

            this.PageLinks.AddLink(
                string.Format(this.GetText("DELETE_ACCOUNT", "TITLE"), this.Get<BoardSettings>().Name),
                string.Empty);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (Config.IsDotNetNuke || this.PageContext.IsAdmin || this.PageContext.IsHostAdmin)
            {
                BuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.Options.Items.Add(
                new ListItem(
                    $"<strong>{this.GetText("OPTION_SUSPEND_TITLE")}</strong><br />{this.GetText("OPTION_SUSPEND_TEXT")}",
                    "suspend"));
            this.Options.Items.Add(
                new ListItem(
                    $"<strong>{this.GetText("OPTION_DELETE_TITLE")}</strong><br />{this.GetText("OPTION_DELETE_TEXT")}",
                    "delete"));

            this.Options.SelectedIndex = 0;

            this.Cancel.NavigateUrl = BuildLink.GetLink(ForumPages.Account);

            this.DeleteUser.ReturnConfirmText = this.GetText("CONFIRM");
        }

        /// <summary>
        /// The delete user click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteUserClick(object sender, EventArgs e)
        {
            switch (this.Options.SelectedValue)
            {
                case "suspend":
                    {
                        // Suspend User for 30 Days
                        // time until when user is suspended
                        var suspend = this.Get<IDateTime>().GetUserDateTime(
                            DateTime.UtcNow,
                            this.PageContext.TimeZoneInfoUser).AddDays(30);

                        // suspend user by calling appropriate method
                        this.GetRepository<User>().Suspend(
                            this.PageContext.PageUserID,
                            suspend,
                            "User Suspended his own account",
                            this.PageContext.PageUserID);

                        var usr = this.GetRepository<User>().UserList(
                            this.PageContext.PageBoardID,
                            this.PageContext.PageUserID,
                            null,
                            null,
                            null,
                            false).ToList();

                        if (usr.Any())
                        {
                            this.Get<ILogger>().Log(
                                this.PageContext.PageUserID,
                                this,
                                $"User {(this.Get<BoardSettings>().EnableDisplayName ? usr.First().DisplayName : usr.First().Name)} Suspended his own account until: {suspend} (UTC)",
                                EventLogTypes.UserSuspended);

                            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));
                        }
                    }

                    break;
                case "delete":
                    {
                        // (Soft) Delete User
                        var user = this.PageContext.User;

                        // Update IsApproved
                        user.IsApproved = false;

                        this.Get<MembershipProvider>().UpdateUser(user);

                        // delete posts...
                        var messages = this.GetRepository<Message>().GetAllUserMessages(this.PageContext.PageUserID);

                        var messageIds = messages.Select(m => m.ID).Distinct().ToList();

                        messageIds.ForEach(
                            x => this.GetRepository<Message>().Delete(x, true, string.Empty, 1, true, false));

                        this.Get<ILogger>().Log(
                            this.PageContext.PageUserID,
                            this,
                            $"User {this.PageContext.PageUserName} Deleted his own account",
                            EventLogTypes.UserDeleted);
                    }

                    break;
            }

            BuildLink.Redirect(ForumPages.forum);
        }

        #endregion
    }
}