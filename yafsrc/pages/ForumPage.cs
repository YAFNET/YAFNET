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
using System.Xml;
using System.Web;
using yaf.classes;

// Grønn: #25C110
// Brown: #D0BF8C

namespace yaf.pages
{
	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class ForumPage : System.Web.UI.UserControl
	{
		#region Variables
		private HiPerfTimer	hiTimer				= new HiPerfTimer(true);
		private	PageInfo	m_pageInfo			= new PageInfo();
		private string		m_strRefreshURL		= null;
		private bool		m_bNoDataBase		= false;
		private bool		m_bShowToolBar		= true;
		private bool		m_checkSuspended	= true;
		#endregion
		#region Constructor and events
		/// <summary>
		/// Constructor
		/// </summary>
		public ForumPage(string transPage)
		{
			PageInfo.TransPage = transPage;

			this.Load += new System.EventHandler(this.ForumPage_Load);
			this.Error += new System.EventHandler(this.ForumPage_Error);
			this.PreRender += new EventHandler(ForumPage_PreRender);
		}

		private void ForumPage_Error(object sender, System.EventArgs e) 
		{
			// This doesn't seem to work...
			Exception x = Server.GetLastError();
			if(!Data.IsLocal) 
				Utils.LogToMail(Server.GetLastError());
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForumPage_Load(object sender, System.EventArgs e) 
		{
			if(!m_bNoDataBase)
				m_pageInfo.PageLoad(m_checkSuspended);
		}

		public DateTime GetForumRead(int forumID)
		{
			System.Collections.Hashtable t = Mession.ForumRead;
			if(t==null || !t.ContainsKey(forumID)) 
				return (DateTime)Mession.LastVisit;
			else
				return (DateTime)t[forumID];
		}
		public void SetForumRead(int forumID,DateTime date) 
		{
			System.Collections.Hashtable t = Mession.ForumRead;
			if(t==null) 
			{
				t = new System.Collections.Hashtable();
			}
			t[forumID] = date;
			Mession.ForumRead = t;
		}
		public DateTime GetTopicRead(int topicID)
		{
			System.Collections.Hashtable t = Mession.TopicRead;
			if(t==null || !t.ContainsKey(topicID)) 
				return (DateTime)Mession.LastVisit;
			else
				return (DateTime)t[topicID];
		}
		public void SetTopicRead(int topicID,DateTime date) 
		{
			System.Collections.Hashtable t = Mession.TopicRead;
			if(t==null) 
			{
				t = new System.Collections.Hashtable();
			}
			t[topicID] = date;
			Mession.TopicRead = t;
		}
		#endregion
		#region Theme Functions
		// XML THEME FILE (TEST)
		public string GetThemeContents(string page,string tag) 
		{
			return PageInfo.GetThemeContents(page,tag);
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
				ctl.InnerText = Config.BoardSettings.Name;

			/// BEGIN HEADER
			StringBuilder header = new StringBuilder();
			header.AppendFormat("<table width=100% cellspacing=0 class=content cellpadding=0><tr>");

			if(User.IsAuthenticated) 
			{
				header.AppendFormat(String.Format("<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>",String.Format(GetText("TOOLBAR","LOGGED_IN_AS"),PageUserName)));

				header.AppendFormat("<td style=\"padding:5px\" align=right valign=middle class=post>");
				header.AppendFormat(String.Format("	<a href=\"{0}\">{1}</a> | ",Forum.GetLink(Pages.search),GetText("TOOLBAR","SEARCH")));
				if(IsAdmin)
					header.AppendFormat(String.Format("	<a target='_top' href=\"{0}admin/\">{1}</a> | ",Data.ForumRoot,GetText("TOOLBAR","ADMIN")));
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
				writer.WriteLine("<link type='text/css' rel='stylesheet' href='{0}forum.css' />",Data.ForumRoot);
				writer.WriteLine("<link type='text/css' rel='stylesheet' href='{0}' />",ThemeFile("theme.css"));
				string script = "";
				if(PageInfo.LoadMessage.Length>0)
					script = String.Format("<script language='javascript'>\nonload=function(){1}\nalert(\"{0}\")\n{2}\n</script>\n",PageInfo.LoadMessage,'{','}');

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
					footer.AppendFormat("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under DotNetNuke.",Data.AppVersionName);
					footer.AppendFormat("<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.");
				} 
				else if(Config.IsRainbow)
				{
					footer.AppendFormat("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under Rainbow.",Data.AppVersionName);
					footer.AppendFormat("<br/>Copyright &copy; 2003 Yet Another Forum.net. All rights reserved.");
				}
				else 
				{
					footer.AppendFormat(GetText("COMMON","POWERED_BY"),
						String.Format("<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a>"),
						String.Format("{0} - {1}",Data.AppVersionName,FormatDateShort(Data.AppVersionDate))
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
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}forum.css>",Data.ForumRoot));
				writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}>",ThemeFile("theme.css")));
				writer.WriteLine(String.Format("<title>{0}</title>",Config.BoardSettings.Name));
				if(m_strRefreshURL!=null) 
					writer.WriteLine(String.Format("<meta HTTP-EQUIV=\"Refresh\" CONTENT=\"10;{0}\">",m_strRefreshURL));
				writer.WriteLine("</head>");
				writer.WriteLine("<body onload='yaf_onload()'>");
				
				RenderBody(writer);
				writer.WriteLine("<script>");
				writer.WriteLine("function yaf_onload() {");
				if(PageInfo.LoadMessage.Length>0)
					writer.WriteLine(String.Format("	alert(\"{0}\");",PageInfo.LoadMessage));
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
		#endregion
		#region Other
		/// <summary>
		/// Find the path of a smiley icon
		/// </summary>
		/// <param name="icon">The file name of the icon you want</param>
		/// <returns>The path to the image file</returns>
		public string Smiley(string icon) 
		{
			return String.Format("{0}images/emoticons/{1}",Data.ForumRoot,icon);
		}
		/// <summary>
		/// The directory of theme files
		/// </summary>
		protected string ThemeDir 
		{
			get 
			{
				return PageInfo.ThemeDir;
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
			PageInfo.AddLoadMessage(msg);
		}

		public string RefreshURL
		{
			set 
			{
				m_strRefreshURL = value;
			}
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
		#endregion

		public PageInfo PageInfo
		{
			get
			{
				if(m_pageInfo==null)
					throw new Exception("PageInfo isn't available.");
				return m_pageInfo;
			}
		}

		public int PageUserID
		{
			get
			{
				return PageInfo.PageUserID;
			}
		}
		public string PageUserName
		{
			get
			{
				return PageInfo.PageUserName;
			}
		}
		public int PageCategoryID
		{
			get
			{
				return PageInfo.PageCategoryID;
			}
		}
		public string PageCategoryName
		{
			get
			{
				return PageInfo.PageCategoryName;
			}
		}
		static public int PageBoardID
		{
			get
			{
				return PageInfo.PageBoardID;
			}
		}
		public bool ForumModeratorAccess
		{
			get
			{
				return PageInfo.ForumModeratorAccess;
			}
		}
		public int PageForumID
		{
			get
			{
				return PageInfo.PageForumID;
			}
		}
		public string PageForumName
		{
			get
			{
				return PageInfo.PageForumName;
			}
		}
		public int PageTopicID
		{
			get
			{
				return PageInfo.PageTopicID;
			}
		}
		public string PageTopicName
		{
			get
			{
				return PageInfo.PageTopicName;
			}
		}
		public bool ForumPriorityAccess
		{
			get
			{
				return PageInfo.ForumPriorityAccess;
			}
		}
		public bool ForumPollAccess
		{
			get
			{
				return PageInfo.ForumPollAccess;
			}
		}
		public bool ForumReplyAccess
		{
			get
			{
				return PageInfo.ForumReplyAccess;
			}
		}
		public bool ForumEditAccess
		{
			get
			{
				return PageInfo.ForumEditAccess;
			}
		}
		public bool ForumPostAccess
		{
			get
			{
				return PageInfo.ForumPostAccess;
			}
		}
		public bool ForumReadAccess
		{
			get
			{
				return PageInfo.ForumReadAccess;
			}
		}
		public bool ForumVoteAccess
		{
			get
			{
				return PageInfo.ForumVoteAccess;
			}
		}
		public bool ForumUploadAccess
		{
			get
			{
				return PageInfo.ForumUploadAccess;
			}
		}
		public bool ForumDeleteAccess
		{
			get
			{
				return PageInfo.ForumDeleteAccess;
			}
		}
		public bool IsAdmin
		{
			get
			{
				return PageInfo.IsAdmin;
			}
		}
		public bool IsGuest
		{
			get
			{
				return PageInfo.IsGuest;
			}
		}
		public bool IsForumModerator
		{
			get
			{
				return PageInfo.IsForumModerator;
			}
		}
		public bool IsModerator
		{
			get
			{
				return PageInfo.IsModerator;
			}
		}
		public string FormatDateTime(object o)
		{
			return PageInfo.FormatDateTime(o);
		}
		public string FormatTime(DateTime dt)
		{
			return PageInfo.FormatTime(dt);
		}
		public string FormatDateShort(object o)
		{
			return PageInfo.FormatDateShort(o);
		}
		public string FormatDateLong(DateTime dt) 
		{
			return PageInfo.FormatDateLong(dt);
		}
		public bool CheckSuspended
		{
			set
			{
				m_checkSuspended = value;
			}
		}
		public IForumUser User
		{
			get
			{
				return PageInfo.User;
			}
		}
		public string GetText(string text)
		{
			return PageInfo.GetText(text);
		}
		public string GetText(string page,string text)
		{
			return PageInfo.GetText(page,text);
		}
	}
}
