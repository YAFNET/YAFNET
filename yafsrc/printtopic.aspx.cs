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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for printtopic.
	/// </summary>
	public class printtopic : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.WebControls.HyperLink ForumLink;
		protected System.Web.UI.WebControls.HyperLink TopicLink;
		protected System.Web.UI.WebControls.Repeater Posts;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["t"] == null || !ForumReadAccess)
				Response.Redirect(BaseDir);

			ShowToolBar = false;

			if(!IsPostBack) {
				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
				CategoryLink.Text = PageCategoryName;
				CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
				ForumLink.Text = PageForumName;
				ForumLink.NavigateUrl = String.Format("topics.aspx?f={0}",PageForumID);
				TopicLink.Text = PageTopicName;
				TopicLink.NavigateUrl = String.Format("posts.aspx?t={0}",PageTopicID);

				Posts.DataSource = DB.post_list(PageTopicID,PageUserID,1);
				DataBind();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		protected string GetPrintHeader(object o) {
			DataRowView row = (DataRowView)o;
			return String.Format("<b>{2}: {0}</b> - {1}",row["UserName"],FormatDateTime((DateTime)row["Posted"]),GetText("postedby"));
		}
		protected string GetPrintBody(object o) {
			DataRowView row = (DataRowView)o;
			FormatMsg fmt = new FormatMsg(this);
			return fmt.FormatMessage((string)row["Message"]);
		}
	}
}
