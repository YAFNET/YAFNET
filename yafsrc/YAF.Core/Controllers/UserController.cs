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

namespace YAF.Core.Controllers;

using System.Web.Http;

using YAF.Types.Interfaces.Identity;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The User controller.
/// </summary>
[RoutePrefix("api")]
public class UserController : ApiController, IHaveServiceLocator
{
    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets all found Users.
    /// </summary>
    /// <param name="searchTopic">
    /// The search topic.
    /// </param>
    /// <returns>
    /// The <see cref="IHttpActionResult"/>.
    /// </returns>
    [Route("User/GetUsers")]
    [HttpPost]
    public IHttpActionResult GetUsers(SearchTopic searchTopic)
    {
        if (!BoardContext.Current.IsAdmin && !BoardContext.Current.IsForumModerator)
        {
            return this.NotFound();
        }

        var users = this.Get<IAspNetUsersHelper>().GetUsersPaged(
            BoardContext.Current.PageBoardID,
            searchTopic.Page,
            15,
            searchTopic.SearchTerm,
            null,
            null,
            false,
            null,
            null,
            false);

        var usersList = (from PagedUser user in users
                         select new SelectOptions
                                {
                                    text = BoardContext.Current.BoardSettings.EnableDisplayName
                                               ? user.DisplayName
                                               : user.Name,
                                    id = user.UserID.ToString()
                                }).ToList();

        var pagedUsers = new SelectPagedOptions
                         {
                             Total = users.Any() ? users.FirstOrDefault().TotalRows : 0,
                             Results = usersList
                         };

        return this.Ok(pagedUsers);
    }
}