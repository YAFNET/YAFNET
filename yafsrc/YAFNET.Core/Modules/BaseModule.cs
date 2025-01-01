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

using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Reflection;

using Autofac.Core;

using YAF.Core.BaseModules;

/// <summary>
/// The base module.
/// </summary>
public abstract class BaseModule : Autofac.Module, IHaveSortOrder
{
    /// <summary>
    ///     Gets or sets ExtensionAssemblies.
    /// </summary>
    readonly static protected Assembly[] ExtensionAssemblies;

    /// <summary>
    /// Initializes static members of the <see cref="BaseModule"/> class.
    /// </summary>
    static BaseModule()
    {
        ExtensionAssemblies =
        [
            .. ModuleScanner.GetModules("YAF*.dll")
                        .Concat(
                            AppDomain.CurrentDomain.GetAssemblies().Where(
                                a => a.FullName.StartsWith("Autofac") && a.FullName.StartsWith("FarsiLibrary")
                                                                      && a.FullName.StartsWith("ServiceStack.")))
                        .Except([Assembly.GetExecutingAssembly()]).Where(a => !a.IsDynamic).Distinct()
                        .OrderByDescending(x => x.GetAssemblySortOrder())
        ];
#if DEBUG

        ExtensionAssemblies.ForEach(s => Debug.WriteLine("Extension Assembly: {0}", s));

#endif
    }

    /// <summary>
    /// Gets the sort order.
    /// </summary>
    /// <value>
    /// The sort order.
    /// </value>
    public virtual int SortOrder => 1000;

    /// <summary>
    /// Registers the base modules.
    /// </summary>
    /// <typeparam name="TModule">
    /// The type of the module.
    /// </typeparam>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="exclude">
    /// The exclude modules.
    /// </param>
    protected virtual void RegisterBaseModules<TModule>(
        ContainerBuilder builder,
        Assembly[] assemblies,
        IEnumerable<Type> exclude = null)
        where TModule : IModule
    {
        var moduleFinder = new ContainerBuilder();

        var excludeList = exclude.IfNullEmpty().ToList();

        moduleFinder.RegisterAssemblyTypes(assemblies)
            .Where(t => typeof(TModule).IsAssignableFrom(t) && !excludeList.Contains(t)).As<IModule>();

        using var moduleContainer = moduleFinder.Build();
        var modules = moduleContainer.Resolve<IEnumerable<IModule>>().ByOptionalSortOrder().ToList();

        modules.ForEach(module => builder.RegisterModule(module));
    }
}