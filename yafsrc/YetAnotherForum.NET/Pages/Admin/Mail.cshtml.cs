
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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
///     Admin Interface to send Mass email's to user groups.
/// </summary>
public class MailModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public MailInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    public List<SelectListItem> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailModel"/> class.
    /// </summary>
    public MailModel()
        : base("ADMIN_MAIL", ForumPages.Admin_Mail)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_MAIL", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public  void OnGet()
    {
        this.Input = new MailInputModel();
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the Send control.
    /// </summary>
    public async Task<IActionResult> OnPostSendAsync()
    {
        var emails = await this.GetRepository<User>().GroupEmailsAsync(this.Input.ToListItem);

        foreach (var email in emails)
        {
            var from = new MailboxAddress(
                this.PageBoardContext.BoardSettings.Name,
                this.PageBoardContext.BoardSettings.ForumEmail);

            var to = new MailboxAddress(email, email);

            await this.Get<IMailService>().SendAsync(
                from,
                to,
                from,
                this.Input.Subject,
                this.Input.Body,
                null);
        }

        this.Input.Subject = string.Empty;
        this.Input.Body = string.Empty;

        return this.PageBoardContext.Notify(this.GetText("ADMIN_MAIL", "MSG_QUEUED"), MessageTypes.success);
    }

    /// <summary>
    /// Send a test email
    /// </summary>
    public async Task<IActionResult> OnPostTestSmtpAsync()
    {
        try
        {
            await this.Get<IMailService>().SendAsync(
                this.Input.TestFromEmail,
                this.Input.TestToEmail,
                this.Input.TestFromEmail,
                this.Input.TestSubject,
                this.Input.TestBody);

            return this.PageBoardContext.Notify(this.GetText("TEST_SUCCESS"), MessageTypes.success);
        }
        catch (Exception x)
        {
            return this.PageBoardContext.Notify(x.Message, MessageTypes.danger);
        }
    }

    /// <summary>
    ///     The bind data.
    /// </summary>
    private void BindData()
    {
        var groups = this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID).Where(g => !g.GroupFlags.IsGuest).ToList();

        this.List = [..new SelectList(groups, nameof(Group.ID), nameof(Group.Name))];

        this.Input.TestSubject = this.GetText("TEST_SUBJECT");
           this.Input.TestBody = this.GetText("TEST_BODY");
           this.Input.TestFromEmail = this.PageBoardContext.BoardSettings.ForumEmail;
    }
}