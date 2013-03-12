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
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Spam Checking Class for the StopForumSpam.com API
    /// </summary>
    public class StopForumSpam : IBotCheck
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
        public bool CheckForBot([NotNull] object ipAddress, [NotNull] object emailAddress, [NotNull] object userName)
        {
            try
            {
                var url = "http://www.stopforumspam.com/api?ip={0}&email={1}&username={2}".FormatWith(
                    ipAddress, emailAddress, userName);

                var req = (HttpWebRequest)WebRequest.Create(url);

                var res = (HttpWebResponse)req.GetResponse();

                var sr = new StreamReader(res.GetResponseStream());

                var value = sr.ReadToEnd();

                return value.ToLowerInvariant().Contains("<appears>yes</appears>");
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}