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

using YAF.Core.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// Class to communicate in Jabber.
/// </summary>
public class JabberModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "JabberModel" /> class.
    /// </summary>
    public JabberModel()
        : base("IM_XMPP", ForumPages.Jabber)
    {
    }

    /// <summary>
    /// Gets or sets the email user.
    /// </summary>
    [BindProperty]
    public User CurrentUser { get; set; }

    /// <summary>
    /// Gets or sets the alert type.
    /// </summary>
    [TempData]
    public MessageTypes AlertType { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    [TempData]
    public string Message { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    public async Task<IActionResult> OnGetAsync(int? u = null)
    {
        if (!u.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.AspNetUser is null || !this.PageBoardContext.BoardSettings.AllowEmailSending)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        // get user data...
        this.CurrentUser = await this.GetRepository<User>().GetByIdAsync(u.Value);

        if (this.CurrentUser is null)
        {
            // No such user exists
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (!this.CurrentUser.UserFlags.IsApproved)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddUser(
            this.CurrentUser.ID,
            this.CurrentUser.DisplayOrUserName());

        this.PageBoardContext.PageLinks.AddLink(this.GetText("IM_XMPP","TITLE"), string.Empty);

        if (u == this.PageBoardContext.PageUserID)
        {
            this.Message = this.GetText("SERVERYOU");
            this.AlertType = MessageTypes.warning;
        }
        else
        {
            // get full user data...
            var userDataHe = await this.Get<IAspNetUsersHelper>().GetUserAsync(this.CurrentUser.ProviderUserKey);

            var serverHe = userDataHe.Profile_XMPP[(userDataHe.Profile_XMPP.IndexOf('@') + 1)..].Trim();

            var serverMe = this.PageBoardContext.MembershipUser.Profile_XMPP[(this.PageBoardContext.MembershipUser.Profile_XMPP.IndexOf('@') + 1)..].Trim();

            this.Message = serverMe == serverHe
                               ? this.GetTextFormatted("SERVERSAME", userDataHe.Profile_XMPP)
                               : this.GetTextFormatted("SERVEROTHER", $"https://{serverHe}");

            this.AlertType = MessageTypes.info;
        }

        return this.Page();
    }
}