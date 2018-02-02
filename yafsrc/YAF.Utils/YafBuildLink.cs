/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Classes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Enumerates forum info messages.
    /// </summary>
    public enum InfoMessage
    {
        /// <summary>
        /// after posting to moderated forum
        /// </summary>
        Moderated = 1,

        /// <summary>
        /// informs user he's suspended
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// informs user about registration email being sent
        /// </summary>
        RegistrationEmail = 3,

        /// <summary>
        /// access was denied
        /// </summary>
        AccessDenied = 4,

        /// <summary>
        /// informs user about feature being disabled by admin 
        /// </summary>
        Disabled = 5,

        /// <summary>
        /// informs user about invalid input/request
        /// </summary>
        Invalid = 6,

        /// <summary>
        /// system error
        /// </summary>
        Failure = 7,

        /// <summary>
        /// requires cookies
        /// </summary>
        RequiresCookies = 8,

        /// <summary>
        /// requires JS
        /// </summary>
        RequiresEcmaScript = 9,

        /// <summary>
        /// unsupported JS version
        /// </summary>
        EcmaScriptVersionUnsupported = 10,

        /// <summary>
        /// The message for admin to ask access for admin pages viewing.
        /// </summary>
        HostAdminPermissionsAreRequired = 11
    }

    /// <summary>
    /// Static class with link building functions.
    /// </summary>
    public static class YafBuildLink
    {
        #region Public Methods

        /// <summary>
        /// Gets base path to the page without ampersand.
        /// </summary>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public static string GetBasePath()
        {
            return YafFactoryProvider.UrlBuilder.BuildUrl(string.Empty).TrimEnd('&');
        }

        /// <summary>
        /// Gets base path to the page without ampersand.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public static string GetBasePath(YafBoardSettings boardSettings)
        {
            return YafFactoryProvider.UrlBuilder.BuildUrl(boardSettings, string.Empty).TrimEnd('&');
        }

        /// <summary>
        /// Redirects response to the access denied page.
        /// </summary>
        public static void AccessDenied()
        {
            Redirect(ForumPages.info, "i=4");
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
                       ? YafFactoryProvider.UrlBuilder.BuildUrlFull("g={0}".FormatWith(page))
                       : YafFactoryProvider.UrlBuilder.BuildUrl("g={0}".FormatWith(page));
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
        public static string GetLink(YafBoardSettings boardSettings, ForumPages page, bool fullUrl = false)
        {
            return fullUrl
                       ? YafFactoryProvider.UrlBuilder.BuildUrlFull(boardSettings, "g={0}".FormatWith(page))
                       : YafFactoryProvider.UrlBuilder.BuildUrl(boardSettings, "g={0}".FormatWith(page));
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
                       ? YafFactoryProvider.UrlBuilder.BuildUrlFull(
                           "g={0}&{1}".FormatWith(page, format.FormatWith(args)))
                       : YafFactoryProvider.UrlBuilder.BuildUrl("g={0}&{1}".FormatWith(page, format.FormatWith(args)));
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
        public static string GetLink(YafBoardSettings boardSettings, ForumPages page, bool fullUrl, string format, params object[] args)
        {
            return fullUrl
                       ? YafFactoryProvider.UrlBuilder.BuildUrlFull(
                           boardSettings,
                           "g={0}&{1}".FormatWith(page, format.FormatWith(args)))
                       : YafFactoryProvider.UrlBuilder.BuildUrl(
                           boardSettings,
                           "g={0}&{1}".FormatWith(page, format.FormatWith(args)));
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
            return GetLink(page, false, format, args);
        }

        /// <summary>
        /// Unescapes ampersands in the link to the given page.
        /// </summary>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <returns>
        /// URL to the given page with unescaped ampersands.
        /// </returns>
        public static string GetLinkNotEscaped(ForumPages page, bool fullUrl = false)
        {
            return GetLink(page, fullUrl).Replace("&amp;", "&");
        }

        /// <summary>
        /// Unescapes ampersands in the link to the given page with parameters.
        /// </summary>
        /// <param name="page">Page to which to create a link.</param>
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <param name="format">Format of parameters.</param>
        /// <param name="args">Array of page parameters.</param>
        /// <returns>
        /// URL to the given page with parameters and unescaped ampersands.
        /// </returns>
        public static string GetLinkNotEscaped(ForumPages page, bool fullUrl, string format, params object[] args)
        {
            return GetLink(page, fullUrl, format, args).Replace("&amp;", "&");
        }

        /// <summary>
        /// Unescapes ampersands in the link to the given page with parameters.
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
        /// URL to the given page with parameters and unescaped ampersands.
        /// </returns>
        public static string GetLinkNotEscaped(ForumPages page, string format, params object[] args)
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
            HttpContext.Current.Response.Redirect(GetLinkNotEscaped(page));
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
            HttpContext.Current.Response.Redirect(GetLinkNotEscaped(page, format, args));
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
            HttpContext.Current.Response.Redirect(GetLinkNotEscaped(page, format, args), endResponse);
        }

        /// <summary>
        /// Redirects response to the info page.
        /// </summary>
        /// <param name="infoMessage">
        /// The info Message.
        /// </param>
        public static void RedirectInfoPage(InfoMessage infoMessage)
        {
            Redirect(ForumPages.info, "i={0}".FormatWith(infoMessage.ToType<int>()));
        }

        #endregion
    }
}