/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Services
{
    #region Using

    using System.Net;
    using System.Net.Mail;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF digest.
    /// </summary>
    public class Digest : IDigest, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Digest"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public Digest([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; }

        #endregion

        #region Implemented Interfaces

        #region IDigest

        /// <summary>
        /// Gets the digest HTML.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
        /// <returns>
        /// The get digest html.
        /// </returns>
        public string GetDigestHtml(int userId, object boardSettings, bool showErrors = false)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.GetDigestUrl(userId, boardSettings, showErrors));

            var digestHtml = string.Empty;

            // set timeout to max 45 seconds
            request.Timeout = 45 * 1000;

            var response = request.GetResponse().ToClass<HttpWebResponse>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                digestHtml = response.GetResponseStream().AsString();
            }

            return digestHtml;
        }

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="showErrors">Show errors creating the digest.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        public string GetDigestUrl(int userId, object boardSettings, bool showErrors)
        {
            var yafBoardSettings = boardSettings as BoardSettings;

            return
                $"{yafBoardSettings.BaseUrlMask}{BaseUrlBuilder.AppPath}digest.aspx?token={yafBoardSettings.WebServiceToken}&userid={userId}&boardid={yafBoardSettings.BoardID}&showerror={showErrors.ToString().ToLower()}";
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
        /// <param name="boardMessage">
        /// The board Message.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <returns>
        /// The <see cref="MailMessage"/>.
        /// </returns>
        public MailMessage CreateDigestMessage(
            [NotNull] string subject,
            [NotNull] string digestHtml,
            [NotNull] MailAddress boardMessage,
            [NotNull] string toEmail,
            [CanBeNull] string toName)
        {
            CodeContracts.VerifyNotNull(digestHtml, "digestHtml");
            CodeContracts.VerifyNotNull(boardMessage, "boardMessage");
            CodeContracts.VerifyNotNull(toEmail, "toEmail");

            return this.Get<ISendMail>().CreateMessage(
                boardMessage,
                new MailAddress(toEmail, toName),
                boardMessage,
                subject,
                "You must have HTML Email Viewer to View.",
                digestHtml);
        }

        #endregion

        #endregion
    }
} 