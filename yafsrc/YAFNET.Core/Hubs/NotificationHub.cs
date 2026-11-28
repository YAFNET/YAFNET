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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using System.Security.Claims;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Notification Hub: the activity notifications of a user and the real-time updates of the pages
/// (guests connect too, for the real-time updates).
/// Implements the <see cref="Hub" />
/// </summary>
/// <seealso cref="Hub" />
public class NotificationHub : Hub<INotificationClient>, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationHub"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public NotificationHub(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The name of the group with the connections that can read a forum.
    /// </summary>
    /// <param name="forumId">The forum id.</param>
    /// <returns>The group name.</returns>
    public static string ForumGroup(int forumId) => $"forum-{forumId}";

    /// <summary>
    /// Sends the new activity notification.
    /// </summary>
    /// <param name="alerts">The alerts.</param>
    /// <returns>Task.</returns>
    public Task SendNewActivityAsync(int alerts)
    {
        return this.Clients
            .Client(this.Context.ConnectionId)
            .NewActivityAsync(alerts);
    }

    /// <summary>
    /// Subscribes the connection to the real-time updates (new topics and posts) of all forums the user can read.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task SubscribeAsync()
    {
        if (!this.Get<BoardSettings>().EnableLiveUpdates)
        {
            return;
        }

        // not BoardContext.Current.PageUserID: that loads the page of the request and would set the
        // location of the user (who is online) to the hub
        var userId = await this.GetUserIdAsync();

        if (!userId.HasValue)
        {
            return;
        }

        var forums = (await this.GetRepository<VAccess>().GetAsync(v => v.UserID == userId.Value && v.ReadAccess > 0))
            .Select(v => v.ForumID).Distinct();

        foreach (var forumId in forums)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, ForumGroup(forumId));
        }
    }

    /// <summary>
    /// Gets the forum user of the connection, the guest user if not signed in.
    /// </summary>
    /// <returns>The user id, null if the signed-in user is not a user of this board.</returns>
    private async Task<int?> GetUserIdAsync()
    {
        // only repositories and the user manager here: a hub method has no web request scope, the
        // services of the request (e.g. IAspNetUsersHelper) can't be resolved
        var boardId = BoardContext.Current.PageBoardID;
        var principal = this.Context.User;

        if (principal?.Identity is not { IsAuthenticated: true })
        {
            var guest = await this.GetRepository<User>().GetSingleAsync(u => u.BoardID == boardId && (u.Flags & 4) == 4);

            return guest?.ID;
        }

        var providerUserKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (providerUserKey.IsNotSet() && principal.Identity.Name.IsSet())
        {
            providerUserKey = (await this.Get<UserManager<AspNetUsers>>().FindByNameAsync(principal.Identity.Name))?.Id;
        }

        if (providerUserKey.IsNotSet())
        {
            return null;
        }

        return (await this.GetRepository<User>().GetUserByProviderKeyAsync(boardId, providerUserKey))?.ID;
    }
}
