/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    /// The ForumAcess Repository Extensions
    /// </summary>
    public static class ForumAcessRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The forumaccess_group.
        /// </summary>
        /// <param name="groupID">
        /// The group id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable GroupAsDataTable(this IRepository<ForumAccess> repository, [NotNull] int groupID)
        {
            var dt = repository.DbFunction.GetData.forumaccess_group(GroupID: groupID);

            return repository.SortList((DataTable)dt, 0, 0, 0);
        }

        /// <summary>
        /// The user_accessmasks.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable UserAccessMasksAsDataTable(
            this IRepository<ForumAccess> repository,
            [NotNull] int boardID,
            [NotNull] int userID)
        {
            var dt = repository.DbFunction.GetData.user_accessmasks(BoardID: boardID, UserID: userID);

            ///TODO: Recursion doesn't work here correctly at all because of UNION in underlying sql script.
            /// Possibly the only acceptable solution will be splitting the UNIONed queries and displaying 2 "trees". Maybe another solution exists.
            return repository.SortList((DataTable)dt, 0, 0, 0);
        }

        /// <summary>
        /// Save Updated Forum Access
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="accessMaskId">The access mask identifier.</param>
        public static void Save(this IRepository<ForumAccess> repository, [NotNull] int forumId, [NotNull] int groupId, [NotNull] int accessMaskId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(
                () => new ForumAccess { AccessMaskID = accessMaskId },
                f => f.ForumID == forumId && f.GroupID == groupId);
        }

        /// <summary>
        /// Gets the paged list of all users Album Images
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public static List<ForumAccessList> GetForumAccessList(
            [NotNull] this IRepository<ForumAccess> repository,
            int forumId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

            expression.Join<ForumAccess, Group>((access, group) => group.ID == access.GroupID)
                .Where<ForumAccess, Group>((access, group) => access.ForumID == forumId)
                .Select<ForumAccess, Group>((access, group) => new { access, GroupName = group.Name });

            var results = new List<ForumAccessList>();

            foreach (var result in repository.DbAccess.Execute(db => db.Connection.Select<dynamic>(expression)))
            {
                var item = new ForumAccessList
                               {
                                   GroupID = result.GroupID,
                                   ForumID = result.ForumID,
                                   AccessMaskID = result.AccessMaskID,
                                   GroupName = result.GroupName
                               };

                results.Add(item);
            }

            return results;
        }

        #endregion

        /// <summary>
        /// The userforumaccess_sort_list.
        /// </summary>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        private static DataTable SortList(
            [NotNull] this IRepository<ForumAccess> repository, [NotNull] DataTable listSource, int parentID, int categoryID, int startingIndent)
        {
            var listDestination = new DataTable();

            listDestination.Columns.Add("ForumID", typeof(String));
            listDestination.Columns.Add("ForumName", typeof(String));

            // it is uset in two different procedures with different tables,
            // so, we must add correct columns
            if (listSource.Columns.IndexOf("AccessMaskName") >= 0)
            {
                listDestination.Columns.Add("AccessMaskName", typeof(String));
            }
            else
            {
                listDestination.Columns.Add("BoardName", typeof(String));
                listDestination.Columns.Add("CategoryName", typeof(String));
                listDestination.Columns.Add("AccessMaskId", typeof(Int32));
            }

            var dv = listSource.DefaultView;
            repository.SortListRecursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);
            return listDestination;
        }

        /// <summary>
        /// The userforumaccess_sort_list_recursive.
        /// </summary>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="listDestination">
        /// The list destination.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="currentIndent">
        /// The current indent.
        /// </param>
        private static void SortListRecursive(
            [NotNull] this IRepository<ForumAccess> repository, [NotNull] DataTable listSource, [NotNull] DataTable listDestination, int parentID, int categoryID, int currentIndent)
        {
            foreach (DataRow row in listSource.Rows)
            {
                // see if this is a root-forum
                if (row["ParentID"] == DBNull.Value)
                {
                    row["ParentID"] = 0;
                }

                if ((int)row["ParentID"] == parentID)
                {
                    var sIndent = string.Empty;

                    for (var j = 0; j < currentIndent; j++)
                    {
                        sIndent += "--";
                    }

                    // import the row into the destination
                    var newRow = listDestination.NewRow();

                    newRow["ForumID"] = row["ForumID"];
                    newRow["ForumName"] = $"{sIndent} {row["ForumName"]}";
                    if (listDestination.Columns.IndexOf("AccessMaskName") >= 0)
                    {
                        newRow["AccessMaskName"] = row["AccessMaskName"];
                    }
                    else
                    {
                        newRow["BoardName"] = row["BoardName"];
                        newRow["CategoryName"] = row["CategoryName"];
                        newRow["AccessMaskId"] = row["AccessMaskId"];
                    }

                    listDestination.Rows.Add(newRow);

                    // recurse through the list...
                    repository.SortListRecursive(
                      listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
                }
            }
        }
    }

    [Serializable]
    public class ForumAccessList
    {
         public int GroupID { get; set; }

        public int ForumID { get; set; }

        public int AccessMaskID { get; set; }

        public string GroupName { get; set; }
    }
}