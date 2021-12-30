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

namespace YAF.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.Import;
    using YAF.Core.Services.Migrations;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    /// <summary>
    ///     The upgrade service.
    /// </summary>
    public class UpgradeService : IHaveServiceLocator
    {
        #region Constants

        /// <summary>
        ///     The BBCode extensions import xml file.
        /// </summary>
        private const string BbcodeImport = "BBCodeExtensions.xml";

        /// <summary>
        ///     The Spam Words list import xml file.
        /// </summary>
        private const string SpamWordsImport = "SpamWords.xml";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeService"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="raiseEvent">
        /// The raise Event.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        public UpgradeService(IServiceLocator serviceLocator, IRaiseEvent raiseEvent, IDbAccess access)
        {
            this.RaiseEvent = raiseEvent;
            this.DbAccess = access;
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

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
        /// Initialize Or Upgrade the Database
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Upgrade()
        {
            this.CreateOrUpdateTables();

            this.AddOrUpdateExtensions();

            var prevVersion = this.GetRepository<Registry>().GetDbVersion();

            this.GetRepository<Registry>().Save("version", BoardInfo.AppVersion.ToString());
            this.GetRepository<Registry>().Save("versionname", BoardInfo.AppVersionName);

            if (prevVersion < 81)
            {
                this.Get<V81_Migration>().MigrateDatabase(this.DbAccess);
            }

            this.GetRepository<Registry>().Save("cdvversion", this.Get<BoardSettings>().CdvVersion++);

            this.Get<IDataCache>().Remove(Constants.Cache.Version);

            this.Get<ILoggerService>().Info($"YAF.NET Upgraded to Version {BoardInfo.AppVersionName}");

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Crate Tables and Update Tables
        /// </summary>
        private void CreateOrUpdateTables()
        {
            this.CreateTablesIfNotExists();
        }

        /// <summary>
        /// Create missing tables
        /// </summary>
        private void CreateTablesIfNotExists()
        {
            //this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Board>());
        }

        /// <summary>
        ///    Add or Update BBCode Extensions and Spam Words
        /// </summary>
        private void AddOrUpdateExtensions()
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
                        using var stream = new StreamReader(fullFile);
                        streamAction(stream.BaseStream);
                        stream.Close();
                    });

            // get all boards...
            var boardIds = this.GetRepository<Board>().GetAll().Select(x => x.ID);

            // Upgrade all Boards
            boardIds.ForEach(
                boardId =>
                    {
                        this.Get<IRaiseEvent>().Raise(new ImportStaticDataEvent(boardId));

                        // load default bbcode if available...
                        loadWrapper(BbcodeImport, s => DataImport.BBCodeExtensionImport(boardId, s));

                        // load default spam word if available...
                        loadWrapper(SpamWordsImport, s => DataImport.SpamWordsImport(boardId, s));
                    });
        }

        /// <summary>
        /// Migrate Legacy Membership Settings
        /// </summary>
        private void MigrateConfig()
        {
            try
            {
            }
            catch (Exception)
            {
                // Can Be ignored if settings have already been removed
            }
        }

        #endregion
    }
}