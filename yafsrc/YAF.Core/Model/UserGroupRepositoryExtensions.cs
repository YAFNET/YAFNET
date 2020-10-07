/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System.Collections.Generic;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The UserGroup repository extensions.
    /// </summary>
    public static class UserGroupRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the User Groups by User Id
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Group> List(this IRepository<UserGroup> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserGroup>();

            expression.Join<Group>((a, b) => b.ID == a.GroupID).Where<UserGroup>((a) => a.UserID == userId)
                .Select<Group>(b => b);

            return repository.DbAccess.Execute(db => db.Connection.Select<Group>(expression));
        }

        /// <summary>
        /// Remove the User Group
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="groupId">
        /// The group Id.
        /// </param>
        public static void Remove(this IRepository<UserGroup> repository, [NotNull] int userId, [NotNull] int groupId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.FireDeleted(repository.Delete(x => x.GroupID == groupId && x.UserID == userId));
        }

        /// <summary>
        /// Add or Removes the User Group
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="groupId">
        /// The group Id.
        /// </param>
        /// <param name="isMember">
        /// The is Member.
        /// </param>
        public static void AddOrRemove(
            this IRepository<UserGroup> repository,
            [NotNull] int userId,
            [NotNull] int groupId,
            [NotNull] bool isMember)
        {
            CodeContracts.VerifyNotNull(repository);

            if (!isMember)
            {
                repository.Remove(userId, groupId);
                return;
            }

            if (!repository.Exists(x => x.UserID == userId && x.GroupID == groupId))
            {
                repository.Insert(new UserGroup { UserID = userId, GroupID = groupId });
            }

            repository.FireNew();
        }

        /// <summary>
        /// Sets the user roles
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void SetRole(
            this IRepository<UserGroup> repository,
            [NotNull] int boardId,
            [NotNull] int userId,
            [NotNull] string role)
        {
            CodeContracts.VerifyNotNull(repository);

            if (role.IsNotSet())
            {
                repository.Delete(x => x.UserID == userId);
            }
            else
            {
                var group = BoardContext.Current.GetRepository<Group>()
                    .GetSingle(x => x.BoardID == boardId && x.Name == role);

                int? groupId;

                if (group == null)
                {
                    groupId = BoardContext.Current.GetRepository<Group>()
                        .Insert(new Group { Name = role, BoardID = boardId, Flags = 0 });

                    repository.DbAccess.Execute(
                        db =>
                        {
                            var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

                            // TODO : Add typed for Insert From Table
                            return db.Connection.ExecuteSql(
                                $@" insert into {expression.Table<ForumAccess>()}(
                                        GroupID,   ForumID, AccessMaskID)
                         select {groupId.Value}, a.ForumID, min(a.AccessMaskID)
                         from {expression.Table<ForumAccess>()} a
                         join {expression.Table<Group>()} b on b.GroupID=a.GroupID
                         where b.BoardID={boardId} and (b.Flags & 4)=4
                         group by a.ForumID");
                        });
                }
                else
                {
                    groupId = group.ID;
                }

                // -- user already can be in the group even if Role isn't null, an extra check is required
                if (!repository.Exists(x => x.UserID == userId && x.GroupID == groupId.Value))
                {
                    repository.Insert(new UserGroup { UserID = userId, GroupID = groupId.Value });
                }
            }
        }

        #endregion
    }
}