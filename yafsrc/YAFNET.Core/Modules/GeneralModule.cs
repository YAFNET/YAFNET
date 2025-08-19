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

using System.Collections.Generic;
using System.Reflection;

using YAF.Core.Data;
using YAF.Core.Events;
using YAF.Core.Services.Cache;
using YAF.Types.Interfaces.Events;

/// <summary>
/// The general module.
/// </summary>
public class GeneralModule : BaseModule
{
    /// <summary>
    /// The load.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    override protected void Load(ContainerBuilder builder)
    {
        RegisterDataBindings(builder);
        RegisterGeneral(builder);
        RegisterEventBindings(builder);
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

        // register generic IRepository handler, which can be easily overriden by more advanced repository handler
        builder.RegisterGeneric(typeof(BasicRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();

        // register filters -- even if they require BoardContext, they MUST BE REGISTERED UNDER GENERAL SCOPE
        // Do the BoardContext check inside the constructor and throw an exception if it's required.
        //builder.RegisterType<StyleFilter>().As<IDbDataFilter>();
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
        builder.RegisterType<ServiceLocatorAsyncEventRaiser>().As<IRaiseEventAsync>().InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(FireEventAsync<>)).As(typeof(IFireEventAsync<>)).InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

        //// scan assemblies for events to wire up...
        builder.RegisterAssemblyTypes(ExtensionAssemblies).AsClosedTypesOf(typeof(IHandleEvent<>)).
         AsImplementedInterfaces().InstancePerLifetimeScope();

        //// scan assemblies for events to wire up...
        builder.RegisterAssemblyTypes(ExtensionAssemblies).AsClosedTypesOf(typeof(IHandleEventAsync<>)).
            AsImplementedInterfaces().InstancePerLifetimeScope();
    }

    /// <summary>
    /// The register basic bindings.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterGeneral(ContainerBuilder builder)
    {
        builder.Register(_ => ExtensionAssemblies).Named<IList<Assembly>>("ExtensionAssemblies").SingleInstance();
        builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>()
            .InstancePerLifetimeScope();

        // BoardContext registration...
        builder.RegisterType<BoardContextPageProvider>().AsSelf().As<IReadOnlyProvider<BoardContext>>().SingleInstance()
            .PreserveExistingDefaults();
        builder.Register((k) => k.Resolve<IComponentContext>().Resolve<BoardContextPageProvider>().Instance)
            .ExternallyOwned().PreserveExistingDefaults();

        // cache bindings.
        builder.RegisterType<StaticLockObject>().As<IHaveLockObject>().SingleInstance().PreserveExistingDefaults();
        builder.RegisterType<HttpRuntimeCache>().As<IDataCache>().SingleInstance().PreserveExistingDefaults();

        // Shared object store -- used for objects local only
        builder.RegisterType<HttpRuntimeCache>().As<IObjectStore>().SingleInstance().PreserveExistingDefaults();
    }

    /// <summary>
    /// The register modules.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterModules(ContainerBuilder builder)
    {
        var assemblies = ExtensionAssemblies.Concat([Assembly.GetExecutingAssembly()]).ToArray();

        // forum modules...
        builder.RegisterAssemblyTypes(assemblies).AssignableTo<IBaseForumModule>().As<IBaseForumModule>()
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
        var assemblies = ExtensionAssemblies.Concat([Assembly.GetExecutingAssembly()]).ToArray();

        builder.RegisterAssemblyTypes(assemblies).AssignableTo<ILocatablePage>().AsImplementedInterfaces()
            .SingleInstance();
    }
}