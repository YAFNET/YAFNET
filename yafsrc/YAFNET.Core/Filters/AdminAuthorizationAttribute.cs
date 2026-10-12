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

using YAF.Core.Model;
using YAF.Types.Models;

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
    /// Initializes a new instance of the <see cref="AdminAuthorizationAttribute"/> class (any admin).
    /// </summary>
    public AdminAuthorizationAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthorizationAttribute"/> class.
    /// An admin who is no host admin needs the access to the admin page (like the page itself).
    /// </summary>
    /// <param name="page">The admin page of the dialogs.</param>
    public AdminAuthorizationAttribute(ForumPages page)
    {
        this.Page = page;
    }

    /// <summary>
    /// Gets the admin page whose access is needed (null for any admin).
    /// </summary>
    public ForumPages? Page { get; }

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

        if (BoardContext.Current.IsAdmin && this.HasPageAccess())
        {
            return;
        }

        context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Checks the access of the admin to the admin page (the host admin has the access to all pages).
    /// </summary>
    /// <returns>Returns if the admin can use the page.</returns>
    private bool HasPageAccess()
    {
        if (!this.Page.HasValue || BoardContext.Current.PageUser.UserFlags.IsHostAdmin)
        {
            return true;
        }

        return BoardContext.Current.GetRepository<AdminPageUserAccess>().HasAccess(
            BoardContext.Current.PageUserID,
            this.Page.Value.ToString());
    }
}