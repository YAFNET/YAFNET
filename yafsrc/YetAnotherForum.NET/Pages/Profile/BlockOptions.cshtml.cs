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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Threading.Tasks;

namespace YAF.Pages.Profile;

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

/// <summary>
/// PageUser Page To Manage PageUser Block Option and handle Ignored Users
/// </summary>
public class BlockOptionsModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "BlockOptionsModel" /> class.
    /// </summary>
    public BlockOptionsModel()
        : base("BLOCK_OPTIONS", ForumPages.Profile_BlockOptions)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether block private messages.
    /// </summary>
    [BindProperty]
    public bool BlockPMs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether block emails.
    /// </summary>
    [BindProperty]
    public bool BlockEmails { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether block friend requests.
    /// </summary>
    [BindProperty]
    public bool BlockFriendRequests { get; set; }

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    public List<User> Users { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("BLOCK_OPTIONS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// The on post.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var blockFlags = new UserBlockFlags
                         {
                             BlockEmails = this.BlockEmails,
                             BlockFriendRequests = this.BlockFriendRequests,
                             BlockPMs = this.BlockPMs
                         };

        await this.GetRepository<User>().UpdateBlockFlagsAsync(this.PageBoardContext.PageUserID, blockFlags.BitValue);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// The on post delete.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    public async Task<IActionResult> OnPostDeleteAsync(int userId)
    {
        await this.Get<IUserIgnored>().RemoveIgnoredAsync(userId);

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.BlockPMs = this.PageBoardContext.PageUser.Block.BlockPMs;
        this.BlockFriendRequests = this.PageBoardContext.PageUser.Block.BlockFriendRequests;
        this.BlockEmails = this.PageBoardContext.PageUser.Block.BlockEmails;

        this.Users = await this.GetRepository<IgnoreUser>().IgnoredUsersAsync(this.PageBoardContext.PageUserID);
    }
}