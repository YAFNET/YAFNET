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
namespace YAF.Types.Interfaces
{
  using System.Data;

  /// <summary>
  /// The Friends interface.
  /// </summary>
  public interface IFriends
  {
    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// The to User ID.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    /// is approved without the target user's approval if the target user has sent a buddy request
    /// to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    string[] AddRequest(int toUserID);

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    /// <param name="mutual">
    /// The mutual.
    /// </param>
    void ApproveAllRequests(bool mutual);

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// the to user id.
    /// </param>
    /// <param name="mutual">
    /// should the second user be added to current user's buddy list too?
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    string ApproveRequest(int toUserID, bool mutual);

    /// <summary>
    /// Gets all the buddies of the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTable"/> of all buddies.
    /// </returns>
    DataTable All();

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="userID">
    /// The User ID.
    /// </param>
    void ClearCache(int userID);

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    void DenyAllRequests();

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    string DenyRequest(int toUserID);

    /// <summary>
    /// Gets all the buddies for the specified user.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// a <see cref="DataTable"/> of all buddies.
    /// </returns>
    DataTable GetForUser(int userID);

    /// <summary>
    /// determines if the "<paramref name="buddyUserID"/>" and current user are buddies.
    /// </summary>
    /// <param name="buddyUserID">
    /// The Buddy User ID.
    /// </param>
    /// <param name="approved">
    /// Just look into approved buddies?
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    bool IsBuddy(int buddyUserID, bool approved);

    /// <summary>
    /// Removes the "<paramref name="toUserID"/>" from current user's buddy list.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    string Remove(int toUserID);
  }
}