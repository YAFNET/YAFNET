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

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

namespace YAF.UI.Chat;

/// <summary>
/// Class AllChatHub.
/// Implements the <see cref="Hub" />
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="Hub" />
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
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
        var currentDateTime = DateTime.UtcNow;

        var dateTimeFormatted = this.Get<BoardSettings>().ShowRelativeTime
            ? currentDateTime.ToRelativeTime()
            : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, currentDateTime);

        var body = HtmlTagHelper.StripHtml(message);

        await this.GetRepository<ChatMessage>().InsertAsync(
            new ChatMessage
            {
                Message = body,
                Created = currentDateTime,
                UserId = BoardContext.Current.PageUserID,
                BoardId = BoardContext.Current.BoardSettings.BoardId
            });

        const string side = "end";
        const string msgClass = "text-bg-primary";
        const string timeSide = "start";

        await this.Clients.All.SendAsync("ReceiveMessage", BoardContext.Current.PageUser.DisplayOrUserName(), body,
            dateTimeFormatted, side, timeSide, msgClass);
    }
}