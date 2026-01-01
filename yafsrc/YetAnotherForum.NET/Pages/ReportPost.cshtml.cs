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

namespace YAF.Pages;

using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// The form for reported post complaint text.
/// </summary>
public class ReportPostModel : ForumPage
{
    /// <summary>
    /// Gets or sets the message author.
    /// </summary>
    /// <value>The message author.</value>
    [BindProperty]
    public User MessageAuthor { get; set; }

    /// <summary>
    /// Gets or sets the report.
    /// </summary>
    /// <value>The report.</value>
    [BindProperty]
    public string Report { get; set; }

    /// <summary>
    ///   Initializes a new instance of the ReportPost class.
    /// </summary>
    public ReportPostModel()
        : base("REPORTPOST", ForumPages.ReportPost)
    {
    }

    /// <summary>
    /// Report Message.
    /// </summary>
    public async Task<IActionResult> OnPostReportAsync()
    {
        if (this.Report.Length > this.PageBoardContext.BoardSettings.MaxReportPostChars)
        {
            return this.PageBoardContext.Notify(
                this.GetTextFormatted("REPORTTEXT_TOOLONG", this.PageBoardContext.BoardSettings.MaxReportPostChars),
                MessageTypes.danger);
        }

        // Save the reported message
        await this.GetRepository<MessageReported>().ReportAsync(
            this.PageBoardContext.PageMessage,
            this.PageBoardContext.PageUserID,
            DateTime.UtcNow,
            this.Report);

        // Send Notification to Mods about the Reported Post.
        if (this.PageBoardContext.BoardSettings.EmailModeratorsOnReportedPost)
        {
            // not approved, notify moderators
            await this.Get<ISendNotification>().ToModeratorsThatMessageWasReportedAsync(
                this.PageBoardContext.PageForumID,
                this.PageBoardContext.PageMessage.ID,
                this.PageBoardContext.PageUserID,
                this.Report);
        }

        this.PageBoardContext.SessionNotify(this.GetText("MSG_REPORTED"), MessageTypes.success);

        // Redirect to reported post
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Post,
            new {m = this.PageBoardContext.PageMessage.ID, name = this.PageBoardContext.PageTopic.TopicName});
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? m = null)
    {
        if (!m.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        try
        {
            if (this.PageBoardContext.PageMessage.IsNullOrEmptyField())
            {
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            this.MessageAuthor = this.GetRepository<User>().GetById(this.PageBoardContext.PageMessage.UserID);
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Moderated);
        }

        // We check here if the user have access to the option
        return !this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ReportPostPermissions)
                   ? this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Moderated)
                   : this.Page();
    }
}