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
    using System;
    using System.Collections.Generic;
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The ForumAccess Repository Extensions
    /// </summary>
    public static class ForumAccessRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The group as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GroupAsDataTable(this IRepository<ForumAccess> repository, [NotNull] int groupId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

            expression.Join<Forum>((access, forum) => forum.ID == access.ForumID)
                .Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Category, Board>((category, board) => board.ID == category.BoardID)
                .Where<ForumAccess>(access => access.GroupID == groupId).OrderBy<Board>(board => board.Name)
                .OrderBy<Category>(category => category.SortOrder).OrderBy<Forum>(forum => forum.SortOrder);

            var list = repository.DbAccess.Execute(
                db => db.Connection.SelectMulti<ForumAccess, Forum, Board, Category>(expression));

            return repository.SortList(list, 0, 0);
        }

        /// <summary>
        /// Save Updated Forum Access
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="accessMaskId">The access mask identifier.</param>
        public static void Save(
            this IRepository<ForumAccess> repository,
            [NotNull] int forumId,
            [NotNull] int groupId,
            [NotNull] int accessMaskId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(
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
            [NotNull] int forumId,
            [NotNull] int groupId,
            [NotNull] int accessMaskId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Tuple<ForumAccess, Group>> GetForumAccessList(
            [NotNull] this IRepository<ForumAccess> repository,
            [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

            expression.Join<ForumAccess, Group>((access, group) => group.ID == access.GroupID)
                .Where<ForumAccess, Group>((access, group) => access.ForumID == forumId)
                .Select<ForumAccess, Group>((access, group) => new { access, GroupName = group.Name });

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<ForumAccess, Group>(expression));
        }

        #endregion

        /// <summary>
        /// The sort list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        [NotNull]
        private static DataTable SortList(
            [NotNull] this IRepository<ForumAccess> repository,
            [NotNull] List<Tuple<ForumAccess, Forum, Board, Category>> listSource,
            int parentID,
            int startingIndent)
        {
            var listDestination = new DataTable();

            listDestination.Columns.Add("ForumID", typeof(string));
            listDestination.Columns.Add("ForumName", typeof(string));

            listDestination.Columns.Add("BoardName", typeof(string));
            listDestination.Columns.Add("CategoryName", typeof(string));
            listDestination.Columns.Add("AccessMaskId", typeof(int));

            repository.SortListRecursive(listSource, listDestination, parentID, startingIndent);
            return listDestination;
        }

        /// <summary>
        /// The sort list recursive.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="listDestination">
        /// The list destination.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="currentIndent">
        /// The current indent.
        /// </param>
        private static void SortListRecursive(
            [NotNull] this IRepository<ForumAccess> repository,
            [NotNull] List<Tuple<ForumAccess, Forum, Board, Category>> listSource,
            [NotNull] DataTable listDestination,
            int parentID,
            int currentIndent)
        {
            listSource.ForEach(
                row =>
                    {
                        // see if this is a root-forum
                        row.Item2.ParentID ??= 0;

                        if (row.Item2.ParentID.Value != parentID)
                        {
                            return;
                        }

                        var sIndent = string.Empty;

                        for (var j = 0; j < currentIndent; j++)
                        {
                            sIndent += "--";
                        }

                        // import the row into the destination
                        var newRow = listDestination.NewRow();

                        newRow["ForumID"] = row.Item2.ID;
                        newRow["ForumName"] = $"{sIndent} {row.Item2.Name}";
                        newRow["BoardName"] = row.Item3.Name;
                        newRow["CategoryName"] = row.Item4.Name;
                        newRow["AccessMaskId"] = row.Item1.AccessMaskID;

                        listDestination.Rows.Add(newRow);

                        // recurse through the list...
                        repository.SortListRecursive(
                            listSource,
                            listDestination,
                            row.Item2.ID,
                            currentIndent + 1);
                    });
        }
    }
}