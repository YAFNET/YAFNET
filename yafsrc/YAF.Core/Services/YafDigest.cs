﻿/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Services
{
    #region Using

    using System.Net;
    using System.Text.RegularExpressions;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf digest.
    /// </summary>
    public class YafDigest : IDigest, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafDigest"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public YafDigest([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; protected set; }

        #endregion

        #region Implemented Interfaces

        #region IDigest

        /// <summary>
        /// The get digest html.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
        /// <returns>
        /// The get digest html.
        /// </returns>
        public string GetDigestHtml(int userId, int boardId, bool showErrors = false)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.GetDigestUrl(userId, boardId, showErrors));

            string digestHtml = string.Empty;

            // set timeout to max 10 seconds
            request.Timeout = 10 * 1000;
            var response = request.GetResponse().ToClass<HttpWebResponse>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                digestHtml = response.GetResponseStream().AsString();
            }

            return digestHtml;
        }

        /// <summary>
        /// The get digest url.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        public string GetDigestUrl(int userId, int boardId)
        {
            return "{0}{1}{2}?{3}".FormatWith(
                BaseUrlBuilder.BaseUrl,
                BaseUrlBuilder.AppPath,
                "digest.aspx",
                "token={0}&userid={1}&boardid={2}".FormatWith(
                    this.Get<YafBoardSettings>().WebServiceToken, userId, boardId));
        }

        /// <summary>
        /// The get digest url.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="showErrors">Show errors creating the digest.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        public string GetDigestUrl(int userId, int boardId, bool showErrors)
        {
            return "{0}{1}{2}?{3}".FormatWith(
                BaseUrlBuilder.BaseUrl,
                BaseUrlBuilder.AppPath,
                "digest.aspx",
                "token={0}&userid={1}&boardid={2}&showerror={3}".FormatWith(
                    this.Get<YafBoardSettings>().WebServiceToken, userId, boardId, showErrors.ToString().ToLower()));
        }

        /// <summary>
        /// Sends the digest html to the email/name specified.
        /// </summary>
        /// <param name="digestHtml">
        /// The digest html.
        /// </param>
        /// <param name="forumName">
        /// The forum name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="sendQueued">
        /// The send queued.
        /// </param>
        public void SendDigest(
            [NotNull] string digestHtml,
            [NotNull] string forumName,
            [NotNull] string toEmail,
            [CanBeNull] string toName,
            bool sendQueued)
        {
            CodeContracts.ArgumentNotNull(digestHtml, "digestHtml");
            CodeContracts.ArgumentNotNull(forumName, "forumName");
            CodeContracts.ArgumentNotNull(toEmail, "toEmail");

            string subject = "Active Topics and New Topics on {0}".FormatWith(forumName);
            var match = Regex.Match(
                digestHtml, @"\<title\>(?<inner>(.*?))\<\/title\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Groups["inner"] != null)
            {
                subject = match.Groups["inner"].Value.Trim();
            }

            if (sendQueued)
            {
                // queue to send...
                this.Get<ISendMail>().Queue(
                    this.Get<YafBoardSettings>().ForumEmail,
                    this.Get<YafBoardSettings>().Name,
                    toEmail,
                    toName,
                    subject,
                    "You must have HTML Email Viewer to View.",
                    digestHtml);
            }
            else
            {
                // send direct...
                this.Get<ISendMail>().Send(
                    this.Get<YafBoardSettings>().ForumEmail,
                    this.Get<YafBoardSettings>().Name,
                    toEmail,
                    toName,
                    subject,
                    "You must have HTML Email Viewer to View.",
                    digestHtml);
            }
        }

        #endregion

        #endregion
    }
}