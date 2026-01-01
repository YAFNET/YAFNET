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

using YAF.Core.Migrations;

namespace YAF.Core.Services;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using YAF.Core.Data;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
///     The upgrade service.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UpgradeService"/> class.
/// </remarks>
/// <param name="serviceLocator">
/// The service locator.
/// </param>
/// <param name="raiseEvent">
/// The raise Event.
/// </param>
/// <param name="access">
/// The access.
/// </param>
public class UpgradeService(IServiceLocator serviceLocator, IRaiseEvent raiseEvent, IDbAccess access) : IHaveServiceLocator
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
    ///     Gets or sets the raise event.
    /// </summary>
    public IRaiseEvent RaiseEvent { get; set; } = raiseEvent;

    /// <summary>
    /// Gets or sets the database access.
    /// </summary>
    /// <value>
    /// The database access.
    /// </value>
    public IDbAccess DbAccess { get; set; } = access;

    /// <summary>
    ///     Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; } = serviceLocator;

    /// <summary>
    /// Initialize Or Upgrade the Database
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public async Task<bool> UpgradeAsync()
    {
        this.CreateTablesIfNotExists();

        var prevVersion = this.GetRepository<Registry>().GetDbVersion();

        // initialize search index
        if (!await this.GetRepository<Registry>().ExistsAsync(r => r.Name == "lastsearchindexupdated"))
        {
            this.GetRepository<Registry>().Save("forceupdatesearchindex", "1");
        }

        // Check if BaseUrlMask is set and if not automatically write it
        if (!await this.GetRepository<Registry>().ExistsAsync(r => r.Name == "baseurlmask"))
        {
            this.GetRepository<Registry>().Save(
                "baseurlmask",
                this.Get<IHttpContextAccessor>().HttpContext?.Request.BaseUrl());
        }

        if (prevVersion < 80)
        {
            await this.Get<Migration80>().MigrateDatabaseAsync(this.DbAccess);

            // Upgrade to ASPNET Identity
            this.DbAccess.Information.IdentityUpgradeScripts.ForEach(this.ExecuteScript);

            this.MigrateAttachments();

            // Delete old registry Settings
            this.GetRepository<Registry>().DeleteLegacy();

            // update default points from 0 to 1
            await this.GetRepository<User>().UpdateOnlyAsync(() => new User { Points = 1 }, u => u.Points == 0);
        }

        if (prevVersion < 30)
        {
            await this.Get<Migration30>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion < 42)
        {
            // un-html encode all topic subject names...
            this.GetRepository<Topic>().UnEncodeAllTopicsSubjects(HttpUtility.HtmlDecode);
        }

        if (prevVersion < 81)
        {
            await this.Get<Migration81>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion < 82)
        {
            await this.Get<Migration82>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion is 80 or 81 or 82 or 84)
        {
            var registeredRole = await this.GetRepository<AspNetRoles>().GetSingleAsync(x => x.Name == "Registered");

            if (registeredRole != null)
            {
                registeredRole.Name = "Registered Users";

                await this.GetRepository<AspNetRoles>().UpdateAsync(registeredRole);
            }

            var users = this.Get<IAspNetUsersHelper>().GetAllUsers();

            foreach (var user in users)
            {
                var roles = await this.Get<IAspNetRolesHelper>().GetRolesForUserAsync(user);

                var yafUser = await this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKeyAsync(user.Id);

                if (roles.NullOrEmpty())
                {
                    // FIX initial roles (if any) for this user
                    await this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(yafUser.BoardID, user);
                }
            }
        }

        await this.MigratePre40Async(prevVersion);

        this.Migrate(prevVersion);

        await this.AddOrUpdateExtensionsAsync();

        this.Get<IDataCache>().Remove(Constants.Cache.Version);

        this.GetRepository<Registry>().Save("version", this.Get<BoardInfo>().AppVersion.ToString());
        this.GetRepository<Registry>().Save("versionname", this.Get<BoardInfo>().AppVersionName);

        this.Get<ILogger<UpgradeService>>().Info($"YAF.NET Upgraded to Version {this.Get<BoardInfo>().AppVersionName}");

        return true;
    }

    /// <summary>
    /// Migrate DB to latest Version.
    /// </summary>
    /// <param name="prevVersion">The previous version.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="NotImplementedException"></exception>
    private void Migrate(int prevVersion)
    {
        if (prevVersion < 1000)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration1000));

            migrator.Run();
        }

        if (prevVersion < 1001)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration1001));

            migrator.Run();
        }

        if (prevVersion < 1002)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration1002));

            migrator.Run();
        }

        if (prevVersion < 1003)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration1003));

            migrator.Run();
        }
    }

    /// <summary>
    /// Run all pre YAF.NET 4.0.0 DB Migrations
    /// </summary>
    /// <param name="prevVersion">The previous version.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task MigratePre40Async(int prevVersion)
    {
        if (prevVersion < 84)
        {
            await this.Get<Migration84>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion < 85)
        {
            await this.Get<Migration85>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion < 86)
        {
            await this.Get<Migration86>().MigrateDatabaseAsync(this.DbAccess);
        }

        // Execute before 87
        if (prevVersion < 89)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration89));

            migrator.Run();
        }

        if (prevVersion < 87)
        {
            await this.Get<Migration87>().MigrateDatabaseAsync(this.DbAccess);
        }

        if (prevVersion < 91)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration91));

            migrator.Run();
        }

        if (prevVersion < 92)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration92));

            migrator.Run();
        }

        if (prevVersion < 93)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration93));

            migrator.Run();
        }

        if (prevVersion < 94)
        {
            var migrator = new Migrator(this.DbAccess.ResolveDbFactory(), typeof(Migration94));

            migrator.Run();
        }
    }

    /// <summary>
    ///    Add or Update BBCode Extensions and Spam Words
    /// </summary>
    private async Task AddOrUpdateExtensionsAsync()
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
        var boardIds = await this.GetRepository<Board>().GetAllBoardIdsAsync();

        // Upgrade all Boards
        boardIds.ForEach(
            boardId =>
                {
                    this.Get<IRaiseEvent>().Raise(new ImportStaticDataEvent(boardId));

                    // load default bbcode if available...
                    loadWrapper(BbcodeImport, s => this.Get<IDataImporter>().BBCodeExtensionImportAsync(boardId, s));

                    // load default spam word if available...
                    loadWrapper(SpamWordsImport, s => this.Get<IDataImporter>().SpamWordsImport(boardId, s));
                });
    }

    /// <summary>
    /// The execute script.
    /// </summary>
    /// <param name="scriptFile">
    /// The script file.
    /// </param>
    private void ExecuteScript(string scriptFile)
    {
        string script;

        var fileName = Path.Combine(this.Get<BoardInfo>().WebRootPath, "Resources", scriptFile);

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

        this.Get<IDbAccess>().SystemInitializeExecuteScripts(
            CommandTextHelpers.GetCommandTextReplaced(script),
            scriptFile,
            Config.SqlCommandTimeout);
    }

    /// <summary>
    /// Create missing tables
    /// </summary>
    private void CreateTablesIfNotExists()
    {
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Board>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Rank>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<User>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Category>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Forum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Topic>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Message>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Thanks>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Buddy>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserAlbum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserAlbumImage>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Active>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ActiveAccess>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Activity>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AdminPageUserAccess>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Group>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedIP>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedName>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedEmail>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedUserAgent>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BannedCountry>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<CheckEmail>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Poll>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Choice>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PollVote>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AccessMask>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumAccess>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageHistory>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReported>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReportedAudit>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PrivateMessage>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchForum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchTopic>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Attachment>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserGroup>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserForum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ReplaceWords>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<SpamWords>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Registry>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<EventLog>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BBCode>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Medal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<GroupMedal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserMedal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<IgnoreUser>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicReadTracking>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumReadTracking>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ReputationVote>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Tag>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicTag>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ProfileDefinition>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ProfileCustom>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<DeviceSubscription>());
    }

    private void MigrateAttachments()
    {
        // attempt to run the migration code...
        var messages = this.GetRepository<Attachment>().GetMessageAttachments().DistinctBy(x => x.ID).ToList();

        if (messages.NullOrEmpty())
        {
            return;
        }

        messages.ForEach(
            message =>
                {
                    var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == message.ID);

                    var updatedMessage = new StringBuilder();

                    updatedMessage.Append(message.MessageText);

                    attachments.ForEach(
                        attach =>
                            {
                                updatedMessage.AppendFormat(" [ATTACH]{0}[/Attach] ", attach.ID);

                                attach.UserID = message.UserID;

                                // Rename filename
                                if (attach.FileData == null)
                                {
                                    var webRootPath = this.Get<IWebHostEnvironment>().WebRootPath;

                                    var oldFilePath = Path.Combine(
                                        webRootPath,
                                        this.Get<BoardFolders>().Uploads,
                                        $"{attach.MessageID}.{attach.FileName}.yafupload");

                                    var newFilePath = Path.Combine(
                                        webRootPath,
                                        this.Get<BoardFolders>().Uploads,
                                        $"u{message.UserID}-{attach.ID}.{attach.FileName}.yafupload");

                                    try
                                    {
                                        File.Move(oldFilePath, newFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Get<ILogger<UpgradeService>>().Log(null, this, ex);
                                    }
                                }

                                attach.MessageID = 0;

                                this.GetRepository<Attachment>().Update(attach);
                            });

                    // Update Message
                    this.GetRepository<Message>().UpdateOnly(
                        () => new Message { MessageText = updatedMessage.ToString() },
                        m => m.ID == message.ID);
                });
    }
}