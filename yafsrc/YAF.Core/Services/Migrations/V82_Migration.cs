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

namespace YAF.Core.Services.Migrations
{
    using ServiceStack.OrmLite;

    using System.Data;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// Version 82 Migrations
    /// </summary>
    public class V82_Migration : IDbMigration
    {
        public void MigrateDatabase(IDbAccess dbAccess)
        {
            dbAccess.Execute(
                dbCommand =>
                {
                    this.UpgradeTableActiveAccess(dbAccess, dbCommand);

                    ///////////////////////////////////////////////////////////

                    return true;
                });
        }

        /// <summary>
        /// The upgrade table active.
        /// </summary>
        /// <param name="dbAccess">The db access.</param>
        /// <param name="dbCommand">The db command.</param>
        public void UpgradeTableActive(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableActiveAccess(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<ActiveAccess>("DownloadAccess"))
            {
                dbCommand.Connection.DropColumn<ActiveAccess>("DownloadAccess");
            }

            if (dbCommand.Connection.ColumnExists<ActiveAccess>("UploadAccess"))
            {
                dbCommand.Connection.DropColumn<ActiveAccess>("UploadAccess");
            }
        }

        public void UpgradeTableTopicTag(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUser(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUserAlbum(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUserAlbumImage(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUserForum(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUserGroup(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableThanks(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableTopic(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableTopicReadTracking(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableAdminPageUserAccess(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableAttachment(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableBannedEmail(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableBannedIP(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableBannedName(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableBBCode(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableActivity(IDbAccess dbAccess, IDbCommand dbCommand)
        {
        }

        public void UpgradeTableBoard(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableBuddy(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableCategory(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableProfileCustom(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableProfileDefinition(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableRank(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableRegistry(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableReplace_Words(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableReputationVote(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableSpam_Words(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableTag(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableCheckEmail(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableChoice(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableEventLog(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableFavoriteTopic(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableWatchTopic(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablesPolls(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableForum(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableForumAccess(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableForumReadTracking(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableUserPMessage(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableWatchForum(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableGroup(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            BoardContext.Current.GetRepository<Group>().Get(g => (g.Flags & 1) == 1 | (g.Flags & 4) == 4).ForEach(
                group =>
                    {
                        group.GroupFlags.AllowDownload = true;
                        group.GroupFlags.AllowUpload = true;

                        BoardContext.Current.GetRepository<Group>().UpdateOnly(
                            () => new Group {Flags = group.GroupFlags.BitValue},
                            g => g.ID == group.ID);
                    });
        }

        public void UpgradeTableUserMedal(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableGroupMedal(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableIgnoreUser(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableMedal(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableMessage(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableMessageHistory(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableMessageReported(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableMessageReportedAudit(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablesNntpForum(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablesNntpServer(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablesNntpTopic(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablePMessage(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablePoll(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTablePollVote(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }

        public void UpgradeTableAccessMask(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // Not used
        }
    }
}