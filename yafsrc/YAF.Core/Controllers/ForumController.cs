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

using System.Collections.Generic;
using System.Web.Http;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Forum controller.
/// </summary>
[RoutePrefix("api")]
public class ForumController : ApiController, IHaveServiceLocator
{
    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets the topics by forum.
    /// </summary>
    /// <param name="searchTopic">
    /// The search Topic.
    /// </param>
    /// <returns>
    /// The <see cref="SelectPagedOptions"/>.
    /// </returns>
    [Route("Forum/GetForums")]
    [HttpPost]
    public IHttpActionResult GetForums(SearchTopic searchTopic)
    {
        if (searchTopic.SearchTerm.IsSet())
        {
            var forums = this.GetRepository<Forum>().ListAllSorted(
                BoardContext.Current.PageBoardID,
                BoardContext.Current.PageUserID,
                searchTopic.SearchTerm.ToLower());

            var pagedForums = new SelectPagedGroupOptions { Total = forums.Count, Results = forums };

            return this.Ok(pagedForums);
        }
        else
        {
            var forums = this.GetRepository<Forum>().ListAllSorted(
                BoardContext.Current.PageBoardID,
                BoardContext.Current.PageUserID,
                searchTopic.Page,
                20,
                out var pager);

            if (searchTopic.AllForumsOption)
            {
                forums.Insert(
                    0,
                    new SelectGroup
                        {
                            text = BoardContext.Current.Get<ILocalization>().GetText("ALL_CATEGORIES"),
                            children = new List<SelectOptions>
                                           {
                                               new ()
                                                   {
                                                       id = "0",
                                                       text = BoardContext.Current.Get<ILocalization>()
                                                           .GetText("ALL_FORUMS")
                                                   }
                                           }
                        });
            }

            var pagedForums = new SelectPagedGroupOptions
                                  {
                                      Total = forums.Any() ? pager.Count : 0,
                                      Results = forums
                                  };

            return this.Ok(pagedForums);
        }
    }

    /// <summary>
    /// Get Forum
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="IHttpActionResult"/>.
    /// </returns>
    [Route("Forum/GetForum/{forumId}")]
    [HttpPost]
    public IHttpActionResult GetForum(int forumId)
    {
        var forums = new List<SelectGroup>();

        if (forumId.Equals(0))
        {
            forums.Insert(
                0,
                new SelectGroup
                    {
                        text = BoardContext.Current.Get<ILocalization>().GetText("ALL_CATEGORIES"),
                        children = new List<SelectOptions>
                                       {
                                           new ()
                                               {
                                                   id = "0",
                                                   text = BoardContext.Current.Get<ILocalization>()
                                                       .GetText("ALL_FORUMS")
                                               }
                                       }
                    });
        }
        else
        {
            forums = this.GetRepository<Forum>().ListAllSorted(
                BoardContext.Current.PageBoardID,
                BoardContext.Current.PageUserID,
                forumId);
        }

        var pagedForums = new SelectPagedGroupOptions { Total = forums.Count, Results = forums };

        return this.Ok(pagedForums);
    }
}