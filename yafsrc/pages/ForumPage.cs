/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.IO;
using System.Text;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.Web;
using yaf.classes;

// Grønn: #25C110
// Brown: #D0BF8C

namespace yaf.pages
{
	#region User Classes
	public interface IForumUser
	{
		string Name
		{
			get;
		}
		string Email
		{
			get;
		}
		bool IsAuthenticated
		{
			get;
		}
		object Location
		{
			get;
		}
		object HomePage
		{
			get;
		}
		bool CanLogin
		{
			get;
		}
	}

	public class RainbowUser : IForumUser
	{
		private	string	m_userName;
		private string	m_email;
		private	string	m_location;
		private int		m_userID;
		private bool	m_isAuthenticated;

		public RainbowUser(string userName,bool isAuthenticated)
		{
			/*
			 * UserID (int)=1
			 * Email (nvarchar)=bh@bhenden.org
			 * Password (nvarchar)=altchs
			 * Name (nvarchar)=bhenden
			 * Company (nvarchar)=
			 * Address (nvarchar)=Engsoleia 13
			 * City (nvarchar)=Kristiansund
			 * Zip (nvarchar)=6518
			 * CountryId (nchar)=NO
			 * StateId (int)=9889982
			 * PIva (nvarchar)=
			 * CFiscale (nvarchar)=
			 * Phone (nvarchar)=71583338
			 * Fax (nvarchar)=
			 * SendNewsletter (bit)=False
			 * MailChecked (tinyint)=
			 * PortalId (int)=0
			 * Country (nvarchar)=Norvegia
			 */
			try 
			{
				if(isAuthenticated)
				{
					m_userName = Rainbow.Configuration.PortalSettings.CurrentUser.Identity.Name;
					m_email = Rainbow.Configuration.PortalSettings.CurrentUser.Identity.Email;
					m_userID = Convert.ToInt32(Rainbow.Configuration.PortalSettings.CurrentUser.Identity.ID);

					Rainbow.Configuration.PortalSettings portalSettings = (Rainbow.Configuration.PortalSettings)HttpContext.Current.Items["PortalSettings"];
					Rainbow.Security.UsersDB usersDB = new Rainbow.Security.UsersDB();
					System.Data.SqlClient.SqlDataReader dr = usersDB.GetSingleUser(m_email,portalSettings.PortalID);
					if(dr.Read())
					{
						m_userName	= dr["Name"].ToString();
						m_email		= dr["Email"].ToString();
						m_userID	= (int)dr["UserID"];
						m_location	= dr["Country"].ToString();
					}
					dr.Close();

					m_isAuthenticated = true;
					return;
				} 
			}
			catch(Exception x) 
			{
				throw new Exception("Failed to read user data from Rainbow.",x);
			}
			m_userName = "";
			m_email = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
		public object Location
		{
			get
			{
				return m_location;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
	}

	public class DotNetNukeUser : IForumUser
	{
		private	int		m_userID;
		private	string	m_userName;
		private string	m_email;
		private	string	m_firstName;
		private string	m_lastName;
		private string	m_location;
		private bool	m_isAuthenticated;

		/*
		 * UserID (int)=2
		 * Username (nvarchar)=host
		 * Password (nvarchar)=host
		 * Email (nvarchar)=host
		 * FullName (nvarchar)=Host Account
		 * FirstName (nvarchar)=Host
		 * LastName (nvarchar)=Account
		 * Unit (nvarchar)=
		 * Street (nvarchar)=
		 * City (nvarchar)=
		 * Region (nvarchar)=
		 * PostalCode (nvarchar)=
		 * Country (nvarchar)=
		 * Telephone (nvarchar)=
		 * IsSuperUser (bit)=True
		 * Authorized (bit)=
		 * CreatedDate (datetime)=
		 * LastLoginDate (datetime)=
		 */

		public DotNetNukeUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					DotNetNuke.UsersDB objUser = new DotNetNuke.UsersDB();
					DotNetNuke.PortalSettings _portalSettings = (DotNetNuke.PortalSettings)HttpContext.Current.Items["PortalSettings"];

					System.Data.SqlClient.SqlDataReader dr;
					if(HttpContext.Current.User.Identity.GetType()==typeof(System.Security.Principal.WindowsIdentity))
						dr = objUser.GetSingleUserByUsername(_portalSettings.PortalId,HttpContext.Current.User.Identity.Name);
					else
						dr = objUser.GetSingleUser(_portalSettings.PortalId,int.Parse(HttpContext.Current.User.Identity.Name));
					
					if(dr.Read())
					{
						m_userID			= (int)dr["UserId"];
						m_userName			= dr["Username"].ToString();
						m_email				= dr["Email"].ToString();
						m_firstName			= dr["FirstName"].ToString();
						m_lastName			= dr["LastName"].ToString();
						m_location			= dr["Country"].ToString();
					}
					dr.Close();

					m_isAuthenticated = true;
					return;
				}
			}
			catch(Exception x)
			{
				throw new Exception("Failed to find user info from DotNetNuke.",x);
			}
			m_userName = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return m_location;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
	}

