/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty fo
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading;

    using YAF.Classes;
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
        ///   The _init user page.
        /// </summary>
        protected bool _initUserPage;

        /// <summary>
        ///   The _page.
        /// </summary>
        protected IDictionary<string, object> _page;

        /// <summary>
        ///   The _user flags.
        /// </summary>
        protected UserFlags _userFlags;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether the current user has access to vote on polls in the current BoardVoteAccess (True).
        /// </summary>
        public bool BoardVoteAccess
        {
            get
            {
                return this.AccessNotNull("BoardVoteAccess");
            }
        }

        /// <summary>
        ///   Gets the culture code for the user
        /// </summary>
        public string CultureUser
        {
            get
            {
                return this.PageValueAsString("CultureUser");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the time zone offset for the user
        /// </summary>
        public bool DSTUser
        {
            get
            {
                return this._userFlags != null && this._userFlags.IsDST;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user can delete own messages in the current forum (True).
        /// </summary>
        public bool ForumDeleteAccess
        {
            get
            {
                return this.AccessNotNull("DeleteAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user can download attachments (True).
        /// </summary>
        public bool ForumDownloadAccess
        {
            get
            {
                return this.AccessNotNull("DownloadAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user can edit own messages in the current forum (True).
        /// </summary>
        public bool ForumEditAccess
        {
            get
            {
                return this.AccessNotNull("EditAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is a moderator of the current forum (True).
        /// </summary>
        public bool ForumModeratorAccess
        {
            get
            {
                return this.AccessNotNull("ModeratorAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has access to create polls in the current forum (True).
        /// </summary>
        public bool ForumPollAccess
        {
            get
            {
                return this.AccessNotNull("PollAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has post access in the current forum (True).
        /// </summary>
        public bool ForumPostAccess
        {
            get
            {
                return this.AccessNotNull("PostAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has access to create priority topics in the current forum (True).
        /// </summary>
        public bool ForumPriorityAccess
        {
            get
            {
                return this.AccessNotNull("PriorityAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has read access in the current forum (True)
        /// </summary>
        public bool ForumReadAccess
        {
            get
            {
                return this.AccessNotNull("ReadAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has reply access in the current forum (True)
        /// </summary>
        public bool ForumReplyAccess
        {
            get
            {
                return this.AccessNotNull("ReplyAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user can upload attachments (True).
        /// </summary>
        public bool ForumUploadAccess
        {
            get
            {
                return this.AccessNotNull("UploadAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user has access to vote on polls in the current forum (True).
        /// </summary>
        public bool ForumVoteAccess
        {
            get
            {
                return this.AccessNotNull("VoteAccess");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the  current user is an administrator (True).
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                return this.IsHostAdmin || this.PageValueAsBool("IsAdmin");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the user is excluded from CAPTCHA check (True).
        /// </summary>
        public bool IsCaptchaExcluded
        {
            get
            {
                return this._userFlags != null && this._userFlags.IsCaptchaExcluded;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user IsCrawler (True).
        /// </summary>
        public bool IsCrawler
        {
            get
            {
                return this.AccessNotNull("IsCrawler");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is a forum moderator (mini-admin) (True).
        /// </summary>
        public bool IsForumModerator
        {
            get
            {
                return this.PageValueAsBool("IsForumModerator");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is a guest (True).
        /// </summary>
        public bool IsGuest
        {
            get
            {
                return this.PageValueAsBool("IsGuest");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user host admin (True).
        /// </summary>
        public bool IsHostAdmin
        {
            get
            {
                return this._userFlags != null && this._userFlags.IsHostAdmin;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user uses a mobile device (True).
        /// </summary>
        public bool IsMobileDevice
        {
            get
            {
                return this.AccessNotNull("IsMobileDevice");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is a modeator for at least one forum (True);
        /// </summary>
        public bool IsModeratorInAnyForum
        {
            get
            {
                return this.PageValueAsBool("IsModerator");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user personal data was changed and not handled by a code;
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return this.PageValueAsBool("IsDirty");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is logged in via Facebook
        /// </summary>
        public bool IsFacebookUser
        {
            get
            {
                return this.PageValueAsBool("IsFacebookUser");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is logged in via Twitter
        /// </summary>
        public bool IsTwitterUser
        {
            get
            {
                return this.PageValueAsBool("IsTwitterUser");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is suspended (True).
        /// </summary>
        public bool IsSuspended
        {
            get
            {
                return this.Page != null && this.Page["Suspended"] is DateTime;
            }
        }

        /// <summary>
        ///   Gets the language file name for the user
        /// </summary>
        public string LanguageFile
        {
            get
            {
                return this.PageValueAsString("LanguageFile");
            }
        }

        /// <summary>
        ///   Gets the number of pending buddy requests.
        /// </summary>
        public DateTime LastPendingBuddies
        {
            get
            {
                return this.Page["LastPendingBuddies"].ToString().IsNotSet()
                           ? DateTimeHelper.SqlDbMinTime()
                           : Convert.ToDateTime(this.Page["LastPendingBuddies"]);
            }
        }

        /// <summary>
        ///   Gets LastUnreadPm.
        /// </summary>
        public DateTime LastUnreadPm
        {
            get
            {
                return this.Page["LastUnreadPm"].ToString().IsNotSet()
                           ? DateTimeHelper.SqlDbMinTime()
                           : Convert.ToDateTime(this.Page["LastUnreadPm"]);
            }
        }

        /// <summary>
        ///   Gets the number of albums which a user already has
        /// </summary>
        public int NumAlbums
        {
            get
            {
                return this.Page["NumAlbums"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets or sets Page.
        /// </summary>
        public virtual IDictionary<string, object> Page
        {
            get
            {
                if (!this._initUserPage)
                {
                    if (Monitor.TryEnter(this))
                    {
                        try
                        {
                            if (!this._initUserPage)
                            {
                                this.InitUserAndPage();
                            }
                        }
                        finally
                        {
                            Monitor.Exit(this);
                        }
                    }
                }

                return this._page;
            }

            set
            {
                this._page = value;
                this._initUserPage = value != null;

                // get user flags
                this._userFlags = this._page != null ? new UserFlags(this._page["UserFlags"]) : null;
            }
        }

        /// <summary>
        ///   Gets PageBoardID.
        /// </summary>
        public int PageBoardID
        {
            get
            {
                return YafControlSettings.Current == null ? 1 : YafControlSettings.Current.BoardID;
            }
        }

        /// <summary>
        ///   Gets the CategoryID for the current page, or 0 if not in any category
        /// </summary>
        public int PageCategoryID
        {
            get
            {
                return YafContext.Current.Settings.CategoryID != 0
                           ? YafContext.Current.Settings.CategoryID
                           : this.PageValueAsInt("CategoryID");
            }
        }

        /// <summary>
        ///   Gets the Name of category for the current page, or an empty string if not in any category
        /// </summary>
        public string PageCategoryName
        {
            get
            {
                return this.PageValueAsString("CategoryName");
            }
        }

        /// <summary>
        ///   Gets the ForumID for the current page, or 0 if not in any forum
        /// </summary>
        public int PageForumID
        {
            get
            {
                int isLockedForum = YafContext.Current.Settings.LockedForum;

                return isLockedForum != 0 ? isLockedForum : this.PageValueAsInt("ForumID");
            }
        }

        /// <summary>
        ///   Gets the Name of forum for the current page, or an empty string if not in any forum
        /// </summary>
        public string PageForumName
        {
            get
            {
                return this.PageValueAsString("ForumName");
            }
        }

        /// <summary>
        ///   Gets the  TopicID of the current page, or 0 if not in any topic
        /// </summary>
        public int PageTopicID
        {
            get
            {
                return this.PageValueAsInt("TopicID");
            }
        }

        /// <summary>
        ///   Gets the Name of topic for the current page, or an empty string if not in any topic
        /// </summary>
        public string PageTopicName
        {
            get
            {
                return this.PageValueAsString("TopicName");
            }
        }

        /// <summary>
        ///   Gets the UserID of the current user.
        /// </summary>
        public int PageUserID
        {
            get
            {
                return this.PageValueAsInt("UserID");
            }
        }

        /// <summary>
        ///   Gets PageUserName.
        /// </summary>
        public string PageUserName
        {
            get
            {
                return this.PageValueAsString("UserName");
            }
        }

        /// <summary>
        ///   Gets the number of pending buddy requests
        /// </summary>
        public int PendingBuddies
        {
            get
            {
                return this.Page["PendingBuddies"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets the number of Reputation Points
        /// </summary>
        public int Reputation
        {
            get
            {
                return this.Page["Reputation"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets the DateTime the user is suspended until
        /// </summary>
        public DateTime SuspendedUntil
        {
            get
            {
                return this.IsSuspended ? Convert.ToDateTime(this.Page["Suspended"]).ToUniversalTime() : DateTime.UtcNow;
            }
        }

        /// <summary>
        ///   Gets the user text editor
        /// </summary>
        public string TextEditor
        {
            get
            {
                return this.PageValueAsString("TextEditor");
            }
        }

        /// <summary>
        ///   Gets the time zone offset for the user
        /// </summary>
        public int TimeZoneUser
        {
            get
            {
                return this.Page["TimeZoneUser"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets the number of private messages that are unread
        /// </summary>
        public int UnreadPrivate
        {
            get
            {
                return this.Page["UnreadPrivate"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets a value indicating whether a user has buddies
        /// </summary>
        public bool UserHasBuddies
        {
            get
            {
                return this.PageValueAsBool("UserHasBuddies");
            }
        }

        /// <summary>
        ///   Gets the UserStyle for the user
        /// </summary>
        public string UserStyle
        {
            get
            {
                return this.PageValueAsString("UserStyle");
            }
        }

        /// <summary>
        ///   Gets the number of albums which a user can have
        /// </summary>
        public int UsrAlbums
        {
            get
            {
                return this.Page["UsrAlbums"].ToType<int>();
            }
        }

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
            if (this.Page != null && this.Page[field] != null)
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
            if (this.Page != null && this.Page[field] != null)
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
            if (this.Page != null && this.Page[field] != null)
            {
                return this.Page[field].ToString();
            }

            return string.Empty;
        }

        #endregion
    }
}