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

namespace YAF.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;

    using ServiceStack.OrmLite;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services.Import;
    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Utils;

    /// <summary>
    ///     The install upgrade service.
    /// </summary>
    public class InstallUpgradeService : IHaveServiceLocator
    {
        #region Constants

        /// <summary>
        ///     The BBCode extensions import xml file.
        /// </summary>
        private const string BbcodeImport = "bbCodeExtensions.xml";

        /// <summary>
        ///     The Spam Words list import xml file.
        /// </summary>
        private const string SpamWordsImport = "SpamWords.xml";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallUpgradeService" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="raiseEvent">The raise Event.</param>
        /// <param name="dbAccess">The database access.</param>
        public InstallUpgradeService(IServiceLocator serviceLocator, IRaiseEvent raiseEvent, IDbAccess dbAccess)
        {
            this.RaiseEvent = raiseEvent;
            this.DbAccess = dbAccess;
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is forum installed.
        /// </summary>
        public bool IsForumInstalled
        {
            get
            {
                try
                {
                    var boards = this.GetRepository<Board>().GetAll();
                    return boards.Any();
                }
                catch
                {
                    // failure... no boards.
                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the raise event.
        /// </summary>
        public IRaiseEvent RaiseEvent { get; set; }

        /// <summary>
        /// Gets or sets the database access.
        /// </summary>
        /// <value>
        /// The database access.
        /// </value>
        public IDbAccess DbAccess { get; set; }

        /// <summary>
        ///     Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initializes the forum.
        /// </summary>
        /// <param name="applicationId">
        /// The application Id.
        /// </param>
        /// <param name="forumName">
        /// The forum name.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="forumEmail">
        /// The forum email.
        /// </param>
        /// <param name="forumLogo">
        /// The forum Logo.
        /// </param>
        /// <param name="forumBaseUrlMask">
        /// The forum base URL mask.
        /// </param>
        /// <param name="adminUserName">
        /// The admin user name.
        /// </param>
        /// <param name="adminEmail">
        /// The admin email.
        /// </param>
        /// <param name="adminProviderUserKey">
        /// The admin provider user key.
        /// </param>
        public void InitializeForum(
            Guid applicationId,
            string forumName,
            string timeZone,
            string culture,
            string forumEmail,
            string forumLogo,
            string forumBaseUrlMask,
            string adminUserName,
            string adminEmail,
            object adminProviderUserKey)
        {
            var cult = StaticDataHelper.Cultures();
            var langFile = "english.xml";

            cult.Where(dataRow => dataRow.CultureTag == culture)
                .ForEach(dataRow => langFile = dataRow.CultureFile);

            this.GetRepository<Board>().SystemInitialize(
                forumName,
                timeZone,
                culture,
                langFile,
                forumEmail,
                forumLogo,
                forumBaseUrlMask,
                string.Empty,
                adminUserName,
                adminEmail,
                adminProviderUserKey,
                Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

            this.GetRepository<Registry>().Save("applicationid", applicationId.ToString());
            this.GetRepository<Registry>().Save("version", BoardInfo.AppVersion.ToString());
            this.GetRepository<Registry>().Save("versionname", BoardInfo.AppVersionName);

            this.ImportStatics();
        }

        /// <summary>
        /// Tests database connection. Can probably be moved to DB class.
        /// </summary>
        /// <param name="exceptionMessage">
        /// The exception message.
        /// </param>
        /// <returns>
        /// The test database connection.
        /// </returns>
        public bool TestDatabaseConnection([NotNull] out string exceptionMessage)
        {
            return this.DbAccess.TestConnection(out exceptionMessage);
        }

        /// <summary>
        /// The upgrade database.
        /// </summary>
        /// <param name="upgradeExtensions">
        /// The upgrade Extensions.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpgradeDatabase(bool upgradeExtensions)
        {
            var isForumInstalled = this.IsForumInstalled;

            // try
            this.FixAccess(false);

            if (!isForumInstalled)
            {
                this.ExecuteInstallScripts();
            }
            else
            {
                this.ExecuteUpgradeScripts();
            }

            this.FixAccess(true);

            var prevVersion = this.GetRepository<Registry>().GetDbVersion();

            this.GetRepository<Registry>().Save("version", BoardInfo.AppVersion.ToString());
            this.GetRepository<Registry>().Save("versionname", BoardInfo.AppVersionName);

            // Handle Tables
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Tag>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicTag>());

            // Ederon : 9/7/2007
            // re-sync all boards - necessary for proper last post bubbling
            this.GetRepository<Board>().ReSync();

            this.RaiseEvent.RaiseIssolated(new AfterUpgradeDatabaseEvent(prevVersion, BoardInfo.AppVersion), null);

            if (isForumInstalled)
            {
                if (prevVersion < 80)
                {
                    // Upgrade to ASPNET Identity
                    this.DbAccess.Information.IdentityUpgradeScripts.ForEach(script => this.ExecuteScript(script, true));
                }

                if (prevVersion < 30 || upgradeExtensions)
                {
                    this.ImportStatics();
                }

                if (prevVersion < 42)
                {
                    // un-html encode all topic subject names...
                    this.GetRepository<Topic>().UnEncodeAllTopicsSubjects(HttpUtility.HtmlDecode);
                }

                // initialize search index
                if (this.Get<BoardSettings>().LastSearchIndexUpdated.IsNotSet())
                {
                    this.GetRepository<Registry>().Save("forceupdatesearchindex", "1");
                }

                // Check if BaseUrlMask is set and if not automatically write it
                if (this.Get<BoardSettings>().BaseUrlMask.IsNotSet())
                {
                    this.GetRepository<Registry>().Save("baseurlmask", BaseUrlBuilder.GetBaseUrlFromVariables());
                }

                this.GetRepository<Registry>().Save("cdvversion", this.Get<BoardSettings>().CdvVersion++);
            }

            if (this.IsForumInstalled)
            {
                this.ExecuteScript(this.DbAccess.Information.FullTextUpgradeScript, false);
            }

            // run custom script...
            this.ExecuteScript("custom/custom.sql", true);

            if (Config.IsDotNetNuke)
            {
                // run dnn custom script...
                this.ExecuteScript("custom/dnn.sql", true);
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the install scripts.
        /// </summary>
        private void ExecuteInstallScripts()
        {
            // Install Membership Scripts
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUsers>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetRoles>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserClaims>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserLogins>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserRoles>());

            //////

            // Run other
            this.DbAccess.Information.InstallScripts.ForEach(script => this.ExecuteScript(script, true));
        }

        /// <summary>
        /// Executes the upgrade scripts.
        /// </summary>
        private void ExecuteUpgradeScripts()
        {
            this.DbAccess.Information.UpgradeScripts.ForEach(script => this.ExecuteScript(script, true));
        }

        /*
        /// <summary>
        /// Create missing tables
        /// </summary>
        private void CreateTables()
        {
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Board>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Rank>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<User>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PollGroupCluster>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Category>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Forum>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Topic>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Message>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Thanks>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Buddy>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<FavoriteTopic>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserAlbum>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserAlbumImage>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Active>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ActiveAccess>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AdminPageUserAccess>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Group>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<EventLogGroupAccess>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedIP>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedName>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedEmail>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<CheckEmail>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Poll>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Choice>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PollVote>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PollVoteRefuse>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AccessMask>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumAccess>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Mail>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageHistory>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReported>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReportedAudit>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PMessage>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserProfile>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchForum>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchTopic>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Attachment>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserGroup>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserForum>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpServer>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpForum>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpTopic>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Replace_Words>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Spam_Words>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Registry>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<EventLog>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<FileExtension>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BBCode>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Medal>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<GroupMedal>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserMedal>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<IgnoreUser>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicReadTracking>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumReadTracking>());
            this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ReputationVote>());
        }*/

        /// <summary>
        /// The execute script.
        /// </summary>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="useTransactions">
        /// The use transactions.
        /// </param>
        private void ExecuteScript([NotNull] string scriptFile, bool useTransactions)
        {
            string script;
            var fileName = this.Get<HttpRequestBase>().MapPath(scriptFile);

            try
            {
                script = $"{File.ReadAllText(fileName)}\r\n";
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (Exception x)
            {
                throw new IOException($"Failed to read {fileName}", x);
            }

            this.Get<IDbFunction>().SystemInitializeExecuteScripts(script, scriptFile, useTransactions);
        }

        /// <summary>
        /// Fixes the access.
        /// </summary>
        /// <param name="grantAccess">if set to <c>true</c> [grant access].</param>
        private void FixAccess(bool grantAccess)
        {
            this.Get<IDbFunction>().SystemInitializeFixAccess(grantAccess);
        }

        /// <summary>
        ///     The import statics.
        /// </summary>
        private void ImportStatics()
        {
            var loadWrapper = new Action<string, Action<Stream>>(
                (file, streamAction) =>
                    {
                        var fullFile = this.Get<HttpRequestBase>().MapPath(file);

                        if (!File.Exists(fullFile))
                        {
                            return;
                        }

                        // import into board...
                        using (var stream = new StreamReader(fullFile))
                        {
                            streamAction(stream.BaseStream);
                            stream.Close();
                        }
                    });

            var boards = this.GetRepository<Board>().ListTyped();

            // Upgrade all Boards
            boards.ForEach(
                board =>
                    {
                        this.Get<IRaiseEvent>().Raise(new ImportStaticDataEvent(board.ID));

                        // load default bbcode if available...
                        loadWrapper(BbcodeImport, s => DataImport.BBCodeExtensionImport(board.ID, s));

                        // load default spam word if available...
                        loadWrapper(SpamWordsImport, s => DataImport.SpamWordsImport(board.ID, s));
                    });
        }

        #endregion
    }
}