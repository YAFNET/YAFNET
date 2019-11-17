/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Core.Services.CheckForSpam
{
    #region

    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
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
            return this.IsBot(ipAddress, emailAddress, userName, out _);
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

                var bannedIpList = YafContext.Current.Get<IDataCache>().GetOrSet(
                    Constants.Cache.BannedIP,
                    () => bannedIPRepository.Get(x => x.BoardID == YafContext.Current.PageBoardID)
                        .Select(x => x.Mask.Trim()).ToList());

                var bannedNameRepository = YafContext.Current.Get<IRepository<BannedName>>();

                var isBot = false;

                foreach (var email in bannedEmailRepository.Get(x => x.BoardID == YafContext.Current.PageBoardID))
                {
                    try
                    {
                        if (!Regex.Match(emailAddress, email.Mask).Success)
                        {
                            continue;
                        }

                        responseText = $"internal detection found email address {emailAddress}";
                        isBot = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>().Error(
                            ex,
                            $"Error while Checking for Bot Email (Check: {email.Mask})");
                    }
                }

                if (bannedIpList.Any(i => i.Equals(ipAddress)))
                {
                    responseText = $"internal detection found ip address {ipAddress}";
                    isBot = true;
                }

                foreach (var name in bannedNameRepository.Get(x => x.BoardID == YafContext.Current.PageBoardID))
                {
                    try
                    {
                        if (!Regex.Match(userName, name.Mask).Success)
                        {
                            continue;
                        }

                        responseText = $"internal detection found name {name.Mask}";
                        isBot = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>().Error(
                            ex,
                            $"Error while Checking for Bot Name (Check: {name.Mask})");
                    }
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