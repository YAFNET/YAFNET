/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System;
using System.Data;
using System.Web;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
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
          InitUserAndPage();
        }

        return this._page;
      }

      set
      {
        this._page = value;
        this._initUserPage = value != null;

        // get user flags
        if (this._page != null)
        {
          this._userFlags = new UserFlags(this._page["UserFlags"]);
        }
        else
        {
          this._userFlags = null;
        }
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
    /// Internal helper function used for redudant page variable access (string)
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
    /// True if current user has post access in the current forum
    /// </summary>
    public bool ForumPostAccess
    {
      get
      {
        return AccessNotNull("PostAccess");
      }
    }

    /// <summary>
    /// True if the current user has reply access in the current forum
    /// </summary>
    public bool ForumReplyAccess
    {
      get
      {
        return AccessNotNull("ReplyAccess");
      }
    }

    /// <summary>
    /// True if the current user has read access in the current forum
    /// </summary>
    public bool ForumReadAccess
    {
      get
      {
        return AccessNotNull("ReadAccess");
      }
    }

    /// <summary>
    /// True if the current user has access to create priority topics in the current forum
    /// </summary>
    public bool ForumPriorityAccess
    {
      get
      {
        return AccessNotNull("PriorityAccess");
      }
    }

    /// <summary>
    /// True if the current user has access to create polls in the current forum.
    /// </summary>
    public bool ForumPollAccess
    {
      get
      {
        return AccessNotNull("PollAccess");
      }
    }

    /// <summary>
    /// True if the current user has access to vote on polls in the current forum
    /// </summary>
    public bool ForumVoteAccess
    {
      get
      {
        return AccessNotNull("VoteAccess");
      }
    }

    /// <summary>
    /// True if the current user is a moderator of the current forum
    /// </summary>
    public bool ForumModeratorAccess
    {
      get
      {
        return AccessNotNull("ModeratorAccess");
      }
    }

    /// <summary>
    /// True if the current user can delete own messages in the current forum
    /// </summary>
    public bool ForumDeleteAccess
    {
      get
      {
        return AccessNotNull("DeleteAccess");
      }
    }

    /// <summary>
    /// True if the current user can edit own messages in the current forum
    /// </summary>
    public bool ForumEditAccess
    {
      get
      {
        return AccessNotNull("EditAccess");
      }
    }

    /// <summary>
    /// True if the current user can upload attachments
    /// </summary>
    public bool ForumUploadAccess
    {
      get
      {
        return AccessNotNull("UploadAccess");
      }
    }

    /// <summary>
    /// True if the current user can download attachments
    /// </summary>
    public bool ForumDownloadAccess
    {
      get
      {
        return AccessNotNull("DownloadAccess");
      }
    }

    /// <summary>
    /// Gets PageBoardID.
    /// </summary>
    public int PageBoardID
    {
      get
      {
        return YafContext.Current.Settings == null ? 1 : YafContext.Current.Settings.BoardID;
      }
    }

    /// <summary>
    /// The UserID of the current user.
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
    /// ForumID for the current page, or 0 if not in any forum
    /// </summary>
    public int PageForumID
    {
      get
      {
        int nLockedForum = YafContext.Current.Settings.LockedForum;
        if (nLockedForum != 0)
        {
          return nLockedForum;
        }

        return PageValueAsInt("ForumID");
      }
    }

    /// <summary>
    /// Name of forum for the current page, or an empty string if not in any forum
    /// </summary>
    public string PageForumName
    {
      get
      {
        return PageValueAsString("ForumName");
      }
    }

    /// <summary>
    /// CategoryID for the current page, or 0 if not in any category
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
    /// Name of category for the current page, or an empty string if not in any category
    /// </summary>
    public string PageCategoryName
    {
      get
      {
        return PageValueAsString("CategoryName");
      }
    }

    /// <summary>
    /// The TopicID of the current page, or 0 if not in any topic
    /// </summary>
    public int PageTopicID
    {
      get
      {
        return PageValueAsInt("TopicID");
      }
    }

    /// <summary>
    /// Name of topic for the current page, or an empty string if not in any topic
    /// </summary>
    public string PageTopicName
    {
      get
      {
        return PageValueAsString("TopicName");
      }
    }

    /// <summary>
    /// Is the current user host admin?
    /// </summary>
    public bool IsHostAdmin
    {
      get
      {
        bool isHostAdmin = false;

        if (this._userFlags != null)
        {
          isHostAdmin = this._userFlags.IsHostAdmin;

          // Obsolette : Ederon
          // if (General.BinaryAnd(Page["UserFlags"], UserFlags.IsHostAdmin))
          // 	isHostAdmin = true;
        }

        return isHostAdmin;
      }
    }

    /// <summary>
    /// True if user is excluded from CAPTCHA check.
    /// </summary>
    public bool IsCaptchaExcluded
    {
      get
      {
        bool isCaptchaExcluded = false;

        if (this._userFlags != null)
        {
          isCaptchaExcluded = this._userFlags.IsCaptchaExcluded;
        }

        return isCaptchaExcluded;
      }
    }

    /// <summary>
    /// True if current user is an administrator
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
    /// True if the current user is a guest
    /// </summary>
    public bool IsGuest
    {
      get
      {
        return PageValueAsBool("IsGuest");
      }
    }

    /// <summary>
    /// True if the current user is a forum moderator (mini-admin)
    /// </summary>
    public bool IsForumModerator
    {
      get
      {
        return PageValueAsBool("IsForumModerator");
      }
    }

    /// <summary>
    /// True if current user is a modeator for at least one forum
    /// </summary>
    public bool IsModerator
    {
      get
      {
        return PageValueAsBool("IsModerator");
      }
    }

    /// <summary>
    /// True if the current user is suspended
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
    /// When the user is suspended until
    /// </summary>
    public DateTime SuspendedUntil
    {
      get
      {
        if (Page == null || Page["Suspended"] == DBNull.Value)
        {
          return DateTime.Now;
        }
        else
        {
          return Convert.ToDateTime(Page["Suspended"]);
        }
      }
    }

    /// <summary>
    /// The number of private messages that are unread
    /// </summary>
    public int UnreadPrivate
    {
      get
      {
        return Convert.ToInt32(Page["Incoming"]);
      }
    }

    /// <summary>
    /// Gets LastUnreadPm.
    /// </summary>
    public DateTime LastUnreadPm
    {
      get
      {
        if (String.IsNullOrEmpty(Page["LastUnreadPm"].ToString()))
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
    /// The number of pending buddy requests
    /// </summary>
    public int PendingBuddies
    {
        get
        {
            return Convert.ToInt32(Page["PendingBuddies"]);
        }
    }

    /// <summary>
    /// The number of pending buddy requests
    /// </summary>
    public DateTime LastPendingBuddies
    {
        get
        {
            if (String.IsNullOrEmpty(Page["LastPendingBuddies"].ToString()))
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
    /// The time zone offset for the user
    /// </summary>
    public int TimeZoneUser
    {
      get
      {
        return Convert.ToInt32(Page["TimeZoneUser"]);
      }
    }

    /// <summary>
    /// The language file for the user
    /// </summary>
    public string LanguageFile
    {
      get
      {
        return PageValueAsString("LanguageFile");
      }
    }

    /// <summary>
    /// True if board is private (20050909 CHP)
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

          // verify db is initialized...
          if (!YafServices.InitializeDb.Initialized)
          {
            // just init the DB from here...
            YafServices.InitializeDb.Run();
          }

          DataRow pageRow;

          // get the current user and update the user last access flag datetime.
          MembershipUser user = UserMembershipHelper.GetUser(true);

          if (user != null && HttpContext.Current.Session["UserUpdated"] == null)
          {
            RoleMembershipHelper.UpdateForumUser(user, PageBoardID);
            HttpContext.Current.Session["UserUpdated"] = true;
          }

          string browser = String.Format("{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
          string platform = HttpContext.Current.Request.Browser.Platform;
          bool isSearchEngine = false;

          if (HttpContext.Current.Request.UserAgent != null)
          {
            if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 5.2") >= 0)
            {
              platform = "Win2003";
            }
            else if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 6.0") >= 0)
            {
              platform = "Vista";
            }
            else if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 6.1") >= 0)
            {
              platform = "Win7";
            }
            else
            {
              // check if it's a search engine spider...
              isSearchEngine = UserAgentHelper.IsSearchEngineSpider(HttpContext.Current.Request.UserAgent);
            }
          }

          int? categoryID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["c"]);
          int? forumID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["f"]);
          int? topicID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["t"]);
          int? messageID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["m"]);

          if (YafContext.Current.Settings.CategoryID != 0)
          {
            categoryID = YafContext.Current.Settings.CategoryID;
          }

          object userKey = DBNull.Value;

          if (user != null)
          {
            userKey = user.ProviderUserKey;
          }

          do
          {          
            pageRow = DB.pageload(
              HttpContext.Current.Session.SessionID, 
              PageBoardID, 
              userKey, 
              HttpContext.Current.Request.UserHostAddress, 
              HttpContext.Current.Request.FilePath,
              HttpContext.Current.Request.QueryString.ToString(),
              browser, 
              platform, 
              categoryID, 
              forumID, 
              topicID, 
              messageID, 
              // don't track if this is a search engine
              isSearchEngine,
              YafContext.Current.BoardSettings.EnableBuddyList,
              YafContext.Current.BoardSettings.AllowPrivateMessages,
              YafContext.Current.BoardSettings.UseStyledNicks);

            // if the user doesn't exist...
            if (user != null && pageRow == null)
            {
              // create the user...
              if (!RoleMembershipHelper.DidCreateForumUser(user, PageBoardID))
              {
                throw new ApplicationException("Failed to use new user.");
              }
            }

            // only continue if either the page has been loaded or the user has been found...
          }
          while (pageRow == null && user != null);

          // page still hasn't been loaded...
          if (pageRow == null)
          {
            throw new ApplicationException("Failed to find guest user.");
          }

          // save this page data to the context...
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
					YAF.Classes.Data.DB.eventlog_create( null, "Failure Initializing User/Page.", x, EventLogTypes.Warning );
					
// log the user out...
					FormsAuthentication.SignOut();

					if ( YafContext.Current.ForumPageType != ForumPages.info )
					{
						// show a failure notice since something is probably up with membership...
						YafBuildLink.RedirectInfoPage( InfoMessage.Failure );
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