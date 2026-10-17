/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    public void BindData(int forumId, int? userId)
    {
        this.ForumId = forumId;
        this.UserId = userId;

        // setup data source for access masks dropdown (only admins use the dialog)
        this.AccessMaskID.DataSource = this.GetRepository<AccessMask>().GetByBoardId();
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
    protected void Page_Load(object sender, EventArgs e)
    {
        // only admins change the moderators of the forum (the list of the moderators is shown to them only)
        if (!this.PageBoardContext.IsAdmin)
        {
            this.Visible = false;
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.SelectUsersLoadJs),
            JavaScriptBlocks.SelectUsersLoadJs(
                "UserSelect",
                this.SelectedUserID.ClientID,
                this.GetText("MOD_FORUMUSER", "FIND")));
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
    protected void UpdateClick(object sender, EventArgs e)
    {
        if (!this.PageBoardContext.IsAdmin)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        var boardId = this.PageBoardContext.PageBoardID;

        var userId = this.UserId ?? (int.TryParse(this.SelectedUserID.Value, out var selectedUserId) ? selectedUserId : 0);

        // no user was specified (the selected user must be one of the board)
        if (userId == 0 || !this.GetRepository<User>().Exists(u => u.ID == userId && u.BoardID == boardId))
        {
            this.PageBoardContext.Notify(this.GetText("MSG_VALID_USER"), MessageTypes.warning);
            return;
        }

        var accessMaskId = this.AccessMaskID.SelectedValue.ToType<int>();

        // the access mask must be one of the board
        if (!this.GetRepository<AccessMask>().Exists(a => a.ID == accessMaskId && a.BoardID == boardId))
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // save permission
        this.GetRepository<UserForum>().Save(userId, this.PageBoardContext.PageForumID, accessMaskId);

        // redirect to forum moderation page
        this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_Forums, new { f = this.PageBoardContext.PageForumID });
    }
}