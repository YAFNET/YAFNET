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

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
///     The UserMedal repository extensions.
/// </summary>
public static class UserMedalRepositoryExtensions
{
    /// <summary>
    /// Lists users assigned to the medal
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="medalId">
    /// The medal Id.
    /// </param>
    public static List<Tuple<Medal, UserMedal, User>> List(
        this IRepository<UserMedal> repository,
        int? userId,
        int medalId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Medal>();

        if (userId.HasValue)
        {
            expression.Join<UserMedal>((a, b) => b.MedalID == a.ID)
                .Join<UserMedal, User>((b, c) => c.ID == b.UserID)
                .Where<UserMedal>(b => b.MedalID == medalId && b.UserID == userId.Value).OrderBy<User>(x => x.Name)
                .ThenBy<UserMedal>(x => x.SortOrder);
        }
        else
        {
            expression.Join<UserMedal>((a, b) => b.MedalID == a.ID)
                .Join<UserMedal, User>((b, c) => c.ID == b.UserID).Where(a => a.ID == medalId)
                .OrderBy<User>(x => x.Name).ThenBy<UserMedal>(x => x.SortOrder);
        }

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Medal, UserMedal, User>(expression));
    }

    /// <summary>
    /// Update existing user-medal allocation.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// ID of user.
    /// </param>
    /// <param name="medalId">
    /// ID of medal.
    /// </param>
    /// <param name="message">
    /// Medal message, to override medal's default one. Can be null.
    /// </param>
    /// <param name="hide">
    /// Hide medal in user box.
    /// </param>
    /// <param name="sortOrder">
    /// Sort order in user box. Overrides medal's default sort order.
    /// </param>
    public static void Save(
        this IRepository<UserMedal> repository,
        int userId,
        int medalId,
        string message,
        bool hide,
        byte sortOrder)
    {
        repository.UpdateOnly(
            () => new UserMedal { Message = message, Hide = hide, SortOrder = sortOrder },
            m => m.UserID == userId && m.MedalID == medalId);

        repository.FireUpdated(medalId);
    }

    /// <summary>
    /// Saves new user-medal allocation.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// ID of user.
    /// </param>
    /// <param name="medalId">
    /// ID of medal.
    /// </param>
    /// <param name="message">
    /// Medal message, to override medal's default one. Can be null.
    /// </param>
    /// <param name="hide">
    /// Hide medal in user box.
    /// </param>
    /// <param name="sortOrder">
    /// Sort order in user box. Overrides medal's default sort order.
    /// </param>
    public static void SaveNew(
        this IRepository<UserMedal> repository,
        int userId,
        int medalId,
        string message,
        bool hide,
        byte sortOrder)
    {
        var newId = repository.Insert(
            new UserMedal
                {
                    UserID = userId,
                    MedalID = medalId,
                    Message = message,
                    Hide = hide,
                    SortOrder = sortOrder,
                    DateAwarded = DateTime.UtcNow
                });

        repository.FireNew(newId);
    }
}