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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Control handling user invitations to forum (i.e. granting permissions by admin/moderator).
    /// </summary>
    public partial class mod_forumuser : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "mod_forumuser" /> class. 
        ///   Default constructor.
        /// </summary>
        public mod_forumuser()
            : base("MOD_FORUMUSER")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The data bind.
        /// </summary>
        public override void DataBind()
        {
            // load data
            IList<AccessMask> dataTable;

            // only admin can assign all access masks
            if (!this.PageContext.IsAdmin)
            {
                // do not include access masks with this flags set
                var flags = AccessFlags.Flags.ModeratorAccess.ToType<int>();

                // non-admins cannot assign moderation access masks
                dataTable = this.GetRepository<AccessMask>()
                    .Get(a => a.BoardID == this.PageContext.PageBoardID && a.Flags == flags);
            }
            else
            {
                dataTable = this.GetRepository<AccessMask>().GetByBoardId();
            }

            // setup datasource for access masks dropdown
            this.AccessMaskID.DataSource = dataTable;
            this.AccessMaskID.DataValueField = "ID";
            this.AccessMaskID.DataTextField = "Name";

            base.DataBind();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to forum moderation page
            YafBuildLink.Redirect(ForumPages.moderating, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            if (this.PageContext.Settings.LockedForum == 0)
            {
                // forum index
                this.PageLinks.AddRoot().AddCategory(
                    this.PageContext.PageCategoryName,
                    this.PageContext.PageCategoryID);
            }

            // forum page
            this.PageLinks.AddForum(this.PageContext.PageForumID);

            // currect page
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        /// <summary>
        /// Handles FindUsers button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void FindUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // we need at least two characters to search user by
            if (this.UserName.Text.Length < 2)
            {
                return;
            }

            // get found users
            var foundUsers = this.Get<IUserDisplayName>().Find(this.UserName.Text.Trim());

            // have we found anyone?
            if (foundUsers.Count > 0)
            {
                // set and enable user dropdown, disable text box
                this.ToList.DataSource = foundUsers;
                this.ToList.DataValueField = "Key";
                this.ToList.DataTextField = "Value";

                // ToList.SelectedIndex = 0;
                this.ToList.Visible = true;
                this.UserName.Visible = false;
                this.FindUsers.Visible = false;
            }

            // bind data (is this necessary?)
            base.DataBind();
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
            // only moderators/admins are allowed in
            if (!this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // do not repeat on postbact
            if (this.IsPostBack)
            {
                return;
            }

            // create page links
            this.CreatePageLinks();

            // load localized texts for buttons
            this.FindUsers.Text = this.GetText("FIND");
            this.Update.Text = this.GetText("UPDATE");
            this.Cancel.Text = this.GetText("CANCEL");

            // bind data
            this.DataBind();

            // if there is concrete user being handled
            if (this.Request.QueryString.GetFirstOrDefault("u") == null)
            {
                return;
            }

            using (var dt = LegacyDb.userforum_list(
                this.Request.QueryString.GetFirstOrDefault("u"),
                this.PageContext.PageForumID))
            {
                foreach (DataRow row in dt.Rows)
                {
                    // set username and disable its editing
                    this.UserName.Text = PageContext.BoardSettings.EnableDisplayName
                                             ? row["DisplayName"].ToString()
                                             : row["Name"].ToString();
                    this.UserName.Enabled = false;

                    // we don't need to find users now
                    this.FindUsers.Visible = false;

                    // get access mask for this user                
                    if (this.AccessMaskID.Items.FindByValue(row["AccessMaskID"].ToString()) != null)
                    {
                        this.AccessMaskID.Items.FindByValue(row["AccessMaskID"].ToString()).Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles click event of Update button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Update_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // no user was specified
            if (this.UserName.Text.Length <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_SUCH_USER"));
                return;
            }

            // if we choose user from drop down, set selected value to text box
            if (this.ToList.Visible)
            {
                this.UserName.Text = this.ToList.SelectedItem.Text;
            }

            // we need to verify user exists
            var userId = this.Get<IUserDisplayName>().GetId(this.UserName.Text.Trim());

            // there is no such user or reference is ambiugous
            if (!userId.HasValue)
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_SUCH_USER"));
                return;
            }

            if (UserMembershipHelper.IsGuestUser(userId))
            {
                this.PageContext.AddLoadMessage(this.GetText("NOT_GUEST"));
                return;
            }

            // save permission
            LegacyDb.userforum_save(userId.Value, this.PageContext.PageForumID, this.AccessMaskID.SelectedValue);

            // clear moderators cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardModerators);

            // redirect to forum moderation page
            YafBuildLink.Redirect(ForumPages.moderating, "f={0}", this.PageContext.PageForumID);

        }

        #endregion
    }
}