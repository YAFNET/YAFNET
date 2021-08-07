/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Configuration;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Class with link building functions.
    /// </summary>
    public class LinkBuilder : IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkBuilder"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public LinkBuilder([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets full URL to the Root of the Forum
        /// </summary>
        public string ForumUrl => this.Get<LinkBuilder>().GetLink(ForumPages.Board, true);

        /// <summary>
        /// Gets the safe raw URL.
        /// </summary>
        /// <returns>Returns the safe raw URL</returns>
        public string GetSafeRawUrl()
        {
            return this.Get<LinkBuilder>().GetSafeRawUrl(this.Get<HttpContextBase>().Request.RawUrl);
        }

        /// <summary>
        /// Cleans up a URL so that it doesn't contain any problem characters.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The get safe raw URL.
        /// </returns>
        [NotNull]
        public string GetSafeRawUrl([NotNull] string url)
        {
            CodeContracts.VerifyNotNull(url, "url");

            var processedRaw = url;
            processedRaw = processedRaw.Replace("\"", string.Empty);
            processedRaw = processedRaw.Replace("<", "%3C");
            processedRaw = processedRaw.Replace(">", "%3E");
            processedRaw = processedRaw.Replace("&", "%26");
            return processedRaw.Replace("'", string.Empty);
        }

        /// <summary>
        /// Function that verifies a string is an integer value or it redirects to invalid "info" page.
        /// Used as a security feature against invalid values submitted to the page.
        /// </summary>
        /// <param name="intValue">
        /// The string value to test
        /// </param>
        /// <returns>
        /// The converted integer value
        /// </returns>
        public int StringToIntOrRedirect(string intValue)
        {
            if (!int.TryParse(intValue, out var value))
            {
                // it's an invalid request. Redirect to the info page on invalid requests.
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            return value;
        }

        /// <summary>
        /// The get user profile link.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetUserProfileLink(int userId, string userName)
        {
            return this.Get<LinkBuilder>().GetLink(ForumPages.UserProfile, "u={0}&name={1}", userId, userName);
        }

        /// <summary>
        /// The get forum link.
        /// </summary>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="forumName">
        /// The forum name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetForumLink(int forumId, string forumName)
        {
            return this.Get<LinkBuilder>().GetLink(ForumPages.Topics, "f={0}&name={1}", forumId, forumName);
        }

        /// <summary>
        /// The get category link.
        /// </summary>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="categoryName">
        /// The category name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCategoryLink(int categoryId, string categoryName)
        {
            return this.Get<LinkBuilder>().GetLink(ForumPages.Board, "c={0}&name={1}", categoryId, categoryName)
                .Replace("&amp;", "&");
        }

        /// <summary>
        /// The get topic link.
        /// </summary>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="topicName">
        /// The topic name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTopicLink(int topicId, string topicName)
        {
            return this.Get<LinkBuilder>().GetLink(ForumPages.Posts, "t={0}&name={1}", topicId, topicName)
                .Replace("&amp;", "&");
        }

        /// <summary>
        /// Gets base path to the page without ampersand.
        /// </summary>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public string GetBasePath()
        {
            return FactoryProvider.UrlBuilder.BuildUrl(string.Empty).TrimEnd('&');
        }

        /// <summary>
        /// Gets base path to the page without ampersand.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public string GetBasePath(BoardSettings boardSettings)
        {
            return FactoryProvider.UrlBuilder.BuildUrl(boardSettings, string.Empty).TrimEnd('&');
        }

        /// <summary>
        /// Redirects response to the access denied page.
        /// </summary>
        public void AccessDenied()
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Info, "i=4");
        }

        /// <summary>
        /// Gets link to the page.
        /// </summary>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <returns>
        /// URL to the given page.
        /// </returns>
        public string GetLink(ForumPages page, bool fullUrl = false)
        {
            return fullUrl
                ? FactoryProvider.UrlBuilder.BuildUrlFull($"g={page}")
                : FactoryProvider.UrlBuilder.BuildUrl($"g={page}");
        }

        /// <summary>
        /// Gets link to the page.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <returns>
        /// URL to the given page.
        /// </returns>
        public string GetLink(BoardSettings boardSettings, ForumPages page, bool fullUrl = false)
        {
            return fullUrl
                ? FactoryProvider.UrlBuilder.BuildUrlFull(boardSettings, $"g={page}")
                : FactoryProvider.UrlBuilder.BuildUrl(boardSettings, $"g={page}");
        }

        /// <summary>
        /// Gets link to the page with given parameters.
        /// </summary>
        /// <param name="page">
        /// Page to which to create a link.
        /// </param>
        /// <param name="fullUrl">
        /// The full Url.
        /// </param>
        /// <param name="format">
        /// Format of parameters.
        /// </param>
        /// <param name="args">
        /// Array of page parameters.
        /// </param>
        /// <returns>
        /// URL to the given page with parameters.
        /// </returns>
        public string GetLink(ForumPages page, bool fullUrl, string format, params object[] args)
        {
            return fullUrl
                ? FactoryProvider.UrlBuilder.BuildUrlFull($"g={page}&{string.Format(format, args)}")
                : FactoryProvider.UrlBuilder.BuildUrl($"g={page}&{string.Format(format, args)}");
        }

        /// <summary>
        /// Gets link to the page with given parameters.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">The full Url.</param>
        /// <param name="format">Format of parameters.</param>
        /// <param name="args">Array of page parameters.</param>
        /// <returns>
        /// URL to the given page with parameters.
        /// </returns>
        public string GetLink(
            BoardSettings boardSettings,
            ForumPages page,
            bool fullUrl,
            string format,
            params object[] args)
        {
            return fullUrl
                ? FactoryProvider.UrlBuilder.BuildUrlFull(boardSettings, $"g={page}&{string.Format(format, args)}")
                : FactoryProvider.UrlBuilder.BuildUrl(boardSettings, $"g={page}&{string.Format(format, args)}");
        }

        /// <summary>
        /// Gets link to the page with given parameters.
        /// </summary>
        /// <param name="page">
        /// Page to which to create a link.
        /// </param>
        /// <param name="format">
        /// Format of parameters.
        /// </param>
        /// <param name="args">
        /// Array of page parameters.
        /// </param>
        /// <returns>
        /// URL to the given page with parameters.
        /// </returns>
        public string GetLink(ForumPages page, string format, params object[] args)
        {
            return this.Get<LinkBuilder>().GetLink(page, false, format, args).Replace("&amp;", "&");
        }

        /// <summary>
        /// Redirects to the given page.
        /// </summary>
        /// <param name="page">
        /// Page to which to redirect response.
        /// </param>
        public void Redirect(ForumPages page)
        {
            this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page).Replace("&amp;", "&"));
        }

        /// <summary>
        /// Redirects to the given page with parameters.
        /// </summary>
        /// <param name="page">
        /// Page to which to redirect response.
        /// </param>
        /// <param name="format">
        /// Format of parameters.
        /// </param>
        /// <param name="args">
        /// Array of page parameters.
        /// </param>
        public void Redirect(ForumPages page, string format, params object[] args)
        {
            this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page, format, args).Replace("&amp;", "&"));
        }

        /// <summary>
        /// Redirects to the given page with parameters.
        /// </summary>
        /// <param name="page">
        /// Page to which to redirect response.
        /// </param>
        /// <param name="endResponse">True to end the Response, false otherwise.</param>
        /// <param name="format">
        /// Format of parameters.
        /// </param>
        /// <param name="args">
        /// Array of page parameters.
        /// </param>
        public void Redirect(ForumPages page, bool endResponse, string format, params object[] args)
        {
            this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page, format, args), endResponse);
        }

        /// <summary>
        /// Redirects response to the info page.
        /// </summary>
        /// <param name="infoMessage">
        /// The info Message.
        /// </param>
        public void RedirectInfoPage(InfoMessage infoMessage)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Info, $"i={infoMessage.ToType<int>()}");
        }

        #endregion
    }
}