using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Xml;

namespace yaf
{
	public class PageInfo
	{
		private DataRow		m_pageinfo			= null;
		private IForumUser	m_forumUser			= null;
		private string		m_strLoadMessage	= "";

		public IForumUser User
		{
			get
			{
				return m_forumUser;
			}
		}

		public string LoadMessage
		{
			get
			{
				return m_strLoadMessage;
			}
		}

		public void AddLoadMessage(string msg) 
		{
			msg = msg.Replace("\\","\\\\");
			msg = msg.Replace("'","\\'");
			msg = msg.Replace("\r\n","\\r\\n");
			msg = msg.Replace("\n","\\n");
			msg = msg.Replace("\"","\\\"");
			m_strLoadMessage += msg + "\\n\\n";
		}

		public PageInfo()
		{
		}

		public void PageLoad(bool checkSuspended)
		{
#if DEBUG
			QueryCounter.Reset();
#endif

			// Set the culture and UI culture to the browser's accept language
			try 
			{
				string sCulture = HttpContext.Current.Request.UserLanguages[0];
				if(sCulture.IndexOf(';')>=0) 
					sCulture = sCulture.Substring(0,sCulture.IndexOf(';'));

				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(sCulture);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(sCulture);
			}
			catch(Exception)
			{
			}

			//Response.Expires = -1000;
			HttpContext.Current.Response.AddHeader("Cache-control","private, no-cache, must-revalidate");
			HttpContext.Current.Response.AddHeader("Expires","Mon, 26 Jul 1997 05:00:00 GMT"); // Past date
			HttpContext.Current.Response.AddHeader("Pragma","no-cache");

			string key = string.Format("BannedIP.{0}",PageBoardID);
			DataTable banip = (DataTable)HttpContext.Current.Cache[key];
			if(banip == null) 
			{
				banip = DB.bannedip_list(PageBoardID,null);
				HttpContext.Current.Cache[key] = banip;
			}
			foreach(DataRow row in banip.Rows)
				if(Utils.IsBanned((string)row["Mask"],HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
					HttpContext.Current.Response.End();

			// Find user name
			AuthType authType = Data.GetAuthType;
			switch(authType)
			{
				case AuthType.Guest:
					m_forumUser = new GuestUser();
					break;
				case AuthType.Rainbow:
					m_forumUser = new RainbowUser(HttpContext.Current.User.Identity.Name,HttpContext.Current.User.Identity.IsAuthenticated);
					break;
				case AuthType.DotNetNuke:
					m_forumUser = new DotNetNukeUser(HttpContext.Current.User.Identity.Name,HttpContext.Current.User.Identity.IsAuthenticated);
					break;
				case AuthType.Windows:
					m_forumUser = new WindowsUser(HttpContext.Current.User.Identity.Name,HttpContext.Current.User.Identity.IsAuthenticated);
					break;
				default:
					m_forumUser = new FormsUser(HttpContext.Current.User.Identity.Name,HttpContext.Current.User.Identity.IsAuthenticated);
					break;
			}

			string browser = String.Format("{0} {1}",HttpContext.Current.Request.Browser.Browser,HttpContext.Current.Request.Browser.Version);
			string platform = HttpContext.Current.Request.Browser.Platform;

			if(HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 5.2")>=0)
				platform = "Win2003";

			m_pageinfo = DB.pageload(
				HttpContext.Current.Session.SessionID,
				PageBoardID,
				User.Name,
				HttpContext.Current.Request.UserHostAddress,
				HttpContext.Current.Request.FilePath,
				browser,
				platform,
				HttpContext.Current.Request.QueryString["c"],
				HttpContext.Current.Request.QueryString["f"],
				HttpContext.Current.Request.QueryString["t"],
				HttpContext.Current.Request.QueryString["m"]);

			// If user wasn't found and we have foreign 
			// authorization, try to register the user.
			if(m_pageinfo==null && authType!=AuthType.Forms && User.IsAuthenticated) 
			{
				if(!DB.user_register(this,PageBoardID,User.Name,"ext",User.Email,User.Location,User.HomePage,0,false))
					throw new ApplicationException("User registration failed.");

				m_pageinfo = DB.pageload(
					HttpContext.Current.Session.SessionID,
					PageBoardID,
					User.Name,
					HttpContext.Current.Request.UserHostAddress,
					HttpContext.Current.Request.FilePath,
					HttpContext.Current.Request.Browser.Browser,
					HttpContext.Current.Request.Browser.Platform,
					HttpContext.Current.Request.QueryString["c"],
					HttpContext.Current.Request.QueryString["f"],
					HttpContext.Current.Request.QueryString["t"],
					HttpContext.Current.Request.QueryString["m"]);
			}

			if(m_pageinfo==null) 
			{
				if(User.IsAuthenticated) 
					throw new ApplicationException(string.Format("User '{0}' isn't registered.",User.Name));
				else
					throw new ApplicationException("Failed to find guest user.");
			}

			if(checkSuspended && IsSuspended) 
			{
				if(SuspendedTo < DateTime.Now) 
				{
					DB.user_suspend(PageUserID,null);
					HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl);
				}
				Forum.Redirect(Pages.info,"i=2");
			}

			if(HttpContext.Current.Request.Cookies["yaf"]!=null) 
			{
				HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies["yaf"]);
				HttpContext.Current.Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
			}

			// This happens when user logs in
			if(Mession.LastVisit == DateTime.MinValue)
			{
				// Only important for portals like Rainbow or DotNetNuke
				if(User.IsAuthenticated)
					User.UpdateUserInfo(PageUserID);

				if((int)m_pageinfo["Incoming"]>0) 
					AddLoadMessage(String.Format("You have {0} unread message(s) in your Inbox",m_pageinfo["Incoming"]));
			}

			if(Mession.LastVisit == DateTime.MinValue && HttpContext.Current.Request.Cookies["yaf"] != null && HttpContext.Current.Request.Cookies["yaf"]["lastvisit"] != null) 
			{
				try 
				{
					Mession.LastVisit = DateTime.Parse(HttpContext.Current.Request.Cookies["yaf"]["lastvisit"]);
				}
				catch(Exception) 
				{
					Mession.LastVisit = DateTime.Now;
				}
				HttpContext.Current.Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
				HttpContext.Current.Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
			}
			else if(Mession.LastVisit == DateTime.MinValue) 
			{
				Mession.LastVisit = DateTime.Now;
			}

			if(HttpContext.Current.Request.Cookies["yaf"] != null && HttpContext.Current.Request.Cookies["yaf"]["lastvisit"] != null) 
			{
				try 
				{
					if(DateTime.Parse(HttpContext.Current.Request.Cookies["yaf"]["lastvisit"]) < DateTime.Now - TimeSpan.FromMinutes(5)) 
					{
						HttpContext.Current.Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
						HttpContext.Current.Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
					}
				}
				catch(Exception) 
				{
					HttpContext.Current.Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
					HttpContext.Current.Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
				}
			}
			else 
			{
				HttpContext.Current.Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
				HttpContext.Current.Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
			}

			// Check if pending mails, and send 10 of them if possible
			if((int)m_pageinfo["MailsPending"]>0) 
			{
				try 
				{
					using(DataTable dt = DB.mail_list()) 
					{
						for(int i=0;i<dt.Rows.Count;i++) 
						{
							// Build a MailMessage
							Utils.SendMail(Config.BoardSettings.ForumEmail,(string)dt.Rows[i]["ToUser"],(string)dt.Rows[i]["Subject"],(string)dt.Rows[i]["Body"]);
							DB.mail_delete(dt.Rows[i]["MailID"]);
						}
						if(IsAdmin) AddLoadMessage(String.Format("Sent {0} mails.",dt.Rows.Count));
					}
				}
				catch(Exception x) 
				{
					if(IsAdmin) 
					{
						AddLoadMessage(x.Message);
					}
				}
			}
		}
		#region Forum Access
		/// <summary>
		/// True if current user has post access in the current forum
		/// </summary>
		public bool ForumPostAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("PostAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["PostAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user has reply access in the current forum
		/// </summary>
		public bool ForumReplyAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("ReplyAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["ReplyAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user has read access in the current forum
		/// </summary>
		public bool ForumReadAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("ReadAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["ReadAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user has access to create priority topics in the current forum
		/// </summary>
		public bool ForumPriorityAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("PriorityAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["PriorityAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user has access to create polls in the current forum.
		/// </summary>
		public bool ForumPollAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("PollAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["PollAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user has access to vote on polls in the current forum
		/// </summary>
		public bool ForumVoteAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("VoteAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["VoteAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user is a moderator of the current forum
		/// </summary>
		public bool ForumModeratorAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("ModeratorAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["ModeratorAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user can delete own messages in the current forum
		/// </summary>
		public bool ForumDeleteAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("DeleteAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["DeleteAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user can edit own messages in the current forum
		/// </summary>
		public bool ForumEditAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("EditAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["EditAccess"].ToString())>0;
			}
		}
		/// <summary>
		/// True if the current user can upload attachments
		/// </summary>
		public bool ForumUploadAccess 
		{
			get 
			{
				if(m_pageinfo.IsNull("UploadAccess"))
					return false;
				else
					return long.Parse(m_pageinfo["UploadAccess"].ToString())>0;
			}
		}
		static public int PageBoardID
		{
			get
			{
				return int.Parse(Config.ConfigSection["boardid"]);
			}
		}
		/// <summary>
		/// The UserID of the current user.
		/// </summary>
		public int PageUserID 
		{
			get 
			{
				if(m_pageinfo!=null)
					return (int)m_pageinfo["UserID"];
				else
					return 0;
			}
		}
		public string PageUserName 
		{
			get 
			{
				if(m_pageinfo!=null)
					return (string)m_pageinfo["UserName"];
				else
					return "";
			}
		}
		/// <summary>
		/// ForumID for the current page, or 0 if not in any forum
		/// </summary>
		public int PageForumID 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("ForumID"))
					return (int)m_pageinfo["ForumID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of forum for the current page, or an empty string if not in any forum
		/// </summary>
		public string PageForumName 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("ForumName"))
					return (string)m_pageinfo["ForumName"];
				else
					return "";
			}
		}
		/// <summary>
		/// CategoryID for the current page, or 0 if not in any category
		/// </summary>
		public int PageCategoryID 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("CategoryID"))
					return (int)m_pageinfo["CategoryID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of category for the current page, or an empty string if not in any category
		/// </summary>
		public string PageCategoryName 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("CategoryName"))
					return (string)m_pageinfo["CategoryName"];
				else
					return "";
			}
		}
		/// <summary>
		/// The TopicID of the current page, or 0 if not in any topic
		/// </summary>
		public int PageTopicID 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("TopicID"))
					return (int)m_pageinfo["TopicID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of topic for the current page, or an empty string if not in any topic
		/// </summary>
		public string PageTopicName 
		{
			get 
			{
				if(m_pageinfo!=null && !m_pageinfo.IsNull("TopicName"))
					return (string)m_pageinfo["TopicName"];
				else
					return "";
			}
		}
		public bool IsHostAdmin
		{
			get 
			{
				if(m_pageinfo!=null)
					return (bool)m_pageinfo["IsHostAdmin"];
				else
					return false;
			}
		}
		/// <summary>
		/// True if current user is an administrator
		/// </summary>
		public bool IsAdmin 
		{
			get 
			{
				if(IsHostAdmin)
					return true;

				if(m_pageinfo!=null)
					return long.Parse(m_pageinfo["IsAdmin"].ToString())!=0;
				else
					return false;
			}
		}
		/// <summary>
		/// True if the current user is a guest
		/// </summary>
		public bool IsGuest 
		{
			get 
			{
				if(m_pageinfo!=null)
					return long.Parse(m_pageinfo["IsGuest"].ToString())!=0;
				else
					return false;
			}
		}
		/// <summary>
		/// True if the current user is a forum moderator (mini-admin)
		/// </summary>
		public bool IsForumModerator 
		{
			get 
			{
				if(m_pageinfo!=null)
					return long.Parse(m_pageinfo["IsForumModerator"].ToString())!=0;
				else
					return false;
			}
		}
		/// <summary>
		/// True if current user is a modeator for at least one forum
		/// </summary>
		public bool IsModerator
		{
			get 
			{
				if(m_pageinfo!=null)
					return long.Parse(m_pageinfo["IsModerator"].ToString())!=0;
				else
					return false;
			}
		}
		#endregion
		#region Date and time functions
		/// <summary>
		/// Returns the user timezone offset from GMT
		/// </summary>
		public TimeSpan TimeZoneOffsetUser 
		{
			get 
			{
				if(m_pageinfo!=null) 
				{
					int min = (int)m_pageinfo["TimeZoneUser"];
					return new TimeSpan(min/60,min%60,0);
				} 
				else
					return new TimeSpan(0);
			}
		}
		/// <summary>
		/// Returns the time zone offset for the current user compared to the forum time zone.
		/// </summary>
		public TimeSpan TimeOffset 
		{
			get 
			{
				return TimeZoneOffsetUser - Config.BoardSettings.TimeZone;
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateTime(object o) 
		{
			DateTime dt = (DateTime)o;
			return String.Format("{0:F}",dt + TimeOffset);
		}
		public string FormatDateTimeShort(object o) 
		{
			DateTime dt = (DateTime)o;
			return String.Format("{0:f}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 7. februar 2003
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateLong(DateTime dt) 
		{
			return String.Format("{0:D}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormatDateShort(object o) 
		{
			DateTime dt = (DateTime)o;
			return String.Format("{0:d}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatTime(DateTime dt) 
		{
			return String.Format("{0:T}",dt + TimeOffset);
		}
		#endregion
		#region Theme Functions
		// XML THEME FILE (TEST)
		private XmlDocument LoadTheme(string themefile) 
		{
			if(themefile==null) 
			{
				if(m_pageinfo==null || m_pageinfo.IsNull("ThemeFile") || !Config.BoardSettings.AllowUserTheme)
					themefile = Config.ConfigSection["theme"];
				else
					themefile = (string)m_pageinfo["ThemeFile"];

				if(themefile==null)
					themefile = "standard.xml";
			}

			XmlDocument doc = null;
#if !DEBUG
			doc = (XmlDocument)System.Web.HttpContext.Current.Cache[themefile];
#endif
			if(doc==null) 
			{
				doc = new XmlDocument();
				doc.Load(System.Web.HttpContext.Current.Server.MapPath(String.Format("{0}themes/{1}",Data.ForumRoot,themefile)));
#if !DEBUG
				System.Web.HttpContext.Current.Cache[themefile] = doc;
#endif
			}
			return doc;
		}

		public string GetThemeContents(string page,string tag) 
		{
			XmlDocument doc = LoadTheme(null);

			string themeDir = doc.DocumentElement.Attributes["dir"].Value;
			string langCode = LoadTranslation().ToUpper();
			string select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']", page.ToUpper(),tag.ToUpper(),langCode);
			XmlNode node = doc.SelectSingleNode(select);
			if(node==null)
			{
				select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(),tag.ToUpper());
				node = doc.SelectSingleNode(select);
			}
			if(node==null)
				throw new Exception(String.Format("Missing theme item: {0}.{1}",page.ToUpper(),tag.ToUpper()));

			string contents = node.InnerText;
			contents = contents.Replace("~",String.Format("{0}themes/{1}",Data.ForumRoot,themeDir));
			return contents;
		}
		public string ThemeDir 
		{
			get 
			{
				XmlDocument doc = LoadTheme(null);
				return String.Format("{0}themes/{1}/",Data.ForumRoot,doc.DocumentElement.Attributes["dir"].Value);
			}
		}
		#endregion
		#region Localizing
		private Localizer	m_localizer = null;
		private	Localizer	m_defaultLocale	= null;

		private	string		m_transPage = null;
		
		/// <summary>
		/// What section of the xml is used to translate this page
		/// </summary>
		public string TransPage
		{
			get
			{
				if(m_transPage!=null)
					return m_transPage;

				throw new ApplicationException(string.Format("Missing TransPage property for {0}",GetType()));
			}
			set
			{
				m_transPage = value;
			}
		}

		public string GetText(string text) 
		{
			return GetText(TransPage,text);
		}

		private string LoadTranslation() 
		{
			if(m_localizer!=null) 
				return m_localizer.LanguageCode;
			
			string filename = null;

			if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !Config.BoardSettings.AllowUserLanguage)
				filename = Config.ConfigSection["language"];
			else
				filename = (string)m_pageinfo["LanguageFile"];

			if(filename==null)
				filename = "english.xml";

#if !DEBUG
			if(m_localizer==null && HttpContext.Current.Cache["Localizer." + filename]!=null)
				m_localizer = (Localizer)HttpContext.Current.Cache["Localizer." + filename];
#endif
			if(m_localizer==null) 
			{

				m_localizer = new Localizer(HttpContext.Current.Server.MapPath(String.Format("{0}languages/{1}",Data.ForumRoot,filename)));
#if !DEBUG
				HttpContext.Current.Cache["Localizer." + filename] = m_localizer;
#endif
			}
			/// If not using default language load that too
			if(filename.ToLower()!="english.xml") 
			{
#if !DEBUG
				if(m_defaultLocale==null && HttpContext.Current.Cache["DefaultLocale"]!=null)
					m_defaultLocale = (Localizer)HttpContext.Current.Cache["DefaultLocale"];
#endif

				if(m_defaultLocale==null) 
				{
					m_defaultLocale = new Localizer(HttpContext.Current.Server.MapPath(String.Format("{0}languages/english.xml",Data.ForumRoot)));
#if !DEBUG
					HttpContext.Current.Cache["DefaultLocale"] = m_defaultLocale;
#endif
				}
			}
			return m_localizer.LanguageCode;
		}

		public string GetText(string page,string text) 
		{
			LoadTranslation();
			string str = m_localizer.GetText(page,text);
			/// If not default language, try to use that instead
			if(str==null && m_defaultLocale!=null) 
			{
				str = m_defaultLocale.GetText(page,text);
				if(str!=null) str = '[' + str + ']';
			}
			if(str==null) 
			{
#if !DEBUG
				string filename = null;

				if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !Config.BoardSettings.AllowUserLanguage)
					filename = Config.ConfigSection["language"];
				else
					filename = (string)m_pageinfo["LanguageFile"];

				if(filename==null)
					filename = "english.xml";

				HttpContext.Current.Cache.Remove("Localizer." + filename);
#endif
				throw new Exception(String.Format("Missing translation for {1}.{0}",text.ToUpper(),page.ToUpper()));
			}
			str = str.Replace("[b]","<b>");
			str = str.Replace("[/b]","</b>");
			return str;
		}
		#endregion
		public bool IsSuspended 
		{
			get 
			{
				if(m_pageinfo==null)
					return false;
				else
					return !m_pageinfo.IsNull("Suspended");
			}
		}

		public DateTime SuspendedTo 
		{
			get 
			{
				if(m_pageinfo==null || m_pageinfo.IsNull("Suspended"))
					return DateTime.Now;
				else
					return DateTime.Parse(m_pageinfo["Suspended"].ToString());
			}
		}
		public int UnreadPrivate 
		{
			get 
			{
				return (int)m_pageinfo["Incoming"];
			}
		}
		public string ForumURL 
		{
			get 
			{
				return string.Format("{0}{1}",ServerURL,Forum.GetLink(Pages.forum));
			}
		}
		public string ServerURL
		{
			get
			{
				long port = long.Parse(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
				if(port!=80)
					return String.Format("http://{0}:{1}",HttpContext.Current.Request.ServerVariables["SERVER_NAME"],port);
				else
					return String.Format("http://{0}",HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);
			}
		}
		public string ThemeFile(string filename) 
		{
			return ThemeDir + filename;
		}
	};
}
