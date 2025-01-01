﻿/* Yet Another Forum.NET
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

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
///  Moderating Page for Reported Posts.
/// </summary>
public partial class ReportedPosts : ModerateForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ReportedPosts" /> class.
    ///   Default constructor.
    /// </summary>
    public ReportedPosts()
        : base("MODERATE_FORUM", ForumPages.Moderate_ReportedPosts)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // forum index
        this.PageBoardContext.PageLinks.AddRoot();

        // moderation index
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("MODERATE_DEFAULT", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Moderate_Index));

        // current page
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageForum.Name);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        this.List.ItemCommand += this.List_ItemCommand;

        base.OnInit(e);
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // do this just on page load, not post-backs
        if (this.IsPostBack)
        {
            return;
        }

        // bind data
        this.BindData();
    }

    /// <summary>
    /// The list_ on item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void List_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var message = (ReportedMessage)e.Item.DataItem;

        var messagePostData = e.Item.FindControlAs<MessagePostData>("MessagePostPrimary");

        messagePostData.MessageID = message.MessageID;

        messagePostData.CurrentMessage = new Message {
                                                         MessageFlags = new MessageFlags(message.Flags),
                                                         MessageText = message.OriginalMessage
                                                     };
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        // get reported posts for this forum
        var dt = this.GetRepository<MessageReported>().ListReported(this.PageBoardContext.PageForumID);

        if (!dt.Any())
        {
            // nope -- redirect back to the moderate main...
            this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_Index);
        }

        this.List.DataSource = dt;

        // bind data to controls
        this.DataBind();
    }

    /// <summary>
    /// Handles post moderation events/buttons.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    private void List_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        Message message;

        // which command are we handling
        switch (e.CommandName.ToLower())
        {
            case "delete":

                var commandArgs = e.CommandArgument.ToString().Split(';');

                var topicId = commandArgs[1].ToType<int>();
                var messageId = commandArgs[0].ToType<int>();

                message = this.GetRepository<Message>().GetById(messageId);

                // delete message
                this.GetRepository<Message>().Delete(
                    this.PageBoardContext.PageForumID,
                    topicId,
                    message,
                    true,
                    string.Empty,
                    true,
                    true);

                // tell user message was deleted
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("DELETED"), MessageTypes.info);
                break;
            case "view":

                // go to the message
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.Posts,
                    new {
                            m = e.CommandArgument,
                            name = this.GetRepository<Topic>().GetNameFromMessage(e.CommandArgument.ToType<int>())
                        });
                break;
            case "copyover":

                message = this.GetRepository<Message>().GetById(e.CommandArgument.ToType<int>());

                // update message text
                this.GetRepository<MessageReported>().ReportCopyOver(message);
                break;
            case "viewhistory":

                // go to history page
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.MessageHistory,
                    new {f = this.PageBoardContext.PageForumID, m = e.CommandArgument});
                break;
            case "resolved":

                // mark message as resolved
                this.GetRepository<Message>().ReportResolve(
                    e.CommandArgument.ToType<int>(),
                    this.PageBoardContext.PageUserID);

                // tell user message was flagged as resolved
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("RESOLVEDFEEDBACK"), MessageTypes.success);
                break;
        }

        this.BindData();
    }
}