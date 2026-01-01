/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Constants;

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
        /// <summary>
        ///   The active discussions.
        /// </summary>
        public const string ActiveDiscussions = "ActiveDiscussions";

        /// <summary>
        ///   The user data which is not refreshed too often.
        /// </summary>
        public const string ActiveUserLazyData = "ActiveUserLazyData{0}";

        /// <summary>
        ///   The banned IP.
        /// </summary>
        public const string BannedIP = "BannedIP";

        /// <summary>
        ///   The banned UserAgent.
        /// </summary>
        public const string BannedUserAgent = "BannedUserAgent";

        /// <summary>
        ///   The banned Country.
        /// </summary>
        public const string BannedCountry = "BannedCountry";

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
        ///   The board user members.
        /// </summary>
        public const string BoardMembers = "BoardMembers";

        /// <summary>
        ///   The custom bb code.
        /// </summary>
        public const string CustomBBCode = "CustomBBCode";

        /// <summary>
        ///   The edit user data
        /// </summary>
        public const string EditUser = "EditUser{0}";

        /// <summary>
        ///   The user data which is not refreshed too often.
        /// </summary>
        public const string UserCustomProfileData = "UserCustomProfileData{0}";

        /// <summary>
        ///   The forum jump.
        /// </summary>
        public const string ForumJump = "ForumJump{0}";

        /// <summary>
        ///   The forum moderators.
        /// </summary>
        public const string ForumModerators = "ForumModerators";

        /// <summary>
        ///   The guest user.
        /// </summary>
        public const string GuestUser = "GuestUser";

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
        /// The task module.
        /// </summary>
        public const string TaskModule = "TaskModule";

        /// <summary>
        ///   The user ignore list.
        /// </summary>
        public const string UserIgnoreList = "UserIgnoreList{0}";

        /// <summary>
        ///   The user medals.
        /// </summary>
        public const string UserMedals = "UserMedals{0}";

        /// <summary>
        ///   The users online status.
        /// </summary>
        public const string UsersOnlineStatus = "UsersOnlineStatus";

        /// <summary>
        /// The Visitors In The Last 30 Days
        /// </summary>
        public const string VisitorsInTheLast30Days = "VisitorsInTheLast30Days";

        /// <summary>
        /// The YAF Cache key.
        /// </summary>
        public const string YafCacheKey = "YAFCACHE";

        /// <summary>
        /// The version.
        /// </summary>
        public const string Version = "DBVersion";

        /// <summary>
        ///   Admin Page Access List.
        /// </summary>
        public const string AdminPageAccess = "AdminPageAccess{0}";

        /// <summary>
        /// The geo ip data cache key
        /// </summary>
        public const string GeoIpData = "GeoIPData";

        /// <summary>
        /// The geo ip data cache check key
        /// </summary>
        public const string GeoIpDataCheck = "GeoIPDataCheck";

        /// <summary>
        /// The registered users by month data-cache key
        /// </summary>
        public const string RegisteredUsersByMonth = "RegisteredUsersByMonth";

        /// <summary>
        /// The latest version
        /// </summary>
        public const string LatestVersion = "LatestVersion";

        /// <summary>
        /// The admin stats
        /// </summary>
        public const string AdminStats = "AdminStats";

        /// <summary>
        /// The custom bb code regex dictionary
        /// </summary>
        public const string CustomBBCodeRegExDictionary = "CustomBBCodeRegExDictionary";
    }

    /// <summary>
    /// The forum rebuild.
    /// </summary>
    public readonly struct ForumRebuild
    {
        /// <summary>
        /// The blocking task names.
        /// </summary>
        public readonly static string[] BlockingTaskNames =
            [
                "ForumDeleteTask"
            ];
    }
}