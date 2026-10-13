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

using System.Threading.Tasks;

namespace YAF.Pages.Moderate;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Forum Moderating Page.
/// </summary>
public class ForumsModel : ModerateForumPage
{
    /// <summary>
    /// Gets or sets the user list.
    /// </summary>
    /// <value>The user list.</value>
    [BindProperty]
    public List<Tuple<User, UserForum, AccessMask>> UserList { get; set; }

    /// <summary>
    /// Gets or sets the topic list.
    /// </summary>
    /// <value>The topic list.</value>
    [BindProperty]
    public List<PagedTopic> TopicList { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public ModerateForumsInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumsModel"/> class.
    /// </summary>
    public ForumsModel()
        : base("MODERATING", ForumPages.Moderate_Forums)
    {
    }

    /// <summary>
    /// Gets or sets the reported.
    /// </summary>
    public List<ReportedMessage> Reported { get; set; }

    /// <summary>
    /// Only moderators of the forum and admins, and the forum must exist (the page failed without a forum).
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        base.OnPageHandlerExecuting(context);

        if (context.Result is null && this.PageBoardContext.PageForum is null)
        {
            context.Result = this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddLink(this.GetText("MODERATE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="f">
    /// The forum Id.
    /// </param>
    public void OnGet(int f)
    {
        this.Input = new ModerateForumsInputModel();

        this.BindData();
    }

    /// <summary>
    /// Called when [get add user].
    /// </summary>
    /// <param name="f">The f.</param>
    /// <returns>PartialViewResult.</returns>
    public IActionResult OnGetAddUser(int f)
    {
        if (!this.PageBoardContext.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        return new PartialViewResult
               {
                   ViewName = "Dialogs/ModForumUser",
                   ViewData = new ViewDataDictionary<ModForumUserModal>(
                       this.ViewData,
                       new ModForumUserModal
                       {
                           ForumId = f
                       })
               };
    }

    /// <summary>
    /// Called when [get edit user].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="f">The f.</param>
    /// <returns>PartialViewResult.</returns>
    public IActionResult OnGetEditUser(int userId, int f)
    {
        if (!this.PageBoardContext.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        // Edit
        var userForum = this.GetRepository<UserForum>().List(userId, this.PageBoardContext.PageForumID).FirstOrDefault();

        if (userForum is null)
        {
            return this.NotFound();
        }

        return new PartialViewResult
               {
                   ViewName = "Dialogs/ModForumUser",
                   ViewData = new ViewDataDictionary<ModForumUserModal>(
                       this.ViewData,
                       new ModForumUserModal
                       {
                           ForumId = f,
                           UserID = userForum.Item1.ID,
                           UserName = userForum.Item1.DisplayOrUserName(),
                           AccessMaskID = userForum.Item2.AccessMaskID
                       })
               };
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    /// <param name="f">The f.</param>
    public void OnPost(int f)
    {
        this.BindData();
    }

    /// <summary>
    /// Called when [post remove user].
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="f">The f.</param>
    public void OnPostRemoveUser(int id, int f)
    {
        if (this.PageBoardContext.IsAdmin)
        {
            this.GetRepository<UserForum>().Delete(id, this.PageBoardContext.PageForumID);
        }

        this.BindData();
    }

    /// <summary>
    /// Called when [post move].
    /// </summary>
    /// <param name="f">The f.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostMoveAsync(int f)
    {
        // a pointer needs the number of days it is shown
        if (this.Input.LeavePointer && this.Input.LinkDays < 1)
        {
            // the page is shown again with the topics
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("POINTER_DAYS_INVALID"), MessageTypes.warning);
        }

        if (this.Input.ForumListSelected <= 0)
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("CANNOT_MOVE_TO_CATEGORY"), MessageTypes.warning);
        }

        // only move if it's a destination is a different forum.
        if (this.Input.ForumListSelected != this.PageBoardContext.PageForumID)
        {
            // ... the user moderates (of this board)
            if (!await this.GetRepository<ActiveAccess>().ExistsAsync(
                    x => x.UserID == this.PageBoardContext.PageUserID && x.ForumID == this.Input.ForumListSelected &&
                         (x.ModeratorAccess || x.IsAdmin)))
            {
                return this.Get<ILinkBuilder>().AccessDenied();
            }

            var list = this.SelectedForumTopics();

            if (list.NullOrEmpty())
            {
                this.BindData();
                return this.PageBoardContext.Notify(this.GetText("MODERATE", "NOTHING"), MessageTypes.warning);
            }

            foreach (var x in list)
            {
                await this.GetRepository<Topic>().MoveAsync(
                    x.TopicID,
                    this.PageBoardContext.PageForumID,
                    this.Input.ForumListSelected,
                    this.Input.LeavePointer,
                    this.Input.LinkDays);
            }

            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("MODERATE", "MOVED"), MessageTypes.success);
        }

        this.BindData();

        return this.PageBoardContext.Notify(this.GetText("MODERATE", "MOVE_TO_DIFFERENT"), MessageTypes.danger);
    }

    /// <summary>
    /// Deletes all the Selected Topics
    /// </summary>
    public async Task<IActionResult> OnPostDeleteTopicsAsync()
    {
        var list = this.SelectedForumTopics();

        if (list.NullOrEmpty())
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("MODERATE", "NOTHING"), MessageTypes.warning);
        }

        foreach (var x in list)
        {
            await this.GetRepository<Topic>().DeleteAsync(this.PageBoardContext.PageForumID, x.TopicID);
        }

        this.PageBoardContext.SessionNotify(this.GetText("moderate", "deleted"), MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Moderate_Forums, new { f = this.PageBoardContext.PageForumID });
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var baseSize = this.Size;
        var currentPageIndex = this.PageBoardContext.PageIndex;

        this.TopicList = this.GetRepository<Topic>().ListPaged(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageUserID,
            DateTimeHelper.SqlDbMinTime(),
            currentPageIndex,
            baseSize,
            this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);

        // the pages are changed by a post of the form: the inputs show the loaded page,
        // not the posted ids and selection of the page before
        foreach (var key in this.ModelState.Keys.Where(k => k.StartsWith("TopicList[", StringComparison.Ordinal)).ToList())
        {
            this.ModelState.Remove(key);
        }

        this.UserList = this.GetRepository<UserForum>().List(null, this.PageBoardContext.PageForumID);

        this.Input.ForumListSelected = 0;
    }

    /// <summary>
    /// Gets the selected topics, only the topics of the moderated forum (the list is sent by the client).
    /// </summary>
    /// <returns>Returns the selected topics</returns>
    private List<PagedTopic> SelectedForumTopics()
    {
        return [.. (this.TopicList ?? []).Where(
            x => x.Selected && this.GetRepository<Topic>().Exists(
                     t => t.ID == x.TopicID && t.ForumID == this.PageBoardContext.PageForumID))];
    }
}
