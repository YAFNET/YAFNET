/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

    public class CompleteSearchContext : ISearchContext
    {
        public string ToSearchWhat { get; private set; }
        public string ToSearchFromWho { get; private set; }
        public SearchWhatFlags SearchFromWhoMethod { get; private set; }
        public SearchWhatFlags SearchWhatMethod { get; private set; }
        public int UserID { get; private set; }
        public int BoardID { get; private set; }
        public int MaxResults { get; private set; }
        public bool UseFullText { get; private set; }
        public bool SearchDisplayName { get; private set; }
        public int[] ForumIDs { get; private set; }

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
            this.ForumIDs = forumIds;
        }

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
            int startFromForumId)
            : this(
                toSearchWhat,
                toSearchFromWho,
                searchFromWhoMethod,
                searchWhatMethod,
                userID,
                boardID,
                maxResults,
                useFullText,
                searchDisplayName)
        {
            this.ForumIDs = GetForumIDsStartingWith(startFromForumId, boardID, userID);
        }

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