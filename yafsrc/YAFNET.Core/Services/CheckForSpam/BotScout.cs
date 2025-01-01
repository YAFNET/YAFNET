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
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Types.Interfaces.CheckForSpam;

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
    /// <returns>Returns Response Text and if User is Bot or Not</returns>
    public async Task<(string ResponseText, bool IsBot)> IsBotAsync(
        string ipAddress,
        string emailAddress,
        string userName)
    {
        string responseText;

        try
        {
            const string BotScoutUrl = "https://www.botscout.com/test/?multi";

            var url =
                $"{BotScoutUrl}{(ipAddress.IsSet() ? $"&ip={ipAddress}" : string.Empty)}{(emailAddress.IsSet() ? $"&mail={emailAddress}" : string.Empty)}{(userName.IsSet() ? $"&name={userName}" : string.Empty)}{(BoardContext.Current.BoardSettings.BotScoutApiKey.IsSet() ? $"&key={BoardContext.Current.BoardSettings.BotScoutApiKey}" : string.Empty)}";

            var client = new HttpClient(new HttpClientHandler());

            var response = await client.GetAsync(url);

            responseText = await response.Content.ReadAsStringAsync();

            if (!responseText.StartsWith("Y|"))
            {
                return (responseText, false);
            }

            // Match name + email address
            if (!responseText.Contains("NAME|0") && !responseText.Contains("MAIL|0"))
            {
                return (responseText, true);
            }

            // Match name + IP address
            if (!responseText.Contains("NAME|0") && !responseText.Contains("IP|0"))
            {
                return (responseText, true);
            }

            // Match IP + email address
            return (responseText, !responseText.Contains("IP|0") && !responseText.Contains("MAIL|0"));
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILogger<BotScout>>().Error(ex, "Error while Checking for Bot");

            responseText = ex.Message;

            return (responseText, false);
        }
    }
}