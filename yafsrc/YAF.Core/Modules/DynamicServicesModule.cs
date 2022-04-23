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

namespace YAF.Core.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Core.Lifetime;

using YAF.Core.Extensions;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;

/// <summary>
/// The dynamic services module.
/// </summary>
public class DynamicServicesModule : BaseModule
{
    #region Public Properties

    /// <summary>
    /// The sort order.
    /// </summary>
    public override int SortOrder => 500;

    #endregion

    #region Methods

    /// <summary>
    /// The load.
    /// </summary>
    /// <param name="builder">
    /// The container builder.
    /// </param>
    protected override void Load(ContainerBuilder builder)
    {
        // external first...
        RegisterDynamicServices(builder, ExtensionAssemblies);

        // internal bindings next...
        RegisterDynamicServices(builder, new[] { Assembly.GetExecutingAssembly() });
    }

    /// <summary>
    /// The register dynamic services.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    private static void RegisterDynamicServices(ContainerBuilder builder, [NotNull] Assembly[] assemblies)
    {
        CodeContracts.VerifyNotNull(assemblies);

        var classes = assemblies.FindClassesWithAttribute<ExportServiceAttribute>();

        var exclude = new List<Type>
                          {
                              typeof(IDisposable), typeof(IHaveServiceLocator), typeof(IHaveLocalization)
                          };

        classes.ForEach(
            c =>
                {
                    var exportAttribute = c.GetAttribute<ExportServiceAttribute>();

                    if (exportAttribute == null)
                    {
                        return;
                    }

                    var built = builder.RegisterType(c).As(c);

                    Type[] typesToRegister;

                    if (exportAttribute.RegisterSpecifiedTypes != null
                        && exportAttribute.RegisterSpecifiedTypes.Any())
                    {
                        // only register types provided...
                        typesToRegister = exportAttribute.RegisterSpecifiedTypes;
                    }
                    else
                    {
                        // register all associated interfaces including inherited interfaces
                        typesToRegister = c.GetInterfaces().Where(i => !exclude.Contains(i)).ToArray();
                    }

                    built = exportAttribute.Named.IsSet()
                                ? typesToRegister.Aggregate(
                                    built,
                                    (current, regType) => current.Named(exportAttribute.Named, regType))
                                : typesToRegister.Aggregate(built, (current, regType) => current.As(regType));

                    switch (exportAttribute.ServiceLifetimeScope)
                    {
                        case ServiceLifetimeScope.Singleton:
                            built.SingleInstance();
                            break;

                        case ServiceLifetimeScope.Transient:
                            built.ExternallyOwned();
                            break;

                        case ServiceLifetimeScope.OwnedByContainer:
                            built.OwnedByLifetimeScope();
                            break;

                        case ServiceLifetimeScope.InstancePerScope:
                            built.InstancePerLifetimeScope();
                            break;

                        case ServiceLifetimeScope.InstancePerDependency:
                            built.InstancePerDependency();
                            break;

                        case ServiceLifetimeScope.InstancePerContext:
                            built.InstancePerMatchingLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
                            break;
                    }
                });
    }

    #endregion
}