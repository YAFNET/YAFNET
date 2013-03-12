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
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Spam Checking Class for the BotScout.com API
    /// </summary>
    public class BotScout : IBotCheck
    {
        /// <summary>
        /// Checks if user is a Bot.
        /// </summary>
        /// <param name="ipAddress">
        /// The ip address.
        /// </param>
        /// <param name="emailAddress">
        /// The email Address.
        /// </param>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <returns>
        /// Returns if user is a possible Bot or not
        /// </returns>
        public bool CheckForBot(object ipAddress, object emailAddress, object userName)
        {
            try
            {
                var apiKey = Config.BotScoutApiKey.IsSet() ? "&key={0}".FormatWith(Config.BotScoutApiKey) : string.Empty;

                var url = "http://botscout.com/test/?multi&ip={0}&mail={1}&name={2}{3}".FormatWith(
                    ipAddress, emailAddress, userName, apiKey);

                var req = (HttpWebRequest)WebRequest.Create(url);

                var res = (HttpWebResponse)req.GetResponse();
                var sr = new StreamReader(res.GetResponseStream());

                var value = sr.ReadToEnd();

                return value.StartsWith("Y|");
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}