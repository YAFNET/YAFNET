/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using YAF.Utils.Helpers;

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// YAF Read Tracking Methods
    /// </summary>
    public class YafReadTrackCurrentUser : IReadTrackCurrentUser
    {
        #region Constants and Fields

        /// <summary>
        ///   The _board settings.
        /// </summary>
        private readonly YafBoardSettings _boardSettings;

        /// <summary>
        /// The _session state.
        /// </summary>
        private readonly HttpSessionStateBase _sessionState;

        /// <summary>
        ///   The _yaf session.
        /// </summary>
        private readonly IYafSession _yafSession;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafReadTrackCurrentUser"/> class. The yaf read track current user.
        /// </summary>
        /// <param name="yafSession">The yaf session.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="sessionState">The session State.</param>
        public YafReadTrackCurrentUser(
            IYafSession yafSession, YafBoardSettings boardSettings, HttpSessionStateBase sessionState)
        {
            this._yafSession = yafSession;
            this._boardSettings = boardSettings;
            this._sessionState = sessionState;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the last visit.
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
        ///   Gets the current user id.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return YafContext.Current.PageUserID;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this user is guest.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this user is guest; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGuest
        {
            get
            {
                return YafContext.Current.IsGuest;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get forum read.
        /// </summary>
        /// <param name="forumID">The forum ID of the Forum</param>
        /// <param name="readTimeOverride">The read Time Override.</param>
        /// <returns>
        /// Returns the DateTime object from the Forum ID.
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
                        if (readTimeOverride.Value >= DateTimeHelper.SqlDbMinTime())
                        {
                            readTime = readTimeOverride.Value;
                        }
                    }
                    else
                    {
                        // last option is to load from the forum...
                        readTime = LegacyDb.ReadForum_lastread(this.CurrentUserID, forumID);
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
       /// The get topic read.
       /// </summary>
       /// <param name="topicID">The topicID you wish to find the DateTime object for.</param>
       /// <param name="readTimeOverride">The read Time Override.</param>
       /// <returns>
       /// Returns the DateTime object from the topicID.
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
                        if (readTimeOverride.Value >= DateTimeHelper.SqlDbMinTime())
                        {
                            readTime = readTimeOverride.Value;
                        }
                    }
                    else
                    {
                        // last option is to load from the forum...
                        readTime = LegacyDb.Readtopic_lastread(this.CurrentUserID, topicID);
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
        /// The set forum read.
        /// </summary>
        /// <param name="forumID">The forum ID of the Forum</param>
        public void SetForumRead(int forumID)
        {
            if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
            {
                LegacyDb.ReadForum_AddOrUpdate(this.CurrentUserID, forumID);
            }

            this._yafSession.SetForumRead(forumID, DateTime.UtcNow);
        }

        /// <summary>
        /// The set topic read.
        /// </summary>
        /// <param name="topicID">The topic id to mark read.</param>
        public void SetTopicRead(int topicID)
        {
            if (this._boardSettings.UseReadTrackingByDatabase && !this.IsGuest)
            {
                LegacyDb.Readtopic_AddOrUpdate(this.CurrentUserID, topicID);
            }

            this._yafSession.SetTopicRead(topicID, DateTime.UtcNow);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the session forum read.
        /// </summary>
        /// <param name="forumId">The forum id.</param>
        /// <returns>
        /// The get session forum read.
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
        /// Gets the session topic read.
        /// </summary>
        /// <param name="topicId">The topic id.</param>
        /// <returns>
        /// The get session topic read.
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