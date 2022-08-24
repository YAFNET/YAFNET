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

namespace YAF.Core.Services;

using System;
using System.IO;
using System.Text;
using System.Web.Configuration;

using YAF.Core.Data;
using YAF.Core.Model;
using YAF.Core.Services.Import;
using YAF.Core.Services.Migrations;
using YAF.Types.Constants;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using Constants = YAF.Types.Constants.Constants;

/// <summary>
///     The upgrade service.
/// </summary>
public class UpgradeService : IHaveServiceLocator
{
    /// <summary>
    ///     The BBCode extensions import xml file.
    /// </summary>
    private const string BbcodeImport = "Install/BBCodeExtensions.xml";

    /// <summary>
    ///     The Spam Words list import xml file.
    /// </summary>
    private const string SpamWordsImport = "Install/SpamWords.xml";

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
    /// Initialize Or Upgrade the Database
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool Upgrade()
    {
        this.CreateTablesIfNotExists();

        var prevVersion = this.GetRepository<Registry>().GetDbVersion();

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

        if (prevVersion < 80)
        {
            if (!Config.IsDotNetNuke)
            {
                this.MigrateConfig();
            }

            this.Get<V80_Migration>().MigrateDatabase(this.DbAccess);

            // Upgrade to ASPNET Identity
            if (!Config.IsDotNetNuke)
            {
                this.DbAccess.Information.IdentityUpgradeScripts.ForEach(this.ExecuteScript);
            }

            this.MigrateAttachments();

            // Delete old registry Settings
            this.GetRepository<Registry>().DeleteLegacy();

            // update default points from 0 to 1
            this.GetRepository<User>().UpdateOnly(() => new User { Points = 1 }, u => u.Points == 0);
        }

        if (prevVersion < 30)
        {
            this.Get<V30_Migration>().MigrateDatabase(this.DbAccess);
        }

        if (prevVersion < 42)
        {
            // un-html encode all topic subject names...
            this.GetRepository<Topic>().UnEncodeAllTopicsSubjects(HttpUtility.HtmlDecode);
        }

        if (prevVersion < 81)
        {
            this.Get<V81_Migration>().MigrateDatabase(this.DbAccess);
        }
            
        if (prevVersion < 82)
        {
            this.Get<V82_Migration>().MigrateDatabase(this.DbAccess);
        }

        if (prevVersion is 80 or 81 or 82 or 83)
        {
            var prefix = Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty;

            var registeredRole = this.GetRepository<AspNetRoles>().GetSingle(x => x.Name == $"{prefix}Registered");

            if (registeredRole != null)
            {
                registeredRole.Name = $"{prefix}Registered Users";

                this.GetRepository<AspNetRoles>().Update(registeredRole);
            }

            var users = this.Get<IAspNetUsersHelper>().GetAllUsers();

            users.ForEach(
                user =>
                    {
                        var roles = this.Get<IAspNetRolesHelper>().GetRolesForUser(user);

                        var yafUser = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

                        if (roles.NullOrEmpty())
                        {
                            // FIX initial roles (if any) for this user
                            this.Get<IAspNetRolesHelper>().SetupUserRoles(yafUser.BoardID, user);
                        }
                    });
        }

        if (prevVersion < 84)
        {
            this.Get<V84_Migration>().MigrateDatabase(this.DbAccess);
        }

        if (prevVersion < 85)
        {
            this.Get<V85_Migration>().MigrateDatabase(this.DbAccess);
        }

        if (prevVersion < 86)
        {
            this.Get<V86_Migration>().MigrateDatabase(this.DbAccess);
        }

        if (prevVersion < 87)
        {
            this.Get<V87_Migration>().MigrateDatabase(this.DbAccess);
        }

        this.AddOrUpdateExtensions();

        this.GetRepository<Registry>().Save("cdvversion", this.Get<BoardSettings>().CdvVersion++);

        this.Get<IDataCache>().Remove(Constants.Cache.Version);

        this.GetRepository<Registry>().Save("version", BoardInfo.AppVersion.ToString());
        this.GetRepository<Registry>().Save("versionname", BoardInfo.AppVersionName);

        this.Get<ILoggerService>().Info($"YAF.NET Upgraded to Version {BoardInfo.AppVersionName}");

        if (BoardContext.Current is {IsAdmin: true}){
            BoardContext.Current.Notify(
                $"YAF.NET Upgraded to Version {BoardInfo.AppVersionName}",
                MessageTypes.success);
        }

        return true;
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
    /// The execute script.
    /// </summary>
    /// <param name="scriptFile">
    /// The script file.
    /// </param>
    private void ExecuteScript([NotNull] string scriptFile)
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

        this.Get<IDbAccess>().SystemInitializeExecuteScripts(
            CommandTextHelpers.GetCommandTextReplaced(script),
            scriptFile,
            Config.SqlCommandTimeout);
    }

