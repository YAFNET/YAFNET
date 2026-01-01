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

using System.Threading.Tasks;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Forum controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
public class ForumController : ForumBaseController
{
    /// <summary>
    /// Gets the topics by forum.
    /// </summary>
    /// <param name="searchTopic">
    /// The search Topic.
    /// </param>
    /// <returns>
    /// The <see cref="SelectPagedOptions"/>.
    /// </returns>
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SelectPagedGroupOptions))]
    [HttpPost("GetForums")]
    public Task<ActionResult<SelectPagedGroupOptions>> GetForums([FromBody] SearchTopic searchTopic)
    {
        if (searchTopic.SearchTerm.IsSet())
        {
            var forums = this.GetRepository<Forum>().ListAllSorted(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                searchTopic.SearchTerm.ToLower());

            var pagedForums = new SelectPagedGroupOptions { Total = forums.Count, Results = forums };

            return Task.FromResult<ActionResult<SelectPagedGroupOptions>>(this.Ok(pagedForums));
        }
        else
        {
            var forums = this.GetRepository<Forum>().ListAllSorted(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                searchTopic.Page,
                20,
                out var pager);

            if (searchTopic.AllForumsOption)
            {
                forums.Insert(
                    0,
                    new SelectGroup
                        {
                            id = 0,
                            text = this.GetText("ALL_CATEGORIES"),
                            children = [
                                new SelectOptions
                                {
                                    id = "0",
                                    text = this.GetText("ALL_FORUMS")
                                }
                            ]
                        });
            }

            var pagedForums = new SelectPagedGroupOptions
                                  {
                                      Total = forums.HasItems() ? pager.Count : 0,
                                      Results = forums
                                  };

            return Task.FromResult<ActionResult<SelectPagedGroupOptions>>(this.Ok(pagedForums));
        }
    }
}