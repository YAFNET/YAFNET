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
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Post Private Message Page
/// </summary>
public class PostPrivateMessageModel : ForumPageRegistered
{
    [TempData]
    public bool ToEnabled { get; set; } = true;

    [TempData]
    public bool AllUsers { get; set; }

    [TempData]
    public bool AllBuddies { get; set; }

    [TempData]
    public bool FindUsers { get; set; } = true;

    [TempData]
    public bool MultiReceiverAlert { get; set; }

    [TempData]
    public bool Clear { get; set; }

    [TempData]
    public bool ToVisible { get; set; } = true;

    [TempData]
    public bool ToListVisible { get; set; }

    [BindProperty]
    public PagedPm ReplyMessage { get; set; }

    [BindProperty]
    public int ToListValue { get; set; }

    [BindProperty]
    public List<SelectListItem> ToList { get; set; }

    [BindProperty]
    public string To { get; set; }

    [BindProperty]
    public string PreviewMessage { get; set; }

    [BindProperty]
    public string Editor { get; set; }

    [BindProperty]
    public string Subject { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "PostPrivateMessageModel" /> class.
    ///   Default constructor.
    /// </summary>
    public PostPrivateMessageModel()
        : base("PMESSAGE", ForumPages.PostPrivateMessage)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // users control panel
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));

        // post new message
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// Send pm to all users
    /// </summary>
    public void OnPostAllUsers()
    {
        this.ToList = new List<SelectListItem> {
                                                   new() {Value = "0", Text = this.GetText("ALLUSERS")}
                                               };
        // bind the list to dropdown
        this.ToListVisible = true;
        this.To = this.GetText("ALLUSERS");

        // hide To text box
        this.ToVisible = false;

        // hide find users/all users buttons
        this.FindUsers = false;
        this.AllUsers = false;
        this.AllBuddies = false;

        // we need clear button now
        this.Clear = true;
    }

    /// <summary>
    /// Send PM to all Buddies
    /// </summary>
    public IActionResult OnPostAllBuddies()
    {
        // try to find users by user name
        var usersFound = this.GetRepository<Buddy>().GetAllFriends(this.PageBoardContext.PageUserID);

        var friendsString = new StringBuilder();

        if (!usersFound.Any())
        {
            return this.Page();
        }

        // we found a user(s)
        usersFound.ForEach(
            row => friendsString.AppendFormat(
                "{0};",
                this.PageBoardContext.BoardSettings.EnableDisplayName ? row.DisplayName : row.Name));

        this.To = friendsString.ToString();

        // hide find users/all users buttons
        this.FindUsers = false;
        this.AllUsers = false;
        this.AllBuddies = false;

        // we need clear button now
        this.Clear = true;

        return this.Page();
    }

    /// <summary>
    /// Redirect user back to his PM inbox
    /// </summary>
    public IActionResult OnPostCancel()
    {
        return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages);
    }

    /// <summary>
    /// Clears the User List
    /// </summary>
    public void OnPostClear()
    {
        // clear drop down
        this.ToList.Clear();

        // hide it and show empty To text box
        this.ToListVisible = false;
        this.To = string.Empty;
        this.ToVisible = true;

        // show find users and all users (if user is admin)
        this.FindUsers = true;
        this.AllUsers = this.PageBoardContext.IsAdmin;
        this.AllBuddies = this.PageBoardContext.UserHasBuddies && this.PageBoardContext.BoardSettings.EnableBuddyList;

        // clear button is not necessary now
        this.Clear = false;
    }

    /// <summary>
    /// Find Users
    /// </summary>
    public IActionResult OnPostFindUsers()
    {
        if (this.To.IsNotSet())
        {
            // need at least 2 letters of user's name
            return this.PageBoardContext.Notify(this.GetText("NEED_MORE_LETTERS"), MessageTypes.warning);
        }

        if (this.To.Length < 2)
        {
            // need at least 2 letters of user's name
            return this.PageBoardContext.Notify(this.GetText("NEED_MORE_LETTERS"), MessageTypes.warning);
        }

        // try to find users by user name
        var usersFound = this.Get<IUserDisplayName>().FindUserContainsName(this.To.Trim()).Where(
            u => !u.Block.BlockPMs && u.UserFlags.IsApproved && u.ID != this.PageBoardContext.PageUserID).ToList();

        if (usersFound.Any())
        {
            this.ToList = new List<SelectListItem>();

            usersFound.ForEach(
                user =>
                {
                    // we found a user(s)
                    this.ToList.Add(new SelectListItem {Value = user.ID.ToString(), Text = user.DisplayOrUserName()});
                });

            // hide To text box and show To drop down
            this.ToListVisible = true;
            this.ToVisible = false;

            // find is no more needed
            this.FindUsers = false;

            // we need clear button displayed now
            this.Clear = true;

            return this.Page();
        }

        // user not found
        return this.PageBoardContext.Notify(this.GetText("USER_NOTFOUND"), MessageTypes.danger);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? p = null, int? q = null, int? report = null, int? u = null, int? r = null)

    {
        this.ToEnabled = true;
        this.AllUsers = false;
        this.AllBuddies = false;
        this.FindUsers = true;
        this.MultiReceiverAlert = false;
        this.Clear = false;
        this.ToVisible  = true;
        this.ToListVisible = false;

        // only administrators can send messages to all users
        this.AllUsers = this.PageBoardContext.IsAdmin;

        this.AllBuddies = this.PageBoardContext.UserHasBuddies && this.PageBoardContext.BoardSettings.EnableBuddyList;

        // Is Reply
        if (p.HasValue)
        {
            // PM is a reply or quoted reply (isQuoting)
            // to the given message id "p"
            var isQuoting = q.HasValue;

            var isReport = report.HasValue;

            // get quoted message
            this.ReplyMessage = this.GetRepository<PMessage>().GetMessage(p.Value);

            // there is such a message
            if (this.ReplyMessage == null)
            {
                return this.Page();
            }

            // get message sender/recipient
            var toUserId = this.ReplyMessage.ToUserID;
            var fromUserId = this.ReplyMessage.FromUserID;

            // verify access to this PM
            if (toUserId != this.PageBoardContext.PageUserID && fromUserId != this.PageBoardContext.PageUserID)
            {
                return this.Get<LinkBuilder>().AccessDenied();
            }

            // handle subject
            var subject = HtmlTagHelper.StripHtml(this.ReplyMessage.Subject);
            if (!subject.StartsWith("Re: "))
            {
                subject = $"Re: {subject}";
            }

            this.Subject = subject;

            var displayName = this.Get<IUserDisplayName>().GetNameById(fromUserId);

            // set "To" user and disable changing...
            this.To = displayName;
            this.ToEnabled = false;
            this.FindUsers = false;
            this.AllUsers = false;
            this.AllBuddies = false;

            if (!isQuoting)
            {
                return this.Page();
            }

            // PM is a quoted reply
            var body = this.ReplyMessage.Body;

            if (this.PageBoardContext.BoardSettings.RemoveNestedQuotes)
            {
                body = this.Get<IFormatMessage>().RemoveNestedQuotes(body);
            }

            // Ensure quoted replies have bad words removed from them
            body = this.Get<IBadWordReplace>().Replace(body);

            // Quote the original message
            body = $"[QUOTE={displayName}]{body}[/QUOTE]";

            // we don't want any whitespaces at the beginning of message
            this.Editor = body.TrimStart();

            if (!isReport)
            {
                return this.Page();
            }

            var hostUser = this.GetRepository<User>()
                .Get(user => user.BoardID == this.PageBoardContext.PageBoardID && (user.Flags & 1) == 1)
                .FirstOrDefault();

            if (hostUser != null)
            {
                this.To = hostUser.DisplayOrUserName();

                this.Subject = this.GetTextFormatted("REPORT_SUBJECT", displayName);

                var bodyReport = $"[QUOTE={displayName}]{this.ReplyMessage.Body}[/QUOTE]";

                // Quote the original message
                bodyReport = this.GetTextFormatted("REPORT_BODY", bodyReport);

                // we don't want any whitespaces at the beginning of message
                this.Editor = bodyReport.TrimStart();
            }
            else
            {
                return this.Get<LinkBuilder>().AccessDenied();
            }
        }
        else if (u.HasValue && r.HasValue)
        {
            // PM is being send as a quoted reply to a reported post
            // We check here if the user have access to the option
            if (!this.PageBoardContext.IsModeratorInAnyForum && !this.PageBoardContext.IsForumModerator)
            {
                return this.Page();
            }

            // get quoted message
            var reporter = this.GetRepository<User>().MessageReporter(r.Value, u.Value).FirstOrDefault();

            // there is such a message
            // message info should be always returned as 1 row
            if (reporter == null)
            {
                return this.Page();
            }

            // handle subject
            this.Subject = this.GetText("REPORTED_SUBJECT");

            var displayName = this.Get<IUserDisplayName>().GetNameById(reporter.Item1.ID);

            // set "To" user and disable changing...
            this.To = displayName;
            this.ToEnabled = false;
            this.FindUsers = false;
            this.AllUsers = false;
            this.AllBuddies = false;

            // Parse content with delimiter '|'
            var quoteList = reporter.Item2.ReportText.Split('|');

            // Quoted replies should have bad words in them
            // Reply to report PM is always a quoted reply
            // Quote the original message in a cycle
            for (var i = 0; i < quoteList.Length; i++)
            {
                // Add quote codes
                quoteList[i] = $"[QUOTE={displayName}]{quoteList[i]}[/QUOTE]\r\n";

                // Replace DateTime delimiter '??' by ': '
                // we don't want any whitespaces at the beginning of message
                this.Editor = quoteList[i].Replace("??", ": ") + this.Editor.TrimStart();
            }
        }
        else if (u.HasValue)
        {
            // find user
            var foundUser = this.GetRepository<User>().GetById(u.Value);

            if (foundUser == null)
            {
                return this.Page();
            }

            if (foundUser.ID == this.PageBoardContext.PageUserID)
            {
                return this.Page();
            }

            this.To = foundUser.DisplayOrUserName();

            this.ToEnabled = false;

            // hide find user/all users buttons
            this.FindUsers = false;
            this.AllUsers = false;
            this.AllBuddies = false;
        }
        else
        {
            // Blank PM

            // multi-receiver info is relevant only when sending blank PM
            if (this.PageBoardContext.BoardSettings.PrivateMessageMaxRecipients < 1 || this.PageBoardContext.IsAdmin)
            {
                return this.Page();
            }

            // display info
            this.MultiReceiverAlert = true;
        }

        return this.Page();
    }

    /// <summary>
    /// Previews the Message Output
    /// </summary>
    public IActionResult OnPostPreview()
    {
        this.PreviewMessage = this.Editor;

        return this.Page();
    }

    /// <summary>
    /// Send Private Message
    /// </summary>
    public IActionResult OnPostSave(int? p = null, int? report = null)
    {
        var replyTo = p;

        // Check if quoted message is Reply
        if (this.ReplyMessage?.ReplyTo != null)
        {
            replyTo = this.ReplyMessage.ReplyTo;
        }

        if (report.HasValue)
        {
            replyTo = null;
        }

        if (this.ToListVisible)
        {
            this.To = this.ToListValue.ToString();
        }

        if (this.To.Length <= 0)
        {
            // recipient is required field
            return this.PageBoardContext.Notify(this.GetText("need_to"), MessageTypes.warning);
        }

        // subject is required
        if (this.Subject.Trim().Length <= 0)
        {
            return this.PageBoardContext.Notify(this.GetText("need_subject"), MessageTypes.warning);
        }

        // message is required
        if (this.Editor.Trim().Length <= 0)
        {
            return this.PageBoardContext.Notify(this.GetText("need_message"), MessageTypes.warning);
        }

        if (this.ToListVisible && this.ToListValue is 0)
        {
            // administrator is sending PMs to all users
            var body = HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.Editor));

            var messageFlags = new MessageFlags {
                                                    IsHtml = false,
                                                    IsBBCode = true
                                                };

            // test user's PM count
            if (!this.VerifyMessageAllowed(1, body))
            {
                return this.Page();
            }

            this.GetRepository<PMessage>().SendMessage(
                this.PageBoardContext.PageUserID,
                0,
                HtmlTagHelper.StripHtml(this.Subject),
                body,
                messageFlags.BitValue,
                replyTo);

            // redirect to outbox (sent items), not control panel
            return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, new {v = "out"});
        }

        if (this.ToListVisible && this.ToListValue is not 0)
        {
            this.To = this.GetRepository<User>().GetById(this.ToListValue).DisplayOrUserName();
        }

        // remove all abundant whitespaces and separators
        var rx = new Regex(@";(\s|;)*;", RegexOptions.NonBacktracking);
        this.To = rx.Replace(this.To, ";");

        if (this.To.StartsWith(";"))
        {
            this.To = this.To[1..];
        }

        if (this.To.EndsWith(";"))
        {
            this.To = this.To[..^1];
        }

        rx = new Regex(@"\s*;\s*", RegexOptions.NonBacktracking);
        this.To = rx.Replace(this.To, ";");

        // list of recipients
        var recipients = new List<string>(this.To.Trim().Split(';'));

        if (recipients.Count > this.PageBoardContext.BoardSettings.PrivateMessageMaxRecipients
            && !this.PageBoardContext.IsAdmin && this.PageBoardContext.BoardSettings.PrivateMessageMaxRecipients != 0)
        {
            // to many recipients
            return this.PageBoardContext.Notify(
                this.GetTextFormatted(
                    "TOO_MANY_RECIPIENTS",
                    this.PageBoardContext.BoardSettings.PrivateMessageMaxRecipients),
                MessageTypes.warning);
        }

        if (!this.VerifyMessageAllowed(recipients.Count, this.Editor))
        {
            return this.Page();
        }

        // list of recipient's ids
        var recipientIds = new List<int>();

        // get recipients' IDs
        foreach (var recipient in recipients)
        {
            var user = this.Get<IUserDisplayName>().FindUserByName(recipient);

            if (user == null)
            {
                return this.PageBoardContext.Notify(
                    this.GetTextFormatted("NO_SUCH_USER", recipient),
                    MessageTypes.warning);
            }

            if (user.UserFlags.IsGuest)
            {
                return this.PageBoardContext.Notify(this.GetText("NOT_GUEST"), MessageTypes.danger);
            }

            // get recipient's ID from the database
            if (!recipientIds.Contains(user.ID))
            {
                recipientIds.Add(user.ID);
            }

            var count = this.GetRepository<PMessage>().UserMessageCount(user.ID);

            // test receiving user's PM count
            if (count.NumberTotal + 1 < count.Allowed || this.PageBoardContext.IsAdmin || this.Get<IAspNetUsersHelper>()
                    .GetBoardUser(user.ID, this.PageBoardContext.PageBoardID).Item4.IsAdmin > 0)
            {
                continue;
            }

            // recipient has full PM box
            return this.PageBoardContext.Notify(
                this.GetTextFormatted("RECIPIENTS_PMBOX_FULL", recipient),
                MessageTypes.danger);
        }

        // send PM to all recipients
        recipientIds.ForEach(
            userId =>

            {
                var body = HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.Editor));

                var messageFlags = new MessageFlags {
                                                        IsHtml = false,
                                                        IsBBCode = true
                                                    };

                this.GetRepository<PMessage>().SendMessage(
                    this.PageBoardContext.PageUserID,
                    userId,
                    HtmlTagHelper.StripHtml(this.Subject),
                    body,
                    messageFlags.BitValue,
                    replyTo);

                // reset lazy data as he should be informed at once
                this.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, userId));

                if (this.PageBoardContext.BoardSettings.AllowPMEmailNotification)
                {
                    this.Get<ISendNotification>().ToPrivateMessageRecipient(userId, this.Subject.Trim());
                }
            });

        // redirect to outbox (sent items), not control panel
        return this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, new {v = "out"});
    }

    /// <summary>
    /// Verifies the message allowed.
    /// </summary>
    /// <param name="count">The recipients count.</param>
    /// <param name="message">The message.</param>
    /// <returns>
    /// Returns if the user is allowed to send a message or not
    /// </returns>
    private bool VerifyMessageAllowed(int count, string message)
    {
        // Check if SPAM Message first...
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
        {
            // Check content for spam
            if (this.Get<ISpamCheck>().CheckPostForSpam(
                    this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName(),
                    this.HttpContext.GetUserRealIPAddress(),
                    message,
                    this.PageBoardContext.MembershipUser.Email,
                    out var spamResult))
            {
                var description = $@"Spam Check detected possible SPAM ({spamResult}) Original message: [{message}]
                       posted by PageUser: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}";

                switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                {
                    case SpamPostHandling.DoNothing:
                        this.Get<ILogger<PostPrivateMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            description);
                        break;
                    case SpamPostHandling.FlagMessageUnapproved:
                        this.Get<ILogger<PostPrivateMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, it was flagged as unapproved post");
                        break;
                    case SpamPostHandling.RejectMessage:
                        this.Get<ILogger<PostPrivateMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, post was rejected");

                        this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                        break;
                    case SpamPostHandling.DeleteBanUser:
                        this.Get<ILogger<PostPrivateMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, user was deleted and banned");

                        this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                            this.PageBoardContext.PageUserID,
                            this.PageBoardContext.MembershipUser,
                            this.PageBoardContext.PageUser.IP);

                        break;
                }

                return false;
            }
        }

        ///////////////////////////////

        // test sending user's PM count
        // get user's name
        var countInfo = this.GetRepository<PMessage>().UserMessageCount(this.PageBoardContext.PageUserID);

        if (countInfo.NumberTotal + count <= countInfo.Allowed || this.PageBoardContext.IsAdmin)
        {
            return true;
        }

        // user has full PM box
        this.PageBoardContext.Notify(this.GetTextFormatted("OWN_PMBOX_FULL", countInfo.Allowed), MessageTypes.danger);

        return false;
    }
}