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

namespace YAF.Core.Services;

using System.Threading.Tasks;

using YAF.Core.Services.CheckForSpam;

/// <summary>
/// PageUser and Content Spam Checking
/// </summary>
public class SpamCheck : ISpamCheck, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpamCheck"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public SpamCheck(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Check a Post for SPAM against the internal Spam Words
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="postMessage">The post message.</param>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="result">The result.</param>
    /// <returns>
    /// Returns if Post is SPAM or not
    /// </returns>
    public bool CheckPostForSpam(
        string userName,
        string ipAddress,
        string postMessage,
        string emailAddress,
        out string result)
    {
        result = string.Empty;

        if (BoardContext.Current.PageUser.NumPosts >= this.Get<BoardSettings>().IgnoreSpamWordCheckPostCount)
        {
            return false;
        }

        var isSpamContent = false;

        if (this.Get<BoardSettings>().SpamService is not SpamService.NoService)
        {
            isSpamContent = this.Get<ISpamWordCheck>().CheckForSpamWord(postMessage, out result);
        }

        if (isSpamContent)
        {
            return true;
        }

        // Check for Urls
        isSpamContent = this.Get<ISpamCheck>().ContainsSpamUrls(postMessage, out result);

        return isSpamContent;
    }

    /// <summary>
    /// Check a PageUser (Bot) against the StopForumSpam, BotScout Service or both
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="ipAddress">The IP address.</param>
    /// <returns>
    /// Returns if Post is SPAM or not
    /// </returns>
    public async Task<(string Result, bool IsBot)> CheckUserForSpamBotAsync(
        string userName,
        string emailAddress,
        string ipAddress)
    {
        // Check internal
        var internalCheck = new InternalCheck();

        var botResult = await internalCheck.IsBotAsync(ipAddress, emailAddress, userName);

        var isInternalFoundBot = botResult.IsBot;
        var result = botResult.ResponseText;

        if (isInternalFoundBot)
        {
            return (result, true);
        }

        if (this.Get<BoardSettings>().BotSpamService == BotSpamService.NoService)
        {
            return (result, false);
        }

        switch (this.Get<BoardSettings>().BotSpamService)
        {
            case BotSpamService.StopForumSpam:
                {
                    var stopForumSpam = new StopForumSpam();

                    return await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                }

            case BotSpamService.BotScout:
                {
                    if (this.Get<BoardSettings>().BotScoutApiKey.IsSet())
                    {
                        var botScout = new BotScout();

                        return await botScout.IsBotAsync(ipAddress, emailAddress, userName);
                    }

                    // use StopForumSpam instead
                    var stopForumSpam = new StopForumSpam();

                    return await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                }

            case BotSpamService.BothServiceMatch:
                {
                    var stopForumSpam = new StopForumSpam();
                    var botScout = new BotScout();

                    if (!this.Get<BoardSettings>().BotScoutApiKey.IsSet())
                    {
                        return await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                    }

                    var stopForumSpamCheck = await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                    var botScoutCheck = await botScout.IsBotAsync(ipAddress, emailAddress, userName);

                    return ($"{stopForumSpamCheck.ResponseText} {botScoutCheck.ResponseText}",
                               stopForumSpamCheck.IsBot && botScoutCheck.IsBot);
                }

            case BotSpamService.OneServiceMatch:
                {
                    // use StopForumSpam instead
                    var stopForumSpam = new StopForumSpam();
                    var botScout = new BotScout();

                    if (!this.Get<BoardSettings>().BotScoutApiKey.IsSet())
                    {
                        return await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                    }

                    var stopForumSpamCheck = await stopForumSpam.IsBotAsync(ipAddress, emailAddress, userName);
                    var botScoutCheck = await botScout.IsBotAsync(ipAddress, emailAddress, userName);

                    return ($"{stopForumSpamCheck.ResponseText} {botScoutCheck.ResponseText}",
                               stopForumSpamCheck.IsBot || botScoutCheck.IsBot);
                }
        }

        return (result, false);
    }

    /// <summary>
    /// Check Content for Spam URLs (Count URLs inside Messages)
    /// </summary>
    /// <param name="message">
    /// The message to check for URLs.
    /// </param>
    /// <param name="result">
    /// The result.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool ContainsSpamUrls(string message, out string result)
    {
        result = string.Empty;

        // Check posts for urls if the user has only x posts
        if (BoardContext.Current.PageUser.NumPosts > this.Get<BoardSettings>().IgnoreSpamWordCheckPostCount
            || BoardContext.Current.IsAdmin || BoardContext.Current.ForumModeratorAccess)
        {
            return false;
        }

        var urlCount = UrlHelper.CountUrls(message);

        if (urlCount <= this.Get<BoardSettings>().AllowedNumberOfUrls)
        {
            return false;
        }

        result = $"The user posted {urlCount} urls but allowed only {this.Get<BoardSettings>().AllowedNumberOfUrls}";

        return true;
    }
}