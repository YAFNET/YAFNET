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
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using yaf.classes;

// Grønn: #25C110
// Brown: #D0BF8C

namespace yaf
{
	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class BasePage : System.Web.UI.Page
	{
		//private long m_timeStart = DateTime.Now.Ticks / 10000;
		private HiPerfTimer hiTimer = new HiPerfTimer(true);
		private string m_strLoadMessage = "";
		private string m_strForumName = "Yet Another Forum.net";
		private string m_strThemeDir = System.Configuration.ConfigurationSettings.AppSettings["themedir"];
		private string m_strBaseDir = System.Configuration.ConfigurationSettings.AppSettings["basedir"];
		private bool m_bTopMenu = true;
		protected DataRow	pageinfo;
		private bool m_bStartPage = false;
		private string m_strSmtpServer = System.Configuration.ConfigurationSettings.AppSettings["smtpserver"];
		private string m_strForumEmail = System.Configuration.ConfigurationSettings.AppSettings["forumemail"];

		/// <summary>
		/// Constructor
		/// </summary>
		public BasePage()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Error += new System.EventHandler(this.Page_Error);
		}

		private void Page_Error(object sender, System.EventArgs e) 
		{
			if(!IsLocal) 
				yaf.Utils.ReportError(Server.GetLastError());
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e) 
		{
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

			if(m_bStartPage) return;

			DataTable banip = (DataTable)Cache["bannedip"];
			if(banip == null) {
				banip = DataManager.GetData("yaf_bannedip_list",CommandType.StoredProcedure);
				Cache["bannedip"] = banip;
			}
			for(int i=0;i<banip.Rows.Count;i++) {
				if(Utils.IsBanned((string)banip.Rows[i]["Mask"],Request.ServerVariables["REMOTE_ADDR"]))
					Response.End();
			}


			using(SqlCommand cmd = new SqlCommand("yaf_pageload")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SessionID",Session.SessionID);
				cmd.Parameters.Add("@User",User.Identity.Name);
				cmd.Parameters.Add("@IP",Request.UserHostAddress);
				cmd.Parameters.Add("@Location",Request.FilePath);
				cmd.Parameters.Add("@Browser",Request.Browser.Browser);
				cmd.Parameters.Add("@Platform",Request.Browser.Platform);
				cmd.Parameters.Add("@CategoryID",Request.QueryString["c"]);
				cmd.Parameters.Add("@ForumID",Request.QueryString["f"]);
				cmd.Parameters.Add("@TopicID",Request.QueryString["t"]);
				cmd.Parameters.Add("@MessageID",Request.QueryString["m"]);
				DataTable dt = DataManager.GetData(cmd);
				if(dt.Rows.Count==0) {
					if(User.Identity.IsAuthenticated) {
						System.Web.Security.FormsAuthentication.SignOut();
						Response.Redirect(BaseDir);
					} else
						throw new Exception("Couldn't find user.");
				}
				pageinfo = dt.Rows[0];
			}

			m_strForumName = (string)pageinfo["BBName"];
			m_strSmtpServer = (string)pageinfo["SmtpServer"];
			m_strForumEmail = (string)pageinfo["ForumEmail"];

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
				Response.Cookies["yaf"].Expires = DateTime.Now + TimeSpan.FromDays(365);
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
						Response.Cookies["yaf"].Expires = DateTime.Now + TimeSpan.FromDays(365);
					}
				}
				catch(Exception) 
				{
					Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
					Response.Cookies["yaf"].Expires = DateTime.Now + TimeSpan.FromDays(365);
				}
			}
			else 
			{
				Response.Cookies["yaf"]["lastvisit"] = DateTime.Now.ToString();
				Response.Cookies["yaf"].Expires = DateTime.Now + TimeSpan.FromDays(365);
			}

			// Check if pending mails, and send 10 of them if possible
			if((int)pageinfo["MailsPending"]>0) {
				try {
					DataTable dt = DataManager.GetData("yaf_mail_list",CommandType.StoredProcedure);
					for(int i=0;i<dt.Rows.Count;i++) {
						// Build a MailMessage
						SendMail(ForumEmail,(string)dt.Rows[i]["ToUser"],(string)dt.Rows[i]["Subject"],(string)dt.Rows[i]["Body"]);
						using(SqlCommand cmd = new SqlCommand("yaf_mail_delete")) {
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add("@MailID",dt.Rows[i]["MailID"]);
							DataManager.ExecuteNonQuery(cmd);
						}
					}
					if(IsAdmin) AddLoadMessage(String.Format("Sent {0} mails.",dt.Rows.Count));
				}
				catch(Exception x) {
					if(IsAdmin) {
						AddLoadMessage(x.Message);
					}
				}
			}
		}

		/// <summary>
		/// Writes the document
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			string html = ReadTemplate("page.html");
			/*
			if(Cache["html"] != null && false) {
				html = Cache["html"].ToString();
			} else {
				string templatefile = Server.MapPath(String.Format("{0}templates/page.html",BaseDir));
				StreamReader sr = new StreamReader(templatefile,Encoding.ASCII);
				html = sr.ReadToEnd();
				sr.Close();
				Cache["html"] = html;
			}
			*/
			
			if(!m_bTopMenu) {
				writer.WriteLine("<html>");
				writer.WriteLine("<!-- Copyright 2003 Bjørnar Henden -->");
				writer.WriteLine("<head>");
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}forum.css>",BaseDir));
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}>",ThemeFile("theme.css")));
				writer.WriteLine(String.Format("<title>{0}</title>",ForumName));
				writer.WriteLine("<script>");
				writer.WriteLine("	function yaf_onload() {");
				if(m_strLoadMessage.Length>0)
					writer.WriteLine(String.Format("		alert(\"{0}\");",m_strLoadMessage));
				writer.WriteLine("	}");
				writer.WriteLine("</script>");
				writer.WriteLine("</head>");
				writer.WriteLine("<body onload='yaf_onload()'>");
			} else {
				string title = String.Format("<title>{0}</title>",ForumName);
				string css = String.Format("<link type=text/css rel=stylesheet href='{0}forum.css' />",BaseDir);
				css += String.Format("\n<link type=text/css rel=stylesheet href='{0}' />",ThemeFile("theme.css"));
				//string script = String.Format("<script>\nfunction yaf_onload() {{\n{0}\n\}}\n</script>\n","//thescript");
				string script = String.Format("<script>\nfunction yaf_onload() {1}\n{0}\n{2}\n</script>\n","//thescript",'{','}');
				if(m_strLoadMessage.Length>0)
					script = String.Format("<script>\nfunction yaf_onload() {1}\nalert(\"{0}\")\n{2}\n</script>\n",m_strLoadMessage,'{','}');

				html = html.Replace("{title}",title);
				html = html.Replace("{css}",css);
				html = html.Replace("{script}",script);
			}

			if(m_bTopMenu) 
			{
				int pos = html.IndexOf("{forum}");
				if(pos<0)
					throw new Exception("Invalid template -- {forum} constant is missing.");

				writer.Write(html.Substring(0,pos));	// Write html before forum


				//writer.WriteLine("<div style=\"text-align:right\">");
				writer.WriteLine("<table width=100% cellspacing=0 class=content cellpadding=0><tr>");

				//writer.WriteLine("	<a class=\"nav1\" href=\"\">Search</a>");
				if(User.Identity.IsAuthenticated) 
				{
					writer.WriteLine(String.Format("<td style=\"padding:5px\" class=post align=left><b>Logged in as: {0}</b></td>",User.Identity.Name));

					writer.WriteLine("<td style=\"padding:5px\" align=right valign=middle class=post>");
					if(IsAdmin)
						writer.WriteLine(String.Format("	<a href=\"admin/\">Admin</a> |"));
					//writer.WriteLine("	<a class=\"nav1\" href=\"\">Private Messenger</a>");
					writer.WriteLine("	<a href=\"active.aspx\">Active Topics</a> |");
					if(!IsGuest)
						writer.WriteLine(String.Format("	<a href=\"cp_profile.aspx\">My Profile</a> |"));
					writer.WriteLine(String.Format("	<a href=\"members.aspx\">Members</a> |"));
					writer.WriteLine(String.Format("	<a href=\"logout.aspx\">Logout</a>"));
				} 
				else 
				{
					writer.WriteLine("<td style=\"padding:5px\" class=post align=left><b>Welcome Guest</b></td>");

					writer.WriteLine("<td style=\"padding:5px\" align=right valign=middle class=post>");
					writer.WriteLine(String.Format("	<a href=\"login.aspx\">Log In</a> |"));
					writer.WriteLine(String.Format("	<a href=\"rules.aspx\">Register</a> |"));
					writer.WriteLine(String.Format("	<a href=\"members.aspx\">Members</a>"));
				}
				writer.WriteLine("</td></tr></table>");
				writer.WriteLine("<br />");


				RenderBody(writer);
				writer.WriteLine("<p style=\"text-align:center;font-size:7pt\">");

				writer.WriteLine(String.Format("Powered by <a title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">{0}</a> version {1} - {2}<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.","Yet Another Forum.net",AppVersionName,FormatDateShort(AppVersionDate)));
				//long timeUsed = DateTime.Now.Ticks / 10000 - m_timeStart;
				//writer.WriteLine(String.Format(CustomCulture,"<br>This page was generated in {0:N3} seconds.",timeUsed/1000.0));
				hiTimer.Stop();
				writer.WriteLine(String.Format(CustomCulture,"<br>This page was generated in {0:N3} seconds.",hiTimer.Duration));

				writer.WriteLine("</p>");
				writer.Write(html.Substring(pos+8));	// Write html after forum
			} else {
				RenderBody(writer);
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

		/// <summary>
		/// Set to false if you don't want the menus at top and bottom. Only admin pages will set this to false
		/// </summary>
		protected bool TopMenu 
		{
			set 
			{
				m_bTopMenu = value;
			}
		}

		/// <summary>
		/// The name of the froum
		/// </summary>
		protected string ForumName 
		{
			get 
			{
				return m_strForumName;
			}
		}
		/// <summary>
		/// Returns the forum url
		/// </summary>
		public string ForumURL {
			get {
				return String.Format("http://{0}{1}",Request.ServerVariables["SERVER_NAME"],BaseDir);
			}
		}
		/// <summary>
		/// Find the path of a smiley icon
		/// </summary>
		/// <param name="icon">The file name of the icon you want</param>
		/// <returns>The path to the image file</returns>
		public string Smiley(string icon) {
			return String.Format("{0}images/emoticons/{1}",BaseDir,icon);
		}
		/// <summary>
		/// The directory of theme files
		/// </summary>
		protected string ThemeDir 
		{
			get 
			{
				return m_strThemeDir;
			}
		}

		/// <summary>
		/// Base directory for forum
		/// </summary>
		protected string BaseDir 
		{
			get 
			{
				return m_strBaseDir;
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
			msg = msg.Replace("'","\\'");
			msg = msg.Replace("\r\n","\\r\\n");
			msg = msg.Replace("\n","\\n");
			msg = msg.Replace("\\","\\\\");
			msg = msg.Replace("\"","\\\"");
			m_strLoadMessage += msg + "\\n\\n";
		}

		/// <summary>
		/// The CultureInfo for the current user
		/// </summary>
		protected CultureInfo CustomCulture {
			get {
				return Thread.CurrentThread.CurrentCulture;
			}
		}

		/// <summary>
		/// Set to true if this is the start page. Should only be set by the page that initialized the database.
		/// </summary>
		protected bool StartPage {
			set {
				m_bStartPage = value;
			}
		}
		/// <summary>
		/// The smtp server to send mails from
		/// </summary>
		public string SmtpServer {
			get {
				return m_strSmtpServer;
			}
		}
		public string SmtpUserName 
		{
			get 
			{
				string tmp = pageinfo["SmtpUserName"].ToString();
				return tmp.Length>0 ? tmp : null;
			}
		}
		public string SmtpUserPass
		{
			get 
			{
				string tmp = pageinfo["SmtpUserPass"].ToString();
				return tmp.Length>0 ? tmp : null;
			}
		}
		/// <summary>
		/// The official forum email address. 
		/// </summary>
		public string ForumEmail 
		{
			get {
				return m_strForumEmail;
			}
		}

		public bool UseBlankLinks 
		{
			get 
			{
				return (bool)pageinfo["BlankLinks"];
			}
		}

		public string ReadTemplate(string name) {
			string file;
			if(Cache[name] != null && false) {
				file = Cache[name].ToString();
			} else {
				string templatefile = Server.MapPath(String.Format("{0}templates/{1}",BaseDir,name));
				StreamReader sr = new StreamReader(templatefile,Encoding.ASCII);
				file = sr.ReadToEnd();
				sr.Close();
				Cache[name] = file;
			}
			return file;
		}

		public bool IsLocal {
			get {
				string s = Request.ServerVariables["SERVER_NAME"];
				return s!=null && s.ToLower()=="localhost";
			}
		}

		static public int AppVersion 
		{
			get 
			{
				return 5;
			}
		}
		static public string AppVersionName 
		{
			get 
			{
				return "0.8.2";
			}
		}
		static public DateTime AppVersionDate 
		{
			get 
			{
				return new DateTime(2003,9,25);
			}
		}

		#region User access functions
		/// <summary>
		/// The UserID of the current user.
		/// </summary>
		public int PageUserID {
			get {
				if(pageinfo!=null)
					return (int)pageinfo["UserID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// ForumID for the current page, or 0 if not in any forum
		/// </summary>
		protected int PageForumID {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("ForumID"))
					return (int)pageinfo["ForumID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of forum for the current page, or an empty string if not in any forum
		/// </summary>
		protected string PageForumName {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("ForumName"))
					return (string)pageinfo["ForumName"];
				else
					return "";
			}
		}
		/// <summary>
		/// CategoryID for the current page, or 0 if not in any category
		/// </summary>
		protected int PageCategoryID {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("CategoryID"))
					return (int)pageinfo["CategoryID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of category for the current page, or an empty string if not in any category
		/// </summary>
		protected string PageCategoryName {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("CategoryName"))
					return (string)pageinfo["CategoryName"];
				else
					return "";
			}
		}
		/// <summary>
		/// The TopicID of the current page, or 0 if not in any topic
		/// </summary>
		protected int PageTopicID {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("TopicID"))
					return (int)pageinfo["TopicID"];
				else
					return 0;
			}
		}
		/// <summary>
		/// Name of topic for the current page, or an empty string if not in any topic
		/// </summary>
		protected string PageTopicName {
			get {
				if(pageinfo!=null && !pageinfo.IsNull("TopicName"))
					return (string)pageinfo["TopicName"];
				else
					return "";
			}
		}
		/// <summary>
		/// True if current user is an administrator
		/// </summary>
		protected bool IsAdmin {
			get {
				if(pageinfo!=null)
					return (bool)pageinfo["IsAdmin"];
				else
					return false;
			}
		}
		/// <summary>
		/// True if the current user is a guest
		/// </summary>
		protected bool IsGuest {
			get {
				if(pageinfo!=null)
					return (bool)pageinfo["IsGuest"];
				else
					return false;
			}
		}
		/// <summary>
		/// True if current user has post access in the current forum
		/// </summary>
		protected bool ForumPostAccess {
			get {
				if(pageinfo.IsNull("PostAccess"))
					return false;
				else
					return (bool)pageinfo["PostAccess"];
			}
		}
		/// <summary>
		/// True if the current user has reply access in the current forum
		/// </summary>
 		protected bool ForumReplyAccess {
			get {
				if(pageinfo.IsNull("ReplyAccess"))
					return false;
				else
					return (bool)pageinfo["ReplyAccess"];
			}
		}
		/// <summary>
		/// True if the current user has read access in the current forum
		/// </summary>
		protected bool ForumReadAccess {
			get {
				if(pageinfo.IsNull("ReadAccess"))
					return false;
				else
					return (bool)pageinfo["ReadAccess"];
			}
		}
		/// <summary>
		/// True if the current user has access to create priority topics in the current forum
		/// </summary>
		protected bool ForumPriorityAccess {
			get {
				if(pageinfo.IsNull("PriorityAccess"))
					return false;
				else
					return (bool)pageinfo["PriorityAccess"];
			}
		}
		/// <summary>
		/// True if the current user has access to create polls in the current forum.
		/// </summary>
		protected bool ForumPollAccess {
			get {
				if(pageinfo.IsNull("PollAccess"))
					return false;
				else
					return (bool)pageinfo["PollAccess"];
			}
		}
		/// <summary>
		/// True if the current user has access to vote on polls in the current forum
		/// </summary>
		protected bool ForumVoteAccess {
			get {
				if(pageinfo.IsNull("VoteAccess"))
					return false;
				else
					return (bool)pageinfo["VoteAccess"];
			}
		}
		/// <summary>
		/// True if the current user is a moderator of the current forum
		/// </summary>
		protected bool ForumModeratorAccess {
			get {
				if(pageinfo.IsNull("ModeratorAccess"))
					return false;
				else
					return (bool)pageinfo["ModeratorAccess"];
			}
		}
		/// <summary>
		/// True if the current user can delete own messages in the current forum
		/// </summary>
		protected bool ForumDeleteAccess {
			get {
				if(pageinfo.IsNull("DeleteAccess"))
					return false;
				else
					return (bool)pageinfo["DeleteAccess"];
			}
		}
		/// <summary>
		/// True if the current user can edit own messages in the current forum
		/// </summary>
		protected bool ForumEditAccess {
			get {
				if(pageinfo.IsNull("EditAccess"))
					return false;
				else
					return (bool)pageinfo["EditAccess"];
			}
		}
		/// <summary>
		/// True if the current user can upload attachments
		/// </summary>
		protected bool ForumUploadAccess 
		{
			get 
			{
				if(pageinfo.IsNull("UploadAccess"))
					return false;
				else
					return (bool)pageinfo["UploadAccess"];
			}
		}
		#endregion
		#region Date and time functions
		/// <summary>
		/// Returns the forum timezone offset from GMT
		/// </summary>
		public TimeSpan TimeZoneOffsetForum 
		{
			get {
				return new TimeSpan(1,0,0);
			}
		}
		/// <summary>
		/// Returns the user timezone offset from GMT
		/// </summary>
		public TimeSpan TimeZoneOffsetUser {
			get {
				if(pageinfo!=null) {
					int min = (int)pageinfo["TimeZone"];
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
				return TimeZoneOffsetUser - TimeZoneOffsetForum;
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateTime(object o) {
			DateTime dt = (DateTime)o;
			return String.Format(CustomCulture,"{0:F}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 7. februar 2003
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateLong(DateTime dt) {
			return String.Format(CustomCulture,"{0:D}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormatDateShort(DateTime dt) {
			return String.Format(CustomCulture,"{0:d}",dt + TimeOffset);
		}
		/// <summary>
		/// Formats a datetime value into 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatTime(DateTime dt) {
			return String.Format(CustomCulture,"{0:T}",dt + TimeOffset);
		}
		#endregion

	
		public void SendMail(string from,string to,string subject,string body) 
		{
#if true
			System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
			mailMessage.From = from;
			mailMessage.To = to;
			mailMessage.Subject = subject;
			mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
			mailMessage.Body = body;
			System.Web.Mail.SmtpMail.SmtpServer = SmtpServer;
			System.Web.Mail.SmtpMail.Send(mailMessage);
#else
			string sUserName = SmtpUserName;
			string sUserPass = SmtpUserPass;
			
			Smtp mail;
			if(sUserName!=null && sUserPass!=null)
				mail = new Smtp(SmtpServer,sUserName,sUserPass);
			else
				mail = new Smtp(SmtpServer);
			mail.SendMail(from,to,subject,body);
			mail.Quit();
#endif
		}
	}
}
