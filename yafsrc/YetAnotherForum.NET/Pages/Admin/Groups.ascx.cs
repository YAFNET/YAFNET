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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Primary administrator interface for groups/roles editing.
    /// </summary>
    public partial class Groups : AdminPage
    {
        #region Constants and Fields

        /// <summary>
        ///   Temporary storage of un-linked provider roles.
        /// </summary>
        private readonly StringCollection availableRoles = new StringCollection();

        #endregion

        #region Methods

        /// <summary>
        /// Format string color.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Set values  are rendered green if true, and if not red
        /// </returns>
        protected string GetItemColorString(string item)
        {
            // show enabled flag red
            return item.IsSet() ? "badge badge-success" : "badge badge-danger";
        }

        /// <summary>
        /// Format access mask setting color formatting.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Set access mask flags  are rendered green if true, and if not red
        /// </returns>
        protected string GetItemColor(bool enabled)
        {
            // show enabled flag red
            return enabled ? "badge badge-success" : "badge badge-danger";
        }

        /// <summary>
        /// Get a user friendly item name.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Item Name.
        /// </returns>
        protected string GetItemName(bool enabled)
        {
            return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // admin index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            // roles
            this.PageLinks.AddLink(this.GetText("ADMIN_GROUPS", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_GROUPS", "TITLE")}";
        }

        /// <summary>
        /// Get status of provider role vs YAF roles.
        /// </summary>
        /// <param name="currentRow">
        /// Data row which contains data about role.
        /// </param>
        /// <returns>
        /// String "Linked" when role is linked to YAF roles, "Un-linkable" otherwise.
        /// </returns>
        [NotNull]
        protected string GetLinkedStatus([NotNull] Group currentRow)
        {
            // check whether role is Guests role, which can't be linked
            return currentRow.Flags.BinaryAnd(2)
                       ? this.GetText("ADMIN_GROUPS", "UNLINKABLE")
                       : this.GetText("ADMIN_GROUPS", "LINKED");
        }

        /// <summary>
        /// Handles click on new role button
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void NewGroupClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to new role page
            BuildLink.Redirect(ForumPages.Admin_EditGroup);
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

            // sync roles just in case...
            RoleMembershipHelper.SyncRoles(BoardContext.Current.PageBoardID);

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Handles provider roles adding/deleting.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RoleListNetItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // detect which command are we handling
            switch (e.CommandName)
            {
                case "add":

                    // save role and get its ID
                    const int InitialPMessages = 0;

                    var groupId = this.GetRepository<Group>().Save(
                        DBNull.Value,
                        this.PageContext.PageBoardID,
                        e.CommandArgument.ToString(),
                        false,
                        false,
                        false,
                        false,
                        1,
                        InitialPMessages,
                        null,
                        100,
                        null,
                        0,
                        null,
                        null,
                        0,
                        0);

                    // redirect to newly created role
                    BuildLink.Redirect(ForumPages.Admin_EditGroup, "i={0}", groupId);
                    break;
                case "delete":

                    // delete role from provider data
                    RoleMembershipHelper.DeleteRole(e.CommandArgument.ToString(), false);

                    // re-bind data
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Handles role editing/deletion buttons.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RoleListYafItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // detect which command are we handling
            switch (e.CommandName)
            {
                case "edit":

                    // go to role editing page
                    BuildLink.Redirect(ForumPages.Admin_EditGroup, "i={0}", e.CommandArgument);
                    break;
                case "delete":

                    var groupId = e.CommandArgument.ToType<int>();

                    // delete role
                    this.GetRepository<GroupMedal>().Delete(g => g.GroupID == groupId);
                    this.GetRepository<ForumAccess>().Delete(g => g.GroupID == groupId);
                    this.GetRepository<UserGroup>().Delete(g => g.GroupID == groupId);
                    this.GetRepository<Group>().Delete(g => g.ID == groupId);

                    // remove cache of forum moderators
                    this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

                    // re-bind data
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // list roles of this board
            var dt = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);

            // set repeater data-source
            this.RoleListYaf.DataSource = dt;

            // clear cached list of roles
            this.availableRoles.Clear();

            // get all provider roles
            foreach (var role in from role in RoleMembershipHelper.GetAllRoles()
                                 let rows = dt.Select(g => g.Name == role)
                                 where dt.Count == 0
                                 select role)
            {
                // doesn't exist in the Yaf Groups
                this.availableRoles.Add(role);
            }

            // check if there are any roles for syncing
            if (this.availableRoles.Count > 0 && !Config.IsDotNetNuke)
            {
                // make it datasource
                this.RoleListNet.DataSource = this.availableRoles;
            }
            else
            {
                // no datasource for provider roles
                this.RoleListNet.DataSource = null;
            }

            // bind data to controls
            this.DataBind();
        }

        #endregion
    }
}