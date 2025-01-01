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

using System;

using MimeKit;

using YAF.Types.Models;

/// <summary>
/// The YAF digest.
/// </summary>
public class DigestService : IDigestService, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DigestService"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public DigestService(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// Creates the digest email
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="boardAddress">The board address.</param>
    /// <param name="toEmail">To email.</param>
    /// <param name="toName">To name.</param>
    /// <returns>MailMessage.</returns>
    public MimeMessage CreateDigest(
        User user,
        MailboxAddress boardAddress,
        string toEmail,
        string toName)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(boardAddress);
        ArgumentNullException.ThrowIfNull(toEmail);

        // get topic hours...
        var topicHours = -this.Get<BoardSettings>().DigestSendEveryXHours;

        var topicsData = this.Get<DataBroker>().GetDigestTopics(
            BoardContext.Current.PageBoardID,
            user.ID,
            DateTime.Now.AddHours(topicHours),
            9999);

        var activeTopics = topicsData.Where(
                t => t.LastPosted > DateTime.Now.AddHours(topicHours) && t.Posted < DateTime.Now.AddHours(topicHours))
            .ToList();

        var newTopics = topicsData.Where(t => t.Posted > DateTime.Now.AddHours(topicHours))
            .OrderByDescending(x => x.LastPosted).ToList();

        if (!newTopics.HasItems() && !activeTopics.HasItems())
        {
            return null;
        }

        var topics = newTopics.Concat(activeTopics).GroupBy(x => x.TopicID).Select(x => x.First()).ToList();

        var languageFile = user.LanguageFile.IsSet() && this.Get<BoardSettings>().AllowUserLanguage
                               ? user.LanguageFile
                               : this.Get<BoardSettings>().Language;

        var email = new TemplateDigestEmail(topics)
                        {
                            TemplateLanguageFile = languageFile
                        };
        var subject = string.Format(
            this.Get<ILocalization>().GetText("DIGEST", "SUBJECT", languageFile),
            this.Get<BoardSettings>().Name);

        return email.CreateEmail(boardAddress, new MailboxAddress(toEmail, toName), subject);
    }
}