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

namespace YAF.Core.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.BasePages;
using YAF.Types.Attributes;
using YAF.Types.Objects;

/// <summary>
/// The achievements of the current user: the "achievement unlocked" dialog and the achievements the user shows.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class AchievementController : ForumBaseController
{
    /// <summary>
    /// Gets the unlocked achievements the user has not seen yet.
    /// </summary>
    /// <returns>The achievements (the most valuable first).</returns>
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AchievementInfo>))]
    [HttpGet("Unseen")]
    public ActionResult<List<AchievementInfo>> Unseen()
    {
        if (this.PageBoardContext.IsGuest || !this.PageBoardContext.BoardSettings.EnableAchievements)
        {
            return this.Ok(new List<AchievementInfo>());
        }

        return this.Ok(this.Get<IAchievementService>().GetUnseen(this.PageBoardContext.PageUserID));
    }

    /// <summary>
    /// Marks an unlocked achievement as seen (the dialog has shown it).
    /// </summary>
    /// <param name="achievementId">The achievement id.</param>
    /// <returns>Ok.</returns>
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("Seen/{achievementId:int}")]
    public async Task<IActionResult> Seen([FromRoute] int achievementId)
    {
        if (this.PageBoardContext.IsGuest)
        {
            return this.NotFound();
        }

        await this.Get<IAchievementService>().MarkSeenAsync(this.PageBoardContext.PageUserID, achievementId);

        return this.Ok();
    }

    /// <summary>
    /// Shows or hides an unlocked achievement in the profile and with the posts.
    /// </summary>
    /// <param name="achievementId">The achievement id.</param>
    /// <param name="featured">Show the achievement.</param>
    /// <returns>The state of the achievement, a conflict if the user shows the most achievements already.</returns>
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("Feature/{achievementId:int}/{featured:bool}")]
    public async Task<IActionResult> Feature([FromRoute] int achievementId, [FromRoute] bool featured)
    {
        if (this.PageBoardContext.IsGuest)
        {
            return this.NotFound();
        }

        if (!await this.Get<IAchievementService>()
                 .SetFeaturedAsync(this.PageBoardContext.PageUserID, achievementId, featured))
        {
            return this.Conflict(
                new
                {
                    message = string.Format(
                        this.Get<ILocalization>().GetText("ACHIEVEMENTS", "MAX_FEATURED"),
                        Services.AchievementService.MaxFeatured)
                });
        }

        return this.Ok(new { featured });
    }
}
