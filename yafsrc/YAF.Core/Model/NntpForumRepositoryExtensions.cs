/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System;
using System.Collections.Generic;

using YAF.Types.Models;

#endregion

/// <summary>
///     The NntpForum repository extensions.
/// </summary>
public static class NntpForumRepositoryExtensions
{
    #region Public Methods and Operators

    /// <summary>
    /// The update.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="nntpForumId">
    /// The NNTP forum id.
    /// </param>
    /// <param name="lastMessageNo">
    /// The last message no.
    /// </param>
    public static void Update(
        this IRepository<NntpForum> repository,
        [NotNull] int nntpForumId,
        [NotNull] int lastMessageNo)
    {
        CodeContracts.VerifyNotNull(repository);

        repository.UpdateOnly(
            () => new NntpForum { LastMessageNo = lastMessageNo, LastUpdate = DateTime.UtcNow },
            n => n.ID == nntpForumId);
    }

    /// <summary>
    /// Gets the NNTP Forums List
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="active">
    /// The active.
    /// </param>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    public static List<Tuple<NntpForum, NntpServer, Forum>> NntpForumList(
        this IRepository<NntpForum> repository,
        int boardId,
        bool? active)
    {
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<NntpForum>();

        if (active.HasValue)
        {
            expression.Join<NntpServer>((b, a) => a.ID == b.NntpServerID).Join<Forum>((b, f) => f.ID == b.ForumID)
                .Where<NntpForum, NntpServer>((b, a) => a.BoardID == boardId && b.Active == active.Value);
        }
        else
        {
            expression.Join<NntpServer>((b, a) => a.ID == b.NntpServerID).Join<Forum>((b, f) => f.ID == b.ForumID)
                .Where<NntpForum, NntpServer>((b, a) => a.BoardID == boardId);
        }

        return repository.DbAccess.Execute(
            db => db.Connection.SelectMulti<NntpForum, NntpServer, Forum>(expression));
    }

    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="nntpForumId">
    /// The NNTP forum id.
    /// </param>
    /// <param name="nntpServerId">
    /// The NNTP server id.
    /// </param>
    /// <param name="groupName">
    /// The group name.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="active">
    /// The active.
    /// </param>
    /// <param name="dateCutOff">
    /// The date Cut Off.
    /// </param>
    public static void Save(
        this IRepository<NntpForum> repository,
        [NotNull] int? nntpForumId,
        [NotNull] int nntpServerId,
        [NotNull] string groupName,
        [NotNull] int forumId,
        [NotNull] bool active,
        [NotNull] DateTime? dateCutOff)
    {
        if (nntpForumId.HasValue)
        {
            repository.UpdateOnly(
                () => new NntpForum
                          {
                              NntpServerID = nntpServerId,
                              GroupName = groupName,
                              ForumID = forumId,
                              Active = active,
                              DateCutOff = dateCutOff
                          },
                n => n.ID == nntpForumId);
        }
        else
        {
            var entity = new NntpForum
                             {
                                 NntpServerID = nntpServerId,
                                 GroupName = groupName,
                                 ForumID = forumId,
                                 Active = active,
                                 DateCutOff = dateCutOff,
                                 LastUpdate = DateTime.UtcNow,
                                 LastMessageNo = 0,
                             };

            repository.Insert(entity);

            repository.FireNew(entity);
        }
    }

    #endregion
}