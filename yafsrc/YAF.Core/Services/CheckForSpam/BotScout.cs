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

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Spam Checking Class for the BotScout.com API
    /// </summary>
    public class BotScout : ICheckForBot
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
            try
            {
                var url =
                    "http://www.botscout.com/test/?multi{0}{1}{2}{3}".FormatWith(
                        ipAddress.IsSet() ? "&ip={0}".FormatWith(ipAddress) : string.Empty,
                        emailAddress.IsSet() ? "&mail={0}".FormatWith(emailAddress) : string.Empty,
                        userName.IsSet() ? "&name={0}".FormatWith(userName) : string.Empty,
                        YafContext.Current.Get<YafBoardSettings>().BotScoutApiKey.IsSet()
                            ? "&key={0}".FormatWith(YafContext.Current.Get<YafBoardSettings>().BotScoutApiKey)
                            : string.Empty);

                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                var response = (HttpWebResponse)webRequest.GetResponse();

                var streamReader = new StreamReader(response.GetResponseStream());

                responseText = streamReader.ReadToEnd();

                if (!responseText.StartsWith("Y|"))
                {
                    return false;
                }

                // Match name + email address
                if (!responseText.Contains("NAME|0") && !responseText.Contains("MAIL|0"))
                {
                    return true;
                }

                // Match name + IP address
                if (!responseText.Contains("NAME|0") && !responseText.Contains("IP|0"))
                {
                    return true;
                }

                // Match IP + email address
                return !responseText.Contains("IP|0") && !responseText.Contains("MAIL|0");
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Bot");

                responseText = ex.Message;

                return false;
            }
        }
    }
}