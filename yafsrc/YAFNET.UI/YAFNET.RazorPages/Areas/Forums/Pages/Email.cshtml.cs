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

using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// Send Email from one user to the user
/// </summary>
public class EmailModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EmailModel" /> class.
    /// </summary>
    public EmailModel()
        : base("IM_EMAIL", ForumPages.Email)
    {
    }

    /// <summary>
    /// Gets or sets the email user.
    /// </summary>
    [BindProperty]
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
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.AspNetUser is null || !this.PageBoardContext.BoardSettings.AllowEmailSending)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // get user data...
        this.EmailUser = this.GetRepository<User>().GetById(u.Value);

        if (this.EmailUser is null)
        {
            // No such user exists
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddUser(this.EmailUser.ID, this.EmailUser.DisplayOrUserName());
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        return !this.EmailUser.UserFlags.IsApproved ? this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid) : this.Page();
    }

    /// <summary>
    /// Send the Email
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    public async Task<IActionResult> OnPostAsync(int u)
    {
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
            return this.Get<LinkBuilder>().Redirect(
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
}