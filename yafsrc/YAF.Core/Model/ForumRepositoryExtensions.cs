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
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Data;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Forum Repository Extensions
    /// </summary>
    public static class ForumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Saves a Forum or if forumId is null creates a new Forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="locked">
        /// The locked.
        /// </param>
        /// <param name="hidden">
        /// The hidden.
        /// </param>
        /// <param name="isTest">
        /// The is test.
        /// </param>
        /// <param name="moderated">
        /// The moderated.
        /// </param>
        /// <param name="moderatedPostCount">
        /// The moderated post count.
        /// </param>
        /// <param name="isModeratedNewTopicOnly">
        /// The is moderated new topic only.
        /// </param>
        /// <param name="remoteURL">
        /// The remote url.
        /// </param>
        /// <param name="themeURL">
        /// The theme url.
        /// </param>
        /// <param name="imageURL">
        /// The image url.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Save(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int? forumID,
            [NotNull] int categoryID,
            [CanBeNull] int? parentID,
            [NotNull] string name,
            [NotNull] string description,
            [NotNull] int sortOrder,
            [NotNull] bool locked,
            [NotNull] bool hidden,
            [NotNull] bool isTest,
            [NotNull] bool moderated,
            [CanBeNull] int? moderatedPostCount,
            [NotNull] bool isModeratedNewTopicOnly,
            [NotNull] string remoteURL,
            [NotNull] string themeURL,
            [NotNull] string imageURL,
            [NotNull] string styles)
        {
            if (parentID.HasValue && parentID.Equals(0))
            {
                parentID = null;
            }

            var flags = new ForumFlags
                            {
                                IsLocked = locked, IsHidden = hidden, IsTest = isTest, IsModerated = moderated
                            };

            if (!forumID.HasValue)
            {
                return repository.Insert(
                    new Forum
                        {
                            Name = name,
                            Description = description,
                            SortOrder = sortOrder,
                            CategoryID = categoryID,
                            RemoteURL = remoteURL,
                            ThemeURL = themeURL,
                            ImageURL = imageURL,
                            Styles = styles,
                            Flags = flags.BitValue,
                            ModeratedPostCount = moderatedPostCount,
                            IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                        });
            }

            repository.UpdateOnly(
                () => new Forum
                          {
                              ParentID = parentID,
                              Name = name,
                              Description = description,
                              SortOrder = sortOrder,
                              CategoryID = categoryID,
                              RemoteURL = remoteURL,
                              ThemeURL = themeURL,
                              ImageURL = imageURL,
                              Styles = styles,
                              Flags = flags.BitValue,
                              ModeratedPostCount = moderatedPostCount,
                              IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                          },
                f => f.ID == forumID);

            return forumID.Value;
        }

        /// <summary>
        /// The method returns an integer value for a  found parent forum
        ///   if a forum is a parent of an existing child to avoid circular dependency
        ///   while creating a new forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <returns>
        /// Integer value for a found dependency
        /// </returns>
        public static int SaveParentsChecker([NotNull] this IRepository<Forum> repository, int forumId, int parentId)
        {
            using (var cmd = repository.DbAccess.GetCommand(
                $"select {CommandTextHelpers.GetObjectName("forum_save_parentschecker")}(@ForumID, @ParentID)",
                CommandType.Text))
            {
                cmd.AddParam("@ForumID", forumId);
                cmd.AddParam("@ParentID", parentId);

                return (int)repository.DbAccess.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// Lists all forums accessible to a user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// DataTable of all accessible forums
        /// </returns>
        public static List<Tuple<Forum, Category, ActiveAccess>> ListAll(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId,
            [NotNull] int userId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<ActiveAccess>((forum, active) => active.ForumID == forum.ID)
                .Where<Forum, Category, ActiveAccess>(
                    (forum, category, active) =>
                        active.UserID == userId && category.BoardID == boardId && active.ReadAccess)
                .Select<Forum, Category, Active>(
                    (forum, category, active) => new
                                                     {
                                                         CategoryID = category.ID,
                                                         Category = category.Name,
                                                         ForumID = forum.ID,
                                                         Forum = forum.Name,
                                                         Indent = 0,
                                                         forum.ParentID,
                                                         forum.PollGroupID
                                                     });

            return repository.DbAccess.Execute(
                db => db.Connection.SelectMulti<Forum, Category, ActiveAccess>(expression));
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category ID.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListAllFromCategory(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int categoryId)
        {
            return repository.ListAllFromCategory(categoryId, true);
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category ID.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty First Row.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListAllFromCategory(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int categoryId,
            bool emptyFirstRow)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Where<Forum, Category>(
                    (forum, category) => category.BoardID == repository.BoardID && category.ID == categoryId);

            var list = repository.DbAccess.Execute(db => db.Connection.Select(expression));

            return repository.SortList(list, 0, 0, emptyFirstRow);
        }

        /// <summary>
        /// Gets the forum list all sorted as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListAllSortedAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID)
        {
            return repository.ListAllSortedAsDataTable(boardID, userID, false);
        }

        /// <summary>
        /// Gets the forum list all sorted as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        [NotNull]
        public static DataTable ListAllSortedAsDataTable(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID,
            bool emptyFirstRow)
        {
            var dataTable = repository.ListAll(boardID, userID);

            return repository.SortList(dataTable, 0, 0, 0, emptyFirstRow);
        }

        /// <summary>
        /// Sorry no idea what this does
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListPathAsDataTable([NotNull] this IRepository<Forum> repository, [NotNull] int forumID)
        {
            return repository.DbFunction.GetData.forum_listpath(ForumID: forumID);
        }

        /// <summary>
        /// Lists read topics
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The Board ID
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
        /// Indicates if the Table should Contain the last Access Date
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
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The Board ID
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

            if (forumId.HasValue)
            {
                if (forumId.Value == 0)
                {
                    forumId = null;
                }
            }

            if (forumId.HasValue)
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>(
                        (forum, category) => category.BoardID == boardId && forum.ID == forumId.Value);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
            else
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>((forum, category) => category.BoardID == boardId).OrderBy(f => f.SortOrder);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
        }

        /// <summary>
        /// The simple list as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="startId">
        /// The start id.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
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
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// Indicate that forum has been deleted
        /// </returns>
        public static bool Delete(this IRepository<Forum> repository, [NotNull] int forumID)
        {
            if (repository.Exists(f => f.ParentID == forumID))
            {
                return false;
            }

            repository.DbFunction.Scalar.forum_delete(ForumID: forumID);

            return true;
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumOldID">
        /// The forum Old ID.
        /// </param>
        /// <param name="forumNewID">
        /// The forum New ID.
        /// </param>
        /// <returns>
        /// Indicates that forum has been deleted
        /// </returns>
        public static bool Move(this IRepository<Forum> repository, [NotNull] int forumOldID, [NotNull] int forumNewID)
        {
            if (repository.Exists(f => f.ParentID == forumOldID))
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
        /// Gets all Forums sorted by category
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Forum> GetByCategorySorted(this IRepository<Forum> repository, [NotNull] int categoryId)
        {
            var forums = repository.Get(f => f.CategoryID == categoryId);

            var forumsSorted = new List<Forum>();

            ForumListSortBasic(forums, forumsSorted, 0, 0);

            return forumsSorted;
        }

        /// <summary>
        /// Return admin view of Categories with Forums/Sub-forums ordered accordingly.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        ///  The User ID
        /// </param>
        /// <param name="boardId">
        ///  The Board ID
        /// </param>
        /// <returns>
        /// Data table with categories
        /// </returns>
        [NotNull]
        public static DataTable ModerateListAsDataTable(
            this IRepository<Forum> repository,
            [NotNull] object userId,
            [NotNull] object boardId)
        {
            var forumUnsorted =
                repository.DbFunction.GetAsDataTable(f => f.forum_moderatelist(BoardID: boardId, UserID: userId));

            var forumListSorted = forumUnsorted.Clone();

            forumListSorted.Dispose();
            ForumListSortBasic(forumUnsorted, forumListSorted, 0, 0);

            // vzrus: Remove here all forums with no reports. Would be better to do it in query...
            // Array to write categories numbers
            var categories = new int[forumListSorted.Rows.Count];
            var count = 0;

            // We should make it before too as the collection was changed
            forumListSorted.AcceptChanges();

            forumListSorted.Rows.Cast<DataRow>().ForEach(
                row =>
                    {
                        categories[count] = row.Field<int>("CategoryID");
                        if (row.Field<int>("ReportedCount") == 0 && row.Field<int>("MessageCount") == 0)
                        {
                            row.Delete();
                            categories[count] = 0;
                        }

                        count++;
                    });

            forumListSorted.AcceptChanges();

            return forumListSorted;
        }

        #endregion

        /// <summary>
        /// The SortList.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        [NotNull]
        private static DataTable SortList(
            this IRepository<Forum> repository,
            [NotNull] List<Forum> listSource,
            int parentId,
            int startingIndent,
            bool emptyFirstRow)
        {
            var listDestination = new DataTable { TableName = "forum_sort_list" };

            listDestination.Columns.Add("ForumID", typeof(int));
            listDestination.Columns.Add("Title", typeof(string));
            listDestination.Columns.Add("Icon", typeof(string));

            if (emptyFirstRow)
            {
                var blankRow = listDestination.NewRow();
                blankRow["ForumID"] = 0;
                blankRow["Title"] = BoardContext.Current.Get<ILocalization>().GetText("NONE");
                blankRow["Icon"] = string.Empty;
                listDestination.Rows.Add(blankRow);
            }

            repository.SortListRecursive(listSource, listDestination, parentId, startingIndent);

            return listDestination;
        }

        /// <summary>
        /// The SortList.
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
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        [NotNull]
        private static DataTable SortList(
            this IRepository<Forum> repository,
            [NotNull] List<Tuple<Forum, Category, ActiveAccess>> listSource,
            int parentID,
            int categoryID,
            int startingIndent,
            bool emptyFirstRow)
        {
            var listDestination = new DataTable { TableName = "forum_sort_list" };

            listDestination.Columns.Add("ForumID", typeof(int));
            listDestination.Columns.Add("Title", typeof(string));
            listDestination.Columns.Add("Icon", typeof(string));

            if (emptyFirstRow)
            {
                var blankRow = listDestination.NewRow();
                blankRow["ForumID"] = 0;
                blankRow["Title"] = BoardContext.Current.Get<ILocalization>().GetText("NONE");
                blankRow["Icon"] = string.Empty;
                listDestination.Rows.Add(blankRow);
            }

            repository.SortListRecursive(listSource, listDestination, parentID, categoryID, startingIndent);

            return listDestination;
        }

        /// <summary>
        /// The SortListRecursive.
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
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="currentIndent">
        /// The current indent.
        /// </param>
        private static void SortListRecursive(
            this IRepository<Forum> repository,
            [NotNull] List<Tuple<Forum, Category, ActiveAccess>> listSource,
            [NotNull] DataTable listDestination,
            int parentID,
            int categoryID,
            int currentIndent)
        {
            foreach (var (item1, item2, _) in listSource)
            {
                // see if this is a root-forum
                if (!item1.ParentID.HasValue)
                {
                    item1.ParentID = 0;
                }

                if (item1.ParentID != parentID)
                {
                    continue;
                }

                DataRow newRow;
                if (item2.ID != categoryID)
                {
                    categoryID = item2.ID;

                    newRow = listDestination.NewRow();
                    newRow["ForumID"] = -categoryID;
                    newRow["Title"] = $"{item2.Name}";
                    newRow["Icon"] = "folder";
                    listDestination.Rows.Add(newRow);
                }

                var indent = string.Empty;

                for (var j = 0; j < currentIndent; j++)
                {
                    indent += "--";
                }

                // import the row into the destination
                newRow = listDestination.NewRow();

                newRow["ForumID"] = item1.ID;
                newRow["Title"] = $" -{indent} {item1.Name}";
                newRow["Icon"] = "comments";

                listDestination.Rows.Add(newRow);

                // recurse through the list...
                repository.SortListRecursive(listSource, listDestination, item1.ID, categoryID, currentIndent + 1);
            }
        }

        /// <summary>
        /// The SortListRecursive.
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
            this IRepository<Forum> repository,
            [NotNull] List<Forum> listSource,
            [NotNull] DataTable listDestination,
            int parentID,
            int currentIndent)
        {
            listSource.ForEach(
                forum =>
                    {
                        // see if this is a root-forum
                        if (!forum.ParentID.HasValue)
                        {
                            forum.ParentID = 0;
                        }

                        if (forum.ParentID != parentID)
                        {
                            return;
                        }

                        var indent = string.Empty;

                        for (var j = 0; j < currentIndent; j++)
                        {
                            indent += "--";
                        }

                        // import the row into the destination
                        var newRow = listDestination.NewRow();

                        newRow["ForumID"] = forum.ID;
                        newRow["Title"] = $" -{indent} {forum.Name}";
                        newRow["Icon"] = "comments";

                        listDestination.Rows.Add(newRow);

                        // recurse through the list...
                        repository.SortListRecursive(listSource, listDestination, forum.ID, currentIndent + 1);
                    });
        }

        /// <summary>
        /// Basic Sorting for the Forum List
        /// </summary>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <param name="currentLevel">
        /// The current level.
        /// </param>
        private static void ForumListSortBasic(
            [NotNull] List<Forum> listSource,
            [NotNull] ICollection<Forum> list,
            int parentId,
            int currentLevel)
        {
            listSource.ForEach(
                row =>
                    {
                        if (!row.ParentID.HasValue)
                        {
                            row.ParentID = 0;
                        }

                        if (row.ParentID != parentId)
                        {
                            return;
                        }

                        var indent = string.Empty;
                        var intentIndex = currentLevel.ToType<int>();

                        for (var j = 0; j < intentIndex; j++)
                        {
                            indent += "--";
                        }

                        row.Name = $" -{indent} {row.Name}";
                        list.Add(row);
                        ForumListSortBasic(listSource, list, row.ID, currentLevel + 1);
                    });
        }

        /// <summary>
        /// Basic Sorting for the Forum List
        /// </summary>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <param name="currentLevel">
        /// The current level.
        /// </param>
        private static void ForumListSortBasic(
            [NotNull] DataTable listSource,
            [NotNull] DataTable list,
            int parentId,
            int currentLevel)
        {
            for (var i = 0; i < listSource.Rows.Count; i++)
            {
                var row = listSource.Rows[i];
                if (row["ParentID"] == DBNull.Value)
                {
                    row["ParentID"] = 0;
                }

                if (row.Field<int>("ParentID") != parentId)
                {
                    continue;
                }

                var indent = string.Empty;
                var intentIndex = currentLevel.ToType<int>();

                for (var j = 0; j < intentIndex; j++)
                {
                    indent += "--";
                }

                row["Name"] = $" -{indent} {row["Name"]}";
                list.Rows.Add(row.ItemArray);
                ForumListSortBasic(listSource, list, (int)row["ForumID"], currentLevel + 1);
            }
        }
    }
}