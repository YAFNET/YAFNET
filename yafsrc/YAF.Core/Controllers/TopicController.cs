/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Controllers
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Web.Http;

    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The YAF Topic controller.
    /// </summary>
    [RoutePrefix("api")]
    public class TopicController : ApiController, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        #endregion

        /// <summary>
        /// Gets the topics by forum.
        /// </summary>
        /// <param name="searchTopic">
        /// The search Topic.
        /// </param>
        /// <returns>
        /// The <see cref="SelectPagedOptions"/>.
        /// </returns>
        [Route("Topic/GetTopics")]
        [HttpPost]
        public IHttpActionResult GetTopics(SearchTopic searchTopic)
        {
            if (!BoardContext.Current.IsAdmin && !BoardContext.Current.IsForumModerator)
            {
                return null;
            }

            if (searchTopic.SearchTerm.IsSet())
            {
                var topics = this.Get<IDataCache>().GetOrSet(
                    $"TopicsList_{searchTopic.ForumId}",
                    () => this.GetRepository<Topic>().ListAsDataTable(
                        searchTopic.ForumId,
                        null,
                        DateTimeHelper.SqlDbMinTime(),
                        DateTime.UtcNow,
                        0,
                        30000,
                        false,
                        false,
                        false),
                    TimeSpan.FromMinutes(5));

                var topicsList = (from DataRow topic in topics.Rows
                                  where topic["Subject"].ToString().ToLower().Contains(searchTopic.SearchTerm.ToLower())
                                  select new SelectOptions
                                  {
                                      text = topic["Subject"].ToString(),
                                      id = topic["TopicID"].ToString()
                                  }).ToList();

                var pagedTopics = new SelectPagedOptions { Total = 0, Results = topicsList };

               return this.Ok(pagedTopics);
            }
            else
            {
                var topics = this.GetRepository<Topic>().ListAsDataTable(
                    searchTopic.ForumId,
                    null,
                    DateTimeHelper.SqlDbMinTime(),
                    DateTime.UtcNow,
                    searchTopic.Page,
                    15,
                    false,
                    false,
                    false);

                var topicsList = (from DataRow topic in topics.Rows
                                  select new SelectOptions
                                  {
                                      text = topic["Subject"].ToString(),
                                      id = topic["TopicID"].ToString()
                                  }).ToList();

                var topicsEnum = topics.AsEnumerable();

                var pagedTopics = new SelectPagedOptions
                {
                    Total = topicsEnum.Any() ? topicsEnum.First().Field<int>("TotalRows") : 0,
                    Results = topicsList
                };

              return this.Ok(pagedTopics);
            }
        }
    }
}
