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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    
    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     Class used for multi-step DB operations so they can be cached, etc.
    /// </summary>
    public class DataBroker : IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBroker"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        public DataBroker(
            IServiceLocator serviceLocator,
            BoardSettings boardSettings,
            IDataCache dataCache)
        {
            this.ServiceLocator = serviceLocator;
            this.BoardSettings = boardSettings;
            this.DataCache = dataCache;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the board settings.
        /// </summary>
        /// <value>
        /// The board settings.
        /// </value>
        public BoardSettings BoardSettings { get; set; }

        /// <summary>
        ///     Gets or sets DataCache.
        /// </summary>
        public IDataCache DataCache { get; set; }

        /// <summary>
        ///     Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The user lazy data.
        /// </summary>
        /// <param name="userId"> The user ID. </param>
        /// <returns> Returns the Active User </returns>
        public DataRow ActiveUserLazyData(int userId)
        {
            // get a row with user lazy data...
            return
                this.DataCache.GetOrSet(
                    string.Format(Constants.Cache.ActiveUserLazyData, userId),
                    () =>
                    this.GetRepository<User>().LazyDataRow(
                        userId,
                        BoardContext.Current.PageBoardID,
                        this.BoardSettings.EnableBuddyList,
                        this.BoardSettings.AllowPrivateMessages,
                        this.BoardSettings.EnableAlbum,
                        this.BoardSettings.UseStyledNicks).Table,
                    TimeSpan.FromMinutes(this.BoardSettings.ActiveUserLazyDataCacheTimeout)).Rows[0];
        }

        /// <summary>
        ///     Returns the layout of the board
        /// </summary>
        /// <param name="boardID"> The board ID. </param>
        /// <param name="userID"> The user ID. </param>
        /// <param name="categoryID"> The category ID. </param>
        /// <param name="parentID"> The parent ID. </param>
        /// <returns> The board layout. </returns>
        public DataSet BoardLayout(int boardID, int userID, int? categoryID, int? parentID)
        {
            if (categoryID.HasValue && categoryID == 0)
            {
                categoryID = null;
            }

            using (var ds = new DataSet())
            {
                // get the cached version of forum moderators if it's valid
                DataTable moderator;
                if (this.BoardSettings.ShowModeratorList)
                {
                    moderator = this.GetModerators();
                }
                else
                {
                    // add dummy table.
                    moderator = new DataTable("Moderator");
                    moderator.Columns.AddRange(
                        new[]
                            {
                                new DataColumn("ForumID", typeof(int)), new DataColumn("ForumName", typeof(string)),
                                new DataColumn("ModeratorName", typeof(string)),
                                new DataColumn("ModeratorDisplayName", typeof(string)),
                                new DataColumn("ModeratorEmail", typeof(string)),
                                new DataColumn("ModeratorAvatar", typeof(string)),
                                new DataColumn("ModeratorAvatarImage", typeof(bool)),
                                new DataColumn("Style", typeof(string)), 
                                new DataColumn("IsGroup", typeof(bool))
                            });
                }

                // insert it into this DataSet
                ds.Tables.Add(moderator.Copy());

                // get the Category Table
                var category = this.DataCache.GetOrSet(
                    Constants.Cache.ForumCategory,
                    () =>
                        {
                            var categories = this.GetRepository<Category>().ListAsDataTable();
                            categories.TableName = "Category";
                            return categories;
                        },
                    TimeSpan.FromMinutes(this.BoardSettings.BoardCategoriesCacheTimeout));

                // add it to this DataSet
                ds.Tables.Add(category.Copy());

                var categoryTable = ds.Tables["Category"];

                if (categoryID.HasValue)
                {
                    // make sure this only has the category desired in the DataSet
                    categoryTable.AsEnumerable().Where(row => row.Field<int>("CategoryID") != categoryID).ForEach(
                        row => row.Delete());

                     categoryTable.AcceptChanges();
                }

                var forum = this.GetRepository<Forum>().ListReadAsDataTable(
                    boardID,
                    userID,
                    categoryID,
                    parentID,
                    this.BoardSettings.UseStyledNicks,
                    this.BoardSettings.UseReadTrackingByDatabase);

                forum.TableName = "Forum";
                ds.Tables.Add(forum.Copy());

                ds.Relations.Add(
                    "FK_Forum_Category",
                    categoryTable.Columns["CategoryID"],
                    ds.Tables["Forum"].Columns["CategoryID"],
                    false);

                ds.Relations.Add(
                    "FK_Moderator_Forum",
                    ds.Tables["Forum"].Columns["ForumID"],
                    ds.Tables["Moderator"].Columns["ForumID"],
                    false);

                var deletedCategory = false;

                // remove empty categories...
                categoryTable.SelectTypedList(
                            row => new { row, childRows = row.GetChildRows("FK_Forum_Category") })
                            .Where(t => !t.childRows.Any())
                            .Select(t => t.row).ForEach(r =>
                {
                    // remove this category...
                    r.Delete();
                    deletedCategory = true;
                });

                if (deletedCategory)
                {
                    categoryTable.AcceptChanges();
                }

                return ds;
            }
        }

        /// <summary>
        ///     The get all moderators.
        /// </summary>
        /// <returns> Returns List with all moderators </returns>
        public List<SimpleModerator> GetAllModerators()
        {
            // get the cached version of forum moderators if it's valid
            var moderator = this.GetModerators();

            return
                moderator.SelectTypedList(
                    row =>
                    new SimpleModerator(
                        row.Field<int>("ForumID"),
                        row.Field<string>("ForumName"),
                        row.Field<int>("ModeratorID"),
                        row.Field<string>("ModeratorName"),
                        row.Field<string>("ModeratorEmail"),
                        row.Field<int>("ModeratorBlockFlags"),
                        row.Field<string>("ModeratorAvatar"),
                        row.Field<bool>("ModeratorAvatarImage"),
                        row.Field<string>("ModeratorDisplayName"),
                        row.Field<string>("Style"),
                        row["IsGroup"].ToType<bool>(),
                        row.Field<System.DateTime?>("Suspended"))).ToList();
        }

        /// <summary>
        ///     Get a simple forum/topic listing.
        /// </summary>
        /// <param name="boardId"> The board Id. </param>
        /// <param name="userId"> The user Id. </param>
        /// <param name="timeFrame"> The time Frame. </param>
        /// <param name="maxCount"> The max Count. </param>
        /// <returns> The get simple forum topic. </returns>
        public List<SimpleForum> GetSimpleForumTopic(int boardId, int userId, System.DateTime timeFrame, int maxCount)
        {
            var forumData = this.GetRepository<Forum>().ListAll(boardId, userId).AsEnumerable()
                .Select(x => new SimpleForum { ForumID = x.Item1.ID, Name = x.Item1.Name }).ToList();

            if (forumData.Any())
            {
                // If the user is not logged in (Active Access Table is empty), we need to make sure the Active Access Tables are set
                this.GetRepository<ActiveAccess>().PageAccessAsDataTable(boardId, userId, false);

                forumData = this.GetRepository<Forum>().ListAll(boardId, userId).AsEnumerable()
                    .Select(x => new SimpleForum { ForumID = x.Item1.ID, Name = x.Item1.Name }).ToList();
            }

            // get topics for all forums...
            forumData.ForEach(forum =>
            {
                var forum1 = forum;

                // add topics
                var topics =
                    this.GetRepository<Topic>().ListAsDataTable(
                        forum1.ForumID,
                        userId,
                        timeFrame,
                        System.DateTime.UtcNow,
                        0,
                        maxCount,
                        false,
                        false,
                        false).AsEnumerable();

                // filter first...
                forum.Topics =
                    topics.Where(x => x.Field<System.DateTime>("LastPosted") >= timeFrame)
                        .Select(x => this.LoadSimpleTopic(x, forum1))
                        .ToList();
            });

            return forumData;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get all moderators by Groups and User
        /// </summary>
        /// <returns> Returns the Moderator List </returns>
        private DataTable GetModerators()
        {
            return this.DataCache.GetOrSet(
                Constants.Cache.ForumModerators,
                () =>
                {
                    var moderator =
                        this.GetRepository<User>().GetForumModeratorsAsDataTable(this.BoardSettings.UseStyledNicks);
                    moderator.TableName = "Moderator";
                    return moderator;
                },
                TimeSpan.FromMinutes(this.Get<BoardSettings>().BoardModeratorsCacheTimeout));
        }

        /// <summary>
        ///     The load simple topic.
        /// </summary>
        /// <param name="row"> The row. </param>
        /// <param name="forum"> The forum. </param>
        /// <returns> Returns the simple topic. </returns>
        [NotNull]
        private SimpleTopic LoadSimpleTopic([NotNull] DataRow row, [NotNull] SimpleForum forum)
        {
            CodeContracts.VerifyNotNull(row, "row");
            CodeContracts.VerifyNotNull(forum, "forum");

            return new SimpleTopic
            {
                           TopicID = row.Field<int>("TopicID"),
                           CreatedDate = row.Field<System.DateTime>("Posted"),
                           Subject = row.Field<string>("Subject"),
                           StartedUserID = row.Field<int>("UserID"),
                           StartedUserName =
                               this.Get<IUserDisplayName>().GetName(row.Field<int>("UserID")),
                           Replies = row.Field<int>("Replies"),
                           LastPostDate = row.Field<System.DateTime>("LastPosted"),
                           LastUserID = row.Field<int>("LastUserID"),
                           LastUserName =
                               this.Get<IUserDisplayName>().GetName(row.Field<int>("LastUserID")),
                           LastMessageID = row.Field<int>("LastMessageID"),
                           FirstMessage = row.Field<string>("FirstMessage"),
                           LastMessage = this.GetRepository<Message>().GetById(row.Field<int>("LastMessageID")).MessageText,
                           Forum = forum
                       };
        }

        #endregion
    }
}