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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Utils;
    
    #endregion

    /// <summary>
    /// The Moderate Forum Dialog.
    /// </summary>
    public partial class ModForumUser : BaseUserControl
    {
        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        public int ForumId
        {
            get => this.ViewState["ForumId"].ToType<int>();

            set => this.ViewState["ForumId"] = value;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int? UserId
        {
            get => this.ViewState["UserId"].ToType<int?>();

            set => this.ViewState["UserId"] = value;
        }

        #region Methods

        /// <summary>
        /// The data bind.
        /// </summary>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        public void BindData([NotNull] int forumId, [CanBeNull] int? userId)
        {
            this.ForumId = forumId;
            this.UserId = userId;

            // load data
            IList<AccessMask> masks;

            // only admin can assign all access masks
            if (!this.PageContext.IsAdmin)
            {
                // do not include access masks with this flags set
                var flags = AccessFlags.Flags.ModeratorAccess.ToType<int>();

                // non-admins cannot assign moderation access masks
                masks = this.GetRepository<AccessMask>()
                    .Get(a => a.BoardID == this.PageContext.PageBoardID && a.Flags == flags);
            }
            else
            {
                masks = this.GetRepository<AccessMask>().GetByBoardId();
            }

            // setup data source for access masks dropdown
            this.AccessMaskID.DataSource = masks;
            this.AccessMaskID.DataValueField = "ID";
            this.AccessMaskID.DataTextField = "Name";
            this.AccessMaskID.DataBind();

            if (!this.UserId.HasValue)
            {
                this.UserName.Enabled = true;
                this.UserName.Text = string.Empty;

                this.FindUsers.Visible = true;

                return;
            }

            var userForum = this.GetRepository<UserForum>().List(this.UserId, this.ForumId).FirstOrDefault();

            if (userForum == null)
            {
                return;
            }

            // set username and disable its editing
            this.UserName.Text = this.PageContext.BoardSettings.EnableDisplayName
                ? userForum.Item1.DisplayName
                : userForum.Item1.Name;
            this.UserName.Enabled = false;

            // we don't need to find users now
            this.FindUsers.Visible = false;

            // get access mask for this user                
            if (this.AccessMaskID.Items.FindByValue(userForum.Item2.AccessMaskID.ToString()) != null)
            {
                this.AccessMaskID.Items.FindByValue(userForum.Item2.AccessMaskID.ToString()).Selected = true;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                this.Visible = false;
            }

            this.PageContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.SelectUsersLoadJs),
                JavaScriptBlocks.SelectUsersLoadJs(
                    this.ToList.ClientID,
                    this.FindUsers.ClientID,
                    this.UserName.ClientID));
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
        protected void UpdateClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // no user was specified
            if (this.UserName.Text.Length <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_SUCH_USER"), MessageTypes.warning);
                return;
            }

            // we need to verify user exists
            var userId = this.Get<IUserDisplayName>().GetId(this.UserName.Text.Trim());

            // there is no such user or reference is ambiguous
            if (!userId.HasValue)
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_SUCH_USER"), MessageTypes.warning);
                return;
            }

            if (this.Get<IAspNetUsersHelper>().IsGuestUser(userId))
            {
                this.PageContext.AddLoadMessage(this.GetText("NOT_GUEST"), MessageTypes.warning);
                return;
            }

            // save permission
            this.GetRepository<UserForum>().Save(
                userId.Value,
                this.PageContext.PageForumID,
                this.AccessMaskID.SelectedValue.ToType<int>());

            // clear moderators cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardModerators);

            // redirect to forum moderation page
            BuildLink.Redirect(ForumPages.Moderate_Forums, "f={0}", this.PageContext.PageForumID);
        }

        #endregion
    }
}