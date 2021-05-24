/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
namespace YAF.Types.Interfaces.Data
{
    using System.Data;

    /// <summary>
    /// The Database Migration interface.
    /// </summary>
    public interface IDbMigration
    {
        /// <summary>
        /// The migrate database.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        void MigrateDatabase(IDbAccess dbAccess);

        /// <summary>
        /// The upgrade table access mask.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableAccessMask(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table active.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableActive(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table active access.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableActiveAccess(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table activity.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableActivity(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table admin page user access.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableAdminPageUserAccess(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table attachment.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableAttachment(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table banned email.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBannedEmail(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table banned ip.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBannedIP(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table banned name.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBannedName(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table bb code.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBBCode(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table board.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBoard(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table buddy.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableBuddy(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table category.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableCategory(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table check email.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableCheckEmail(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table choice.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableChoice(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table event log.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableEventLog(IDbAccess dbAccess, IDbCommand dbCommand);

        /// <summary>
        /// The upgrade table favorite topic.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void UpgradeTableFavoriteTopic(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableForum(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableForumAccess(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableForumReadTracking(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableGroup(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableGroupMedal(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableIgnoreUser(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableMedal(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableMessage(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableMessageHistory(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableMessageReported(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableMessageReportedAudit(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablesNntpForum(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablesNntpServer(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablesNntpTopic(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablePMessage(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablePoll(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTablePollVote(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableProfileCustom(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableRank(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableRegistry(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableReplace_Words(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableReputationVote(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableSpam_Words(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableTag(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableThanks(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableTopic(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableTopicReadTracking(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableTopicTag(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUser(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUserAlbum(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUserAlbumImage(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUserForum(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUserGroup(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableUserPMessage(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableWatchForum(IDbAccess dbAccess, IDbCommand dbCommand);

        void UpgradeTableWatchTopic(IDbAccess dbAccess, IDbCommand dbCommand);
    }
}