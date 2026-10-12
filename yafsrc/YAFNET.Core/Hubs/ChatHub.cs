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

namespace YAF.Core.Hubs;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Notification Hub
/// Implements the <see cref="Hub" />
/// </summary>
/// <seealso cref="Hub" />
[Authorize]
public class ChatHub : Hub, IHaveServiceLocator
{
    /// <summary>
    /// The connected users, keyed by SignalR connection id.
    /// </summary>
    private readonly static ConcurrentDictionary<string, ChatUser> ConnectedUsers = new();

    /// <summary>
    /// The user of the conversation that is open in a connection, keyed by SignalR connection id.
    /// </summary>
    private readonly static ConcurrentDictionary<string, int> OpenConversations = new();

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
    /// <param name="avatar">the avatar sent by the client (not used, the avatar of the user is taken)</param>
    /// <param name="toUserId">The Sender User Id</param>
    public async Task ConnectAsync(
        string avatar,
        int toUserId)
    {
        var id = this.Context.ConnectionId;

        var user = BoardContext.Current.PageUser;
        var userId = BoardContext.Current.PageUserID;

        if (!this.Get<BoardSettings>().AllowPrivateMessages || BoardContext.Current.IsGuest)
        {
            return;
        }

        ConnectedUsers.TryAdd(
            id,
            new ChatUser
                {
                    ConnectionId = id,
                    UserName = user.Name,
                    DisplayName = user.DisplayOrUserName(),
                    UserId = userId,

                    // the recipient shows it in the message html, a value of the client could inject script
                    Avatar = HttpUtility.HtmlEncode(this.Get<IAvatars>().GetAvatarUrlForUser(user))
                });

        OpenConversations[id] = toUserId;

        // Load existing conversations form db
        var conversation = await this.GetRepository<PrivateMessage>().GetConversationAsync(userId, toUserId);

        foreach (var message in conversation)
        {
            message.DateTime = this.Get<BoardSettings>().ShowRelativeTime
                                   ? message.Created.ToRelativeTime()
                                   : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, message.Created);

            message.Body = await this.FormatMessageAsync(message.Body);
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
        ConnectedUsers.TryRemove(this.Context.ConnectionId, out _);
        OpenConversations.TryRemove(this.Context.ConnectionId, out _);

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

        ConnectedUsers.TryGetValue(fromUserId, out var fromConnectUser);
        var currentDateTime = DateTime.UtcNow;

        var dateTimeFormatted = this.Get<BoardSettings>().ShowRelativeTime
                                    ? currentDateTime.ToRelativeTime()
                                    : this.Get<IDateTimeService>().Format(DateTimeFormat.Both, currentDateTime);

        // a suspended user can't send messages (the forum pages send the user to the info page)
        if (fromConnectUser == null || BoardContext.Current.IsSuspendedNow
            || !await this.CanSendToAsync(fromConnectUser.UserId, toUserId))
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

        var bodyFormatted = await this.FormatMessageAsync(body);

        // the connections (browser tabs) of the other user that show the conversation with the sender,
        // otherwise the message stays unread
        var toConnections = ConnectedUsers.Values.Where(
            x => x.UserId == toUserId && OpenConversations.TryGetValue(x.ConnectionId, out var openUserId)
                                      && openUserId == fromConnectUser.UserId).ToList();

        foreach (var toConnectUser in toConnections)
        {
            flags.IsRead = true;

            // send to YAF user if online
            await this.Clients.Client(toConnectUser.ConnectionId).SendAsync(
                "sendPrivateMessage",
                fromConnectUser.UserId,
                bodyFormatted,
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
            bodyFormatted,
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

    /// <summary>
    /// Checks if the user can send a private message to the other user of the board
    /// (the other user does not block private messages or ignores the sender,
    /// admins and users in the friend list of the other user are excluded from the blocking).
    /// </summary>
    /// <param name="fromUserId">The from user identifier.</param>
    /// <param name="toUserId">The to user identifier.</param>
    /// <returns>Returns if the message can be sent.</returns>
    private async Task<bool> CanSendToAsync(int fromUserId, int toUserId)
    {
        if (!this.Get<BoardSettings>().AllowPrivateMessages || fromUserId == toUserId)
        {
            return false;
        }

        var toUser = await this.GetRepository<User>().GetByIdAsync(toUserId);

        if (toUser is null || toUser.BoardID != BoardContext.Current.PageBoardID || toUser.UserFlags.IsGuest
            || !toUser.UserFlags.IsApproved || toUser.UserFlags.IsDeleted)
        {
            return false;
        }

        if (BoardContext.Current.IsAdmin)
        {
            return true;
        }

        if (toUser.Block.BlockPMs
            && !await this.GetRepository<Buddy>().ExistsAsync(
                b => b.FromUserID == toUserId && b.ToUserID == fromUserId && b.Approved))
        {
            return false;
        }

        return !await this.GetRepository<IgnoreUser>()
                   .ExistsAsync(i => i.UserID == toUserId && i.IgnoredUserID == fromUserId);
    }

    /// <summary>
    /// Formats the message with BBCode.
    /// </summary>
    /// <param name="inputMessage">The input message.</param>
    /// <returns>System.Threading.Tasks.Task&lt;System.String&gt;.</returns>
    private Task<string> FormatMessageAsync(string inputMessage)
    {
        return this.Get<IFormatMessage>().FormatMessageWithAllBBCodesAsync(
            inputMessage, 0, 0);
    }
}