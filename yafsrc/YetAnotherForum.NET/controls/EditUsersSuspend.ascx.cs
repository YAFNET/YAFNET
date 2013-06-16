/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users suspend.
    /// </summary>
    public partial class EditUsersSuspend : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether ShowHeader.
        /// </summary>
        public bool ShowHeader
        {
            get
            {
                return this.ViewState["ShowHeader"] == null || Convert.ToBoolean(this.ViewState["ShowHeader"]);
            }

            set
            {
                this.ViewState["ShowHeader"] = value;
            }
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        protected long? CurrentUserID
        {
            get
            {
                return this.PageContext.QueryIDs["u"];
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
            // is there suspension expiration in the viewstate?
            if (this.ViewState["SuspendedUntil"] != null)
            {
                // return it
                return (string)this.ViewState["SuspendedUntil"];
            }

            return string.Empty;
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
            LegacyDb.user_suspend(this.CurrentUserID, null);
            var usr =
                LegacyDb.UserList(this.PageContext.PageBoardID,  (int?)this.CurrentUserID, null, null, null, false).ToList();

            if (usr.Any())
            {
                this.Get<ILogger>()
                    .Log(
                        this.PageContext.PageUserID,
                        "YAF.Controls.EditUsersSuspend",
                        "User {0} was unsuspended by {1}.".FormatWith(
                            this.Get<YafBoardSettings>().EnableDisplayName ? usr.First().DisplayName : usr.First().Name,
                            this.Get<YafBoardSettings>().EnableDisplayName
                                ? this.PageContext.CurrentUserData.DisplayName
                                : this.PageContext.CurrentUserData.UserName),
                        EventLogTypes.UserUnsuspended);
            }
           
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID.ToType<int>()));

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
            using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, null))
            {
                foreach (DataRow row in dt.Rows)
                {
                    // is user to be suspended admin?
                    if (row["IsAdmin"] != DBNull.Value && row["IsAdmin"].ToType<int>() > 0)
                    {
                        // tell user he can't suspend admin
                        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_ADMINISTRATORS"), MessageTypes.Error);
                        return;
                    }

                    // is user to be suspended forum moderator, while user suspending him is not admin?
                    if (!this.PageContext.IsAdmin && int.Parse(row["IsForumModerator"].ToString()) > 0)
                    {
                        // tell user he can't suspend forum moderator when he's not admin
                        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_FORUMMODERATORS"), MessageTypes.Error);
                        return;
                    }

                    object isGuest = row["IsGuest"];

                    // verify the user isn't guest...
                    if (isGuest != DBNull.Value && isGuest.ToType<int>() > 0)
                    {
                        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "ERROR_GUESTACCOUNT"), MessageTypes.Error);
                        return;
                    }
                }
            }

            // time until when user is suspended
            DateTime suspend = DateTime.UtcNow;

            // number inserted by suspending user
            int count = int.Parse(this.SuspendCount.Text);

            // what time units are used for suspending
            switch (this.SuspendUnit.SelectedValue)
            {
                    // days
                case "1":

                    // add user inserted suspension time to current time
                    suspend += new TimeSpan(count, 0, 0, 0);
                    break;

                    // hours
                case "2":

                    // add user inserted suspension time to current time
                    suspend += new TimeSpan(0, count, 0, 0);
                    break;

                    // minutes
                case "3":

                    // add user inserted suspension time to current time
                    suspend += new TimeSpan(0, 0, count, 0);
                    break;
            }

            // suspend user by calling appropriate method
            LegacyDb.user_suspend(this.CurrentUserID, suspend);
            var usr =
               LegacyDb.UserList(this.PageContext.PageBoardID, this.CurrentUserID.ToType<int?>(), null, null, null, false).ToList();

            if (usr.Any())
            {
                this.Get<ILogger>()
                    .Log(
                        this.PageContext.PageUserID,
                        "YAF.Controls.EditUsersSuspend",
                        "User {0} was suspended by {1} until: {2} (UTC)".FormatWith(
                            this.Get<YafBoardSettings>().EnableDisplayName ? usr.First().DisplayName : usr.First().Name,
                            this.Get<YafBoardSettings>().EnableDisplayName
                                ? this.PageContext.CurrentUserData.DisplayName
                                : this.PageContext.CurrentUserData.UserName,
                            suspend),
                        EventLogTypes.UserSuspended);
            }
            
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID.ToType<int>()));

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // get user's info
            using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, null))
            {
                // there is no such user
                if (dt.Rows.Count < 1)
                {
                    YafBuildLink.AccessDenied(/*No such user exists*/);
                }

                // get user's data in form of data row
                DataRow user = dt.Rows[0];

                // if user is not suspended, hide row with suspend information and remove suspension button
                this.SuspendedRow.Visible = !user.IsNull("Suspended");

                // is user suspended?
                if (!user.IsNull("Suspended"))
                {
                    // get time when his suspension expires to the view state
                    this.ViewState["SuspendedUntil"] = this.Get<IDateTime>().FormatDateTime(user["Suspended"]);

                    // localize remove suspension button
                    this.RemoveSuspension.Text = this.GetText("PROFILE", "REMOVESUSPENSION");
                }

                // localize suspend button
                this.Suspend.Text = this.GetText("PROFILE", "SUSPEND");
            }
        }

        #endregion
    }
}