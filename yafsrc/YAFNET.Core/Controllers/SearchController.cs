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

using YAF.Types.Attributes;

namespace YAF.Core.Controllers;

using System.Threading.Tasks;

using Microsoft.AspNetCore.OutputCaching;

using YAF.Core.BasePages;
using YAF.Types.Objects;

/// <summary>
/// The YAF Search controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class SearchController : ForumBaseController
{
    /// <summary>
    /// Get similar topic titles
    /// </summary>
    /// <param name="searchTopic">
    /// The search Topic.
    /// </param>
    /// <returns>
    /// Returns the search Results.
    /// </returns>
    [ValidateAntiForgeryToken]
    [OutputCache]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchGridDataSet))]
    [HttpPost("GetSimilarTitles")]
    public Task<ActionResult<SearchGridDataSet>> GetSimilarTitles([FromBody] SearchTopic searchTopic)
    {
        var results = this.Get<ISearch>().SearchSimilar(
            string.Empty,
            searchTopic.SearchTerm,
            "Topic");

        if (results is null)
        {
            return Task.FromResult<ActionResult<SearchGridDataSet>>(
                this.Ok(new SearchGridDataSet {PageNumber = 0, TotalRecords = 0, PageSize = 0}));
        }

        return Task.FromResult<ActionResult<SearchGridDataSet>>(
            this.Ok(
                new SearchGridDataSet
                    {
                        PageNumber = 1, TotalRecords = results.Count, PageSize = 20, SearchResults = results
                    }));
    }

    /// <summary>
    /// Gets the search results.
    /// </summary>
    /// <param name="searchTopic">
    /// The search Topic.
    /// </param>
    /// <returns>
    /// Returns the search Results.
    /// </returns>
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchGridDataSet))]
    [HttpPost("GetSearchResults")]
    public async Task<ActionResult<SearchGridDataSet>> GetSearchResults([FromBody] SearchTopic searchTopic)
    {
        var results = await this.Get<ISearch>().SearchPagedAsync(
            searchTopic.ForumId,
            searchTopic.SearchTerm,
            searchTopic.Page,
            searchTopic.PageSize);

        return await Task.FromResult<ActionResult<SearchGridDataSet>>(
                   this.Ok(
                       new SearchGridDataSet
                       {
                           PageNumber = searchTopic.Page,
                           TotalRecords = results.Item2,
                           PageSize = searchTopic.PageSize,
                           SearchResults = results.Item1
                       }));
    }
}