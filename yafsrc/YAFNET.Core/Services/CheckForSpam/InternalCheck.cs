/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services.CheckForSpam;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Types.Interfaces.CheckForSpam;
using YAF.Types.Models;

/// <summary>
/// Spam Checking Class for the Internal Spam Check
/// </summary>
public class InternalCheck : ICheckForBot
{
    /// <summary>
    /// Checks if user is a Bot.
    /// </summary>
    /// <param name="ipAddress">The IP Address.</param>
    /// <param name="emailAddress">The email Address.</param>
    /// <param name="userName">Name of the user.</param>
    /// <returns>Returns Response Text and if User is Bot or Not</returns>
    public Task<(string ResponseText, bool IsBot)> IsBotAsync(
        string ipAddress,
        string emailAddress,
        string userName)
    {
        var responseText = string.Empty;

        try
        {
            var bannedEmailRepository = BoardContext.Current.GetRepository<BannedEmail>();
            var bannedIpRepository = BoardContext.Current.GetRepository<BannedIP>();

            var bannedIpList = BoardContext.Current.Get<IDataCache>().GetOrSet(
                Constants.Cache.BannedIP,
                () => bannedIpRepository.Get(x => x.BoardID == BoardContext.Current.PageBoardID).Select(x => x.Mask.Trim()).ToList());

            var bannedNameRepository = BoardContext.Current.GetRepository<BannedName>();

            var isBot = false;

            try
            {
                var banned = bannedEmailRepository.Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Find(b => Regex.Match(emailAddress, b.Mask, RegexOptions.None,
                        TimeSpan.FromMilliseconds(100)).Success);

                if (banned != null)
                {
                    responseText = $"internal detection found email address {emailAddress}";
                    isBot = true;
                }
            }
            catch (Exception ex)
            {
                BoardContext.Current.Get<ILogger<InternalCheck>>().Error(ex, "Error while Checking for Bot Email");
            }

            if (bannedIpList.Contains(ipAddress))
            {
                responseText = $"internal detection found ip address {ipAddress}";
                isBot = true;
            }

            foreach (var mask in bannedNameRepository.Get(x => x.BoardID == BoardContext.Current.PageBoardID).Select(x => x.Mask))
            {
                try
                {
                    if (!Regex.Match(userName, mask, RegexOptions.None,
                            TimeSpan.FromMilliseconds(100)).Success)
                    {
                        continue;
                    }

                    responseText = $"internal detection found name {mask}";
                    isBot = true;
                    break;
                }
                catch (Exception ex)
                {
                    isBot = false;

                    BoardContext.Current.Get<ILogger<InternalCheck>>().Error(
                        ex,
                        $"Error while Checking for Bot Name (Check: {mask})");
                }
            }

            return Task.FromResult((responseText, isBot));
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILogger<InternalCheck>>().Error(ex, "Error while Checking for Bot");

            return Task.FromResult((responseText, false));
        }
    }
}