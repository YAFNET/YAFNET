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
using yaf.pages;

// Grønn: #25C110
// Brown: #D0BF8C

namespace yaf
{
	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class BaseAdminPage : System.Web.UI.Page
	{
		#region Variables
		private DataRow		m_pageinfo;
		private string		m_strLoadMessage	= "";
		private string		m_strRefreshURL		= null;
		private bool		m_bNoDataBase		= false;
		private bool		m_bCheckSuspended	= true;
		private IForumUser	m_forumUser			= null;

		public new IForumUser User
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
		public BaseAdminPage()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Error += new System.EventHandler(this.Page_Error);
		}

		private void Page_Error(object sender, System.EventArgs e) 
		{
			if(!IsLocal) 
				Utils.LogToMail(Server.GetLastError());
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e) 
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
			if(banip == null) {
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
					m_forumUser = new RainbowUser(base.User.Identity.Name,base.User.Identity.IsAuthenticated);
					break;
				case AuthType.DotNetNuke:
					m_forumUser = new DotNetNukeUser(base.User.Identity.Name,base.User.Identity.IsAuthenticated);
					break;
				case AuthType.Windows:
					m_forumUser = new WindowsUser(base.User.Identity.Name,base.User.Identity.IsAuthenticated);
					break;
				default:
					m_forumUser = new FormsUser(base.User.Identity.Name,base.User.Identity.IsAuthenticated);
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

#if false
			// If user wasn't found and we have foreign 
			// authorization, try to register the user.
			if(m_pageinfo==null && authType!=AuthType.YetAnotherForum) 
			{
				DB.user_register(this,sUserIdentityName,"ext",sUserEmail,null,null,0,false);

				m_pageinfo = DB.pageload(
					Session.SessionID,
					sUserIdentityName,
					Request.UserHostAddress,
					Request.FilePath,
					Request.Browser.Browser,
					Request.Browser.Platform,
					Request.QueryString["c"],
					Request.QueryString["f"],
					Request.QueryString["t"],
					Request.QueryString["m"]);
			}
#endif
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
			if((int)m_pageinfo["MailsPending"]>0) {
				try {
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
				catch(Exception x) {
					if(IsAdmin) {
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
			string langCode = "en"; //LoadTranslation().ToUpper();
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
		/// <summary>
		/// Writes the document
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteLine("<html>");
			writer.WriteLine("<!-- Copyright 2003 Bjørnar Henden -->");
			writer.WriteLine("<head>");
			writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}forum.css>",ForumRoot));
			writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}>",ThemeFile("theme.css")));
			writer.WriteLine(String.Format("<title>{0}</title>",Config.ForumSettings.Name));
			writer.WriteLine("<script>");
			writer.WriteLine("function yaf_onload() {");
			if(m_strLoadMessage.Length>0)
				writer.WriteLine(String.Format("	alert(\"{0}\");",m_strLoadMessage));
			writer.WriteLine("}");
			writer.WriteLine("</script>");
			if(m_strRefreshURL!=null) 
				writer.WriteLine(String.Format("<meta HTTP-EQUIV=\"Refresh\" CONTENT=\"10;{0}\">",m_strRefreshURL));
			writer.WriteLine("</head>");
			writer.WriteLine("<body onload='yaf_onload()'>");
			
			RenderBody(writer);
			
			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
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
		/// True if current user is an administrator
		/// </summary>
		protected bool IsAdmin 
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
		protected bool IsGuest 
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
		protected bool IsForumModerator 
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
		protected bool IsModerator
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
		#region Other
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

		/// <summary>
		/// The smtp server to send mails from
		/// </summary>
		public string ReadTemplate(string name) 
		{
			string file;
			if(Cache[name] != null && false) {
				file = Cache[name].ToString();
			} else {
				string templatefile = Server.MapPath(String.Format("{0}templates/{1}",ForumRoot,name));
				StreamReader sr = new StreamReader(templatefile,Encoding.ASCII);
				file = sr.ReadToEnd();
				sr.Close();
				Cache[name] = file;
			}
			return file;
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
		public TimeSpan TimeZoneOffsetUser {
			get {
				if(m_pageinfo!=null) {
					int min = (int)m_pageinfo["TimeZoneUser"];
					return new TimeSpan(min/60,min%60,0);
				} else
					return new TimeSpan(0);
			}
		}
		/// <summary>
		/// Returns the time zone offset for the current user compared to the forum time zone.
		/// </summary>
		public TimeSpan TimeOffset {
			get {
				return TimeZoneOffsetUser - Config.ForumSettings.TimeZone;
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateTime(object o) {
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
		public string FormatDateShort(object o) {
			DateTime dt = (DateTime)o;
			return String.Format("{0:d}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatTime(DateTime dt) {
			return String.Format("{0:T}",dt + TimeOffset);
		}
		#endregion
		#region Layout functions
		public bool IsLocal 
		{
			get 
			{
				string s = Request.ServerVariables["SERVER_NAME"];
				return s!=null && s.ToLower()=="localhost";
			}
		}

		#endregion
	}
}
