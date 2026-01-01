
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
            this.GetText("TEAM", "FORUMS"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int fa)
    {
        this.Forum = this.GetRepository<Forum>().GetById(fa);

        if (this.Forum is null)
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

        var newForumId = this.ForumListSelected;

        if (this.MoveTopics && newForumId != fa)
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

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }
}