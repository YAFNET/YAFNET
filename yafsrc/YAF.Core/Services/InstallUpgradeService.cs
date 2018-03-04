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

namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
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
        private const string _BbcodeImport = "bbCodeExtensions.xml";

        /// <summary>
        ///     The File type extensions import xml file.
        /// </summary>
        private const string _FileImport = "fileExtensions.xml";

        /// <summary>
        ///     The Topic Status list import xml file.
        /// </summary>
        private const string _TopicStatusImport = "TopicStatusList.xml";

        /// <summary>
        ///     The Spam Words list import xml file.
        /// </summary>
        private const string _SpamWordsImport = "SpamWords.xml";

        #endregion

        #region Fields

        /// <summary>
        ///     The _messages.
        /// </summary>
        private readonly List<string> _messages = new List<string>();

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
        public string[] Messages => this._messages.ToArray();

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

        #region Properties

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

            LegacyDb.system_initialize(
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

            LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

            // vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
            // YAF.Classes.Data.DB.system_deleteinstallobjects();
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
            this._messages.Clear();
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

                var prevVersion = LegacyDb.GetDBVersion();

                LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

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
                        LegacyDb.unencode_all_topics_subjects(HttpUtility.HtmlDecode);
                    }

                    if (prevVersion < 49)
                    {
                        // Reset The UserBox Template
                        try
                        {
                            this.Get<YafBoardSettings>().UserBox = Constants.UserBox.DisplayTemplateDefault;
                            ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();
                        }
                        catch (Exception)
                        {
                            LegacyDb.registry_save("userbox", Constants.UserBox.DisplayTemplateDefault);
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
                        LegacyDb.registry_save("baseurlmask", BaseUrlBuilder.GetBaseUrlFromVariables());
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
                script = "{0}\r\n".FormatWith(File.ReadAllText(fileName));
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (Exception x)
            {
                throw new IOException("Failed to read {0}".FormatWith(fileName), x);
            }

            LegacyDb.system_initialize_executescripts(script, scriptFile, useTransactions);
        }

        /// <summary>
        /// Fixes the access.
        /// </summary>
        /// <param name="grantAccess">if set to <c>true</c> [grant access].</param>
        private void FixAccess(bool grantAccess)
        {
            LegacyDb.system_initialize_fixaccess(grantAccess);
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
                loadWrapper(_BbcodeImport, s => DataImport.BBCodeExtensionImport(board.ID, s));

                // load default extensions if available...
                loadWrapper(_FileImport, s => DataImport.FileExtensionImport(board.ID, s));

                // load default topic status if available...
                loadWrapper(_TopicStatusImport, s => DataImport.TopicStatusImport(board.ID, s));

                // load default spam word if available...
                loadWrapper(_SpamWordsImport, s => DataImport.SpamWordsImport(board.ID, s));
            }
        }

        #endregion
    }
}