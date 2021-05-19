/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System.Globalization;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The edit users suspend.
    /// </summary>
    public partial class EditUsersSuspend : BaseUserControl
    {
        /// <summary>
        /// The user.
        /// </summary>
        private User user;

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
        protected int CurrentUserID =>
            this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private User User
        {
            get
            {
                var id = this.user;
                if (id != null)
                {
                    return id;
                }

                return this.user = this.GetRepository<User>().GetById(this.CurrentUserID);
            }
        }

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
            return this.Get<IDateTimeService>().GetUserDateTime(
                this.User.Suspended.Value,
                this.User.TimeZoneInfo).ToString(CultureInfo.InvariantCulture);
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
            this.SuspendInfo.Text = this.GetTextFormatted(
                "SUSPEND_INFO",
                this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow, this.User.TimeZoneInfo)
                    .ToString(CultureInfo.InvariantCulture));

            // this needs to be done just once, not during post-back
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

            if (this.PageContext.BoardSettings.LogUserSuspendedUnsuspended)
            {
                this.Get<ILogger>().Log(
                    this.PageContext.PageUserID,
                    "YAF.Controls.EditUsersSuspend",
                    $"User {this.User.DisplayOrUserName()} was unsuspended by {this.PageContext.User.DisplayOrUserName()}.",
                    EventLogTypes.UserUnsuspended);
            }

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            this.Get<ISendNotification>().SendUserSuspensionEndedNotification(
                this.User.Email,
                this.User.DisplayOrUserName());

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
            var access = this.GetRepository<vaccess>().GetSingle(v => v.UserID == this.CurrentUserID);

            // is user to be suspended admin?
            if (access.IsAdmin)
            {
                // tell user he can't suspend admin
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_ADMINISTRATORS"), MessageTypes.danger);
                return;
            }

            // is user to be suspended forum moderator, while user suspending him is not admin?
            if (!this.PageContext.IsAdmin && access.IsForumModerator)
            {
                // tell user he can't suspend forum moderator when he's not admin
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_FORUMMODERATORS"), MessageTypes.danger);
                return;
            }

            var isGuest = this.User.IsGuest;

            // verify the user isn't guest...
            if (isGuest.HasValue && isGuest.Value)
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_GUESTACCOUNT"), MessageTypes.danger);
            }

            // time until when user is suspended
            var suspend = this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow, this.User.TimeZoneInfo);

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

            this.Get<ILogger>().Log(
                this.PageContext.PageUserID,
                "YAF.Controls.EditUsersSuspend",
                $"User {this.User.DisplayOrUserName()} was suspended by {this.PageContext.User.DisplayOrUserName()} until: {suspend} (UTC)",
                EventLogTypes.UserSuspended);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            this.Get<ISendNotification>().SendUserSuspensionNotification(
                suspend,
                this.SuspendedReason.Text.Trim(),
                this.User.Email,
                this.User.DisplayOrUserName());


            this.SuspendedReason.Text = string.Empty;

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // is user suspended?
            if (!this.User.Suspended.HasValue)
            {
                this.SuspendedHolder.Visible = false;

                // if user is not suspended, hide row with suspend information and remove suspension button
                return;
            }

            this.CurrentSuspendedReason.Text = this.User.SuspendedReason;

            var suspendedByUser = this.GetRepository<User>().GetById(this.User.SuspendedBy);

            this.SuspendedBy.UserID = suspendedByUser.ID;
            this.SuspendedBy.Suspended = suspendedByUser.Suspended;
            this.SuspendedBy.Style = suspendedByUser.UserStyle;
            this.SuspendedBy.ReplaceName = suspendedByUser.DisplayOrUserName();
        }

        #endregion
    }
}