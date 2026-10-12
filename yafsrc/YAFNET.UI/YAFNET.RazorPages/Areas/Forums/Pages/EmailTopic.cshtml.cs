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
using YAF.Core.Services;
using YAF.Types.Extensions;

/// <summary>
/// The Share Topic via email
/// </summary>
public class EmailTopicModel : ForumPage
{
    /// <summary>
    /// How many emails a user can send in the time window.
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
    ///   Initializes a new instance of the <see cref = "EmailTopicModel" /> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    public EmailTopicModel(IMemoryCache memoryCache)
        : base("EMAILTOPIC", ForumPages.EmailTopic)
    {
        this.memoryCache = memoryCache;
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EmailTopicInputModel Input { get; set; }

    /// <summary>
    /// Gets the subject of the email (the topic name, it can not be changed).
    /// </summary>
    public string Subject { get; private set; }

    /// <summary>
    /// Gets the text of the email (the link to the topic, it can not be changed).
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    /// Checks the access.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        if (this.PageBoardContext.PageTopic is null)
        {
            context.Result = this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            return;
        }

        if (this.PageBoardContext.IsGuest || !this.PageBoardContext.ForumReadAccess
                                          || !this.PageBoardContext.BoardSettings.AllowEmailTopic)
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
            return;
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EMAILTOPIC", "TITLE"), string.Empty);

        this.BindData();
    }

    /// <summary>
    /// Shows the form.
    /// </summary>
    public IActionResult OnGet()
    {
        this.Input = new EmailTopicInputModel();

        return this.Page();
    }

    /// <summary>
    /// Sends the email. Only the recipient comes from the form, the subject and the text are fixed,
    /// otherwise the forum could be used to send any email (spam).
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var email = this.Input?.Email?.Trim();

        if (email.IsNotSet() || !ValidationHelper.IsValidEmail(email))
        {
            return this.PageBoardContext.Notify(this.GetText("EDIT_PROFILE", "BAD_EMAIL"), MessageTypes.warning);
        }

        if (!SendLimitHelper.TryCount(
                this.memoryCache,
                $"EmailTopic_{this.PageBoardContext.PageUserID}",
                MaxEmailsPerWindow,
                EmailWindow))
        {
            return this.PageBoardContext.Notify(this.GetText("EMAILTOPIC", "TOO_MANY"), MessageTypes.warning);
        }

        try
        {
            var emailTopic = new TemplateEmail("EMAILTOPIC")
                             {
                                 TemplateParams = { ["{message}"] = this.Body }
                             };

            await emailTopic.SendEmailAsync(new MailboxAddress(email, email), this.Subject);

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

    /// <summary>
    /// Creates the subject and the text of the email.
    /// </summary>
    private void BindData()
    {
        this.Subject = this.PageBoardContext.PageTopic.TopicName;

        var emailTopic = new TemplateEmail("EMAILTOPIC")
                         {
                             TemplateParams =
                             {
                                 ["{link}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                     ForumPages.Posts,
                                     new
                                     {
                                         t = this.PageBoardContext.PageTopicID,
                                         name = this.PageBoardContext.PageTopic.TopicName
                                     }),
                                 ["{user}"] = this.PageBoardContext.PageUser.DisplayOrUserName()
                             }
                         };

        this.Body = emailTopic.ProcessTemplate("EMAILTOPIC");
    }
}
