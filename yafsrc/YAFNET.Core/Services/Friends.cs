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

using System;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// YAF Friends service
/// </summary>
public class Friends : IFriends, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Friends"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Friends(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="toUserId">
    /// the to user id.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    ///   is approved without the target user's approval if the target user has sent a buddy request
    ///   to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    public bool AddRequest(int toUserId)
    {
        this.ClearCache(toUserId);

        return this.GetRepository<Buddy>().AddRequest(
            BoardContext.Current.PageUserID,
            toUserId);
    }

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    public void ApproveAllRequests()
    {
        var users = this.GetRepository<Buddy>().GetReceivedRequests(BoardContext.Current.PageUserID);

        users.ForEach(user => this.ApproveRequest(user));
    }

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="toUser">
    /// the to user.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool ApproveRequest(BuddyUser toUser)
    {
        this.ClearCache(toUser.UserID);

        return this.GetRepository<Buddy>().ApproveRequest(
            toUser,
            BoardContext.Current.PageUser);
    }

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    public void ClearCache(int userId)
    {
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(BoardContext.Current.PageUserID));
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
    }

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    public void DenyAllRequests()
    {
        var users = this.GetRepository<Buddy>().GetReceivedRequests(BoardContext.Current.PageUserID)
            .Where(x => Convert.ToDateTime(x.Requested).AddDays(14) < DateTime.UtcNow);

        users.ForEach(user => this.DenyRequest(user.UserID));
    }

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="fromUserId">
    /// The from PageUser Id.
    /// </param>
    public void DenyRequest(int fromUserId)
    {
        this.ClearCache(fromUserId);

        this.GetRepository<Buddy>().DenyRequest(
            fromUserId,
            BoardContext.Current.PageUserID);
    }

    /// <summary>
    /// determines if the "<paramref name="buddyUserId"/>" and current user are buddies.
    /// </summary>
    /// <param name="buddyUserId">
    /// The Buddy PageUser ID.
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    public bool IsBuddy(int buddyUserId)
    {
        if (buddyUserId == BoardContext.Current.PageUserID)
        {
            return true;
        }

        var userBuddyList = this.GetRepository<Buddy>().Get(
            x => x.ToUserID == buddyUserId && x.FromUserID == BoardContext.Current.PageUserID);

        return !userBuddyList.NullOrEmpty();
    }

    /// <summary>
    /// Removes the "<paramref name="toUserId"/>" from current user's buddy list.
    /// </summary>
    /// <param name="toUserId">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public string Remove(int toUserId)
    {
        this.ClearCache(toUserId);

        this.GetRepository<Buddy>().Remove(BoardContext.Current.PageUserID, toUserId);

        return this.Get<IUserDisplayName>().GetNameById(toUserId);
    }
}