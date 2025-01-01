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
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// Moderating Page for Unapproved Posts.
/// </summary>
public class UnapprovedPostsModel : ModerateForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnapprovedPostsModel"/> class.
    /// </summary>
    public UnapprovedPostsModel()
        : base("MODERATE_FORUM", ForumPages.Moderate_UnapprovedPosts)
    {
    }

    /// <summary>
    /// Gets or sets the messages.
    /// </summary>
    public List<Tuple<Topic, Message, User>> Messages { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // moderation index
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("MODERATE_DEFAULT", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Moderate_Moderate));

        // current page
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageForum.Name);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="f">
    /// The forum Id.
    /// </param>
    public IActionResult OnGet(int f)
    {
        // bind data
        return this.BindData(f);
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
    public IActionResult OnPostDelete(int messageId, int topicId)
    {
        var message = this.GetRepository<Message>().GetById(messageId);

        this.GetRepository<Message>().Delete(
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
        return this.BindData(this.PageBoardContext.PageForumID);
    }

    /// <summary>
    /// approve post
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public async Task<IActionResult> OnPostApproveAsync(int messageId)
    {
        this.GetRepository<Message>().Approve(
            messageId,
            this.PageBoardContext.PageForumID);

        // re-bind data
        this.BindData(this.PageBoardContext.PageForumID);

        // tell user message was approved
        this.PageBoardContext.SessionNotify(this.GetText("APPROVED"), MessageTypes.success);

        // send notification to watching users...
        await this.Get<ISendNotification>().ToWatchingUsersAsync(messageId);

        // bind data
        return this.BindData(this.PageBoardContext.PageForumID);
    }

    /// <summary>
    /// Format message.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Formatted string with escaped HTML markup and formatted.
    /// </returns>
    public string FormatMessage(Tuple<Topic, Message, User> item)
    {
        if (item.Item2.MessageFlags.NotFormatted)
        {
            // just encode it for HTML output
            return this.HtmlEncode(item.Item2.MessageText);
        }

        // fully format message
        return this.Get<IFormatMessage>().Format(
            item.Item2.ID,
            item.Item2.MessageText,
            item.Item2.MessageFlags,
            item.Item2.IsModeratorChanged.Value);
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    /// <param name="f">
    /// The forum Id.
    /// </param>
    private IActionResult BindData(int f)
    {
        // get reported posts for this forum
        var messages = this.GetRepository<Message>().Unapproved(f);

        if (messages.NullOrEmpty())
        {
            // nope -- redirect back to the moderate main...
            return this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_Moderate);
        }

        this.Messages = messages;

        return this.Page();
    }
}