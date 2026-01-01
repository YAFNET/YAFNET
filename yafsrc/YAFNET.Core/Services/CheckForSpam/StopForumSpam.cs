/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Types.Interfaces.CheckForSpam;
using YAF.Types.Objects;

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
    /// <returns>Returns if User is Bot or Not</returns>
    public async Task<(string ResponseText, bool IsBot)> IsBotAsync(
        string ipAddress,
        string emailAddress,
        string userName)
    {
        var responseText = string.Empty;

        try
        {
            var url =
                $"https://www.stopforumspam.com/api?{(ipAddress.IsSet() ? $"ip={ipAddress}" : string.Empty)}{(emailAddress.IsSet() ? $"&email={emailAddress}" : string.Empty)}{(userName.IsSet() ? $"&username={userName}" : string.Empty)}&f=json";

            var client = new HttpClient(new HttpClientHandler());

            var stopForumResponse = await client.GetFromJsonAsync<StopForumSpamResponse>(url);

            if (!stopForumResponse.Success)
            {
                return (responseText, false);
            }

            switch (stopForumResponse.UserName.Appears)
            {
                // Match name + email address
                case true when stopForumResponse.Email.Appears:
                // Match name + IP address
                case true when stopForumResponse.IpAddress.Appears:
                    return (responseText, true);
                default:
                    // Match IP + email address
                    return (responseText, stopForumResponse.IpAddress.Appears && stopForumResponse.Email.Appears);
            }
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILogger<StopForumSpam>>().Error(ex, "Error while Checking for Bot");

            return (responseText, false);
        }
    }

    /// <summary>
    /// Reports the user as bot.
    /// </summary>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="userName">Name of the user.</param>
    /// <returns>Returns If the report was successful or not</returns>
    public async Task<bool> ReportUserAsBotAsync(
        string ipAddress,
        string emailAddress,
        string userName)
    {
        var parameters =
            $"username={userName}&ip_addr={ipAddress}&email={emailAddress}&api_key={BoardContext.Current.BoardSettings.StopForumSpamApiKey}";

        var client = new HttpClient(new HttpClientHandler());
        var response = await client.GetAsync($"https://www.stopforumspam.com/add.php?{parameters}");

        var result = await response.Content.ReadAsStringAsync();

        if (!result.Contains("success"))
        {
            BoardContext.Current.Get<ILogger<StopForumSpam>>().Log(
                null,
                " Report to StopForumSpam.com Failed",
                result);
        }

        return result.Contains("success");
    }
}