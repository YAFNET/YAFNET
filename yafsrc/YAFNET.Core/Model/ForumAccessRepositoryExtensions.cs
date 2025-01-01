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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The ForumAccess Repository Extensions
/// </summary>
public static class ForumAccessRepositoryExtensions
{
    /// <summary>
    /// Assign New Role with Initial Access Mask for all forums.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group identifier.
    /// </param>
    /// <param name="accessMaskId">
    /// The access mask identifier.
    /// </param>
    public static void InitialAssignGroup(
        this IRepository<ForumAccess> repository,
        int groupId,
        int accessMaskId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Category>((f, c) => c.ID == f.CategoryID)
            .Where<Category>(c => c.BoardID == repository.BoardID);

        var forums = repository.DbAccess.Execute(
            db => db.Connection.Select(expression));

        var accessList = new List<ForumAccess>();

        forums.ForEach(
            f => accessList.Add(new ForumAccess
                                       {
                                           GroupID = groupId,
                                           ForumID = f.ID,
                                           AccessMaskID = accessMaskId
                                       }));

        repository.BulkInsert(accessList);
    }

    /// <summary>
    /// Lists the by groups.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group identifier.
    /// </param>
    public static List<(int ForumID, string ForumName, int? ParentID, int AccessMaskID)> ListByGroups(
        this IRepository<ForumAccess> repository,
        int groupId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

        expression.Join<Forum>((access, forum) => forum.ID == access.ForumID)
            .Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
            .Join<Category, Board>((category, board) => board.ID == category.BoardID)
            .Where<ForumAccess, Category>((access, category) => access.GroupID == groupId &&  (category.Flags & 1) == 1).OrderBy<Board>(board => board.Name)
            .OrderBy<Category>(category => category.SortOrder).OrderBy<Forum>(forum => forum.SortOrder)
            .Select<Forum, ForumAccess>(
                (f, a) => new
                              {
                                  ForumID = f.ID, ForumName = f.Name, f.ParentID, AccesMaskID = a.AccessMaskID
                              });

        var list = repository.DbAccess.Execute(
            db => db.Connection
                .Select<(int ForumID, string ForumName, int? ParentID, int AccessMaskID)>(expression));

        return list;
    }

    /// <summary>
    /// Save Updated Forum Access
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="accessMaskId">The access mask identifier.</param>
    public async static Task SaveAsync(
        this IRepository<ForumAccess> repository,
        int forumId,
        int groupId,
        int accessMaskId)
    {
        await repository.UpdateOnlyAsync(
            () => new ForumAccess { AccessMaskID = accessMaskId },
            f => f.ForumID == forumId && f.GroupID == groupId);
    }

    /// <summary>
    /// Creates new Forum Access
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="groupId">
    /// The group id.
    /// </param>
    /// <param name="accessMaskId">
    /// The access mask id.
    /// </param>
    public static void Create(
        this IRepository<ForumAccess> repository,
        int forumId,
        int groupId,
        int accessMaskId)
    {
        repository.Insert(new ForumAccess { AccessMaskID = accessMaskId, GroupID = groupId, ForumID = forumId });
    }

    /// <summary>
    /// Gets the forum access mask as List
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    public static List<Tuple<ForumAccess, Group>> GetForumAccessList(
        this IRepository<ForumAccess> repository,
        int forumId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

        expression.Join<ForumAccess, Group>((access, group) => group.ID == access.GroupID)
            .Where<ForumAccess, Group>((access, group) => access.ForumID == forumId)
            .Select<ForumAccess, Group>((access, group) => new { access, GroupName = group.Name });

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<ForumAccess, Group>(expression));
    }
}