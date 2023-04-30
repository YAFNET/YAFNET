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

namespace YAF.Core.Services;

using System.Net.Http;
using System.Threading.Tasks;

using MimeKit;

using YAF.Types.Attributes;
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
    public DigestService([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// Gets the digest HTML.
    /// </summary>
    /// <param name="url">The digest url.</param>
    /// <returns>
    /// The get digest html.
    /// </returns>
    public async Task<string> GetDigestHtmlAsync(string url)
    {
        var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });

        var response = await client.GetAsync(url);

        var digestHtml = string.Empty;

        if (response.IsSuccessStatusCode)
        {
            digestHtml = await response.Content.ReadAsStringAsync();
        }

        return digestHtml;
    }

    /// <summary>
    /// Gets the digest HTML.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="boardSettings">The board settings.</param>
    /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
    /// <returns>
    /// The get digest html.
    /// </returns>
    public async Task<string> GetDigestHtmlAsync(User user, object boardSettings, bool showErrors = false)
    {
        var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });

        var response = await client.GetAsync(this.GetDigestUrl(user.ID, boardSettings, showErrors));

        var digestHtml = string.Empty;

        if (response.IsSuccessStatusCode)
        {
            digestHtml = await response.Content.ReadAsStringAsync();
        }

        return digestHtml;
    }

    /// <summary>
    /// Gets the digest URL.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="boardSettings">The board settings.</param>
    /// <param name="showError">Show errors creating the digest.</param>
    /// <returns>
    /// The get digest url.
    /// </returns>
    public string GetDigestUrl(int userId, object boardSettings, bool showError)
    {
        var yafBoardSettings = boardSettings as BoardSettings;

        var digestUrl = $"Digest?token={yafBoardSettings.WebServiceToken}&userId={userId}&boardId={yafBoardSettings.BoardId}&showError={showError}";

        return
            $"{yafBoardSettings.BaseUrlMask}/{digestUrl}";
    }

    /// <summary>
    /// Gets the digest URL.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="showError">Show errors creating the digest.</param>
    /// <returns>
    /// The get digest url.
    /// </returns>
    public string GetDigestUrl(int userId, bool showError)
    {
        var digestUrl = this.Get<IUrlHelper>().Page(
            ForumPages.Digest.GetPageName(),
            null,
            new
                {
                    token = this.Get<BoardSettings>().WebServiceToken,
                    userId,
                    boardId = this.Get<BoardSettings>().BoardId,
                    showError
                });

        return
            $"{this.Get<BoardSettings>().BaseUrlMask}{digestUrl}";
    }

    /// <summary>
    /// Creates the Digest Mail Message.
    /// </summary>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="digestHtml">
    /// The digest html.
    /// </param>
    /// <param name="boardAddress">
    /// The board Address.
    /// </param>
    /// <param name="toEmail">
    /// The to email.
    /// </param>
    /// <param name="toName">
    /// The to name.
    /// </param>
    /// <returns>
    /// The <see cref="MimeMessage"/>.
    /// </returns>
    public MimeMessage CreateDigestMessage(
        [NotNull] string subject,
        [NotNull] string digestHtml,
        [NotNull] MailboxAddress boardAddress,
        [NotNull] string toEmail,
        [CanBeNull] string toName)
    {
        CodeContracts.VerifyNotNull(digestHtml);
        CodeContracts.VerifyNotNull(boardAddress);
        CodeContracts.VerifyNotNull(toEmail);

        return this.Get<IMailService>().CreateMessage(
            boardAddress,
            new MailboxAddress(toName, toEmail),
            boardAddress,
            subject,
            "You must have HTML Email Viewer to View.",
            digestHtml);
    }
}