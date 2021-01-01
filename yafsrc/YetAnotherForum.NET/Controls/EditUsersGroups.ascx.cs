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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The edit users groups.
    /// </summary>
    public partial class EditUsersGroups : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserID =>
            this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

        #endregion

        #region Methods

        /// <summary>
        /// Handles click on cancel button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to user admin page.
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
        }

        /// <summary>
        /// Checks if user is member of role or not depending on value of parameter.
        /// </summary>
        /// <param name="o">
        /// Parameter if 0, user is not member of a role.
        /// </param>
        /// <returns>
        /// True if user is member of role (o &gt; 0), false otherwise.
        /// </returns>
        protected bool IsMember([NotNull] object o)
        {
            return long.Parse(o.ToString()) > 0;
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
            // this needs to be done just once, not during post-backs
            if (this.IsPostBack)
            {
                return;
            }

            this.SendEmail.Text = this.GetText("ADMIN_EDITUSER", "SEND_EMAIL");

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Handles click on save button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var addedRoles = new List<string>();
            var removedRoles = new List<string>();

            // get user's name
            var user = this.Get<IAspNetUsersHelper>().GetMembershipUserById(this.CurrentUserID);

            // go through all roles displayed on page
            for (var i = 0; i < this.UserGroups.Items.Count; i++)
            {
                // get current item
                var item = this.UserGroups.Items[i];

                // get role ID from it
                var roleID = int.Parse(item.FindControlAs<Label>("GroupID").Text);

                // get role name
                var roleName = this.GetRepository<Group>().GetById(roleID).Name;

                // is user supposed to be in that role?
                var isChecked = item.FindControlAs<CheckBox>("GroupMember").Checked;

                // save user in role
                this.GetRepository<UserGroup>().AddOrRemove(this.CurrentUserID, roleID, isChecked);

                // update roles if this user isn't the guest
                if (this.Get<IAspNetUsersHelper>().IsGuestUser(this.CurrentUserID))
                {
                    continue;
                }

                // add/remove user from roles in membership provider
                if (isChecked && !this.Get<IAspNetRolesHelper>().IsUserInRole(user, roleName))
                {
                    this.Get<IAspNetRolesHelper>().AddUserToRole(user, roleName);

                    addedRoles.Add(roleName);
                }
                else if (!isChecked && this.Get<IAspNetRolesHelper>().IsUserInRole(user, roleName))
                {
                    this.Get<IAspNetRolesHelper>().RemoveUserFromRole(user.Id, roleName);

                    removedRoles.Add(roleName);
                }
            }

            this.GetRepository<User>().UpdateStyle(this.CurrentUserID);

            if (this.SendEmail.Checked)
            {
                // send notification to user
                if (addedRoles.Any())
                {
                    this.Get<ISendNotification>().SendRoleAssignmentNotification(user, addedRoles);
                }

                if (removedRoles.Any())
                {
                    this.Get<ISendNotification>().SendRoleUnAssignmentNotification(user, removedRoles);
                }
            }

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // get user roles
            this.UserGroups.DataSource = this.GetRepository<Group>().Member(
                this.PageContext.PageBoardID,
                this.CurrentUserID);

            // bind data to controls
            this.DataBind();
        }

        #endregion
    }
}