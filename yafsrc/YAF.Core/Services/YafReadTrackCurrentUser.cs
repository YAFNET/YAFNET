/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     YAF Read Tracking Methods
    /// </summary>
    public class YafReadTrackCurrentUser : IReadTrackCurrentUser
    {
        #region Fields

        /// <summary>
        ///     The _board settings.
        /// </summary>
        private readonly YafBoardSettings _boardSettings;

        private readonly IRepository<ForumReadTracking> _forumReadRepository;

        /// <summary>
        ///     The _session state.
        /// </summary>
        private readonly HttpSessionStateBase _sessionState;

        private readonly IRepository<TopicReadTracking> _topicReadRepository;

        /// <summary>
        ///     The _yaf session.
        /// </summary>
        private readonly IYafSession _yafSession;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YafReadTrackCurrentUser" /> class. The yaf read track current user.
        /// </summary>
        /// <param name="yafSession">The yaf session.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="sessionState">The session State.</param>
        /// <param name="forumReadRepository"></param>
        /// <param name="topicReadRepository"></param>
        public YafReadTrackCurrentUser(
            IYafSession yafSession,
            YafBoardSettings boardSettings,
            HttpSessionStateBase sessionState,
            IRepository<ForumReadTracking> forumReadRepository,
            IRepository<TopicReadTracking> topicReadRepository)
        {
            this._yafSession = yafSession;
            this._boardSettings = boardSettings;
            this._sessionState = sessionState;
            this._forumReadRepository = forumReadRepository;
            this._topicReadRepository = topicReadRepository;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the last visit.
        /// </summary>
        public DateTime LastRead
        {
            get
            {
                DateTime? lastRead = this._sessionState["LastRead"] == null
                                         ? null
                                         : this._sessionState["LastRead"].ToType<DateTime?>();

                if (!lastRead.HasValue && this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
                {
                    lastRead = LegacyDb.User_LastRead(this.CurrentUserID);
                }
                else
                {
                    lastRead = this._yafSession.LastVisit;
                }

                return lastRead ?? DateTimeHelper.SqlDbMinTime();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current user id.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return YafContext.Current.PageUserID;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this user is guest.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this user is guest; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGuest
        {
            get
            {
                return YafContext.Current.IsGuest;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get forum read.
        /// </summary>
        /// <param name="forumID">The forum ID of the Forum</param>
        /// <param name="readTimeOverride">The read Time Override.</param>
        /// <returns>
        ///     Returns the DateTime object from the Forum ID.
        /// </returns>
        public DateTime GetForumRead(int forumID, DateTime? readTimeOverride = null)
        {
            DateTime? readTime = this.GetSessionForumRead(forumID);

            if (!readTime.HasValue)
            {
                if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
                {
                    if (readTimeOverride.HasValue)
                    {
                        // use it if it's not the min value...
                        if (readTimeOverride.Value != DateTimeHelper.SqlDbMinTime())
                        {
                            readTime = readTimeOverride.Value;
                        }
                    }
                    else
                    {
                        // last option is to load from the forum...
                        readTime = this._forumReadRepository.Lastread(this.CurrentUserID, forumID);
                    }

                    // save value in session so that the db doesn't get called again...
                    this._yafSession.SetForumRead(forumID, readTime ?? DateTimeHelper.SqlDbMinTime());
                }
                else
                {
                    // use the last visit...
                    readTime = this.LastRead;
                }
            }

            return readTime ?? DateTimeHelper.SqlDbMinTime();
        }

        /// <summary>
        ///     The get topic read.
        /// </summary>
        /// <param name="topicID">The topicID you wish to find the DateTime object for.</param>
        /// <param name="readTimeOverride">The read Time Override.</param>
        /// <returns>
        ///     Returns the DateTime object from the topicID.
        /// </returns>
        public DateTime GetTopicRead(int topicID, DateTime? readTimeOverride = null)
        {
            DateTime? readTime = this.GetSessionTopicRead(topicID);

            if (!readTime.HasValue)
            {
                if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
                {
                    if (readTimeOverride.HasValue)
                    {
                        // use it if it's not the min value...
                        if (readTimeOverride.Value != DateTimeHelper.SqlDbMinTime())
                        {
                            readTime = readTimeOverride.Value;
                        }
                    }
                    else
                    {
                        // last option is to load from the forum...
                        readTime = this._topicReadRepository.Lastread(this.CurrentUserID, topicID);
                    }

                    // save value in session so that the db doesn't get called again...
                    this._yafSession.SetTopicRead(topicID, readTime ?? DateTimeHelper.SqlDbMinTime());
                }
                else
                {
                    // use last visit...
                    readTime = this.LastRead;
                }
            }

            return readTime ?? DateTimeHelper.SqlDbMinTime();
        }

        /// <summary>
        ///     The set forum read.
        /// </summary>
        /// <param name="forumID">The forum ID of the Forum</param>
        public void SetForumRead(int forumID)
        {
            if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
            {
                this._forumReadRepository.AddOrUpdate(this.CurrentUserID, forumID);
            }

            this._yafSession.SetForumRead(forumID, DateTime.UtcNow);
        }

        /// <summary>
        ///     The set topic read.
        /// </summary>
        /// <param name="topicID">The topic id to mark read.</param>
        public void SetTopicRead(int topicID)
        {
            if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
            {
                this._topicReadRepository.AddOrUpdate(this.CurrentUserID, topicID);
            }

            this._yafSession.SetTopicRead(topicID, DateTime.UtcNow);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the session forum read.
        /// </summary>
        /// <param name="forumId">The forum id.</param>
        /// <returns>
        ///     The get session forum read.
        /// </returns>
        private DateTime? GetSessionForumRead(int forumId)
        {
            Hashtable forumReadHashtable = this._yafSession.ForumRead;

            if (forumReadHashtable != null && forumReadHashtable.ContainsKey(forumId))
            {
                return forumReadHashtable[forumId].ToType<DateTime>();
            }

            return null;
        }

        /// <summary>
        ///     Gets the session topic read.
        /// </summary>
        /// <param name="topicId">The topic id.</param>
        /// <returns>
        ///     The get session topic read.
        /// </returns>
        private DateTime? GetSessionTopicRead(int topicId)
        {
            Hashtable topicReadHashtable = this._yafSession.TopicRead;

            if (topicReadHashtable != null && topicReadHashtable.ContainsKey(topicId))
            {
                return topicReadHashtable[topicId].ToType<DateTime>();
            }

            return null;
        }

        #endregion
    }
}