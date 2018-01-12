/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

    using System;
    using System.Data;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users info
    /// </summary>
    public partial class EditUsersInfo : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return this.PageContext.QueryIDs["u"].ToType<int>();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            this.IsHostAdminRow.Visible = this.PageContext.IsHostAdmin;

            if (this.IsPostBack)
            {
                return;
            }

            this.Save.Text = "<i class=\"fa fa-save fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "SAVE"));

            this.BindData();
        }

        /// <summary>
        /// Updates the User Info
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Update the Membership
            if (!this.IsGuestX.Checked)
            {
                var user = UserMembershipHelper.GetUser(this.Name.Text.Trim());

                var userName = this.Get<MembershipProvider>().GetUserNameByEmail(this.Email.Text.Trim());
                if (userName.IsSet() && userName != user.UserName)
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                    return;
                }

                if (this.Email.Text.Trim() != user.Email)
                {
                    // update the e-mail here too...
                    user.Email = this.Email.Text.Trim();
                }

                // Update IsApproved
                user.IsApproved = this.IsApproved.Checked;

                this.Get<MembershipProvider>().UpdateUser(user);
            }
            else
            {
                if (!this.IsApproved.Checked)
                {
                    this.PageContext.AddLoadMessage(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_GUEST_APPROVED"), MessageTypes.success);
                    return;
                }
            }

            var userFlags = new UserFlags
                                {
                                    IsHostAdmin = this.IsHostAdminX.Checked,
                                    IsGuest = this.IsGuestX.Checked,
                                    IsCaptchaExcluded = this.IsCaptchaExcluded.Checked,
                                    IsActiveExcluded = this.IsExcludedFromActiveUsers.Checked,
                                    IsApproved = this.IsApproved.Checked
                                };

            LegacyDb.user_adminsave(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                this.Name.Text.Trim(),
                this.DisplayName.Text.Trim(),
                this.Email.Text.Trim(),
                userFlags.BitValue,
                this.RankID.SelectedValue);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.RankID.DataSource = LegacyDb.rank_list(this.PageContext.PageBoardID, null);
            this.RankID.DataValueField = "RankID";
            this.RankID.DataTextField = "Name";
            this.RankID.DataBind();

            using (var dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, null))
            {
                var row = dt.Rows[0];
                var userFlags = new UserFlags(row["Flags"]);

                this.Name.Text = (string)row["Name"];
                this.DisplayName.Text = row.Field<string>("DisplayName");
                this.Email.Text = row["Email"].ToString();
                this.IsHostAdminX.Checked = userFlags.IsHostAdmin;
                this.IsApproved.Checked = userFlags.IsApproved;
                this.IsGuestX.Checked = userFlags.IsGuest;
                this.IsCaptchaExcluded.Checked = userFlags.IsCaptchaExcluded;
                this.IsExcludedFromActiveUsers.Checked = userFlags.IsActiveExcluded;
                this.Joined.Text = row["Joined"].ToString();
                this.IsFacebookUser.Checked = row["IsFacebookUser"].ToType<bool>();
                this.IsTwitterUser.Checked = row["IsTwitterUser"].ToType<bool>();
                this.IsGoogleUser.Checked = row["IsGoogleUser"].ToType<bool>();
                this.LastVisit.Text = row["LastVisit"].ToString();
                var item = this.RankID.Items.FindByValue(row["RankID"].ToString());

                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        #endregion
    }
}