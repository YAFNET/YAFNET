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

using System.Linq;
using System.Threading.Tasks;

using YAF.Types.Objects.Model;

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Admin Restore Topics Page
/// </summary>
public class RestoreModel : AdminPage
{
    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>The filter.</value>
    [BindProperty]
    public string Filter { get; set; }

    /// <summary>
    /// Gets or sets the deleted topics.
    /// </summary>
    /// <value>The deleted topics.</value>
    [BindProperty]
    public List<PagedTopic> DeletedTopics { get; set; }

    /// <summary>
    /// Gets or sets the deleted messages.
    /// </summary>
    /// <value>The deleted messages.</value>
    [BindProperty]
    public List<PagedMessage> DeletedMessages { get; set; }

    /// <summary>
    /// Gets or sets the size of the messages page.
    /// </summary>
    /// <value>The size of the messages page.</value>
    [BindProperty]
    public int MessagesPageSize { get; set; }

    /// <summary>
    /// Gets or sets the pageSize List.
    /// </summary>
    public SelectList MessagesPageSizeList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestoreModel"/> class.
    /// </summary>
    public RestoreModel()
        : base("ADMIN_RESTORE", ForumPages.Admin_Restore)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RESTORE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    public void OnGet(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    public void OnPost(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// The refresh click.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    public void OnPostRefresh(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// Restore topic.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostRestoreTopicAsync(int p, int p2, int topicId, int forumId)
    {
        var getFirstMessage = await this.GetRepository<Message>()
            .GetSingleAsync(m => m.TopicID == topicId && m.Position == 0);

        if (getFirstMessage != null)
        {
            await this.GetRepository<Message>().RestoreAsync(
                forumId,
                topicId,
                getFirstMessage);
        }

        var topic = await this.GetRepository<Topic>().GetByIdAsync(topicId);

        var flags = topic.TopicFlags;

        flags.IsDeleted = false;

        await this.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic { Flags = flags.BitValue },
            t => t.ID == topicId);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);
    }

    /// <summary>
    /// Delete topic.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteTopicAsync(int p, int p2, int topicId, int forumId)
    {
        await this.GetRepository<Topic>().DeleteAsync(forumId, topicId, true);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// Delete all topics.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAllTopicsAsync(int p, int p2)
    {
        var deletedTopics = await this.GetRepository<Topic>()
            .GetDeletedTopicsAsync(this.PageBoardContext.PageBoardID, this.Filter);

        foreach (var x in deletedTopics.Select(x => x.Item2))
        {
            await this.GetRepository<Topic>().DeleteAsync(x.ForumID, x.ID, true);
        }

        this.BindData(0, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// Delete all topics with no posts.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteZeroTopicsAsync(int p, int p2)
    {
        var deletedTopics = await this.GetRepository<Topic>().GetAsync(t => (t.Flags & 8) == 8 && t.NumPosts.Equals(0));

        foreach (var topic in deletedTopics)
        {
            await this.GetRepository<Topic>().DeleteAsync(topic.ForumID, topic.ID, true);
        }

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// Restore post.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostRestorePostAsync(int p, int p2, int topicId, int forumId, int messageId)
    {
        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

        await this.GetRepository<Message>().RestoreAsync(
            forumId,
            topicId,
            message);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);
    }

    /// <summary>
    /// Delete post.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeletePostAsync(int p, int p2, int topicId, int forumId, int messageId)
    {
        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

        // delete message
        await this.GetRepository<Message>().DeleteAsync(
            forumId,
            topicId,
            message,
            true,
            string.Empty,
            true,
            true);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// Delete all posts.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAllPostsAsync(int p, int p2)
    {
        var messages = this.GetRepository<Message>()
            .GetDeletedMessages(this.PageBoardContext.PageBoardID);

        foreach (var x in messages)
        {
            await this.GetRepository<Message>().DeleteAsync(
                x.Item1.ID,
                x.Item3.TopicID,
                x.Item3,
                true,
                string.Empty,
                true,
                true);
        }

        this.BindData(p, 0);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="p">The topics page index.</param>
    /// <param name="p2">The messages page index.</param>
    private void BindData(int p, int p2)
    {
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.MessagesPageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.MessagesPageSize = this.PageBoardContext.PageUser.PageSize;

        this.BindDeletedTopics(p);

        this.BindDeletedMessages(p2);
    }

    /// <summary>
    /// Binds the deleted topics.
    /// </summary>
    /// <param name="p">
    /// The page index.
    /// </param>
    private void BindDeletedTopics(int p)
    {
        var deletedTopics =
            this.GetRepository<Topic>().GetDeletedTopicsPaged(this.PageBoardContext.PageBoardID, this.Filter, p - 1, this.Size);

        this.DeletedTopics = deletedTopics;
    }

    /// <summary>
    /// Bind deleted Messages.
    /// </summary>
    /// <param name="p2">
    /// The page index.
    /// </param>
    private void BindDeletedMessages(int p2)
    {
        var deletedMessages = this.GetRepository<Message>()
            .GetDeletedMessagesPaged(this.PageBoardContext.PageBoardID, p2 - 1, this.MessagesPageSize);

        this.DeletedMessages = deletedMessages;
    }
}