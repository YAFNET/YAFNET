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