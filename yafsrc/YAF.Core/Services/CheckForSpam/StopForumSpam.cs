/* Yet Another Foru.NET
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

namespace YAF.Core.Services.CheckForSpam
{
    #region

    using System;
    using System.IO;
    using System.Net;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Spam Checking Class for the StopForumSpam.com API
    /// </summary>
    public class StopForumSpam : ICheckForBot
    {
        /// <summary>
        /// Checks if user is a Bot.
        /// </summary>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        /// Returns if user is a possible Bot or not
        /// </returns>
        public bool IsBot([CanBeNull] string ipAddress, [CanBeNull] string emailAddress, [CanBeNull] string userName)
        {
            string responseText;
            return this.IsBot(ipAddress, emailAddress, userName, out responseText);
        }

        /// <summary>
        /// Checks if user is a Bot.
        /// </summary>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="responseText">The response text.</param>
        /// <returns>
        /// Returns if user is a possible Bot or not
        /// </returns>
        public bool IsBot(
            [CanBeNull] string ipAddress,
            [CanBeNull] string emailAddress,
            [CanBeNull] string userName,
            out string responseText)
        {
            responseText = string.Empty;
            try
            {
                var url =
                    "http://www.stopforumspam.com/api?{0}{1}{2}".FormatWith(
                        ipAddress.IsSet() ? "ip={0}".FormatWith(ipAddress) : string.Empty,
                        emailAddress.IsSet() ? "&email={0}".FormatWith(emailAddress) : string.Empty,
                        userName.IsSet() ? "&username={0}".FormatWith(userName) : string.Empty);

                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                var response = (HttpWebResponse)webRequest.GetResponse();

                var streamReader = new StreamReader(response.GetResponseStream());

                responseText = streamReader.ReadToEnd();

                return responseText.ToLowerInvariant().Contains("<appears>yes</appears>");
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Bot");

                return false;
            }
        }
    }
}