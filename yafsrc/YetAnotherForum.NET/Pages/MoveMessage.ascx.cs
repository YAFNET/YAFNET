/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Types.Models;

/// <summary>
/// Move Message Page
/// </summary>
public partial class MoveMessage : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "MoveMessage" /> class.
    /// </summary>
    public MoveMessage()
        : base("MOVEMESSAGE", ForumPages.MoveMessage)
    {
    }

    /// <summary>
    /// The move message id.
    /// </summary>
    protected int MoveMessageId =>
        this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

    /// <summary>
    /// Handles the PreRender event
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.ForumListSelected.Value = this.PageBoardContext.PageForumID.ToString();

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.SelectForumsLoadJs),
            JavaScriptBlocks.SelectForumsLoadJs(
                "ForumList",
                this.GetText("SELECT_FORUM"),
                false,
                false,
                this.ForumListSelected.ClientID));

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.SelectTopicsLoadJs),
            JavaScriptBlocks.SelectTopicsLoadJs(
                "TopicList",
                this.ForumListSelected.ClientID,
                this.TopicListSelected.ClientID));
    }

    /// <summary>
    /// Handles the Click event of the CreateAndMove control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void CreateAndMove_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.TopicSubject.Text.IsSet())
        {
            var topicId = this.GetRepository<Topic>().CreateByMessage(
                this.MoveMessageId,
                this.ForumListSelected.Value.ToType<int>(),
                this.TopicSubject.Text);

            this.GetRepository<Message>().Move(
                this.MoveMessageId,
                topicId.ToType<int>(),
                true);

            this.Get<LinkBuilder>().Redirect(
                ForumPages.Topics,
                new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
        }
        else
        {
            this.PageBoardContext.Notify(this.GetText("Empty_Topic"), MessageTypes.warning);
        }
    }

    /// <summary>
    /// Handles the Click event of the Move control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        var moveTopicId = this.TopicListSelected.Value.ToType<int>();

        if (moveTopicId == 0 || moveTopicId == this.PageBoardContext.PageMessage.ID)
        {
            this.PageBoardContext.Notify(this.GetText("MOVE_TITLE"), MessageTypes.warning);
            
            return;
        }
        
        this.GetRepository<Message>().Move(
            this.MoveMessageId,
            moveTopicId,
            true);

        var topic = this.GetRepository<Topic>().GetById(moveTopicId);

        this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { m = this.MoveMessageId, name = topic.TopicName });

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Topics,
            new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Get<HttpRequestBase>().QueryString.Exists("m") || !this.PageBoardContext.ForumModeratorAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    protected override void CreatePageLinks()
    {
        this.PageLinks.AddRoot();
        this.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
        this.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageLinks.AddTopic(this.PageBoardContext.PageTopic.TopicName, this.PageBoardContext.PageTopicID);

        this.PageLinks.AddLink(this.GetText("MOVE_MESSAGE"));
    }
}