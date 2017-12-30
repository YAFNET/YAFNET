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
    using System.Linq;
    using System.Reflection;

    using YAF.Types;
    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    ///     The assembly extensions.
    /// </summary>
    public static class AssemblyExtensions
    {
        #region Public Methods and Operators

        [NotNull]
        public static IEnumerable<Type> FindClassesWithAttribute<T>([NotNull] this IEnumerable<Assembly> assemblies)
            where T : Attribute
        {
            CodeContracts.VerifyNotNull(assemblies, "assemblies");

            var moduleClassTypes = new List<Type>();
            var attributeType = typeof(T);

            // get classes...
            foreach (
                var types in
                    assemblies.Select(
                        a =>
                        a.GetExportedTypes().Where(t => !t.IsAbstract && t.GetCustomAttributes(attributeType, true).Any()).ToList()))
            {
                moduleClassTypes.AddRange(types);
            }

            return moduleClassTypes.Distinct();
        }

        [NotNull]
        public static IEnumerable<Type> FindModules<T>([NotNull] this IEnumerable<Assembly> assemblies)
        {
            CodeContracts.VerifyNotNull(assemblies, "assemblies");

            var moduleClassTypes = new List<Type>();
            var implementedInterfaceType = typeof(T);

            // get classes...
            foreach (
                var types in
                    assemblies.Select(
                        a =>
                        a.GetExportedTypes().Where(t => !t.IsAbstract).ToList()))
            {
                moduleClassTypes.AddRange(types.Where(implementedInterfaceType.IsAssignableFrom));
            }

            return moduleClassTypes.Distinct();
        }

        /// <summary>
        ///     The find modules.
        /// </summary>
        /// <param name="assemblies">
        ///     The assemblies.
        /// </param>
        /// <param name="namespaceName">
        ///     The module namespace.
        /// </param>
        /// <param name="implementedInterfaceName">
        ///     The module base interface.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static IEnumerable<Type> FindModules(
            [NotNull] this IEnumerable<Assembly> assemblies,
            [NotNull] string namespaceName,
            [NotNull] string implementedInterfaceName)
        {
            CodeContracts.VerifyNotNull(assemblies, "assemblies");
            CodeContracts.VerifyNotNull(namespaceName, "namespaceName");
            CodeContracts.VerifyNotNull(implementedInterfaceName, "implementedInterfaceName");

            var moduleClassTypes = new List<Type>();
            var implementedInterfaceType = Type.GetType(implementedInterfaceName);

            // get classes...
            foreach (var types in assemblies.OfType<Assembly>().Select(
                        a =>
                        a.GetExportedTypes().Where(t => t.Namespace != null && !t.IsAbstract && t.Namespace.Equals(namespaceName))
                            .ToList()))
            {
                moduleClassTypes.AddRange(types.Where(implementedInterfaceType.IsAssignableFrom));
            }

            return moduleClassTypes.Distinct();
        }

        /// <summary>
        ///     The get assembly sort order.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The get assembly sort order.
        /// </returns>
        public static int GetAssemblySortOrder([NotNull] this Assembly assembly)
        {
            CodeContracts.VerifyNotNull(assembly, "assembly");

            var attribute = assembly.GetCustomAttributes(typeof(AssemblyModuleSortOrder), true).OfType<AssemblyModuleSortOrder>();

            return attribute.Any() ? attribute.First().SortOrder : 9999;
        }

        #endregion
    }
}