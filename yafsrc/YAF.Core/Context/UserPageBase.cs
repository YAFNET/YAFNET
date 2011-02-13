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
  using System;
  using System.Data;
  using System.Threading;
  using System.Web;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers;
  using YAF.Core.Services;
  using YAF.Types.Constants;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;

  /// <summary>
  /// User Page Class.
  /// </summary>
  public partial class UserPageBase
  {
    /// <summary>
    /// The _init user page.
    /// </summary>
    private bool _initUserPage = false;

    /// <summary>
    /// The _page.
    /// </summary>
    private DataRow _page = null;

    /// <summary>
    /// The _user flags.
    /// </summary>
    private UserFlags _userFlags = null;

    /// <summary>
    /// Gets or sets Page.
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
                InitUserAndPage();
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
    /// The before init.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    /// The after init.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    /// Helper function to see if the Page variable is populated
    /// </summary>
    /// <returns>
    /// The page is null.
    /// </returns>
    public bool PageIsNull()
    {
      return Page == null;
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
      if (Page[field] == DBNull.Value)
      {
        return false;
      }

      return Convert.ToInt32(Page[field]) > 0;
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
      if (Page != null && Page[field] != DBNull.Value)
      {
        return Convert.ToInt32(Page[field]) != 0;
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
      if (Page != null && Page[field] != DBNull.Value)
      {
        return Convert.ToInt32(Page[field]);
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
      if (Page != null && Page[field] != DBNull.Value)
      {
        return Page[field].ToString();
      }

      return string.Empty;
    }

    #region Forum and Page Helper Properties

    /// <summary>
    /// Gets a value indicating whether the current user has post access in the current forum (True).
    /// </summary>
    public bool ForumPostAccess
    {
      get
      {
        return AccessNotNull("PostAccess");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user has reply access in the current forum (True)
    /// </summary>
    public bool ForumReplyAccess
    {
      get
      {
        return AccessNotNull("ReplyAccess");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user has read access in the current forum (True)
    /// </summary>
    public bool ForumReadAccess
    {
      get
      {
        return AccessNotNull("ReadAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user has access to create priority topics in the current forum (True).
    /// </summary>
    public bool ForumPriorityAccess
    {
      get
      {
        return AccessNotNull("PriorityAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user has access to create polls in the current forum (True).
    /// </summary>
    public bool ForumPollAccess
    {
      get
      {
        return AccessNotNull("PollAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user has access to vote on polls in the current forum (True).
    /// </summary>
    public bool ForumVoteAccess
    {
      get
      {
        return AccessNotNull("VoteAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user has access to vote on polls in the current BoardVoteAccess (True).
    /// </summary>
    public bool BoardVoteAccess
    {
      get
      {
        return AccessNotNull("BoardVoteAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user is a moderator of the current forum (True).
    /// </summary>
    public bool ForumModeratorAccess
    {
      get
      {
        return AccessNotNull("ModeratorAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user can delete own messages in the current forum (True).
    /// </summary>
    public bool ForumDeleteAccess
    {
      get
      {
        return AccessNotNull("DeleteAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user can edit own messages in the current forum (True).
    /// </summary>
    public bool ForumEditAccess
    {
      get
      {
        return AccessNotNull("EditAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user can upload attachments (True).
    /// </summary>
    public bool ForumUploadAccess
    {
      get
      {
        return AccessNotNull("UploadAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user can download attachments (True).
    /// </summary>
    public bool ForumDownloadAccess
    {
      get
      {
        return AccessNotNull("DownloadAccess");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user IsCrawler (True).
    /// </summary>
    public bool IsCrawler
    {
      get
      {
        return AccessNotNull("IsCrawler");
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the current user uses a mobile device (True).
    /// </summary>
    public bool IsMobileDevice
    {
      get
      {
        return AccessNotNull("IsMobileDevice");
      }
    }

    /// <summary>
    /// Gets PageBoardID.
    /// </summary>
    public int PageBoardID
    {
      get
      {
        return YafControlSettings.Current == null ? 1 : YafControlSettings.Current.BoardID;
      }
    }

    /// <summary>
    /// Gets PageBoardUID.
    /// </summary>
    public Guid PageBoardUid
    {
      get
      {
        return YafContext.Current.Settings == null ? Guid.Empty : YafContext.Current.Settings.BoardUid;
      }
    }

    /// <summary>
    /// Gets the UserID of the current user.
    /// </summary>
    public int PageUserID
    {
      get
      {
        return PageValueAsInt("UserID");
      }
    }

    /// <summary>
    /// Gets PageUserName.
    /// </summary>
    public string PageUserName
    {
      get
      {
        return PageValueAsString("UserName");
      }
    }

    /// <summary>
    /// Gets the ForumID for the current page, or 0 if not in any forum
    /// </summary>
    public int PageForumID
    {
      get
      {
        int isLockedForum = YafContext.Current.Settings.LockedForum;
        if (isLockedForum != 0)
        {
          return isLockedForum;
        }

        return PageValueAsInt("ForumID");
      }
    }

    /// <summary>
    /// Gets the Name of forum for the current page, or an empty string if not in any forum
    /// </summary>
    public string PageForumName
    {
      get
      {
        return PageValueAsString("ForumName");
      }
    }

    /// <summary>
    /// Gets the CategoryID for the current page, or 0 if not in any category
    /// </summary>
    public int PageCategoryID
    {
      get
      {
        if (YafContext.Current.Settings.CategoryID != 0)
        {
          return YafContext.Current.Settings.CategoryID;
        }

        return PageValueAsInt("CategoryID");
      }
    }

    /// <summary>
    /// Gets the Name of category for the current page, or an empty string if not in any category
    /// </summary>
    public string PageCategoryName
    {
      get
      {
        return PageValueAsString("CategoryName");
      }
    }

    /// <summary>
    /// Gets the  TopicID of the current page, or 0 if not in any topic
    /// </summary>
    public int PageTopicID
    {
      get
      {
        return PageValueAsInt("TopicID");
      }
    }

    /// <summary>
    /// Gets the Name of topic for the current page, or an empty string if not in any topic
    /// </summary>
    public string PageTopicName
    {
      get
      {
        return PageValueAsString("TopicName");
      }
    }

    /// <summary>
    ///   Gets a value indicating whether the current user host admin (True).
    /// </summary>
    public bool IsHostAdmin
    {
      get
      {

        if (this._userFlags != null)
        {
          return this._userFlags.IsHostAdmin;

          // Obsolete : Ederon
          // if (General.BinaryAnd(Page["UserFlags"], UserFlags.IsHostAdmin))
          // isHostAdmin = true;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the user is excluded from CAPTCHA check (True).
    /// </summary>
    public bool IsCaptchaExcluded
    {
      get
      {
        if (this._userFlags != null)
        {
          return this._userFlags.IsCaptchaExcluded;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the  current user is an administrator (True).
    /// </summary>
    public bool IsAdmin
    {
      get
      {
        if (IsHostAdmin)
        {
          return true;
        }

        return PageValueAsBool("IsAdmin");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is a guest (True).
    /// </summary>
    public bool IsGuest
    {
      get
      {
        return PageValueAsBool("IsGuest");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is a forum moderator (mini-admin) (True).
    /// </summary>
    public bool IsForumModerator
    {
      get
      {
        return PageValueAsBool("IsForumModerator");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is a modeator for at least one forum (True);
    /// </summary>
    public bool IsModerator
    {
      get
      {
        return PageValueAsBool("IsModerator");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is suspended (True).
    /// </summary>
    public bool IsSuspended
    {
      get
      {
        if (Page != null && Page["Suspended"] != DBNull.Value)
        {
          return true;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets the DateTime the user is suspended until
    /// </summary>
    public DateTime SuspendedUntil
    {
      get
      {
        if (Page == null || Page["Suspended"] == DBNull.Value)
        {
          return DateTime.UtcNow;
        }
        else
        {
          return Convert.ToDateTime(Page["Suspended"]);
        }
      }
    }

    /// <summary>
    /// Gets the number of private messages that are unread
    /// </summary>
    public int UnreadPrivate
    {
      get
      {
        return Convert.ToInt32(Page["UnreadPrivate"]);
      }
    }

    /// <summary>
    /// Gets LastUnreadPm.
    /// </summary>
    public DateTime LastUnreadPm
    {
      get
      {
        if (this.Page["LastUnreadPm"].ToString().IsNotSet())
        {
          return DateTime.MinValue;
        }
        else
        {
          return Convert.ToDateTime(Page["LastUnreadPm"]);
        }
      }
    }

    /// <summary>
    /// Gets the number of albums which a user already has
    /// </summary>
    public int NumAlbums
    {
      get
      {
        return Convert.ToInt32(Page["NumAlbums"]);
      }
    }


    /// <summary>
    /// Gets the number of albums which a user can have
    /// </summary>
    public int UsrAlbums
    {
      get
      {
        return Convert.ToInt32(Page["UsrAlbums"]);
      }
    }

    /// <summary>
    /// Gets the value indicating whether  a user has buddies
    /// </summary>
    public bool UserHasBuddies
    {
      get
      {
        return PageValueAsBool("UserHasBuddies");

      }
    }

    /// <summary>
    /// Gets the number of pending buddy requests
    /// </summary>
    public int PendingBuddies
    {
      get
      {
        return Convert.ToInt32(Page["PendingBuddies"]);
      }
    }

    /// <summary>
    /// Gets the number of pending buddy requests.
    /// </summary>
    public DateTime LastPendingBuddies
    {
      get
      {
        if (this.Page["LastPendingBuddies"].ToString().IsNotSet())
        {
          return DateTime.MinValue;
        }
        else
        {
          return Convert.ToDateTime(Page["LastPendingBuddies"]);
        }
      }
    }

    /// <summary>
    /// Gets the time zone offset for the user
    /// </summary>
    public int TimeZoneUser
    {
      get
      {
        return Convert.ToInt32(Page["TimeZoneUser"]);
      }
    }

    /// <summary>
    /// Gets the time zone offset for the user
    /// </summary>
    public bool DSTUser
    {
      get
      {
        if (this._userFlags != null)
        {
          return this._userFlags.IsDST;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets the language file name for the user
    /// </summary>
    public string LanguageFile
    {
      get
      {
        return PageValueAsString("LanguageFile");
      }
    }

    /// <summary>
    /// Gets the user text editor
    /// </summary>
    public string TextEditor
    {
        get
        {
            return PageValueAsString("TextEditor");
        }
    }

    /// <summary>
    /// Gets the UserStyle for the user
    /// </summary>
    public string UserStyle
    {
      get
      {
        return PageValueAsString("UserStyle");
      }
    }

    /// <summary>
    /// Gets the culture code for the user
    /// </summary>
    public string CultureUser
    {
      get
      {
        return PageValueAsString("CultureUser");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the board is private (20050909 CHP) (True)
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

    #endregion

    #region Init Functions

    /// <summary>
    /// Initialize the user data and page data...
    /// </summary>
    protected void InitUserAndPage()
    {
      if (!this._initUserPage)
      {
        try
        {
          if (BeforeInit != null)
          {
            BeforeInit(this, new EventArgs());
          }

          DataRow pageRow;

          // get the current user and update the user last access flag datetime.
          MembershipUser user = UserMembershipHelper.GetUser(true);

          if (user != null && YafContext.Current.Get<HttpSessionStateBase>()["UserUpdated"] == null)
          {
            RoleMembershipHelper.UpdateForumUser(user, this.PageBoardID);
            YafContext.Current.Get<HttpSessionStateBase>()["UserUpdated"] = true;
          }

          string browser = "{0} {1}".FormatWith(YafContext.Current.Get<HttpRequestBase>().Browser.Browser, YafContext.Current.Get<HttpRequestBase>().Browser.Version);
          string platform = YafContext.Current.Get<HttpRequestBase>().Browser.Platform;

          bool isSearchEngine = false;
          bool dontTrack = false;

          string userAgent = YafContext.Current.Get<HttpRequestBase>().UserAgent;
          bool isMobileDevice = UserAgentHelper.IsMobileDevice(userAgent) || YafContext.Current.Get<HttpRequestBase>().Browser.IsMobileDevice;

          // try and get more verbose platform name by ref and other parameters             
          UserAgentHelper.Platform(userAgent, YafContext.Current.Get<HttpRequestBase>().Browser.Crawler, ref platform, ref browser, out isSearchEngine, out dontTrack);
          dontTrack = !YafContext.Current.BoardSettings.ShowCrawlersInActiveList && isSearchEngine;

          // don't track if this is a feed reader. May be to make it switchable in host settings.
          // we don't have page 'g' token for the feed page.
          if (browser.Contains("Unknown") && !dontTrack)
          {
            dontTrack = UserAgentHelper.IsFeedReader(userAgent);
          }

          int? categoryID = ObjectExtensions.ValidInt(YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("c"));
          int? forumID = ObjectExtensions.ValidInt(YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"));
          int? topicID = ObjectExtensions.ValidInt(YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"));
          int? messageID = ObjectExtensions.ValidInt(YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

          if (YafContext.Current.Settings.CategoryID != 0)
          {
            categoryID = YafContext.Current.Settings.CategoryID;
          }

          // vzrus: to log unhandled UserAgent strings
          if (YafContext.Current.BoardSettings.UserAgentBadLog)
          {
            if (userAgent.IsNotSet())
            {
              LegacyDb.eventlog_create(null, this, "UserAgent string is empty.", EventLogTypes.Warning);
            }

            if (platform.ToLower().Contains("unknown") || browser.ToLower().Contains("unknown"))
            {
              LegacyDb.eventlog_create(
                null,
                this,
                "Unhandled UserAgent string:'{0}' /r/nPlatform:'{1}' /r/nBrowser:'{2}' /r/nSupports cookies='{3}' /r/nUserID='{4}'."
                  .FormatWith(
                    userAgent,
                    YafContext.Current.Get<HttpRequestBase>().Browser.Platform,
                    YafContext.Current.Get<HttpRequestBase>().Browser.Browser,
                    YafContext.Current.Get<HttpRequestBase>().Browser.Cookies,
                    user != null ? user.UserName : String.Empty),
                EventLogTypes.Warning);
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
            PageBoardID,
            PageBoardUid,
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
            dontTrack
           );

            // if the user doesn't exist...
            if (user != null && pageRow == null)
            {
              // create the user...
              if (!RoleMembershipHelper.DidCreateForumUser(user, PageBoardID))
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

          YafContext.Current.Vars["DontTrack"] = dontTrack;

          Page = pageRow;

          if (AfterInit != null)
          {
            AfterInit(this, new EventArgs());
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

    #endregion
  }
}