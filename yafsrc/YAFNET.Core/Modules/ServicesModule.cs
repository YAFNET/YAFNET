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

namespace YAF.Core.Modules;

using System.Reflection;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using YAF.Core.BaseModules;
using YAF.Core.BBCode;
using YAF.Core.Handlers;
using YAF.Core.Hubs;
using YAF.Core.Identity;
using YAF.Core.Services;
using YAF.Core.Services.Cache;
using YAF.Core.Services.Import;

/// <summary>
/// Registers all Service Modules
/// </summary>
public class ServicesModule : BaseModule
{
    /// <summary>
    /// Loads the specified container builder.
    /// </summary>
    /// <param name="builder">The container builder.</param>
    override protected void Load(ContainerBuilder builder)
    {
        RegisterServices(builder);
        RegisterMigrationServices(builder);
        RegisterIdentityServices(builder);
        RegisterBoardContextServices(builder);
        RegisterHubs(builder);
    }

    /// <summary>
    /// The register services.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterServices(ContainerBuilder builder)
    {
        // optional defaults.
        builder.RegisterType<MailService>().As<IMailService>().SingleInstance().PreserveExistingDefaults();
        builder.RegisterType<ActivityStream>().As<IActivityStream>().SingleInstance().PreserveExistingDefaults();
        builder.RegisterType<SendNotification>().As<ISendNotification>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<DigestService>().As<IDigestService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<BoardInfo>().AsSelf().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<TwoFactorAuthService>().As<ITwoFactorAuthService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();

        builder.RegisterType<BBCodeService>().As<IBBCodeService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<FormatMessage>().As<IFormatMessage>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<DataBroker>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<Avatars>().As<IAvatars>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<Album>().As<IAlbum>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<Reputation>().As<IReputation>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<UserMedalService>().As<IUserMedalService>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<IpInfoService>().As<IIpInfoService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<GeoIpCountryService>().As<IGeoIpCountryService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<CurrentBoardId>().As<IHaveBoardID>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<Search>().As<ISearch>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<LinkBuilder>().As<ILinkBuilder>().InstancePerLifetimeScope().PreserveExistingDefaults();

        builder.RegisterType<SessionService>().As<ISessionService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();

        builder.RegisterType<BadWordReplace>().As<IBadWordReplace>().SingleInstance().PreserveExistingDefaults();
        builder.RegisterType<SpamWordCheck>().As<ISpamWordCheck>().SingleInstance().PreserveExistingDefaults();

        builder.RegisterType<Permissions>().As<IPermissions>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<DateTimeService>().As<IDateTimeService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<UserIgnored>().As<IUserIgnored>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<Friends>().As<IFriends>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<LatestInformationService>().As<ILatestInformationService>().InstancePerLifetimeScope()
            .PreserveExistingDefaults();
        builder.RegisterType<SyndicationFeeds>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();

        builder.RegisterType<PollService>().AsSelf().InstancePerLifetimeScope()
            .PreserveExistingDefaults();

        builder.RegisterType<InstallService>().AsSelf().PreserveExistingDefaults();
        builder.RegisterType<UpgradeService>().AsSelf().PreserveExistingDefaults();

        builder.RegisterType<DataImporter>().As<IDataImporter>().PreserveExistingDefaults();

        // localization registration...
        builder.RegisterType<LocalizationProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<LocalizationProvider>().Localization)
            .PreserveExistingDefaults();

        // theme registration...
        builder.RegisterType<ThemeProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<ThemeProvider>().Theme)
            .PreserveExistingDefaults();

        // replace rules registration...
        builder.RegisterType<ProcessReplaceRulesProvider>().AsSelf().As<IReadOnlyProvider<IProcessReplaceRules>>()
            .InstancePerLifetimeScope().PreserveExistingDefaults();

        builder.Register((k, p) => k.Resolve<IComponentContext>().Resolve<ProcessReplaceRulesProvider>(p).Instance)
            .InstancePerLifetimeScope().PreserveExistingDefaults();

        // module resolution bindings...
        builder.RegisterGeneric(typeof(StandardModuleManager<>)).As(typeof(IModuleManager<>))
            .InstancePerLifetimeScope();

        // board settings...
        builder.RegisterType<BoardSettingsService>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<CurrentBoardSettings>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentBoardSettings>().Instance)
            .ExternallyOwned().PreserveExistingDefaults();

        builder.RegisterInstance(new BoardFolders()).AsSelf().SingleInstance();
        builder.RegisterInstance(new ControlSettings()).AsSelf().SingleInstance();

        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
        builder.RegisterType<NotificationClient>().AsSelf().SingleInstance();
    }

    /// <summary>
    /// Register Migration Services
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterMigrationServices(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AssignableTo<IRepositoryMigration>().AsSelf()
            .PreserveExistingDefaults();
    }

    /// <summary>
    /// Register all Identity (Membership) Services
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterIdentityServices(ContainerBuilder builder)
    {
        builder.RegisterType<UserStore>().As<IUserStore<AspNetUsers>>();
        builder.RegisterType<AspNetUsersManager>().AsSelf().InstancePerBoardContext();

        builder.RegisterType<RoleStore>().As<IRoleStore<AspNetRoles>>();
        builder.RegisterType<AspNetRoleManager>().As<IAspNetRoleManager>().InstancePerBoardContext();

        builder.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>().InstancePerBoardContext();
        builder.RegisterType<AspNetRolesHelper>().As<IAspNetRolesHelper>().InstancePerBoardContext();
    }

    /// <summary>
    /// Register all Services that are based on BoardContext
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterBoardContextServices(ContainerBuilder builder)
    {
        builder.RegisterType<StyleTransform>().As<IStyleTransform>().InstancePerBoardContext()
            .PreserveExistingDefaults();
        builder.RegisterType<ReadTrackCurrentUser>().As<IReadTrackCurrentUser>().InstancePerBoardContext()
            .PreserveExistingDefaults();
        builder.RegisterType<StopWatch>().As<IStopWatch>().InstancePerLifetimeScope().PreserveExistingDefaults();
        builder.RegisterType<ThankYou>().As<IThankYou>().InstancePerBoardContext();
        builder.RegisterType<SpamCheck>().As<ISpamCheck>().InstancePerBoardContext();
    }

    /// <summary>
    /// Registers all the SignalR Hubs.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private static void RegisterHubs(ContainerBuilder builder)
    {
        builder.RegisterType<NotificationHub>().ExternallyOwned();
        builder.RegisterType<ChatHub>().ExternallyOwned();
    }
}