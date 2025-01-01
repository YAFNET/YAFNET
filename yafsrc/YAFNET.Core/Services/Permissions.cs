/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services;

using YAF.Types.Objects;

/// <summary>
/// The permissions.
/// </summary>
public class Permissions : IPermissions, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Permissions"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Permissions(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Check Viewing Permissions
    /// </summary>
    /// <param name="permission">
    /// The permission.
    /// </param>
    /// <returns>
    /// The check.
    /// </returns>
    public bool Check(ViewPermissions permission)
    {
        return permission switch
            {
                ViewPermissions.Everyone => true,
                ViewPermissions.RegisteredUsers => !BoardContext.Current.IsGuest,
                _ => BoardContext.Current.IsAdmin
            };
    }

    /// <summary>
    /// The handle request.
    /// </summary>
    /// <param name="permission">
    ///     The permission.
    /// </param>
    public IActionResult HandleRequest(ViewPermissions permission)
    {
        if (this.Check(permission))
        {
            return null;
        }

        if (permission != ViewPermissions.RegisteredUsers)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        return this.Get<BoardConfiguration>().AllowLoginAndLogoff ? this.Get<LinkBuilder>().Redirect(ForumPages.Account_Login) :
                   // fall-through with no access...
                   this.Get<LinkBuilder>().AccessDenied();
    }
}