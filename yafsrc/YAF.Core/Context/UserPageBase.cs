/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Core
{
    #region

    using System;
    using System.Data;
    using System.Threading;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// User Page Class.
    /// </summary>
    public class UserPageBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The _init user page.
        /// </summary>
        private bool _initUserPage;

        /// <summary>
        ///   The _page.
        /// </summary>
        private DataRow _page;

        /// <summary>
        ///   The _user flags.
        /// </summary>
        private UserFlags _userFlags;

        #endregion

        #region Events

        /// <summary>
        ///   The after init.
        /// </summary>
        public event EventHandler<EventArgs> AfterInit;

        /// <summary>
        ///   The before init.
        /// </summary>
        public event EventHandler<EventArgs> BeforeInit;

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
        public bool IsModerator
        {
            get
            {
                return this.PageValueAsBool("IsModerator");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the board is private (20050909 CHP) (True)
        /// </summary>
        public bool IsPrivate
        {
            get
            {
#if TODO
				try
				{
					return
						int.Parse(Utils.UtilsSection[string.Format("isprivate{0}", PageBoardID)])!=0;
				}
				catch
				{
					return false;
				}
#else
                return false;
#endif
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current user is suspended (True).
        /// </summary>
        public bool IsSuspended
        {
            get
            {
                return this.Page != null && this.Page["Suspended"] != DBNull.Value;
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
                           ? DateTime.MinValue
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
                           ? DateTime.MinValue
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
        public virtual DataRow Page
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
        ///   Gets the DateTime the user is suspended until
        /// </summary>
        public DateTime SuspendedUntil
        {
            get
            {
                return this.Page == null || this.Page["Suspended"] == DBNull.Value
                           ? DateTime.UtcNow
                           : Convert.ToDateTime(this.Page["Suspended"]);
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
        protected void InitUserAndPage()
        {
            if (!this._initUserPage)
            {
                try
                {
                    if (this.BeforeInit != null)
                    {
                        this.BeforeInit(this, new EventArgs());
                    }

                    DataRow pageRow;

                    // get the current user and update the user last access flag datetime.
                    MembershipUser user = UserMembershipHelper.GetUser(true);

                    if (user != null && YafContext.Current.Get<HttpSessionStateBase>()["UserUpdated"] == null)
                    {
                        RoleMembershipHelper.UpdateForumUser(user, this.PageBoardID);
                        YafContext.Current.Get<HttpSessionStateBase>()["UserUpdated"] = true;
                    }

                    string browser = "{0} {1}".FormatWith(
                        YafContext.Current.Get<HttpRequestBase>().Browser.Browser, 
                        YafContext.Current.Get<HttpRequestBase>().Browser.Version);
                    string platform = YafContext.Current.Get<HttpRequestBase>().Browser.Platform;

                    bool isSearchEngine;
                    bool dontTrack;

                    string userAgent = YafContext.Current.Get<HttpRequestBase>().UserAgent;
                    bool isMobileDevice = UserAgentHelper.IsMobileDevice(userAgent) ||
                                          YafContext.Current.Get<HttpRequestBase>().Browser.IsMobileDevice;

                    // try and get more verbose platform name by ref and other parameters             
                    UserAgentHelper.Platform(
                        userAgent, 
                        YafContext.Current.Get<HttpRequestBase>().Browser.Crawler, 
                        ref platform, 
                        ref browser, 
                        out isSearchEngine, 
                        out dontTrack);
                    dontTrack = !YafContext.Current.Get<YafBoardSettings>().ShowCrawlersInActiveList && isSearchEngine;

                    // don't track if this is a feed reader. May be to make it switchable in host settings.
                    // we don't have page 'g' token for the feed page.
                    if (browser.Contains("Unknown") && !dontTrack)
                    {
                        dontTrack = UserAgentHelper.IsFeedReader(userAgent);
                    }

                    int? categoryID =
                        ObjectExtensions.ValidInt(
                            YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("c"));
                    int? forumID =
                        ObjectExtensions.ValidInt(
                            YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"));
                    int? topicID =
                        ObjectExtensions.ValidInt(
                            YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"));
                    int? messageID =
                        ObjectExtensions.ValidInt(
                            YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

                    if (YafContext.Current.Settings.CategoryID != 0)
                    {
                        categoryID = YafContext.Current.Settings.CategoryID;
                    }

                    // vzrus: to log unhandled UserAgent strings
                    if (YafContext.Current.Get<YafBoardSettings>().UserAgentBadLog)
                    {
                        if (userAgent.IsNotSet())
                        {
                            LegacyDb.eventlog_create(null, this, "UserAgent string is empty.", EventLogTypes.Warning);
                        }
                        else
                        {
                            if (YafContext.Current.Get<HttpRequestBase>() != null &&
                                YafContext.Current.Get<HttpRequestBase>().Browser != null &&
                                platform.ToLower().Contains("unknown") || browser.ToLower().Contains("unknown"))
                            {
                                LegacyDb.eventlog_create(
                                    null, 
                                    this, 
                                    "Unhandled UserAgent string:'{0}' /r/nPlatform:'{1}' /r/nBrowser:'{2}' /r/nSupports cookies='{3}' /r/nSupports EcmaScript='{4}' /r/nUserID='{5}'."
                                        .FormatWith(
                                            userAgent, 
                                            YafContext.Current.Get<HttpRequestBase>().Browser.Platform, 
                                            YafContext.Current.Get<HttpRequestBase>().Browser.Browser, 
                                            YafContext.Current.Get<HttpRequestBase>().Browser.Cookies, 
                                            YafContext.Current.Get<HttpRequestBase>().Browser.EcmaScriptVersion.ToString(), 
                                            user != null ? user.UserName : String.Empty), 
                                    EventLogTypes.Warning);
                            }
                        }
                    }

                    object userKey = DBNull.Value;

                    if (user != null)
                    {
                        userKey = user.ProviderUserKey;
                    }

                    int tries = 0;

                    do
                    {
                        pageRow = LegacyDb.pageload(
                            YafContext.Current.Get<HttpSessionStateBase>().SessionID, 
                            this.PageBoardID, 
                            userKey, 
                            YafContext.Current.Get<HttpRequestBase>().UserHostAddress, 
                            YafContext.Current.Get<HttpRequestBase>().FilePath, 
                            YafContext.Current.Get<HttpRequestBase>().QueryString.ToString(), 
                            browser, 
                            platform, 
                            categoryID, 
                            forumID, 
                            topicID, 
                            messageID, 
                            // don't track if this is a search engine
                            isSearchEngine, 
                            isMobileDevice, 
                            dontTrack);

                        // if the user doesn't exist...
                        if (user != null && pageRow == null)
                        {
                            // create the user...
                            if (!RoleMembershipHelper.DidCreateForumUser(user, this.PageBoardID))
                            {
                                throw new ApplicationException("Failed to use new user.");
                            }

                            // if we've tried 5 times, they have no access
                            if (tries++ > 5)
                            {
                                // they have NO access -- they are a guest user.
                                userKey = DBNull.Value;
                                user = null;
                                tries = 0;
                            }
                        }
                        else if (tries++ > 5)
                        {
                            // fail...
                            break;
                        }
                    }
                    while (pageRow == null && user != null);

                    // page still hasn't been loaded...
                    if (pageRow == null)
                    {
                        throw new ApplicationException("Failed to find guest user.");
                    }

                    // We should be sure that all columns are added
                    DataRow auldRow;
                    do
                    {
                        auldRow = YafContext.Current.Get<IDBBroker>().ActiveUserLazyData((int)pageRow["UserID"]);

                        if (auldRow != null)
                        {
                            foreach (DataColumn col in auldRow.Table.Columns)
                            {
                                var dc = new DataColumn(col.ColumnName, col.DataType);
                                pageRow.Table.Columns.Add(dc);
                                pageRow.Table.Rows[0][dc] = auldRow[col];
                            }

                            pageRow.Table.AcceptChanges();
                        }

                        // vzrus: Current column count is 43 - change it if the total count changes
                        // TODO: THIS IS TERRIBLE CODE. FIX FIX FIX FIX FIX REMOVE FIX REMOVE FIX
                        // Lazy user!? This is LAZY PROGRAMMER!!
                    }
                    while (pageRow.Table.Columns.Count < 43);

                    // save this page data to the context...
                    // vzrus: it can be anywhere, but temporary is here. To reset active users cache if a new user is in the active list
                    if (Convert.ToBoolean(pageRow["ActiveUpdate"]))
                    {
                        YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
                    }

                    // vzrus: to log unhandled UserAgent strings
                    if (YafContext.Current.Get<YafBoardSettings>().UserAgentBadLog)
                    {
                        new UserAgentLogger().WriteLog(
                            userAgent, 
                            YafContext.Current.Get<HttpRequestBase>(), 
                            platform, 
                            browser, 
                            user != null ? user.UserName : string.Empty);
                    }

                    YafContext.Current.Vars["DontTrack"] = dontTrack;

                    this.Page = pageRow;

                    if (this.AfterInit != null)
                    {
                        this.AfterInit(this, new EventArgs());
                    }
                }
                catch (Exception x)
                {
#if !DEBUG

                    // log the exception...
                    LegacyDb.eventlog_create(null, "Failure Initializing User/Page.", x, EventLogTypes.Warning);

                    // log the user out...
                    FormsAuthentication.SignOut();

                    if (YafContext.Current.ForumPageType != ForumPages.info)
                    {
                        // show a failure notice since something is probably up with membership...
                        YafBuildLink.RedirectInfoPage(InfoMessage.Failure);
                    }
                    else
                    {
                        // totally failing... just re-throw the exception...
                        throw;
                    }

#else

    // re-throw exception...
          throw;
#endif
                }
            }
        }

        /// <summary>
        /// Helper function used for redundant "access" fields internally
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <returns>
        /// The access not null.
        /// </returns>
        private bool AccessNotNull(string field)
        {
            if (this.Page[field] == DBNull.Value)
            {
                return false;
            }

            return this.Page[field].ToType<int>() > 0;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (bool)
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <returns>
        /// The page value as bool.
        /// </returns>
        private bool PageValueAsBool(string field)
        {
            if (this.Page != null && this.Page[field] != DBNull.Value)
            {
                return this.Page[field].ToType<int>() != 0;
            }

            return false;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (int)
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <returns>
        /// The page value as int.
        /// </returns>
        private int PageValueAsInt(string field)
        {
            if (this.Page != null && this.Page[field] != DBNull.Value)
            {
                return this.Page[field].ToType<int>();
            }

            return 0;
        }

        /// <summary>
        /// Internal helper function used for redundant page variable access (string)
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <returns>
        /// The page value as string.
        /// </returns>
        private string PageValueAsString(string field)
        {
            if (this.Page != null && this.Page[field] != DBNull.Value)
            {
                return this.Page[field].ToString();
            }

            return string.Empty;
        }

        #endregion
    }
}