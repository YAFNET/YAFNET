/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Services.CheckForSpam
{
    #region

    using System;
    using System.IO;
    using System.Net;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.CheckForSpam;

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
            try
            {
                const string BotScoutUrl = "http://www.botscout.com/test/?multi";

                var url =
                    $"{BotScoutUrl}{(ipAddress.IsSet() ? $"&ip={ipAddress}" : string.Empty)}{(emailAddress.IsSet() ? $"&mail={emailAddress}" : string.Empty)}{(userName.IsSet() ? $"&name={userName}" : string.Empty)}{(BoardContext.Current.Get<BoardSettings>().BotScoutApiKey.IsSet() ? $"&key={BoardContext.Current.Get<BoardSettings>().BotScoutApiKey}" : string.Empty)}";

                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                var response = (HttpWebResponse)webRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = streamReader.ReadToEnd();
                }

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
                BoardContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Bot");

                responseText = ex.Message;

                return false;
            }
        }
    }
}