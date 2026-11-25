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

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

namespace YAF.UI.Chat;

/// <summary>
/// Class AllChatHub.
/// Implements the <see cref="Hub" />
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="Hub" />
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
[Authorize]
public class AllChatHub : Hub, IHaveServiceLocator
{
    /// <summary>
    /// Gets the service locator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public async Task SendMessage(string message)
    {
        // the page sends guests to the login page, but the hub can be called directly
        if (!ChatRules.CanSendMessages(BoardContext.Current.IsGuest, BoardContext.Current.IsSuspendedNow))
        {
            return;
        }

        var currentDateTime = DateTime.UtcNow;

        var dateTimeFormatted = this.Get<BoardSettings>().ShowRelativeTime
            ? currentDateTime.ToRelativeTime()
            : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, currentDateTime);

        // an empty message (or only spaces or html) is not saved
        var body = ChatRules.NormalizeMessage(HtmlTagHelper.StripHtml(message ?? string.Empty));

        if (body is null)
        {
            return;
        }

        await this.GetRepository<ChatMessage>().InsertAsync(
            new ChatMessage
            {
                Message = body,
                Created = currentDateTime,
                UserId = BoardContext.Current.PageUserID,
                BoardId = BoardContext.Current.BoardSettings.BoardId
            });

        // the sender, every page decides itself if the message is its own (the message was shown as their own to all users)
        var user = BoardContext.Current.PageUser;

        await this.Clients.All.SendAsync(
            "ReceiveMessage",
            user.ID,
            user.DisplayOrUserName(),
            this.Get<IAvatars>().GetAvatarUrlForUser(user),
            GetUserColor(user),
            body,
            dateTimeFormatted);
    }

    /// <summary>
    /// Gets the background color of the messages of a user (the messages of the other users).
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The css color.</returns>
    public static string GetUserColor(User user)
    {
        // the guest user has no ProviderUserKey (messages of guests could be sent before the hub checked it)
        if (user.ProviderUserKey.IsNotSet())
        {
            return "var(--bs-secondary)";
        }

        return ValidationHelper.IsNumeric(user.ProviderUserKey)
            ? $"#{user.ProviderUserKey.ToGuid().ToString()[..6]}"
            : $"#{user.ProviderUserKey[..6]}";
    }
}