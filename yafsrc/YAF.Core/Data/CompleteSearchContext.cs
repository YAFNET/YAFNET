/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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