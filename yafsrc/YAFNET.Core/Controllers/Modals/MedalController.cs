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

namespace YAF.Core.Controllers.Modals;

using System.Threading.Tasks;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Medal controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class MedalController : ForumBaseController
{
    /// <summary>
    /// Add or Edit the User Medal
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("EditUserMedal")]
    public async Task<IActionResult> EditUserMedalAsync([FromBody] UserMedalEditModal model)
    {
        if (model.UserID == 0)
        {
            // no username, nor userID specified
            return this.Ok(
                new MessageModalNotification(this.GetText("ADMIN_EDITMEDAL", "MSG_VALID_USER"), MessageTypes.warning));
        }

        if (model.UserName.IsSet())
        {
            // Add Medal to user
            this.GetRepository<UserMedal>().Save(
                model.UserID,
                model.MedalId,
                model.UserMessage,
                model.UserHide,
                model.UserSortOrder.ToType<byte>());
        }
        else
        {
            // update user medal
            this.GetRepository<UserMedal>().SaveNew(
                model.UserID,
                model.MedalId,
                model.UserMessage,
                model.UserHide,
                model.UserSortOrder.ToType<byte>());

            if (this.PageBoardContext.BoardSettings.EmailUserOnMedalAward)
            {
                await this.Get<ISendNotification>().ToUserWithNewMedalAsync(model.UserID, model.MedalName);
            }
        }

        // clear cache...
        // remove user from cache...
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.UserMedals, model.UserID));

        return this.Ok();
    }

    /// <summary>
    /// Add or Edit the Group Medal
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("EditGroupMedal")]
    public IActionResult EditGroupMedal([FromBody] GroupMedalEditModal model)
    {
        if (model.GroupName.IsSet())
        {
            this.GetRepository<GroupMedal>().Save(
                model.GroupId,
                model.MedalId,
                model.GroupMessage,
                model.GroupHide,
                model.GroupSortOrder.ToType<byte>());
        }
        else
        {
            this.GetRepository<GroupMedal>().SaveNew(
                model.GroupId,
                model.MedalId,
                model.GroupMessage,
                model.GroupHide,
                model.GroupSortOrder.ToType<byte>());
        }

        return this.Ok();
    }
}