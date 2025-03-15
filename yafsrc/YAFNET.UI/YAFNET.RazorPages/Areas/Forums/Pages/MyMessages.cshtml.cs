/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// The Private Message Page
/// </summary>
public class MyMessagesModel : ForumPageRegistered
{
    /// <summary>
    /// Gets or sets the private messages.
    /// </summary>
    /// <value>The private messages.</value>
    [BindProperty]
    public IList<PrivateMessage> PrivateMessages { get; set; }

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>The users.</value>
    [BindProperty]
    public List<User> Users { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MyMessagesModel" /> class.
    /// </summary>
    public MyMessagesModel()
        : base("PM", ForumPages.MyMessages)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int? u = null)
    {
        // check if this feature is disabled
        if (!this.PageBoardContext.BoardSettings.AllowPrivateMessages)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Disabled);
        }

        return await this.BindDataAsync(u);
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("PM","TITLE"));
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task<IActionResult> BindDataAsync(int? u = null)
    {
        this.PrivateMessages = [];

        this.Users = [];

        this.Users = await this.GetRepository<PrivateMessage>().GetUserListAsync(this.PageBoardContext.PageUserID);

        User conversationUser;

        if (u.HasValue)
        {
            conversationUser = await this.GetRepository<User>().GetByIdAsync(u.Value);

            if (conversationUser != null && conversationUser.ID != this.PageBoardContext.PageUserID && !conversationUser.Block.BlockPMs)
            {
                return this.OpenUserChat(conversationUser);
            }
        }

        // Get last conversation ?!
        conversationUser = await this.GetRepository<PrivateMessage>().GetLatestConversationUserAsync(this.PageBoardContext.PageUserID);

        if (conversationUser != null && conversationUser.ID != this.PageBoardContext.PageUserID)
        {
            // Open existing with user or open new conversation
            return this.OpenUserChat(conversationUser);
        }

        return this.Users.NullOrEmpty() ? this.Get<ILinkBuilder>().Redirect(ForumPages.MyAccount) :
                   // If no user is selected open
                   this.OpenUserChat(this.Users[^1]);
    }

    /// <summary>
    /// Opens the user chat with the conversation user
    /// </summary>
    /// <param name="conversationUser">The conversation user.</param>
    /// <returns>PageResult.</returns>
    private PageResult OpenUserChat(User conversationUser)
    {
        // Open existing with user or open new conversation
        if (this.Users.Exists(x => x.ID == conversationUser.ID))
        {
            this.Users.Find(x => x.ID == conversationUser.ID).Selected = true;
        }
        else
        {
            conversationUser.Selected = true;
            this.Users.Add(conversationUser);
        }

        return this.Page();
    }
}