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
namespace YAF.Core
{
    #region Using

    using Autofac;
    using Autofac.Core.Lifetime;

    using YAF.Core.Modules;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Instance of the Global Container... yes, a God class. It's the best way to do it, though.
    /// </summary>
    public static class GlobalContainer
    {
        /// <summary>
        /// Initializes static members of the <see cref="GlobalContainer"/> class.
        /// </summary>
        static GlobalContainer()
        {
            var container = CreateContainer();

            using (var scope = container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
            {
                ServiceLocatorAccess.CurrentServiceProvider = scope.Resolve<IServiceLocator>();
            }

            Container = container;
        }

        #region Properties

        /// <summary>
        ///   Gets Container.
        /// </summary>
        public static IContainer Container { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Create Container
        /// </summary>
        /// <returns>
        /// The <see cref="IContainer"/>.
        /// </returns>
        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<BootstrapModule>();

            return builder.Build();
        }

        #endregion
    }
}