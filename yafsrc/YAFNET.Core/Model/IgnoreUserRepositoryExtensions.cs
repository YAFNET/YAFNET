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
/// The IgnoreUser Repository Extensions
/// </summary>
public static class IgnoreUserRepositoryExtensions
{
    /// <summary>
    /// Deletes the specified user identifier.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ignoreUserId">The ignore user identifier.</param>
    /// <returns>Returns if deleting was successfully or not</returns>
    public static bool Delete(
        this IRepository<IgnoreUser> repository,
        int userId,
        int ignoreUserId)
    {
        var success = repository.DbAccess.Execute(
                          db => db.Connection.Delete<IgnoreUser>(
                              x => x.UserID == userId && x.IgnoredUserID == ignoreUserId)) ==
                      1;

        if (success)
        {
            repository.FireDeleted();
        }

        return success;
    }

    /// <summary>
    /// Add Ignored User
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public static void AddIgnoredUser(
        this IRepository<IgnoreUser> repository,
        int userId,
        int ignoredUserId)
    {
        var ignoreUser = repository.GetSingle(i => i.UserID == userId && i.IgnoredUserID == ignoredUserId);

        if (ignoreUser == null)
        {
            repository.Insert(new IgnoreUser { UserID = userId, IgnoredUserID = ignoredUserId });
        }
    }

    /// <summary>
    /// Gets the List of Ignored Users
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    public static List<User> IgnoredUsers(this IRepository<IgnoreUser> repository, int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<IgnoreUser>();

        expression.Join<User>((i, u) => u.ID == i.IgnoredUserID).Where<IgnoreUser>(u => u.UserID == userId);

        return repository.DbAccess.Execute(db => db.Connection.Select<User>(expression));
    }
}