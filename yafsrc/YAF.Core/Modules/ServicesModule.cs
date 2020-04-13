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

namespace YAF.Core.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using YAF.Configuration;
    using YAF.Core.BaseModules;
    using YAF.Core.BBCode;
    using YAF.Core.Extensions;
    using YAF.Core.Handlers;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Core.Services.Cache;
    using YAF.Core.Services.Startup;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// Registers all Service Modules
    /// </summary>
    public class ServicesModule : BaseModule
    {
        #region Methods

        /// <summary>
        /// Loads the specified container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        protected override void Load(ContainerBuilder containerBuilder)
        {
            RegisterServices(containerBuilder);
            RegisterStartupServices(containerBuilder);
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
            builder.RegisterType<SendMail>().As<ISendMail>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<ActivityStream>().As<IActivityStream>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<SendNotification>().As<ISendNotification>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<Search>().As<ISearch>().InstancePerLifetimeScope().PreserveExistingDefaults();
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

            builder.RegisterType<ReadTrackCurrentUser>().As<IReadTrackCurrentUser>().InstancePerBoardContext()
                .PreserveExistingDefaults();

            builder.RegisterType<Session>().As<ISession>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<BadWordReplace>().As<IBadWordReplace>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<SpamWordCheck>().As<ISpamWordCheck>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<SpamCheck>().As<ISpamCheck>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<ThankYou>().As<IThankYou>().SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<Permissions>().As<IPermissions>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<DateTime>().As<IDateTime>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<UserIgnored>().As<IUserIgnored>().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.RegisterType<Buddys>().As<IBuddy>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<InstallUpgradeService>().AsSelf().PreserveExistingDefaults();

            // builder.RegisterType<RewriteUrlBuilder>().Named<IUrlBuilder>("rewriter").InstancePerLifetimeScope();
            builder.RegisterType<StopWatch>().As<IStopWatch>()
                .InstancePerMatchingLifetimeScope(LifetimeScope.Context).PreserveExistingDefaults();

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

            // style transformation...
            builder.RegisterType<StyleTransform>().As<IStyleTransform>().InstancePerBoardContext()
                .PreserveExistingDefaults();

            // board settings...
            builder.RegisterType<CurrentBoardSettings>().AsSelf().InstancePerBoardContext().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentBoardSettings>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            // favorite topic is based on BoardContext
            builder.RegisterType<FavoriteTopic>().As<IFavoriteTopic>().InstancePerBoardContext()
                .PreserveExistingDefaults();
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
                             .FirstOrDefault(t => t is StartupInitializeDb) as StartupInitializeDb)
                .InstancePerLifetimeScope();
        }

        #endregion
    }
}