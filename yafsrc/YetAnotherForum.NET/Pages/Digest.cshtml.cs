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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using YAF.Configuration;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Localization;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;

public class DigestModel : ForumPage
{
    /// <summary>
    ///   Gets NewTopics.
    /// </summary>
    public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> NewTopics { get; set; }

    /// <summary>
    ///   Gets ActiveTopics.
    /// </summary>
    public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> ActiveTopics { get; set; }

    public string InlineCss { get; set; }

    public string LanguageFile { get; set; }

    public string ErrorMessage { get; set; }

    /// <summary>
    ///   The YAF localization.
    /// </summary>
    private ILocalization localization;

    public DigestModel()
        : base("DIGEST", ForumPages.Digest)
    {
    }

    public IActionResult OnGet(int userId, int boardId, string token, bool showError)
    {
        if (token.IsNotSet() || !token.Equals(this.PageBoardContext.BoardSettings.WebServiceToken))
        {
            if (showError)
            {
                this.ErrorMessage = "Invalid Web Service Token. Please go into your host settings and save them committing a unique web service token to the database.";
            }

            return new EmptyResult();
        }

        var currentUser = this.GetRepository<User>().GetById(userId);

        // get topic hours...
        var topicHours = -this.Get<BoardSettings>().DigestSendEveryXHours;

        var forumData = this.Get<DataBroker>().GetSimpleForumTopic(
            boardId,
            userId,
            DateTime.Now.AddHours(topicHours),
            9999);

        var topicsFlattened = forumData.SelectMany(x => x.Topics).ToList();

        this.ActiveTopics = topicsFlattened.Where(
            t => t.LastPostDate > DateTime.Now.AddHours(topicHours) &&
                 t.CreatedDate < DateTime.Now.AddHours(topicHours)).GroupBy(x => x.Forum).ToList();

        this.NewTopics = topicsFlattened.Where(t => t.CreatedDate > DateTime.Now.AddHours(topicHours))
            .OrderByDescending(x => x.LastPostDate).GroupBy(x => x.Forum).ToList();

        if (this.NewTopics.NullOrEmpty() && this.ActiveTopics.NullOrEmpty() && showError)
        {
            this.ErrorMessage = $"No topics for the last {this.PageBoardContext.BoardSettings.DigestSendEveryXHours} hours.";
        }

        if (!this.NewTopics.Any() && !this.ActiveTopics.Any())
        {
            return !showError ? new EmptyResult() : this.Page();
        }

        this.LanguageFile = UserHelper.GetUserLanguageFile(currentUser);

        if (this.LanguageFile.IsNotSet())
        {
            this.LanguageFile = this.PageBoardContext.BoardSettings.Language;
        }

        var cssPath =
            $"{this.Get<IWebHostEnvironment>().WebRootPath}{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css").Replace("/", "\\")}";

        this.InlineCss = System.IO.File.ReadAllText(cssPath);

        return this.Page();
    }

    /// <summary>
    /// Gets the localized text.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText([NotNull] string tag)
    {
        if (this.LanguageFile.IsSet() && this.localization == null)
        {
            this.localization = new Localization();
            this.localization.LoadTranslation(this.LanguageFile);
        }
        else if (this.localization == null)
        {
            this.localization = this.Get<ILocalization>();
        }

        return this.localization.GetText("DIGEST", tag);
    }

    /// <summary>
    /// Gets the message formatted and truncated.
    /// </summary>
    /// <param name="lastMessage">The last message.</param>
    /// <param name="maxlength">The max length.</param>
    /// <returns>
    /// The get message formatted and truncated.
    /// </returns>
    public string GetMessageFormattedAndTruncated([NotNull] string lastMessage, int maxlength)
    {
        return BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(lastMessage)))
            .RemoveMultipleWhitespace().Truncate(maxlength);
    }
}