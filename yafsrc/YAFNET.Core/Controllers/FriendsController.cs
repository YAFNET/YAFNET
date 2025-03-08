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
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.BasePages;
using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Friends controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Authorize]
[Route("[controller]")]
public class FriendsController : ForumBaseController
{
    /// <summary>
    /// Adds the friend.
    /// </summary>
    /// <param name="m">The message id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("AddFriend/{m:int}")]
    public IActionResult AddFriend(int m)
    {
        try
        {
            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.Find(message => message.MessageID == m);

            if (source == null)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            var userName = this.PageBoardContext.BoardSettings.EnableDisplayName ? source.DisplayName : source.UserName;

            this.PageBoardContext.SessionNotify(
                this.Get<IFriends>().AddRequest(source.UserID)
                    ? this.GetTextFormatted(
                        "NOTIFICATION_BUDDYAPPROVED_MUTUAL",
                        userName)
                    : this.GetText("NOTIFICATION_BUDDYREQUEST"),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Post,
                new {m, name = source.Topic});
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new {m, name = "Topic" });
        }
    }

    /// <summary>
    /// Adds the friend.
    /// </summary>
    /// <param name="u">The user id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("AddFriendRequest/{u:int}")]
    public IActionResult AddFriendRequest(int u)
    {
        try
        {
            var user = this.GetRepository<User>().GetById(u);

            if (user == null)
            {
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Info,
                    new { info = InfoMessage.Invalid.ToType<int>() });
            }

            this.PageBoardContext.SessionNotify(
                this.Get<IFriends>().AddRequest(user.ID)
                    ? this.GetTextFormatted(
                        "NOTIFICATION_BUDDYAPPROVED_MUTUAL",
                        user.DisplayOrUserName())
                    : this.GetText("NOTIFICATION_BUDDYREQUEST"),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.UserProfile,
                new { u, name = user.DisplayOrUserName() });
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Info,
                new { info = InfoMessage.Invalid.ToType<int>() });
        }
    }

    /// <summary>
    /// Removes the friend.
    /// </summary>
    /// <param name="m">The message id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("RemoveFriend/{m:int}")]
    public IActionResult RemoveFriend(int m)
    {
        try
        {
            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.Find(message => message.MessageID == m);

            if (source == null)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            this.Get<IFriends>().Remove(source.UserID);

            this.PageBoardContext.SessionNotify(
                this.GetTextFormatted(
                    "REMOVEBUDDY_NOTIFICATION",
                    this.Get<IFriends>().Remove(source.UserID)),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Post,
                new {m, name = source.Topic});
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new {m, name = "Topic" });
        }
    }

    /// <summary>
    /// Removes the friend.
    /// </summary>
    /// <param name="u">The user id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("RemoveBuddy/{u:int}")]
    public IActionResult RemoveBuddy(int u)
    {
        try
        {
            var user = this.GetRepository<User>().GetById(u);

            if (user == null)
            {
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Info,
                    new { info = InfoMessage.Invalid.ToType<int>() });
            }

            this.Get<IFriends>().Remove(user.ID);

            this.PageBoardContext.SessionNotify(
                this.GetTextFormatted(
                    "REMOVEBUDDY_NOTIFICATION",
                    this.Get<IFriends>().Remove(user.ID)),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.UserProfile,
                new { u, name = user.DisplayOrUserName() });
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Info,
                new { info = InfoMessage.Invalid.ToType<int>() });
        }
    }
}