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

namespace YAF.Types.Constants
{
    /// <summary>
    /// For globally or multiple times used constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Cache key constants
        /// </summary>
        public struct Cache
        {
            #region Constants and Fields

            /// <summary>
            /// The Album count for the User.
            /// </summary>
            public const string AlbumCountUser = "AlbumCountUser{0}";

            /// <summary>
            ///   The active discussions.
            /// </summary>
            public const string ActiveDiscussions = "ActiveDiscussions";

            /// <summary>
            ///   The user data which is not refreshed too often.
            /// </summary>
            public const string ActiveUserLazyData = "ActiveUserLazyData{0}";

            /// <summary>
            ///   The banned ip.
            /// </summary>
            public const string BannedIP = "BannedIP";

            /// <summary>
            ///  The board moderators cache.
            /// </summary>
            public const string BoardModerators = "BoardModerators";

            /// <summary>
            ///   The board admins cache.
            /// </summary>
            public const string BoardAdmins = "BoardAdmins";

            /// <summary>
            ///   The board settings.
            /// </summary>
            public const string BoardSettings = "BoardSettings";

            /// <summary>
            ///   The board stats.
            /// </summary>
            public const string BoardStats = "BoardStats";

            /// <summary>
            ///   The board user stats.
            /// </summary>
            public const string BoardUserStats = "BoardUserStats";

            /// <summary>
            ///   The custom bb code.
            /// </summary>
            public const string CustomBBCode = "CustomBBCode";

            /// <summary>
            ///   The favorite topic list.
            /// </summary>
            public const string FavoriteTopicCount = "FavoriteTopicId{0}";

            /// <summary>
            ///   The favorite topic list.
            /// </summary>
            public const string FavoriteTopicList = "FavoriteTopicList{0}";

            /// <summary>
            ///   The first post cleaned.
            /// </summary>
            public const string FirstPostCleaned = "FirstPostCleaned{0}{1}";

            /// <summary>
            ///   The forum active discussions.
            /// </summary>
            public const string ForumActiveDiscussions = "ForumActiveDiscussions";

            /// <summary>
            ///   The forum category.
            /// </summary>
            public const string ForumCategory = "ForumCategory";

            /// <summary>
            ///   The forum jump.
            /// </summary>
            public const string ForumJump = "ForumJump{0}";

            /// <summary>
            ///   The forum moderators.
            /// </summary>
            public const string ForumModerators = "ForumModerators";

            /// <summary>
            ///   The guest groups cache.
            /// </summary>
            public const string GuestGroupsCache = "GuestGroupsCache";
            
            /// <summary>
            ///   The group rank styles.
            /// </summary>
            public const string GroupRankStyles = "GroupRankStyles";

            /// <summary>
            ///   The guest user id.
            /// </summary>
            public const string GuestUserID = "GuestUserID";

            /// <summary>
            ///   The most active users.
            /// </summary>
            public const string MostActiveUsers = "MostActiveUsers";

            /// <summary>
            ///   The replace rules.
            /// </summary>
            public const string ReplaceRules = "ReplaceRules{0}";

            /// <summary>
            ///   The replace words.
            /// </summary>
            public const string ReplaceWords = "ReplaceWords";

            /// <summary>
            ///   The spam words.
            /// </summary>
            public const string SpamWords = "SpamWords";

            /// <summary>
            ///   The shoutbox.
            /// </summary>
            public const string Shoutbox = "Shoutbox";

            /// <summary>
            /// The task module.
            /// </summary>
            public const string TaskModule = "YafTaskModule";

            /// <summary>
            ///   The user boxes.
            /// </summary>
            public const string UserBoxes = "UserBoxes";

            /// <summary>
            ///   The user buddies.
            /// </summary>
            public const string UserBuddies = "UserBuddies{0}";

            /// <summary>
            ///   The user ignore list.
            /// </summary>
            public const string UserIgnoreList = "UserIgnoreList{0}";

            /// <summary>
            ///   The user list for id.
            /// </summary>
            public const string UserListForID = "UserListForID{0}";

            /// <summary>
            ///   The user medals.
            /// </summary>
            public const string UserMedals = "UserMedals{0}";

            /// <summary>
            ///   The user signature cache.
            /// </summary>
            public const string UserSignatureCache = "UserSignatureCache";

            /// <summary>
            ///   The users display name collection.
            /// </summary>
            public const string UsersDisplayNameCollection = "UsersDisplayNameCollection";

            /// <summary>
            ///   The users online status.
            /// </summary>
            public const string UsersOnlineStatus = "UsersOnlineStatus";

            /// <summary>
            ///  The Todays Birthdays
            /// </summary>
            public const string TodaysBirthdays = "TodaysBirthdays";

            /// <summary>
            /// The Visitors In The Last 30 Days
            /// </summary>
            public const string VisitorsInTheLast30Days = "VisitorsInTheLast30Days";

            #endregion
        }

        /// <summary>
        /// Constants for UserBox templating
        /// </summary>
        public struct UserBox
        {
            #region Constants and Fields

            /// <summary>
            ///   The avatar.
            /// </summary>
            public const string Avatar = @"<yaf:avatar\s*/>";

            /// <summary>
            ///   The display template default.
            /// </summary>
            public const string DisplayTemplateDefault =
              @"<yaf:avatar /><yaf:rankimage /><yaf:rank /><yaf:reputation /><yaf:medals /><yaf:groups /><yaf:joindate /><yaf:posts /><li class=""list-group-item""><yaf:gender /><yaf:countryimage /></li><yaf:location /><yaf:thanksfrom /><yaf:thanksto />";

            /// <summary>
            ///   The gender.
            /// </summary>
            public const string Gender = @"<yaf:gender\s*/>";

            /// <summary>
            ///   The groups.
            /// </summary>
            public const string Groups = @"<yaf:groups\s*/>";

            /// <summary>
            ///   The join date.
            /// </summary>
            public const string JoinDate = @"<yaf:joindate\s*/>";

            /// <summary>
            ///   The location.
            /// </summary>
            public const string Location = @"<yaf:location\s*/>";
            
            /// <summary>
            ///   The rank image.
            /// </summary>
            public const string CountryImage = @"<yaf:countryimage\s*/>";

            /// <summary>
            ///   The medals.
            /// </summary>
            public const string Medals = @"<yaf:medals\s*/>";

            /// <summary>
            ///   The Reputation (points).
            /// </summary>
            public const string Reputation = @"<yaf:reputation\s*/>";

            /// <summary>
            ///   The posts.
            /// </summary>
            public const string Posts = @"<yaf:posts\s*/>";

            /// <summary>
            ///   The rank.
            /// </summary>
            public const string Rank = @"<yaf:rank\s*/>";

            /// <summary>
            ///   The rank image.
            /// </summary>
            public const string RankImage = @"<yaf:rankimage\s*/>";

            /// <summary>
            ///   The thanks from.
            /// </summary>
            public const string ThanksFrom = @"<yaf:thanksfrom\s*/>";

            /// <summary>
            ///   The thanks to.
            /// </summary>
            public const string ThanksTo = @"<yaf:thanksto\s*/>";

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ForumRebuild
        {
            public static readonly string[] BlockingTaskNames = {
                                                                    "BoardDeleteTask", "BoardCreateTask",
                                                                    "ForumDeleteTask", "ForumSaveTask",
                                                                    "CategoryDeleteTask", "CategorySaveTask"
                                                                };
        }
    }
}