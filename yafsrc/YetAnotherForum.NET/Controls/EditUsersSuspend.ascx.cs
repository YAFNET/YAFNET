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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users suspend.
    /// </summary>
    public partial class EditUsersSuspend : BaseUserControl
    {
        /// <summary>
        /// The _user data.
        /// </summary>
        private CombinedUserDataHelper _userData;

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether ShowHeader.
        /// </summary>
        public bool ShowHeader
        {
            get => this.ViewState["ShowHeader"] == null || Convert.ToBoolean(this.ViewState["ShowHeader"]);

            set => this.ViewState["ShowHeader"] = value;
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        protected int CurrentUserID => this.PageContext.QueryIDs["u"].ToType<int>();

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private CombinedUserDataHelper UserData =>
            this._userData ?? (this._userData = new CombinedUserDataHelper(this.CurrentUserID));

        #endregion

        #region Methods

        /// <summary>
        /// Gets the time until user is suspended.
        /// </summary>
        /// <returns>
        /// Date and time until when user is suspended. Empty string when user is not suspended.
        /// </returns>
        protected string GetSuspendedTo()
        {
            // is there suspension expiration in the view-state?
            return this.ViewState["SuspendedUntil"] != null
                       ? this.ViewState["SuspendedUntil"].ToString()
                       : string.Empty;
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // init ids...
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            this.SuspendInfo.Text = this.GetTextFormatted(
                "SUSPEND_INFO",
                this.Get<IDateTime>().GetUserDateTime(DateTime.UtcNow, this.UserData.TimeZoneInfo)
                    .ToString(CultureInfo.InvariantCulture));

            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // add items to the dropdown
            this.SuspendUnit.Items.Add(new ListItem(this.GetText("PROFILE", "DAYS"), "1"));
            this.SuspendUnit.Items.Add(new ListItem(this.GetText("PROFILE", "HOURS"), "2"));
            this.SuspendUnit.Items.Add(new ListItem(this.GetText("PROFILE", "MINUTES"), "3"));

            // select hours
            this.SuspendUnit.SelectedIndex = 1;

            // default number of hours to suspend user for
            this.SuspendCount.Text = "2";

            // bind data
            this.BindData();
        }

        /// <summary>
        /// The page_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.trHeader.Visible = this.ShowHeader;
        }

        /// <summary>
        /// Removes suspension from a user.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemoveSuspension_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // un-suspend user
            this.GetRepository<User>().Suspend(this.CurrentUserID);

            var usr = this.GetRepository<User>().UserList(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                null,
                null,
                null,
                false).ToList();

            if (usr.Any())
            {
                if (this.Get<BoardSettings>().LogUserSuspendedUnsuspended)
                {
                    this.Get<ILogger>().Log(
                        this.PageContext.PageUserID,
                        "YAF.Controls.EditUsersSuspend",
                        $"User {(this.Get<BoardSettings>().EnableDisplayName ? usr.First().DisplayName : usr.First().Name)} was unsuspended by {(this.Get<BoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName)}.",
                        EventLogTypes.UserUnsuspended);
                }

                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

                this.Get<ISendNotification>().SendUserSuspensionEndedNotification(
                    this.UserData.Email,
                    this.PageContext.BoardSettings.EnableDisplayName
                        ? this.UserData.DisplayName
                        : this.UserData.UserName);
            }

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Suspends a user when clicked.
        /// </summary>
        /// <param name="sender">
        /// The object sender inherit from Page.
        /// </param>
        /// <param name="e">
        /// The System.EventArgs inherit from Page.
        /// </param>
        protected void Suspend_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Admins can suspend anyone not admins
            // Forum Moderators can suspend anyone not admin or forum moderator
            using (var dt = this.GetRepository<User>().ListAsDataTable(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                null))
            {
                dt.Rows.Cast<DataRow>().ForEach(row =>
                {
                    // is user to be suspended admin?
                    if (row["IsAdmin"] != DBNull.Value && row["IsAdmin"].ToType<int>() > 0)
                    {
                        // tell user he can't suspend admin
                        this.PageContext.AddLoadMessage(
                            this.GetText("PROFILE", "ERROR_ADMINISTRATORS"),
                            MessageTypes.danger);
                        return;
                    }

                    // is user to be suspended forum moderator, while user suspending him is not admin?
                    if (!this.PageContext.IsAdmin && int.Parse(row["IsForumModerator"].ToString()) > 0)
                    {
                        // tell user he can't suspend forum moderator when he's not admin
                        this.PageContext.AddLoadMessage(
                            this.GetText("PROFILE", "ERROR_FORUMMODERATORS"),
                            MessageTypes.danger);
                        return;
                    }

                    var isGuest = row["IsGuest"];

                    // verify the user isn't guest...
                    if (isGuest != DBNull.Value && isGuest.ToType<int>() > 0)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("PROFILE", "ERROR_GUESTACCOUNT"),
                            MessageTypes.danger);
                    }
                });
            }

            // time until when user is suspended
            var suspend = this.Get<IDateTime>().GetUserDateTime(DateTime.UtcNow, this.UserData.TimeZoneInfo);

            // number inserted by suspending user
            var count = int.Parse(this.SuspendCount.Text);

            // what time units are used for suspending
            suspend = this.SuspendUnit.SelectedValue switch
                {
                    // days
                    "1" =>
                    // add user inserted suspension time to current time
                    suspend.AddDays(count),
                    // hours
                    "2" =>
                    // add user inserted suspension time to current time
                    suspend.AddHours(count),
                    // minutes
                    "3" =>
                    // add user inserted suspension time to current time
                    suspend.AddHours(count),
                    _ => suspend
                };

            // suspend user by calling appropriate method
            this.GetRepository<User>().Suspend(
                this.CurrentUserID,
                suspend,
                this.SuspendedReason.Text.Trim(),
                this.PageContext.PageUserID);

            var usr = this.GetRepository<User>().UserList(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                null,
                null,
                null,
                false).ToList();

            if (usr.Any())
            {
                this.Get<ILogger>().Log(
                    this.PageContext.PageUserID,
                    "YAF.Controls.EditUsersSuspend",
                    $"User {(this.Get<BoardSettings>().EnableDisplayName ? usr.First().DisplayName : usr.First().Name)} was suspended by {(this.Get<BoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName)} until: {suspend} (UTC)",
                    EventLogTypes.UserSuspended);

                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

                this.Get<ISendNotification>().SendUserSuspensionNotification(
                    suspend,
                    this.SuspendedReason.Text.Trim(),
                    this.UserData.Email,
                    this.PageContext.BoardSettings.EnableDisplayName
                        ? this.UserData.DisplayName
                        : this.UserData.UserName);
            }

            this.SuspendedReason.Text = string.Empty;

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // get user's info
            using (var dt = this.GetRepository<User>().ListAsDataTable(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                null))
            {
                // there is no such user
                if (dt.Rows.Count < 1)
                {
                    BuildLink.AccessDenied(/*No such user exists*/);
                }

                // get user's data in form of data row
                var user = dt.GetFirstRow();

                // if user is not suspended, hide row with suspend information and remove suspension button
                this.SuspendedHolder.Visible = !user.IsNull("Suspended");

                // is user suspended?
                if (!user.IsNull("Suspended"))
                {
                    // get time when his suspension expires to the view state
                    this.ViewState["SuspendedUntil"] = this.Get<IDateTime>().GetUserDateTime(
                        user["Suspended"].ToType<DateTime>(),
                        this.UserData.TimeZoneInfo);

                    this.CurrentSuspendedReason.Text = user["SuspendedReason"].ToString();

                    this.SuspendedBy.UserID = user["SuspendedBy"].ToType<int>();
                }
            }
        }

        #endregion
    }
}