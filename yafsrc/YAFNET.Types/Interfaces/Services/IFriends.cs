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

namespace YAF.Types.Interfaces.Services;

using YAF.Types.Objects.Model;

/// <summary>
/// The Friends interface.
/// </summary>
public interface IFriends
{
    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    /// is approved without the target user's approval if the target user has sent a buddy request
    /// to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    bool AddRequest(int toUserId);

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    void ApproveAllRequests();

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="toUser">
    /// the to user.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    bool ApproveRequest(BuddyUser toUser);

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="userId">
    /// The User Id.
    /// </param>
    void ClearCache(int userId);

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    void DenyAllRequests();

    /// <summary>
    /// Denies a friend request.
    /// </summary>
    /// <param name="fromUserId">
    /// The from User Id.
    /// </param>
    void DenyRequest(int fromUserId);

    /// <summary>
    /// determines if the "<paramref name="buddyUserId"/>" and current user are buddies.
    /// </summary>
    /// <param name="buddyUserId">
    /// The Buddy User Id.
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    bool IsBuddy(int buddyUserId);

    /// <summary>
    /// Removes the "<paramref name="toUserId"/>" from current user's buddy list.
    /// </summary>
    /// <param name="toUserId">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    string Remove(int toUserId);
}