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

namespace YAF.Core.Context;

using System;

/// <summary>
/// The board context page provider.
/// </summary>
public class BoardContextPageProvider : IReadOnlyProvider<BoardContext>
{
    /// <summary>
    /// The Page Context name.
    /// </summary>
    private const string PageBoardContextName = "YAF.BoardContext";

    /// <summary>
    /// The global instance.
    /// </summary>
    private static BoardContext globalInstance;

    /// <summary>
    /// The container.
    /// </summary>
    private readonly ILifetimeScope lifetimeScope;

    /// <summary>
    /// The inject services.
    /// </summary>
    private readonly IInjectServices injectServices;

    private readonly IHttpContextAccessor httpContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardContextPageProvider"/> class.
    /// </summary>
    /// <param name="lifetimeScope">
    /// The container.
    /// </param>
    /// <param name="injectServices">
    /// The inject Services.
    /// </param>
    /// <param name="httpContextAccessor">the HttpContext Accessor</param>
    public BoardContextPageProvider(ILifetimeScope lifetimeScope, IInjectServices injectServices, IHttpContextAccessor httpContextAccessor)
    {
        this.lifetimeScope = lifetimeScope;
        this.injectServices = injectServices;
        this.httpContext = httpContextAccessor;
    }

    /// <summary>
    /// Gets Instance.
    /// </summary>
    public BoardContext Instance
    {
        get
        {
            if (this.httpContext.HttpContext == null)
            {
                return globalInstance ??= this.CreateContextInstance();
            }

            if (this.httpContext.HttpContext.Items[PageBoardContextName] is BoardContext context)
            {
                return context;
            }

            var pageInstance = this.CreateContextInstance();

            try
            {
                // make sure it's put back in the page...
                this.httpContext.HttpContext.Items[PageBoardContextName] = pageInstance;
            }
            catch (Exception)
            {
                // Ignore
            }

            return pageInstance;
        }
    }

    /// <summary>
    /// The create context instance.
    /// </summary>
    /// <returns>
    /// The <see cref="BoardContext"/>.
    /// </returns>
    private BoardContext CreateContextInstance()
    {
        var lifetimeContainer = this.lifetimeScope.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);

        var instance = new BoardContext(lifetimeContainer);
        this.injectServices.Inject(instance);

        return instance;
    }
}