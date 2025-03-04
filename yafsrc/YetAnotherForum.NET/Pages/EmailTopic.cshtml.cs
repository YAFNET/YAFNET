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
using YAF.Types.Models;

/// <summary>
/// The Share Topic via email
/// </summary>
public class EmailTopicModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EmailTopicModel" /> class.
    /// </summary>
    public EmailTopicModel()
        : base("EMAILTOPIC", ForumPages.EmailTopic)
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
    public EmailTopicInputModel Input { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="t">
    /// The t.
    /// </param>
    public IActionResult OnGet(int? t)
    {
        if (!t.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.PageBoardContext.PageTopic is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("EMAILTOPIC", "TITLE"),
            string.Empty);

        this.Input = new EmailTopicInputModel();

        if (!this.PageBoardContext.ForumReadAccess || !this.PageBoardContext.BoardSettings.AllowEmailTopic)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input.Subject = this.PageBoardContext.PageTopic.TopicName;

        var emailTopic = new TemplateEmail("EMAILTOPIC") {
                                                             TemplateParams = {
                                                                                  ["{link}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                                                                      ForumPages.Posts,
                                                                                      new {
                                                                                          t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName
                                                                                      }),
                                                                                  ["{user}"] = this.PageBoardContext.PageUser.DisplayOrUserName()
                                                                              }
                                                         };

        this.Input.Body = emailTopic.ProcessTemplate("EMAILTOPIC");

        return this.Page();
    }

    /// <summary>
    /// Send the Email
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var emailTopic = new TemplateEmail("EMAILTOPIC")
                             {
                                 TemplateParams = { ["{message}"] = this.Input.Body.Trim() }
                             };

            // send a change email message...
            await emailTopic.SendEmailAsync(MailboxAddress.Parse(this.Input.Email.Trim()), this.Input.Subject.Trim());

            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Posts,
                new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });
        }
        catch (Exception x)
        {
            this.Get<ILogger<EmailTopicModel>>().Log(this.PageBoardContext.PageUserID, this, x);
            return this.PageBoardContext.Notify(this.GetTextFormatted("failed", x.Message), MessageTypes.danger);
        }
    }
}