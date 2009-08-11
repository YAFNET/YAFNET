/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Collections.Generic;
using System.Text;
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
		private System.Data.DataRow _page = null;
		private UserFlags _userFlags = null;
		private bool _initUserPage = false;

		public event EventHandler<EventArgs> BeforeInit;
		public event EventHandler<EventArgs> AfterInit;

		public virtual System.Data.DataRow Page
		{
			get
			{
				if (!_initUserPage) InitUserAndPage();
				return _page;
			}
			set
			{
				_page = value;
				_initUserPage = (value != null);

				// get user flags
				if (_page != null) _userFlags = new UserFlags(_page["UserFlags"]);
				else _userFlags = null;
			}
		}

		/// <summary>
		/// Helper function to see if the Page variable is populated
		/// </summary>
		public bool PageIsNull()
		{
			return (Page == null);
		}

		/// <summary>
		/// Helper function used for redundant "access" fields internally
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool AccessNotNull(string field)
		{
			if (Page[field] == DBNull.Value) return false;
			return (Convert.ToInt32(Page[field]) > 0);
		}

		/// <summary>
		/// Internal helper function used for redundant page variable access (bool)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool PageValueAsBool(string field)
		{
			if (Page != null && Page[field] != DBNull.Value)
				return Convert.ToInt32(Page[field]) != 0;

			return false;
		}

		/// <summary>
		/// Internal helper function used for redundant page variable access (int)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private int PageValueAsInt(string field)
		{
			if (Page != null && Page[field] != DBNull.Value)
				return Convert.ToInt32(Page[field]);

			return 0;
		}

		/// <summary>
		/// Internal helper function used for redudant page variable access (string)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private string PageValueAsString(string field)
		{
			if (Page != null && Page[field] != DBNull.Value)
				return Page[field].ToString();

			return "";
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
					return nLockedForum;

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

				if (_userFlags != null)
				{
					isHostAdmin = _userFlags.IsHostAdmin;
					// Obsolette : Ederon
					// if (General.BinaryAnd(Page["UserFlags"], UserFlags.IsHostAdmin))
					//	isHostAdmin = true;
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

				if (_userFlags != null)
				{
					isCaptchaExcluded = _userFlags.IsCaptchaExcluded;
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
					return true;

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
					return true;

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
					return DateTime.Now;
				else
					return Convert.ToDateTime(Page["Suspended"]);
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
			if (!_initUserPage)
			{
				try
				{
					if (BeforeInit != null) BeforeInit(this, new EventArgs());

					// verify db is initialized...
					if ( !YafServices.InitializeDb.Initialized ) throw new Exception("Database is not initialized.");

					System.Data.DataRow pageRow;

					// Find user name
					MembershipUser user = Membership.GetUser();
					if (user != null && HttpContext.Current.Session["UserUpdated"] == null)
					{
						RoleMembershipHelper.UpdateForumUser(user, this.PageBoardID);
						HttpContext.Current.Session["UserUpdated"] = true;
					}

					string browser = String.Format("{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
					string platform = HttpContext.Current.Request.Browser.Platform;
					bool isSearchEngine = false;

					if (HttpContext.Current.Request.UserAgent != null)
					{
						if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 5.2") >= 0)
							platform = "Win2003";
						else if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 6.0") >= 0)
							platform = "Vista";
						else if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 6.1") >= 0)
							platform = "Win7";
						else
							// check if it's a search engine spider...
							isSearchEngine = General.IsSearchEngineSpider(HttpContext.Current.Request.UserAgent);
					}

					int? categoryID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["c"]);
					int? forumID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["f"]);
					int? topicID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["t"]);
					int? messageID = TypeHelper.ValidInt(HttpContext.Current.Request.QueryString["m"]);

					if (YafContext.Current.Settings.CategoryID != 0)
						categoryID = YafContext.Current.Settings.CategoryID;

					object userKey = DBNull.Value;

					if (user != null)
					{
						userKey = user.ProviderUserKey;
					}

					do
					{
						pageRow = DB.pageload(
							HttpContext.Current.Session.SessionID,
							this.PageBoardID,
							userKey,
							HttpContext.Current.Request.UserHostAddress,
							HttpContext.Current.Request.FilePath,
							browser,
							platform,
							categoryID,
							forumID,
							topicID,
							messageID,
							// don't track if this is a search engine
							isSearchEngine);

						// if the user doesn't exist...
						if (user != null && pageRow == null)
						{
							// create the user...
							if (!RoleMembershipHelper.DidCreateForumUser(user, this.PageBoardID))
								throw new ApplicationException("Failed to use new user.");
						}

						// only continue if either the page has been loaded or the user has been found...
					} while (pageRow == null && user != null);

					// page still hasn't been loaded...
					if (pageRow == null)
					{
						throw new ApplicationException("Failed to find guest user.");
					}

					// save this page data to the context...
					this.Page = pageRow;

					if (AfterInit != null) AfterInit(this, new EventArgs());
				}
				catch (Exception x)
				{
#if !DEBUG
	// log the exception...
					YAF.Classes.Data.DB.eventlog_create( null, "Failure Initializing User/Page.", x, EventLogTypes.Warning );
					// show a failure notice since something is probably up with membership...
					YafBuildLink.RedirectInfoPage( InfoMessage.Failure );
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
