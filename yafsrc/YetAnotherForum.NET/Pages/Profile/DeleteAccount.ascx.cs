/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Pages.Profile
{
    #region Using

    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;

    #endregion

    /// <summary>
    /// User Page To Delete (deactivate) his account
    /// </summary>
    public partial class DeleteAccount : ProfilePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAccount"/> class.
        /// </summary>
        public DeleteAccount()
            : base("DELETE_ACCOUNT", ForumPages.Profile_DeleteAccount)
        {
        }

        #region Methods

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));

            this.PageLinks.AddLink(
                string.Format(this.GetText("DELETE_ACCOUNT", "TITLE"), this.PageBoardContext.BoardSettings.Name),
                string.Empty);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (Config.IsDotNetNuke || this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
            {
                this.Get<LinkBuilder>().AccessDenied();
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

            this.Cancel.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount);

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
                        var suspend = this.Get<IDateTimeService>().GetUserDateTime(
                            DateTime.UtcNow,
                            this.PageBoardContext.TimeZoneInfoUser).AddDays(30);

                        // suspend user by calling appropriate method
                        this.GetRepository<User>().Suspend(
                            this.PageBoardContext.PageUserID,
                            suspend,
                            "User Suspended his own account",
                            this.PageBoardContext.PageUserID);

                        this.Get<ILoggerService>().Log(
                            this.PageBoardContext.PageUserID,
                            this,
                            $"User {this.PageBoardContext.PageUser.DisplayOrUserName()} Suspended his own account until: {suspend} (UTC)",
                            EventLogTypes.UserSuspended);

                        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));
                    }

                    break;
                case "delete":
                    {
                        // (Soft) Delete User
                        var yafUser = this.PageBoardContext.PageUser;

                        yafUser.UserFlags.IsDeleted = true;
                        yafUser.UserFlags.IsApproved = false;

                        this.GetRepository<User>().UpdateOnly(
                            () => new User { Flags = yafUser.UserFlags.BitValue },
                            u => u.ID == this.PageBoardContext.PageUserID);

                        this.GetRepository<AspNetUsers>().UpdateOnly(
                            () => new AspNetUsers { IsApproved = false },
                            u => u.Id == yafUser.ProviderUserKey);

                        // delete posts...
                        var messages = this.GetRepository<Message>().GetAllUserMessages(this.PageBoardContext.PageUserID);

                        messages.ForEach(
                            x => this.GetRepository<Message>().Delete(
                                x.Topic.ForumID,
                                x.TopicID,
                                x.ID,
                                true,
                                string.Empty,
                                true,
                                true,
                                false));

                        this.Get<ILoggerService>().UserDeleted(
                            this.PageBoardContext.PageUserID,
                            $"User {this.PageBoardContext.PageUser.DisplayOrUserName()} Deleted his own account");
                    }

                    break;
            }

            this.Get<LinkBuilder>().Redirect(ForumPages.Board);
        }

        #endregion
    }
}