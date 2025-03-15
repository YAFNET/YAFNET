
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

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// Move Message Page
/// </summary>
public class MoveMessageModel : ForumPageRegistered
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public MoveMessageInputModel Input { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MoveMessageModel" /> class.
    /// </summary>
    public MoveMessageModel()
        : base("MOVEMESSAGE", ForumPages.MoveMessage)
    {
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        this.PageBoardContext.PageLinks.AddLink(this.GetText("MOVE_MESSAGE"));
    }

    /// <summary>
    /// Handles the Click event of the CreateAndMove control.
    /// </summary>
    public IActionResult OnPostCreateAndMove()
    {
        if (this.Input.TopicSubject.IsNotSet())
        {
            return this.PageBoardContext.Notify(this.GetText("Empty_Topic"), MessageTypes.warning);
        }

        var topicId = this.GetRepository<Topic>().CreateByMessage(
            this.PageBoardContext.PageMessage.ID,
            this.Input.ForumListSelected,
            this.Input.TopicSubject);

        this.GetRepository<Message>().Move(
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageMessage,
            topicId.ToType<int>(),
            true);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Topics,
            new {f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name});
    }

    /// <summary>
    /// Handles the Click event of the Move control.
    /// </summary>
    public IActionResult OnPostMove()
    {
        var moveTopicId = this.Input.TopicListSelected;

        if (moveTopicId == 0 || moveTopicId == this.PageBoardContext.PageMessage.ID)
        {
            return this.PageBoardContext.Notify(this.GetText("MOVE_TITLE"), MessageTypes.warning);
        }

        this.GetRepository<Message>().Move(
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageMessage,
            moveTopicId,
            true);

        var topic = this.GetRepository<Topic>().GetById(moveTopicId);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Post,
            new {m = this.PageBoardContext.PageMessage.ID, name = topic.TopicName});
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Input = new MoveMessageInputModel {
                                        ForumListSelected = this.PageBoardContext.PageForumID
                                    };

        if (this.PageBoardContext.PageMessage is null || !this.PageBoardContext.ForumModeratorAccess)
        {
            this.Get<ILinkBuilder>().AccessDenied();
        }
    }
}