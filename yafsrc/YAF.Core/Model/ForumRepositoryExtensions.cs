/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
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
    using System.Data.SqlClient;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Data;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    /// <summary>
    /// The Forum Repository Extensions
    /// </summary>
    public static class ForumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The forum_save.
        /// </summary>
        /// <param name="forumID">The forum id.</param>
        /// <param name="categoryID">The category id.</param>
        /// <param name="parentID">The parent id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="locked">The locked.</param>
        /// <param name="hidden">The hidden.</param>
        /// <param name="isTest">The is test.</param>
        /// <param name="moderated">The moderated.</param>
        /// <param name="moderatedPostCount">The moderated post count.</param>
        /// <param name="isModeratedNewTopicOnly">The is moderated new topic only.</param>
        /// <param name="accessMaskID">The access mask id.</param>
        /// <param name="remoteURL">The remote url.</param>
        /// <param name="themeURL">The theme url.</param>
        /// <param name="imageURL">The imageURL.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="dummy">The dummy.</param>
        /// <returns>
        /// The forum_save.
        /// </returns>
        public static long Save(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] object forumID,
            [NotNull] object categoryID,
            [NotNull] object parentID,
            [NotNull] object name,
            [NotNull] object description,
            [NotNull] object sortOrder,
            [NotNull] object locked,
            [NotNull] object hidden,
            [NotNull] object isTest,
            [NotNull] object moderated,
            [NotNull] object moderatedPostCount,
            [NotNull] object isModeratedNewTopicOnly,
            [NotNull] object accessMaskID,
            [NotNull] object remoteURL,
            [NotNull] object themeURL,
            [NotNull] object imageURL,
            [NotNull] object styles,
            bool dummy)
        {
            return repository.DbFunction.Scalar.forum_save(
                ForumID: forumID,
                CategoryID: categoryID,
                ParentID: parentID,
                Name: name,
                Description: description,
                SortOrder: sortOrder,
                Locked: locked,
                Hidden: hidden,
                IsTest: isTest,
                Moderated: moderated,
                ModeratedPostCount: moderatedPostCount,
                IsModeratedNewTopicOnly: isModeratedNewTopicOnly,
                RemoteURL: remoteURL,
                ThemeURL: themeURL,
                ImageURL: imageURL,
                Styles: styles,
                AccessMaskID: accessMaskID).ToType<long>();
        }

        /// <summary>
        /// The method returns an integer value for a  found parent forum
        ///   if a forum is a parent of an existing child to avoid circular dependency
        ///   while creating a new forum
        /// </summary>
        /// <param name="forumID">
        /// </param>
        /// <param name="parentID">
        /// </param>
        /// <returns>
        /// Integer value for a found dependency
        /// </returns>
        public static int SaveParentschecker(
            [NotNull] this IRepository<Forum> repository, [NotNull] int forumID, [NotNull] int parentID)
        {
            return repository.DbFunction.GetData.forum_save_parentschecker(ForumID: forumID, ParentID: parentID);
        }

        /// <summary>
        /// The forum list all.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns The forum list all.
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedForumListAll> ForumListAll([NotNull] this IRepository<Forum> repository, int boardId, int userId)
        {
            return repository.ListAllAsDataTable(boardId, userId, 0).AsEnumerable().Select(r => new TypedForumListAll(r));
        }

        /// <summary>
        /// Listes all forums accessible to a user
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="userID">
        /// ID of user
        /// </param>
        /// <returns>
        /// DataTable of all accessible forums
        /// </returns>
        public static DataTable ListAllAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID)
        {
            return repository.ListAllAsDataTable(boardID, userID, 0);
        }

        /// <summary>
        /// Lists all forums accessible to a user
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="userID">
        /// ID of user
        /// </param>
        /// <param name="startAt">
        /// startAt ID
        /// </param>
        /// <returns>
        /// DataTable of all accessible forums
        /// </returns>
        public static DataTable ListAllAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID,
            [NotNull] int startAt)
        {
            return repository.DbFunction.GetData.forum_listall(BoardID: boardID, UserID: userID, Root: startAt);
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="categoryID">
        /// The category ID.
        /// </param>
        /// <returns>
        /// DataTable with list
        /// </returns>
        public static DataTable ListAllFromCatAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int categoryID)
        {
            return repository.ListAllFromCatAsDataTable(boardID, categoryID, true);
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="categoryID">
        /// The category ID.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty First Row.
        /// </param>
        /// <returns>
        /// DataTable with list
        /// </returns>
        public static DataTable ListAllFromCatAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int categoryID,
            bool emptyFirstRow)
        {
            var dt = repository.DbFunction.GetData.forum_listall_fromCat(BoardID: boardID, CategoryID: categoryID);

            return repository.SortList(
                (DataTable)dt,
                0,
                categoryID,
                0,
                null,
                emptyFirstRow);
        }

        /// <summary>
        /// The forum_listall_sorted.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable ListAllSortedAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID)
        {
            return repository.ListAllSortedAsDataTable(boardID, userID, null, false, 0);
        }

        /// <summary>
        /// The forum_listall_sorted.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumidExclusions">
        /// The forumid exclusions.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <param name="startAt">
        /// The start at.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static DataTable ListAllSortedAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID,
            [NotNull] int[] forumidExclusions,
            bool emptyFirstRow,
            int startAt)
        {
            using (var dataTable = repository.ListAllAsDataTable(boardID, userID, startAt))
            {
                var baseForumId = 0;
                var baseCategoryId = 0;

                if (startAt != 0)
                {
                    // find the base ids...
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        if (Convert.ToInt32(dataRow["ForumID"]) == startAt && dataRow["ParentID"] != DBNull.Value
                                                                           && dataRow["CategoryID"] != DBNull.Value)
                        {
                            baseForumId = Convert.ToInt32(dataRow["ParentID"]);
                            baseCategoryId = Convert.ToInt32(dataRow["CategoryID"]);
                            break;
                        }
                    }
                }

                return repository.SortList(dataTable, baseForumId, baseCategoryId, 0, forumidExclusions, emptyFirstRow);
            }
        }

        /// <summary>
        /// Sorry no idea what this does
        /// </summary>
        /// <param name="forumID">
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable ListPathAsDataTable([NotNull] this IRepository<Forum> repository, [NotNull] int forumID)
        {
            return repository.DbFunction.GetData.forum_listpath(
                ForumID: forumID);
        }

        /// <summary>
        /// Lists read topics
        /// </summary>
        /// <param name="boardID">
        ///  The Board ID
        /// </param>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="categoryID">
        /// The category ID.
        /// </param>
        /// <param name="parentID">
        /// The Parent ID.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// DataTable with list
        /// </returns>
        public static DataTable ListReadAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID,
            [NotNull] int? categoryID,
            [NotNull] int? parentID,
            [NotNull] bool useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            return repository.DbFunction.GetData.forum_listread(
                BoardID: boardID,
                UserID: userID,
                CategoryID: categoryID,
                ParentID: parentID,
                StyledNicks: useStyledNicks,
                FindLastRead: findLastRead);
        }



        /// <summary>
        /// Gets a list of topics in a forum
        /// </summary>
        /// <param name="boardId">
        /// boardID
        /// </param>
        /// <param name="forumId">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// DataTable with list of topics from a forum
        /// </returns>
        public static List<Forum> List(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId,
            [CanBeNull] int? forumId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            if (forumId.Value == 0)
            {
                forumId = null;
            }

            if (forumId.HasValue)
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>((forum, category) => category.BoardID == boardId).OrderBy(f => f.SortOrder);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
            else
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>(
                        (forum, category) => category.BoardID == boardId && forum.ID == forumId.Value);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
        }

        public static DataTable SimpleListAsDataTable(
            this IRepository<Forum> repository,
            [CanBeNull] int startId = 0,
            [CanBeNull] int limit = 500)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.forum_simplelist(StartID: startId, Limit: limit);
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// bool to indicate that forum has been deleted
        /// </returns>
        public static bool Delete(this IRepository<Forum> repository, [NotNull] int forumID)
        {

            if (!(repository.DbFunction.GetData.forum_listSubForums(ForumID: forumID) is DBNull))
            {
                return false;
            }

            repository.DeleteAttachments(forumID);

            repository.DbFunction.Scalar.forum_delete(ForumID: forumID);

            return true;
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forumOldID">
        /// The forum Old ID.
        /// </param>
        /// <param name="forumNewID">
        /// The forum New ID.
        /// </param>
        /// <returns>
        /// bool to indicate that forum has been deleted
        /// </returns>
        public static bool Move(this IRepository<Forum> repository, [NotNull] int forumOldID, [NotNull] int forumNewID)
        {

            if (!(repository.DbFunction.GetData.forum_listSubForums(ForumID: forumOldID) is DBNull))
            {
                return false;
            }

            repository.DbFunction.Scalar.forum_move(
                ForumOldID: forumOldID,
                ForumNewID: forumNewID,
                UTCTIMESTAMP: DateTime.UtcNow);

            return true;
        }

        /// <summary>
        /// Gets a list of categories????
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <returns>
        /// DataSet with categories
        /// </returns>
        [NotNull]
        public static DataSet ForumAdminAsDataSet(this IRepository<Forum> repository, [NotNull] object boardID)
        {
            // TODO: this function is TERRIBLE. Recode or remove completely.
                using (var ds = new DataSet())
                {
                    using (var trans = repository.DbAccess.BeginTransaction())
                    {
                        var sqlConnection = trans.Connection as SqlConnection;

                    using (var da = new SqlDataAdapter(
                            CommandTextHelpers.GetObjectName("category_list"),
                            sqlConnection))
                        {
                            da.SelectCommand.Transaction = trans as SqlTransaction;
                            da.SelectCommand.AddParam("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, CommandTextHelpers.GetObjectName("Category"));
                            da.SelectCommand.CommandText = CommandTextHelpers.GetObjectName("forum_list");
                            da.Fill(ds, CommandTextHelpers.GetObjectName("ForumUnsorted"));

                            var forumListSorted = ds.Tables[CommandTextHelpers.GetObjectName("ForumUnsorted")].Clone();
                            forumListSorted.TableName = CommandTextHelpers.GetObjectName("Forum");
                            ds.Tables.Add(forumListSorted);
                            forumListSorted.Dispose();
                            ForumListSortBasic(
                                ds.Tables[CommandTextHelpers.GetObjectName("ForumUnsorted")],
                                ds.Tables[CommandTextHelpers.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(CommandTextHelpers.GetObjectName("ForumUnsorted"));
                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[CommandTextHelpers.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[CommandTextHelpers.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
            
        }

        /// <summary>
        /// Return admin view of Categories with Forums/Subforums ordered accordingly.
        /// </summary>
        /// <param name="userID">
        /// UserID
        /// </param>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <returns>
        /// DataSet with categories
        /// </returns>
        [NotNull]
        public static DataSet ModerateListADataSet(this IRepository<Forum> repository, [NotNull] object userID, [NotNull] object boardID)
        {
            
                using (var ds = new DataSet())
                {
                    var sqlConnection = repository.DbAccess.CreateConnectionOpen() as SqlConnection;

                    using (var da = new SqlDataAdapter(
                        CommandTextHelpers.GetObjectName("category_list"),
                        sqlConnection))
                    {
                        using (var trans = da.SelectCommand.Connection.BeginTransaction())
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.AddParam("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, CommandTextHelpers.GetObjectName("Category"));
                            da.SelectCommand.CommandText = CommandTextHelpers.GetObjectName("forum_moderatelist");
                            da.SelectCommand.AddParam("UserID", userID);
                            da.Fill(ds, CommandTextHelpers.GetObjectName("ForumUnsorted"));

                            var forumListSorted = ds.Tables[CommandTextHelpers.GetObjectName("ForumUnsorted")].Clone();

                            forumListSorted.TableName = CommandTextHelpers.GetObjectName("Forum");
                            ds.Tables.Add(forumListSorted);
                            forumListSorted.Dispose();
                            ForumListSortBasic(
                                ds.Tables[CommandTextHelpers.GetObjectName("ForumUnsorted")],
                                ds.Tables[CommandTextHelpers.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(CommandTextHelpers.GetObjectName("ForumUnsorted"));

                            // vzrus: Remove here all forums with no reports. Would be better to do it in query...
                            // Array to write categories numbers
                            var categories = new int[ds.Tables[CommandTextHelpers.GetObjectName("Forum")].Rows.Count];
                            var cntr = 0;

                            // We should make it before too as the colection was changed
                            ds.Tables[CommandTextHelpers.GetObjectName("Forum")].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[CommandTextHelpers.GetObjectName("Forum")].Rows)
                            {
                                categories[cntr] = dr["CategoryID"].ToType<int>();
                                if (dr["ReportedCount"].ToType<int>() == 0 && dr["MessageCount"].ToType<int>() == 0)
                                {
                                    dr.Delete();
                                    categories[cntr] = 0;
                                }

                                cntr++;
                            }

                            ds.Tables[CommandTextHelpers.GetObjectName("Forum")].AcceptChanges();

                            foreach (var dr in from DataRow dr in ds.Tables[CommandTextHelpers.GetObjectName("Category")].Rows
                                               let dr1 = dr
                                               where !categories.Any(category => category == dr1["CategoryID"].ToType<int>())
                                               select dr)
                            {
                                dr.Delete();
                            }

                            ds.Tables[CommandTextHelpers.GetObjectName("Category")].AcceptChanges();

                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[CommandTextHelpers.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[CommandTextHelpers.GetObjectName("Forum")].Columns["CategoryID"]);

                            trans.Commit();
                        }

                        return ds;
                    }
                }
        }

        #endregion

        /// <summary>
        /// Deletes attachments out of a entire forum
        /// </summary>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        private static void DeleteAttachments(this IRepository<Forum> repository, [NotNull] int forumID)
        {
            var topicRepository = YafContext.Current.GetRepository<Topic>();

            topicRepository.Get(t => t.ForumID == forumID).ForEach(t => topicRepository.Delete(t.ID, true));
        }

        /// <summary>
        /// The SortList.
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
        /// <param name="forumidExclusions">
        /// The forumid exclusions.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        private static DataTable SortList(
            this IRepository<Forum> repository,
            [NotNull] DataTable listSource,
            int parentID,
            int categoryID,
            int startingIndent,
            [NotNull] int[] forumidExclusions,
            bool emptyFirstRow)
        {
            var listDestination = new DataTable { TableName = "forum_sort_list" };

            listDestination.Columns.Add("ForumID", typeof(int));
            listDestination.Columns.Add("Title", typeof(string));

            if (emptyFirstRow)
            {
                var blankRow = listDestination.NewRow();
                blankRow["ForumID"] = 0;
                blankRow["Title"] = string.Empty;
                listDestination.Rows.Add(blankRow);
            }

            // filter the forum list
            var dv = listSource.DefaultView;

            if (forumidExclusions != null && forumidExclusions.Length > 0)
            {
                dv.RowFilter = string.Format("ForumID NOT IN ({0})", forumidExclusions.ToDelimitedString(","));
                dv.ApplyDefaultSort = true;
            }

            repository.SortListRecursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);

            return listDestination;
        }

        /// <summary>
        /// The SortListRecursive.
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
            this IRepository<Forum> repository,
            [NotNull] DataTable listSource,
            [NotNull] DataTable listDestination,
            int parentID,
            int categoryID,
            int currentIndent)
        {
            foreach (DataRow row in listSource.Rows)
            {
                // see if this is a root-forum
                if (row["ParentID"] == DBNull.Value)
                {
                    row["ParentID"] = 0;
                }

                if (row["ParentID"].ToType<int>() != parentID)
                {
                    continue;
                }

                DataRow newRow;
                if ((int)row["CategoryID"] != categoryID)
                {
                    categoryID = (int)row["CategoryID"];

                    newRow = listDestination.NewRow();
                    newRow["ForumID"] = -categoryID; // Ederon : 9/4/2007
                    newRow["Title"] = string.Format("{0}", row["Category"]);
                    listDestination.Rows.Add(newRow);
                }

                var indent = string.Empty;

                for (var j = 0; j < currentIndent; j++)
                {
                    indent += "--";
                }

                // import the row into the destination
                newRow = listDestination.NewRow();

                newRow["ForumID"] = row["ForumID"];
                newRow["Title"] = string.Format(" -{0} {1}", indent, row["Forum"]);

                listDestination.Rows.Add(newRow);

                // recurse through the list...
                repository.SortListRecursive(
                    listSource,
                    listDestination,
                    (int)row["ForumID"],
                    categoryID,
                    currentIndent + 1);
            }
        }

        /// <summary>
        /// The forum_list_sort_basic.
        /// </summary>
        /// <param name="listsource">
        /// The listsource.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="parentid">
        /// The parentid.
        /// </param>
        /// <param name="currentLvl">
        /// The current lvl.
        /// </param>
        private static void ForumListSortBasic(
            [NotNull] DataTable listsource,
            [NotNull] DataTable list,
            int parentid,
            int currentLvl)
        {
            for (var i = 0; i < listsource.Rows.Count; i++)
            {
                var row = listsource.Rows[i];
                if (row["ParentID"] == DBNull.Value)
                {
                    row["ParentID"] = 0;
                }

                if ((int)row["ParentID"] == parentid)
                {
                    var indent = string.Empty;
                    var iIndent = Convert.ToInt32(currentLvl);
                    for (var j = 0; j < iIndent; j++)
                    {
                        indent += "--";
                    }

                    row["Name"] = string.Format(" -{0} {1}", indent, row["Name"]);
                    list.Rows.Add(row.ItemArray);
                    ForumListSortBasic(listsource, list, (int)row["ForumID"], currentLvl + 1);
                }
            }
        }
    }
}