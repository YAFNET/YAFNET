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

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Post Message History Page.
/// </summary>
public class MessageHistoryModel : ForumPageRegistered
{
    /// <summary>
    /// Gets or sets the revisions list.
    /// </summary>
    /// <value>The revisions list.</value>
    [BindProperty]
    public List<MessageHistoryTopic> RevisionsList { get; set; }

    /// <summary>
    /// Gets or sets the difference text.
    /// </summary>
    /// <value>The difference text.</value>
    [BindProperty]
    public string DiffText { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MessageHistoryModel" /> class.
    /// </summary>
    public MessageHistoryModel()
        : base("MESSAGEHISTORY", ForumPages.MessageHistory)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="m">The message ID.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int m)
    {
        if (this.PageBoardContext.IsGuest)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (this.PageBoardContext.PageMessage is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        return this.BindData();
    }

    /// <summary>
    /// Redirect to the changed post
    /// </summary>
    public IActionResult OnPostReturn()
    {
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Post,
            new {m = this.PageBoardContext.PageMessage.ID, name = this.PageBoardContext.PageTopic.TopicName});
    }

    /// <summary>
    /// Redirect to the reported posts page
    /// </summary>
    public IActionResult OnPostReturnForum(int f)
    {
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Moderate_ReportedPosts, new {f});
    }

    /// <summary>
    /// Handle Commands for restoring an old Message Version
    /// </summary>
    public IActionResult OnPostRestore(string edited)
    {
        var editedDate = edited.ToType<DateTime>();

        var messageToRestore = this.GetRepository<MessageHistory>().GetSingle(
            m => m.MessageID == this.PageBoardContext.PageMessage.ID && m.Edited == editedDate);

        var messageUser = this.GetRepository<User>().GetById(this.PageBoardContext.PageMessage.UserID);

        this.GetRepository<Message>().Update(
            null,
            messageToRestore.Message,
            null,
            null,
            null,
            null,
            messageToRestore.EditReason,
            this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID,
            this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess,
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageMessage,
            this.PageBoardContext.PageForum,
            messageUser,
            this.PageBoardContext.PageUserID);

        this.BindData();

        return this.PageBoardContext.Notify(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);
    }

    /// <summary>
    /// The get IP address.
    /// </summary>
    /// <param name="dataItem">
    /// The data item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetIpAddress(MessageHistoryTopic dataItem)
    {
        var ip = IPHelper.GetIpAddressAsString(dataItem.IP);

        return ip.IsSet() ? ip : IPHelper.GetIpAddressAsString(dataItem.MessageIP);
    }

    /// <summary>
    /// Render Diff Message
    /// </summary>
    public IActionResult OnPostShowDiff()
    {
        var dmp = new DiffMatchPatch();

        var messages = this.RevisionsList.Where(item => item.Selected).ToList();

        if (messages.NullOrEmpty())
        {
            return this.PageBoardContext.Notify(
                this.GetText("MESSAGEHISTORY", "NOTHING_SELECTED"),
                MessageTypes.warning);
        }

        if (messages.Count == 1)
        {
            return this.PageBoardContext.Notify(this.GetText("MESSAGEHISTORY", "SELECT_BOTH"), MessageTypes.warning);
        }

        var message1 = messages[0];
        var message2 = messages[1];

        var diff = dmp.DiffMain(message1.Message, message2.Message, true);

        dmp.CleanupSemantic(diff);

        this.DiffText = DiffMatchPatch.PrettyHtml(diff);

        return this.Page();
    }

    /// <summary>
    /// Gets the edit reason.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>System.String.</returns>
    public string GetEditReason(MessageHistoryTopic message)
    {
        if(message.Edited != message.Posted)
        {
            return message.EditReason.IsNotSet() ? this.GetText("EDIT_REASON_NA") : message.EditReason;
        }

        return this.GetText("ORIGINALMESSAGE");
    }

    /// <summary>
    /// Creates the page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// Binds data to data source
    /// </summary>
    private IActionResult BindData()
    {
        // Fill revisions list repeater.
        this.RevisionsList = this.GetRepository<MessageHistory>().List(
            this.PageBoardContext.PageMessage.ID,
            this.PageBoardContext.BoardSettings.MessageHistoryDaysToLog);

        this.Get<ISessionService>().SetPageData(this.RevisionsList);

        if (this.RevisionsList.NullOrEmpty())
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        var originalMessage = this.GetRepository<Message>().GetMessageWithAccess(
            this.PageBoardContext.PageMessage.ID,
            this.PageBoardContext.PageUserID);

        return originalMessage is null ? this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid) : this.Page();
    }
}