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
    using YAF.Types.Interfaces.Events;

    /// <summary>
    /// The general module.
    /// </summary>
    public class GeneralModule : BaseModule
    {
        #region Methods

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            RegisterDataBindings(builder);
            RegisterGeneral(builder);
            RegisterEventBindings(builder);
            RegisterMembershipProviders(builder);
            RegisterModules(builder);
            RegisterPages(builder);
        }

        /// <summary>
        ///     The register data bindings.
        /// </summary>
        /// <param name="builder">
        ///     The builder.
        /// </param>
        private static void RegisterDataBindings(ContainerBuilder builder)
        {
            // data
            builder.RegisterType<DbAccessProvider>().As<IDbAccessProvider>().SingleInstance();
            builder.Register(c => c.Resolve<IComponentContext>().Resolve<IDbAccessProvider>().Instance).As<IDbAccess>()
                .InstancePerDependency().PreserveExistingDefaults();
            builder.Register((c, p) => DbProviderFactories.GetFactory(p.TypedAs<string>())).ExternallyOwned()
                .PreserveExistingDefaults();

            builder.RegisterType<DynamicDbFunction>().As<IDbFunction>().InstancePerDependency();

            // register generic IRepository handler, which can be easily overriden by more advanced repository handler
            builder.RegisterGeneric(typeof(BasicRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();

            // register filters -- even if they require BoardContext, they MUST BE REGISTERED UNDER GENERAL SCOPE
            // Do the BoardContext check inside the constructor and throw an exception if it's required.
            builder.RegisterType<StyleFilter>().As<IDbDataFilter>();
        }

        /// <summary>
        /// Register event bindings
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterEventBindings(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

            //// scan assemblies for events to wire up...
            // builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AsClosedTypesOf(typeof(IHandleEvent<>)).
            // AsImplementedInterfaces().InstancePerLifetimeScope();
        }

        /// <summary>
        /// The register basic bindings.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterGeneral(ContainerBuilder builder)
        {
            builder.Register(x => ExtensionAssemblies).Named<IList<Assembly>>("ExtensionAssemblies").SingleInstance();
            builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>()
                .InstancePerLifetimeScope();

            // BoardContext registration...
            builder.RegisterType<BoardContextPageProvider>().AsSelf().As<IReadOnlyProvider<BoardContext>>().SingleInstance()
                .PreserveExistingDefaults();
            builder.Register((k) => k.Resolve<IComponentContext>().Resolve<BoardContextPageProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            // Http Application Base
            builder.RegisterType<CurrentHttpApplicationStateBaseProvider>().SingleInstance().PreserveExistingDefaults();
            builder.Register(
                    k => k.Resolve<IComponentContext>().Resolve<CurrentHttpApplicationStateBaseProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            // Task Module
            builder.RegisterType<CurrentTaskModuleProvider>().SingleInstance().PreserveExistingDefaults();
            builder.Register(k => k.Resolve<IComponentContext>().Resolve<CurrentTaskModuleProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            builder.RegisterType<Nntp>().As<INewsreader>().InstancePerLifetimeScope().PreserveExistingDefaults();

            // cache bindings.
            builder.RegisterType<StaticLockObject>().As<IHaveLockObject>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<HttpRuntimeCache>().As<IDataCache>().SingleInstance().PreserveExistingDefaults();

            // Shared object store -- used for objects local only
            builder.RegisterType<HttpRuntimeCache>().As<IObjectStore>().SingleInstance().PreserveExistingDefaults();
        }

        /// <summary>
        /// Register membership providers
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterMembershipProviders(ContainerBuilder builder)
        {
            // membership
            builder.RegisterType<CurrentMembershipProvider>().AsSelf().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentMembershipProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            // roles
            builder.RegisterType<CurrentRoleProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentRoleProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();

            // profiles
            builder.RegisterType<CurrentProfileProvider>().AsSelf().InstancePerLifetimeScope()
                .PreserveExistingDefaults();
            builder.Register(x => x.Resolve<IComponentContext>().Resolve<CurrentProfileProvider>().Instance)
                .ExternallyOwned().PreserveExistingDefaults();
        }

        /// <summary>
        /// The register modules.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterModules(ContainerBuilder builder)
        {
            var assemblies = ExtensionAssemblies.Concat(new[] { Assembly.GetExecutingAssembly() }).ToArray();

            // forum modules...
            builder.RegisterAssemblyTypes(assemblies).AssignableTo<IBaseForumModule>().As<IBaseForumModule>()
                .InstancePerLifetimeScope();

            // editor modules...
            builder.RegisterAssemblyTypes(assemblies).AssignableTo<ForumEditor>().As<ForumEditor>()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// The register pages
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private static void RegisterPages(ContainerBuilder builder)
        {
            var assemblies = ExtensionAssemblies.Concat(new[] { Assembly.GetExecutingAssembly() }).ToArray();

            builder.RegisterAssemblyTypes(assemblies).AssignableTo<ILocatablePage>().AsImplementedInterfaces()
                .SingleInstance();
        }

        #endregion
    }
}