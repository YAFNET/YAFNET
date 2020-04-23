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
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
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
        /// Initializes a new instance of the <see cref="DataBroker" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="httpSessionState">The http session state.</param>
        /// <param name="dataCache">The data cache.</param>
        /// <param name="dbFunction">The database function.</param>
        public DataBroker(
            IServiceLocator serviceLocator,
            BoardSettings boardSettings,
            HttpSessionStateBase httpSessionState,
            IDataCache dataCache,
            IDbFunction dbFunction)
        {
            this.ServiceLocator = serviceLocator;
            this.BoardSettings = boardSettings;
            this.HttpSessionState = httpSessionState;
            this.DataCache = dataCache;
            this.DbFunction = dbFunction;
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
        /// Gets or sets the database function.
        /// </summary>
        /// <value>
        /// The database function.
        /// </value>
        public IDbFunction DbFunction { get; set; }

        /// <summary>
        ///     Gets or sets HttpSessionState.
        /// </summary>
        public HttpSessionStateBase HttpSessionState { get; set; }

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
        ///     Adds the Thanks info to a dataTable
        /// </summary>
        /// <param name="dataRows"> The data Rows. </param>
        public void AddThanksInfo(IEnumerable<DataRow> dataRows)
        {
            var messageIds = dataRows.Select(x => x.Field<int>("MessageID"));

            // Initialize the "IsthankedByUser" column.
            dataRows.ForEach(x => x["IsThankedByUser"] = false);

            // Initialize the "Thank Info" column.
            dataRows.ForEach(x => x["ThanksInfo"] = string.Empty);

            // Iterate through all the thanks relating to this topic and make appropriate
            // changes in columns.
            var allThanks = this.GetRepository<Thanks>().MessageGetAllThanks(messageIds.ToDelimitedString(",")).ToList();

            foreach (var f in
                allThanks.Where(t => t.FromUserID != null && t.FromUserID == BoardContext.Current.PageUserID)
                    .SelectMany(thanks => dataRows.Where(x => x.Field<int>("MessageID") == thanks.MessageID)))
            {
                f["IsThankedByUser"] = "true";
                f.AcceptChanges();
            }

            var thanksFieldNames = new[] { "ThanksFromUserNumber", "ThanksToUserNumber", "ThanksToUserPostsNumber" };

            foreach (var postRow in dataRows)
            {
                var messageId = postRow.Field<int>("MessageID");

                postRow["MessageThanksNumber"] = allThanks.Count(t => t.FromUserID != null && t.MessageID == messageId);

                var thanksFiltered = allThanks.Where(t => t.MessageID == messageId);

                if (thanksFiltered.Any())
                {
                    var thanksItem = thanksFiltered.First();

                    postRow["ThanksFromUserNumber"] = thanksItem.ThanksFromUserNumber ?? 0;
                    postRow["ThanksToUserNumber"] = thanksItem.ThanksToUserNumber ?? 0;
                    postRow["ThanksToUserPostsNumber"] = thanksItem.ThanksToUserPostsNumber ?? 0;
                }
                else
                {
                    var row = postRow;
                    thanksFieldNames.ForEach(f => row[f] = 0);
                }

                // load all all thanks info into a special column...
                postRow["ThanksInfo"] =
                    thanksFiltered.Where(t => t.FromUserID != null)
                        .Select(x => $"{x.FromUserID.Value}|{x.ThanksDate}")
                        .ToDelimitedString(",");

                postRow.AcceptChanges();
            }
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
                                new DataColumn("Style", typeof(string)), new DataColumn("IsGroup", typeof(bool))
                            });
                }

                // insert it into this DataSet
                ds.Tables.Add(moderator.Copy());

                // get the Category Table
                var category = this.DataCache.GetOrSet(
                    Constants.Cache.ForumCategory,
                    () =>
                        {
                            var catDt = this.DbFunction.GetAsDataTable(cdb => cdb.category_list(boardID, null));
                            catDt.TableName = "Category";
                            return catDt;
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
        ///     The favorite topic list.
        /// </summary>
        /// <param name="userID"> The user ID. </param>
        /// <returns> Returns The favorite topic list. </returns>
        public List<int> FavoriteTopicList(int userID)
        {
            var key = this.Get<ITreatCacheKey>().Treat(string.Format(Constants.Cache.FavoriteTopicList, userID));

            // stored in the user session...

            // was it in the cache?
            if (this.HttpSessionState[key] is List<int> favoriteTopicList)
            {
                return favoriteTopicList;
            }

            // get fresh values
            var favoriteTopics = this.GetRepository<YAF.Types.Models.FavoriteTopic>().Get(f => f.UserID == userID).Select(f => f.TopicID);

            // convert to list...
            favoriteTopicList = favoriteTopics.ToList();

            // store it in the user session...
            this.HttpSessionState.Add(key, favoriteTopicList);

            return favoriteTopicList;
        }

        /// <summary>
        /// The get active list.
        /// </summary>
        /// <param name="guests">The guests.</param>
        /// <param name="crawlers">The bots.</param>
        /// <param name="activeTime">The active time.</param>
        /// <returns>
        /// Returns the active list.
        /// </returns>
        public DataTable GetActiveList(bool guests, bool crawlers, int? activeTime = null)
        {
            return this.GetRepository<Active>()
                .List(
                    guests,
                    crawlers,
                    activeTime ?? this.BoardSettings.ActiveListTime,
                    this.BoardSettings.UseStyledNicks);
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
                        row["IsGroup"].ToType<bool>())).ToList();
        }

        /// <summary>
        ///     The get custom bb code.
        /// </summary>
        /// <returns> Returns List with Custom BBCodes </returns>
        public IEnumerable<BBCode> GetCustomBBCode()
        {
            return this.DataCache.GetOrSet(Constants.Cache.CustomBBCode, () => this.GetRepository<BBCode>().GetByBoardId());
        }

        /// <summary>
        ///     Get all moderators by Groups and User
        /// </summary>
        /// <returns> Returns the Moderator List </returns>
        public DataTable GetModerators()
        {
            return this.DataCache.GetOrSet(
                Constants.Cache.ForumModerators,
                () =>
                    {
                        var moderator =
                            this.DbFunction.GetAsDataTable(
                                cdb => cdb.forum_moderators(BoardContext.Current.PageBoardID, this.BoardSettings.UseStyledNicks));
                        moderator.TableName = "Moderator";
                        return moderator;
                    },
                TimeSpan.FromMinutes(this.Get<BoardSettings>().BoardModeratorsCacheTimeout));
        }

        /// <summary>
        /// Get the list of recently logged in users.
        /// </summary>
        /// <param name="timeSinceLastLogin">The time since last login in minutes.</param>
        /// <returns>
        /// The list of users in Data table format.
        /// </returns>
        public DataTable GetRecentUsers(int timeSinceLastLogin)
        {
            return
                this.StyleTransformDataTable(
                    this.DbFunction.GetAsDataTable(
                        cdb =>
                        cdb.recent_users(
                            BoardContext.Current.PageBoardID,
                            timeSinceLastLogin,
                            this.BoardSettings.UseStyledNicks)));
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

        /// <summary>
        ///     Loads the message text into the paged data if "Message" and
        ///     "MessageID" exists.
        /// </summary>
        /// <param name="searchResults"> The data Rows. </param>
        public void LoadMessageText(IEnumerable<SearchResult> searchResults)
        {
            var results = searchResults as SearchResult[] ?? searchResults.ToArray();

            var messageIds = results.Where(x => x.Message.IsNotSet()).Select(x => x.MessageID);

            var messageTextTable =
                this.DbFunction.GetAsDataTable(cdb => cdb.message_GetTextByIds(messageIds.ToDelimitedString(",")));

            if (messageTextTable == null)
            {
                return;
            }

            // load them into the page data...
            results.ForEach(
                r =>
                    {
                        // find the message id in the results...
                        var message = messageTextTable.AsEnumerable()
                            .FirstOrDefault(x => x.Field<int>("MessageID") == r.MessageID);

                        if (message != null)
                        {
                            r.Message = message.Field<string>("Message");
                        }
                    });
        }

        /// <summary>
        ///     The style transform function wrap.
        /// </summary>
        /// <param name="dt"> The DateTable </param>
        /// <returns> The style transform wrap. </returns>
        public DataTable StyleTransformDataTable(DataTable dt)
        {
            if (!this.BoardSettings.UseStyledNicks)
            {
                return dt;
            }

            var styleTransform = this.Get<IStyleTransform>();
            styleTransform.DecodeStyleByTable(dt, true);

            return dt;
        }

        /// <summary>
        ///     The Buddy list for the user with the specified UserID.
        /// </summary>
        /// <param name="userID"> The User ID. </param>
        /// <returns> The user buddy list. </returns>
        public DataTable UserBuddyList(int userID)
        {
            return this.DataCache.GetOrSet(
                string.Format(Constants.Cache.UserBuddies, userID),
                () => this.DbFunction.GetAsDataTable(cdb => cdb.buddy_list(userID)),
                TimeSpan.FromMinutes(10));
        }

        /// <summary>
        ///     The user ignored list.
        /// </summary>
        /// <param name="userId"> The user id. </param>
        /// <returns> Returns the user ignored list. </returns>
        public List<int> UserIgnoredList(int userId)
        {
            var key = string.Format(Constants.Cache.UserIgnoreList, userId);

            // stored in the user session...

            // was it in the cache?
            if (this.HttpSessionState[key] is List<int> userList)
            {
                return userList;
            }
            else
            {
                userList = new List<int>();
            }

            // get fresh values
            this.GetRepository<IgnoreUser>().Get(i => i.UserID == userId).ForEach(user => userList.Add(user.IgnoredUserID));

            // store it in the user session...
            this.HttpSessionState.Add(key, userList);

            return userList;
        }

        /// <summary>
        ///     The user medals.
        /// </summary>
        /// <param name="userId"> The user id. </param>
        /// <returns> Returns the User Medals </returns>
        public DataTable UserMedals(int userId)
        {
            var key = string.Format(Constants.Cache.UserMedals, userId);

            // get the medals cached...
            var dt = this.DataCache.GetOrSet(
                key,
                () => this.DbFunction.GetAsDataTable(cdb => cdb.user_listmedals(userId)),
                TimeSpan.FromMinutes(10));

            return dt;
        }

        #endregion

        #region Methods

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
                               UserMembershipHelper.GetDisplayNameFromID(row.Field<int>("UserID")),
                           Replies = row.Field<int>("Replies"),
                           LastPostDate = row.Field<System.DateTime>("LastPosted"),
                           LastUserID = row.Field<int>("LastUserID"),
                           LastUserName =
                               UserMembershipHelper.GetDisplayNameFromID(row.Field<int>("LastUserID")),
                           LastMessageID = row.Field<int>("LastMessageID"),
                           FirstMessage = row.Field<string>("FirstMessage"),
                           LastMessage = this.GetRepository<Message>().MessageList(row.Field<int>("LastMessageID")).First().Message,
                           Forum = forum
                       };
        }

        #endregion
    }
}