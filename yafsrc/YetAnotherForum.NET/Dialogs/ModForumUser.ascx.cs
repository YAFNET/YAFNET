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

namespace YAF.Dialogs;

using YAF.Types.Models;

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
        if (!this.PageBoardContext.IsAdmin)
        {
            // non-admins cannot assign moderation access masks
            masks = this.GetRepository<AccessMask>()
                .Get(a => a.BoardID == this.PageBoardContext.PageBoardID && a.AccessFlags.ModeratorAccess);
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
            this.UserSelectHolder.Visible = true;
            this.UserName.Visible = false;

            return;
        }

        var userForum = this.GetRepository<UserForum>().List(this.UserId, this.ForumId).FirstOrDefault();

        if (userForum == null)
        {
            return;
        }

        // set username and disable its editing
        this.UserName.Text = userForum.Item1.DisplayOrUserName();
        this.UserName.Enabled = false;

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
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            this.Visible = false;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.SelectUsersLoadJs),
            JavaScriptBlocks.SelectUsersLoadJs(
                "ModForumUserDialog",
                "UserSelect",
                this.SelectedUserID.ClientID));
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

        if (this.UserId.HasValue)
        {
            // save permission
            this.GetRepository<UserForum>().Save(
                this.UserId.Value,
                this.PageBoardContext.PageForumID,
                this.AccessMaskID.SelectedValue.ToType<int>());
        }
        else
        {
            var userId = this.SelectedUserID.Value.ToType<int>();

            // no user was specified
            if (userId == 0)
            {
                this.PageBoardContext.Notify(this.GetText("MSG_VALID_USER"), MessageTypes.warning);
                return;
            }

            // save permission
            this.GetRepository<UserForum>().Save(
                userId,
                this.PageBoardContext.PageForumID,
                this.AccessMaskID.SelectedValue.ToType<int>());
        }

        // redirect to forum moderation page
        this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_Forums, new { f = this.PageBoardContext.PageForumID });
    }
}