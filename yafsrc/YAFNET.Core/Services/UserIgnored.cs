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

using System.Threading.Tasks;

namespace YAF.Core.Services;

using System.Collections.Generic;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// PageUser Ignored Service for the current user.
/// </summary>
public class UserIgnored : IUserIgnored, IHaveServiceLocator
{
    /// <summary>
    ///   The user ignore list.
    /// </summary>
    private List<int> _userIgnoreList;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIgnored"/> class.
    /// </summary>
    /// <param name="contextAccessor">The context accessor.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public UserIgnored(IHttpContextAccessor contextAccessor, IServiceLocator serviceLocator)
    {
        this.SessionState = contextAccessor.HttpContext.Session;
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets SessionState.
    /// </summary>
    public ISession SessionState { get; set; }

    /// <summary>
    ///     Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The add ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    ///     The ignored user id.
    /// </param>
    public async Task AddIgnoredAsync(int ignoredUserId)
    {
        await this.GetRepository<IgnoreUser>().AddIgnoredUserAsync(BoardContext.Current.PageUserID, ignoredUserId);
        this.ClearIgnoreCache();
    }

    /// <summary>
    /// The clear ignore cache.
    /// </summary>
    public void ClearIgnoreCache()
    {
        // clear for the session
        this.SessionState.Remove(string.Format(Constants.Cache.UserIgnoreList, BoardContext.Current.PageUserID));
    }

    /// <summary>
    /// The is ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    ///     The ignored user id.
    /// </param>
    /// <returns>
    /// The is ignored.
    /// </returns>
    public async Task<bool> IsIgnoredAsync(int ignoredUserId)
    {
        this._userIgnoreList ??= await this.UserIgnoredListAsync(BoardContext.Current.PageUserID);

        return this._userIgnoreList.Count > 0 && this._userIgnoreList.Contains(ignoredUserId);
    }

    /// <summary>
    /// The remove ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    ///     The ignored user id.
    /// </param>
    public async Task RemoveIgnoredAsync(int ignoredUserId)
    {
        await this.GetRepository<IgnoreUser>().DeleteAsync(BoardContext.Current.PageUserID, ignoredUserId);
        this.ClearIgnoreCache();
    }

    /// <summary>
    ///  Gets the list of ignored user for the specified user.
    /// </summary>
    /// <param name="userId"> The user id. </param>
    /// <returns> Returns the user ignored list. </returns>
    public async Task<List<int>> UserIgnoredListAsync(int userId)
    {
        var key = string.Format(Constants.Cache.UserIgnoreList, userId);

        // stored in the user session...

        var userList = this.SessionState.GetData<List<int>>(key);

        // was it in the cache?
        if (userList != null)
        {
            return userList;
        }
        else
        {
            userList = [];
        }

        // get fresh values
        var users = await this.GetRepository<IgnoreUser>().GetAsync(i => i.UserID == userId);

        users
            .ForEach(user => userList.Add(user.IgnoredUserID));

        // store it in the user session...
        this.SessionState.SetData(key, userList);

        return userList;
    }
}