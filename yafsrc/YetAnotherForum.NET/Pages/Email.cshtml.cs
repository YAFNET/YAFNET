/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// Send Email from one user to the user
/// </summary>
public class EmailModel : ForumPage
{
    /// <summary>
    /// How many emails a user can send in the time window (the same as for "email this topic").
    /// </summary>
    private const int MaxEmailsPerWindow = 5;

    /// <summary>
    /// The time window of the email limit.
    /// </summary>
    private readonly static TimeSpan EmailWindow = TimeSpan.FromHours(1);

    /// <summary>
    /// The memory cache that keeps the sent emails of the users.
    /// </summary>
    private readonly IMemoryCache memoryCache;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "EmailModel" /> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    public EmailModel(IMemoryCache memoryCache)
        : base("IM_EMAIL", ForumPages.Email)
    {
        this.memoryCache = memoryCache;
    }

    /// <summary>
    /// Gets or sets the email user (not sent with the form, it is loaded for the page).
    /// </summary>
    public User EmailUser { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EmailInputModel Input { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    public IActionResult OnGet(int? u)
    {
        if (!u.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // get user data...
        this.EmailUser = this.GetRepository<User>().GetById(u.Value);

        if (!this.CanSendEmailTo(this.EmailUser))
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddUser(this.EmailUser.ID, this.EmailUser.DisplayOrUserName());
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        return this.Page();
    }

    /// <summary>
    /// Send the Email
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    public async Task<IActionResult> OnPostAsync(int u)
    {
        // the same checks as for the page, the user id is sent by the client
        this.EmailUser = await this.GetRepository<User>().GetByIdAsync(u);

        if (!this.CanSendEmailTo(this.EmailUser))
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.Input?.Subject.IsNotSet() != false)
        {
            return this.PageBoardContext.Notify(this.GetText("EMAILTOPIC", "NEED_SUBJECT"), MessageTypes.warning);
        }

        if (this.Input.Body.IsNotSet())
        {
            return this.PageBoardContext.Notify(this.GetText("EMAILTOPIC", "NEED_MESSAGE"), MessageTypes.warning);
        }

        // the forum must not be used to send many emails (spam)
        if (!SendLimitHelper.TryCount(
                this.memoryCache,
                $"Email_{this.PageBoardContext.PageUserID}",
                MaxEmailsPerWindow,
                EmailWindow))
        {
            return this.PageBoardContext.Notify(this.GetText("EMAILTOPIC", "TOO_MANY"), MessageTypes.warning);
        }

        try
        {
            // get "to" user...
            var toUser = await this.Get<IAspNetUsersHelper>().GetMembershipUserByIdAsync(u);

            // send it...
            await this.Get<IMailService>().SendAsync(
                new MailboxAddress(
                    this.PageBoardContext.MembershipUser.UserName,
                    this.PageBoardContext.MembershipUser.Email),
                new MailboxAddress(toUser.UserName.Trim(), toUser.Email.Trim()),
                new MailboxAddress(
                    this.PageBoardContext.BoardSettings.Name,
                    this.PageBoardContext.BoardSettings.ForumEmail),
                this.Input.Subject.Trim(),
                this.Input.Body.Trim());

            // redirect to profile page...
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.UserProfile,
                new { u, name = this.Get<IUserDisplayName>().GetNameById(u) });
        }
        catch (Exception x)
        {
            this.Get<ILogger<EmailModel>>().Log(this.PageBoardContext.PageUserID, this, x);

            return this.PageBoardContext.Notify(
                this.PageBoardContext.IsAdmin ? x.Message : this.GetText("ERROR"),
                MessageTypes.danger);
        }
    }

    /// <summary>
    /// Checks if the current user can send an email to the user
    /// (admins and users in the friend list of the user are excluded from the email blocking,
    /// a user who is ignored by the user can't send emails, like private messages).
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Returns if an email can be sent.</returns>
    private bool CanSendEmailTo(User user)
    {
        if (this.AspNetUser is null || !this.PageBoardContext.BoardSettings.AllowEmailSending)
        {
            return false;
        }

        if (user is null || user.ID == this.PageBoardContext.PageUserID || user.BoardID != this.PageBoardContext.PageBoardID
            || user.UserFlags.IsGuest || !user.UserFlags.IsApproved || user.UserFlags.IsDeleted)
        {
            return false;
        }

        if (this.PageBoardContext.IsAdmin)
        {
            return true;
        }

        if (this.GetRepository<IgnoreUser>().Exists(
                i => i.UserID == user.ID && i.IgnoredUserID == this.PageBoardContext.PageUserID))
        {
            return false;
        }

        return !user.Block.BlockEmails || this.GetRepository<Buddy>().Exists(
                   b => b.FromUserID == user.ID && b.ToUserID == this.PageBoardContext.PageUserID && b.Approved);
    }
}