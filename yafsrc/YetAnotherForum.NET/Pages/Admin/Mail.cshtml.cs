
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public InputModel Input { get; set; }

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
        this.Input = new InputModel();
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the Send control.
    /// </summary>
    public IActionResult OnPostSend()
    {
        var emails = this.GetRepository<User>().GroupEmails(this.Input.ToListItem);

        Parallel.ForEach(
            emails,
            email =>
            {
                var from = new MailboxAddress(
                    this.PageBoardContext.BoardSettings.Name,
                    this.PageBoardContext.BoardSettings.ForumEmail);

                    var to = new MailboxAddress(email, email);

                    this.Get<IMailService>().Send(
                        from,
                        to,
                        from,
                        this.Input.Subject,
                        this.Input.Body,
                        null);
                });

        this.Input.Subject = string.Empty;
        this.Input.Body = string.Empty;

        return this.PageBoardContext.Notify(this.GetText("ADMIN_MAIL", "MSG_QUEUED"), MessageTypes.success);
    }

    /// <summary>
    /// Send a test email
    /// </summary>
    public IActionResult OnPostTestSmtp()
    {
        try
        {
            // TODO : Handle Validation for multiple forms on page
            this.Get<IMailService>().Send(
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

        this.List = new List<SelectListItem>(new SelectList(groups, nameof(Group.ID), nameof(Group.Name)));

        this.Input.TestSubject = this.GetText("TEST_SUBJECT");
           this.Input.TestBody = this.GetText("TEST_BODY");
           this.Input.TestFromEmail = this.PageBoardContext.BoardSettings.ForumEmail;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        [Required]
        public string TestFromEmail { get; set; }

        [Required]
        public string TestToEmail { get; set; }

        public int ToListItem { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Body { get; set; }

        [Required]
        public string TestSubject { get; set; }

        [Required]
        public string TestBody { get; set; }
    }
}