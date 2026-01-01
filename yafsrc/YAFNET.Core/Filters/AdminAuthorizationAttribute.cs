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

namespace YAF.Core.Filters;

using Microsoft.AspNetCore.Authorization;

using System;

/// <summary>
/// Class AdminAuthorizationAttribute.
/// Implements the <see cref="AuthorizeAttribute" />
/// Implements the <see cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="AuthorizeAttribute" />
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AdminAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter, IHaveServiceLocator
{
    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Called early in the filter pipeline to confirm request is authorized.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            return;
        }

        if (BoardContext.Current.IsAdmin)
        {
            return;
        }

        context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
    }
}