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

namespace YAF.Core.Data
{
    using System.Linq;

    using YAF.Classes.Data;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// 
    /// </summary>
    public class CompleteSearchContext : ISearchContext
    {
        /// <summary>
        /// Gets to search what.
        /// </summary>
        /// <value>
        /// To search what.
        /// </value>
        public string ToSearchWhat { get; private set; }
        
        /// <summary>
        /// Gets to search from who.
        /// </summary>
        /// <value>
        /// To search from who.
        /// </value>
        public string ToSearchFromWho { get; private set; }

        /// <summary>
        /// Gets the search from who method.
        /// </summary>
        /// <value>
        /// The search from who method.
        /// </value>
        public SearchWhatFlags SearchFromWhoMethod { get; private set; }

        /// <summary>
        /// Gets the search what method.
        /// </summary>
        /// <value>
        /// The search what method.
        /// </value>
        public SearchWhatFlags SearchWhatMethod { get; private set; }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserID { get; private set; }

        /// <summary>
        /// Gets the board identifier.
        /// </summary>
        /// <value>
        /// The board identifier.
        /// </value>
        public int BoardID { get; private set; }

        /// <summary>
        /// Gets the maximum results.
        /// </summary>
        /// <value>
        /// The maximum results.
        /// </value>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use full text].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use full text]; otherwise, <c>false</c>.
        /// </value>
        public bool UseFullText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [search display name].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [search display name]; otherwise, <c>false</c>.
        /// </value>
        public bool SearchDisplayName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [search title only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [search title only]; otherwise, <c>false</c>.
        /// </value>
        public bool SearchTitleOnly { get; private set; }

        /// <summary>
        /// Gets the forum ids.
        /// </summary>
        /// <value>
        /// The forum ids.
        /// </value>
        public int[] ForumIDs { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteSearchContext" /> class.
        /// </summary>
        /// <param name="toSearchWhat">To search what.</param>
        /// <param name="toSearchFromWho">To search from who.</param>
        /// <param name="searchFromWhoMethod">The search from who method.</param>
        /// <param name="searchWhatMethod">The search what method.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="useFullText">if set to <c>true</c> [use full text].</param>
        /// <param name="searchDisplayName">if set to <c>true</c> [search display name].</param>
        /// <param name="searchTitleOnly">if set to <c>true</c> [search title only].</param>
        /// <param name="forumIds">The forum ids.</param>
        public CompleteSearchContext(
            string toSearchWhat,
            string toSearchFromWho,
            SearchWhatFlags searchFromWhoMethod,
            SearchWhatFlags searchWhatMethod,
            int userID,
            int boardID,
            int maxResults,
            bool useFullText,
            bool searchDisplayName,
            bool searchTitleOnly,
            int[] forumIds = null)
        {
            if (toSearchWhat == "*")
            {
                toSearchWhat = string.Empty;
            }

            this.ToSearchWhat = toSearchWhat;
            this.ToSearchFromWho = toSearchFromWho;
            this.SearchFromWhoMethod = searchFromWhoMethod;
            this.SearchWhatMethod = searchWhatMethod;
            this.UserID = userID;
            this.BoardID = boardID;
            this.MaxResults = maxResults;
            this.UseFullText = useFullText;
            this.SearchDisplayName = searchDisplayName;
            this.SearchTitleOnly = searchTitleOnly;
            this.ForumIDs = forumIds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteSearchContext" /> class.
        /// </summary>
        /// <param name="toSearchWhat">To search what.</param>
        /// <param name="toSearchFromWho">To search from who.</param>
        /// <param name="searchFromWhoMethod">The search from who method.</param>
        /// <param name="searchWhatMethod">The search what method.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="useFullText">if set to <c>true</c> [use full text].</param>
        /// <param name="searchDisplayName">if set to <c>true</c> [search display name].</param>
        /// <param name="startFromForumId">The start from forum identifier.</param>
        /// <param name="searchTitleOnly">if set to <c>true</c> [search title only].</param>
        public CompleteSearchContext(
            string toSearchWhat,
            string toSearchFromWho,
            SearchWhatFlags searchFromWhoMethod,
            SearchWhatFlags searchWhatMethod,
            int userID,
            int boardID,
            int maxResults,
            bool useFullText,
            bool searchDisplayName,
            int startFromForumId,
            bool searchTitleOnly)
            : this(
                toSearchWhat,
                toSearchFromWho,
                searchFromWhoMethod,
                searchWhatMethod,
                userID,
                boardID,
                maxResults,
                useFullText,
                searchDisplayName,
                searchTitleOnly)
        {
            this.ForumIDs = GetForumIDsStartingWith(startFromForumId, boardID, userID);
        }

        /// <summary>
        /// Gets the forum ids starting with.
        /// </summary>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static int[] GetForumIDsStartingWith(int forumId, int boardId, int userId)
        {
            if (forumId != 0)
            {
                return LegacyDb.ForumListAll(boardId, userId, forumId)
                    .Select(f => f.ForumID ?? 0)
                    .Distinct()
                    .ToArray();
            }

            return new int[0];
        }
    }
}