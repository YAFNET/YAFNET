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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System.Collections.Generic;
using System.IO;

using MimeKit;

using YAF.Types.Objects.Model;

/// <summary>
///     The digest template email.
/// </summary>
public class TemplateDigestEmail : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateDigestEmail"/> class.
    /// </summary>
    /// <param name="topics">The topics.</param>
    public TemplateDigestEmail(List<PagedTopic> topics)
    {
        this.HtmlTemplateFileName = "EmailTemplate.html";
        this.HtmlDigestTopicTemplateFileName = "DigestTopicTemplate.html";

        this.Topics = topics;

        var logoUrl =
            $"/{this.Get<BoardFolders>().Logos}/{this.Get<BoardSettings>().ForumLogo}";

        this.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
        this.TemplateParams["{forumlink}"] = this.Get<ILinkBuilder>().ForumUrl;
        this.TemplateParams["%%forumlink%%"] = this.Get<ILinkBuilder>().ForumUrl;
        this.TemplateParams["%%logo%%"] = $"{this.Get<BoardInfo>().ForumBaseUrl}{logoUrl}";
        this.TemplateParams["%%manageLink%%"] = this.Get<ILinkBuilder>().GetAbsoluteLink(ForumPages.Profile_Subscriptions);
    }

    /// <summary>
    ///     Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    ///     Gets or sets TemplateLanguageFile.
    /// </summary>
    public string TemplateLanguageFile { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    /// <value>The topics.</value>
    public List<PagedTopic> Topics { get; set; }

    /// <summary>
    /// Gets or sets the html template file name.
    /// </summary>
    public string HtmlDigestTopicTemplateFileName { get; set; }

    /// <summary>
    /// Gets or sets the html template file name.
    /// </summary>
    public string HtmlTemplateFileName { get; set; }

    /// <summary>
    ///     Gets or sets Template Parameter
    /// </summary>
    public IDictionary<string, string> TemplateParams { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the template topic parameters.
    /// </summary>
    /// <value>The template topic parameters.</value>
    public IDictionary<string, string> TemplateTopicParams { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Create Email.
    /// </summary>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <returns>
    /// The <see cref="MimeMessage"/>.
    /// </returns>
    public MimeMessage CreateEmail(MailboxAddress fromAddress, MailboxAddress toAddress, string subject)
    {
        // Create Mail Message
        return this.Get<IMailService>().CreateMessage(
            fromAddress,
            toAddress,
            fromAddress,
            subject,
            "You must have HTML Email Viewer to View.",
            this.ProcessHtml(subject));
    }

    /// <summary>
    /// Processes the HTML.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <returns>System.String.</returns>
    public string ProcessHtml(string subject)
    {
        var path =
            $"{this.Get<BoardInfo>().WebRootPath}{this.Get<ITheme>().BuildThemePath(this.HtmlTemplateFileName).Replace("/", Path.DirectorySeparatorChar.ToString())}";

        var htmlTemplate = File.ReadAllText(path);

        var formattedBody = new StringBuilder();

        formattedBody.AppendLine(subject);

        this.Topics.ForEach(topic => formattedBody.Append(this.ProcessTopic(topic)));

        this.TemplateParams["{manageText}"] = this.Get<ILocalization>().GetText("DIGEST", "REMOVALTEXT", this.TemplateLanguageFile);

        this.TemplateParams["{manageLinkText}"] =
            this.Get<ILocalization>().GetText("DIGEST", "REMOVALLINK", this.TemplateLanguageFile);

        var html = this.TemplateParams.Keys.Aggregate(
            htmlTemplate,
            (current, key) => current.Replace(key, this.TemplateParams[key]));

        return html.Replace("{CONTENT}", formattedBody.ToString());
    }

    /// <summary>
    /// Processes the topic html.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <returns>System.String.</returns>
    private string ProcessTopic(PagedTopic topic)
    {
        var path =
            $"{this.Get<BoardInfo>().WebRootPath}{this.Get<ITheme>().BuildThemePath(this.HtmlDigestTopicTemplateFileName).Replace("/", Path.DirectorySeparatorChar.ToString())}";

        var htmlTemplate = File.ReadAllText(path);

        var commentsText = this.Get<ILocalization>().GetText("DIGEST", "COMMENTS", this.TemplateLanguageFile);
        var starterText = this.Get<ILocalization>().GetText("DIGEST", "STARTEDBY", this.TemplateLanguageFile);

        this.TemplateTopicParams["{topicName}"] = topic.Subject;
        this.TemplateTopicParams["{forumName}"] = topic.ForumName;
        this.TemplateTopicParams["{replies}"] = string.Format(
            commentsText,
            topic.Replies);
        this.TemplateTopicParams["{starter}"] = string.Format(
            starterText,
            topic.Starter);
        this.TemplateTopicParams["{link}"] =
            this.Get<ILocalization>().GetText("DIGEST", "LINK", this.TemplateLanguageFile);
        this.TemplateTopicParams["{message}"] = BBCodeHelper
            .StripBBCode(HtmlTagHelper.StripHtml(HtmlTagHelper.CleanHtmlString(topic.LastMessage)))
            .RemoveMultipleWhitespace().Truncate(200);

        this.TemplateTopicParams["%%topicLink%%"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
            ForumPages.Posts,
            new { m = topic.LastMessageID, name = topic.Subject, t = topic.TopicID });

        var html = this.TemplateTopicParams.Keys.Aggregate(
            htmlTemplate,
            (current, key) => current.Replace(key, this.TemplateTopicParams[key]));

        return html;
    }
}