﻿/* Yet Another Forum.NET
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

using ServiceStack.OrmLite.Base.Text;

namespace YAF.Core.Services.CheckForSpam;

using System;
using System.IO;
using System.Net;

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
    /// <returns>
    /// Returns if user is a possible Bot or not
    /// </returns>
    public bool IsBot(string ipAddress, string emailAddress, string userName)
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
        string ipAddress,
        string emailAddress,
        string userName,
        out string responseText)
    {
        responseText = string.Empty;
        try
        {
            var url =
                $"https://www.stopforumspam.com/api?{(ipAddress.IsSet() ? $"ip={ipAddress}" : string.Empty)}{(emailAddress.IsSet() ? $"&email={emailAddress}" : string.Empty)}{(userName.IsSet() ? $"&username={userName}" : string.Empty)}&f=json";

            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            var response = (HttpWebResponse)webRequest.GetResponse();

            var streamReader = new StreamReader(response.GetResponseStream());

            responseText = streamReader.ReadToEnd();

            var stopForumResponse = responseText.FromJson<StopForumSpamResponse>();

            if (!stopForumResponse.Success)
            {
                return false;
            }

            // Match name + email address
            if (stopForumResponse.UserName.Appears && stopForumResponse.Email.Appears)
            {
                return true;
            }

            // Match name + IP address
            if (stopForumResponse.UserName.Appears && stopForumResponse.IPAddress.Appears)
            {
                return true;
            }

            // Match IP + email address
            return stopForumResponse.IPAddress.Appears && stopForumResponse.Email.Appears;
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILoggerService>().Error(ex, "Error while Checking for Bot");

            return false;
        }
    }

    /// <summary>
    /// Reports the user as bot.
    /// </summary>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="userName">Name of the user.</param>
    /// <returns>Returns If the report was successful or not</returns>
    public bool ReportUserAsBot(
        string ipAddress,
        string emailAddress,
        string userName)
    {
        var parameters =
            $"username={userName}&ip_addr={ipAddress}&email={emailAddress}&api_key={BoardContext.Current.BoardSettings.StopForumSpamApiKey}";

        var result = new HttpClient().PostRequest(
            new Uri("https://www.stopforumspam.com/add.php"),
            null,
            60 * 1000,
            parameters);

        if (!result.Contains("success"))
        {
            BoardContext.Current.Get<ILoggerService>().Log(
                null,
                " Report to StopForumSpam.com Failed",
                result);
        }

        return result.Contains("success");
    }
}