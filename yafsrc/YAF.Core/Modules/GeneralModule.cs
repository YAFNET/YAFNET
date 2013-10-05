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
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using YAF.Core.Data;
    using YAF.Core.Data.Filters;
    using YAF.Core.Nntp;
    using YAF.Core.Services.Cache;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    public class GeneralModule : BaseModule
    {
        #region Methods

        protected override void Load(ContainerBuilder builder)
        {
            this.RegisterDataBindings(builder);
            this.RegisterGeneral(builder);
            this.RegisterEventBindings(builder);
            this.RegisterMembershipProviders(builder);
            this.RegisterModules(builder);
            this.RegisterPages(builder);
        }

        /// <summary>
        ///     The register data bindings.
        /// </summary>
        /// <param name="builder">
        ///     The builder.
        /// </param>
        private void RegisterDataBindings(ContainerBuilder builder)
        {
            // data
            builder.RegisterType<DbAccessProvider>().As<IDbAccessProvider>().SingleInstance();
            builder.Register(c => c.Resolve<IComponentContext>().Resolve<IDbAccessProvider>().Instance)
                .As<IDbAccess>()
                .InstancePerDependency()
                .PreserveExistingDefaults();
            builder.Register((c, p) => DbProviderFactories.GetFactory(p.TypedAs<string>())).ExternallyOwned().PreserveExistingDefaults();

            builder.RegisterType<DynamicDbFunction>().As<IDbFunction>().InstancePerDependency();

            // register generic IRepository handler, which can be easily overriden by more advanced repository handler
            builder.RegisterGeneric(typeof(BasicRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();

            // register filters -- even if they require YafContext, they MUST BE REGISTERED UNDER GENERAL SCOPE
            // Do the YafContext check inside the constructor and throw an exception if it's required.
            builder.RegisterType<StyleFilter>().As<IDbDataFilter>();
        }

        /// <summary>
        ///     Register event bindings
        /// </summary>
        private void RegisterEventBindings(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

            //// scan assemblies for events to wire up...
            // builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AsClosedTypesOf(typeof(IHandleEvent<>)).
            // AsImplementedInterfaces().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     The register basic bindings.
        /// </summary>
        private void RegisterGeneral(ContainerBuilder builder)
        {
            builder.Register(x => ExtensionAssemblies).Named<IList<Assembly>>("ExtensionAssemblies").SingleInstance();
            builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>().InstancePerLifetimeScope();

            // YafContext registration...
            builder.RegisterType<YafContextPageProvider>().AsSelf().As<IReadOnlyProvider<YafContext>>().SingleInstance().PreserveExistingDefaults();
            builder.Register((k) => k.Resolve<IComponentContext>().Resolve<YafContextPageProvider>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            // Http Application Base
            builder.RegisterType<CurrentHttpApplicationStateBaseProvider>().SingleInstance().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentHttpApplicationStateBaseProvider>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            // Task Module
            builder.RegisterType<CurrentTaskModuleProvider>().SingleInstance().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentTaskModuleProvider>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            builder.RegisterType<YafNntp>().As<INewsreader>().InstancePerLifetimeScope().PreserveExistingDefaults();

            // cache bindings.
            builder.RegisterType<StaticLockObject>().As<IHaveLockObject>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<HttpRuntimeCache>().As<IDataCache>().SingleInstance().PreserveExistingDefaults();

            // Shared object store -- used for objects local only
            builder.RegisterType<HttpRuntimeCache>().As<IObjectStore>().SingleInstance().PreserveExistingDefaults();
        }

        /// <summary>
        ///     Register membership providers
        /// </summary>
        private void RegisterMembershipProviders(ContainerBuilder builder)
        {
            // membership
            builder.RegisterType<CurrentMembershipProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentMembershipProvider>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            // roles
            builder.RegisterType<CurrentRoleProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentRoleProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

            // profiles
            builder.RegisterType<CurrentProfileProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentProfileProvider>().Instance)
                .ExternallyOwned()
                .PreserveExistingDefaults();
        }

        /// <summary>
        ///     The register modules.
        /// </summary>
        private void RegisterModules(ContainerBuilder builder)
        {
            var assemblies = ExtensionAssemblies.Concat(new[] { Assembly.GetExecutingAssembly() }).ToArray();

            // forum modules...
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IBaseForumModule>()
                .As<IBaseForumModule>()
                .InstancePerLifetimeScope();

            // editor modules...
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<ForumEditor>()
                .As<ForumEditor>()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        ///     The register pages
        /// </summary>
        private void RegisterPages(ContainerBuilder builder)
        {
            var assemblies = ExtensionAssemblies.Concat(new[] { Assembly.GetExecutingAssembly() }).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<ILocatablePage>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        #endregion
    }
}