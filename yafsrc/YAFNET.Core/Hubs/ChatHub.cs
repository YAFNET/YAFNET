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

namespace YAF.Core.Hubs;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Notification Hub
/// Implements the <see cref="Hub" />
/// </summary>
/// <seealso cref="Hub" />
public class ChatHub : Hub, IHaveServiceLocator
{
    /// <summary>
    /// The connected users.
    /// </summary>
    private readonly static List<ChatUser> ConnectedUsers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SendNotification"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public ChatHub(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>Connects the specified username.</summary>
    /// <param name="avatar">the avatar</param>
    /// <param name="toUserId">The Sender User Id</param>
    public async Task ConnectAsync(
        string avatar,
        int toUserId)
    {
        var id = this.Context.ConnectionId;

        var user = BoardContext.Current.PageUser;
        var userId = BoardContext.Current.PageUserID;

        if (ConnectedUsers.TrueForAll(x => x.ConnectionId != id))
        {
            ConnectedUsers.Add(
                new ChatUser
                    {
                        ConnectionId = id,
                        UserName = user.Name,
                        DisplayName = user.DisplayOrUserName(),
                        UserId = userId,
                        Avatar = avatar
                    });
        }

        // Load existing conversations form db
        var conversation = await this.GetRepository<PrivateMessage>().GetConversationAsync(userId, toUserId);

        foreach (var message in conversation)
        {
            message.DateTime = this.Get<BoardSettings>().ShowRelativeTime
                                   ? message.Created.ToRelativeTime()
                                   : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, message.Created);
        }

        // send to caller
        await this.Clients.Client(id).SendAsync("onConnected", toUserId, conversation);
    }

    /// <summary>
    /// The on disconnected.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public override Task OnDisconnectedAsync(Exception exception)
    {
        var item = ConnectedUsers.Find(x => x.ConnectionId == this.Context.ConnectionId);

        if (item == null)
        {
            return base.OnDisconnectedAsync(exception);
        }

        ConnectedUsers.Remove(item);

        return base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// The send private message.
    /// </summary>
    /// <param name="toUserId">
    /// The send to user id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public async Task SendPrivateMessageAsync(int toUserId, string message)
    {
        var fromUserId = this.Context.ConnectionId;

        var toConnectUser = ConnectedUsers.Find(x => x.UserId == toUserId);

        var fromConnectUser = ConnectedUsers.Find(x => x.ConnectionId == fromUserId);
        var currentDateTime = DateTime.UtcNow;

        var dateTimeFormatted = this.Get<BoardSettings>().ShowRelativeTime
                                    ? currentDateTime.ToRelativeTime()
                                    : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, currentDateTime);

        if (fromConnectUser == null)
        {
            return;
        }

        // Save message in db
        var body = HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(message));

        if (body.IsNotSet())
        {
            return;
        }

        var flags = new PrivateMessageFlags();

        if (toConnectUser != null)
        {
            flags.IsRead = true;

            // send to YAF user if online
            await this.Clients.Client(toConnectUser.ConnectionId).SendAsync(
                "sendPrivateMessage",
                fromConnectUser.UserId,
                body,
                fromConnectUser.Avatar,
                dateTimeFormatted);
        }

        await this.GetRepository<PrivateMessage>().InsertAsync(
            new PrivateMessage
                {
                    Body = body,
                    Created = currentDateTime,
                    Flags = flags.BitValue,
                    FromUserId = fromConnectUser.UserId,
                    ToUserId = toUserId
                });

        // send to caller user
        await this.Clients.Caller.SendAsync(
            "sendPrivateMessage",
            toUserId,
            body,
            fromConnectUser.Avatar,
            dateTimeFormatted);
    }

    /// <summary>
    /// Deletes the conversation asynchronous.
    /// </summary>
    /// <param name="toUserId">To user identifier.</param>
    /// <returns>Task.</returns>
    public Task DeleteConversationAsync(
        int toUserId)
    {
        // Delete from sender
        return this.GetRepository<PrivateMessage>().DeleteConversationAsync(BoardContext.Current.PageUserID, toUserId);
    }
}