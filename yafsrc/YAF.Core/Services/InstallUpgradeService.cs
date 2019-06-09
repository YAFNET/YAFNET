/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;

    using ServiceStack.OrmLite;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services.Import;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
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
        ///     The File type extensions import xml file.
        /// </summary>
        private const string FileImport = "fileExtensions.xml";

        /// <summary>
        ///     The Spam Words list import xml file.
        /// </summary>
        private const string SpamWordsImport = "SpamWords.xml";

        #endregion

        #region Fields

        /// <summary>
        ///     The messages.
        /// </summary>
        private readonly List<string> messages = new List<string>();

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
                    var boards = this.GetRepository<Board>().List();
                    return boards.HasRows();
                }
                catch
                {
                    // failure... no boards.
                }

                return false;
            }
        }

        /// <summary>
        ///     Gets the messages.
        /// </summary>
        public string[] Messages => this.messages.ToArray();

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
        /// <param name="forumName">The forum name.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="forumEmail">The forum email.</param>
        /// <param name="forumBaseUrlMask">The forum base URL mask.</param>
        /// <param name="adminUserName">The admin user name.</param>
        /// <param name="adminEmail">The admin email.</param>
        /// <param name="adminProviderUserKey">The admin provider user key.</param>
        public void InitializeForum(
            string forumName, string timeZone, string culture, string forumEmail, string forumBaseUrlMask, string adminUserName, string adminEmail, object adminProviderUserKey)
        {
            var cult = StaticDataHelper.Cultures();
            var langFile = "english.xml";

            foreach (var drow in cult.Rows.Cast<DataRow>().Where(drow => drow["CultureTag"].ToString() == culture))
            {
                langFile = (string)drow["CultureFile"];
            }

            this.GetRepository<Board>().SystemInitialize(
                forumName,
                timeZone,
                culture,
                langFile,
                forumEmail,
                forumBaseUrlMask,
                string.Empty,
                adminUserName,
                adminEmail,
                adminProviderUserKey,
                Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

            this.GetRepository<Registry>().Save("version", YafForumInfo.AppVersion.ToString());
            this.GetRepository<Registry>().Save("versionname", YafForumInfo.AppVersionName);

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
            this.messages.Clear();
            {
                var isForumInstalled = this.IsForumInstalled;

                // try
                this.FixAccess(false);

                var isAzureEngine = this.Get<IDbFunction>().GetSQLEngine().Equals("Azure");

                if (!isForumInstalled)
                {
                    this.ExecuteInstallScripts(isAzureEngine);
                }
                else
                {
                    this.ExecuteUpgradeScripts(isAzureEngine);
                }

                this.FixAccess(true);

                var prevVersion = this.GetRepository<Registry>().GetDbVersion();

                this.GetRepository<Registry>().Save("version", YafForumInfo.AppVersion.ToString());
                this.GetRepository<Registry>().Save("versionname", YafForumInfo.AppVersionName);

                // Ederon : 9/7/2007
                // resync all boards - necessary for propr last post bubbling
                this.GetRepository<Board>().Resync();

                this.RaiseEvent.RaiseIssolated(
                    new AfterUpgradeDatabaseEvent(prevVersion, YafForumInfo.AppVersion),
                    null);

                if (isForumInstalled)
                {
                    if (prevVersion < 30 || upgradeExtensions)
                    {
                        this.ImportStatics();
                    }

                    if (prevVersion < 42)
                    {
                        // un-html encode all topic subject names...
                        this.GetRepository<Topic>().UnencodeAllTopicsSubjects(HttpUtility.HtmlDecode);
                    }

                    if (prevVersion < 70)
                    {
                        // Reset The UserBox Template
                        try
                        {
                            this.Get<YafBoardSettings>().UserBox = Constants.UserBox.DisplayTemplateDefault;
                            this.Get<YafBoardSettings>().UserBoxAvatar = @"<li class=""list-group-item"">{0}</li>";
                            this.Get<YafBoardSettings>().UserBoxMedals = @"<li class=""list-group-item""><strong>{0}</strong><br /> {1}{2}</li>";
                            this.Get<YafBoardSettings>().UserBoxRankImage = @"<li class=""list-group-item"">{0}</li>";
                            this.Get<YafBoardSettings>().UserBoxRank = @"<li class=""list-group-item""><strong>{0}:</strong> {1}</li>";
                            this.Get<YafBoardSettings>().UserBoxGroups = @"<li class=""list-group-item""><strong>{0}:</strong><br /> {1}</li>";
                            this.Get<YafBoardSettings>().UserBoxJoinDate = @"<li class=""list-group-item""><strong>{0}:</strong> {1}</li>";
                            this.Get<YafBoardSettings>().UserBoxPosts = @"<li class=""list-group-item""><strong>{0}:</strong> {1:N0}</li>";
                            this.Get<YafBoardSettings>().UserBoxReputation = @"<li class=""list-group-item""><strong>{0}:</strong> {1:N0}</li>";
                            this.Get<YafBoardSettings>().UserBoxCountryImage = @"{0}";
                            this.Get<YafBoardSettings>().UserBoxLocation = @"<li class=""list-group-item""><strong>{0}:</strong> {1}</li>";
                            this.Get<YafBoardSettings>().UserBoxGender = @"{0}&nbsp;";
                            this.Get<YafBoardSettings>().UserBoxThanksFrom = @"<li class=""list-group-item"">{0}</li>";
                            this.Get<YafBoardSettings>().UserBoxThanksTo = @"<li class=""list-group-item"">{0}</li>";

                            ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();
                        }
                        catch (Exception)
                        {
                            this.GetRepository<Registry>().Save("userbox", Constants.UserBox.DisplayTemplateDefault);
                        }
                    }

                    try
                    {
                        // Check if BaskeUrlMask is set and if not automatically write it
                        if (this.Get<YafBoardSettings>().BaseUrlMask.IsNotSet())
                        {
                            this.Get<YafBoardSettings>().BaseUrlMask = BaseUrlBuilder.GetBaseUrlFromVariables();

                            ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();
                        }
                    }
                    catch (Exception)
                    {
                        this.GetRepository<Registry>().Save("baseurlmask", BaseUrlBuilder.GetBaseUrlFromVariables());
                    }
                }

                // vzrus: uncomment it to not keep install/upgrade objects in DB and for better security
                // DB.system_deleteinstallobjects();
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
        /// <param name="isAzureEngine">if set to <c>true</c> [is azure engine].</param>
        private void ExecuteInstallScripts(bool isAzureEngine)
        {
            // Install Membership Scripts
            if (isAzureEngine)
            {
                this.DbAccess.Information.AzureScripts.ForEach(script => this.ExecuteScript(script, true));
            }
            else
            {
                this.DbAccess.Information.YAFProviderInstallScripts.ForEach(script => this.ExecuteScript(script, true));
            }

            //////

            // Run other
            this.DbAccess.Information.InstallScripts.ForEach(script => this.ExecuteScript(script, true));
        }

        /// <summary>
        /// Executes the upgrade scripts.
        /// </summary>
        /// <param name="isAzureEngine">if set to <c>true</c> [is azure engine].</param>
        private void ExecuteUpgradeScripts(bool isAzureEngine)
        {
            // upgrade Membership Scripts
            if (!isAzureEngine)
            {
                this.DbAccess.Information.YAFProviderUpgradeScripts.ForEach(script => this.ExecuteScript(script, true));
            }

            this.DbAccess.Information.UpgradeScripts.ForEach(script => this.ExecuteScript(script, true));
        }

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
        }

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

            this.Get<IDbFunction>().SystemInitializeExecutescripts(script, scriptFile, useTransactions);
        }

        /// <summary>
        /// Fixes the access.
        /// </summary>
        /// <param name="grantAccess">if set to <c>true</c> [grant access].</param>
        private void FixAccess(bool grantAccess)
        {
            this.Get<IDbFunction>().SystemInitializeFixaccess(grantAccess);
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
            foreach (var board in boards)
            {
                this.Get<IRaiseEvent>().Raise(new ImportStaticDataEvent(board.ID));

                // load default bbcode if available...
                loadWrapper(BbcodeImport, s => DataImport.BBCodeExtensionImport(board.ID, s));

                // load default extensions if available...
                loadWrapper(FileImport, s => DataImport.FileExtensionImport(board.ID, s));

                // load default spam word if available...
                loadWrapper(SpamWordsImport, s => DataImport.SpamWordsImport(board.ID, s));
            }
        }

        #endregion
    }
}