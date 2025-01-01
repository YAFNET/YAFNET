/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using YAF.Core.Migrations;

namespace YAF.Core.Services;

using Microsoft.AspNetCore.Hosting;

using System;
using System.IO;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
///     The install upgrade service.
/// </summary>
public class InstallService : IHaveServiceLocator
{
    /// <summary>
    ///     The BBCode extensions import xml file.
    /// </summary>
    private const string BbcodeImport = "BBCodeExtensions.xml";

    /// <summary>
    ///     The Spam Words list import xml file.
    /// </summary>
    private const string SpamWordsImport = "SpamWords.xml";

    /// <summary>
    /// Initializes a new instance of the <see cref="InstallService"/> class.
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
    public InstallService(IServiceLocator serviceLocator, IRaiseEvent raiseEvent, IDbAccess access)
    {
        this.RaiseEvent = raiseEvent;
        this.DbAccess = access;
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///     Gets a value indicating whether this instance is forum installed.
    /// </summary>
    public bool IsForumInstalled => this.GetRepository<Board>().TableExists() && this.GetRepository<Board>().Exists(b => b.ID > 0);

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

    /// <summary>
    /// Initializes the forum.
    /// </summary>
    /// <param name="applicationId">
    /// The application Id.
    /// </param>
    /// <param name="forumName">
    /// The forum name.
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
        string culture,
        string forumEmail,
        string forumLogo,
        string forumBaseUrlMask,
        string adminUserName,
        string adminEmail,
        string adminProviderUserKey)
    {
        ArgumentNullException.ThrowIfNull(forumName);
        ArgumentNullException.ThrowIfNull(forumName);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(forumLogo);
        ArgumentNullException.ThrowIfNull(forumBaseUrlMask);
        ArgumentNullException.ThrowIfNull(adminUserName);
        ArgumentNullException.ThrowIfNull(adminEmail);
        ArgumentNullException.ThrowIfNull(adminProviderUserKey);

        var cult = StaticDataHelper.Cultures();

        var languageFromCulture = cult
            .FirstOrDefault(c => c.CultureTag == culture);

        var langFile = languageFromCulture != null ? languageFromCulture.CultureFile : "english.json";

        // -- initialize required 'registry' settings
        this.GetRepository<Registry>().Save("applicationid", applicationId.ToString());

        if (forumEmail.IsSet())
        {
            this.GetRepository<Registry>().Save("forumemail", forumEmail);
        }

        this.GetRepository<Registry>().Save("forumlogo", forumLogo);
        this.GetRepository<Registry>().Save("baseurlmask", forumBaseUrlMask);

        var boardId = this.GetRepository<Board>().Create(
            forumName,
            forumEmail,
            culture,
            langFile,
            adminUserName,
            adminEmail,
            adminProviderUserKey,
            true,
            string.Empty);

        // reload the board settings...
        BoardContext.Current.BoardSettings = this.Get<BoardSettingsService>().LoadBoardSettings(boardId, null);

        this.CreateUploadsFolder();

        this.AddOrUpdateExtensions();
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
    public bool TestDatabaseConnection(out string exceptionMessage)
    {
        return this.DbAccess.TestConnection(out exceptionMessage);
    }

    /// <summary>
    /// Initialize Or Upgrade the Database
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool InitializeDatabase()
    {
        var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration01));

        migrator.Run();

        // Create views
        this.DbAccess.Execute(dbCommand => this.DbAccess.Information.CreateViews(this.DbAccess, dbCommand));

        this.DbAccess.Execute(dbCommand => this.DbAccess.Information.CreateIndexViews(this.DbAccess, dbCommand));

        this.GetRepository<Registry>().Save("version", this.Get<BoardInfo>().AppVersion.ToString());
        this.GetRepository<Registry>().Save("versionname", this.Get<BoardInfo>().AppVersionName);

        this.GetRepository<Registry>().Save("cdvversion", this.Get<BoardSettings>().CdvVersion++);

        return true;
    }

    /// <summary>
    /// Creates the uploads folder if not exist.
    /// </summary>
    private void CreateUploadsFolder()
    {
        var uploadFolder = Path.Combine(
            this.Get<IWebHostEnvironment>().WebRootPath,
            this.Get<BoardFolders>().Uploads);

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }
    }


    /// <summary>
    ///    Add or Update BBCode Extensions and Spam Words
    /// </summary>
    private void AddOrUpdateExtensions()
    {
        var loadWrapper = new Action<string, Action<Stream>>(
            (file, streamAction) =>
                {
                    var fullFile = Path.Combine(this.Get<BoardInfo>().WebRootPath, "Resources", file);

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
                    loadWrapper(BbcodeImport, s => this.Get<IDataImporter>().BBCodeExtensionImport(boardId, s));

                    // load default spam word if available...
                    loadWrapper(SpamWordsImport, s => this.Get<IDataImporter>().SpamWordsImport(boardId, s));
                });
    }
}