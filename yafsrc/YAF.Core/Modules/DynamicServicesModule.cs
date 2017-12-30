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

namespace YAF.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    public class DynamicServicesModule : BaseModule
    {
        #region Public Properties

        public override int SortOrder
        {
            get
            {
                return 500;
            }
        }

        #endregion

        #region Methods

        protected override void Load(ContainerBuilder containerBuilder)
        {
            // external first...
            this.RegisterDynamicServices(ExtensionAssemblies);

            // internal bindings next...
            this.RegisterDynamicServices(new[] { Assembly.GetExecutingAssembly() });
        }

        private void RegisterDynamicServices([NotNull] Assembly[] assemblies)
        {
            CodeContracts.VerifyNotNull(assemblies, "assemblies");

            var builder = new ContainerBuilder();

            var classes = assemblies.FindClassesWithAttribute<ExportServiceAttribute>();

            var exclude = new List<Type> { typeof(IDisposable), typeof(IHaveServiceLocator), typeof(IHaveLocalization) };

            foreach (var c in classes)
            {
                var exportAttribute = c.GetAttribute<ExportServiceAttribute>();

                if (exportAttribute == null)
                {
                    continue;
                }

                var built = builder.RegisterType(c).As(c);

                Type[] typesToRegister = null;

                if (exportAttribute.RegisterSpecifiedTypes != null &&
                    exportAttribute.RegisterSpecifiedTypes.Any())
                {
                    // only register types provided...
                    typesToRegister = exportAttribute.RegisterSpecifiedTypes;
                }
                else
                {
                    // register all associated interfaces including inherited interfaces
                    typesToRegister = c.GetInterfaces().Where(i => !exclude.Contains(i)).ToArray();
                }

                if (exportAttribute.Named.IsSet())
                {
                    // register types as "Named"
                    built = typesToRegister.Aggregate(built, (current, regType) => current.Named(exportAttribute.Named, regType));
                }
                else
                {
                    // register types "As"
                    built = typesToRegister.Aggregate(built, (current, regType) => current.As(regType));
                }

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

                    case ServiceLifetimeScope.InstancePerDependancy:
                        built.InstancePerDependency();
                        break;

                    case ServiceLifetimeScope.InstancePerContext:
                        built.InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
                        break;
                }
            }

            this.UpdateRegistry(builder);
        }

        #endregion
    }
}