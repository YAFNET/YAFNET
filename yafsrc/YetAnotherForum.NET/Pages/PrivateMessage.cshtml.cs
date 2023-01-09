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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Private Message page
/// </summary>
public class PrivateMessageModel : ForumPageRegistered
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateMessageModel"/> class.
    /// </summary>
    public PrivateMessageModel()
        : base("MESSAGE", ForumPages.PrivateMessage)
    {
    }

    /// <summary>
    /// Gets or sets the message title.
    /// </summary>
    /// <value>The message title.</value>
    [TempData]
    public string MessageTitle { get; set; }

    /// <summary>
    /// Gets or sets the messages.
    /// </summary>
    /// <value>The messages.</value>
    [BindProperty]
    public IList<PagedPm> Messages { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether IsOutbox.
    /// </summary>
    [TempData]
    public bool IsOutbox { get; set; }

    public IActionResult OnPostDelete (int id)
    {
        var message = this.GetRepository<UserPMessage>().GetById(id);

        this.GetRepository<UserPMessage>().Delete(message, message.UserID != this.PageBoardContext.PageUserID);

        this.PageBoardContext.SessionNotify(this.GetText("msg_deleted"), MessageTypes.success);
        
        return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages);
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet([NotNull]int? pm, [NotNull] string v) 
            
    {
        // check if this feature is disabled
        if (!this.PageBoardContext.BoardSettings.AllowPrivateMessages)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        if (!pm.HasValue)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // handle custom YafBBCode javascript or CSS...
        this.Get<IBBCodeService>().RegisterCustomBBCodeInlineElements();

        return this.BindData(pm.Value, v);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="v">The view.</param>
    /// <returns>IActionResult.</returns>
    private IActionResult BindData(int messageId, string v)
    {
        var messages = this.GetRepository<PMessage>().List(messageId, true);

        if (!messages.NullOrEmpty())
        {
            messages.ForEach(
                m => this.GetRepository<UserPMessage>().MarkAsRead(m.PMessageID, new PMessageFlags(m.UserPMFlags)));

            var message = messages.FirstOrDefault();

            this.SetMessageView(message.FromUserID, message.ToUserID, v, message.IsInOutbox);

            this.MessageTitle = this.PageTitle = message.Subject;
            
            this.Messages = messages;

            // get the return link to the pm listing
            if (this.IsOutbox)
            {
                this.PageBoardContext.PageLinks.AddLink(
                    this.GetText("SENTITEMS"),
                    this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages, new { v = "out" }));
            }
            else
            {
                this.PageBoardContext.PageLinks.AddLink(this.GetText("INBOX"), this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages));
            }

            this.PageBoardContext.PageLinks.AddLink(message.Subject);
        }
        else
        {
            return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages);
        }

        if (this.IsOutbox)
        {
            return this.Page();
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserPrivateMessageEvent(this.PageBoardContext.PageUserID, messageId));

        return this.Page();
    }

    /// <summary>
    /// Sets the IsOutbox property as appropriate for this private message.
    /// </summary>
    /// <param name="fromUserId">
    /// The from User Id.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    /// <param name="messageIsInOutbox">
    /// Indicating whether the message is in the sender's outbox
    /// </param>
    /// <param name="v">The view.</param>
    private IActionResult SetMessageView(
        [NotNull] int fromUserId,
        [NotNull] int toUserId,
        [NotNull] string v,
        bool messageIsInOutbox)
    {
        var isCurrentUserFrom = fromUserId.Equals(this.PageBoardContext.PageUserID);
        var isCurrentUserTo = toUserId.Equals(this.PageBoardContext.PageUserID);

        switch (isCurrentUserFrom)
        {
            // check if it's the same user...
            case true when isCurrentUserTo:
            {
                // it is... handle the view based on the query string passed
                this.IsOutbox = v == "out";

                // see if the message got deleted, if so, redirect to their outbox/archive
                if (this.IsOutbox && !messageIsInOutbox)
                {
                    return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, new { v = "out" });
                }

                break;
            }
            // see if it's been deleted by the from user...
            case true when !messageIsInOutbox:
                // deleted for this user, redirect...
                return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, new { v = "out" });
            case true:
                // nope
                this.IsOutbox = true;
                break;
            default:
            {
                if (isCurrentUserTo)
                {
                    // get the status for the receiver
                    this.IsOutbox = false;
                }
                else
                {
                    return this.Get<LinkBuilder>().AccessDenied();
                }

                break;
            }
        }

        return this.Page();
    }
}