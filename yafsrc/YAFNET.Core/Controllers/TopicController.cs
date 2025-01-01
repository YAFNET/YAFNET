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

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The YAF Topic controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class TopicController : ForumBaseController
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
    [HttpPost("GetTopics")]
    public IActionResult GetTopics([FromBody] SearchTopic searchTopic)
    {
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.IsForumModerator)
        {
            return this.NotFound();
        }

        if (searchTopic.SearchTerm.IsSet())
        {
            var topics = this.Get<IDataCache>().GetOrSet(
                $"TopicsList_{searchTopic.ForumId}",
                () => this.GetRepository<Topic>().Get(t => t.ForumID == searchTopic.ForumId),
                TimeSpan.FromMinutes(5));

            topics.RemoveAll(t => t.ID == searchTopic.TopicId);

            var topicsList = topics
                .Where(topic => topic.TopicName.ToLower().Contains(searchTopic.SearchTerm.ToLower()))
                .Select(
                    topic => new SelectOptions
                                 {
                                     text = topic.TopicName, id = topic.ID.ToString()
                                 }).ToList();

            var pagedTopics = new SelectPagedOptions { Total = 0, Results = topicsList };

            return this.Ok(pagedTopics);
        }
        else
        {
            var topics = this.GetRepository<Topic>().ListPaged(
                searchTopic.ForumId,
                this.PageBoardContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                searchTopic.Page,
                15,
                false);

            topics.RemoveAll(t => t.TopicID == searchTopic.TopicId);

            var topicsList = (from PagedTopic topic in topics
                              select new SelectOptions
                                         {
                                             text = topic.Subject,
                                             id = topic.TopicID.ToString()
                                         }).ToList();

            var pagedTopics = new SelectPagedOptions
                                  {
                                      Total = !topics.NullOrEmpty() ? topics.FirstOrDefault()!.TotalRows : 0,
                                      Results = topicsList
                                  };

            return this.Ok(pagedTopics);
        }
    }
}