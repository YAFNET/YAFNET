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

namespace YAF.Core.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Autofac;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    using YAF.Configuration;
    using YAF.Core.BaseModules;
    using YAF.Core.BBCode;
    using YAF.Core.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Handlers;
    using YAF.Core.Helpers;
    using YAF.Core.Identity;
    using YAF.Core.Services;
    using YAF.Core.Services.Cache;
    using YAF.Core.Services.Migrations;
    using YAF.Core.Services.Startup;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models.Identity;

    using BBCode = YAF.Core.BBCode.BBCode;

    /// <summary>
    /// Registers all Service Modules
    /// </summary>
    public class ServicesModule : BaseModule
    {
        #region Methods

        /// <summary>
        /// Loads the specified container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            RegisterServices(builder);
            RegisterIdentityServices(builder);
            RegisterBoardContextServices(builder);
            RegisterStartupServices(builder);
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
            builder.RegisterType<Digest>().As<IDigest>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<DefaultUrlBuilder>().As<IUrlBuilder>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<BBCode>().As<IBBCode>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<FormatMessage>().As<IFormatMessage>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<DataBroker>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<Avatars>().As<IAvatars>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<Album>().As<IAlbum>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<Attachments>().As<IAttachment>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<Reputation>().As<IReputation>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<Resources>().As<IResources>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<IpInfoService>().As<IIpInfoService>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<CurrentBoardId>().As<IHaveBoardID>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<Search>().As<ISearch>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<LinkBuilder>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<Session>().As<ISession>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<BadWordReplace>().As<IBadWordReplace>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<SpamWordCheck>().As<ISpamWordCheck>().SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<Permissions>().As<IPermissions>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<DateTimeService>().As<IDateTimeService>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<UserIgnored>().As<IUserIgnored>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<Friends>().As<IFriends>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<LatestInformation>().As<ILatestInformation>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<SyndicationFeeds>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<InstallService>().AsSelf().PreserveExistingDefaults();
            builder.RegisterType<UpgradeService>().AsSelf().PreserveExistingDefaults();

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
            builder.RegisterType<CurrentBoardSettings>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentBoardSettings>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            builder.RegisterInstance(new BoardFolders()).AsSelf().SingleInstance();
            builder.RegisterInstance(new ControlSettings()).AsSelf().SingleInstance();

            // Migrations
            builder.RegisterType<V30_Migration>().AsSelf().PreserveExistingDefaults();
            builder.RegisterType<V80_Migration>().AsSelf().PreserveExistingDefaults();
            builder.RegisterType<V81_Migration>().AsSelf().PreserveExistingDefaults();
            builder.RegisterType<V82_Migration>().AsSelf().PreserveExistingDefaults();
            builder.RegisterType<V84_Migration>().AsSelf().PreserveExistingDefaults();

            // Caching
            //builder.RegisterType<MemoryCache>().As<IMemoryCache>().SingleInstance();
        }

        /// <summary>
        /// Register all Identity (Membership) Services
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterIdentityServices(ContainerBuilder builder)
        {
            // user manager
            var x = new IdentityDbContext();
            builder.Register(c => x);

            builder.RegisterType<UserStore>().As<IUserStore<AspNetUsers>>().InstancePerBoardContext();
            builder.RegisterType<AspNetUsersManager>().AsSelf().InstancePerBoardContext();

            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>();

            builder.RegisterType<RoleStore>().As<IRoleStore<AspNetRoles, string>>().InstancePerBoardContext();
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

            builder.RegisterType<StopWatch>().As<IStopWatch>().InstancePerBoardContext().PreserveExistingDefaults();

            builder.RegisterType<ThankYou>().As<IThankYou>().InstancePerBoardContext();
            builder.RegisterType<SpamCheck>().As<ISpamCheck>().InstancePerBoardContext();
        }

        /// <summary>
        /// The register startup services.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterStartupServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AssignableTo<IStartupService>()
                .As<IStartupService>().InstancePerLifetimeScope();

            builder.Register(
                x => x.Resolve<IComponentContext>().Resolve<IEnumerable<IStartupService>>()
                    .FirstOrDefault(t => t is StartupInitializeDb) as StartupInitializeDb).InstancePerLifetimeScope();
        }

        #endregion
    }
}