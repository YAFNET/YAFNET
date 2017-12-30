/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Modules
{
    #region Using

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The search data cleanup module.
    /// </summary>
    public class SearchDataCleanupForumModule : SimpleBaseForumModule
    {
        #region Fields

        /// <summary>
        ///     The _page pre load.
        /// </summary>
        private readonly IFireEvent<ForumPagePreLoadEvent> _pagePreLoad;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchDataCleanupForumModule" /> class.
        /// </summary>
        /// <param name="pagePreLoad">
        ///     The page pre load.
        /// </param>
        public SearchDataCleanupForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> pagePreLoad)
        {
            this._pagePreLoad = pagePreLoad;

            this._pagePreLoad.HandleEvent += this._pagePreLoad_HandleEvent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// _pages the pre load_ handle event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void _pagePreLoad_HandleEvent([NotNull] object sender, [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
        {
            // no security features for login/logout pages
            if (this.ForumPageType == ForumPages.search || this.ForumPageType == ForumPages.posts)
            {
                return;
            }

            // clear out any search data in the session.... just in case...
            this.Get<IYafSession>().SearchData = null;
        }

        #endregion
    }
}