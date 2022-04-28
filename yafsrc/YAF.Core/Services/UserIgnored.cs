/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System.Collections.Generic;

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Models;

#endregion

/// <summary>
/// User Ignored Service for the current user.
/// </summary>
public class UserIgnored : IUserIgnored, IHaveServiceLocator
{
    #region Constants and Fields

    /// <summary>
    ///   The _user ignore list.
    /// </summary>
    private List<int> _userIgnoreList;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIgnored"/> class.
    /// </summary>
    /// <param name="sessionStateBase">
    /// The session state base.
    /// </param>
    /// <param name="dbBroker">
    /// The db broker.
    /// </param>
    public UserIgnored([NotNull] HttpSessionStateBase sessionStateBase, IServiceLocator serviceLocator)
    {
        this.SessionStateBase = sessionStateBase;
        this.ServiceLocator = serviceLocator;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets SessionStateBase.
    /// </summary>
    public HttpSessionStateBase SessionStateBase { get; set; }

    /// <summary>
    ///     Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IUserIgnored

    /// <summary>
    /// The add ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public void AddIgnored(int ignoredUserId)
    {
        this.GetRepository<IgnoreUser>().AddIgnoredUser(BoardContext.Current.PageUserID, ignoredUserId);
        this.ClearIgnoreCache();
    }

    /// <summary>
    /// The clear ignore cache.
    /// </summary>
    public void ClearIgnoreCache()
    {
        // clear for the session
        this.SessionStateBase.Remove(string.Format(Constants.Cache.UserIgnoreList, BoardContext.Current.PageUserID));
    }

    /// <summary>
    /// The is ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    /// <returns>
    /// The is ignored.
    /// </returns>
    public bool IsIgnored(int ignoredUserId)
    {
        this._userIgnoreList ??= this.UserIgnoredList(BoardContext.Current.PageUserID);

        return this._userIgnoreList.Count > 0 && this._userIgnoreList.Contains(ignoredUserId);
    }

    /// <summary>
    /// The remove ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public void RemoveIgnored(int ignoredUserId)
    {
        this.GetRepository<IgnoreUser>().Delete(BoardContext.Current.PageUserID, ignoredUserId);
        this.ClearIgnoreCache();
    }

    #endregion

    /// <summary>
    ///     The user ignored list.
    /// </summary>
    /// <param name="userId"> The user id. </param>
    /// <returns> Returns the user ignored list. </returns>
    private List<int> UserIgnoredList(int userId)
    {
        var key = string.Format(Constants.Cache.UserIgnoreList, userId);

        // stored in the user session...

        // was it in the cache?
        if (this.SessionStateBase[key] is List<int> userList)
        {
            return userList;
        }

        userList = new List<int>();

        // get fresh values
        this.GetRepository<IgnoreUser>().Get(i => i.UserID == userId).ForEach(user => userList.Add(user.IgnoredUserID));

        // store it in the user session...
        this.SessionStateBase.Add(key, userList);

        return userList;
    }

    #endregion
}