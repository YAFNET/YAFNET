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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace YAF.Core.Controllers;

using System;
using System.Collections.Generic;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Reputation controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Authorize]
[Route("[controller]")]
public class ReputationController : ForumBaseController
{
    /// <summary>
    /// Adds the user reputation.
    /// </summary>
    [HttpGet]
    [Route("AddReputation/{m:int}")]
    public async Task<IActionResult> AddReputation(int m)
    {
        try
        {
            if (this.PageBoardContext.IsGuest)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.Find(message => message.MessageID == m);

            if (source == null)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            if (!this.Get<IReputation>().CheckIfAllowReputationVoting(source.ReputationVoteDate))
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = source.Topic });
            }

            await this.GetRepository<User>().AddPointsAsync(source.UserID, this.PageBoardContext.PageUserID, 1);

            this.PageBoardContext.SessionNotify(
                this.GetTextFormatted(
                    "REP_VOTE_UP_MSG",
                    new UnicodeEncoder().XSSEncode(
                        this.PageBoardContext.BoardSettings.EnableDisplayName ? source.DisplayName : source.UserName)),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new {m, name = source.Topic });
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
        }
    }

    /// <summary>
    /// Removes the user reputation.
    /// </summary>
    [HttpGet]
    [Route("RemoveReputation/{m:int}")]
    public async Task<IActionResult> RemoveReputation(int m)
    {
        try
        {
            if (this.PageBoardContext.IsGuest)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.Find(message => message.MessageID == m);

            if (source == null)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            if (!this.Get<IReputation>().CheckIfAllowReputationVoting(source.ReputationVoteDate))
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = source.Topic });
            }

            await this.GetRepository<User>().RemovePointsAsync(source.UserID, BoardContext.Current.PageUserID, 1);

            this.PageBoardContext.SessionNotify(
                this.GetTextFormatted(
                    "REP_VOTE_DOWN_MSG",
                    new UnicodeEncoder().XSSEncode(
                        this.PageBoardContext.BoardSettings.EnableDisplayName ? source.DisplayName : source.UserName)),
                MessageTypes.success);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = source.Topic });
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
        }
    }
}