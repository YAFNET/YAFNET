/* Yet Another Forum.net
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
        /// <param name="fullUrl">if set to <c>true</c> [full URL].</param>
        /// <returns>
        /// Base URL to the given page.
        /// </returns>
        public static string GetBasePath(bool fullUrl = false)
        {
            return fullUrl
                       ? YafFactoryProvider.UrlBuilder.BuildUrlFull("").TrimEnd('&')
                       : YafFactoryProvider.UrlBuilder.BuildUrl("").TrimEnd('&');
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
            Redirect(ForumPages.info, "i={0}".FormatWith((int)infoMessage));
        }

        /// <summary>
        /// Gets URL of given smiley.
        /// </summary>
        /// <param name="icon">
        /// Name of icon image file.
        /// </param>
        /// <returns>
        /// URL of a smiley.
        /// </returns>
        public static string Smiley(string icon)
        {
            return "{0}{1}/{2}".FormatWith(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, icon);
        }

        #endregion
    }
}