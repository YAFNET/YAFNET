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

using System;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;

using Model;

using Types.Models;
using Types.Objects;

using YAF.Core.BasePages;
using YAF.Types.Attributes;

/// <summary>
/// The User Info controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
public class UserInfo : ForumBaseController
{
    /// <summary>
    /// Gets the forum user info as JSON string for the hover cards
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ForumUserInfo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetUserInfo")]
    [OutputCache]
    public Task<ActionResult<ForumUserInfo>> GetUserInfo(int userId)
    {
        try
        {
            // Check if user has access
            if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ProfileViewPermissions))
            {
                return Task.FromResult<ActionResult<ForumUserInfo>>(this.NotFound());
            }

            var user = this.Get<IAspNetUsersHelper>().GetBoardUser(userId);

            if (user == null || user.Item1.ID == 0)
            {
                return Task.FromResult<ActionResult<ForumUserInfo>>(this.NotFound());
            }

            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(user.Item1);

            avatarUrl = avatarUrl.IsNotSet()
                            ? this.Get<IUrlHelper>().Action(
                                "GetTextAvatar",
                                "Avatar",
                                new { userId })
                            : avatarUrl;

            var activeUsers = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.UsersOnlineStatus,
                () => this.GetRepository<Active>().List(
                    this.Get<BoardSettings>().ShowCrawlersInActiveList,
                    this.Get<BoardSettings>().ActiveListTime),
                TimeSpan.FromMilliseconds(this.Get<BoardSettings>().OnlineStatusCacheTimeout));

#pragma warning disable S1125
            var userIsOnline = activeUsers.Exists(x => x.UserID == userId && x.IsActiveExcluded == false);

            var userName = user.Item1.DisplayOrUserName();

            userName = HttpUtility.HtmlEncode(userName);

            var location = user.Item2.Profile_Country.IsSet()
                               ? this.GetText(
                                   "COUNTRY", user.Item2.Profile_Country.Trim())
                               : HttpUtility.HtmlEncode(user.Item2.Profile_Location);

            if (user.Item2.Profile_Region.IsSet() && user.Item2.Profile_Country.IsSet())
            {
                var tag = $"RGN_{user.Item2.Profile_Country.Trim()}_{user.Item2.Profile_Region}";

                location += $", {this.GetText("REGION", tag)}";
            }

            var userInfo = new ForumUserInfo
                               {
                                   Name = userName,
                                   RealName = HttpUtility.HtmlEncode(user.Item2.Profile_RealName),
                                   Avatar = avatarUrl,
                                   Interests = HttpUtility.HtmlEncode(user.Item2.Profile_Interests),
                                   HomePage = user.Item2.Profile_Homepage,
                                   Posts = $"{user.Item1.NumPosts:N0}",
                                   Rank = user.Item3.Name,
                                   Location = location,
                                   Joined =
                                       $"{this.GetText("PROFILE", "JOINED")} {this.Get<IDateTimeService>().FormatDateLong(user.Item1.Joined)}",
                                   Online = userIsOnline
                               };

            if (this.Get<BoardSettings>().EnableUserReputation)
            {
                userInfo.Points = (user.Item1.Points > 0 ? "+" : string.Empty) + user.Item1.Points;
            }

            return Task.FromResult<ActionResult<ForumUserInfo>>(this.Ok(userInfo));
        }
        catch (Exception x)
        {
            this.Get<ILogger<UserInfo>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return Task.FromResult<ActionResult<ForumUserInfo>>(this.NotFound());
        }
    }
}