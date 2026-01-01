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

using System;

namespace YAF.Core.BaseModules;

using System.Collections.Generic;

/// <summary>
/// The standard module manager.
/// </summary>
/// <typeparam name="TModule">
/// The module type based on IBaseModule.
/// </typeparam>
public class StandardModuleManager<TModule> : IModuleManager<TModule>
    where TModule : IModuleDefinition
{
    /// <summary>
    /// The _modules.
    /// </summary>
    private readonly IList<TModule> modules;

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardModuleManager{TModule}"/> class.
    /// </summary>
    /// <param name="modules">
    /// The modules.
    /// </param>
    public StandardModuleManager(IEnumerable<TModule> modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        this.modules = [.. modules];
    }

    /// <summary>
    /// Get all instances of modules available.
    /// </summary>
    /// <param name="getInactive">
    /// The get Inactive.
    /// </param>
    /// <returns>
    /// </returns>
    public IEnumerable<TModule> GetAll(bool getInactive)
    {
        return !getInactive ? this.modules.Where(m => m.Active) : this.modules;
    }

    /// <summary>
    /// Get an instance of a module (based on its id).
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="getInactive">
    /// The get Inactive.
    /// </param>
    /// <returns>
    /// Instance of TModule or null if not found.
    /// </returns>
    public TModule GetBy(string id, bool getInactive)
    {
        ArgumentNullException.ThrowIfNull(id);

        return !getInactive
                   ? this.modules.SingleOrDefault(e => e.ModuleId.Equals(id) && e.Active)
                   : this.modules.SingleOrDefault(e => e.ModuleId.Equals(id));
    }

    /// <summary>
    /// Get an instance of a module (based on its id).
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// Instance of TModule or null if not found.
    /// </returns>
    public TModule GetBy(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return this.modules.SingleOrDefault(e => e.ModuleId.Equals(id));
    }
}