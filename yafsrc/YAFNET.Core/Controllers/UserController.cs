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

namespace YAF.Core.Controllers;

using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Core.BasePages;
using YAF.Types.Objects.Model;

using Microsoft.AspNetCore.OutputCaching;

/// <summary>
/// The User controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class UserController : ForumBaseController
{
    /// <summary>
    /// Gets all found Users.
    /// </summary>
    /// <param name="searchTopic">
    /// The search topic.
    /// </param>
    /// <returns>
    /// The <see cref="IActionResult"/>.
    /// </returns>
    [ValidateAntiForgeryToken]
    [HttpPost("GetUsers")]
    [OutputCache]
    public IActionResult GetUsers([FromBody] SearchTopic searchTopic)
    {
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.IsForumModerator)
        {
            return this.NotFound();
        }

        var users = this.Get<IAspNetUsersHelper>().GetUsersPaged(
            this.PageBoardContext.PageBoardID,
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
            select new SelectOptions {
                text = this.PageBoardContext.BoardSettings.EnableDisplayName
                    ? user.DisplayName
                    : user.Name,
                id = user.UserID.ToString()
            }).ToList();

        var pagedUsers = new SelectPagedOptions {
            Total = !users.NullOrEmpty() ? users.FirstOrDefault()!.TotalRows : 0,
            Results = usersList
        };

        return this.Ok(pagedUsers);
    }

    /// <summary>
    /// Gets the forum user info as JSON string for the hover cards
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetMentionUsers")]
    [OutputCache]
    public Task<ActionResult> GetMentionUsers(string users)
    {
        try
        {
            // Check if user has access
            if (BoardContext.Current == null)
            {
                return Task.FromResult<ActionResult>(this.NotFound());
            }

            var searchQuery = users;

            var usersList = this.GetRepository<User>().Get(
                user => this.PageBoardContext.BoardSettings.EnableDisplayName
                    ? user.DisplayName.StartsWith(searchQuery)
                    : user.Name.StartsWith(searchQuery));

            var userList = usersList.AsEnumerable().Where(u =>
                !this.Get<IUserIgnored>().IsIgnored(u.ID) && u.BoardID == BoardContext.Current.PageBoardID &&
                !u.UserFlags.IsDeleted && u.UserFlags.IsApproved).Select(
                u => new {
                    id = u.ID,
                    name = u.DisplayOrUserName(),
                    avatar = this.Get<IAvatars>().GetAvatarUrlForUser(u)
                });

            return Task.FromResult<ActionResult>(this.Ok(userList));
        }
        catch (Exception x)
        {
            this.Get<ILogger<UserController>>()
                .Log(BoardContext.Current != null ? this.PageBoardContext.PageUserID : null, this, x,
                    EventLogTypes.Information);

            return Task.FromResult<ActionResult>(this.NotFound());
        }
    }
}