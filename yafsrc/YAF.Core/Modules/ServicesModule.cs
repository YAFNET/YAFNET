/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using YAF.Classes;
    using YAF.Core.BBCode;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Services.Cache;
    using YAF.Core.Services.Startup;
    using YAF.Types;
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
            this.RegisterServices();
            this.RegisterStartupServices();
        }

        /// <summary>
        ///     The register services.
        /// </summary>
        private void RegisterServices()
        {
            var builder = new ContainerBuilder();

            // optional defaults.
            builder.RegisterType<YafSendMail>().As<ISendMail>().SingleInstance().PreserveExistingDefaults();

            if (Config.IsDotNetNuke)
            {
                builder.RegisterType<YafActivityStream>()
                    .As<IActivityStream>()
                    .SingleInstance()
                    .PreserveExistingDefaults();
            }

            builder.RegisterType<YafSendNotification>().As<ISendNotification>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<YafDigest>().As<IDigest>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<DefaultUrlBuilder>().As<IUrlBuilder>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafBBCode>().As<IBBCode>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafFormatMessage>().As<IFormatMessage>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafDbBroker>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafAvatars>().As<IAvatars>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<CurrentBoardId>().As<IHaveBoardID>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<YafReadTrackCurrentUser>().As<IReadTrackCurrentUser>().InstancePerYafContext().PreserveExistingDefaults();

            builder.RegisterType<YafSession>().As<IYafSession>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafBadWordReplace>().As<IBadWordReplace>().SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<YafPermissions>().As<IPermissions>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafDateTime>().As<IDateTime>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafUserIgnored>().As<IUserIgnored>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<YafBuddy>().As<IBuddy>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<InstallUpgradeService>().AsSelf().PreserveExistingDefaults();

            // builder.RegisterType<RewriteUrlBuilder>().Named<IUrlBuilder>("rewriter").InstancePerLifetimeScope();
            builder.RegisterType<YafStopWatch>()
                .As<IStopWatch>()
                .InstancePerMatchingLifetimeScope(YafLifetimeScope.Context)
                .PreserveExistingDefaults();

            // localization registration...
            builder.RegisterType<LocalizationProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<LocalizationProvider>().Localization).PreserveExistingDefaults();

            // theme registration...
            builder.RegisterType<ThemeProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<ThemeProvider>().Theme).PreserveExistingDefaults();

            // replace rules registration...
            builder.RegisterType<ProcessReplaceRulesProvider>()
                .AsSelf()
                .As<IReadOnlyProvider<IProcessReplaceRules>>()
                .InstancePerLifetimeScope()
                .PreserveExistingDefaults();

            builder.Register((k, p) => k.Resolve<IComponentContext>().Resolve<ProcessReplaceRulesProvider>(p).Instance)
                .InstancePerLifetimeScope()
                .PreserveExistingDefaults();

            // module resolution bindings...
            builder.RegisterGeneric(typeof(StandardModuleManager<>)).As(typeof(IModuleManager<>)).InstancePerLifetimeScope();

            // background emailing...
            builder.RegisterType<YafSendMailThreaded>().As<ISendMailThreaded>().SingleInstance().PreserveExistingDefaults();

            // style transformation...
            builder.RegisterType<StyleTransform>().As<IStyleTransform>().InstancePerYafContext().PreserveExistingDefaults();

            // board settings...
            builder.RegisterType<CurrentBoardSettings>().AsSelf().InstancePerYafContext().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentBoardSettings>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            // favorite topic is based on YafContext
            builder.RegisterType<YafFavoriteTopic>().As<IFavoriteTopic>().InstancePerYafContext().PreserveExistingDefaults();

            this.UpdateRegistry(builder);
        }

        /// <summary>
        ///     The register services.
        /// </summary>
        private void RegisterStartupServices()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AssignableTo<IStartupService>()
                .As<IStartupService>()
                .InstancePerLifetimeScope();

            builder.Register(
                x => x.Resolve<IComponentContext>()
                    .Resolve<IEnumerable<IStartupService>>()
                    .FirstOrDefault(t => t is StartupInitializeDb) as
                    StartupInitializeDb)
                .InstancePerLifetimeScope();

            this.UpdateRegistry(builder);
        }

        #endregion
    }
}