    /// <summary>
    /// Migrate Legacy Membership Settings
    /// </summary>
    private void MigrateConfig()
    {
        try
        {
            var membershipSection = (MembershipSection)WebConfigurationManager.GetSection("system.web/membership");

            var defaultProvider = membershipSection.DefaultProvider;
            var hashAlgorithmType = membershipSection.HashAlgorithmType;

            var providerSettings = membershipSection.Providers[defaultProvider];

            var hashHex = providerSettings.Parameters["hashHex"];
            var hashCase = providerSettings.Parameters["hashCase"];

            ConfigHelper config = new();

            if (hashAlgorithmType.IsSet())
            {
                config.WriteAppSetting("YAF.LegacyMembershipHashAlgorithmType", hashAlgorithmType);
            }

            if (hashHex.IsSet())
            {
                config.WriteAppSetting("YAF.LegacyMembershipHashHex", hashHex);
            }

            if (hashCase.IsSet())
            {
                config.WriteAppSetting("YAF.LegacyMembershipHashCase", hashCase);
            }
        }
        catch (Exception)
        {
            // Can Be ignored if settings have already been removed
        }
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
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<CheckEmail>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Poll>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Choice>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PollVote>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AccessMask>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumAccess>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageHistory>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReported>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<MessageReportedAudit>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PMessage>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchForum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<WatchTopic>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Attachment>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserGroup>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserForum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpServer>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpForum>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<NntpTopic>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PMessage>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Replace_Words>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Spam_Words>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Registry>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<EventLog>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<BBCode>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Medal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<GroupMedal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserMedal>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<IgnoreUser>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicReadTracking>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<UserPMessage>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ForumReadTracking>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ReputationVote>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<Tag>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<TopicTag>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ProfileDefinition>());
        this.DbAccess.Execute(db => db.Connection.CreateTableIfNotExists<ProfileCustom>());
    }

    /// <summary>
    /// Migrate old attachments from message to user attachments.
    /// </summary>
    private void MigrateAttachments()
    {
        // attempt to run the migration code...
        var messages = this.GetRepository<Attachment>().GetMessageAttachments().DistinctBy(x => x.ID).ToList();

        if (!messages.Any())
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
                                    var oldFilePath = this.Get<HttpRequestBase>().MapPath(
                                        $"{Path.Combine(BaseUrlBuilder.ServerFileRoot, this.Get<BoardFolders>().Uploads)}/{attach.MessageID}.{attach.FileName}.yafupload");

                                    var newFilePath = this.Get<HttpRequestBase>().MapPath(
                                        $"{Path.Combine(BaseUrlBuilder.ServerFileRoot, this.Get<BoardFolders>().Uploads)}/u{message.UserID}-{attach.ID}.{attach.FileName}.yafupload");

                                    try
                                    {
                                        File.Move(oldFilePath, newFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Get<ILoggerService>().Log(null, this, ex);
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