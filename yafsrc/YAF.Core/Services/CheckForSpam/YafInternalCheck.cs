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
    using System.Linq;
    using System.Text.RegularExpressions;

    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Spam Checking Class for the Internal Spam Check
    /// </summary>
    public class YafInternalCheck : ICheckForBot
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
                var bannedEmailRepository = YafContext.Current.Get<IRepository<BannedEmail>>();
                var bannedIPRepository = YafContext.Current.Get<IRepository<BannedIP>>();
                /* var bannedIPs = YafContext.Current.Get<IDataCache>().GetOrSet(
                Constants.Cache.BannedIP,
                () => this.BannedIpRepository.ListTyped().Select(x => x.Mask.Trim()).ToList());*/

                var bannedIpList = YafContext.Current.Get<IDataCache>()
                    .GetOrSet(
                        Constants.Cache.BannedIP,
                        () => bannedIPRepository.ListTyped().Select(x => x.Mask.Trim()).ToList());

                var bannedNameRepository = YafContext.Current.Get<IRepository<BannedName>>();

                var isBot = false;

                if (bannedEmailRepository.ListTyped().Any(email => Regex.Match(emailAddress, email.Mask).Success))
                {
                    responseText = "internal detection found email address {0}".FormatWith(emailAddress);
                    isBot = true;
                }

                if (bannedIpList.Any(i => i.Equals(ipAddress)))
                {
                    responseText = "internal detection found ip address {0}".FormatWith(ipAddress);
                    isBot = true;
                }

                if (bannedNameRepository.ListTyped().Any(name => Regex.Match(userName, name.Mask).Success))
                {
                    responseText = "internal detection found name {0}".FormatWith(userName);
                    isBot = true;
                }

                return isBot;
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Bot");

                return false;
            }
        }
    }
}