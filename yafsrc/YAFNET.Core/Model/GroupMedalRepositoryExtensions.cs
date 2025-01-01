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
/// The group medal repository extensions.
/// </summary>
public static class GroupMedalRepositoryExtensions
{
    /// <summary>
    /// Lists all Groups assigned to the medal
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group Id.
    /// </param>
    /// <param name="medalId">
    /// The medal Id.
    /// </param>
    public static List<Tuple<Medal, GroupMedal, Group>> List(
        this IRepository<GroupMedal> repository,
        int? groupId,
        int medalId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Medal>();

        if (groupId.HasValue)
        {
            expression.Join<GroupMedal>((a, b) => b.MedalID == a.ID)
                .Join<GroupMedal, Group>((b, c) => c.ID == b.GroupID)
                .Where<GroupMedal>(b => b.MedalID == medalId && b.GroupID == groupId.Value).OrderBy<Group>(x => x.Name)
                .ThenBy<GroupMedal>(x => x.SortOrder);
        }
        else
        {
            expression.Join<GroupMedal>((a, b) => b.MedalID == a.ID)
                .Join<GroupMedal, Group>((b, c) => c.ID == b.GroupID)
                .Where(a => a.ID == medalId).OrderBy<Group>(x => x.Name).ThenBy<GroupMedal>(x => x.SortOrder);
        }

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Medal, GroupMedal, Group>(expression));
    }

    /// <summary>
    /// Update existing group-medal allocation.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group ID.
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
        this IRepository<GroupMedal> repository,
        int groupId,
        int medalId,
        string message,
        bool hide,
        byte sortOrder)
    {
        repository.UpdateOnly(
            () => new GroupMedal
                      {
                          Message = message,
                          Hide = hide,
                          SortOrder = sortOrder
                      },
            m => m.GroupID == groupId && m.MedalID == medalId);
    }

    /// <summary>
    /// Saves new group-medal allocation.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group ID.
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
        this IRepository<GroupMedal> repository,
        int groupId,
        int medalId,
        string message,
        bool hide,
        byte sortOrder)
    {
        repository.Insert(
            new GroupMedal
                {
                    GroupID = groupId,
                    MedalID = medalId,
                    Message = message,
                    Hide = hide,
                    SortOrder = sortOrder
                });
    }
}