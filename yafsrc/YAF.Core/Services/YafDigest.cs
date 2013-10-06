/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
        /// Gets the digest HTML.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
        /// <returns>
        /// The get digest html.
        /// </returns>
        public string GetDigestHtml(int userId, int boardId, string webServiceToken, bool showErrors = false)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.GetDigestUrl(userId, boardId, webServiceToken, showErrors));

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
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        public string GetDigestUrl(int userId, int boardId, string webServiceToken)
        {
            return "{0}{1}{2}?{3}".FormatWith(
                BaseUrlBuilder.BaseUrl,
                BaseUrlBuilder.AppPath,
                "digest.aspx",
                "token={0}&userid={1}&boardid={2}".FormatWith(webServiceToken, userId, boardId));
        }

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <param name="showErrors">Show errors creating the digest.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        public string GetDigestUrl(int userId, int boardId, string webServiceToken, bool showErrors)
        {
            return "{0}{1}{2}?{3}".FormatWith(
                BaseUrlBuilder.BaseUrl,
                BaseUrlBuilder.AppPath,
                "digest.aspx",
                "token={0}&userid={1}&boardid={2}&showerror={3}".FormatWith(webServiceToken, userId, boardId, showErrors.ToString().ToLower()));
        }

        /// <summary>
        /// Sends the digest html to the email/name specified.
        /// </summary>
        /// <param name="digestHtml">The digest html.</param>
        /// <param name="forumName">The forum name.</param>
        /// <param name="forumEmail">The forum email.</param>
        /// <param name="toEmail">The to email.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="sendQueued">The send queued.</param>
        public void SendDigest(
            [NotNull] string digestHtml,
            [NotNull] string forumName,
            [NotNull] string forumEmail,
            [NotNull] string toEmail,
            [CanBeNull] string toName,
            bool sendQueued)
        {
            CodeContracts.VerifyNotNull(digestHtml, "digestHtml");
            CodeContracts.VerifyNotNull(forumName, "forumName");
            CodeContracts.VerifyNotNull(forumEmail, "forumEmail");
            CodeContracts.VerifyNotNull(toEmail, "toEmail");

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
                this.GetRepository<Mail>().Create(
                    forumEmail,
                    forumName,
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
                    forumEmail,
                    forumName,
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