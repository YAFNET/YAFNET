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
		private bool		m_bNoDataBase	= false;
		private PageInfo	m_pageInfo		= new PageInfo();

		public new IForumUser User
		{
			get
			{
				return PageInfo.User;
			}
		}
		public PageInfo PageInfo
		{
			get
			{
				return m_pageInfo;
			}
		}
		public BaseAdminPage()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Error += new System.EventHandler(this.Page_Error);
		}
		private void Page_Error(object sender, System.EventArgs e) 
		{
			if(!Data.IsLocal) 
				Utils.LogToMail(Server.GetLastError());
		}
		private void Page_Load(object sender, System.EventArgs e) 
		{
			if(!m_bNoDataBase)
				m_pageInfo.PageLoad(true);
		}
		public string GetThemeContents(string page,string tag) 
		{
			return PageInfo.GetThemeContents(page,tag);
		}
		public string FormatDateTime(object o)
		{
			return PageInfo.FormatDateTime(o);
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteLine("<html>");
			writer.WriteLine("<!-- Copyright 2003 Bjørnar Henden -->");
			writer.WriteLine("<head>");
			writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}forum.css>",Data.ForumRoot));
			writer.WriteLine(String.Format("<link rel=stylesheet type=text/css href={0}>",PageInfo.ThemeFile("theme.css")));
			writer.WriteLine(String.Format("<title>{0}</title>",Config.BoardSettings.Name));
			writer.WriteLine("<script>");
			writer.WriteLine("function yaf_onload() {");
			if(PageInfo.LoadMessage.Length>0)
				writer.WriteLine(String.Format("	alert(\"{0}\");",PageInfo.LoadMessage));
			writer.WriteLine("}");
			writer.WriteLine("</script>");
			writer.WriteLine("</head>");
			writer.WriteLine("<body onload='yaf_onload()'>");
			
			RenderBody(writer);
			
			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
		}

		protected virtual void RenderBody(System.Web.UI.HtmlTextWriter writer) 
		{
			RenderBase(writer);
		}

		protected void RenderBase(System.Web.UI.HtmlTextWriter writer) 
		{
			base.Render(writer);
		}

		protected bool NoDataBase 
		{
			set 
			{
				m_bNoDataBase = value;
			}
		}
		public int PageBoardID
		{
			get
			{
				return ForumPage.PageBoardID;
			}
		}
		public bool IsHostAdmin
		{
			get
			{
				return PageInfo.IsHostAdmin;
			}
		}
		public bool IsAdmin
		{
			get
			{
				return PageInfo.IsAdmin;
			}
		}
		public void AddLoadMessage(string msg) 
		{
			PageInfo.AddLoadMessage(msg);
		}
	}
}
