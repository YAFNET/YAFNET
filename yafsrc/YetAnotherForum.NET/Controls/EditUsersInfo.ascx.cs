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
    
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
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
        protected int CurrentUserID => this.PageContext.QueryIDs["u"].ToType<int>();

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
                var user = this.Get<IAspNetUsersHelper>().GetUserByName(this.Name.Text.Trim());

                var userName = this.Get<IAspNetUsersHelper>().GetUserByEmail(this.Email.Text.Trim()).UserName;
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

                this.Get<IAspNetUsersHelper>().Update(user);
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
                                    IsApproved = this.IsApproved.Checked,
                                    Moderated = this.Moderated.Checked
                                };

            this.GetRepository<User>().AdminSave(
                this.PageContext.PageBoardID,
                this.CurrentUserID,
                this.Name.Text.Trim(),
                this.DisplayName.Text.Trim(),
                this.Email.Text.Trim(),
                userFlags.BitValue,
                this.RankID.SelectedValue.ToType<int>());

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.RankID.DataSource = this.GetRepository<Rank>().GetByBoardId();
            this.RankID.DataValueField = "ID";
            this.RankID.DataTextField = "Name";
            this.RankID.DataBind();

            using (var dt = this.GetRepository<User>().ListAsDataTable(this.PageContext.PageBoardID, this.CurrentUserID, null))
            {
                var row = dt.GetFirstRow();
                var userFlags = new UserFlags(row["Flags"]);

                this.Name.Text = row.Field<string>("Name");
                this.DisplayName.Text = row.Field<string>("DisplayName");
                this.Email.Text = row.Field<string>("Email");
                this.IsHostAdminX.Checked = userFlags.IsHostAdmin;
                this.IsApproved.Checked = userFlags.IsApproved;
                this.IsGuestX.Checked = userFlags.IsGuest;
                this.IsCaptchaExcluded.Checked = userFlags.IsCaptchaExcluded;
                this.IsExcludedFromActiveUsers.Checked = userFlags.IsActiveExcluded;
                this.Moderated.Checked = userFlags.Moderated;
                this.Joined.Text = row["Joined"].ToString();
                this.IsFacebookUser.Checked = row.Field<bool>("IsFacebookUser");
                this.IsTwitterUser.Checked = row.Field<bool>("IsTwitterUser");
                this.IsGoogleUser.Checked = row.Field<bool>("IsGoogleUser");
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