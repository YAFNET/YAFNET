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
    using System.Linq;
    using System.Text;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// Version 80 Migrations
    /// </summary>
    public class V80_Migration : IRepositoryMigration, IHaveServiceLocator
    {
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        /// <summary>
        /// Migrate Repositories (Database).
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        public void MigrateDatabase(IDbAccess dbAccess)
        {
            dbAccess.Execute(
                dbCommand =>
                    {
                        this.UpgradeTable(this.GetRepository<Board>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<BBCode>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Active>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<ActiveAccess>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<BannedIP>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Category>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<EventLog>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<ForumReadTracking>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<UserPMessage>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Topic>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<TopicReadTracking>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<User>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Forum>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Group>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<UserMedal>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<GroupMedal>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<AccessMask>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<NntpForum>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<NntpServer>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<NntpTopic>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Replace_Words>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Registry>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Medal>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Message>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<MessageHistory>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<MessageReported>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<MessageReportedAudit>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<PMessage>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Rank>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<CheckEmail>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<Attachment>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<PollVote>(), dbAccess, dbCommand);

                        ///////////////////////////////////////////////////////////

                        // Remove old Stuff
                        this.UpgradeTablesPolls(dbAccess, dbCommand);

                        if (dbCommand.Connection.TableExists("EventLogGroupAccess"))
                        {
                            dbCommand.Connection.DropTable("EventLogGroupAccess");
                        }

                        if (dbCommand.Connection.TableExists("TopicStatus"))
                        {
                            dbCommand.Connection.DropTable("TopicStatus");
                        }

                        if (dbCommand.Connection.TableExists("ShoutboxMessage"))
                        {
                            dbCommand.Connection.DropTable("ShoutboxMessage");
                        }

                        if (dbCommand.Connection.TableExists("Mail"))
                        {
                            dbCommand.Connection.DropTable("Mail");
                        }

                        if (dbCommand.Connection.TableExists("Extension"))
                        {
                            dbCommand.Connection.DropTable("Extension");
                        }

                        if (dbCommand.Connection.TableExists("UserProfile"))
                        {
                            dbCommand.Connection.DropTable("UserProfile");
                        }

                        if (dbCommand.Connection.TableExists<FileExtension>())
                        {
                            // Migrate File Extensions
                            var extensions = this.GetRepository<FileExtension>().Get(
                                x => x.BoardId == this.Get<BoardSettings>().BoardId);

                            this.GetRepository<Registry>().Save(
                                "allowedfileextensions",
                                extensions.Select(x => x.Extension).ToDelimitedString(","));

                            dbCommand.Connection.DropTable<FileExtension>();
                        }

                        return true;
                    });

            dbAccess.Execute(
                dbCommand =>
                    {
                        this.DeleteStoredProcedures(dbCommand);

                        this.DeleteTriggers(dbCommand);

                        return true;
                    });

            dbAccess.Execute(
                dbCommand =>
                    {
                        // display names upgrade routine can run really for ages on large forums
                        this.InitDisplayNames(dbCommand);

                        return true;
                    });

            dbAccess.Execute(
                dbCommand =>
                    {
                        this.RemoveLegacyFullTextSearch(dbCommand);

                        return true;
                    });

            // Upgrade Views
            dbAccess.Execute(
                dbCommand =>
                    {
                        this.DropIndexViews(dbCommand);

                        this.DropViews(dbCommand);

                        dbAccess.Information.CreateViews(dbAccess, dbCommand);

                        dbAccess.Information.CreateIndexViews(dbAccess, dbCommand);

                        return true;
                    });

            // Upgrade Functions
            dbAccess.Execute(
                dbCommand =>
                    {
                        this.DropFunctions(dbCommand);

                        return true;
                    });
        }

        /// <summary>
        /// The upgrade table active.
        /// </summary>
        /// <param name="dbAccess">The db access.</param>
        /// <param name="dbCommand">The db command.</param>
        private void UpgradeTable(IRepository<Active> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnMaxLength<Active>(x => x.Location) < 255)
            {
                dbCommand.Connection.AlterColumn<Active>(x => x.Location);
            }

            if (!dbCommand.Connection.ColumnExists<Active>(x => x.ForumPage))
            {
                dbCommand.Connection.AddColumn<Active>(x => x.ForumPage);
            }
            else
            {
                if (dbCommand.Connection.ColumnMaxLength<Active>(x => x.ForumPage) < 1024)
                {
                    dbCommand.Connection.AlterColumn<Active>(x => x.ForumPage);
                }
            }

            if (dbCommand.Connection.ColumnMaxLength<Active>(x => x.IP) < 39)
            {
                dbCommand.Connection.AlterColumn<Active>(x => x.IP);
            }

            if (!dbCommand.Connection.ColumnExists<Active>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<Active>(x => x.Flags);
            }

            if (dbCommand.Connection.ColumnMaxLength<Active>(x => x.SessionID) < 50)
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                dbCommand.Connection.DropPrimaryKey<Active>(string.Empty);

                dbCommand.Connection.AlterColumn<Active>(x => x.SessionID);

                dbCommand.Connection.AddCompositePrimaryKey<Active>(x => x.SessionID, x => x.BoardID);
            }

            repository.DeleteAll();
        }

        private void UpgradeTable(IRepository<ActiveAccess> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<ActiveAccess>(x => x.LastActive))
            {
                dbCommand.Connection.AddColumn<ActiveAccess>(x => x.LastActive);
            }

            if (dbCommand.Connection.ColumnIsNullable<ActiveAccess>(x => x.ForumID))
            {
                dbCommand.Connection.AlterColumn<ActiveAccess>(x => x.ForumID);
            }

            if (!dbCommand.Connection.ColumnExists<ActiveAccess>(x => x.IsGuestX))
            {
                dbCommand.Connection.DeleteAll<ActiveAccess>();

                dbCommand.Connection.AddColumn<ActiveAccess>(x => x.IsGuestX);
            }

            // -- drop the old contrained just in case
            dbCommand.Connection.DropIndex<ActiveAccess>();
        }

        private void UpgradeTable(IRepository<TopicTag> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            dbCommand.Connection.DropIndex<TopicTag>($"UC_{Config.DatabaseObjectQualifier}TopicTag_TopicID_TagID");
        }

        private void UpgradeTable(IRepository<User> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<User>(x => x.NotificationType))
            {
                dbCommand.Connection.AddColumn<User>(x => x.NotificationType);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.ProviderUserKey))
            {
                dbCommand.Connection.AddColumn<User>(x => x.ProviderUserKey);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.AvatarImageType))
            {
                dbCommand.Connection.AddColumn<User>(x => x.AvatarImageType);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.Culture))
            {
                dbCommand.Connection.AddColumn<User>(x => x.Culture);
            }
            else
            {
                if (dbCommand.Connection.ColumnMaxLength<User>(x => x.Culture) < 10)
                {
                    dbCommand.Connection.AlterColumn<User>(x => x.Culture);
                }
            }

            if (dbCommand.Connection.ColumnMaxLength<User>(x => x.IP) < 39)
            {
                dbCommand.Connection.AlterColumn<User>(x => x.IP);
            }

            if (dbCommand.Connection.ColumnMaxLength<User>(x => x.Name) < 255)
            {
                dbCommand.Connection.AlterColumn<User>(x => x.Name);
            }

            if (dbCommand.Connection.ColumnMaxLength<User>(x => x.Email) < 255)
            {
                dbCommand.Connection.AlterColumn<User>(x => x.Email);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.UserStyle))
            {
                dbCommand.Connection.AddColumn<User>(x => x.UserStyle);
            }

            if (dbCommand.Connection.ColumnDataType<User>(x => x.Signature) == "ntext")
            {
                dbCommand.Connection.AlterColumn<User>(x => x.Signature);
            }

            var timeZoneType = dbCommand.Connection.ColumnDataType<User>(x => x.TimeZone);

            if (timeZoneType == "ntext")
            {
                dbCommand.Connection.AlterColumn<User>(x => x.TimeZone);
            }
            else if (timeZoneType == "int")
            {
                dbCommand.Connection.AlterColumn<User>(x => x.TimeZone);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.SuspendedReason))
            {
                dbCommand.Connection.AddColumn<User>(x => x.SuspendedReason);
                dbCommand.Connection.AddColumn<User>(x => x.SuspendedBy);
            }
            else
            {
                if (dbCommand.Connection.ColumnDataType<User>(x => x.SuspendedReason) == "ntext")
                {
                    dbCommand.Connection.AlterColumn<User>(x => x.SuspendedReason);
                }
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.BlockFlags))
            {
                dbCommand.Connection.AddColumn<User>(x => x.BlockFlags);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.Activity))
            {
                dbCommand.Connection.AddColumn<User>(x => x.Activity);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.PageSize))
            {
                dbCommand.Connection.AddColumn<User>(x => x.PageSize);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.PageSize))
            {
                dbCommand.Connection.AddColumn<User>(x => x.PageSize);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.DisplayName))
            {
                dbCommand.Connection.AddColumnWithCommand<User>("DisplayName nvarchar(255)");

                var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                dbCommand.Connection.ExecuteSql($@" update {expression.Table<User>()} set DisplayName = Name");

                dbCommand.Connection.AlterColumn<User>(x => x.DisplayName);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.AutoWatchTopics))
            {
                dbCommand.Connection.AddColumn<User>(x => x.AutoWatchTopics);
            }

            if (!dbCommand.Connection.ColumnExists<User>(x => x.DailyDigest))
            {
                dbCommand.Connection.AddColumn<User>(x => x.DailyDigest);
            }
        }

        private void UpgradeTable(IRepository<Topic> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.AnswerMessageId))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.AnswerMessageId);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.TopicImage))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.TopicImage);
            }

            if (dbCommand.Connection.ColumnMaxLength<Topic>(x => x.UserName) < 255)
            {
                dbCommand.Connection.AlterColumn<Topic>(x => x.UserName);
            }

            if (dbCommand.Connection.ColumnMaxLength<Topic>(x => x.LastUserName) < 255)
            {
                dbCommand.Connection.AlterColumn<Topic>(x => x.LastUserName);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.UserDisplayName))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.UserDisplayName);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.LastUserDisplayName))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.LastUserDisplayName);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.Description))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.Description);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.LinkDate))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.LinkDate);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.Status))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.Status);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.Styles))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.Styles);
            }

            if (!dbCommand.Connection.ColumnExists<Topic>(x => x.LastMessageFlags))
            {
                dbCommand.Connection.AddColumn<Topic>(x => x.LastMessageFlags);
            }
        }

        private void UpgradeTable(IRepository<TopicReadTracking> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            dbCommand.Connection.DropPrimaryKey<TopicReadTracking>($"{Config.DatabaseObjectQualifier}TopicTracking");

            if (dbCommand.Connection.ColumnExists<TopicReadTracking>("TrackingID"))
            {
                dbCommand.Connection.DropColumn<TopicReadTracking>("TrackingID");
            }
        }

        private void UpgradeTable(IRepository<Attachment> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            dbCommand.Connection.DropForeignKey<Attachment>($"{Config.DatabaseObjectQualifier}Message");

            if (dbCommand.Connection.ColumnMaxLength<Attachment>(x => x.ContentType) < 255)
            {
                dbCommand.Connection.AlterColumn<Attachment>(x => x.ContentType);
            }

            if (dbCommand.Connection.ColumnExists<Attachment>("FileID"))
            {
                dbCommand.Connection.DropColumn<Attachment>("FileID");
            }

            if (!dbCommand.Connection.ColumnExists<Attachment>(x => x.UserID))
            {
                dbCommand.Connection.AddColumn<Attachment>(x => x.UserID);
            }
        }

        private void UpgradeTable(IRepository<BannedIP> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<BannedIP>(x => x.Reason))
            {
                dbCommand.Connection.AddColumn<BannedIP>(x => x.Reason);
            }

            if (!dbCommand.Connection.ColumnExists<BannedIP>(x => x.UserID))
            {
                dbCommand.Connection.AddColumn<BannedIP>(x => x.UserID);
            }

            if (dbCommand.Connection.ColumnMaxLength<BannedIP>(x => x.Mask) < 56)
            {
                dbCommand.Connection.AlterColumn<BannedIP>(x => x.Mask);
            }
        }

        private void UpgradeTable(IRepository<BBCode> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<BBCode>(x => x.UseModule))
            {
                dbCommand.Connection.AddColumn<BBCode>(x => x.UseModule);
                dbCommand.Connection.AddColumn<BBCode>(x => x.ModuleClass);
            }

            if (!dbCommand.Connection.ColumnExists<BBCode>(x => x.UseToolbar))
            {
                dbCommand.Connection.AddColumn<BBCode>(x => x.UseToolbar);
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.DisplayJS) == "ntext")
            {
                dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayJS);
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.EditJS) == "ntext")
            {
                dbCommand.Connection.AlterColumn<BBCode>(x => x.EditJS);
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.DisplayCSS) == "ntext")
            {
                dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayCSS);
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.SearchRegex) == "ntext")
            {
                dbCommand.Connection.AlterColumn<BBCode>(x => x.SearchRegex);
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.ReplaceRegex) == "ntext")
            {
                dbCommand.Connection.AlterColumn<BBCode>(x => x.ReplaceRegex);
            }
        }

        private void UpgradeTable(IRepository<Board> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<Board>("BoardUID"))
            {
                dbCommand.Connection.DropColumn<Board>("BoardUID");
            }

            if (dbCommand.Connection.ColumnExists<Board>("MembershipAppName"))
            {
                dbCommand.Connection.DropColumn<Board>("MembershipAppName");
            }

            if (dbCommand.Connection.ColumnExists<Board>("RolesAppName"))
            {
                dbCommand.Connection.DropColumn<Board>("RolesAppName");
            }

            if (dbCommand.Connection.ColumnExists<Board>("AllowThreaded"))
            {
                dbCommand.Connection.DropColumn<Board>("AllowThreaded");
            }
        }

        private void UpgradeTable(IRepository<Category> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Category>(x => x.CategoryImage))
            {
                dbCommand.Connection.AddColumn<Category>(x => x.CategoryImage);
            }
        }

        private void UpgradeTable(IRepository<Rank> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.PMLimit))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.PMLimit);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.Flags);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.Style))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.Style);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.SortOrder))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.SortOrder);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.Description))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.Description);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.UsrSigChars))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.UsrSigChars);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.UsrSigBBCodes))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.UsrSigBBCodes);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.UsrAlbums))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.UsrAlbums);
            }

            if (!dbCommand.Connection.ColumnExists<Rank>(x => x.UsrAlbumImages))
            {
                dbCommand.Connection.AddColumn<Rank>(x => x.UsrAlbumImages);
            }

            if (dbCommand.Connection.ColumnExists<Rank>("RankImage"))
            {
                dbCommand.Connection.DropColumn<Rank>("RankImage");
            }

            if (dbCommand.Connection.ColumnExists<Rank>("IsStart"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Rank>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Rank>()} set Flags = Flags | 1 where IsStart<>0");

                dbCommand.Connection.DropColumn<Rank>("IsStart");
            }

            if (dbCommand.Connection.ColumnExists<Rank>("IsLadder"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Rank>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Rank>()} set Flags = Flags | 2 where IsLadder<>0");

                dbCommand.Connection.DropColumn<Rank>("IsLadder");
            }

            if (dbCommand.Connection.ColumnExists<Rank>("UsrSigHTMLTags"))
            {
                dbCommand.Connection.DropColumn<Rank>("UsrSigHTMLTags");
            }
        }

        private void UpgradeTable(IRepository<Registry> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Registry>(x => x.BoardID))
            {
                dbCommand.Connection.AddColumn<Registry>(x => x.BoardID);
            }

            if (dbCommand.Connection.ColumnDataType<Registry>(x => x.Value) == "ntext")
            {
                dbCommand.Connection.AlterColumn<Registry>(x => x.Value);
            }
        }

        private void UpgradeTable(IRepository<Replace_Words> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnMaxLength<Replace_Words>(x => x.BadWord) < 255)
            {
                dbCommand.Connection.AlterColumn<Replace_Words>(x => x.BadWord);
            }

            if (dbCommand.Connection.ColumnMaxLength<Replace_Words>(x => x.GoodWord) < 255)
            {
                dbCommand.Connection.AlterColumn<Replace_Words>(x => x.GoodWord);
            }

            if (!dbCommand.Connection.ColumnExists<Replace_Words>(x => x.BoardID))
            {
                dbCommand.Connection.AddColumn<Replace_Words>(x => x.BoardID);
            }
        }

        private void UpgradeTable(IRepository<CheckEmail> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            dbCommand.Connection.DropIndex<CheckEmail>();

            if (dbCommand.Connection.ColumnMaxLength<CheckEmail>(x => x.Email) < 255)
            {
                dbCommand.Connection.AlterColumn<CheckEmail>(x => x.Email);
            }

            if (dbCommand.Connection.ColumnMaxLength<CheckEmail>(x => x.Email) == 64)
            {
                dbCommand.Connection.AlterColumn<CheckEmail>(x => x.Hash);
            }

            if (dbCommand.Connection.ColumnMaxLength<CheckEmail>(x => x.Hash) < 255)
            {
                dbCommand.Connection.AlterColumn<CheckEmail>(x => x.Hash);
            }
        }

        private void UpgradeTable(IRepository<EventLog> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<EventLog>(x => x.Description) == "ntext")
            {
                dbCommand.Connection.AlterColumn<EventLog>(x => x.Description);
            }

            if (!dbCommand.Connection.ColumnIsNullable<EventLog>(x => x.UserID))
            {
                dbCommand.Connection.AlterColumn<EventLog>(x => x.UserID);
            }

            if (!dbCommand.Connection.ColumnExists<EventLog>(x => x.Type))
            {
                dbCommand.Connection.AddColumn<EventLog>(x => x.Type);
            }
        }

        public void UpgradeTablesPolls(IDbAccess dbAccess, IDbCommand dbCommand)
        {
            // should drop it else error
            dbCommand.Connection.DropForeignKey<Topic>($"{Config.DatabaseObjectQualifier}Poll");

            if (!dbCommand.Connection.ColumnExists<Choice>(x => x.ObjectPath))
            {
                dbCommand.Connection.AddColumn<Choice>(x => x.ObjectPath);
            }

            if (!dbCommand.Connection.ColumnExists<Poll>(x => x.Closes))
            {
                dbCommand.Connection.AddColumn<Poll>(x => x.Closes);
            }

            if (dbCommand.Connection.ColumnMaxLength<Poll>(x => x.Question) < 256)
            {
                dbCommand.Connection.AlterColumn<Poll>(x => x.Question);
            }

            if (!dbCommand.Connection.ColumnExists<Poll>(x => x.UserID))
            {
                dbCommand.Connection.AddColumn<Poll>(x => x.UserID);
            }

            if (!dbCommand.Connection.ColumnExists<Poll>(x => x.ObjectPath))
            {
                dbCommand.Connection.AddColumn<Poll>(x => x.ObjectPath);
            }

            if (!dbCommand.Connection.ColumnExists<Poll>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<Poll>(x => x.Flags);
            }

            if (dbCommand.Connection.ColumnExists<Poll>("MimeType"))
            {
                dbCommand.Connection.DropColumn<Poll>("MimeType");
            }

            if (dbCommand.Connection.ColumnExists<Choice>("MimeType"))
            {
                dbCommand.Connection.DropColumn<Choice>("MimeType");
            }

            dbCommand.Connection.DropForeignKey<Topic>($"{Config.DatabaseObjectQualifier}PollGroupCluster");

            dbCommand.Connection.DropForeignKey<Poll>($"{Config.DatabaseObjectQualifier}PollGroupCluster");

            dbCommand.Connection.DropForeignKey<Forum>($"{Config.DatabaseObjectQualifier}PollGroupCluster");

            dbCommand.Connection.DropForeignKey<Category>($"{Config.DatabaseObjectQualifier}PollGroupCluster");

            dbCommand.Connection.DropForeignKey<PollGroupCluster>($"{Config.DatabaseObjectQualifier}PollGroupCluster");

            if (dbCommand.Connection.TableExists("PollVoteRefuse"))
            {
                dbCommand.Connection.DropTable("PollVoteRefuse");
            }

            // Drop Columns first
            if (dbCommand.Connection.ColumnExists<Poll>("PollGroupID"))
            {
                dbCommand.Connection.DropColumn<Poll>("PollGroupID");
            }

            if (dbCommand.Connection.ColumnExists<Category>("PollGroupID"))
            {
                dbCommand.Connection.DropColumn<Category>("PollGroupID");
            }

            if (dbCommand.Connection.ColumnExists<Forum>("PollGroupID"))
            {
                dbCommand.Connection.DropColumn<Forum>("PollGroupID");
            }

            if (dbCommand.Connection.TableExists<PollGroupCluster>())
            {
                dbCommand.Connection.DropTable<PollGroupCluster>();
            }
        }

        private void UpgradeTable(IRepository<Forum> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.RemoteURL))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.RemoteURL);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.ModeratedPostCount))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.ModeratedPostCount);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.IsModeratedNewTopicOnly))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.IsModeratedNewTopicOnly);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.Flags);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.ThemeURL))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.ThemeURL);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.ImageURL))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.ImageURL);
            }

            if (dbCommand.Connection.ColumnExists<Forum>("Locked"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Forum>()} set Flags = Flags | 1 where Locked<>0");

                dbCommand.Connection.DropColumn<Forum>("Locked");
            }

            if (dbCommand.Connection.ColumnExists<Forum>("Hidden"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Forum>()} set Flags = Flags | 2 where Hidden<>0");

                dbCommand.Connection.DropColumn<Forum>("Hidden");
            }

            if (dbCommand.Connection.ColumnExists<Forum>("IsTest"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Forum>()} set Flags = Flags | 4 where IsTest<>0");

                dbCommand.Connection.DropColumn<Forum>("IsTest");
            }

            if (dbCommand.Connection.ColumnExists<Forum>("Moderated"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Forum>()} set Flags = Flags | 8 where Moderated<>0");

                dbCommand.Connection.DropColumn<Forum>("Moderated");
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.Styles))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.Styles);
            }

            if (dbCommand.Connection.ColumnMaxLength<Forum>(x => x.LastUserName) < 255)
            {
                dbCommand.Connection.AlterColumn<Forum>(x => x.LastUserName);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.LastUserDisplayName))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.LastUserDisplayName);
            }

            if (!dbCommand.Connection.ColumnExists<Forum>(x => x.UserID))
            {
                dbCommand.Connection.AddColumn<Forum>(x => x.UserID);
            }

            if (!dbCommand.Connection.ColumnIsNullable<Forum>(x => x.Description))
            {
                dbCommand.Connection.AlterColumn<Forum>(x => x.Description);
            }
        }

        private void UpgradeTable(IRepository<ForumReadTracking> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            dbCommand.Connection.DropPrimaryKey<ForumReadTracking>(
                $"{Config.DatabaseObjectQualifier}ForumReadTracking");
        }

        private void UpgradeTable(IRepository<UserPMessage> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<UserPMessage>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<UserPMessage>(x => x.Flags);
            }

            if (!dbCommand.Connection.ColumnExists<UserPMessage>(x => x.IsReply))
            {
                dbCommand.Connection.AddColumn<UserPMessage>(x => x.IsReply);
            }
        }

        private void UpgradeTable(IRepository<Group> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnMaxLength<Group>(x => x.Name) < 255)
            {
                dbCommand.Connection.DropIndex<Group>();

                dbCommand.Connection.AlterColumn<Group>(x => x.Name);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.Flags);
            }

            if (dbCommand.Connection.ColumnExists<Group>("IsAmin"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Group>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Group>()} set Flags = Flags | 1 where IsAdmin<>0");

                dbCommand.Connection.DropColumn<Group>("IsAdmin");
            }

            if (dbCommand.Connection.ColumnExists<Group>("IsGuest"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Group>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Group>()} set Flags = Flags | 2 where IsGuest<>0");

                dbCommand.Connection.DropColumn<Group>("IsGuest");
            }

            if (dbCommand.Connection.ColumnExists<Group>("IsStart"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Group>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Group>()} set Flags = Flags | 4 where IsStart<>0");

                dbCommand.Connection.DropColumn<Group>("IsStart");
            }

            if (dbCommand.Connection.ColumnExists<Group>("IsModerator"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Group>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Group>()} set Flags = Flags | 8 where IsModerator<>0");

                dbCommand.Connection.DropColumn<Group>("IsModerator");
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.PMLimit))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.PMLimit);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.Style))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.Style);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.SortOrder))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.SortOrder);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.Description))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.Description);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.UsrSigChars))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.UsrSigChars);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.UsrSigBBCodes))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.UsrSigBBCodes);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.UsrAlbums))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.UsrAlbums);
            }

            if (!dbCommand.Connection.ColumnExists<Group>(x => x.UsrAlbumImages))
            {
                dbCommand.Connection.AddColumn<Group>(x => x.UsrAlbumImages);
            }

            if (dbCommand.Connection.ColumnExists<Group>("UsrSigHTMLTags"))
            {
                dbCommand.Connection.DropColumn<Group>("UsrSigHTMLTags");
            }
        }

        private void UpgradeTable(IRepository<UserMedal> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<UserMedal>("OnlyRibbon"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserMedal>();

                dbCommand.Connection.DropConstraint<UserMedal>(
                    $"DF_{Config.DatabaseObjectQualifier}{nameof(UserMedal)}_OnlyRibbon");

                dbCommand.Connection.DropColumn<UserMedal>("OnlyRibbon");
            }
        }

        private void UpgradeTable(IRepository<GroupMedal> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<GroupMedal>("OnlyRibbon"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<GroupMedal>();

                // delete any old medals without valid groups.
                dbCommand.Connection.ExecuteSql(
                    $@"DELETE FROM {expression.Table<GroupMedal>()} WHERE GroupID NOT IN (SELECT GroupID FROM {expression.Table<Group>()})");

                dbCommand.Connection.DropConstraint<GroupMedal>(
                    $"DF_{Config.DatabaseObjectQualifier}{nameof(GroupMedal)}_OnlyRibbon");

                dbCommand.Connection.DropColumn<GroupMedal>("OnlyRibbon");
            }
        }

        private void UpgradeTable(IRepository<Medal> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<Medal>(x => x.Description) == "ntext")
            {
                dbCommand.Connection.AlterColumn<Medal>(x => x.Description);
            }

            if (dbCommand.Connection.ColumnExists<Medal>("SmallMedalWidth"))
            {
                dbCommand.Connection.DropColumn<Medal>("SmallMedalWidth");
                dbCommand.Connection.DropColumn<Medal>("SmallMedalHeight");
                dbCommand.Connection.DropColumn<Medal>("SmallRibbonWidth");
                dbCommand.Connection.DropColumn<Medal>("SmallRibbonHeight");
                dbCommand.Connection.DropColumn<Medal>("RibbonURL");
                dbCommand.Connection.DropColumn<Medal>("SmallMedalURL");
                dbCommand.Connection.DropColumn<Medal>("SmallRibbonURL");

                dbCommand.Connection.DropConstraint<Medal>(
                    $"DF_{Config.DatabaseObjectQualifier}{nameof(Medal)}_DefaultOrder");
                dbCommand.Connection.DropColumn<Medal>("SortOrder");
            }
        }

        private void UpgradeTable(IRepository<Message> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<Message>(x => x.Flags))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                dbCommand.Connection.AddColumn<Message>(x => x.Flags);

                dbCommand.Connection.ExecuteSql($@" update {expression.Table<Message>()} set Flags = Flags & 7");
            }

            if (dbCommand.Connection.ColumnDataType<Message>(x => x.MessageText) == "ntext")
            {
                dbCommand.Connection.AlterColumn<Message>(x => x.MessageText);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.UserDisplayName))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.UserDisplayName);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.EditReason))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.EditReason);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.IsModeratorChanged))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.IsModeratorChanged);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.DeleteReason))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.DeleteReason);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.EditedBy))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.EditedBy);
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.ExternalMessageId))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.ExternalMessageId);
            }
            else
            {
                if (dbCommand.Connection.ColumnMaxLength<Message>(x => x.ExternalMessageId) < 255)
                {
                    dbCommand.Connection.AlterColumn<Message>(x => x.ExternalMessageId);
                }
            }

            if (!dbCommand.Connection.ColumnExists<Message>(x => x.ReferenceMessageId))
            {
                dbCommand.Connection.AddColumn<Message>(x => x.ReferenceMessageId);
            }

            if (dbCommand.Connection.ColumnMaxLength<Message>(x => x.IP) < 39)
            {
                dbCommand.Connection.AlterColumn<Message>(x => x.IP);
            }

            if (dbCommand.Connection.ColumnMaxLength<Message>(x => x.UserName) < 255)
            {
                dbCommand.Connection.AlterColumn<Message>(x => x.UserName);
            }

            if (dbCommand.Connection.ColumnExists<Message>("Approved"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<Message>()} set Flags = Flags | 16 where Approved <> 0");

                dbCommand.Connection.DropColumn<Message>("Approved");
            }
        }

        private void UpgradeTable(IRepository<MessageHistory> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<MessageHistory>(x => x.Message) == "ntext")
            {
                dbCommand.Connection.AlterColumn<MessageHistory>(x => x.Message);
            }

            if (dbCommand.Connection.ColumnMaxLength<MessageHistory>(x => x.IP) < 39)
            {
                dbCommand.Connection.AlterColumn<MessageHistory>(x => x.IP);
            }

            if (dbCommand.Connection.ColumnIsNullable<MessageHistory>(x => x.Edited))
            {
                // the dependency should be dropped first
                dbCommand.Connection.DropIndex<MessageHistory>();

                var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageHistory>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<MessageHistory>()} set Edited = GETDATE() WHERE Edited IS NULL");

                dbCommand.Connection.AlterColumn<MessageHistory>(x => x.Edited);
            }

            if (dbCommand.Connection.ColumnExists<MessageHistory>("MessageHistoryID"))
            {
                dbCommand.Connection.DropConstraint<MessageHistory>(
                    $"DF_{Config.DatabaseObjectQualifier}{nameof(MessageHistory)}_MessageHistoryID");

                dbCommand.Connection.DropColumn<MessageHistory>("MessageHistoryID");
            }
        }

        private void UpgradeTable(IRepository<MessageReported> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<MessageReported>(x => x.Message) == "ntext")
            {
                dbCommand.Connection.AlterColumn<MessageReported>(x => x.Message);
            }
        }

        private void UpgradeTable(
            IRepository<MessageReportedAudit> repository,
            IDbAccess dbAccess,
            IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<MessageReportedAudit>(x => x.ReportedNumber))
            {
                dbCommand.Connection.AddColumn<MessageReportedAudit>(x => x.ReportedNumber);
            }

            if (!dbCommand.Connection.ColumnExists<MessageReportedAudit>(x => x.ReportText))
            {
                dbCommand.Connection.AddColumn<MessageReportedAudit>(x => x.ReportText);
            }

            if (dbCommand.Connection.ColumnIsNullable<MessageReportedAudit>(x => x.MessageID))
            {
                dbCommand.Connection.AlterColumn<MessageReportedAudit>(x => x.MessageID);
            }
        }

        private void UpgradeTable(IRepository<NntpForum> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<NntpForum>(x => x.DateCutOff))
            {
                dbCommand.Connection.AddColumn<NntpForum>(x => x.DateCutOff);
            }

            if (!dbCommand.Connection.ColumnExists<NntpForum>(x => x.Active))
            {
                dbCommand.Connection.AddColumn<NntpForum>(x => x.Active);
            }
        }

        private void UpgradeTable(IRepository<NntpServer> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnMaxLength<NntpServer>(x => x.UserName) < 255)
            {
                dbCommand.Connection.AlterColumn<NntpServer>(x => x.UserName);
            }
        }

        private void UpgradeTable(IRepository<NntpTopic> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<NntpTopic>(x => x.Thread))
            {
                dbCommand.Connection.AddColumn<NntpTopic>(x => x.Thread);
            }
        }

        private void UpgradeTable(IRepository<PMessage> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<PMessage>(x => x.Body) == "ntext")
            {
                dbCommand.Connection.AlterColumn<PMessage>(x => x.Body);
            }

            if (!dbCommand.Connection.ColumnExists<PMessage>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<PMessage>(x => x.Flags);
            }

            if (!dbCommand.Connection.ColumnExists<PMessage>(x => x.ReplyTo))
            {
                dbCommand.Connection.AddColumn<PMessage>(x => x.ReplyTo);
            }
        }

        private void UpgradeTable(IRepository<PollVote> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<PollVote>(x => x.ChoiceID))
            {
                dbCommand.Connection.AddColumn<PollVote>(x => x.ChoiceID);
            }
        }

        private void UpgradeTable(IRepository<AccessMask> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<AccessMask>(x => x.Flags))
            {
                dbCommand.Connection.AddColumn<AccessMask>(x => x.Flags);
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("ReadAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 1 where ReadAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("ReadAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("PostAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 2 where PostAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("PostAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("ReplyAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 4 where ReplyAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("ReplyAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("PriorityAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 8 where PriorityAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("PriorityAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("PollAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 16 where PollAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("PollAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("VoteAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 32 where VoteAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("VoteAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("ModeratorAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 64 where ModeratorAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("ModeratorAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("EditAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 128 where EditAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("EditAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("DeleteAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 256 where DeleteAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("DeleteAccess");
            }

            if (dbCommand.Connection.ColumnExists<AccessMask>("UploadAccess"))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<AccessMask>();

                dbCommand.Connection.ExecuteSql(
                    $@" update {expression.Table<AccessMask>()} set Flags = Flags | 512 where UploadAccess<>0");

                dbCommand.Connection.DropColumn<AccessMask>("UploadAccess");
            }

            if (!dbCommand.Connection.ColumnExists<AccessMask>(x => x.SortOrder))
            {
                dbCommand.Connection.AddColumn<AccessMask>(x => x.SortOrder);
            }
        }

        /// <summary>
        /// Deletes all the stored procedures.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void DeleteStoredProcedures(IDbCommand dbCommand)
        {
            var list = dbCommand.Connection.SqlList<string>(
                $@"DECLARE @DropScript varchar(max)

                   set @DropScript = ''

                   SELECT 'DROP PROC '+SCHEMA_NAME(schema_id)+'.' + name +';'
                   FROM sys.procedures
                   WHERE TYPE='P'
                   AND name like '{Config.DatabaseObjectQualifier}%'");

            if (list.Any())
            {
                list.ForEach(drop => dbCommand.Connection.ExecuteSql(drop));
            }
        }

        /// <summary>
        /// Deletes the triggers.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void DeleteTriggers(IDbCommand dbCommand)
        {
            var list = dbCommand.Connection.SqlList<string>(
                $@"DECLARE @DropScript varchar(max)

                   set @DropScript = ''

                   SELECT 'DROP TRIGGER '+ OBJECT_SCHEMA_NAME(object_id) +'.' + name +';'
                   FROM sys.triggers
                   where name like '{Config.DatabaseObjectQualifier}%'");

            if (list.Any())
            {
                list.ForEach(drop => dbCommand.Connection.ExecuteSql(drop));
            }
        }

        /// <summary>
        /// Initializes the display names.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void InitDisplayNames(IDbCommand dbCommand)
        {
            var expression = dbCommand.Connection.From<Message>().Where(x => x.UserDisplayName == null)
                .Select(Sql.Count("*"));

            if (dbCommand.Connection.SqlScalar<int>(expression) > 0)
            {
                var sql = $@"declare @tmpUserName nvarchar(255)
                             declare @tmpUserDisplayName nvarchar(255)
                             declare @tmpLastUserName nvarchar(255)
                             declare @tmpLastUserDisplayName nvarchar(255)
                             declare @tmp int
                             declare @tmpUserID int
                             declare @tmpLastUserID int

                              update d set d.LastUserDisplayName = ISNULL((select top 1 f.LastUserDisplayName FROM {expression.Table<Forum>()} f
                              join {expression.Table<User>()} u on u.UserID = f.UserID where u.UserID = d.UserID),
                                    (select top 1 f.LastUserName FROM {expression.Table<Forum>()} f
                              join {expression.Table<User>()} u on u.UserID = f.UserID where u.UserID = d.UserID ))
                              from  {expression.Table<Forum>()} d where d.LastUserDisplayName IS NULL OR d.LastUserDisplayName = d.LastUserName;

                              update d set d.UserDisplayName = ISNULL((select top 1 m.UserDisplayName FROM {expression.Table<Message>()} m
                              join {expression.Table<User>()} u on u.UserID = m.UserID where u.UserID = d.UserID),
                                   (select top 1 m.UserName FROM {expression.Table<Message>()} m
                              join {expression.Table<User>()} u on u.UserID = m.UserID where u.UserID = d.UserID ))
                              from  {expression.Table<Message>()} d where d.UserDisplayName IS NULL OR d.UserDisplayName = d.UserName;

                              update d set d.UserDisplayName = ISNULL((select top 1 t.UserDisplayName FROM {expression.Table<Topic>()} t
                              join {expression.Table<User>()} u on u.UserID = t.UserID where u.UserID = d.UserID),
                                   (select top 1 t.UserName FROM {expression.Table<Topic>()} t
                              join {expression.Table<User>()} u on u.UserID = t.UserID where u.UserID = d.UserID ))
                              from  {expression.Table<Message>()} d where d.UserDisplayName IS NULL OR d.UserDisplayName = d.UserName;";

                dbCommand.Connection.ExecuteSql(sql);
            }
        }

        /// <summary>
        /// Removes the legacy full text search.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void RemoveLegacyFullTextSearch(IDbCommand dbCommand)
        {
            var sb = new StringBuilder();

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

            sb.Append(
                $"if exists (select top 1 1 from sys.columns where object_id = object_id('{expression.Table<Message>()}')");
            sb.Append(
                $"and name = 'Message' and system_type_id = 99 and exists(select * from sys.sysfulltextcatalogs where name = N'YafSearch'))");
            sb.AppendLine($"begin");
            sb.AppendLine($"alter fulltext index on [dbo].[yaf_Message] drop ([Message])");

            sb.AppendLine($"alter table {expression.Table<Message>()} alter column [Message] nvarchar(max)");
            sb.AppendLine($"end");

            dbCommand.Connection.ExecuteSql(sb.ToString());
        }

        /// <summary>
        /// Drop indexes on views here
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void DropIndexViews(IDbCommand dbCommand)
        {
            dbCommand.Connection.DropViewIndex<vaccess_user>("UserForum_PK");
            dbCommand.Connection.DropViewIndex<vaccess_null>("UserForum_PK");
            dbCommand.Connection.DropViewIndex<vaccess_group>("UserForum_PK");
        }

        /// <summary>
        /// Drops the views.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void DropViews(IDbCommand dbCommand)
        {
            dbCommand.Connection.DropView<vaccess>();
            dbCommand.Connection.DropView<vaccessfull>();
            dbCommand.Connection.DropView<vaccess_group>();
            dbCommand.Connection.DropView<vaccess_null>();
            dbCommand.Connection.DropView<vaccess_user>();
        }

        /// <summary>
        /// Drops the scalar functions.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        private void DropFunctions(IDbCommand dbCommand)
        {
            dbCommand.Connection.DropFunction("registry_value");
            dbCommand.Connection.DropFunction("bitset");
            dbCommand.Connection.DropFunction("forum_posts");
            dbCommand.Connection.DropFunction("forum_topics");
            dbCommand.Connection.DropFunction("forum_subforums");
            dbCommand.Connection.DropFunction("forum_lasttopic");
            dbCommand.Connection.DropFunction("forum_lastposted");
            dbCommand.Connection.DropFunction("medal_getribbonsetting");
            dbCommand.Connection.DropFunction("medal_getsortorder");
            dbCommand.Connection.DropFunction("medal_gethide");
            dbCommand.Connection.DropFunction("get_userstyle");
            dbCommand.Connection.DropFunction("message_getthanksinfo");
            dbCommand.Connection.DropFunction("forum_save_parentschecker");
            dbCommand.Connection.DropFunction("Split");
        }
    }
}