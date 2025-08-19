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

namespace YAF.Pages.Moderate;

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Base root control for moderating, linking to other moderating controls/pages.
/// </summary>
public class ReportedPostsModel : ModerateForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReportedPostsModel"/> class.
    /// </summary>
    public ReportedPostsModel()
        : base("MODERATE_FORUM", ForumPages.Moderate_ReportedPosts)
    {
    }

    /// <summary>
    /// Gets or sets the reported.
    /// </summary>
    public List<ReportedMessage> Reported { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // moderation index
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("MODERATE_DEFAULT", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Moderate_Moderate));

        // current page
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageForum.Name);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="f">
    /// The forum Id.
    /// </param>
    public Task<IActionResult> OnGetAsync(int f)
    {
        // bind data
        return this.BindDataAsync(f);
    }

    /// <summary>
    /// The on post copy over.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public async Task<IActionResult> OnPostCopyOverAsync(int messageId)
    {
        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

        // update message text
        await this.GetRepository<MessageReported>().ReportCopyOverAsync(message);

        // bind data
        return await this.BindDataAsync(this.PageBoardContext.PageForumID);
    }

    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    public async Task<IActionResult> OnPostDeleteAsync(int messageId, int topicId)
    {
        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

        await this.GetRepository<Message>().DeleteAsync(
            this.PageBoardContext.PageForumID,
            topicId,
            message,
            true,
            string.Empty,
            true,
            true);

        // tell user message was deleted
        this.PageBoardContext.SessionNotify(this.GetText("DELETED"), MessageTypes.info);

        // bind data
        return await this.BindDataAsync(this.PageBoardContext.PageForumID);
    }

    /// <summary>
    /// Mark message as resolved
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public async Task<IActionResult> OnPostResolvedAsync(int messageId)
    {
        await this.GetRepository<Message>().ReportResolveAsync(messageId, this.PageBoardContext.PageUserID);

        // tell user message was flagged as resolved
        this.PageBoardContext.SessionNotify(this.GetText("RESOLVEDFEEDBACK"), MessageTypes.success);

        // bind data
        return await this.BindDataAsync(this.PageBoardContext.PageForumID);
    }

    /// <summary>
    /// Go to the message
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostViewAsync(int messageId)
    {
        var topic = await this.GetRepository<Topic>().GetTopicFromMessageAsync(messageId);

        return this.Get<ILinkBuilder>().Redirect(
           ForumPages.Post,
            new { m = messageId, name = topic.TopicName });
    }

    /// <summary>
    /// Go to history page
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public IActionResult OnPostViewHistory(int messageId)
    {
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.MessageHistory,
            new { f = this.PageBoardContext.PageForumID, m = messageId });
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    /// <param name="f">
    /// The forum Id.
    /// </param>
    private async Task<IActionResult> BindDataAsync(int f)
    {
        // get reported posts for this forum
        var reported = await this.GetRepository<MessageReported>().ListReportedAsync(f);

        if (reported.Count == 0)
        {
            // nope -- redirect back to the moderate main...
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Moderate_Moderate);
        }

        this.Reported = reported;

        return this.Page();
    }
}