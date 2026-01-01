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

namespace YAF.Core.BBCode;

using System.Collections.Generic;

/// <summary>
/// Gets an instance of replace rules and uses
///   caching if possible.
/// </summary>
public class ProcessReplaceRulesProvider : IHaveServiceLocator, IReadOnlyProvider<IProcessReplaceRules>
{
    /// <summary>
    ///   The inject services.
    /// </summary>
    private readonly IInjectServices injectServices;

    /// <summary>
    /// The object store.
    /// </summary>
    private readonly IObjectStore objectStore;

    /// <summary>
    ///   The unique flags.
    /// </summary>
    private readonly IEnumerable<bool> uniqueFlags;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessReplaceRulesProvider"/> class.
    /// </summary>
    /// <param name="objectStore">
    /// The object Store.
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="injectServices">
    /// The inject services.
    /// </param>
    /// <param name="uniqueFlags">
    /// The unique Flags.
    /// </param>
    public ProcessReplaceRulesProvider(
        IObjectStore objectStore,
        IServiceLocator serviceLocator,
        IInjectServices injectServices,
        IEnumerable<bool> uniqueFlags)
    {
        this.ServiceLocator = serviceLocator;
        this.objectStore = objectStore;
        this.injectServices = injectServices;
        this.uniqueFlags = uniqueFlags;
    }

    /// <summary>
    ///   Gets the Instance of this provider.
    /// </summary>
    public IProcessReplaceRules Instance
    {
        get
        {
            return this.objectStore.GetOrSet(
                string.Format(Constants.Cache.ReplaceRules, this.uniqueFlags.ToIntOfBits()),
                () =>
                    {
                        var processRules = new ProcessReplaceRules();

                        // inject
                        this.injectServices.Inject(processRules);

                        return processRules;
                    });
        }
    }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }
}