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
namespace YAF.Utils
{
    #region Using

    using System.Web;

    using YAF.Configuration;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Static class with link building functions.
    /// </summary>
    public static class BuildLink
    {
        #region Public Methods

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
        public static string GetUserProfileLink(int userId, string userName)
        {
            return GetLink(ForumPages.UserProfile, "u={0}&name={1}", userId, userName);
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
        public static string GetForumLink(int forumId, string forumName)
        {
            return GetLink(ForumPages.Topics, "f={0}&name={1}", forumId, forumName);
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
        public static string GetCategoryLink(int categoryId, string categoryName)
        {
            return GetLink(ForumPages.Board, "c={0}&name={1}", categoryId, categoryName).Replace("&amp;", "&");
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
        public static string GetTopicLink(int topicId, string topicName)
        {
            return GetLink(ForumPages.Posts, "t={0}&name={1}", topicId, topicName).Replace("&amp;", "&");
        }

        /// <summary>
        /// Gets base path to the page without ampersand.
        /// </summary>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public static string GetBasePath()
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
        public static string GetBasePath(BoardSettings boardSettings)
        {
            return FactoryProvider.UrlBuilder.BuildUrl(boardSettings, string.Empty).TrimEnd('&');
        }

        /// <summary>
        /// Redirects response to the access denied page.
        /// </summary>
        public static void AccessDenied()
        {
            Redirect(ForumPages.Info, "i=4");
        }

        /// <summary>
        /// Gets link to the page.
        /// </summary>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <returns>
        /// URL to the given page.
        /// </returns>
        public static string GetLink(ForumPages page, bool fullUrl = false)
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
        public static string GetLink(BoardSettings boardSettings, ForumPages page, bool fullUrl = false)
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
        public static string GetLink(ForumPages page, bool fullUrl, string format, params object[] args)
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
        public static string GetLink(BoardSettings boardSettings, ForumPages page, bool fullUrl, string format, params object[] args)
        {
            return fullUrl
                       ? FactoryProvider.UrlBuilder.BuildUrlFull(
                           boardSettings,
                           $"g={page}&{string.Format(format, args)}")
                       : FactoryProvider.UrlBuilder.BuildUrl(
                           boardSettings,
                           $"g={page}&{string.Format(format, args)}");
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
        public static string GetLink(ForumPages page, string format, params object[] args)
        {
            return GetLink(page, false, format, args).Replace("&amp;", "&");
        }

        /// <summary>
        /// Redirects to the given page.
        /// </summary>
        /// <param name="page">
        /// Page to which to redirect response.
        /// </param>
        public static void Redirect(ForumPages page)
        {
            HttpContext.Current.Response.Redirect(GetLink(page).Replace("&amp;", "&"));
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
        public static void Redirect(ForumPages page, string format, params object[] args)
        {
            HttpContext.Current.Response.Redirect(GetLink(page, format, args).Replace("&amp;", "&"));
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
        public static void Redirect(ForumPages page, bool endResponse, string format, params object[] args)
        {
            HttpContext.Current.Response.Redirect(GetLink(page, format, args), endResponse);
        }

        /// <summary>
        /// Redirects response to the info page.
        /// </summary>
        /// <param name="infoMessage">
        /// The info Message.
        /// </param>
        public static void RedirectInfoPage(InfoMessage infoMessage)
        {
            Redirect(ForumPages.Info, $"i={infoMessage.ToType<int>()}");
        }

        #endregion
    }
}