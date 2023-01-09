/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Admin Restore Topics Page
/// </summary>
public class RestoreModel : AdminPage
{
    [BindProperty]
    public string Filter { get; set; }

    [BindProperty]
    public IList<Tuple<Forum, Topic>> DeletedTopics { get; set; }

    [BindProperty]
    public IList<Tuple<Forum, Topic, Message>> DeletedMessages { get; set; }

    [BindProperty]
    public int MessagesPageSize { get; set; }

    [BindProperty]
    public int TopicsCount { get; set; }

    [BindProperty]
    public int MessagesCount { get; set; }

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
    public void OnGet(int p, int p2) 
            
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// The refresh click.
    /// </summary>
    public void OnPostRefresh(int p, int p2)
    {
        this.BindData(p, p2);
    }

    public IActionResult OnPostRestoreTopic(int p, int p2, int topicId, int forumId)
    {
        var getFirstMessage = this.GetRepository<Message>()
            .GetSingle(m => m.TopicID == topicId && m.Position == 0);

        if (getFirstMessage != null)
        {
            this.GetRepository<Message>().Restore(
                forumId,
                topicId,
                getFirstMessage);
        }

        var topic = this.GetRepository<Topic>().GetById(topicId);

        var flags = topic.TopicFlags;

        flags.IsDeleted = false;

        this.GetRepository<Topic>().UpdateOnly(
            () => new Topic { Flags = flags.BitValue },
            t => t.ID == topicId);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);
    }

    public IActionResult OnPostDeleteTopic(int p, int p2, int topicId, int forumId)
    {
        this.GetRepository<Topic>().Delete(forumId, topicId, true);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    public IActionResult OnPostDeleteAllTopics(int p, int p2)
    {
        var deletedTopics = this.GetRepository<Topic>()
            .GetDeletedTopics(this.PageBoardContext.PageBoardID, this.Filter);

        deletedTopics.ForEach(
            x => this.GetRepository<Topic>().Delete(x.Item2.ForumID, x.Item2.ID, true));

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    public IActionResult OnPostDeleteZeroTopics(int p, int p2)
    {
        var deletedTopics = this.GetRepository<Topic>().Get(t => (t.Flags & 8) == 8 && t.NumPosts.Equals(0));

        deletedTopics.ForEach(
            topic => this.GetRepository<Topic>().Delete(topic.ForumID, topic.ID, true));

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    public IActionResult OnPostRestorePost(int p, int p2, int topicId, int forumId, int messageId)
    {
        var message = this.GetRepository<Message>().GetById(messageId);

        this.GetRepository<Message>().Restore(
            forumId,
            topicId,
            message);

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);
    }

    public IActionResult OnPostDeletePost(int p, int p2, int topicId, int forumId, int messageId)
    {
        var message = this.GetRepository<Message>().GetById(messageId);

        // delete message
        this.GetRepository<Message>().Delete(
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

    public IActionResult OnPostDeleteAllPosts(int p, int p2)
    {
        var messages = this.GetRepository<Message>()
            .GetDeletedMessages(this.PageBoardContext.PageBoardID);

        messages.ForEach(
            x => this.GetRepository<Message>().Delete(
                x.Item1.ID,
                x.Item3.TopicID,
                x.Item3,
                true,
                string.Empty,
                true,
                true));

        this.BindData(p, p2);

        return this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
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

        // Bind Topics
        var deletedTopics =
            this.GetRepository<Topic>().GetDeletedTopics(this.PageBoardContext.PageBoardID, this.Filter);

        this.TopicsCount = deletedTopics.Count;

        var pagerTopics = new Paging {
                                         CurrentPageIndex = p,
                                         PageSize = this.Size,
                                         Count = deletedTopics.Count
                                     };

        var deletedTopicPaged = deletedTopics.GetPaged(pagerTopics);

        this.DeletedTopics = deletedTopicPaged;

        // Bind Messages
        var deletedMessages = this.GetRepository<Message>().GetDeletedMessages(this.PageBoardContext.PageBoardID);

        this.MessagesCount = deletedMessages.Count;

        var pagerMessages = new Paging {
                                           CurrentPageIndex = p2,
                                           PageSize = this.MessagesPageSize,
                                           Count = MessagesCount
                                       };

        var deletedMessagesPaged = deletedMessages.GetPaged(pagerMessages);

        this.DeletedMessages = deletedMessagesPaged;
    }
}