	public class WindowsUser : IForumUser
	{
		private	string	m_userName;
		private	string	m_email;
		private	bool	m_isAuthenticated;

		public WindowsUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					string[] parts = userName.Split('\\');
					m_userName  = parts[parts.Length-1];
					if(parts.Length>1)
						m_email = String.Format("{0}@{1}",parts[1],parts[0]);
					else
						m_email = m_userName;
					
					m_isAuthenticated = true;
				}
			}
			catch(Exception)
			{
			}
			m_userName = "";
			m_email = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
	}
	
	public class FormsUser : IForumUser
	{
		private	string	m_userName;
		private	bool	m_isAuthenticated;

		public FormsUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					m_userName = userName;
					m_isAuthenticated = true;
					return;
				}
			}
			catch(Exception)
			{
			}
			m_userName = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
	}
	public class GuestUser : IForumUser
	{
		public string Name
		{
			get
			{
				return "";
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
	}
	#endregion

	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class ForumPage : System.Web.UI.UserControl
	{
		#region Variables
		private HiPerfTimer	hiTimer				= new HiPerfTimer(true);
		private DataRow		m_pageinfo;
		private string		m_strLoadMessage	= "";
		private string		m_strRefreshURL		= null;
		private bool		m_bNoDataBase		= false;
		private bool		m_bShowToolBar		= true;
		private bool		m_bCheckSuspended	= true;
		private IForumUser	m_forumUser			= null;

		public IForumUser User
		{
			get
			{
				return m_forumUser;
			}
		}
		
		#endregion
		#region Constructor and events
		/// <summary>
		/// Constructor
		/// </summary>
		public ForumPage(string transPage)
		{
			TransPage = transPage;

			this.Load += new System.EventHandler(this.ForumPage_Load);
			this.Error += new System.EventHandler(this.ForumPage_Error);
			this.PreRender += new EventHandler(ForumPage_PreRender);
		}

		private void ForumPage_Error(object sender, System.EventArgs e) 
		{
			// This doesn't seem to work...
			Exception x = Server.GetLastError();
			if(!IsLocal) 
				Utils.LogToMail(Server.GetLastError());
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForumPage_Load(object sender, System.EventArgs e) 
		{
#if DEBUG
			QueryCounter.Reset();
#endif

			// Set the culture and UI culture to the browser's accept language
			try 
			{
				string sCulture = Request.UserLanguages[0];
				if(sCulture.IndexOf(';')>=0) 
					sCulture = sCulture.Substring(0,sCulture.IndexOf(';'));

				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(sCulture);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(sCulture);
			}
			catch(Exception)
			{
			}

			//Response.Expires = -1000;
			Response.AddHeader("Cache-control","private, no-cache, must-revalidate");
			Response.AddHeader("Expires","Mon, 26 Jul 1997 05:00:00 GMT"); // Past date
			Response.AddHeader("Pragma","no-cache");

			if(m_bNoDataBase) return;

			DataTable banip = (DataTable)Cache["bannedip"];
			if(banip == null) 
			{
				banip = DB.bannedip_list(null);
				Cache["bannedip"] = banip;
			}
			foreach(DataRow row in banip.Rows)
				if(Utils.IsBanned((string)row["Mask"],Request.ServerVariables["REMOTE_ADDR"]))
					Response.End();

			// Find user name
			AuthType authType = Data.GetAuthType;
			switch(authType)
			{
				case AuthType.Guest:
					m_forumUser = new GuestUser();
					break;
				case AuthType.Rainbow:
					m_forumUser = new RainbowUser(Page.User.Identity.Name,Page.User.Identity.IsAuthenticated);
					break;
				case AuthType.DotNetNuke:
					m_forumUser = new DotNetNukeUser(Page.User.Identity.Name,Page.User.Identity.IsAuthenticated);
					break;
				case AuthType.Windows:
					m_forumUser = new WindowsUser(Page.User.Identity.Name,Page.User.Identity.IsAuthenticated);
					break;
				default:
					m_forumUser = new FormsUser(Page.User.Identity.Name,Page.User.Identity.IsAuthenticated);
					break;
			}

			string browser = String.Format("{0} {1}",Request.Browser.Browser,Request.Browser.Version);
			string platform = Request.Browser.Platform;

			if(Request.UserAgent.IndexOf("Windows NT 5.2")>=0)
				platform = "Win2003";

			m_pageinfo = DB.pageload(
				Session.SessionID,
				User.Name,
				Request.UserHostAddress,
				Request.FilePath,
				browser,
				platform,
				Request.QueryString["c"],
				Request.QueryString["f"],
				Request.QueryString["t"],
				Request.QueryString["m"]);

			// If user wasn't found and we have foreign 
			// authorization, try to register the user.
			if(m_pageinfo==null && authType!=AuthType.Forms && User.IsAuthenticated) 
			{
				if(!DB.user_register(this,User.Name,"ext",User.Email,User.Location,User.HomePage,0,false))
					throw new ApplicationException("User registration failed.");

				m_pageinfo = DB.pageload(
					Session.SessionID,
					User.Name,
					Request.UserHostAddress,
					Request.FilePath,
					Request.Browser.Browser,
					Request.Browser.Platform,
					Request.QueryString["c"],
					Request.QueryString["f"],
					Request.QueryString["t"],
					Request.QueryString["m"]);
			}

			if(m_pageinfo==null) 
			{
				if(User.IsAuthenticated) 
					throw new ApplicationException(string.Format("User '{0}' not in database.",User.Name));
				else
					throw new ApplicationException("Failed to find guest user.");
			}

			if(CheckSuspended && IsSuspended) 
			{
				if(SuspendedTo < DateTime.Now) 
				{
					DB.user_suspend(PageUserID,null);
					Response.Redirect(Request.RawUrl);
				}
				Forum.Redirect(Pages.info,"i=2");
			}

			if(Request.Cookies["yaf"]!=null) 
			{
				Response.Cookies.Add(Request.Cookies["yaf"]);
				Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
			}

			if(Session["lastvisit"] == null && (int)m_pageinfo["Incoming"]>0) 
			{
				AddLoadMessage(String.Format("You have {0} unread message(s) in your Inbox",m_pageinfo["Incoming"]));
			}

			if(Session["lastvisit"] == null && Request.Cookies["yaf"] != null && Request.Cookies["yaf"]["lastvisit"] != null) 
			{
				try 
				{
					Session["lastvisit"] = DateTime.Parse(Request.Cookies["yaf"]["lastvisit"]);
				}
				catch(Exception) 
				{
					Session["lastvisit"] = DateTime.Now;
				}
				Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
				Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
			}
			else if(Session["lastvisit"] == null) 
			{
				Session["lastvisit"] = DateTime.Now;
			}

			if(Request.Cookies["yaf"] != null && Request.Cookies["yaf"]["lastvisit"] != null) 
			{
				try 
				{
					if(DateTime.Parse(Request.Cookies["yaf"]["lastvisit"]) < DateTime.Now - TimeSpan.FromMinutes(5)) 
					{
						Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
						Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
					}
				}
				catch(Exception) 
				{
					Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
					Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
				}
			}
			else 
			{
				Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
				Response.Cookies["yaf"].Expires = DateTime.Now.AddYears(1);
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
							Utils.SendMail(Config.ForumSettings.ForumEmail,(string)dt.Rows[i]["ToUser"],(string)dt.Rows[i]["Subject"],(string)dt.Rows[i]["Body"]);
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

		public DateTime GetForumRead(int forumID)
		{
			System.Collections.Hashtable t = (System.Collections.Hashtable)Session["forumread"];
			if(t==null || !t.ContainsKey(forumID)) 
				return (DateTime)Session["lastvisit"];
			else
				return (DateTime)t[forumID];
		}
		public void SetForumRead(int forumID,DateTime date) 
		{
			System.Collections.Hashtable t = (System.Collections.Hashtable)Session["forumread"];
			if(t==null) 
			{
				t = new System.Collections.Hashtable();
			}
			t[forumID] = date;
			Session["forumread"] = t;
		}
		public DateTime GetTopicRead(int topicID)
		{
			System.Collections.Hashtable t = (System.Collections.Hashtable)Session["topicread"];
			if(t==null || !t.ContainsKey(topicID)) 
				return (DateTime)Session["lastvisit"];
			else
				return (DateTime)t[topicID];
		}
		public void SetTopicRead(int topicID,DateTime date) 
		{
			System.Collections.Hashtable t = (System.Collections.Hashtable)Session["topicread"];
			if(t==null) 
			{
				t = new System.Collections.Hashtable();
			}
			t[topicID] = date;
			Session["topicread"] = t;
		}
		#endregion
		#region Theme Functions
		// XML THEME FILE (TEST)
		private XmlDocument LoadTheme(string themefile) 
		{
			if(themefile==null) 
			{
				if(m_pageinfo==null || m_pageinfo.IsNull("ThemeFile") || !Config.ForumSettings.AllowUserTheme)
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
				doc.Load(System.Web.HttpContext.Current.Server.MapPath(String.Format("{0}themes/{1}",ForumRoot,themefile)));
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
			contents = contents.Replace("~",String.Format("{0}themes/{1}",ForumRoot,themeDir));
			return contents;
		}
		#endregion
		#region Render Functions
		private Forum m_forumControl = null;
		public yaf.Forum ForumControl
		{
			get 
			{
				if(m_forumControl!=null)
					return m_forumControl;

				System.Web.UI.Control ctl = Parent;
				while(ctl.GetType()!=typeof(yaf.Forum))
					ctl = ctl.Parent;

				m_forumControl = (yaf.Forum)ctl;
				return m_forumControl;
			}
			set
			{
				m_forumControl = value;
			}
		}

		private string	m_headerInfo = null;

		private void ForumPage_PreRender(object sender,EventArgs e)
		{
			System.Web.UI.HtmlControls.HtmlGenericControl ctl;
			ctl = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.FindControl("ForumTitle");
			if(ctl!=null)
				ctl.InnerText = Config.ForumSettings.Name;

			/// BEGIN HEADER
			StringBuilder header = new StringBuilder();
			header.AppendFormat("<table width=100% cellspacing=0 class=content cellpadding=0><tr>");

			if(User.IsAuthenticated) 
			{
				header.AppendFormat(String.Format("<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>",String.Format(GetText("TOOLBAR","LOGGED_IN_AS"),PageUserName)));

				header.AppendFormat("<td style=\"padding:5px\" align=right valign=middle class=post>");
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.search),GetText("TOOLBAR","SEARCH")));
				if(IsAdmin)
					header.AppendFormat(String.Format("	<a target='_top' href=\"{0}admin/\">{1}</a> | ",ForumRoot,GetText("TOOLBAR","ADMIN")));
				if(IsModerator || IsForumModerator)
					header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.moderate_index),GetText("TOOLBAR","MODERATE")));
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.active),GetText("TOOLBAR","ACTIVETOPICS")));
				if(!IsGuest)
					header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.cp_profile),GetText("TOOLBAR","MYPROFILE")));
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.members),GetText("TOOLBAR","MEMBERS")));
				if(User.CanLogin)
					header.AppendFormat(String.Format(" | <a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.logout),GetText("TOOLBAR","LOGOUT")));
			} 
			else 
			{
				header.AppendFormat(String.Format("<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>",GetText("TOOLBAR","WELCOME_GUEST")));

				header.AppendFormat("<td style=\"padding:5px\" align=right valign=middle class=post>");
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.search),GetText("TOOLBAR","SEARCH")));
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.active),GetText("TOOLBAR","ACTIVETOPICS")));
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.members),GetText("TOOLBAR","MEMBERS")));
				if(User.CanLogin) 
				{
					header.AppendFormat(String.Format(" | <a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.login),GetText("TOOLBAR","LOGIN")));
					header.AppendFormat(String.Format(" | <a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.rules),GetText("TOOLBAR","REGISTER")));
				}
			}
			header.AppendFormat("</td></tr></table>");
			header.AppendFormat("<br />");
			if(ForumControl.Header!=null)
				ForumControl.Header.Info = header.ToString();
			else
				m_headerInfo = header.ToString();
			/// END HEADER
		}
		/// <summary>
		/// Writes the document
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			if(m_bShowToolBar) 
			{
				writer.WriteLine("<link type='text/css' rel='stylesheet' href='{0}forum.css' />",ForumRoot);
				writer.WriteLine("<link type='text/css' rel='stylesheet' href='{0}' />",ThemeFile("theme.css"));
				string script = "";
				if(m_strLoadMessage.Length>0)
					script = String.Format("<script language='javascript'>\nonload=function(){1}\nalert(\"{0}\")\n{2}\n</script>\n",m_strLoadMessage,'{','}');

#if TODO
				if(m_strRefreshURL!=null) 
					script = script.Insert(0,String.Format("<meta HTTP-EQUIV=\"Refresh\" CONTENT=\"10;{0}\">\n",m_strRefreshURL));
#endif

				/// BEGIN HEADER
				if(m_headerInfo!=null)
					writer.Write(m_headerInfo);
				/// END HEADER

				RenderBody(writer);

				/// BEGIN FOOTER
				StringBuilder footer = new StringBuilder();
				footer.AppendFormat("<p style=\"text-align:center;font-size:7pt\">");

				if(Config.IsDotNetNuke) 
				{
					footer.AppendFormat("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under DotNetNuke.",AppVersionName);
					footer.AppendFormat("<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.");
				} 
				else if(Config.IsRainbow)
				{
					footer.AppendFormat("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under Rainbow.",AppVersionName);
					footer.AppendFormat("<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.");
				}
				else 
				{
					footer.AppendFormat(GetText("COMMON","POWERED_BY"),
						String.Format("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a>"),
						String.Format("{0} - {1}",AppVersionName,FormatDateShort(AppVersionDate))
						);
					footer.AppendFormat("<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.");
					footer.AppendFormat("<br/>");
					hiTimer.Stop();
					footer.AppendFormat(GetText("COMMON","GENERATED"),hiTimer.Duration);
				}

#if DEBUG
				footer.AppendFormat("<br/>{0} queries ({1:N3} seconds, {2:N2}%).<br/>{3}",QueryCounter.Count,QueryCounter.Duration,100 * QueryCounter.Duration/hiTimer.Duration,QueryCounter.Commands);
#endif
				footer.AppendFormat("</p>");
				if(ForumControl.Footer!=null)
					ForumControl.Footer.Info = footer.ToString();
				else
					writer.Write(footer.ToString());
				/// END FOOTER

				writer.WriteLine(script);
			} 
			else 
			{
				writer.WriteLine("<html>");
				writer.WriteLine("<!-- Copyright 2003 Bjørnar Henden -->");
				writer.WriteLine("<head>");
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}forum.css>",ForumRoot));
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}>",ThemeFile("theme.css")));
				writer.WriteLine(String.Format("<title>{0}</title>",Config.ForumSettings.Name));
				if(m_strRefreshURL!=null) 
					writer.WriteLine(String.Format("<meta HTTP-EQUIV=\"Refresh\" CONTENT=\"10;{0}\">",m_strRefreshURL));
				writer.WriteLine("</head>");
				writer.WriteLine("<body onload='yaf_onload()'>");
				
				RenderBody(writer);
				writer.WriteLine("<script>");
				writer.WriteLine("function yaf_onload() {");
				if(m_strLoadMessage.Length>0)
					writer.WriteLine(String.Format("	alert(\"{0}\");",m_strLoadMessage));
				writer.WriteLine("}");
				writer.WriteLine("yaf_onload();");
				writer.WriteLine("</script>");
				
				writer.WriteLine("</body>");
				writer.WriteLine("</html>");
			}
		}

		/// <summary>
		/// Renders the body
		/// </summary>
		/// <param name="writer"></param>
		protected virtual void RenderBody(System.Web.UI.HtmlTextWriter writer) 
		{
			RenderBase(writer);
		}

		/// <summary>
		/// Calls the base class to render components
		/// </summary>
		/// <param name="writer"></param>
		protected void RenderBase(System.Web.UI.HtmlTextWriter writer) 
		{
			base.Render(writer);
		}

		#endregion
		#region Page/User properties
		/// <summary>
		/// Set to true if this is the start page. Should only be set by the page that initialized the database.
		/// </summary>
		protected bool NoDataBase 
		{
			set 
			{
				m_bNoDataBase = value;
			}
		}
		/// <summary>
		/// Returns the forum url
		/// </summary>
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
				long port = long.Parse(Request.ServerVariables["SERVER_PORT"]);
				if(port!=80)
					return String.Format("http://{0}:{1}",Request.ServerVariables["SERVER_NAME"],port);
				else
					return String.Format("http://{0}",Request.ServerVariables["SERVER_NAME"]);
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
		protected string PageForumName 
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
		protected int PageCategoryID 
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
		protected string PageCategoryName 
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
		/// <summary>
		/// True if current user is an administrator
		/// </summary>
		public bool IsAdmin 
		{
			get 
			{
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
		#endregion
		#region Other
		/// <summary>
		/// Find the path of a smiley icon
		/// </summary>
		/// <param name="icon">The file name of the icon you want</param>
		/// <returns>The path to the image file</returns>
		public string Smiley(string icon) 
		{
			return String.Format("{0}images/emoticons/{1}",ForumRoot,icon);
		}
		/// <summary>
		/// The directory of theme files
		/// </summary>
		protected string ThemeDir 
		{
			get 
			{
				XmlDocument doc = LoadTheme(null);
				return String.Format("{0}themes/{1}/",ForumRoot,doc.DocumentElement.Attributes["dir"].Value);
			}
		}

		/// <summary>
		/// Base directory for forum
		/// </summary>
		public string ForumRoot
		{
			get 
			{
				return Data.BaseDir;
			}
		}

		/// <summary>
		/// Returns the full path of a file in the themedir
		/// </summary>
		/// <param name="filename">The name of the file relative to the theme directory</param>
		/// <returns></returns>
		public string ThemeFile(string filename) 
		{
			return ThemeDir + filename;
		}

		/// <summary>
		/// Adds a message that is displayed to the user when the page is loaded.
		/// </summary>
		/// <param name="msg">The message to display</param>
		public void AddLoadMessage(string msg) 
		{
			msg = msg.Replace("\\","\\\\");
			msg = msg.Replace("'","\\'");
			msg = msg.Replace("\r\n","\\r\\n");
			msg = msg.Replace("\n","\\n");
			msg = msg.Replace("\"","\\\"");
			m_strLoadMessage += msg + "\\n\\n";
		}

		public string RefreshURL
		{
			set 
			{
				m_strRefreshURL = value;
			}
		}

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

		public bool CheckSuspended 
		{
			set 
			{
				m_bCheckSuspended = value;
			}
			get 
			{
				return m_bCheckSuspended;
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
				return TimeZoneOffsetUser - Config.ForumSettings.TimeZone;
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
		#region Layout functions
		/// <summary>
		/// Set to false if you don't want the menus at top and bottom. Only admin pages will set this to false
		/// </summary>
		protected bool ShowToolBar 
		{
			set 
			{
				m_bShowToolBar = value;
			}
		}

		public int UnreadPrivate 
		{
			get 
			{
				return (int)m_pageinfo["Incoming"];
			}
		}

		public bool IsLocal 
		{
			get 
			{
				string s = Request.ServerVariables["SERVER_NAME"];
				return s!=null && s.ToLower()=="localhost";
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

			if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !Config.ForumSettings.AllowUserLanguage)
				filename = Config.ConfigSection["language"];
			else
				filename = (string)m_pageinfo["LanguageFile"];

			if(filename==null)
				filename = "english.xml";

#if !DEBUG
			if(m_localizer==null && Cache["Localizer." + filename]!=null)
				m_localizer = (Localizer)Cache["Localizer." + filename];
#endif
			if(m_localizer==null) 
			{

				m_localizer = new Localizer(Server.MapPath(String.Format("{0}languages/{1}",ForumRoot,filename)));
#if !DEBUG
				Cache["Localizer." + filename] = m_localizer;
#endif
			}
			/// If not using default language load that too
			if(filename.ToLower()!="english.xml") 
			{
#if !DEBUG
				if(m_defaultLocale==null && Cache["DefaultLocale"]!=null)
					m_defaultLocale = (Localizer)Cache["DefaultLocale"];
#endif

				if(m_defaultLocale==null) 
				{
					m_defaultLocale = new Localizer(Server.MapPath(String.Format("{0}languages/english.xml",ForumRoot)));
#if !DEBUG
					Cache["DefaultLocale"] = m_defaultLocale;
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

				if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !Config.ForumSettings.AllowUserLanguage)
					filename = Config.ConfigSection["language"];
				else
					filename = (string)m_pageinfo["LanguageFile"];

				if(filename==null)
					filename = "english.xml";

				Cache.Remove("Localizer." + filename);
#endif
				throw new Exception(String.Format("Missing translation for {1}.{0}",text.ToUpper(),page.ToUpper()));
			}
			str = str.Replace("[b]","<b>");
			str = str.Replace("[/b]","</b>");
			return str;
		}
		#endregion
		#region Version Information
		static public string AppVersionNameFromCode(long code) 
		{
			if((code & 0xFF)>0)
				return String.Format("{0}.{1}.{2}.{3}",(code>>24) & 0xFF,(code>>16) & 0xFF,(code>>8) & 0xFF,code & 0xFF);
			else
				return String.Format("{0}.{1}.{2}",(code>>24) & 0xFF,(code>>16) & 0xFF,(code>>8) & 0xFF);
		}
		static public string AppVersionName 
		{
			get 
			{
				return AppVersionNameFromCode(AppVersionCode);
			}
		}
		static public int AppVersion 
		{
			get 
			{
				return 12;
			}
		}
		static public long AppVersionCode 
		{
			get 
			{
				return 0x00090600;
			}
		}
		static public DateTime AppVersionDate 
		{
			get 
			{
				return new DateTime(2003,11,25);
			}
		}
		#endregion
	}
}
