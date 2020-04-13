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
    using System.Web.Http;

    using YAF.Core.Context;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The favorite topic controller.
    /// </summary>
    [RoutePrefix("api")]
    public class FavoriteTopicController : ApiController, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        #endregion

        /// <summary>
        /// The add favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// The add favorite topic JS.
        /// </returns>
        [Route("FavoriteTopic/AddFavoriteTopic/{topicId}")]
        [HttpPost]
        public IHttpActionResult AddFavoriteTopic(int topicId)
        {
            return this.Ok(this.Get<IFavoriteTopic>().AddFavoriteTopic(topicId));
        }

        /// <summary>
        /// The remove favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The favorite topic id.
        /// </param>
        /// <returns>
        /// The remove favorite topic JS.
        /// </returns>
        [Route("FavoriteTopic/RemoveFavoriteTopic/{topicId}")]
        [HttpPost]
        public IHttpActionResult RemoveFavoriteTopic(int topicId)
        {
            return this.Ok(this.Get<IFavoriteTopic>().RemoveFavoriteTopic(topicId));
        }
    }
}
