/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Reflection;

using YAF.Types.Attributes;

/// <summary>
///     The assembly extensions.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// The find classes with attribute.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static IEnumerable<Type> FindClassesWithAttribute<T>(this IEnumerable<Assembly> assemblies)
        where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        var moduleClassTypes = new List<Type>();
        var attributeType = typeof(T);

        // get classes...
        assemblies.Select(
            a => a.GetExportedTypes()
                .Where(t => !t.IsAbstract && t.GetCustomAttributes(attributeType, true).Length != 0)
                .ToList()).ForEach(types => moduleClassTypes.AddRange(types));

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
    public static int GetAssemblySortOrder(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attribute = assembly.GetCustomAttributes(typeof(AssemblyModuleSortOrderAttribute), true)
            .OfType<AssemblyModuleSortOrderAttribute>().ToList();

        return attribute.HasItems() ? attribute[0].SortOrder : 9999;
    }
}