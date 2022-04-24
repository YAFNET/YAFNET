/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
using System.Linq;
using System.Web.Http;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Tags controller.
/// </summary>
[RoutePrefix("api")]
public class TagsController : ApiController, IHaveServiceLocator
{
    #region Properties

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    #endregion

    /// <summary>
    /// Get all tags by Board Id
    /// </summary>
    /// <param name="searchTopic">
    /// The search Topic.
    /// </param>
    /// <returns>
    /// Returns list of all tags.
    /// </returns>
    [Route("Tags/GetBoardTags")]
    [HttpPost]
    public IHttpActionResult GetBoardTags(SearchTopic searchTopic)
    {
        var tags = this.Get<IDataCache>().GetOrSet(
            $"Tags_{BoardContext.Current.PageBoardID}",
            () => this.GetRepository<Tag>().GetByBoardId(),
            TimeSpan.FromMinutes(5));

        if (searchTopic.SearchTerm.IsSet())
        {
            var tagsList = tags
                .Where(tag => tag.TagName.ToLower().Contains(searchTopic.SearchTerm.ToLower()))
                .Select(
                    tag => new SelectOptions
                               {
                                   text = tag.TagName,
                                   id = tag.ID.ToString()
                               }).ToList();

            var pagedTags = new SelectPagedOptions { Total = 0, Results = tagsList };

            return this.Ok(pagedTags);
        }
        else
        {
            var pager = new Paging { CurrentPageIndex = searchTopic.Page, PageSize = 20 };
                
            var tagsPaged = tags.GetPaged(pager);
            var tagsList = (from Tag tag in tagsPaged
                            select new SelectOptions
                                       {
                                           text = tag.TagName,
                                           id = tag.ID.ToString()
                                       }).ToList();

            var pagedTags = new SelectPagedOptions
                                {
                                    Total = tagsList.Any() ? tags.Count : 0,
                                    Results = tagsList
                                };

            return this.Ok(pagedTags);
        }
    }
}