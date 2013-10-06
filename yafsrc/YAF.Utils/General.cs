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

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Web;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Summary description for General Utils.
    /// </summary>
    public static class General
    {
        /* Ederon : 9/12/2007 */

        #region Public Methods

        /// <summary>
        /// Compares two messages.
        /// </summary>
        /// <param name="originalMessage">
        /// Original message text.
        /// </param>
        /// <param name="newMessage">
        /// New message text.
        /// </param>
        /// <returns>
        /// True if messages differ, <see langword="false"/> if they are identical.
        /// </returns>
        public static bool CompareMessage([NotNull] object originalMessage, [NotNull] object newMessage)
        {
            CodeContracts.VerifyNotNull(originalMessage, "originalMessage");
            CodeContracts.VerifyNotNull(newMessage, "newMessage");

            return originalMessage.ToString() != newMessage.ToString();
        }

        /// <summary>
        /// The encode message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The encode message.
        /// </returns>
        public static string EncodeMessage([NotNull] string message)
        {
            CodeContracts.VerifyNotNull(message, "message");

            return message.IndexOf('<') >= 0 ? HttpUtility.HtmlEncode(message) : message;
        }

        /// <summary>
        /// Gets the current ASP.NET Hosting Security Level.
        /// </summary>
        /// <returns>
        /// The get current trust level.
        /// </returns>
        public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            // Gets an override value, useful for Custom Trust Levels
            if (!string.IsNullOrEmpty(Config.OverrideTrustLevel)) 
            {
                return
                    (AspNetHostingPermissionLevel)
                    Enum.Parse(typeof(AspNetHostingPermissionLevel), Config.OverrideTrustLevel, true); // return non custom trust level
            }

            foreach (AspNetHostingPermissionLevel trustLevel in
                new[]
                    {
                        AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High,
                        AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low,
                        AspNetHostingPermissionLevel.Minimal
                    })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }

        /// <summary>
        /// The get safe raw url.
        /// </summary>
        /// <returns>
        /// The get safe raw url.
        /// </returns>
        public static string GetSafeRawUrl()
        {
            return GetSafeRawUrl(HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// Cleans up a URL so that it doesn't contain any problem characters.
        /// </summary>
        /// <param name="url">
        /// </param>
        /// <returns>
        /// The get safe raw url.
        /// </returns>
        [NotNull]
        public static string GetSafeRawUrl([NotNull] string url)
        {
            CodeContracts.VerifyNotNull(url, "url");

            string tProcessedRaw = url;
            tProcessedRaw = tProcessedRaw.Replace("\"", string.Empty);
            tProcessedRaw = tProcessedRaw.Replace("<", "%3C");
            tProcessedRaw = tProcessedRaw.Replace(">", "%3E");
            tProcessedRaw = tProcessedRaw.Replace("&", "%26");
            return tProcessedRaw.Replace("'", string.Empty);
        }

        /// <summary>
        /// The trace resources.
        /// </summary>
        /// <returns>
        /// The trace resources.
        /// </returns>
        public static string TraceResources()
        {
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            // populate the textbox with information about our resources
            // also look for images and put them in our arraylist
            string txtInfo = string.Empty;

            txtInfo += "Found {0} resources\r\n".FormatWith(resNames.Length);
            txtInfo += "----------\r\n";

            txtInfo = resNames.Aggregate(txtInfo, (current, s) => current + (s + "\r\n"));

            txtInfo += "----------\r\n";

            return txtInfo;
        }

        #endregion
    }
}