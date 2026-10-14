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

namespace YAF.Core.Controllers;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Album controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization(ForumPages.Admin_EditGroup)]
public class AccessMaskController : ForumBaseController
{
    /// <summary>
    /// Sets the group access mask for the forum
    /// </summary>
    /// <param name="pagedResults">The paged results.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("SetGroupMask")]
    public async Task<IActionResult> SetGroupMask([FromBody] PagedResults pagedResults)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Ok();
        }

        var forumId = pagedResults.UserId;
        var accessMaskId = pagedResults.PageSize;
        var roleId = pagedResults.PageNumber;

        // groups and access masks are per board, the ids are sent by the client
        var boardId = this.PageBoardContext.PageBoardID;

        if (!await this.GetRepository<Group>().ExistsAsync(g => g.ID == roleId && g.BoardID == boardId)
            || !await this.GetRepository<AccessMask>().ExistsAsync(a => a.ID == accessMaskId && a.BoardID == boardId))
        {
            return this.BadRequest();
        }

        // the forum too (a forum belongs to the board of its category)
        var forum = await this.GetRepository<Forum>().GetByIdAsync(forumId);

        if (forum is null || !await this.GetRepository<Category>().ExistsAsync(c => c.ID == forum.CategoryID && c.BoardID == boardId))
        {
            return this.BadRequest();
        }

        await this.GetRepository<ForumAccess>().SaveAsync(
            forumId,
            roleId,
            accessMaskId);

        return this.Ok();
    }
}