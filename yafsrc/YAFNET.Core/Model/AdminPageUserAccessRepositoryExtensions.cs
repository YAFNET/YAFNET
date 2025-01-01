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

namespace YAF.Core.Model;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The admin page user access repository extensions.
/// </summary>
public static class AdminPageUserAccessRepositoryExtensions
{
    /// <summary>
    /// Lists all Pages
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    public static IEnumerable<AdminPageUserAccess> List(
        this IRepository<AdminPageUserAccess> repository,
        int userId)
    {
        return repository.Get(a => a.UserID == userId).Select(
            a => new AdminPageUserAccess { PageName = a.PageName, UserID = a.UserID, ReadAccess = true });
    }

    /// <summary>
    /// Checks if the Admin user has Access to the Admin Page.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageName">
    /// The page name.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool HasAccess(
        this IRepository<AdminPageUserAccess> repository,
        int userId,
        string pageName)
    {
        var access = repository.GetSingle(a => a.UserID == userId && a.PageName == pageName);

        return access != null;
    }

    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageName">
    /// The page name.
    /// </param>
    public static void Save(
        this IRepository<AdminPageUserAccess> repository,
        int userId,
        string pageName)
    {
        if (!repository.Exists(a => a.UserID == userId && a.PageName == pageName))
        {
            repository.Insert(new AdminPageUserAccess { UserID = userId, PageName = pageName });
        }
    }

    /// <summary>
    /// The delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageName">
    /// The page name.
    /// </param>
    public static void Delete(
        this IRepository<AdminPageUserAccess> repository,
        int userId,
        string pageName)
    {
        repository.Delete(u => u.UserID == userId && u.PageName == pageName);
    }
}