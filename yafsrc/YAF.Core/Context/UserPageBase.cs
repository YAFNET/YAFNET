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

namespace YAF.Core.Context
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading;

    using YAF.Configuration;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// User Page Class.
    /// </summary>
    public abstract class UserPageBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The init. user page.
        /// </summary>
        protected bool InitUserPage;

        /// <summary>
        /// The page
        /// </summary>
        private IDictionary<string, object> page;

        /// <summary>
        ///   The user flags.
        /// </summary>
        private UserFlags userFlags;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether the current user has access to vote on polls in the current BoardVoteAccess (True).
        /// </summary>
        public bool BoardVoteAccess => this.AccessNotNull("BoardVoteAccess");

        /// <summary>
        ///   Gets the culture code for the user
        /// </summary>
        public string CultureUser => this.PageValueAsString("CultureUser");

        /// <summary>
        ///   Gets a value indicating whether the time zone offset for the user
        /// </summary>
        public bool DSTUser => this.userFlags != null && this.userFlags.IsDST;

        /// <summary>
        ///   Gets a value indicating whether the current user can delete own messages in the current forum (True).
        /// </summary>
        public bool ForumDeleteAccess => this.AccessNotNull("DeleteAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user can download attachments (True).
        /// </summary>
        public bool ForumDownloadAccess => this.AccessNotNull("DownloadAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user can edit own messages in the current forum (True).
        /// </summary>
        public bool ForumEditAccess => this.AccessNotNull("EditAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user is a moderator of the current forum (True).
        /// </summary>
        public bool ForumModeratorAccess => this.AccessNotNull("ModeratorAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has access to create polls in the current forum (True).
        /// </summary>
        public bool ForumPollAccess => this.AccessNotNull("PollAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has post access in the current forum (True).
        /// </summary>
        public bool ForumPostAccess => this.AccessNotNull("PostAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has access to create priority topics in the current forum (True).
        /// </summary>
        public bool ForumPriorityAccess => this.AccessNotNull("PriorityAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has read access in the current forum (True)
        /// </summary>
        public bool ForumReadAccess => this.AccessNotNull("ReadAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has reply access in the current forum (True)
        /// </summary>
        public bool ForumReplyAccess => this.AccessNotNull("ReplyAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user can upload attachments (True).
        /// </summary>
        public bool ForumUploadAccess => this.AccessNotNull("UploadAccess");

        /// <summary>
        ///   Gets a value indicating whether the current user has access to vote on polls in the current forum (True).
        /// </summary>
        public bool ForumVoteAccess => this.AccessNotNull("VoteAccess");

        /// <summary>
        ///   Gets a value indicating whether the  current user is an administrator (True).
        /// </summary>
        public bool IsAdmin => this.IsHostAdmin || this.PageValueAsBool("IsAdmin");

        /// <summary>
        ///   Gets a value indicating whether the user is excluded from CAPTCHA check (True).
        /// </summary>
        public bool IsCaptchaExcluded => this.userFlags != null && this.userFlags.IsCaptchaExcluded;

        /// <summary>
        /// The moderated.
        /// </summary>
        public bool Moderated => this.userFlags != null && this.userFlags.Moderated;

        /// <summary>
        ///   Gets a value indicating whether the current user IsCrawler (True).
        /// </summary>
        public bool IsCrawler => this.AccessNotNull("IsCrawler");

        /// <summary>
        ///   Gets a value indicating whether the current user is a forum moderator (mini-admin) (True).
        /// </summary>
        public bool IsForumModerator => this.PageValueAsBool("IsForumModerator");

        /// <summary>
        ///   Gets a value indicating whether the current user is a guest (True).
        /// </summary>
        public bool IsGuest => this.PageValueAsBool("IsGuest");

        /// <summary>
        ///   Gets a value indicating whether the current user host admin (True).
        /// </summary>
        public bool IsHostAdmin => this.userFlags != null && this.userFlags.IsHostAdmin;

        /// <summary>
        ///   Gets a value indicating whether the current user uses a mobile device (True).
        /// </summary>
        public bool IsMobileDevice => this.AccessNotNull("IsMobileDevice");

        /// <summary>
        ///   Gets a value indicating whether the current user is a moderator for at least one forum (True);
        /// </summary>
        public bool IsModeratorInAnyForum =>
            this.PageValueAsBool("IsModerator") || this.PageValueAsBool("IsModeratorAny");

        /// <summary>
        ///   Gets a value indicating whether the current user personal data was changed and not handled by a code;
        /// </summary>
        public bool IsDirty => this.PageValueAsBool("IsDirty");

        /// <summary>
        ///   Gets a value indicating whether the current user is logged in via Facebook
        /// </summary>
        public bool IsFacebookUser => this.PageValueAsBool("IsFacebookUser");

        /// <summary>
        ///   Gets a value indicating whether the current user is logged in via Twitter
        /// </summary>
        public bool IsTwitterUser => this.PageValueAsBool("IsTwitterUser");

        /// <summary>
        ///   Gets a value indicating whether the current user is logged in via Google
        /// </summary>
        public bool IsGoogleUser => this.PageValueAsBool("IsGoogleUser");

        /// <summary>
        ///   Gets a value indicating whether the current user is suspended (True).
        /// </summary>
        public bool IsSuspended => this.Page?["Suspended"] is DateTime;

        /// <summary>
        ///   Gets the language file name for the user
        /// </summary>
        public string LanguageFile => this.PageValueAsString("LanguageFile");

        /// <summary>
        ///   Gets the number of pending buddy requests.
        /// </summary>
        public DateTime LastPendingBuddies => this.Page["LastPendingBuddies"].ToString().IsNotSet()
                                                  ? DateTimeHelper.SqlDbMinTime()
                                                  : Convert.ToDateTime(this.Page["LastPendingBuddies"]);

        /// <summary>
        ///   Gets LastUnreadPm.
        /// </summary>
        public DateTime LastUnreadPm => this.Page["LastUnreadPm"].ToString().IsNotSet()
                                            ? DateTimeHelper.SqlDbMinTime()
                                            : Convert.ToDateTime(this.Page["LastUnreadPm"]);

        /// <summary>
        ///   Gets the number of albums which a user already has
        /// </summary>
        public int NumAlbums => this.Page["NumAlbums"].ToType<int>();

        /// <summary>
        ///   Gets or sets Page.
        /// </summary>
        public virtual IDictionary<string, object> Page
        {
            get
            {
                if (this.InitUserPage)
                {
                    return this.page;
                }

                if (!Monitor.TryEnter(this))
                {
                    return this.page;
                }

                try
                {
                    if (!this.InitUserPage)
                    {
                        this.InitUserAndPage();
                    }
                }
                finally
                {
                    Monitor.Exit(this);
                }

                return this.page;
            }

            set
            {
                this.page = value;
                this.InitUserPage = value != null;

                // get user flags
                this.userFlags = this.page != null ? new UserFlags(this.page["UserFlags"]) : null;
            }
        }

        /// <summary>
        ///   Gets PageBoardID.
        /// </summary>
        public int PageBoardID => ControlSettings.Current == null ? 1 : ControlSettings.Current.BoardID;

        /// <summary>
        ///   Gets the CategoryID for the current page, or 0 if not in any category
        /// </summary>
        public int PageCategoryID => BoardContext.Current.Settings.CategoryID != 0
                                         ? BoardContext.Current.Settings.CategoryID
                                         : this.PageValueAsInt("CategoryID");

        /// <summary>
        ///   Gets the Name of category for the current page, or an empty string if not in any category
        /// </summary>
        public string PageCategoryName => this.PageValueAsString("CategoryName");

        /// <summary>
        ///   Gets the Parent ForumID for the current page, or 0 if not in any forum
        /// </summary>
        public int? PageParentForumID
        {
            get
            {
                var isLockedForum = BoardContext.Current.Settings.LockedForum;

                return isLockedForum != 0 ? isLockedForum : this.PageValueAsInt("ParentForumID");
            }
        }

        /// <summary>
        ///   Gets the ForumID for the current page, or 0 if not in any forum
        /// </summary>
        public int PageForumID
        {
            get
            {
                var isLockedForum = BoardContext.Current.Settings.LockedForum;

                return isLockedForum != 0 ? isLockedForum : this.PageValueAsInt("ForumID");
            }
        }

        /// <summary>
        ///   Gets the Name of forum for the current page, or an empty string if not in any forum
        /// </summary>
        public string PageForumName => this.PageValueAsString("ForumName");

        /// <summary>
        ///   Gets the  TopicID of the current page, or 0 if not in any topic
        /// </summary>
        public int PageTopicID => this.PageValueAsInt("TopicID");

        /// <summary>
        ///   Gets the Name of topic for the current page, or an empty string if not in any topic
        /// </summary>
        public string PageTopicName => this.PageValueAsString("TopicName");

        /// <summary>
        ///   Gets the UserID of the current user.
        /// </summary>
        public int PageUserID => this.PageValueAsInt("UserID");

        /// <summary>
        ///   Gets PageUserName.
        /// </summary>
        public string PageUserName => this.PageValueAsString("UserName");

        /// <summary>
        ///   Gets the number of pending buddy requests
        /// </summary>
        public int PendingBuddies => this.Page["PendingBuddies"].ToType<int>();

        /// <summary>
        ///   Gets the number of Reputation Points
        /// </summary>
        public int Reputation => this.Page["Reputation"].ToType<int>();

        /// <summary>
        ///   Gets the DateTime the user is suspended until
        /// </summary>
        public DateTime SuspendedUntil =>
            this.IsSuspended ? this.Page["Suspended"].ToType<DateTime>() : DateTime.UtcNow;

        /// <summary>
        ///   Gets the DateTime the user is suspended until
        /// </summary>
        public string SuspendedReason => this.IsSuspended ? this.Page["SuspendedReason"].ToString() : string.Empty;

        /// <summary>
        ///   Gets the time zone offset for the user
        /// </summary>
        public string TimeZoneUser => this.Page["TimeZoneUser"].ToString();

        /// <summary>
        /// Gets the time zone information user.
        /// </summary>
        /// <value>
        /// The time zone information user.
        /// </value>
        public TimeZoneInfo TimeZoneInfoUser => DateTimeHelper.GetTimeZoneInfo(this.TimeZoneUser);

        /// <summary>
        /// Gets the time zone user off set.
        /// </summary>
        /// <value>
        /// The time zone user off set.
        /// </value>
        public int TimeZoneUserOffSet => DateTimeHelper.GetTimeZoneOffset(this.Page["TimeZoneUser"].ToString());

        /// <summary>
        /// The received thanks.
        /// </summary>
        public int ReceivedThanks => this.Page["ReceivedThanks"].ToType<int>();

        /// <summary>
        /// The mention.
        /// </summary>
        public int Mention => this.Page["Mention"].ToType<int>();

        /// <summary>
        /// The quoted.
        /// </summary>
        public int Quoted => this.Page["Quoted"].ToType<int>();

        /// <summary>
        ///   Gets the number of private messages that are unread
        /// </summary>
        public int UnreadPrivate => this.Page["UnreadPrivate"].ToType<int>();

        /// <summary>
        ///   Gets the number of posts that needs moderating
        /// </summary>
        public int ModeratePosts => this.Page["ModeratePosts"].ToType<int>();

        /// <summary>
        ///   Gets a value indicating whether a user has buddies
        /// </summary>
        public bool UserHasBuddies => this.PageValueAsBool("UserHasBuddies");

        /// <summary>
        ///   Gets the UserStyle for the user
        /// </summary>
        public string UserStyle => this.PageValueAsString("UserStyle");

        /// <summary>
        ///   Gets the number of albums which a user can have
        /// </summary>
        public int UsrAlbums => this.Page["UsrAlbums"].ToType<int>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Helper function to see if the Page variable is populated
        /// </summary>
        /// <returns>
        /// The page is null.
        /// </returns>
        public bool PageIsNull()
        {
            return this.Page == null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the user data and page data...
        /// </summary>
        protected virtual void InitUserAndPage()
        {
        }

        /// <summary>
        /// Helper function used for redundant "access" fields internally
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The access not null.
        /// </returns>
        private bool AccessNotNull(string field)
        {
            if (this.Page[field] == null)
            {
                return false;
            }

            return this.Page[field].ToType<int>() > 0;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (boolean)
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The page value as boolean.
        /// </returns>
        private bool PageValueAsBool(string field)
        {
            if (this.Page?[field] != null)
            {
                return this.Page[field].ToType<int>() != 0;
            }

            return false;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (integer)
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The page value as integer.
        /// </returns>
        private int PageValueAsInt(string field)
        {
            if (this.Page?[field] != null)
            {
                return this.Page[field].ToType<int>();
            }

            return 0;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (string)
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The page value as string.
        /// </returns>
        private string PageValueAsString(string field)
        {
            return this.Page?[field] != null ? this.Page[field].ToString() : string.Empty;
        }

        #endregion
    }
}