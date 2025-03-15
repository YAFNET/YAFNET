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
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Collections.Generic;
using System.Web;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Models;

/// <summary>
/// The Delete Message Page.
/// </summary>
public class DeleteMessageModel : ForumPage
{
    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    [BindProperty]
    public List<Message> LinkedPosts { get; set; }

    [BindProperty]
    public bool DeleteAllPosts { get; set; } = true;

    [BindProperty]
    public bool EraseMessage { get; set; }

    [BindProperty]
    public string ReasonEditor { get; set; }

    [BindProperty]
    public MessageFlags DeleteMessageFlags { get; set; }

    /// <summary>
    ///   The is moderator changed.
    /// </summary>
    private bool isModeratorChanged;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DeleteMessageModel" /> class.
    /// </summary>
    public DeleteMessageModel()
        : base("DELETEMESSAGE", ForumPages.DeleteMessage)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether CanDeletePost.
    /// </summary>
    public bool CanDeletePost =>
        (!this.PostLocked && !this.PageBoardContext.PageForum.ForumFlags.IsLocked
                          && !this.PageBoardContext.PageTopic.TopicFlags.IsLocked
                          && this.PageBoardContext.PageMessage.UserID == this.PageBoardContext.PageUserID
         || this.PageBoardContext.ForumModeratorAccess) && this.PageBoardContext.ForumDeleteAccess;

    /// <summary>
    ///   Gets a value indicating whether CanUnDeletePost.
    /// </summary>
    public bool CanUnDeletePost => this.PageBoardContext.PageMessage.MessageFlags.IsDeleted && this.CanDeletePost;

    /// <summary>
    ///   Gets a value indicating whether PostLocked.
    /// </summary>
    private bool PostLocked {
        get {
            if (this.PageBoardContext.IsAdmin || this.PageBoardContext.BoardSettings.LockPosts <= 0)
            {
                return false;
            }

            var edited = this.PageBoardContext.PageMessage.Edited ?? this.PageBoardContext.PageMessage.Posted;

            return edited.AddDays(this.PageBoardContext.BoardSettings.LockPosts) < DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? m)
    {
        if (!m.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.PageBoardContext.PageMessage.IsNullOrEmptyField())
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.isModeratorChanged = this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID;

        if (!this.PageBoardContext.ForumModeratorAccess && this.isModeratorChanged)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        // delete message...
        var replies = this.GetRepository<Message>().Replies(this.PageBoardContext.PageMessage.ID);

        if (replies.Count != 0 && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
        {
            this.LinkedPosts = replies;
        }

        this.ReasonEditor = this.PageBoardContext.PageMessage.DeleteReason;

        // populate the message preview with the message data-row...
        var messageFlags = this.PageBoardContext.PageMessage.MessageFlags;

        if (this.PageBoardContext.PageMessage.MessageFlags.IsDeleted)
        {
            // Override Delete Flag to show Message if Un-Delete action
            messageFlags.IsDeleted = false;
        }

        this.DeleteMessageFlags = messageFlags;

        // setup page links
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        return this.Page();
    }

    /// <summary>
    /// Delete Message(s)
    /// </summary>
    public IActionResult OnPostDelete()
    {
        if (!this.CanDeletePost)
        {
            return this.Page();
        }

        this.GetRepository<Message>().Delete(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageMessage.TopicID,
            this.PageBoardContext.PageMessage,
            this.isModeratorChanged,
            HttpUtility.HtmlEncode(this.ReasonEditor),
            this.DeleteAllPosts,
            this.EraseMessage);

        var topic = this.GetRepository<Topic>().GetById(this.PageBoardContext.PageMessage.TopicID);

        // If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
        if (topic is null)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Topics,
                new {f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name});
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Posts, new {t = topic.ID, name = topic.TopicName});
    }

    /// <summary>
    /// Restore Message(s).
    /// </summary>
    public IActionResult OnPostRestore()
    {
        this.GetRepository<Message>().Restore(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageMessage.TopicID,
            this.PageBoardContext.PageMessage);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Post,
            new {m = this.PageBoardContext.PageMessage.ID, name = this.PageBoardContext.PageTopic.TopicName});
    }
}