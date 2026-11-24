
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

namespace YAF.Pages.Admin;

using System;
using System.Net;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Tasks;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// Administrative Page for the deleting of forum properties.
/// </summary>
public class DeleteForumModel : AdminPage
{
    /// <summary>
    /// Gets or sets a value indicating whether [move topics].
    /// </summary>
    /// <value><c>true</c> if [move topics]; otherwise, <c>false</c>.</value>
    [BindProperty]
    public bool MoveTopics { get; set; }

    /// <summary>
    /// Gets or sets the forum.
    /// </summary>
    /// <value>The forum.</value>
    [BindProperty]
    public Forum Forum { get; set; }

    /// <summary>
    /// Gets or sets the forum list selected.
    /// </summary>
    /// <value>The forum list selected.</value>
    [BindProperty]
    public int ForumListSelected { get; set; }

    /// <summary>
    /// Gets or sets the forum name typed in to confirm the delete (needed when the topics are deleted).
    /// </summary>
    [BindProperty]
    public string ConfirmName { get; set; }

    /// <summary>
    /// Gets or sets the number of sub forums (they are kept as forums without parent).
    /// </summary>
    public int SubForumCount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteForumModel"/> class.
    /// </summary>
    public DeleteForumModel()
        : base("ADMIN_DELETEFORUM", ForumPages.Admin_DeleteForum)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMINMENU", "ADMIN_FORUMS"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int fa)
    {
        if (!this.LoadForum(fa))
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }

        this.ForumListSelected = this.Forum.ID;

        return this.Page();
    }

    /// <summary>
    /// Delete The Forum
    /// </summary>
    public async Task<IActionResult> OnPostAsync(int fa)
    {
        string errorMessage;

        // the page is shown again with the forum (it is not sent with the form)
        if (!this.LoadForum(fa))
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }

        var newForumId = this.ForumListSelected;
        var moveTopics = this.MoveTopics && newForumId != fa;

        // the topics are only moved to a forum of the board (without a selected forum they were moved to the
        // forum 0 and lost)
        if (moveTopics && !this.IsBoardForum(newForumId))
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_DELETEFORUM", "NEW_FORUM_HELP"), MessageTypes.warning);
        }

        // the topics are deleted only when the name of the forum is typed in (the same check as on the page)
        if (!moveTopics && !string.Equals(this.ConfirmName?.Trim(), this.Forum.Name.Trim(), StringComparison.Ordinal))
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_DELETEFORUM", "NAME_MISMATCH"), MessageTypes.danger);
        }

        if (moveTopics)
        {
            // schedule...
            ForumDeleteTask.Start(this.PageBoardContext.PageBoardID, fa, newForumId, out errorMessage);
        }
        else
        {
            // schedule...
            ForumDeleteTask.Start(this.PageBoardContext.PageBoardID, fa, out errorMessage);
        }

        if (errorMessage.IsSet())
        {
            return this.PageBoardContext.Notify(errorMessage, MessageTypes.danger);
        }

        while (this.Get<ITaskModuleManager>().IsTaskRunning(nameof(ForumDeleteTask)))
        {
            await Task.Delay(4000);
        }

        // the notification shows the message as html
        return this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("FORUM_DELETED", WebUtility.HtmlEncode(this.Forum.Name)),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Loads the forum and the number of its sub forums.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns><c>true</c> if the forum exists.</returns>
    private bool LoadForum(int forumId)
    {
        this.Forum = this.GetRepository<Forum>().GetById(forumId);

        if (this.Forum is null)
        {
            return false;
        }

        this.SubForumCount = (int)this.GetRepository<Forum>().Count(f => f.ParentID == forumId);

        return true;
    }

    /// <summary>
    /// Only forums of the current board can be deleted, the topics can only be moved to a forum of the board (all handlers, the id is sent by the client).
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        var id = context.HandlerArguments.TryGetValue("fa", out var argument) && argument is int argumentId ? argumentId : 0;

        if (id > 0 && !this.IsBoardForum(id) || this.ForumListSelected > 0 && !this.IsBoardForum(this.ForumListSelected))
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Checks if the forum belongs to the current board.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns>Returns if the forum belongs to the current board.</returns>
    private bool IsBoardForum(int forumId)
    {
        var forum = this.GetRepository<Forum>().GetById(forumId);

        return forum != null && this.GetRepository<Category>().Exists(
                   c => c.ID == forum.CategoryID && c.BoardID == this.PageBoardContext.PageBoardID);
    }
}