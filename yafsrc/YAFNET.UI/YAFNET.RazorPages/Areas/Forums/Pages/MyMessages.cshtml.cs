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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
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
    /// Gets or sets the number of unread messages of each conversation (by the user id of the sender).
    /// </summary>
    public Dictionary<int, int> UnreadCounts { get; set; } = [];

    /// <summary>
    /// Gets or sets the last message of each conversation (by the user id of the other user).
    /// </summary>
    public Dictionary<int, PrivateMessage> LastMessages { get; set; } = [];

    /// <summary>
    /// Gets the preview (plain text, shortened) of the last message of the conversation with the user.
    /// </summary>
    /// <param name="userId">The user id of the other user.</param>
    /// <returns>The preview or null if the conversation has no messages.</returns>
    public string GetLastMessagePreview(int userId)
    {
        if (!this.LastMessages.TryGetValue(userId, out var message) || message.Body.IsNotSet())
        {
            return null;
        }

        var text = BBCodeHelper.StripBBCode(HtmlTagHelper.StripHtml(message.Body)).Replace('\n', ' ').Trim();

        return text.Truncate(60);
    }

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
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("PM", "TITLE"));
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task<IActionResult> BindDataAsync(int? u = null)
    {
        this.PrivateMessages = [];

        this.Users = [];

        this.Users = await this.GetRepository<PrivateMessage>().GetUserListAsync(this.PageBoardContext.PageUserID);

        // the unread messages to the user (that the user not deleted) by the sender
        var unreadMessages = await this.GetRepository<PrivateMessage>().GetAsync(
            p => p.ToUserId == this.PageBoardContext.PageUserID && (p.Flags & 1) != 1 && (p.Flags & 4) != 4);

        this.UnreadCounts = unreadMessages.GroupBy(p => p.FromUserId).ToDictionary(g => g.Key, g => g.Count());

        await this.LoadLastMessagesAsync();

        // the conversations with unread messages first, then the latest conversation first
        this.Users =
        [
            .. this.Users.OrderByDescending(x => this.UnreadCounts.ContainsKey(x.ID))
                .ThenByDescending(x => this.LastMessages.TryGetValue(x.ID, out var message) ? message.ID : 0)
        ];

        User conversationUser;

        if (u.HasValue)
        {
            conversationUser = await this.GetRepository<User>().GetByIdAsync(u.Value);

            // only a member of the board (no guest, not deleted or unapproved) can be selected, the page shows
            // the name, avatar and last visit of the user,
            // admins and users in the friend list of the user are excluded from the blocking
            if (conversationUser != null && conversationUser.ID != this.PageBoardContext.PageUserID &&
                conversationUser.BoardID == this.PageBoardContext.PageBoardID && !conversationUser.UserFlags.IsGuest &&
                conversationUser.UserFlags.IsApproved && !conversationUser.UserFlags.IsDeleted &&
                (!conversationUser.Block.BlockPMs || this.PageBoardContext.IsAdmin || await this.GetRepository<Buddy>().ExistsAsync(
                     b => b.FromUserID == conversationUser.ID && b.ToUserID == this.PageBoardContext.PageUserID && b.Approved)))
            {
                return this.OpenUserChat(conversationUser);
            }
        }

        // Get last conversation ?!
        conversationUser = await this.GetRepository<PrivateMessage>()
            .GetLatestConversationUserAsync(this.PageBoardContext.PageUserID);

        if (conversationUser != null && conversationUser.ID != this.PageBoardContext.PageUserID)
        {
            // Open existing with user or open new conversation
            return this.OpenUserChat(conversationUser);
        }

        // no conversations yet, the page shows the button to start a new conversation
        return this.Users.NullOrEmpty()
            ? this.Page()
            :
            // If no user is selected open the latest conversation
            this.OpenUserChat(this.Users[0]);
    }

    /// <summary>
    /// Loads the last message of each conversation (that the user not deleted).
    /// </summary>
    private async Task LoadLastMessagesAsync()
    {
        var userId = this.PageBoardContext.PageUserID;

        var repository = this.GetRepository<PrivateMessage>();

        // the id of the last message by the user id of the other user
        var lastIds = await repository.DbAccess.ExecuteAsync(
            async db =>
            {
                var sent = await db.DictionaryAsync<int, int>(
                    db.From<PrivateMessage>().Where(p => p.FromUserId == userId && (p.Flags & 2) != 2)
                        .GroupBy(p => p.ToUserId).Select(p => new { p.ToUserId, Id = Sql.Max(p.ID) }));

                var received = await db.DictionaryAsync<int, int>(
                    db.From<PrivateMessage>().Where(p => p.ToUserId == userId && (p.Flags & 4) != 4)
                        .GroupBy(p => p.FromUserId).Select(p => new { p.FromUserId, Id = Sql.Max(p.ID) }));

                foreach (var (otherUserId, id) in received)
                {
                    if (!sent.TryGetValue(otherUserId, out var sentId) || sentId < id)
                    {
                        sent[otherUserId] = id;
                    }
                }

                return sent;
            });

        if (lastIds.Count == 0)
        {
            return;
        }

        var ids = lastIds.Values.ToList();

        var messages = await repository.GetAsync(p => Sql.In(p.ID, ids));

        this.LastMessages = messages.ToDictionary(p => p.FromUserId == userId ? p.ToUserId : p.FromUserId);
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
            // a new conversation is shown on top of the list
            conversationUser.Selected = true;
            this.Users.Insert(0, conversationUser);
        }

        return this.Page();
    }
}