/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Autofac;
    using Autofac.Core;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseModule : IModule, IHaveComponentRegistry, IHaveSortOrder
    {
        #region Static Fields

        /// <summary>
        ///     Gets or sets ExtensionAssemblies.
        /// </summary>
        protected static readonly Assembly[] ExtensionAssemblies;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes the <see cref="BaseModule"/> class.
        /// </summary>
        static BaseModule()
        {
            ExtensionAssemblies =
                new YafModuleScanner().GetModules("YAF*.dll")
                    .Concat(
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                a =>
                                a.FullName.StartsWith("Autofac") && a.FullName.StartsWith("CookComputing.XmlRpcV2")
                                && a.FullName.StartsWith("FarsiLibrary")
                                && a.FullName.StartsWith("Intelligencia.UrlRewriter")
                                && a.FullName.StartsWith("nStuff.UpdateControls")
                                && a.FullName.StartsWith("Omu.ValueInjecter") && a.FullName.StartsWith("ServiceStack.")))
                    .Except(new[] { Assembly.GetExecutingAssembly() })
                    .Where(a => !a.IsDynamic)
                    .Distinct()
                    .OrderByDescending(x => x.GetAssemblySortOrder())
                    .ToArray();
#if DEBUG
            foreach (var s in ExtensionAssemblies)
            {
                Debug.WriteLine("Extension Assembly: {0}", s);
            }
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ComponentRegistry.
        /// </summary>
        public IComponentRegistry ComponentRegistry { get; set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public virtual int SortOrder
        {
            get
            {
                return 1000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Apply the module to the component registry.
        /// </summary>
        /// <param name="componentRegistry">
        ///     Component registry to apply configuration to.
        /// </param>
        public virtual void Configure([NotNull] IComponentRegistry componentRegistry)
        {
            CodeContracts.VerifyNotNull(componentRegistry, "componentRegistry");

            this.ComponentRegistry = componentRegistry;

            var builder = new ContainerBuilder();
            this.Load(builder);
            this.UpdateRegistry(builder);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected abstract void Load(ContainerBuilder builder);

        /// <summary>
        /// Registers the base modules.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="exclude">The exclude.</param>
        protected virtual void RegisterBaseModules<TModule>(Assembly[] assemblies, IEnumerable<Type> exclude = null)
            where TModule : IModule
        {
            var moduleFinder = new ContainerBuilder();

            var excludeList = exclude.IfNullEmpty().ToList();

            moduleFinder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(TModule).IsAssignableFrom(t) && !excludeList.Contains(t))
                .As<IModule>();

            var builder = new ContainerBuilder();

            using (var moduleContainer = moduleFinder.Build())
            {
                var modules = moduleContainer.Resolve<IEnumerable<IModule>>()
                    .ByOptionalSortOrder()
                    .ToList();

                foreach (var module in modules)
                {
                    builder.RegisterModule(module);
                }
            }

            this.UpdateRegistry(builder);
        }

        #endregion
    }
}